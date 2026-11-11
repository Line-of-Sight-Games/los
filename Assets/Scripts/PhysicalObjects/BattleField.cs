using UnityEngine;

public class Battlefield : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Color lineColor = Color.cyan;
    [SerializeField] private float lineWidth = 0.15f;

    private LineRenderer[] lines;

    public void Init(float maxX, float maxY, float maxZ)
    {
        Vector3[] worldCorners =
        {
            new (1,    0,    1),
            new (maxX, 0,    1),
            new (maxX, 0,    maxY),
            new (1,    0,    maxY),

            new (1,    maxZ, 1),
            new (maxX, maxZ, 1),
            new (maxX, maxZ, maxY),
            new (1,    maxZ, maxY)
        };

        Vector3[] localCorners = new Vector3[worldCorners.Length];

        for (int i = 0; i < worldCorners.Length; i++)
        {
            localCorners[i] = transform.InverseTransformPoint(worldCorners[i]);
        }

        int[,] edges =
        {
            {0,1},{1,2},{2,3},{3,0},
            {4,5},{5,6},{6,7},{7,4},
            {0,4},{1,5},{2,6},{3,7}
        };

        ClearLines();
        lines = new LineRenderer[12];

        for (int i = 0; i < 12; i++)
        {
            GameObject lineObject = new GameObject($"WireframeEdge_{i}");
            lineObject.transform.SetParent(transform, false);

            LineRenderer line = lineObject.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.useWorldSpace = false;

            line.SetPosition(0, localCorners[edges[i, 0]]);
            line.SetPosition(1, localCorners[edges[i, 1]]);

            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.startColor = lineColor;
            line.endColor = lineColor;
            line.material = lineMaterial;
            line.numCapVertices = 2;
            line.numCornerVertices = 2;
            line.alignment = LineAlignment.View;

            lines[i] = line;
        }
    }

    private void ClearLines()
    {
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("WireframeEdge_"))
                Destroy(child.gameObject);
        }

        lines = null;
    }
}
