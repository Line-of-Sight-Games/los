using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIconGoodyBox : InventorySourceIcon
{
    public GoodyBox linkedGB;
    public GameObject locaterDisplay;
    public bool inventorySourceViewOnly;

    public InventorySourceIcon Init(GoodyBox gb, InventorySourcePanel inventorySourcePanel)
    {
        linkedGB = gb;
        linkedInventoryPanel = inventorySourcePanel;
        transform.GetComponentInChildren<POIPortrait>().Init(gb);

        return this;
    }

    public void SetLocated()
    {
        //locater ability
        locaterDisplay.SetActive(true);
        inventorySourceViewOnly = true;
    }

    public void OpenGBItemPanel()
    {
        if (locaterDisplay.activeSelf) //locater ability
        {
            if (MenuManager.Instance.OverrideKey())
            {
                MenuManager.Instance.inventorySourceViewOnly = this.inventorySourceViewOnly;
                OpenItemPanel();
            }
        }
        else
            OpenItemPanel();
    }
}
