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
    public ItemManager itemManager;
    public Item item; // The item associated with this icon
    public RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Transform originalSlot;
    private Transform temporaryParent;

    public ItemIcon Init(Item item)
    {
        this.item = item;
        gameObject.name = item.itemName;
        transform.Find("ItemImage").GetComponent<Image>().sprite = FindObjectOfType<ItemAssets>().GetSprite(this.gameObject.name);
        transform.Find("ItemWeight").GetComponent<TextMeshProUGUI>().text = $"{item.weight}";
        if (item.ammo != 0)
        {
            transform.Find("Ammo").gameObject.SetActive(true);
            transform.Find("Ammo").GetComponent<TextMeshProUGUI>().text = $"{item.ammo}";
        }

        return this;
    }
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalSlot = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        temporaryParent = new GameObject("TemporaryParent").transform;
        temporaryParent.SetParent(GetComponentInParent<Canvas>().transform);
        transform.SetParent(temporaryParent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        //Destroy(temporaryParent.gameObject);

        // Check if the item was dropped over a valid slot
        if (eventData.pointerEnter != null)
        {
            ItemSlot targetSlot = eventData.pointerEnter.GetComponent<ItemSlot>();
            ItemSlot oldSlot = originalSlot.GetComponent<ItemSlot>();
            // Check if the target slot is empty
            if (targetSlot != null && targetSlot.item == null && CheckValidSlot(targetSlot))
            {
                targetSlot.AssignItemIcon(this);
                oldSlot.ClearItemIcon();
                item.ChangeOwner(item.owner, targetSlot.linkedInventoryObject);
            }
            else
            {
                // Return the item to the original slot
                transform.SetParent(originalSlot);
                rectTransform.localPosition = Vector3.zero;
            }
        }
        else
        {
            // Return the item to the original slot
            transform.SetParent(originalSlot);
            rectTransform.localPosition = Vector3.zero;
        }
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
        }
            
        return false;
    }
}
