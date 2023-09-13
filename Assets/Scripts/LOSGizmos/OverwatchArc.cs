using UnityEngine;

public class OverwatchArc : LOSGizmo
{
    public int numberOfLines = 90; // Number of segments in the arc

    public OverwatchArc Init(Soldier from)
    {
        transform.position = from.transform.position;
        Vector3 direction = (new Vector3(from.overwatchXPoint, 0, from.overwatchYPoint) - from.transform.position).normalized;

        float angleStep = 45f / numberOfLines;

        for (int i = - numberOfLines; i < numberOfLines; i++)
        {
            float angle = i * angleStep;
            Vector3 lineStart = from.transform.position;
            Vector3 lineEnd = lineStart + Quaternion.Euler(0, angle, 0) * direction * from.overwatchConeRadius;

            LineRenderer lineRenderer = Instantiate(lineRendererPrefab, transform);
            lineRenderer.material.color = Color.yellow;
            lineRenderer.transform.localPosition = Vector3.zero;
            lineRenderer.SetPosition(0, lineStart);
            lineRenderer.SetPosition(1, lineEnd);
            lineRenderer.sortingOrder = 1;
        }

        return this;
    }
}