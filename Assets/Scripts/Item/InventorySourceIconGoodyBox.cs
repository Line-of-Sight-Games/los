using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIconGoodyBox : InventorySourceIcon
{
    public GoodyBox linkedGB;

    public InventorySourceIcon Init(GoodyBox gb, GameObject inventorySourcePanel)
    {
        linkedGB = gb;
        linkedInventoryPanel = inventorySourcePanel;
        transform.GetComponentInChildren<POIPortrait>().Init(gb);

        return this;
    }
}
