using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DrugCabinet : POI, IDataPersistence, IHaveInventory
{
    public Inventory inventory;
    public JArray itemsJArray;
    public List<string> inventoryList;

    public DrugCabinet Init(Tuple<Vector3, string> location)
    {
        Id = GenerateGuid();
        poiType = "drugcab";
        X = (int)location.Item1.x;
        Y = (int)location.Item1.y;
        Z = (int)location.Item1.z;
        TerrainOn = location.Item2;
        MapPhysicalPosition(x, y, z);
        inventory = new Inventory(this);

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

            //load items
            inventory = new Inventory(this);
            itemsJArray = (JArray)details["inventory"];
            foreach (string itemId in itemsJArray)
                inventoryList.Add(itemId);
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

            //save inventory
            { "inventory", Inventory.AllItemIds }
        };

        //add the item in
        if (data.allPOIDetails.ContainsKey(id))
            data.allPOIDetails.Remove(id);

        data.allPOIDetails.Add(id, details);
    }

    public void AddItemToDC(Item item)
    {
        inventory.AddItem(item);
    }

    public Inventory Inventory { get { return inventory; } }
    public GameObject GameObject { get { return gameObject; } }
    public List<string> InventoryList { get { return inventoryList; } }
    public Dictionary<string, string> InventorySlots { get { return null; } }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
