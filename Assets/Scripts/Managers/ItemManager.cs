using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemManager : MonoBehaviour, IDataPersistence
{
    public List<string> allItemIds = new();
    public List<Item> allItems = new();
    public Item itemPrefab;
    public ItemReader reader;
    public GameObject battlefield;
    public MainGame game;

    public int[,] scoreTable = new int[,]
    {
        {0,1,2,3,4,5,6,7,8,9},
        {1,3,5,7,9,11,13,15,17,19},
        {2,5,8,11,14,17,20,23,26,29},
        {3,7,11,15,19,23,27,31,35,39},
        {4,9,14,19,24,29,34,39,44,49},
        {5,11,17,23,29,35,41,47,53,59},
        {6,13,20,27,34,41,48,55,62,69},
        {7,15,23,31,39,47,55,63,71,79},
        {8,17,26,35,44,53,62,71,80,89},
        {9,19,29,39,49,59,69,79,89,99},
    };
    public Tuple<int, string, int, int, int>[] strikeTable = new Tuple<int, string, int, int, int>[]
    {
        Tuple.Create(3, "40mm Mortar", 4, 1, 4),
        Tuple.Create(5, "40mm Mortar", 4, 2, 4),
        Tuple.Create(7, "52mm Mortar", 8, 3, 5),
        Tuple.Create(8, "52mm Mortar", 8, 2, 5),
        Tuple.Create(9, "81mm Mortar", 12, 4, 6),
        Tuple.Create(11, "81mm Mortar", 12, 3, 6),
        Tuple.Create(13, "155mm Howitzer", 18, 6, 8),
        Tuple.Create(14, "155mm Howitzer", 18, 4, 8),
        Tuple.Create(15, "240mm Howitzer", 24, 3, 10),
        Tuple.Create(17, "240mm Howitzer", 24, 5, 10),
        Tuple.Create(19, "Hydra Airstrike", 30, 4, 14),
        Tuple.Create(20, "Hydra Airstrike", 30, 6, 14),
        Tuple.Create(23, "Hydra Airstrike", 30, 5, 14),
        Tuple.Create(24, "Hydra Airstrike", 30, 4, 14),
        Tuple.Create(26, "Maverick Airstrike", 40, 8, 18),
        Tuple.Create(27, "Maverick Airstrike", 40, 6, 18),
        Tuple.Create(29, "Maverick Airstrike", 40, 5, 18),
        Tuple.Create(31, "Maverick Airstrike", 40, 9, 18),
        Tuple.Create(34, "Hellfire Airstrike", 50, 6, 24),
        Tuple.Create(35, "Hellfire Airstrike", 50, 5, 24),
        Tuple.Create(39, "Hellfire Airstrike", 50, 7, 24),
        Tuple.Create(41, "Hellfire Airstrike", 50, 6, 24),
        Tuple.Create(44, "Sidewinder Airstrike", 60, 8, 34),
        Tuple.Create(47, "Sidewinder Airstrike", 60, 7, 34),
        Tuple.Create(48, "Sidewinder Airstrike", 60, 6, 34),
        Tuple.Create(49, "Sidewinder Airstrike", 60, 9, 34),
    };
    public string[] drugTable = new string[]
    {
        "Amphetamine",
        "Androstenedione",
        "Cannabinoid",
        "Danazol",
        "Glucocorticoid",
        "Modafinil",
        "Shard",
        "Trenbolone",
    };
    public void LoadData(GameData data)
    {
        //destroy items ready to be regenerated
        IEnumerable<Item> allItems = FindObjectsOfType<Item>();
        foreach (Item item in allItems)
            if (item != null)
            Destroy(item.gameObject);

        allItemIds = data.allItemIds;
        foreach (string id in allItemIds)
        {
            var newItem = Instantiate(itemPrefab);
            newItem.id = id;
            newItem.LoadData(data);
        }

        AssignItemsToOwners();
    }

    public void SaveData(ref GameData data)
    {
        allItemIds.Clear();
        data.allItemIds.Clear();

        IEnumerable<Item> allItems = FindObjectsOfType<Item>();
        foreach (Item item in allItems)
            if (!allItemIds.Contains(item.id))
                allItemIds.Add(item.id);

        data.allItemIds = allItemIds;
    }

    public void AssignItemsToOwners()
    {
        IEnumerable<Item> allItems = FindObjectsOfType<Item>();
        IEnumerable<MonoBehaviour> allObjects = FindObjectsOfType<MonoBehaviour>();

        // Filter objects that implement IHaveInventory interface
        foreach (MonoBehaviour obj in allObjects)
        {
            if (obj is IHaveInventory inventoryObject)
            {
                foreach (string itemId in inventoryObject.InventoryList)
                {
                    foreach (Item item in allItems)
                    {
                        if (item.id == itemId)
                        {
                            if (inventoryObject is Soldier inventorySoldier)
                                inventorySoldier.AssignItemToSlot(item);
                            else
                                inventoryObject.Inventory.AddItem(item);
                        }
                    }
                }
            }
        }
    }
    public Tuple<int, string, int, int, int> GetStrike(int score)
    {
        for (int i = strikeTable.Length - 1; i >= 0; i--)
            if (strikeTable[i].Item1 <= score)
                return strikeTable[i];
        return null;
    }
    public Item SpawnItem(string itemName)
    {
        var item = Instantiate(itemPrefab).Init(itemName);
        RefreshItemList();

        return item;
    }
    public void DestroyItem(Item item)
    {
        Destroy(item.gameObject);
        RefreshItemList();
    }
    public void DestroyBreakableItem(Soldier destroyedBy, Item item)
    {
        if (item.IsBreakable())
        {
            if (item.IsGrenade())
                game.CheckExplosionGrenade(item, destroyedBy, new(item.X, item.Y, item.Z));
            else
                DestroyItem(item);
        }
    }
    public void RefreshItemList()
    {
        allItems = FindObjectsOfType<Item>().ToList();
    }
    public Item FindItemById(string searchId)
    {
        foreach (Item i in allItems)
            if (i.id == searchId)
                return i;

        return null;
    }
}
