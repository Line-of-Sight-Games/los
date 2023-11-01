using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Search;
using System.Linq;
using UnityEngine;

public class InventoryDisplayPanelSoldier : MonoBehaviour
{
    public string[,] blockedSlotMatrix = new string[,]
    {
        {"SlotsBlocked", "Head","Chest","Back","LeftHand","RightHand","Lateral","Posterior","LeftLeg","LeftBrace","RightLeg","RightBrace","Armour1","Armour2","Armour3","Armour4","Backpack1","Backpack2","Backpack3","Misc1","Misc2","Misc3","Misc4","Misc5"},
        {"Armour_Exo", "Back|LeftLeg|RightLeg|Lateral|Posterior", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Armour_Ghillie", "Chest|LeftLeg|RightLeg|Posterior", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Armour_Juggernaut", "Chest|Posterior", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Armour_Stim", "Chest", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Binoculars", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Claymore", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"E_Tool", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Food_Pack", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Flashbang", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Frag", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Smoke", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Tabun", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Knife", "", "", "", "", "", "LeftBrace", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Medkit_Large", "", "", "", "RightHand", "LeftHand", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Poison_Satchel", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Riot_Shield", "", "", "", "RightHand", "LeftHand", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Amphetamine", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Androstenedione", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Cannabinoid", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Danazol", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Glucocorticoid", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Modafinil", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Shard", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Trenbolone", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Unlabelled", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Thermal_Camera", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"UHF_Radio", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Water_Canteen", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
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
        transform.FindRecursively(slotName).GetComponent<ItemSlot>().ClearItemIcon();
    }
    public void AddItemIconInSlot(Soldier s, string slotName)
    {
        Transform targetSlot = transform.FindRecursively(slotName);
        s.inventorySlots.TryGetValue(slotName, out string itemId);

        if (itemId != "")
            targetSlot.GetComponent<ItemSlot>().AssignItemIcon(Instantiate(itemIconPrefab, targetSlot).GetComponent<ItemIcon>().Init(s.itemManager.FindItemById(itemId)));
    }
    private void Update()
    {
        DisplayAvailableSlots();
    }
    public void DisplayAvailableSlots()
    {
        //slot hiding and revealing
        if (CheckSlotContains("Back", "Backpack"))
        {
            RevealSlot("Backpack1");
            RevealSlot("Backpack2");
            RevealSlot("Backpack3");
            RevealSlot("Misc1");
            RevealSlot("Misc2");
        }
        else
        {
            HideSlot("Backpack1");
            HideSlot("Backpack2");
            HideSlot("Backpack3");
            HideSlot("Misc1");
            HideSlot("Misc2");
        }
        if (CheckSlotContains("Chest", "Armour_Body"))
        {
            RevealSlot("Armour1");
            RevealSlot("Armour2");
            HideSlot("Armour3");
            HideSlot("Armour4");
            RevealSlot("ArmourLine");
        }
        else if (CheckSlotContains("Head", "Armour_Juggernaut"))
        {
            RevealSlot("Armour1");
            RevealSlot("Armour2");
            RevealSlot("Armour3");
            RevealSlot("Armour4");
            RevealSlot("ArmourLine");
        }
        else
        {
            HideSlot("Armour1");
            HideSlot("Armour2");
            HideSlot("Armour3");
            HideSlot("Armour4");
            HideSlot("ArmourLine");
        }
        if (CheckSlotContains("LeftLeg", "Brace"))
        {
            RevealSlot("LeftBrace");
            RevealSlot("Misc5");
        }
        else
        {
            HideSlot("LeftBrace");
            HideSlot("Misc5");
        }
        if (CheckSlotContains("RightLeg", "Brace"))
        {
            RevealSlot("RightBrace");
            RevealSlot("Misc4");
        }
        else
        {
            HideSlot("RightBrace");
            HideSlot("Misc4");
        }
        if (CheckSlotContains("Posterior", "Bag"))
            RevealSlot("Misc3");
        else
            HideSlot("Misc3");

        //dynamic slot blocking
        UnblockAllSlots();
        BlockSlotsCheck("Head");
        BlockSlotsCheck("LeftHand");
        BlockSlotsCheck("RightHand");
        BlockSlotsCheck("Posterior");
        BlockSlotsCheck("Lateral");
        BlockSlotsCheck("LeftBrace");
    }
    public void UnblockAllSlots()
    {
        foreach (Transform child in transform)
            if (child.TryGetComponent(out ItemSlot slot))
                slot.unavailable = false;
    }
    public void BlockSlotsCheck(string slotName)
    {
        Transform targetTransform = transform.FindRecursively(slotName);
        if (targetTransform != null)
            if (targetTransform.TryGetComponent(out ItemSlot slot))
                if (slot.item != null)
                    for (int i = 0; i < blockedSlotMatrix.GetLength(0); i++)
                        for (int j = 0; j < blockedSlotMatrix.GetLength(1); j++)
                            if (blockedSlotMatrix[i, 0] == slot.item.itemName && blockedSlotMatrix[0, j] == slotName)
                                blockedSlotMatrix[i,j].ToString().Split('|').ToList().ForEach(part => BlockSlot(part));
    }
    public void BlockSlot(string slotName)
    {
        if (transform.FindRecursively(slotName) != null)
            if (transform.FindRecursively(slotName).TryGetComponent(out ItemSlot blockedSlot))
                blockedSlot.unavailable = true;
    }
    public bool CheckSlotContains(string slotName, string itemName)
    {
        Transform targetTransform = transform.FindRecursively(slotName);
        if (targetTransform != null)
            foreach (Transform child in targetTransform)
                if (child.name == itemName)
                    return true;
        return false;
    }
    public void RevealSlot(string slotName)
    {
        Transform targetTransform = transform.FindRecursively(slotName);
        if (targetTransform != null)
            targetTransform.gameObject.SetActive(true);
    }
    public void HideSlot(string slotName)
    {
        Transform targetTransform = transform.FindRecursively(slotName);
        if (targetTransform != null)
            targetTransform.gameObject.SetActive(false);
    }
}
