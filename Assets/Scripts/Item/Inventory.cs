using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [JsonIgnore] public Item itemPrefab;
    [JsonIgnore] public IHaveInventory linkedInventoryObject;
    private List<Item> itemList;
    private List<string> itemIds;

    public Inventory(IHaveInventory inventoryObject)
    {
        itemList = new List<Item>();
        itemIds = new List<string>();
        linkedInventoryObject = inventoryObject;
    }
    public Item GetItemInSlot(string slotName)
    {
        if (linkedInventoryObject is Soldier linkedSoldier)
            foreach (Item i in itemList)
                if (i.id == linkedSoldier.inventorySlots[linkedSoldier.inventorySlots.FirstOrDefault(kvp => kvp.Key == slotName).Key])
                    return i;
        return null;
    }
    public void AddItemToSlot(Item item, string slotName)
    {
        if (linkedInventoryObject is Soldier linkedSoldier)
        {
            AddItem(item);
            linkedSoldier.inventorySlots[slotName] = item.id;
            item.whereEquipped = slotName;
        }
    }
    public void RemoveItemFromSlot(Item item, string slotName)
    {
        if (linkedInventoryObject is Soldier linkedSoldier)
        {
            RemoveItem(item);
            if (linkedSoldier.inventorySlots[slotName] == item.id)
            {
                linkedSoldier.inventorySlots[slotName] = "";
                item.whereEquipped = "";
            }
        }
    }
    public void ConsumeItemInSlot(Item item, string slotName)
    {
        if (item != null)
        {
            RemoveItemFromSlot(item, slotName);
            item.itemManager.DestroyItem(item);
        }
    }
    public void AddItem(Item item)
    {
        itemList.Add(item);
        itemIds.Add(item.id);
        item.transform.SetParent(linkedInventoryObject.GameObject.transform, true);
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.owner = linkedInventoryObject;
    }
    public void RemoveItem(Item item)
    {
        //print("ran remove item");
        itemList.Remove(item);
        itemIds.Remove(item.id);
        item.transform.SetParent(null, true);
        item.owner = null;
    }

    public bool HasItem(string id)
    {
        foreach (Item i in itemList)
            if (i.id == id)
                return true;

        return false;
    }
    public bool HasItemOfType(string name)
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
