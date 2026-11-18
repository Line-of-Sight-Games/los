
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapCameraController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static MapCameraController Instance { get; private set; }

    [Header("References")]
    public Camera mapCamera;
    public Transform cameraRig;

    [Header("Zoom")]
    private float minZoomDistance = 5f;
    private float maxZoomDistance = 1000f;
    private float zoomSpeed = 100f;
    private float scrollSensitivity = 100f;
    private float zoomSmoothness = 10f;

    [Header("Rotation")]
    private float rotationSpeed = 2f;
    private float defaultPitch = 90f;

    [Header("Panning")]
    private float panDragThreshold = 5f;

    [Header("Battlefield Bounds")]
    private float edgeOverscroll = 100f;
    private float verticalOverscroll = 100f;

    [Header("Initial View")]
    private float initialViewPadding = 1.1f;
    private float defaultFieldOfView = 50f;

    [Header("Double Click")]
    private float doubleClickInterval = 0.3f;

    [Header("Object Focus")]
    private float objectFocusDistance = 15f;
    private float focusTransitionSpeed = 5f;
    private float focusZoomPadding = 2f;

    [Header("Object Clicking")]
    private LayerMask clickableLayers = ~0;
    private float maxRaycastDistance = 2000f;

    // Battlefield bounds
    private readonly float minX = 1f;
    private readonly float minY = 1f;
    private readonly float minZ = 0f;

    private float maxX;
    private float maxY;
    private float maxZ;

    // Camera state
    private Vector3 focusPoint;
    private float yaw;
    private float pitch;

    private float currentZoomDistance;
    private float targetZoomDistance;

    // Input state
    private bool initialised;
    private bool pointerInside;
    private bool isRotating;

    // Grab-and-drag state
    private bool isPanning;
    private bool hasDragged;
    private bool leftClickWasDrag;

    private Vector2 panStartPosition;
    private Vector2 previousMousePosition;

    private float lastLeftClickTime = -1f;

    // Object focus state
    private Vector3 targetFocusPoint;
    private bool isFocusing;
    private float focusTargetZoom;

    private RawImage rawImage;

    private RaycastHit[] raycastHits = new RaycastHit[32];

    private static readonly System.Collections.Generic.IComparer<RaycastHit>
        raycastHitComparer =
            System.Collections.Generic.Comparer<RaycastHit>.Create(
                CompareRaycastHits
            );

    private static int CompareRaycastHits(RaycastHit a, RaycastHit b)
    {
        return a.distance.CompareTo(b.distance);
    }


    // ---------------------------------------------------------
    // UNITY LIFECYCLE
    // ---------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple MapCameraControllers detected.");
            enabled = false;
            return;
        }

        Instance = this;

        rawImage = GetComponent<RawImage>();

        if (rawImage == null)
        {
            Debug.LogError(
                "MapCameraController must be attached to the MinimapDisplay RawImage.",
                this
            );

            enabled = false;
            return;
        }

        rawImage.raycastTarget = true;

        if (mapCamera == null)
        {
            Debug.LogError("MapCameraController: Map Camera is not assigned.", this);
            enabled = false;
            return;
        }

        if (cameraRig == null)
        {
            Debug.LogError("MapCameraController: Camera Rig is not assigned.", this);
            enabled = false;
            return;
        }

        mapCamera.orthographic = false;
        mapCamera.fieldOfView = defaultFieldOfView;
    }


    private void Update()
    {
        if (!initialised || !pointerInside)
            return;

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        HandleZoom(mouse);
        HandleRotation(mouse);
        HandlePanning(mouse);

        // Process left-click actions on release so we can distinguish
        // an actual click from a drag.
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            if (leftClickWasDrag)
            {
                // A drag should not count as a double-click.
                lastLeftClickTime = -1f;
            }
            else
            {
                if (!HandleDoubleClick())
                {
                    HandleObjectClick(mouse);
                }
            }

            leftClickWasDrag = false;
        }
    }


    private void LateUpdate()
    {
        if (!initialised)
            return;

        float smoothFactor = 1f - Mathf.Exp(
            -focusTransitionSpeed * Time.unscaledDeltaTime
        );

        if (isFocusing)
        {
            focusPoint = Vector3.Lerp(
                focusPoint,
                targetFocusPoint,
                smoothFactor
            );

            targetZoomDistance = Mathf.Lerp(
                targetZoomDistance,
                focusTargetZoom,
                smoothFactor
            );

            if (Vector3.Distance(focusPoint, targetFocusPoint) < 0.01f &&
                Mathf.Abs(targetZoomDistance - focusTargetZoom) < 0.01f)
            {
                focusPoint = targetFocusPoint;
                targetZoomDistance = focusTargetZoom;
                isFocusing = false;
            }
        }

        currentZoomDistance = Mathf.Lerp(
            currentZoomDistance,
            targetZoomDistance,
            1f - Mathf.Exp(-zoomSmoothness * Time.unscaledDeltaTime)
        );

        UpdateCameraPosition();
    }


    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    // ---------------------------------------------------------
    // INITIALIZATION
    // ---------------------------------------------------------

    public void Init(
        float battlefieldMaxX,
        float battlefieldMaxY,
        float battlefieldMaxZ)
    {
        maxX = battlefieldMaxX;
        maxY = battlefieldMaxY;
        maxZ = battlefieldMaxZ;

        if (mapCamera == null || cameraRig == null)
        {
            Debug.LogError("MapCameraController: Camera references are missing.");
            return;
        }

        mapCamera.orthographic = false;
        mapCamera.fieldOfView = defaultFieldOfView;

        focusPoint = GetBattlefieldCentre();

        yaw = 0f;
        pitch = defaultPitch;

        targetZoomDistance = Mathf.Clamp(
            CalculateFitDistance(),
            minZoomDistance,
            maxZoomDistance
        );

        currentZoomDistance = targetZoomDistance;

        initialised = true;

        ClampFocusPoint();
        UpdateCameraPosition();

        Debug.Log(
            $"Map camera initialised. " +
            $"Dimensions: ({maxX}, {maxY}, {maxZ}) | " +
            $"Rig: {cameraRig.position} | " +
            $"Camera: {mapCamera.transform.position} | " +
            $"Zoom: {currentZoomDistance}"
        );
    }


    private Vector3 GetBattlefieldCentre()
    {
        return new Vector3(
            maxX * 0.5f,
            0f,
            maxY * 0.5f
        );
    }


    private float CalculateFitDistance()
    {
        float battlefieldWidth = maxX - 1f;
        float battlefieldDepth = maxY - 1f;

        float verticalFov = mapCamera.fieldOfView * Mathf.Deg2Rad;
        float aspect = mapCamera.aspect;

        float horizontalFov = 2f * Mathf.Atan(
            Mathf.Tan(verticalFov * 0.5f) * aspect
        );

        float distanceForWidth =
            (battlefieldWidth * 0.5f) /
            Mathf.Tan(horizontalFov * 0.5f);

        float distanceForDepth =
            (battlefieldDepth * 0.5f) /
            Mathf.Tan(verticalFov * 0.5f);

        float fitDistance = Mathf.Max(
            distanceForWidth,
            distanceForDepth
        );

        return fitDistance * initialViewPadding;
    }


    // ---------------------------------------------------------
    // CAMERA POSITION
    // ---------------------------------------------------------

    private void UpdateCameraPosition()
    {
        if (!initialised || cameraRig == null || mapCamera == null)
            return;

        cameraRig.SetPositionAndRotation(
            focusPoint,
            Quaternion.Euler(pitch, yaw, 0f)
        );

        mapCamera.transform.localPosition = new Vector3(
            0f,
            0f,
            -currentZoomDistance
        );

        mapCamera.transform.localRotation = Quaternion.identity;
    }


    // ---------------------------------------------------------
    // ZOOM
    // ---------------------------------------------------------

    private void HandleZoom(Mouse mouse)
    {
        float scroll = mouse.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) < 0.01f)
            return;

        float zoomAmount =
            scroll * scrollSensitivity * zoomSpeed * 0.001f;

        targetZoomDistance -= zoomAmount;

        targetZoomDistance = Mathf.Clamp(
            targetZoomDistance,
            minZoomDistance,
            maxZoomDistance
        );
    }


    // ---------------------------------------------------------
    // ROTATION - RIGHT MOUSE BUTTON
    // ---------------------------------------------------------

    private void HandleRotation(Mouse mouse)
    {
        if (mouse.rightButton.wasPressedThisFrame)
            isRotating = true;

        if (mouse.rightButton.wasReleasedThisFrame)
            isRotating = false;

        if (!isRotating)
            return;

        Vector2 mouseDelta = mouse.delta.ReadValue();

        yaw += mouseDelta.x * rotationSpeed;

        pitch -= mouseDelta.y * rotationSpeed;

        pitch = Mathf.Clamp(pitch, -89f, 89f);
    }


    // ---------------------------------------------------------
    // GRAB AND DRAG PANNING - LEFT MOUSE BUTTON
    // ---------------------------------------------------------

    private void HandlePanning(Mouse mouse)
    {
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isPanning = true;
            hasDragged = false;

            panStartPosition = mouse.position.ReadValue();
            previousMousePosition = panStartPosition;
        }

        if (isPanning && mouse.leftButton.isPressed)
        {
            Vector2 currentMousePosition = mouse.position.ReadValue();

            if (!hasDragged)
            {
                float dragDistance = Vector2.Distance(
                    panStartPosition,
                    currentMousePosition
                );

                if (dragDistance >= panDragThreshold)
                {
                    hasDragged = true;

                    // Start from the original click position so the
                    // entire initial drag distance is accounted for.
                    previousMousePosition = panStartPosition;
                }
            }

            if (hasDragged)
            {
                PanCamera(
                    previousMousePosition,
                    currentMousePosition
                );

                previousMousePosition = currentMousePosition;
            }
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            leftClickWasDrag = hasDragged;

            isPanning = false;
            hasDragged = false;
        }
    }


    private void PanCamera(
        Vector2 previousScreenPosition,
        Vector2 currentScreenPosition)
    {
        // The battlefield is assumed to lie on the XZ plane at Y = 0.
        Plane battlefieldPlane = new Plane(
            Vector3.up,
            Vector3.zero
        );

        if (!TryGetBattlefieldPoint(
            previousScreenPosition,
            battlefieldPlane,
            out Vector3 previousWorldPoint))
        {
            return;
        }

        if (!TryGetBattlefieldPoint(
            currentScreenPosition,
            battlefieldPlane,
            out Vector3 currentWorldPoint))
        {
            return;
        }

        // Move the camera in the opposite direction to the mouse.
        // This keeps the grabbed world point underneath the cursor.
        Vector3 movement = previousWorldPoint - currentWorldPoint;

        focusPoint += movement;
    }


    private bool TryGetBattlefieldPoint(
        Vector2 screenPosition,
        Plane battlefieldPlane,
        out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;

        Canvas canvas = rawImage.canvas;

        Camera uiCamera =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImage.rectTransform,
            screenPosition,
            uiCamera,
            out Vector2 localPoint))
        {
            return false;
        }

        Rect rect = rawImage.rectTransform.rect;

        float u = Mathf.InverseLerp(
            rect.xMin,
            rect.xMax,
            localPoint.x
        );

        float v = Mathf.InverseLerp(
            rect.yMin,
            rect.yMax,
            localPoint.y
        );

        // Account for the RawImage UV rectangle.
        Rect uv = rawImage.uvRect;

        u = uv.x + u * uv.width;
        v = uv.y + v * uv.height;

        Ray ray = mapCamera.ViewportPointToRay(
            new Vector3(u, v, 0f)
        );

        if (battlefieldPlane.Raycast(ray, out float distance))
        {
            worldPoint = ray.GetPoint(distance);
            return true;
        }

        return false;
    }


    // ---------------------------------------------------------
    // DOUBLE CLICK - RESET VIEW
    // ---------------------------------------------------------

    private bool HandleDoubleClick()
    {
        float currentClickTime = Time.unscaledTime;

        if (lastLeftClickTime >= 0f &&
            currentClickTime - lastLeftClickTime <= doubleClickInterval)
        {
            ResetView();

            lastLeftClickTime = -1f;

            return true;
        }

        lastLeftClickTime = currentClickTime;

        return false;
    }


    public void ResetView()
    {
        focusPoint = GetBattlefieldCentre();

        yaw = 0f;
        pitch = defaultPitch;

        targetZoomDistance = Mathf.Clamp(
            CalculateFitDistance(),
            minZoomDistance,
            maxZoomDistance
        );

        currentZoomDistance = targetZoomDistance;

        isFocusing = false;

        ClampFocusPoint();
        UpdateCameraPosition();
    }


    // ---------------------------------------------------------
    // OBJECT CLICKING
    // ---------------------------------------------------------

    private void HandleObjectClick(Mouse mouse)
    {
        Vector2 mousePosition = mouse.position.ReadValue();

        Canvas canvas = rawImage.canvas;

        Camera uiCamera =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImage.rectTransform,
            mousePosition,
            uiCamera,
            out Vector2 localPoint))
        {
            return;
        }

        Rect rect = rawImage.rectTransform.rect;

        float u = Mathf.InverseLerp(
            rect.xMin,
            rect.xMax,
            localPoint.x
        );

        float v = Mathf.InverseLerp(
            rect.yMin,
            rect.yMax,
            localPoint.y
        );

        Rect uv = rawImage.uvRect;

        u = uv.x + u * uv.width;
        v = uv.y + v * uv.height;

        Ray ray = mapCamera.ViewportPointToRay(
            new Vector3(u, v, 0f)
        );

        int raycastMask = clickableLayers.value;

        int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");

        if (ignoreLayer >= 0)
        {
            raycastMask &= ~(1 << ignoreLayer);
        }

        int hitCount;

        while (true)
        {
            hitCount = Physics.RaycastNonAlloc(
                ray,
                raycastHits,
                maxRaycastDistance,
                raycastMask,
                QueryTriggerInteraction.Collide
            );

            if (hitCount < raycastHits.Length)
                break;

            System.Array.Resize(
                ref raycastHits,
                raycastHits.Length * 2
            );
        }

        System.Array.Sort(
            raycastHits,
            0,
            hitCount,
            raycastHitComparer
        );

        bool foundObject = false;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = raycastHits[i];

            PhysicalObject physicalObject =
                hit.collider.GetComponentInParent<PhysicalObject>();

            if (physicalObject == null)
                continue;

            if (!physicalObject.IsBodyCollider(hit.collider))
                continue;

            Debug.Log(
                $"TACTICAL MAP HIT: {physicalObject.name} | " +
                $"Collider: {hit.collider.name}"
            );

            FocusOnObject(hit.collider);

            // SOLDIER
            if (physicalObject is Soldier soldier)
            {
                TooltipPopup.Instance.Show(soldier);
                foundObject = true;
                break;
            }

            // POI
            if (physicalObject is POI poi)
            {
                TooltipPopup.Instance.Show(poi);
                foundObject = true;
                break;
            }

            // ITEM
            if (physicalObject is Item item)
            {
                TooltipPopup.Instance.Show(item);
                foundObject = true;
                break;
            }
        }

        if (!foundObject)
        {
            TooltipPopup.Instance?.Close();

            Debug.Log("Tactical map click: No selectable object found.");
        }
    }


    // ---------------------------------------------------------
    // FOCUS ON OBJECT
    // ---------------------------------------------------------

    public void FocusOnObject(Transform target)
    {
        if (target == null)
            return;

        targetFocusPoint = target.position;

        Renderer objectRenderer =
            target.GetComponentInChildren<Renderer>();

        if (objectRenderer != null)
        {
            Bounds bounds = objectRenderer.bounds;

            float objectSize = Mathf.Max(
                bounds.size.x,
                bounds.size.y,
                bounds.size.z
            );

            focusTargetZoom = Mathf.Max(
                objectFocusDistance,
                objectSize * focusZoomPadding
            );
        }
        else
        {
            focusTargetZoom = objectFocusDistance;
        }

        focusTargetZoom = Mathf.Clamp(
            focusTargetZoom,
            minZoomDistance,
            maxZoomDistance
        );

        isFocusing = true;
    }


    public void FocusOnObject(Collider targetCollider)
    {
        if (targetCollider == null)
            return;

        Bounds bounds = targetCollider.bounds;

        targetFocusPoint = bounds.center;

        float objectSize = Mathf.Max(
            bounds.size.x,
            bounds.size.y,
            bounds.size.z
        );

        focusTargetZoom = Mathf.Max(
            objectFocusDistance,
            objectSize * focusZoomPadding
        );

        focusTargetZoom = Mathf.Clamp(
            focusTargetZoom,
            minZoomDistance,
            maxZoomDistance
        );

        isFocusing = true;

        Debug.Log($"Focusing tactical camera on: {targetCollider.name}");
    }


    // ---------------------------------------------------------
    // BATTLEFIELD BOUNDS
    // ---------------------------------------------------------

    private void ClampFocusPoint()
    {
        focusPoint.x = Mathf.Clamp(
            focusPoint.x,
            minX - edgeOverscroll,
            maxX + edgeOverscroll
        );

        focusPoint.y = Mathf.Clamp(
            focusPoint.y,
            minZ - verticalOverscroll,
            maxZ + verticalOverscroll
        );

        focusPoint.z = Mathf.Clamp(
            focusPoint.z,
            minY - edgeOverscroll,
            maxY + edgeOverscroll
        );
    }


    // ---------------------------------------------------------
    // POINTER DETECTION
    // ---------------------------------------------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;

        isRotating = false;
        isPanning = false;
        hasDragged = false;

        leftClickWasDrag = false;
        lastLeftClickTime = -1f;
    }
}
