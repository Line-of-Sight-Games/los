using TMPro;
using UnityEditor;
using UnityEngine;

public class InfiniteItemSlot : MonoBehaviour
{
    public ItemSlot linkedSlot;
    public ItemIcon itemIconPrefab;
    public Item itemPrefab, linkedItem;

    void Update()
    {
        if (linkedSlot.item == null)
        {
            print($"itemslot is empty | {transform.name}");
            linkedItem = Instantiate(itemPrefab).Init(transform.name);
            linkedItem.transform.position = new(1, 1, 0);
            linkedItem.X = 1;
            linkedItem.Y = 1;
            linkedItem.Z = 0;
            linkedSlot.AssignItemIcon(Instantiate(itemIconPrefab).Init(linkedItem, linkedSlot));
        }
    }

    private void OnDisable()
    {
        linkedItem.DestroyItem(null);
        linkedSlot.ClearItemIcon();
    }
}