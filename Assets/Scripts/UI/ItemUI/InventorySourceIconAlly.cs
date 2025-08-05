using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIconAlly : InventorySourceIcon
{
    public Soldier linkedSoldier;

    public InventorySourceIcon Init(Soldier s, InventorySourcePanel inventorySourcePanel)
    {
        linkedSoldier = s;
        linkedInventoryPanel = inventorySourcePanel;
        transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(s);

        return this;
    }
}
