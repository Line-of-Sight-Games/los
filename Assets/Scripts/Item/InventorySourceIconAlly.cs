using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIconAlly : InventorySourceIcon
{
    public Soldier linkedSoldier;

    public InventorySourceIcon Init(Soldier s, GameObject inventorySourcePanel)
    {
            linkedSoldier = s;
            linkedInventoryPanel = inventorySourcePanel;
        return this;
    }
}
