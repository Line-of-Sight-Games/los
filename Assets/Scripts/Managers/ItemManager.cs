using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour, IDataPersistence
{
    public List<string> allItemIds = new();
    public List<Item> allItems = new();
    public Item itemPrefab;
    public ItemReader reader;
    public GameObject battlefield;

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
