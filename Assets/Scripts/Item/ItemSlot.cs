using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour
{
    public Item item; // The item currently in the slot
    public IHaveInventory linkedInventoryObject;

    public ItemSlot Init(IHaveInventory linkedInventoryObject)
    {
        this.linkedInventoryObject = linkedInventoryObject;

        return this;
    }
    public void AssignItemIcon(ItemIcon itemIcon)
    {
        // Move the item to the new slot
        itemIcon.originalSlot = transform;

        itemIcon.transform.SetParent(transform);
        itemIcon.rectTransform.localPosition = Vector3.zero;
        itemIcon.rectTransform.sizeDelta = Vector2.zero;

        // Set the item in the target slot
        item = itemIcon.item;
    }
    public void ClearItemIcon()
    {
        item = null;
    }
}