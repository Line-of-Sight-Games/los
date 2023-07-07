using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    private List<Item> itemList;
    public Item itemPrefab;
    public Soldier linkedSoldier;

    public Inventory(Soldier soldier)
    {
        itemList = new List<Item>();
        linkedSoldier = soldier;
    }

    public void AddItem(Item item)
    {
        //Debug.Log("ran add item");
        itemList.Add(item);
        item.transform.SetParent(linkedSoldier.gameObject.transform, true);
        item.transform.localPosition = new Vector3(0, 0, 0);
        item.owner = linkedSoldier;
    }

    public void RemoveItem(Item item)
    {
        //Debug.Log("ran remove item");
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

    public List<Item> Items 
    {
        get { return itemList; }
    }

    public string ListItems()
    {
        string items = "";
        foreach(Item i in Items)
        {
            items += i.itemName + "\n";
        }

        return items;
    }

    public List<string> ListItemIds()
    {
        List<string> itemIds = new();
        foreach (Item i in Items)
        {
            itemIds.Add(i.id);
        }

        return itemIds;
    }
    public bool HasArmourIntegrity()
    {
        if ((FindItem("Armour_Juggernaut") && GetItem("Armour_Juggernaut").ablativeHealth > 0) || (FindItem("Armour_Body") && GetItem("Armour_Body").ablativeHealth > 0))
            return true;
        else
            return false;
    }
    public bool IsWearingBodyArmour()
    {
        if (FindItem("Armour_Body"))
            return true;
        else
            return false;
    }
    public bool IsWearingJuggernautArmour()
    {
        if (FindItem("Armour_Juggernaut"))
            return true;
        else
            return false;
    }
    public bool IsWearingExoArmour()
    {
        if (FindItem("Armour_Exo"))
            return true;
        else
            return false;
    }
    public bool IsWearingGhillieArmour()
    {
        if (FindItem("Armour_Ghillie"))
            return true;
        else
            return false;
    }
    public bool IsWearingStimulantArmour()
    {
        if (FindItem("Armour_Stimulant"))
            return true;
        else
            return false;
    }
    public bool IsCarryingRiotShield()
    {
        if (FindItem("Riot_Shield"))
            return true;
        else
            return false;
    }
    public bool IsWearingLogisticsBelt()
    {
        if (FindItem("Logistics_Belt"))
            return true;
        else
            return false;
    }
}
