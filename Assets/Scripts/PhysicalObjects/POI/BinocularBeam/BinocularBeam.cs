using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class BinocularBeam : POI, IDataPersistence
{
    public string placedById;
    public Soldier placedBy;
    public int facingX, facingY;
    public float beamHeight, beamWidth;
    public Beam beam;
    public BinocularBeamTriggerCollider linkedCollider;
    public bool flashMode;
    public int turnsActive;

    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
    }
    private void Update()
    {
        placedBy = SoldierManager.Instance.FindSoldierById(placedById);
    }
    public BinocularBeam Init(Vector3 location, Tuple<int, int, int, string> otherDetails, string mode)
    {
        id = GenerateGuid();
        poiType = "binocularBeam";
        x = (int)location.x;
        y = (int)location.y;
        z = 0;
        MapPhysicalPosition(x, y, z);

        facingX = otherDetails.Item1;
        facingY = otherDetails.Item2;
        turnsActive = otherDetails.Item3;
        placedById = otherDetails.Item4;
        placedBy = SoldierManager.Instance.FindSoldierById(placedById);

        beamHeight = game.maxZ;
        beamWidth = GetBeamSize();
        beam.Init(transform.position, HelperFunctions.ConvertMathPosToPhysicalPos(new(facingX, facingY, 0)), beamHeight, beamWidth);

        //set flash mode
        if (mode.Equals("Flash"))
            flashMode = true;

        return this;
    }
    public void LoadData(GameData data)
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
            turnsActive = Convert.ToInt32(details["turnsActive"]);
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
            { "turnsActive", turnsActive },
            { "placedById", placedById },
            { "beamHeight", beamHeight },
            { "beamWidth", beamWidth },
        };

        //add the poi in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
    public IEnumerator DestroyBeam()
    {
        //squish beam to zero to trigger collider exits 
        beam.Init(Vector3.zero, Vector3.zero, 0, 0);
        beam.SetBeam(Vector3.zero, Vector3.zero, 0, 0);

        yield return new WaitForSeconds(0.01f);

        POIManager.Instance.DestroyPOI(this);
    }
    public float GetBeamSize()
    {
        if (placedBy.IsAdept()) //adept ability
            return 8f;

        return 4f;
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
