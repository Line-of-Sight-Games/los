using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabunCloud : POI, IDataPersistence
{
    public int turnsUntilDissipation;
    public List<string> alliesAffected, enemiesAffected;
    public string placedById;
    public Soldier placedBy;
    public SphereCollider innerCloud;
    public SphereCollider outerCloud;

    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
        poiManager = FindFirstObjectByType<POIManager>();
    }

    public TabunCloud Init(Tuple<Vector3, string> location, string thrownBy)
    {
        Id = GenerateGuid();
        poiType = "tabun";
        X = (int)location.Item1.x;
        Y = (int)location.Item1.y;
        Z = (int)location.Item1.z;
        TerrainOn = location.Item2;
        MapPhysicalPosition(X, Y, Z);

        turnsUntilDissipation = 3;
        placedById = thrownBy;
        placedBy = menu.soldierManager.FindSoldierById(placedById);

        SpawnCloud();

        return this;
    }
    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
    }
    public void LoadData(GameData data)
    {
        if (data.allPOIDetails.TryGetValue(id, out details))
        {
            //load position
            poiType = (string)details["poiType"];
            x = Convert.ToInt32(details["x"]);
            y = Convert.ToInt32(details["y"]);
            z = Convert.ToInt32(details["z"]);
            terrainOn = (string)details["terrainOn"];
            transform.position = new Vector3(x - 0.5f, z + 0.2f, y - 0.5f);

            turnsUntilDissipation = Convert.ToInt32(details["turnsUntilDissipation"]);
            alliesAffected = (details["alliesAffected"] as JArray).Select(token => token.ToString()).ToList();
            enemiesAffected = (details["enemiesAffected"] as JArray).Select(token => token.ToString()).ToList();
            placedById = (string)details["placedById"];
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
            { "turnsUntilDissipation", turnsUntilDissipation },
            { "alliesAffected", alliesAffected },
            { "enemiesAffected", enemiesAffected },
            { "placedById", placedById }
        };

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
    public void SpawnCloud()
    {
        game.CheckAllTabunClouds();
    }
    public void DissipateCloud()
    {
        //pay xp
        int xp = enemiesAffected.Count - alliesAffected.Count;
        if (xp > 0)
            menu.AddXpAlert(placedBy, xp, $"Tabun grenade affected {enemiesAffected.Count} enemies and {alliesAffected.Count} allies.", false);

        //show dissipation alert
        menu.CreateCloudDissipationAlert(this);

        //dissipate and recheck
        game.CheckAllTabunClouds();
    }
    public int TurnsUntilDissipation
    {
        get { return turnsUntilDissipation; }
        set
        {
            turnsUntilDissipation = value;
            if (turnsUntilDissipation == 0)
                DissipateCloud();
        }
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
