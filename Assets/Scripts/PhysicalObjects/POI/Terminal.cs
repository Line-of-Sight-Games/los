using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : POI, IDataPersistence
{
    public string terminalType;
    public bool terminalEnabled;
    public List<string> soldiersAlreadyNegotiated;
    public JArray soldiersAlreadyNegotiatedJArray;
    public List<string> soldiersAlreadyHacked;
    public JArray soldiersAlreadyHackedJArray;

    private void Start()
    {
        terminalEnabled = true;
    }

    public Terminal Init(Tuple<Vector3, string> location, string type)
    {
        id = GenerateGuid();
        poiType = "terminal";
        x = (int)location.Item1.x;
        y = (int)location.Item1.y;
        z = (int)location.Item1.z;
        terrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);
        terminalType = type;

        poiPortrait = LoadPortrait(poiType);

        return this;
    }

    public void LoadData(GameData data)
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

            terminalType = (string)details["terminalType"];
            terminalEnabled = (bool)details["terminalEnabled"];

            //load list of soldier already negotiated
            soldiersAlreadyNegotiated = new();
            soldiersAlreadyNegotiatedJArray = (JArray)details["soldiersAlreadyNegotiated"];
            foreach (string soldierId in soldiersAlreadyNegotiatedJArray)
                soldiersAlreadyNegotiated.Add(soldierId);

            //load list of soldier already hacked
            soldiersAlreadyHacked = new();
            soldiersAlreadyHackedJArray = (JArray)details["soldiersAlreadyHacked"];
            foreach (string soldierId in soldiersAlreadyHackedJArray)
                soldiersAlreadyHacked.Add(soldierId);
        }

        isDataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        details = new()
        {
            { "poiType", poiType },
            { "x", x },
            { "y", y },
            { "z", z },
            { "terrainOn", terrainOn },
            { "terminalType", terminalType },
            { "terminalEnabled", terminalEnabled },

            //save list of soldiers already interacted
            { "soldiersAlreadyNegotiated", soldiersAlreadyNegotiated },
            { "soldiersAlreadyHacked", soldiersAlreadyHacked }
        };

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }

    public List<string> SoldiersAlreadyNegotiated
    {
        get { return soldiersAlreadyNegotiated; }
        set { soldiersAlreadyNegotiated = value; }
    }
    public List<string> SoldiersAlreadyHacked
    {
        get { return soldiersAlreadyHacked; }
        set { soldiersAlreadyHacked = value; }
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}

