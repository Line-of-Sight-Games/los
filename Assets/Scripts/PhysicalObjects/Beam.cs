using UnityEngine;

public class Beam : MonoBehaviour
{
    public MainGame game;
    public Vector3 startingPosition, targetPosition;
    public float beamHeight, beamWidth;

    public Renderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game = FindFirstObjectByType<MainGame>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPosition != null)
            SetBeam(startingPosition, targetPosition, beamHeight, beamWidth);
    }
    public Beam Init(Vector3 startingPosition, Vector3 targetPosition, float beamHeight, float beamWidth)
    {
        print($"{startingPosition} ---> {targetPosition}");
        this.startingPosition = startingPosition;
        this.targetPosition = targetPosition;
        this.beamHeight = beamHeight;
        this.beamWidth = beamWidth;

        return this;
    }
    public void SetBeam(Vector3 startingPosition, Vector3 targetPosition, float beamHeight, float beamWidth)
    {
        // Calculate direction vector based on facingCoordinates
        Vector3 origin = startingPosition;
        Vector3 direction = (targetPosition - origin).normalized;

        //find map endpoint extrapolated from targetPosition
        Vector3 boundaryPoint = CalculateBoundaryPoint(origin, direction);
        float distance = Vector3.Distance(origin, boundaryPoint);

        //set position and rotate to endpoint
        transform.SetPositionAndRotation(origin + (direction * (distance / 2)) + (Vector3.up * (beamHeight / 2)), Quaternion.LookRotation(direction));

        //scale beam to match the distance
        transform.localScale = new Vector3(beamWidth, beamHeight, distance);
    }

    public Vector3 CalculateBoundaryPoint(Vector3 start, Vector3 direction)
    {
        float tX = direction.x != 0 ? ((direction.x > 0 ? game.maxX : 1) - start.x) / direction.x : float.MaxValue;
        float tY = direction.y != 0 ? ((direction.y > 0 ? game.maxY : 1) - start.y) / direction.y : float.MaxValue;
        float tZ = direction.z != 0 ? ((direction.z > 0 ? game.maxZ : 0) - start.z) / direction.z : float.MaxValue;

        // Find the smallest positive t (time to reach boundary)
        float t = Mathf.Min(tX, tY, tZ);

        // Return the boundary point along the direction vector
        return start + direction * t;
    }
}
