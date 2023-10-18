using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public Item item; // The item currently in the slot

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<ItemIcon>(out var draggedIcon))
        {
            // Drop the item into the slot
            item = draggedIcon.item;

            // Optionally, you can update the visuals or perform other actions

            // Reset the dragged icon's position
            draggedIcon.transform.SetParent(transform);
            draggedIcon.transform.localPosition = Vector3.zero;
        }
    }
}