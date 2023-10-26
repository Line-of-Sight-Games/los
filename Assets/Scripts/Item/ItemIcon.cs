using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Diagnostics.Contracts;

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
        transform.Find("ItemImage").GetComponent<Image>().sprite = FindObjectOfType<ItemAssets>().GetSprite(this.gameObject.name);
        transform.Find("ItemWeight").GetComponent<TextMeshProUGUI>().text = $"{item.weight}";
        if (item.ammo != 0)
        {
            transform.Find("Ammo").gameObject.SetActive(true);
            transform.Find("Ammo").GetComponent<TextMeshProUGUI>().text = $"{item.ammo}";
        }

        return this;
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

        if (targetDrop != null)
        {
            ItemSlot targetSlot = targetDrop.GetComponent<ItemSlot>();
            InventorySourcePanel targetPanel = targetDrop.GetComponentInParent<InventorySourcePanel>();
            if (originalInventoryObject != null)
                originalInventoryId = originalInventoryObject.Id;

            if (targetSlot != null)
            {
                if (targetSlot.item == null && CheckValidSlot(targetSlot))
                {
                    targetSlot.AssignItemIcon(this);
                    SetCurrentSlot();

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
                //item.ChangeOwner(item.owner, targetPanel.linkedInventorySource);
            }
            else if (targetPanel != null)
            {
                
                if (targetPanel.linkedInventorySource == originalInventoryObject)
                {
                    ReturnToOldSlot();

                    item.markedForAction = string.Empty;
                }
                else if (targetPanel.name.Contains("Ground"))
                {
                    Instantiate(targetPanel.itemSlotPrefab, targetPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>().Init(targetPanel.linkedInventorySource).AssignItemIcon(this);
                    SetCurrentSlot();

                    targetSlotName = transform.parent.name;
                    item.markedForAction = $"{originalInventoryId}|{originalSlotName}|{targetInventoryId}|{targetSlotName}";
                }
                else if (targetPanel.name.Contains("GB"))
                {
                    Instantiate(targetPanel.itemSlotPrefab, targetPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>().Init(targetPanel.linkedInventorySource).AssignItemIcon(this);
                    SetCurrentSlot();

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
    }
    public void Update()
    {

    }

    public void LeftClick()
    {

    }

    public void RightClick()
    {

    }
    public bool CheckValidSlot(ItemSlot targetSlot)
    {
        foreach (string slot in item.equippableSlots)
        {
            print($"Trying Slot: {slot} in {targetSlot.name}");
            if (targetSlot.name.Contains(slot))
                return true;

            if (targetSlot.name.Contains("ItemSlot"))
                return true;
        }
            
        return false;
    }
    public void ReturnToOldSlot()
    {
        currentSlot.AssignItemIcon(this);
    }
    public void SetCurrentSlot()
    {
        currentSlot.ClearItemIcon();
        currentSlot = transform.parent.GetComponent<ItemSlot>();
    }
}
