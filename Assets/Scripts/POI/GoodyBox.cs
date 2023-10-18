using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemReader;

public class GoodyBox : POI, IDataPersistence, IHaveInventory
{
    public Inventory inventory;
    public JArray itemsJArray;
    public List<string> inventoryList;


    private void Start()
    {
        poiType = "gb";
    }

    public GoodyBox Init(Tuple<Vector3, string> location)
    {
        id = GenerateGuid();
        x = (int)location.Item1.x;
        y = (int)location.Item1.y;
        z = (int)location.Item1.z;
        terrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);
        inventory = new Inventory(this);

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
            inventory = new Inventory(this);
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

    public void AddItemToGB(Item item)
    {
        inventory.AddItem(item);
    }

    public Inventory Inventory { get { return inventory; } }
    public GameObject GameObject { get { return gameObject; } }
}
