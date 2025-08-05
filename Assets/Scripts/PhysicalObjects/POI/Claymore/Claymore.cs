using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Claymore : POI, IDataPersistence, IExplosive, IAmDetectable, IAmDisarmable
{
    public bool triggered;
    public bool revealed;
    public bool exploded;
    public int c, facingX, facingY;
    public string placedById;
    public Soldier placedBy;
    public Renderer renderer;

    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
    }
    private void Update()
    {
        placedBy = menu.soldierManager.FindSoldierById(placedById);
        if (Exploded)
            POIManager.Instance.DestroyPOI(this);
    }
    public Claymore Init(Vector3 location, Tuple<int, int, int, bool, string> otherDetails)
    {
        id = GenerateGuid();
        poiType = "claymore";
        x = (int)location.x;
        y = (int)location.y;
        z = (int)location.z;
        MapPhysicalPosition(x, y, z);

        c = otherDetails.Item1;
        facingX = otherDetails.Item2;
        facingY = otherDetails.Item3;
        triggered = otherDetails.Item4;
        placedById = otherDetails.Item5;
        placedBy = menu.soldierManager.FindSoldierById(placedById);

        poiPortrait = LoadPortrait(poiType);

        return this;
    }
    public void LoadData(GameData data)
    {
        if (data.allPOIDetails.TryGetValue(id, out details))
        {
            //load position
            poiType = (string)details["poiType"];
            poiPortrait = LoadPortrait(poiType);
            x = Convert.ToInt32(details["x"]);
            y = Convert.ToInt32(details["y"]);
            z = Convert.ToInt32(details["z"]);
            terrainOn = (string)details["terrainOn"];
            MapPhysicalPosition(x, y, z);

            c = Convert.ToInt32(details["c"]);
            facingX = Convert.ToInt32(details["facingX"]);
            facingY = Convert.ToInt32(details["facingY"]);
            revealed = (bool)details["revealed"];
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
        if (triggeringSoldier.IsAlive())
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
    public void CheckExplosionClaymore(Soldier explodedBy, bool exploded)
    {
        //play explosion sfx
        game.soundManager.PlayExplosion();

        GameObject explosionList = Instantiate(menu.explosionListPrefab, menu.explosionUI.transform).GetComponent<ExplosionList>().Init($"Claymore : {X},{Y},{Z}", new(X, Y, Z)).gameObject;
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

    public bool Triggered 
    { get { return triggered; } set { triggered = value; } }
    public bool Exploded
    { get { return exploded; } set { exploded = value; } }
    public Sprite DisarmImage { get { return poiPortrait; } }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
