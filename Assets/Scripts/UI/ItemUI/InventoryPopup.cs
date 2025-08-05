using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class InventoryPopup : MonoBehaviour
{
    public MainMenu menu;
    public GameObject inventoryArea;
    public ItemSlot itemSlotPrefab;
    public ItemIcon itemIconPrefab;
    public Item linkedItem;

    void Update()
    {
        if (Input.anyKey && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            HideInventoryPopup();
        }
    }
    public void ShowInventoryPopup()
    {
        foreach (KeyValuePair<string, string> inventorySlotKVP in linkedItem.inventorySlots)
        {
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, inventoryArea.transform).Init(linkedItem);
            itemSlot.AssignItemIcon(Instantiate(itemIconPrefab, itemSlot.transform).Init(ItemManager.Instance.FindItemById(inventorySlotKVP.Value), itemSlot));
        }
        gameObject.SetActive(true);
    }
    public void HideInventoryPopup()
    {
        gameObject.SetActive(false);
    }
}
