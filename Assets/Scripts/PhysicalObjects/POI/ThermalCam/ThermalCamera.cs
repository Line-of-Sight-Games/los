using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThermalCamera : POI, IDataPersistence, IAmDisarmable
{
    public string placedById;
    public Soldier placedBy;
    public int facingX, facingY;
    public bool active;
    public GameObject beam;

    private void Start()
    {
        poiType = "thermalcam";
        active = true;
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
        poiManager = FindFirstObjectByType<POIManager>();
    }

    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
        InitBeam(HelperFunctions.ConvertMathPosToPhysicalPos(new(facingX, facingY, Z)));
    }

    public ThermalCamera Init(Tuple<Vector3, string> location, Tuple<int, int, string> otherDetails)
    {
        id = GenerateGuid();
        x = (int)location.Item1.x;
        y = (int)location.Item1.y;
        z = (int)location.Item1.z;
        terrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);

        facingX = otherDetails.Item1;
        facingY = otherDetails.Item2;
        placedById = otherDetails.Item3;
        placedBy = menu.soldierManager.FindSoldierById(placedById);

        InitBeam(HelperFunctions.ConvertMathPosToPhysicalPos(new(facingX, facingY, Z)));

        poiPortrait = LoadPortrait(poiType);

        return this;
    }

    public override void LoadData(GameData data)
    {
        if (data.allPOIDetails.TryGetValue(id, out details))
        {
            poiType = (string)details["poiType"];
            poiPortrait = LoadPortrait(poiType);
            x = Convert.ToInt32(details["x"]);
            y = Convert.ToInt32(details["y"]);
            z = Convert.ToInt32(details["z"]);
            terrainOn = (string)details["terrainOn"];
            MapPhysicalPosition(x, y, z);

            active = (bool)details["active"];
            facingX = Convert.ToInt32(details["facingX"]);
            facingY = Convert.ToInt32(details["facingY"]);
            placedById = (string)details["placedById"];
        }
    }

    public override void SaveData(ref GameData data)
    {
        details = new()
        {
            { "poiType", poiType },
            { "x", x },
            { "y", y },
            { "z", z },
            { "terrainOn", terrainOn },
            { "active", active },
            { "facingX", facingX },
            { "facingY", facingY },
            { "placedById", placedById }
        };

        //add the poi in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }

    public void InitBeam(Vector3 targetPosition)
    {
        // Calculate direction vector based on facingCoordinates
        Vector3 origin = transform.position;
        Vector3 direction = (targetPosition - origin).normalized;

        //find map endpoint extrapolated from targetPosition
        Vector3 boundaryPoint = CalculateBoundaryPoint(origin, direction);
        float distance = Vector3.Distance(origin, boundaryPoint);

        //set position and rotate to endpoint
        beam.transform.SetPositionAndRotation(origin + direction * (distance / 2), Quaternion.LookRotation(direction));

        //scale beam to match the distance
        beam.transform.localScale = new Vector3(beam.transform.localScale.x, beam.transform.localScale.y, distance);
    }

    public Vector3 CalculateBoundaryPoint(Vector3 start, Vector3 direction)
    {
        float tX = direction.x != 0 ? ((direction.x > 0 ? game.maxX : 1) - start.x) / direction.x : float.MaxValue;
        float tY = direction.y != 0 ? ((direction.y > 0 ? game.maxY : 1) - start.y) / direction.y : float.MaxValue;
        float tZ = direction.z != 0 ? ((direction.z > 0 ? game.maxZ : 0) - start.z) / direction.z : float.MaxValue;

        // Find the smallest positive t (time to reach boundary)
        float t = Mathf.Min(tX, tY, tZ);

        // Return the boundary point along the direction vector
        return start + direction * t;
    }
    public Sprite DisarmImage { get { return poiPortrait; } }
}
