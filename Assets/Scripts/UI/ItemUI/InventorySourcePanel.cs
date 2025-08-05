using UnityEngine;

public class InventorySourcePanel : MonoBehaviour
{
    public IHaveInventory linkedInventorySource;
    public ItemSlot itemSlotPrefab;
    public ItemIcon itemIconPrefab;

    public InventorySourcePanel Init(IHaveInventory inventorySource)
    {
        linkedInventorySource = inventorySource;

        if (linkedInventorySource is Soldier linkedSoldier)
        {
            transform.Find("SoldierLoadout").GetComponent<InventoryDisplayPanelSoldier>().Init(linkedSoldier);
            transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(linkedSoldier);
        }
        else if (linkedInventorySource is GoodyBox linkedGoodyBox)
        {
            AddGBInventoryContents(linkedGoodyBox);
        }
        else if (linkedInventorySource is DrugCabinet linkedDrugCabinet)
        {
            AddGBInventoryContents(linkedDrugCabinet);
        }


        return this;
    }

    public void CloseInventorySourcePanel()
    {
        MenuManager.Instance.inventorySourceViewOnly = false; //locater ability
        MenuManager.Instance.CloseInventoryPanel(gameObject);
    }

    public void ButtonSoundTrigger()
    {
        SoundManager.Instance.PlayButtonPress();
    }

    public void AddGBInventoryContents(GoodyBox gb)
    {
        foreach (Item i in gb.Inventory.AllItems)
        {
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, transform.Find("Viewport").Find("Contents")).Init(gb);
            itemSlot.AssignItemIcon(Instantiate(itemIconPrefab, itemSlot.transform).GetComponent<ItemIcon>().Init(i, itemSlot));
        }
    }
    public void AddGBInventoryContents(DrugCabinet dc)
    {
        foreach (Item i in dc.Inventory.AllItems)
        {
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, transform.Find("Viewport").Find("Contents")).Init(dc);
            itemSlot.AssignItemIcon(Instantiate(itemIconPrefab, itemSlot.transform).GetComponent<ItemIcon>().Init(i, itemSlot));
        }
    }
}
