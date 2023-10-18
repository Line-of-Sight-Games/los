using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayPanelSoldier : MonoBehaviour
{
    public ItemIcon itemIconPrefab;
    public InventoryDisplayPanelSoldier Init(Soldier s)
    {
        GetItemInSlot(s, "Head");
        GetItemInSlot(s, "Chest");
        GetItemInSlot(s, "Back");
        GetItemInSlot(s, "Posterior");
        GetItemInSlot(s, "Lateral");
        GetItemInSlot(s, "LeftLeg");
        GetItemInSlot(s, "RightLeg");
        GetItemInSlot(s, "LeftHand");
        GetItemInSlot(s, "RightHand");
        GetItemInSlot(s, "LeftBrace");
        GetItemInSlot(s, "RightBrace");
        GetItemInSlot(s, "Backpack1");
        GetItemInSlot(s, "Backpack2");
        GetItemInSlot(s, "Backpack3");
        GetItemInSlot(s, "Armour1");
        GetItemInSlot(s, "Armour2");
        GetItemInSlot(s, "Armour3");
        GetItemInSlot(s, "Armour4");
        //GetItemInSlot(s, "Misc");

        return this;
    }
    

    public void GetItemInSlot(Soldier s, string slotName)
    {
        s.inventorySlots.TryGetValue(slotName, out string itemId);

        if (itemId != "")
            Instantiate(itemIconPrefab.Init(s.itemManager.FindItemById(itemId)), transform.Find(slotName));
    }
}
