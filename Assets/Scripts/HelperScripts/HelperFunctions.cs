using UnityEngine;

public static class HelperFunctions
{
    public static bool IsWithinAngle(Vector3 pointA, Vector3 pointB, Vector3 centralPoint, float angleThreshold)
    {
        return CalculateAngle180(pointA, pointB, centralPoint) <= angleThreshold;
    }

    public static float CalculateAngle180(Vector3 pointA, Vector3 pointB, Vector3 centralPoint)
    {
        Vector2 directionA = new Vector2(pointA.x - centralPoint.x, pointA.y - centralPoint.y).normalized;
        Vector2 directionB = new Vector2(pointB.x - centralPoint.x, pointB.y - centralPoint.y).normalized;

        return Vector2.Angle(directionA, directionB);
    }

    public static float CalculateAngle360(Vector3 pointA, Vector3 pointB, Vector3 centralPoint)
    {
        Vector2 directionA = new Vector2(pointA.x - centralPoint.x, pointA.y - centralPoint.y).normalized;
        Vector2 directionB = new Vector2(pointB.x - centralPoint.x, pointB.y - centralPoint.y).normalized;

        float angle = Vector2.SignedAngle(directionA, directionB);

        if (angle < 0)
            angle += 360;

        return angle;
    }
}
