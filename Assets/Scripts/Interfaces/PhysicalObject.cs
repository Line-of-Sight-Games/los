using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    public string objectName;
    public string id, terrainOn;
    public int x, y, z;
    public MainGame game;
    public MainMenu menu;

    private void Awake()
    {
        game = FindObjectOfType<MainGame>();
        menu = FindObjectOfType<MainMenu>();
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
    public bool PhysicalObjectWithinRadius(Vector3 point, float radius)
    {
        if (OnBattlefield() && (point.x > 0 && X <= game.maxX && point.y > 0 && point.y <= game.maxY && point.z <= game.maxZ) && game.CalculateRange(this, point) <= radius)
            return true;

        return false;
    }
    public string TerrainOn
    {
        get { return terrainOn; }
        set { terrainOn = value; }
    }
}
