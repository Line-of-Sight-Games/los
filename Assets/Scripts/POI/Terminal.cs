using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : POI, IDataPersistence
{
    public string terminalType;
    public List<string> soldierAlreadyInteracted;
    public JArray soldierAlreadyInteractedJArray;

    private void Start()
    {
        poiType = "terminal";
    }

    public Terminal Init(Tuple<Vector3, string> location, string type)
    {
        id = GenerateGuid();
        x = (int)location.Item1.x;
        y = (int)location.Item1.y;
        z = (int)location.Item1.z;
        terrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);
        terminalType = type;

        return this;
    }

    public override void LoadData(GameData data)
    {
        if (data.allPOIDetails.TryGetValue(id, out details))
        {
            poiType = (string)details["poiType"];
            x = Convert.ToInt32(details["x"]);
            y = Convert.ToInt32(details["y"]);
            z = Convert.ToInt32(details["z"]);
            terrainOn = (string)details["terrainOn"];
            MapPhysicalPosition(x, y, z);

            terminalType = (string)details["terminalType"];

            //load list of soldier already interacted
            soldierAlreadyInteracted = new();
            soldierAlreadyInteractedJArray = (JArray)details["soldierAlreadyInteracted"];
            foreach (string soldierId in soldierAlreadyInteractedJArray)
                soldierAlreadyInteracted.Add(soldierId);
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
        details.Add("terminalType", terminalType);

        //save list of soldiers already interacted
        details.Add("soldierAlreadyInteracted", soldierAlreadyInteracted);

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }

    public List<string> SoldiersAlreadyInteracted
    {
        get { return soldierAlreadyInteracted; }
        set { soldierAlreadyInteracted = value; }
    }
}

