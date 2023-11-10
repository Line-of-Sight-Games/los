using UnityEngine;

public class LOSArrow : LOSGizmo
{
    public Soldier to;

    public LOSArrow Init(Soldier from, Soldier to)
    {
        this.to = to;
        this.from = from;
        transform.position = new Vector3((from.transform.position.x + to.transform.position.x) / 2.0f, (from.transform.position.y + to.transform.position.y) / 2.0f, (from.transform.position.z + to.transform.position.z) / 2.0f);

        LineRenderer lineRenderer = Instantiate(lineRendererPrefab, transform);

        lineRenderer.SetPosition(0, new Vector3(from.transform.position.x, from.transform.position.y, from.transform.position.z));
        lineRenderer.SetPosition(1, new Vector3(to.transform.position.x, to.transform.position.y, to.transform.position.z));
        lineRenderer.sortingOrder = 1;

        //determine colour based on mutual or team
        if (from.CanSeeInOwnRight(to) && to.CanSeeInOwnRight(from))
            lineRenderer.material.color = Color.green;
        else if (!from.CanSeeInOwnRight(to) && !to.CanSeeInOwnRight(from))
            lineRenderer.material.color = Color.grey;
        else
        {
            if (from.soldierTeam == 1)
                lineRenderer.material.color = Color.red;
            else
                lineRenderer.material.color = Color.blue;
        }

        return this;
    }
}
