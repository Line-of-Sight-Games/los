
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LOSArrow : LOSGizmo
{
    public PhysicalObject to;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        ConfigureLineRenderer();
    }

    private void ConfigureLineRenderer()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;

        // Ensure the line has a visible width.
        if (lineRenderer.startWidth <= 0f)
            lineRenderer.startWidth = 0.1f;

        if (lineRenderer.endWidth <= 0f)
            lineRenderer.endWidth = 0.1f;

        lineRenderer.sortingOrder = 1;
    }

    public LOSArrow Init(Soldier from, Soldier to)
    {
        if (from == null || to == null)
        {
            Debug.LogWarning("LOSArrow: Cannot initialise because a Soldier is null.");
            return this;
        }

        this.from = from;
        this.to = to;

        Vector3 heightOffset = Vector3.up * 1f;

        SetLinePositions(
            from.transform.position + heightOffset,
            to.transform.position + heightOffset
        );

        // Determine colour based on mutual visibility or team.
        bool fromCanSee = from.CanSeeInOwnRight(to);
        bool toCanSee = to.CanSeeInOwnRight(from);

        if (fromCanSee && toCanSee)
        {
            SetLineColour(Color.green);
        }
        else if (!fromCanSee && !toCanSee)
        {
            SetLineColour(Color.grey);
        }
        else if (fromCanSee)
        {
            SetLineColour(
                from.soldierTeam == 1 ? Color.red : Color.blue
            );
        }
        else
        {
            SetLineColour(
                to.soldierTeam == 1 ? Color.red : Color.blue
            );
        }

        return this;
    }

    public LOSArrow Init(Soldier from, POI to)
    {
        if (from == null || to == null)
        {
            Debug.LogWarning("LOSArrow: Cannot initialise because a Soldier or POI is null.");
            return this;
        }

        this.from = from;
        this.to = to;

        Vector3 heightOffset = Vector3.up * 1f;

        SetLinePositions(
            from.transform.position + heightOffset,
            to.transform.position + heightOffset
        );

        SetLineColour(
            from.soldierTeam == 1 ? Color.red : Color.blue
        );

        return this;
    }

    private void SetLinePositions(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        ConfigureLineRenderer();

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void SetLineColour(Color colour)
    {
        // Use the instantiated material so changing this LOSArrow
        // does not change the colour of every other LOSArrow.
        lineRenderer.material.color = colour;
    }
}
