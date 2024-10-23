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
        if (linkedInventoryObject != null)
        {
            foreach (Item i in itemList)
            {
                if (i.Id == linkedInventoryObject.InventorySlots[linkedInventoryObject.InventorySlots.FirstOrDefault(kvp => kvp.Key == slotName).Key])
                    return i;
            }
        }

        return null;
    }
    public void AddItemToSlot(Item item, string slotName)
    {
        if (!HasItem(item.id))
        {
            AddItem(item);
            item.whereEquipped = slotName;

            if (linkedInventoryObject != null && linkedInventoryObject.InventorySlots != null)
            {
                linkedInventoryObject.InventorySlots[slotName] = item.Id;
                if (item.IsNestedOnSoldier())
                    item.RunPickupEffect(item.SoldierNestedOn());
            }
        }
    }
    public void RemoveItemFromSlot(Item item, string slotName)
    {
        if (HasItem(item.id))
        {
            //run any drop effects
            if (item.IsNestedOnSoldier())
                item.RunDropEffect(item.SoldierNestedOn());

            RemoveItem(item);
            item.whereEquipped = "";
            if (linkedInventoryObject != null && linkedInventoryObject.InventorySlots != null)
            {
                //safe replacement to account for internal item swap glitch
                linkedInventoryObject.InventorySlots[slotName] = linkedInventoryObject.InventorySlots[slotName].Replace($"{item.Id}", "");
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
        itemIds.Add(item.Id);
        item.transform.SetParent(linkedInventoryObject.GameObject.transform, true);
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.owner = linkedInventoryObject;
    }
    public void RemoveItem(Item item)
    {
        //print("ran remove item");
        itemList.Remove(item);
        itemIds.Remove(item.Id);
        item.transform.SetParent(null, true);
        item.owner = null;
    }

    public bool HasItem(string id)
    {
        foreach (Item i in itemList)
        {
            if (i.Id == id)
                return true;
            else if (i.HasInventory())
            {
                foreach (Item i2 in i.Inventory.itemList)
                {
                    if (i2.Id == id)
                        return true;
                }
            }
        }

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
