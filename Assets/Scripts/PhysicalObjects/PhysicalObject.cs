using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    public string objectName;
    public string id, terrainOn;
    public int x, y, z;
    public Collider bodyCollider;
    public MainGame game;

    private void Awake()
    {
        game = FindFirstObjectByType<MainGame>();
    }
    public string GenerateGuid()
    {
        return System.Guid.NewGuid().ToString();
    }
    public void MapPhysicalPosition(int x, int y, int z)
    {
        //print("Mapped physical position");
        transform.position = new Vector3(x - 0.5f, z, y - 0.5f);
    }
    public string Id
    {
        get { return id; }
        set { id = value; }
    }
    public Collider BodyCollider
    {
        get { return bodyCollider; }
    }
    public int X
    {
        get { return x; }
        set { x = value; MapPhysicalPosition(x, y, z); }
    }

    public int Y
    {
        get { return y; }
        set { y = value; MapPhysicalPosition(x, y, z); }
    }

    public int Z
    {
        get { return z; }
        set { z = value; MapPhysicalPosition(x, y, z); }
    }
    public string TerrainOn
    {
        get { return terrainOn; }
        set { terrainOn = value; }
    }
    public bool OnBattlefield()
    {
        if (X > 0 && X <= game.maxX && Y > 0 && Y <= game.maxY && Z <= game.maxZ)
            return true;
        return false;
    }
    public bool PhysicalObjectWithinRadius(PhysicalObject obj, float radius)
    {
        if (OnBattlefield() && obj.OnBattlefield() && game.CalculateRange(this, obj) <= radius)
            return true;

        return false;
    }
    // Function to check if any collider intersects with any sphere collider on the soldier
    public bool IsWithinSphere(Collider collider)
    {
        if (this.OnBattlefield())
        {
            if (collider is SphereCollider sphere)
            {
                //Get all colliders that are inside the sphere (transform sphere.center from local to global position)
                Collider[] hitColliders = Physics.OverlapSphere(sphere.transform.TransformPoint(sphere.center), sphere.radius);
                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider == this.BodyCollider)
                        return true;
                }
            }
        }
        return false;
    }
    public bool PointWithinRadius(Vector3 point, float radius)
    {
        if (OnBattlefield() && (point.x > 0 && X <= game.maxX && point.y > 0 && point.y <= game.maxY && point.z <= game.maxZ) && game.CalculateRange(this, point) <= radius)
            return true;

        return false;
    }
}
