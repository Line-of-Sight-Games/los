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
        IEnumerable<Soldier> allSoldiers = FindObjectsOfType<Soldier>();
        foreach (Soldier soldier in allSoldiers)
        {
            //Debug.Log("check soldier " + soldier.soldierName);
            foreach (string itemId in soldier.inventoryList)
            {
                //Debug.Log("soldier's itemlist " + itemId);
                foreach (Item item in allItems)
                {
                    //Debug.Log("item name " + item.itemName);
                    //Debug.Log("item id " + item.id);
                    if (item.id == itemId)
                    {
                        soldier.Inventory.AddItem(item);
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
        Destroy(item);
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
