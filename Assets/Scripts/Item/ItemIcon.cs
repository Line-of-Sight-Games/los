using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

public class ItemIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public MainMenu menu;
    public Item item; // The item associated with this icon
    public RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public ItemSlot originalSlot, currentSlot;
    public IHaveInventory originalInventoryObject;

    public ItemIcon Init(Item item)
    {
        this.item = item;
        gameObject.name = item.itemName;
        menu = FindObjectOfType<MainMenu>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalSlot = transform.parent.GetComponent<ItemSlot>();
        currentSlot = originalSlot;
        originalInventoryObject = originalSlot.linkedInventoryObject;
        item.markedForAction = string.Empty;
        transform.Find("ItemImage").GetComponent<Image>().sprite = FindObjectOfType<ItemAssets>().GetSprite(item.itemName);

        return this;
    }

    private void Update()
    {
        DisplayItemDetails();
    }

    public void DisplayItemDetails()
    {
        if (menu.activeSoldier.IsBull() && (item.IsGun() || item.IsAmmo()))
            transform.Find("ItemWeight").GetComponent<TextMeshProUGUI>().text = $"{1}";
        else
            transform.Find("ItemWeight").GetComponent<TextMeshProUGUI>().text = $"{item.weight}";
        if (item.IsGun() || item.IsAmmo())
        {
            transform.Find("Ammo").gameObject.SetActive(true);
            transform.Find("Ammo").GetComponent<TextMeshProUGUI>().text = $"{item.ammo}";
        }

        if (menu.overrideView)
        {
            if (item.IsGun() || item.IsAmmo())
            {
                transform.Find("OverrideAmmo").gameObject.SetActive(true);
                transform.Find("OverrideAmmo").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = $"{item.ammo}";
            }
        }
        else
            transform.Find("OverrideAmmo").gameObject.SetActive(false);
    }
    public void ChangeAmmo()
    {
        TMP_InputField ammoInput = transform.Find("OverrideAmmo").GetComponent<TMP_InputField>();

        if (int.TryParse(ammoInput.text, out int newAmmo))
            if (newAmmo >= 0)
                item.ammo = newAmmo;

        ammoInput.text = "";
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        GameObject targetDrop = eventData.pointerEnter;
        string originalInventoryId = "none", targetInventoryId = "none", originalSlotName = originalSlot.name, targetSlotName = "none";
        if (!menu.onItemUseScreen)
        {
            if (targetDrop != null)
            {
                ItemSlot targetSlot = targetDrop.GetComponent<ItemSlot>();
                InventorySourcePanel targetPanel = targetDrop.GetComponentInParent<InventorySourcePanel>();
                if (originalInventoryObject != null)
                    originalInventoryId = originalInventoryObject.Id;

                if (targetSlot != null)
                {
                    if (CheckValidSlot(targetSlot) && CheckBlockedSlotsAreFree(targetSlot))
                    {
                        SetCurrentSlot(targetSlot.AssignItemIcon(this));

                        targetSlotName = targetSlot.name;

                        InventoryDisplayPanelSoldier soldierInventory = targetSlot.GetComponentInParent<InventoryDisplayPanelSoldier>();
                        if (targetSlotName.Contains("Backpack"))
                            targetInventoryId = soldierInventory.GetItemInSlot("Back").Id;
                        else if (targetSlotName.Contains("LeftBrace"))
                        {
                            targetInventoryId = soldierInventory.GetItemInSlot("LeftLeg").Id;
                            targetSlotName = "Brace1";
                        } 
                        else if (targetSlotName.Contains("RightBrace"))
                        {
                            targetInventoryId = soldierInventory.GetItemInSlot("RightLeg").Id;
                            targetSlotName = "Brace1";
                        }
                        else if (targetSlotName.Contains("BArmour"))
                            targetInventoryId = soldierInventory.GetItemInSlot("Chest").Id;
                        else if (targetSlotName.Contains("JArmour"))
                            targetInventoryId = soldierInventory.GetItemInSlot("Head").Id;
                        else if (targetSlotName.Contains("Bag"))
                            targetInventoryId = soldierInventory.GetItemInSlot("Posterior").Id;
                        else
                            targetInventoryId = targetSlot.linkedInventoryObject.Id;
                        
                        if (targetSlot == originalSlot)
                            item.markedForAction = string.Empty;
                        else
                            item.markedForAction = $"{originalInventoryId}|{originalSlotName}|{targetInventoryId}|{targetSlotName}";
                    }
                    else
                        ReturnToOldSlot();
                }
                else if (targetPanel != null)
                {

                    if (targetPanel.linkedInventorySource == originalInventoryObject)
                    {
                        ReturnToOriginalSlot();

                        item.markedForAction = string.Empty;
                    }
                    else if (targetPanel.name.Contains("Ground"))
                    {
                        SetCurrentSlot(Instantiate(targetPanel.itemSlotPrefab, targetPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>().Init(targetPanel.linkedInventorySource).AssignItemIcon(this));

                        targetSlotName = transform.parent.name;
                        item.markedForAction = $"{originalInventoryId}|{originalSlotName}|{targetInventoryId}|{targetSlotName}";
                    }
                    else if (targetPanel.name.Contains("GB"))
                    {
                        SetCurrentSlot(Instantiate(targetPanel.itemSlotPrefab, targetPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>().Init(targetPanel.linkedInventorySource).AssignItemIcon(this));

                        targetInventoryId = targetPanel.linkedInventorySource.Id;
                        targetSlotName = transform.parent.name;
                        item.markedForAction = $"{originalInventoryId}|{originalSlotName}|{targetInventoryId}|{targetSlotName}";
                    }
                    else
                        ReturnToOldSlot();
                }
                else
                    ReturnToOldSlot();
            }
            else
                ReturnToOldSlot();

            menu.game.UpdateConfigureAP();
        }
        else
            ReturnToOldSlot();
    }
    public void UseItem()
    {
        if (menu.onItemUseScreen && !menu.overrideView)
        {
            if (item.IsUsable())
            {
                int ap = item.usageAP;
                //adept ability
                if (menu.activeSoldier.IsAdept() && item.usageAP > 1)
                    ap--;
                //gunner ability
                if (menu.activeSoldier.IsGunner() && item.IsAmmo())
                    ap = 1;

                if (menu.activeSoldier.game.CheckAP(ap))
                {
                    if (menu.activeSoldier.HandsFreeToUseItem(item))
                        menu.OpenUseItemUI(item, transform.parent.name, this, ap);
                }
            }
        }
    }
    public void ThrowItem()
    {
        if (menu.onItemUseScreen && !menu.overrideView)
        {
            int ap = 0;
            if (item.IsThrowable())
                ap++;
            else
            {
                if (!item.whereEquipped.Contains("Hand"))
                    ap++;
            }

            if (menu.activeSoldier.game.CheckAP(ap))
            {
                if (item.IsThrowable())
                {
                    if (menu.activeSoldier.HandsFreeToUseItem(item))
                        menu.OpenDropThrowItemUI(item, transform.parent.name, this, ap);
                }
                else
                    menu.OpenDropThrowItemUI(item, transform.parent.name, this, ap);
            }
        }
    }
    public bool CheckValidSlot(ItemSlot targetSlot)
    {
        foreach (string slot in item.equippableSlots)
            if (targetSlot.item == null && !targetSlot.unavailable && (targetSlot.name.Contains(slot) || targetSlot.name.Contains("ItemSlot")))
                return true;
        return false;
    }
    public bool CheckEmptySlot(ItemSlot targetSlot)
    {
        if (targetSlot.item == null)
            return true;
        return false;
    }
    /*public bool CheckInventorySlotsAreFree(GameObject targetDrop)
    {
        if (item.HasInventory())
        {
            InventorySourcePanel targetPanel = targetDrop.GetComponentInParent<InventorySourcePanel>();
            if (targetPanel is InventoryDisplayPanelSoldier inventoryDisplay)
            {
                if (inventoryDisplay != null)
                {
                    if (item.itemName == "Backpack")
                        if (!CheckEmptySlot(inventoryDisplay.transform.FindRecursively("Backpack1").GetComponent<ItemSlot>())
                            || !CheckEmptySlot(inventoryDisplay.transform.FindRecursively("Backpack2").GetComponent<ItemSlot>())
                            || !CheckEmptySlot(inventoryDisplay.transform.FindRecursively("Backpack3").GetComponent<ItemSlot>()))
                            return false;
                        else if (item.itemName == "Armour_Body" || item.itemName == "Armour_Juggernaut")
                            if (!CheckEmptySlot(inventoryDisplay.transform.FindRecursively("Armour1").GetComponent<ItemSlot>())
                                || !CheckEmptySlot(inventoryDisplay.transform.FindRecursively("Armour2").GetComponent<ItemSlot>())
                                || !CheckEmptySlot(inventoryDisplay.transform.FindRecursively("Armour3").GetComponent<ItemSlot>())
                                || !CheckEmptySlot(inventoryDisplay.transform.FindRecursively("Armour4").GetComponent<ItemSlot>()))
                                return false;
                            else if (item.itemName == "Brace" && currentSlot.name == "LeftLeg")
                                if (!CheckEmptySlot(inventoryDisplay.transform.FindRecursively("LeftBrace").GetComponent<ItemSlot>()))
                                    return false;
                                else if (item.itemName == "Brace" && currentSlot.name == "RightLeg")
                                    if (!CheckEmptySlot(inventoryDisplay.transform.FindRecursively("RightBrace").GetComponent<ItemSlot>()))
                                        return false;
                }
            }
        }
        return true;
    }*/
    public bool CheckBlockedSlotsAreFree(ItemSlot targetSlot)
    {
        InventoryDisplayPanelSoldier inventoryDisplay = targetSlot.GetComponentInParent<InventoryDisplayPanelSoldier>();
        if (inventoryDisplay != null)
            for (int i = 0; i < inventoryDisplay.blockedSlotMatrix.GetLength(0); i++)
                for (int j = 0; j < inventoryDisplay.blockedSlotMatrix.GetLength(1); j++)
                    if (inventoryDisplay.blockedSlotMatrix[i, 0] == item.itemName && inventoryDisplay.blockedSlotMatrix[0, j] == targetSlot.name)
                    {
                        List<string> slotNames = inventoryDisplay.blockedSlotMatrix[i, j].ToString().Split('|').ToList();
                        foreach (string slotName in slotNames)
                            if (slotName != "")
                                if (!CheckEmptySlot(inventoryDisplay.transform.FindRecursively(slotName).GetComponent<ItemSlot>()))
                                    return false; 
                    }
        return true;                
    }
    public void ReturnToOriginalSlot()
    {
        SetCurrentSlot(originalSlot);
        originalSlot.AssignItemIcon(this);
    }
    public void ReturnToOldSlot()
    {
        SetCurrentSlot(currentSlot);
        currentSlot.AssignItemIcon(this);
    }
    public void SetCurrentSlot(ItemSlot slot)
    {
        currentSlot.ClearItemIcon();
        currentSlot = slot;
    }
}
