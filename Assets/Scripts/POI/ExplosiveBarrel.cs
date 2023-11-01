using System;
using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : POI, IDataPersistence, IAmShootable
{
    public MainMenu menu;
    public MainGame game;
    public POIManager poiManager;
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

    public IEnumerator ExplosionCheck(Soldier explodedBy)
    {
        //imperceptible delay to allow colliders to be recalculated at new destination
        yield return new WaitUntil(() => menu.shotResolvedFlag == true);
        bool showExplosionUI = false;
        
        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsAlive())
            {
                if (s.PhysicalObjectWithinRadius(this, 3))
                {
                    menu.AddExplosionAlert(s, explodedBy, 8);
                    showExplosionUI = true;
                }
                else if (s.PhysicalObjectWithinRadius(this, 8))
                {
                    menu.AddExplosionAlert(s, explodedBy, 4);
                    showExplosionUI = true;
                }
                else if (s.PhysicalObjectWithinRadius(this, 15))
                {
                    menu.AddExplosionAlert(s, explodedBy, 2);
                    showExplosionUI = true;
                }
            }
        }

        if (showExplosionUI)
        {
            yield return new WaitUntil(() => menu.meleeResolvedFlag == true);
            menu.OpenExplosionUI();
            poiManager.DestroyPOI(this);
        }
    }
    public string Id 
    { 
        get { return id; } 
    }
}
