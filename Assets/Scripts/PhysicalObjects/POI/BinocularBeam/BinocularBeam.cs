using System;
using UnityEditor;
using UnityEngine;

public class BinocularBeam : POI, IDataPersistence
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
        poiManager = FindFirstObjectByType<POIManager>();
    }

    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
    }
    public BinocularBeam Init(Vector3 location, Tuple<int, int, string> otherDetails)
    {
        id = GenerateGuid();
        poiType = "binocularBeam";
        x = (int)location.x;
        y = (int)location.y;
        z = 0;
        MapPhysicalPosition(x, y, z);

        facingX = otherDetails.Item1;
        facingY = otherDetails.Item2;
        placedById = otherDetails.Item3;
        placedBy = menu.soldierManager.FindSoldierById(placedById);

        beamHeight = game.maxZ;
        beamWidth = GetBeamSize();
        beam.Init(transform.position, HelperFunctions.ConvertMathPosToPhysicalPos(new(facingX, facingY, 0)), beamHeight, beamWidth);

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
            MapPhysicalPosition(x, y, z);

            facingX = Convert.ToInt32(details["facingX"]);
            facingY = Convert.ToInt32(details["facingY"]);
            placedById = (string)details["placedById"];
            beamHeight = Convert.ToInt32(details["beamHeight"]);
            beamWidth = Convert.ToInt32(details["beamWidth"]);

            beam.Init(transform.position, HelperFunctions.ConvertMathPosToPhysicalPos(new(facingX, facingY, Z)), beamHeight, beamWidth);
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

        return 4f;
    }
}
