using System;
using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : POI, IDataPersistence, IAmShootable, IExplosive
{
    public POIManager poiManager;
    public bool triggered;
    private void Start()
    {
        poiType = "barrel";
        menu = FindObjectOfType<MainMenu>();
        game = FindObjectOfType<MainGame>();
        poiManager = FindObjectOfType<POIManager>();
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
        details = new()
        {
            { "poiType", poiType },
            { "x", x },
            { "y", y },
            { "z", z },
            { "terrainOn", terrainOn }
        };

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }

    public void CheckExplosion(Soldier explodedBy, GameObject explosionList)
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsAlive())
            {
                if (s.PhysicalObjectWithinRadius(this, 3))
                    menu.AddExplosionAlert(explosionList, s, explodedBy, 8, true, true);
                else if (s.PhysicalObjectWithinRadius(this, 8))
                    menu.AddExplosionAlert(explosionList, s, explodedBy, 4, true, true);
                else if (s.PhysicalObjectWithinRadius(this, 15))
                    menu.AddExplosionAlert(explosionList, s, explodedBy, 2, true, true);
            }
        }
        foreach (POI poi in FindObjectsOfType<POI>())
        {
            if (poi != this && poi.PhysicalObjectWithinRadius(this, 15))
            {
                if (poi is ExplosiveBarrel barrel)
                {
                    if (!barrel.triggered)
                        menu.AddExplosionAlertPOI(explosionList, barrel, explodedBy);
                }
                else if (poi is Terminal terminal)
                    menu.AddExplosionAlertPOI(explosionList, terminal, explodedBy);
            }
        }

        //if any exploded candidates
        if (explosionList.transform.childCount > 0)
            menu.OpenExplosionUI();
        
        poiManager.DestroyPOI(this);
    }
    public string Id 
    { 
        get { return id; } 
    }
}
