using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : POI, IDataPersistence
{
    private void Start()
    {
        poiType = "barrel";
    }

    public ExplosiveBarrel Init(Tuple<Vector3, string> location)
    {
        id = GenerateGuid();
        x = (int)location.Item1.x;
        y = (int)location.Item1.y;
        z = (int)location.Item1.z;
        terrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);

        return this;
    }

    public override void LoadData(GameData data)
    {
        if (data.allPOIDetails.TryGetValue(id, out details))
        {
            //load position
            poiType = (string)details["poiType"];
            x = Convert.ToInt32(details["x"]);
            y = Convert.ToInt32(details["y"]);
            z = Convert.ToInt32(details["z"]);
            terrainOn = (string)details["terrainOn"];
            MapPhysicalPosition(x, y, z);
        }
    }

    public override void SaveData(ref GameData data)
    {
        details = new();

        details.Add("poiType", poiType);
        details.Add("x", x);
        details.Add("y", y);
        details.Add("z", z);
        details.Add("terrainOn", terrainOn);

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
}
