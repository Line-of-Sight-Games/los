using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [JsonIgnore] public Item itemPrefab;
    [JsonIgnore] public Soldier linkedSoldier;
    private List<Item> itemList;
    private List<string> itemIds;

    public Inventory(Soldier soldier)
    {
        itemList = new List<Item>();
        itemIds = new List<string>();
        linkedSoldier = soldier;
    }

    public void AddItemToSlot(Item item, string slotName)
    {
        //save the item id in the appropriate slot
        linkedSoldier.inventorySlots[linkedSoldier.inventorySlots.FirstOrDefault(kvp => kvp.Key == slotName).Key] = item.id;

        itemList.Add(item);
        itemIds.Add(item.id);
        item.transform.SetParent(linkedSoldier.gameObject.transform, true);
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.owner = linkedSoldier;
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
        itemIds.Add(item.id);
        item.transform.SetParent(linkedSoldier.gameObject.transform, true);
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.owner = linkedSoldier;
    }

    public void RemoveItem(Item item)
    {
        //print("ran remove item");
        itemList.Remove(item);
        item.transform.SetParent(null, true);
        item.X = linkedSoldier.X;
        item.Y = linkedSoldier.Y;
        item.Z = linkedSoldier.Z;
        item.owner = null;
    }

    public bool FindItem(string name)
    {
        foreach (Item i in itemList)
            if (i.itemName == name)
                return true;

        return false;
    }

    public Item GetItem(string name)
    {
        foreach (Item i in itemList)
            if (i.itemName == name)
                return i;

        return null;
    }

    [JsonIgnore]
    public List<Item> AllItems 
    {
        get { return itemList; }
    }

    public List<string> AllItemIds
    {
        get { return itemIds; }
    }

    public string ListItems()
    {
        string items = "";
        foreach(Item i in AllItems)
        {
            items += i.itemName + "\n";
        }

        return items;
    }
}
