using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentBeacon : POI, IDataPersistence, IAmDisarmable
{
    public string placedById;
    public Soldier placedBy;

    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
        poiManager = FindFirstObjectByType<POIManager>();
    }

    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
    }

    public DeploymentBeacon Init(Vector3 location, string placedBySoldierId)
    {
        Id = GenerateGuid();
        poiType = "depbeacon";
        X = (int)location.x;
        Y = (int)location.y;
        Z = (int)location.z;
        MapPhysicalPosition(x, y, z);
        placedById = placedBySoldierId;
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
            { "placedById", placedById }
        };

        //add the poi in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
    public Sprite DisarmImage { get { return poiPortrait; } }
}

