using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySourceIconDrugCabinet : InventorySourceIcon
{
    public DrugCabinet linkedDC;
    public GameObject locaterDisplay;
    public bool inventorySourceViewOnly;

    public InventorySourceIcon Init(DrugCabinet dc, InventorySourcePanel inventorySourcePanel)
    {
        menu = FindFirstObjectByType<MainMenu>();
        linkedDC = dc;
        linkedInventoryPanel = inventorySourcePanel;
        transform.GetComponentInChildren<POIPortrait>().Init(dc);

        return this;
    }
    public void SetLocated()
    {
        //locater ability
        locaterDisplay.SetActive(true);
        inventorySourceViewOnly = true;
    }

    public void OpenDCItemPanel()
    {
        if (locaterDisplay.activeSelf) //locater ability
        {
            if (menu.OverrideKey())
            {
                menu.inventorySourceViewOnly = this.inventorySourceViewOnly;
                OpenItemPanel();
            }
        }
        else
            OpenItemPanel();
    }
}
