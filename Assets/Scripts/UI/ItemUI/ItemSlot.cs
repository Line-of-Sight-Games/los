using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public IHaveInventory linkedInventoryObject;
    public Item item; // The item currently in the slot
    public bool unavailable;
    public ItemIcon parentIcon;
    public Transform blocked;

    public ItemSlot Init(IHaveInventory linkedInventoryObject)
    {
        this.linkedInventoryObject = linkedInventoryObject;

        return this;
    }
    public ItemSlot AssignItemIcon(ItemIcon itemIcon)
    {
        print($"trying to assign the item icon: {itemIcon.name}|{itemIcon.item.itemName}");
        // Set the item in the target slot
        item = itemIcon.item;

        itemIcon.transform.SetParent(transform);
        itemIcon.rectTransform.localPosition = Vector3.zero;
        itemIcon.rectTransform.sizeDelta = Vector2.zero;
        itemIcon.rectTransform.localScale = Vector2.one;

        return this;
    }
    public void ClearItemIcon()
    {
        foreach(Transform child in transform)
            if (child.name != "Blocked")
                Destroy(child.gameObject);

        item = null;
    }
    private void Update()
    {
        blocked.gameObject.SetActive(unavailable);
    }
}