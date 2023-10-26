using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class InventoryDisplayPanelSoldier : MonoBehaviour
{
    static string[,] blockedSlotMatrix = new string[,]
    {
        {"SlotsBlocked", "Head","Chest","Back","LeftHand","RightHand","Lateral","Posterior","LeftLeg","LeftBrace","RightLeg","RightBrace","Armour1","Armour2","Armour3","Armour4","Backpack1","Backpack2","Backpack3","Misc1","Misc2","Misc3","Misc4","Misc5","Misc6","Misc7","Misc8" },
        {"Binoculars", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Claymore", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"E_Tool", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Food_Pack", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Flashbang", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Frag", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Smoke", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Tabun", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Knife", "", "", "", "", "", "LeftBrace", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Poison_Satchel", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Amphetamine", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Androstenedione", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Cannabinoid", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Danazol", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Glucocorticoid", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Modafinil", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Shard", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Trenbolone", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Unlabelled", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Thermal_Camera", "", "", "", "", "", "LeftBrace", "", "", "Lateral", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"UHF_Radio", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Water_Canteen", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
    };
    public ItemIcon itemIconPrefab;
    public Soldier linkedSoldier;
    public InventoryDisplayPanelSoldier Init(Soldier s)
    {
        RemoveItemIconInSlot("Head");
        RemoveItemIconInSlot("Chest");
        RemoveItemIconInSlot("Back");
        RemoveItemIconInSlot("Posterior");
        RemoveItemIconInSlot("Lateral");
        RemoveItemIconInSlot("LeftLeg");
        RemoveItemIconInSlot("RightLeg");
        RemoveItemIconInSlot("LeftHand");
        RemoveItemIconInSlot("RightHand");
        RemoveItemIconInSlot("LeftBrace");
        RemoveItemIconInSlot("RightBrace");
        RemoveItemIconInSlot("Backpack1");
        RemoveItemIconInSlot("Backpack2");
        RemoveItemIconInSlot("Backpack3");
        RemoveItemIconInSlot("Armour1");
        RemoveItemIconInSlot("Armour2");
        RemoveItemIconInSlot("Armour3");
        RemoveItemIconInSlot("Armour4");
        RemoveItemIconInSlot("Misc1");
        RemoveItemIconInSlot("Misc2");
        RemoveItemIconInSlot("Misc3");
        RemoveItemIconInSlot("Misc4");
        RemoveItemIconInSlot("Misc5");
        RemoveItemIconInSlot("Misc6");
        RemoveItemIconInSlot("Misc7");
        RemoveItemIconInSlot("Misc8");

        linkedSoldier = s;
        LinkSlots(s);
        AddItemIconInSlot(s, "Head");
        AddItemIconInSlot(s, "Chest");
        AddItemIconInSlot(s, "Back");
        AddItemIconInSlot(s, "Posterior");
        AddItemIconInSlot(s, "Lateral");
        AddItemIconInSlot(s, "LeftLeg");
        AddItemIconInSlot(s, "RightLeg");
        AddItemIconInSlot(s, "LeftHand");
        AddItemIconInSlot(s, "RightHand");
        AddItemIconInSlot(s, "LeftBrace");
        AddItemIconInSlot(s, "RightBrace");
        AddItemIconInSlot(s, "Backpack1");
        AddItemIconInSlot(s, "Backpack2");
        AddItemIconInSlot(s, "Backpack3");
        AddItemIconInSlot(s, "Armour1");
        AddItemIconInSlot(s, "Armour2");
        AddItemIconInSlot(s, "Armour3");
        AddItemIconInSlot(s, "Armour4");
        AddItemIconInSlot(s, "Misc1");
        AddItemIconInSlot(s, "Misc2");
        AddItemIconInSlot(s, "Misc3");
        AddItemIconInSlot(s, "Misc4");
        AddItemIconInSlot(s, "Misc5");
        AddItemIconInSlot(s, "Misc6");
        AddItemIconInSlot(s, "Misc7");
        AddItemIconInSlot(s, "Misc8");

        return this;
    }
    public void LinkSlots(Soldier s)
    {
        foreach (Transform child in transform)
            if (child.GetComponent<ItemSlot>() != null)
                child.GetComponent<ItemSlot>().Init(s);
    }
    public void RemoveItemIconInSlot(string slotName)
    {
        FindChildRecursively(transform, slotName).GetComponent<ItemSlot>().ClearItemIcon();
    }
    public void AddItemIconInSlot(Soldier s, string slotName)
    {
        s.inventorySlots.TryGetValue(slotName, out string itemId);

        if (itemId != "")
            Instantiate(itemIconPrefab, FindChildRecursively(transform, slotName)).GetComponent<ItemIcon>().Init(s.itemManager.FindItemById(itemId));
    }
    public Transform FindChildRecursively(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);

        if (child != null)
            return child;

        // If not found, search through all immediate children
        foreach (Transform childTransform in parent)
        {
            Transform foundInChildren = FindChildRecursively(childTransform, childName);

            if (foundInChildren != null)
                return foundInChildren;
        }

        return null;
    }

    private void Update()
    {
        DisplayAvailableSlots();
    }
    public void DisplayAvailableSlots()
    {
        CheckSlotAvailable("Posterior");
        CheckSlotAvailable("Lateral");
        CheckSlotAvailable("LeftBrace");
    }
    public void CheckSlotAvailable(string slotName)
    {
        if (FindChildRecursively(transform, slotName).TryGetComponent<ItemSlot>(out var slot))
            for (int i = 0; i < blockedSlotMatrix.GetLength(0); i++)
                for (int j = 0; j < blockedSlotMatrix.GetLength(j); j++)
                    if (blockedSlotMatrix[i, 0] == slot.item.itemName && blockedSlotMatrix[0, j] == slotName)
                        FindChildRecursively(transform, blockedSlotMatrix[0, j]).GetComponent<ItemSlot>().unavailable = true;
    }
}
