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
        soundManager = FindFirstObjectByType<SoundManager>();
        menu = FindFirstObjectByType<MainMenu>();
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
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, transform.Find("Viewport").Find("Contents")).Init(gb);
            itemSlot.AssignItemIcon(Instantiate(itemIconPrefab, itemSlot.transform).GetComponent<ItemIcon>().Init(i));
        }
    }
    public void AddGBInventoryContents(DrugCabinet dc)
    {
        foreach (Item i in dc.Inventory.AllItems)
        {
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, transform.Find("Viewport").Find("Contents")).Init(dc);
            itemSlot.AssignItemIcon(Instantiate(itemIconPrefab, itemSlot.transform).GetComponent<ItemIcon>().Init(i));
        }
    }
}
