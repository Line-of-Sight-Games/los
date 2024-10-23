using UnityEngine;

public class Explosion : POI
{
    public GameObject physicalExplosion;
    public Explosion Init(float radius, Vector3 position)
    {
        MapPhysicalPosition((int)position.x, (int)position.y, (int)position.z);
        ((SphereCollider)bodyCollider).radius = radius;
        physicalExplosion.transform.localScale *= (2*radius);

        return this;
    }
}
