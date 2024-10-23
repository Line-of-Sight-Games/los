using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySourceIconDrugCabinet : InventorySourceIcon
{
    public DrugCabinet linkedDC;

    public InventorySourceIcon Init(DrugCabinet dc, GameObject inventorySourcePanel)
    {
        linkedDC = dc;
        linkedInventoryPanel = inventorySourcePanel;
        transform.GetComponentInChildren<POIPortrait>().Init(dc);

        return this;
    }
}
