using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SightRadiusCircle : LOSGizmo
{
    readonly int segments = 100;
    public SightRadiusCircle Init(Soldier from)
    {
        transform.position = from.transform.position;
        if (from.IsOnOverwatch())
            GenerateArcSectorMesh(from.overwatchConeRadius, from.overwatchConeArc, new (from.overwatchYPoint, 0f, from.overwatchXPoint));
        else
            GenerateCircleMesh(from.stats.SR.Val);

        return this;
    }

    void GenerateCircleMesh(int radius)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new();
        meshFilter.mesh = mesh;

        float angleStep = 2 * Mathf.PI / segments;

        Vector3[] vertices = new Vector3[segments + 1]; // +1 for the center vertex
        int[] triangles = new int[segments * 3];

        // Central vertex
        vertices[0] = Vector3.zero;

        // Create vertices in a circle
        for (int i = 1; i <= segments; i++)
        {
            float x = Mathf.Cos(angleStep * (i - 1)) * radius;
            float z = Mathf.Sin(angleStep * (i - 1)) * radius;
            vertices[i] = new Vector3(x, 0f, z);
        }

        // Create triangles
        for (int i = 0; i < segments - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Close the loop with the last triangle
        triangles[(segments - 1) * 3] = 0;
        triangles[(segments - 1) * 3 + 1] = segments;
        triangles[(segments - 1) * 3 + 2] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    void GenerateArcSectorMesh(int radius, int angle, Vector3 overwatchTarget)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new();
        meshFilter.mesh = mesh;

        float angleStep = Mathf.Deg2Rad * angle / segments;

        Vector3[] vertices = new Vector3[segments + 1]; // +1 for the center vertex
        int[] triangles = new int[segments * 3];

        // Central vertex
        vertices[0] = Vector3.zero;

        // Create vertices in a quarter circle
        for (int i = 1; i <= segments; i++)
        {
            float x = Mathf.Cos(angleStep * (i - 1)) * radius;
            float z = Mathf.Sin(angleStep * (i - 1)) * radius;
            vertices[i] = new Vector3(x, 0f, z);
        }

        // Create triangles for a quarter circle
        for (int i = 0; i < segments - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Close the loop with the last triangle
        triangles[(segments - 1) * 3] = 0;
        triangles[(segments - 1) * 3 + 1] = segments;
        triangles[(segments - 1) * 3 + 2] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Calculate the rotation angle based on the difference in degrees between the original position and overwatchTarget
        Vector3 directionToTarget = overwatchTarget - transform.position;
        float angleToRotate = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

        // Rotate the whole mesh about transform.position (central vertex) by the calculated angle
        transform.Rotate(Vector3.up, angleToRotate + (angle/2.0f));
    }
}
