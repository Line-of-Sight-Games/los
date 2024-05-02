using UnityEngine;

public class Coverman : PhysicalObject, IAmShootable
{
    private void Start()
    {
        id = "coverman";
    }
    public string Id
    {
        get { return id; }
    }

    public void SetCovermanLocation(Vector3 location)
    {
        X = (int)location.x;
        Y = (int)location.y;
        Z = (int)location.z;
    }
}
