using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void CheckExplosionBarrel(Soldier explodedBy, GameObject explosionList)
    {
        int damage = 0, stun = 1;
        foreach (PhysicalObject obj in FindObjectsOfType<PhysicalObject>())
        {
            if (obj.PhysicalObjectWithinRadius(this, 3))
                damage = 8;
            else if (obj.PhysicalObjectWithinRadius(this, 8))
                damage = 4;
            else if (obj.PhysicalObjectWithinRadius(this, 15))
                damage = 2;

            if (damage > 0)
            {
                if (obj is Item hitItem)
                    menu.AddExplosionAlertItem(explosionList, hitItem, explodedBy, damage);
                else if (obj is POI hitPoi && hitPoi != this)
                    menu.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, damage);
                else if (obj is Soldier hitSoldier)
                    menu.AddExplosionAlert(explosionList, hitSoldier, explodedBy, damage, stun); 
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
