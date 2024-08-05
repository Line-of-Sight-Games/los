using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Claymore : POI, IDataPersistence, IExplosive, IAmDetectable
{
    public bool triggered;
    public bool revealed;
    public bool exploded;
    public int f, c, facingX, facingY;
    public string placedById;
    public Soldier placedBy;
    private void Start()
    {
        poiType = "claymore";
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
        poiManager = FindFirstObjectByType<POIManager>();
    }
    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
        if (Exploded)
            poiManager.DestroyPOI(this);
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

        //add the poi in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }
    public bool CheckClaymoreTriggered(Soldier triggeringSoldier)
    {
        if (triggeringSoldier.IsFielded() && triggeringSoldier.PhysicalObjectWithinRadius(this, 3))
        {
            Vector2 centreLine = new(facingX - X, facingY - Y);
            Vector2 targetLine = new(triggeringSoldier.X - X, triggeringSoldier.Y - Y);
            centreLine.Normalize();
            targetLine.Normalize();

            if (Vector2.Angle(centreLine, targetLine) <= 30f)
            {
                triggered = true;
                return true;
            }
        }

        return false;
    }
    public void MoveOverClaymore(Soldier movingSoldier)
    {
        if (CheckClaymoreTriggered(movingSoldier))
            CheckExplosionClaymore(movingSoldier, false);
    }
    public void PlaceClaymore()
    {
        Soldier triggeringSoldier = null;
        foreach (Soldier s in game.AllSoldiers())
        {
            if (CheckClaymoreTriggered(s))
            {
                triggeringSoldier = s;
                break;
            }
        }
        if (triggered && triggeringSoldier != null)
            CheckExplosionClaymore(triggeringSoldier, false);
    }
    public void CheckExplosionClaymore(Soldier explodedBy, bool exploded)
    {
        GameObject explosionList = Instantiate(menu.explosionListPrefab, menu.explosionUI.transform).GetComponent<ExplosionList>().Init($"Claymore : {this.X},{this.Y},{this.Z}").gameObject;
        explosionList.transform.Find("ExplodedBy").GetComponent<TextMeshProUGUI>().text = explodedBy.id;

        float arc;
        if (exploded)
            arc = 360f;
        else
            arc = 60f;

        foreach (PhysicalObject obj in FindObjectsByType<PhysicalObject>(default))
        {
            if (PhysicalObjectWithinClaymoreCone(obj, arc))
            {
                if (obj is Item hitItem)
                    menu.AddExplosionAlertItem(explosionList, hitItem, new(X, Y, Z), explodedBy, 8);
                else if (obj is POI hitPoi && hitPoi != this)
                    menu.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, 8);
                else if (obj is Soldier hitSoldier)
                    menu.AddExplosionAlert(explosionList, hitSoldier, new(X, Y, Z), explodedBy, 8, 0);
            }
        }

        //show explosion ui
        menu.OpenExplosionUI();

        Exploded = true;
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
    public int ActiveC
    { get { return c; } }
    public int ActiveF
    { get { return f; } }

    public bool Triggered 
    { get { return triggered; } set { triggered = value; } }
    public bool Exploded
    { get { return exploded; } set { exploded = value; } }
}
