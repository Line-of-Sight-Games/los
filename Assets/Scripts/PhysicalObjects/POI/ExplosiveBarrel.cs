using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosiveBarrel : POI, IDataPersistence, IAmShootable, IExplosive
{
    public bool triggered;
    public bool exploded;
    private void Start()
    {
        game = FindFirstObjectByType<MainGame>();
    }

    public ExplosiveBarrel Init(Tuple<Vector3, string> location)
    {
        Id = GenerateGuid();
        poiType = "barrel";
        X = (int)location.Item1.x;
        Y = (int)location.Item1.y;
        Z = (int)location.Item1.z;
        TerrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);

        return this;
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
            MapPhysicalPosition(x, y, z);
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
            { "terrainOn", terrainOn }
        };

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }

    public void CheckExplosionBarrel(Soldier explodedBy)
    {
        //play explosion sfx
        SoundManager.Instance.PlayExplosion();

        GameObject explosionList = Instantiate(MenuManager.Instance.explosionListPrefab, MenuManager.Instance.explosionUI.transform).GetComponent<ExplosionList>().Init($"Explosive Barrel | Detonated: {X},{Y},{Z}", new(X, Y, Z)).gameObject;
        explosionList.transform.Find("ExplodedBy").GetComponent<TextMeshProUGUI>().text = explodedBy.id;

        //create explosion objects
        Explosion explosion1 = Instantiate(POIManager.Instance.explosionPrefab).Init(3, new(X, Y, Z));
        Explosion explosion2 = Instantiate(POIManager.Instance.explosionPrefab).Init(8, new(X, Y, Z));
        Explosion explosion3 = Instantiate(POIManager.Instance.explosionPrefab).Init(15, new(X, Y, Z));

        foreach (PhysicalObject obj in FindObjectsByType<PhysicalObject>(default))
        {
            int damage = 0;
            if (obj.IsWithinSphere(explosion1.BodyCollider))
                damage = 8;
            else if (obj.IsWithinSphere(explosion2.BodyCollider))
                damage = 4;
            else if (obj.IsWithinSphere(explosion3.BodyCollider))
                damage = 2;

            if (damage > 0)
            {
                if (obj is Item hitItem)
                    MenuManager.Instance.AddExplosionAlertItem(explosionList, hitItem, new(X, Y), explodedBy, damage);
                else if (obj is POI hitPoi && hitPoi != this)
                    MenuManager.Instance.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, damage);
                else if (obj is Soldier hitSoldier)
                    MenuManager.Instance.AddExplosionAlert(explosionList, hitSoldier, new(X, Y), explodedBy, damage, 1);
            }
        }

        //show explosion ui
        MenuManager.Instance.OpenExplosionUI();
        
        POIManager.Instance.DestroyPOI(this);
    }
    public bool Triggered
    { get { return triggered; } set { triggered = value; } }
    public bool Exploded
    { get { return exploded; } set { exploded = value; } }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
