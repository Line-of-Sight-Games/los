using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodyBox : POI, IDataPersistence, IHaveInventory
{
    public Inventory inventory;
    public JArray itemsJArray;
    public List<string> inventoryList;


    private void Start()
    {
        poiType = "gb";
    }

    public GoodyBox Init(Vector3 location, string terrain)
    {
        id = GenerateGuid();
        x = (int)location.x;
        y = (int)location.y;
        z = (int)location.z;
        terrainOn = terrain;
        MapPhysicalPosition(x, y, z);
        inventory = new Inventory(null);

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

            //load items
            inventory = new Inventory(null);
            itemsJArray = (JArray)details["inventory"];
            foreach (string itemId in itemsJArray)
                inventoryList.Add(itemId);
        }
    }

    public override void SaveData(ref GameData data)
    {
        details = new();

        details.Add("poiType", poiType);
        details.Add("x", x);
        details.Add("y", y);
        details.Add("z", z);
        details.Add("terrainOn", terrainOn);

        //save inventory
        details.Add("inventory", Inventory.AllItemIds);

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }

    public Inventory Inventory
    {
        get { return inventory; }
    } 
}
