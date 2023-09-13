using UnityEngine;

public class SightRadiusCircle : LOSGizmo
{
    public int numberOfLines = 720;
    public int lineWidth = 10;

    public SightRadiusCircle Init(Soldier from)
    {
        transform.position = from.transform.position;
        Vector3 direction = (new Vector3(0, 0, 0) - from.transform.position).normalized;

        float angleStep = 360f / numberOfLines;

        for (int i = 0; i < numberOfLines; i++)
        {
            float angle = i * angleStep;
            Vector3 origin = from.transform.position;
            Vector3 lineStart = origin + Quaternion.Euler(0, angle, 0) * direction * (from.SRColliderMax.radius - 0.1f);
            Vector3 lineEnd = origin + Quaternion.Euler(0, angle, 0) * direction * from.SRColliderMax.radius;

            LineRenderer lineRenderer = Instantiate(lineRendererPrefab, transform);
            lineRenderer.material.color = Color.yellow;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.transform.localPosition = Vector3.zero;
            lineRenderer.SetPosition(0, lineStart);
            lineRenderer.SetPosition(1, lineEnd);
            lineRenderer.sortingOrder = 1;
        }

        return this;
    }
}
