using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalCamera : POI, IDataPersistence, IAmDisarmable
{
    public string placedById;
    public Soldier placedBy;
    public int facingX, facingY;
    public bool active;

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
    public Sprite DisarmImage { get { return poiPortrait; } }
}
