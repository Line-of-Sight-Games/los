using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAlert : MonoBehaviour
{
    public Soldier explodedBy;
    public PhysicalObject hitByExplosion;
    public void SetObjects(Soldier explodedBy, PhysicalObject hitByExplosion)
    {
        this.explodedBy = explodedBy;
        this.hitByExplosion = hitByExplosion;
    }

    private void Update()
    {
        if (hitByExplosion == null)
            Destroy(gameObject);
    }
}
