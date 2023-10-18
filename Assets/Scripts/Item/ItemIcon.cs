using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine.EventSystems;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemManager itemManager;
    public Item item; // The item associated with this icon
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalSlot;
    public ItemIcon Init(Item item)
    {
        gameObject.name = item.itemName;
        transform.Find("ItemImage").GetComponent<Image>().sprite = FindObjectOfType<ItemAssets>().GetSprite(this.gameObject.name);

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
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if the item was dropped over a valid slot
        if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<ItemSlot>() != null)
        {
            Transform newSlot = eventData.pointerEnter.transform;
            originalSlot = newSlot;
        }
        else
        {
            // Return the item to the original slot
            transform.SetParent(originalSlot);
            rectTransform.localPosition = Vector3.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // You can implement additional behavior when the icon is clicked
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
}
