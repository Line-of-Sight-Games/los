using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Diagnostics.Contracts;
using System;

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

        return this;
    }
    private void Update()
    {
        transform.Find("ItemImage").GetComponent<Image>().sprite = FindObjectOfType<ItemAssets>().GetSprite(this.gameObject.name);
        if (item.owner is Soldier linkedSoldier && linkedSoldier.IsBull() && (item.IsGun() || item.IsAmmo()))
            transform.Find("ItemWeight").GetComponent<TextMeshProUGUI>().text = $"{1}";
        else
            transform.Find("ItemWeight").GetComponent<TextMeshProUGUI>().text = $"{item.weight}";
        if (item.IsGun() || item.IsAmmo())
        {
            transform.Find("Ammo").gameObject.SetActive(true);
            transform.Find("Ammo").GetComponent<TextMeshProUGUI>().text = $"{item.ammo}";
        }
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
                    if (CheckValidSlot(targetSlot))
                    {
                        SetCurrentSlot(targetSlot.AssignItemIcon(this));

                        targetSlotName = targetSlot.name;
                        if (targetSlot.linkedInventoryObject != null)
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
        if (menu.onItemUseScreen)
        {
            if (item.owner is Soldier linkedSoldier)
            {
                if (linkedSoldier.game.CheckAP(item.usageAP))
                {
                    if (item.IsUsable())
                    {
                        switch (item.itemName)
                        {
                            case "Food_Pack":
                            case "Water_Canteen":
                            case "ULF_Radio":
                                menu.OpenUseBasicItemUI(item, transform.parent.name, this);
                                break;
                            default:
                                break;
                        }
                    }
                }
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
