using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InventorySourcePanel : MonoBehaviour
{
    public MainMenu menu;
    public IHaveInventory linkedInventorySource;
    public SoundManager soundManager;
    public ItemSlot itemSlotPrefab;
    public ItemIcon itemIconPrefab;

    public InventorySourcePanel Init(IHaveInventory inventorySource)
    {
        soundManager = FindObjectOfType<SoundManager>();
        menu = FindObjectOfType<MainMenu>();
        linkedInventorySource = inventorySource;

        if (linkedInventorySource is Soldier linkedSoldier)
        {
            //do some things
        }
        else if (linkedInventorySource is GoodyBox linkedGoodyBox)
        {
            AddGBInventoryContents(linkedGoodyBox);
        }


        return this;
    }

    public void CloseInventorySourcePanel()
    {
        menu.CloseInventoryPanel(gameObject);
    }

    public void ButtonSoundTrigger()
    {
        soundManager.PlayButtonPress();
    }

    public void AddGBInventoryContents(GoodyBox gb)
    {
        foreach (Item i in gb.Inventory.AllItems)
        {
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, this.transform.Find("Viewport").Find("Contents"));
            itemSlot.AssignItemIcon(Instantiate(itemIconPrefab).Init(i));
        }
    }
}
