
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapCameraController : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Camera mapCamera;
    [SerializeField] private Transform cameraRig;

    [Header("Zoom")]
    [SerializeField] private float minZoomDistance = 5f;
    [SerializeField] private float maxZoomDistance = 250f;
    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float scrollSensitivity = 1f;
    [SerializeField] private float zoomSmoothness = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float minPitch = 20f;
    [SerializeField] private float maxPitch = 90f;
    [SerializeField] private float defaultPitch = 90f;

    [Header("Panning")]
    [SerializeField] private float panSpeed = 0.5f;
    [SerializeField] private float verticalPanSpeed = 0.5f;
    [SerializeField] private float mousePanSensitivity = 0.05f;

    [Header("Battlefield Bounds")]
    [SerializeField] private float edgeOverscroll = 10f;
    [SerializeField] private float verticalOverscroll = 2f;

    [Header("Initial View")]
    [SerializeField] private float initialViewPadding = 1.25f;
    [SerializeField] private float defaultFieldOfView = 50f;

    [Header("Double Click")]
    [SerializeField] private float doubleClickInterval = 0.3f;

    // Battlefield bounds
    private float minX = 1f;
    private float minY = 1f;
    private float minZ = 0f;

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
    private bool isPanning;

    private float lastLeftClickTime = -1f;

    private RawImage rawImage;


    private void Awake()
    {
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
        Keyboard keyboard = Keyboard.current;

        if (mouse == null)
            return;

        HandleDoubleClick(mouse);
        HandleZoom(mouse);
        HandleRotation(mouse);
        HandlePanning(mouse, keyboard);
    }


    private void LateUpdate()
    {
        if (!initialised)
            return;

        currentZoomDistance = Mathf.Lerp(
            currentZoomDistance,
            targetZoomDistance,
            1f - Mathf.Exp(-zoomSmoothness * Time.unscaledDeltaTime)
        );

        UpdateCameraPosition();
    }


    // ---------------------------------------------------------
    // INITIALIZATION
    // ---------------------------------------------------------

    public void Init(float battlefieldMaxX, float battlefieldMaxY, float battlefieldMaxZ)
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

        targetZoomDistance = CalculateFitDistance();

        targetZoomDistance = Mathf.Clamp(
            targetZoomDistance * initialViewPadding,
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
            (minX + maxX) * 0.5f,
            (minY + maxY) * 0.5f,
            (minZ + maxZ) * 0.5f
        );
    }


    private float CalculateFitDistance()
    {
        float width = maxX - minX;
        float depth = maxZ - minZ;
        float height = maxY - minY;

        float horizontalDiagonal = Mathf.Sqrt(
            width * width + depth * depth
        );

        float mapSize = Mathf.Sqrt(
            horizontalDiagonal * horizontalDiagonal +
            height * height
        );

        float halfFovRadians =
            mapCamera.fieldOfView * 0.5f * Mathf.Deg2Rad;

        float fitDistance =
            (mapSize * 0.5f) / Mathf.Tan(halfFovRadians);

        return fitDistance;
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

        // Normalise mouse wheel input. A typical wheel notch is 120.
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
    // ROTATION - MIDDLE MOUSE BUTTON
    // ---------------------------------------------------------

    private void HandleRotation(Mouse mouse)
    {
        if (mouse.middleButton.wasPressedThisFrame)
            isRotating = true;

        if (mouse.middleButton.wasReleasedThisFrame)
            isRotating = false;

        if (!isRotating)
            return;

        Vector2 mouseDelta = mouse.delta.ReadValue();

        yaw += mouseDelta.x * rotationSpeed;

        pitch -= mouseDelta.y * rotationSpeed;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );
    }


    // ---------------------------------------------------------
    // PANNING - RIGHT MOUSE BUTTON
    // ---------------------------------------------------------

    private void HandlePanning(Mouse mouse, Keyboard keyboard)
    {
        if (mouse.rightButton.wasPressedThisFrame)
            isPanning = true;

        if (mouse.rightButton.wasReleasedThisFrame)
            isPanning = false;

        if (!isPanning)
            return;

        Vector2 mouseDelta = mouse.delta.ReadValue();

        bool shiftHeld = keyboard != null &&
            (keyboard.leftShiftKey.isPressed ||
             keyboard.rightShiftKey.isPressed);

        // Shift + right drag moves the focus vertically.
        if (shiftHeld)
        {
            focusPoint.y -=
                mouseDelta.y *
                verticalPanSpeed *
                currentZoomDistance *
                mousePanSensitivity;
        }
        else
        {
            // Normal right drag pans across the battlefield.
            Vector3 right = Quaternion.Euler(0f, yaw, 0f) *
                            Vector3.right;

            Vector3 forward = Quaternion.Euler(0f, yaw, 0f) *
                              Vector3.forward;

            Vector3 movement =
                (-right * mouseDelta.x - forward * mouseDelta.y) *
                panSpeed *
                currentZoomDistance *
                mousePanSensitivity;

            focusPoint += movement;
        }

        ClampFocusPoint();
    }


    // ---------------------------------------------------------
    // DOUBLE CLICK - RESET VIEW
    // ---------------------------------------------------------

    private void HandleDoubleClick(Mouse mouse)
    {
        if (!mouse.leftButton.wasPressedThisFrame)
            return;

        float currentClickTime = Time.unscaledTime;

        if (lastLeftClickTime >= 0f &&
            currentClickTime - lastLeftClickTime <= doubleClickInterval)
        {
            ResetView();

            lastLeftClickTime = -1f;
        }
        else
        {
            lastLeftClickTime = currentClickTime;
        }
    }


    public void ResetView()
    {
        if (!initialised)
            return;

        focusPoint = GetBattlefieldCentre();

        yaw = 0f;
        pitch = defaultPitch;

        targetZoomDistance = Mathf.Clamp(
            CalculateFitDistance() * initialViewPadding,
            minZoomDistance,
            maxZoomDistance
        );

        currentZoomDistance = targetZoomDistance;

        ClampFocusPoint();
        UpdateCameraPosition();

        Debug.Log("Map viewer reset to default view.");
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
            minY - verticalOverscroll,
            maxY + verticalOverscroll
        );

        focusPoint.z = Mathf.Clamp(
            focusPoint.z,
            minZ - edgeOverscroll,
            maxZ + edgeOverscroll
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

        lastLeftClickTime = -1f;
    }
}