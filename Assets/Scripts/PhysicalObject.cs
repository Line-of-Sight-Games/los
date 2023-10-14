using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    public string id, terrainOn;
    public int x, y, z;

    public string GenerateGuid()
    {
        return System.Guid.NewGuid().ToString();
    }

    public void MapPhysicalPosition(int x, int y, int z)
    {
        //print("Mapped physical position");
        transform.position = new Vector3(x - 0.5f, z, y - 0.5f);
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
}
