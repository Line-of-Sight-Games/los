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
        Tuple.Create(5, "40mm Mortar (tuned)", 4, 2, 4),
        Tuple.Create(7, "52mm Mortar (tuned)", 8, 3, 5),
        Tuple.Create(8, "52mm Mortar", 8, 2, 5),
        Tuple.Create(9, "81mm Mortar (tuned)", 12, 4, 6),
        Tuple.Create(11, "81mm Mortar", 12, 3, 6),
        Tuple.Create(13, "155mm Howitzer (tuned)", 18, 6, 8),
        Tuple.Create(14, "155mm Howitzer", 18, 4, 8),
        Tuple.Create(15, "240mm Howitzer", 24, 3, 10),
        Tuple.Create(17, "240mm Howitzer (tuned)", 24, 5, 10),
        Tuple.Create(19, "Hydra Airstrike", 30, 4, 14),
        Tuple.Create(20, "Hydra Airstrike (guided)", 30, 6, 14),
        Tuple.Create(23, "Hydra Airstrike (tuned)", 30, 5, 14),
        Tuple.Create(24, "Hydra Airstrike", 30, 4, 14),
        Tuple.Create(26, "Maverick Airstrike (guided)", 40, 8, 18),
        Tuple.Create(27, "Maverick Airstrike (tuned)", 40, 6, 18),
        Tuple.Create(29, "Maverick Airstrike", 40, 5, 18),
        Tuple.Create(31, "Maverick Airstrike (zeroed)", 40, 9, 18),
        Tuple.Create(34, "Hellfire Airstrike (tuned)", 50, 6, 24),
        Tuple.Create(35, "Hellfire Airstrike", 50, 5, 24),
        Tuple.Create(39, "Hellfire Airstrike (guided)", 50, 7, 24),
        Tuple.Create(41, "Hellfire Airstrike (tuned)", 50, 6, 24),
        Tuple.Create(44, "Sidewinder Airstrike (guided)", 60, 8, 34),
        Tuple.Create(47, "Sidewinder Airstrike (tuned)", 60, 7, 34),
        Tuple.Create(48, "Sidewinder Airstrike", 60, 6, 34),
        Tuple.Create(49, "Sidewinder Airstrike (zeroed)", 60, 9, 34),
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
        IEnumerable<Item> allItems = FindObjectsByType<Item>(default);
        foreach (Item item in allItems)
            if (item != null)
                Destroy(item.gameObject);

        allItemIds = data.allItemIds;
        foreach (string id in allItemIds)
        {
            Item newItem = Instantiate(itemPrefab);
            newItem.id = id;
            newItem.LoadData(data);
        }

        isDataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        allItemIds.Clear();
        data.allItemIds.Clear();

        IEnumerable<Item> allItems = FindObjectsByType<Item>(default);
        foreach (Item item in allItems)
            if (!allItemIds.Contains(item.id))
                allItemIds.Add(item.id);

        data.allItemIds = allItemIds;
    }

    public void AssignItemsToOwners()
    {
        Debug.Log("Assigning items to owners...");
        IEnumerable<Item> allItems = FindObjectsByType<Item>(default);
        IEnumerable<PhysicalObject> allPhysicalObjects = FindObjectsByType<PhysicalObject>(default);

        // Filter objects that implement IHaveInventory interface
        foreach (Item item in allItems)
        {
            foreach (PhysicalObject obj in allPhysicalObjects)
            {
                if (item.ownerId == obj.Id)
                    (obj as IHaveInventory).Inventory.AddItemToSlotFromSave(item, item.whereEquipped);
            }
        }
    }
    public Tuple<int, string, int, int, int> GetStrike(string strikeName)
    {
        foreach (Tuple<int, string, int, int, int> strike in strikeTable)
        {
            if (strike.Item2.Equals(strikeName))
                return strike;
        }
        return null;
    }
    public List<string> GetStrikeAndLowerNames(int score)
    {
        List<string> strikeOptions = new();
        for (int i = strikeTable.Length - 1; i >= 0; i--)
            if (strikeTable[i].Item1 <= score)
                strikeOptions.Add($"{strikeTable[i].Item2}");

        return strikeOptions;
    }
    public Item SpawnItem(string itemName)
    {
        Item item = Instantiate(itemPrefab).Init(itemName);

        //spawn small medikit inside brace
        if (item.IsBrace())
            item.Inventory.AddItemToSlot(SpawnItem("Medikit_Small"), "Medikit1");

        //spawn med medikit in bag
        if (item.IsBag())
            item.Inventory.AddItemToSlot(SpawnItem("Medikit_Medium"), "Medikit1");

        //spawn small & med medikit in backpack
        if (item.IsBackpack())
        {
            item.Inventory.AddItemToSlot(SpawnItem("Medikit_Medium"), "Medikit1");
            item.Inventory.AddItemToSlot(SpawnItem("Medikit_Small"), "Medikit2");
        }

        RefreshItemList();

        return item;
    }
    public void DestroyItem(Item item)
    {
        Destroy(item.gameObject);
        RefreshItemList();
    }
    public void RefreshItemList()
    {
        allItems = FindObjectsByType<Item>(default).ToList();
    }
    public Item FindItemById(string searchId)
    {
        foreach (Item i in allItems)
            if (i.id == searchId)
                return i;

        return null;
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
