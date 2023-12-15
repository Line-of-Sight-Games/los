using UnityEngine;

public static class HelperFunctions
{
    public static bool IsWithinAngle(Vector3 fromA, Vector3 toA, Vector3 fromB, Vector3 toB, float angleThreshold)
    {
        Vector2 directionA = new Vector2(toA.x - fromA.x, toA.y - fromA.y).normalized;
        Vector2 directionB = new Vector2(toB.x - fromB.x, toB.y - fromB.y).normalized;


        float dotProduct = Vector3.Dot(directionA, directionB);

        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        Debug.Log(angle);
        Debug.Log(angle <= angleThreshold);
        return angle <= angleThreshold;
    }
}
