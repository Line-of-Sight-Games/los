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
    public float beamHeight, beamWidth;
    public Beam beam;

    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
    }

    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
    }

    public ThermalCamera Init(Vector3 location, Tuple<int, int, string> otherDetails)
    {
        id = GenerateGuid();
        poiType = "thermalcam";
        x = (int)location.x;
        y = (int)location.y;
        z = (int)location.z;
        MapPhysicalPosition(x, y, z);

        facingX = otherDetails.Item1;
        facingY = otherDetails.Item2;
        placedById = otherDetails.Item3;
        placedBy = menu.soldierManager.FindSoldierById(placedById);
        
        beamHeight = GetBeamSize();
        beamWidth = GetBeamSize();
        beam.Init(transform.position, HelperFunctions.ConvertMathPosToPhysicalPos(new(facingX, facingY, Z)), beamHeight, beamWidth);

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
            MapPhysicalPosition(x, y, z);

            facingX = Convert.ToInt32(details["facingX"]);
            facingY = Convert.ToInt32(details["facingY"]);
            placedById = (string)details["placedById"];
            beamHeight = Convert.ToInt32(details["beamHeight"]);
            beamWidth = Convert.ToInt32(details["beamWidth"]);

            beam.Init(transform.position, HelperFunctions.ConvertMathPosToPhysicalPos(new(facingX, facingY, Z)), beamHeight, beamWidth);
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
            { "facingX", facingX },
            { "facingY", facingY },
            { "placedById", placedById },
            { "beamHeight", beamHeight },
            { "beamWidth", beamWidth },
        };

        //add the poi in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
    public float GetBeamSize()
    {
        if (placedBy.IsAdept()) //adept ability
            return 8f;

        return 1.5f;
    }

    public Sprite DisarmImage { get { return poiPortrait; } }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
