using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class Claymore : POI, IDataPersistence, IExplosive
{
    public POIManager poiManager;
    public bool triggered;
    public bool revealed;
    public int f, c, facingX, facingY;
    public string placedById;
    public Soldier placedBy;
    private void Start()
    {
        poiType = "claymore";
        menu = FindObjectOfType<MainMenu>();
        game = FindObjectOfType<MainGame>();
        poiManager = FindObjectOfType<POIManager>();
    }

    public Claymore Init(Tuple<Vector3, string> location, Tuple<int, int, int, int, string> otherDetails)
    {
        id = GenerateGuid();
        x = (int)location.Item1.x;
        y = (int)location.Item1.y;
        z = (int)location.Item1.z;
        terrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);

        f = otherDetails.Item1;
        c = otherDetails.Item2;
        facingX = otherDetails.Item3;
        facingY = otherDetails.Item4;
        placedById = otherDetails.Item5;
        placedBy = menu.soldierManager.FindSoldierById(placedById);

        return this;
    }
    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
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

            f = Convert.ToInt32(details["f"]);
            c = Convert.ToInt32(details["c"]);
            facingX = Convert.ToInt32(details["facingX"]);
            facingY = Convert.ToInt32(details["facingY"]);
            revealed = (bool)details["revealed"];
            placedById = (string)details["placedById"];
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
            { "terrainOn", terrainOn },
            { "f", f },
            { "c", c },
            { "facingX", facingX },
            { "facingY", facingY },
            { "revealed", revealed },
            { "placedById", placedById }
        };

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
    public void CheckClaymoreTriggeredBy(Soldier movingSoldier)
    {
        if (movingSoldier.IsFielded() && movingSoldier.PhysicalObjectWithinRadius(this, 3))
        {
            Vector2 centreLine = new(facingX - X, facingY - Y);
            Vector2 targetLine = new(movingSoldier.X - X, movingSoldier.Y - Y);
            centreLine.Normalize();
            targetLine.Normalize();

            if (Vector2.Angle(centreLine, targetLine) <= 30f)
                CheckExplosionClaymore(placedBy, false);
        }
    }
    public void CheckExplosionClaymore(Soldier explodedBy, bool exploded)
    {
        GameObject explosionList = Instantiate(menu.explosionListPrefab, menu.explosionUI.transform).GetComponent<ExplosionList>().Init($"Claymore : {this.X},{this.Y},{this.Z}").gameObject;
        float arc;
        if (exploded)
            arc = 360f;
        else
            arc = 60f;

        foreach (PhysicalObject obj in FindObjectsOfType<PhysicalObject>())
        {
            if (PhysicalObjectWithinClaymoreCone(obj, arc))
            {
                if (obj is Item hitItem)
                    menu.AddExplosionAlertItem(explosionList, hitItem, new(X, Y, Z), explodedBy, 10);
                else if (obj is POI hitPoi && hitPoi != this)
                    menu.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, 10);
                else if (obj is Soldier hitSoldier)
                    menu.AddExplosionAlert(explosionList, hitSoldier, new(X, Y, Z), explodedBy, hitSoldier.hp, 0);
            }
        }

        //if any exploded candidates
        if (explosionList.transform.childCount > 0)
            menu.OpenExplosionUI();
        
        poiManager.DestroyPOI(this);
    }
    public bool PhysicalObjectWithinClaymoreCone(PhysicalObject obj, float radius)
    {
        if (obj.PhysicalObjectWithinRadius(this, 3))
        {
            Vector2 centreLine = new(facingX - X, facingY - Y);
            Vector2 targetLine = new(obj.X - X, obj.Y - Y);
            centreLine.Normalize();
            targetLine.Normalize();

            if (Vector2.Angle(centreLine, targetLine) <= radius / 2.0f)
                return true;
        }
        return false;
    }
    public string Id 
    { 
        get { return id; } 
    }
}
