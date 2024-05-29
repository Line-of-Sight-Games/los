using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayPanelSoldier : MonoBehaviour
{
    public string[,] blockedSlotMatrix = new string[,]
    {
        {"SlotsBlocked", "Head","Chest","Back","LeftHand","RightHand","Lateral","Posterior","LeftLeg","LeftBrace","RightLeg","RightBrace","Armour1","Armour2","Armour3","Armour4","Backpack1","Backpack2","Backpack3"},
        {"Armour_Exo", "Back|LeftLeg|RightLeg|Lateral|Posterior", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Armour_Ghillie", "Chest|LeftLeg|RightLeg|Posterior", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Armour_Juggernaut", "Chest|Posterior", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Armour_Stim", "Chest", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Binoculars", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Claymore", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"E_Tool", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Food_Pack", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Flashbang", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Frag", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Smoke", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Tabun", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Knife", "", "", "", "", "", "LeftBrace", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        //{"Medkit_Large", "", "", "", "RightHand", "LeftHand", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Poison_Satchel", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        //{"Riot_Shield", "", "", "", "RightHand", "LeftHand", "", "", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Amphetamine", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Androstenedione", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Cannabinoid", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Danazol", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Glucocorticoid", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Modafinil", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Shard", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Trenbolone", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Syringe_Unlabelled", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Thermal_Camera", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", ""},
        {"UHF_Radio", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", ""},
        {"Water_Canteen", "", "", "", "", "", "", "", "", "Lateral", "", "", "", "", "", "", "", "", ""},
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
        RemoveItemIconInSlot("BArmour1");
        RemoveItemIconInSlot("BArmour2");
        RemoveItemIconInSlot("JArmour1");
        RemoveItemIconInSlot("JArmour2");
        RemoveItemIconInSlot("JArmour3");
        RemoveItemIconInSlot("JArmour4");
        RemoveItemIconInSlot("BackpackMedM");
        RemoveItemIconInSlot("BackpackMedS");
        RemoveItemIconInSlot("BagMedM");
        RemoveItemIconInSlot("RightBraceMedS");
        RemoveItemIconInSlot("LeftBraceMedS");
        
        linkedSoldier = s;
        LinkSlots(new() { "Head" , "Chest", "Back", "Posterior", "Lateral", "LeftLeg", "RightLeg", "LeftHand", "RightHand" }, s);
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Head"), "Head");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Chest"), "Chest");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Back"), "Back");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Posterior"), "Posterior");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Lateral"), "Lateral");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("LeftLeg"), "LeftLeg");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("RightLeg"), "RightLeg");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("LeftHand"), "LeftHand");
        AddItemIconInSlot(s.Inventory.GetItemInSlot("RightHand"), "RightHand");

        //show items attached to other items
        if (s.Inventory.GetItemInSlot("Back") is Item backItem && backItem.IsBackpack())
        {
            LinkSlots(new() { "Backpack1", "Backpack2", "Backpack3", "BackpackMedM", "BackpackMedS" }, backItem);
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack1"), "Backpack1");
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack2"), "Backpack2");
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack3"), "Backpack3");
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("BackpackMedM"), "BackpackMedM");
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("BackpackMedS"), "BackpackMedS");
        }
        if (s.Inventory.GetItemInSlot("Chest") is Item armour)
        {
            if (armour.IsBodyArmour())
            {
                LinkSlots(new() { "BArmour1", "BArmour2" }, armour);
                AddItemIconInSlot(armour.Inventory.GetItemInSlot("BArmour1"), "BArmour1");
                AddItemIconInSlot(armour.Inventory.GetItemInSlot("BArmour2"), "BArmour2");
            }
            else if (armour.IsJuggernautArmour())
            {
                LinkSlots(new() { "JArmour1", "JArmour2", "JArmour3", "JArmour4" }, armour);
                AddItemIconInSlot(armour.Inventory.GetItemInSlot("JArmour1"), "JArmour1");
                AddItemIconInSlot(armour.Inventory.GetItemInSlot("JArmour2"), "JArmour2");
                AddItemIconInSlot(armour.Inventory.GetItemInSlot("JArmour3"), "JArmour3");
                AddItemIconInSlot(armour.Inventory.GetItemInSlot("JArmour4"), "JArmour4");
            }
        }
        if (s.Inventory.GetItemInSlot("Posterior") is Item posteriorItem && posteriorItem.IsBag())
        {
            LinkSlots(new() { "BagMedM" }, posteriorItem);
            AddItemIconInSlot(posteriorItem.Inventory.GetItemInSlot("BagMedM"), "BagMedM");
        }
            
        if (s.Inventory.GetItemInSlot("LeftLeg") is Item leftBrace && leftBrace.IsBrace())
        {
            LinkSlots(new() { "LeftBrace", "LeftBraceMedS" }, leftBrace);
            AddItemIconInSlot(leftBrace.Inventory.GetItemInSlot("Brace1"), "LeftBrace");
            AddItemIconInSlot(leftBrace.Inventory.GetItemInSlot("BraceMedS"), "LeftBraceMedS");
        }
        if (s.Inventory.GetItemInSlot("RightLeg") is Item rightBrace && rightBrace.IsBrace())
        {
            LinkSlots(new() { "RightBrace", "RightBraceMedS" }, rightBrace);
            AddItemIconInSlot(rightBrace.Inventory.GetItemInSlot("Brace1"), "RightBrace");
            AddItemIconInSlot(rightBrace.Inventory.GetItemInSlot("BraceMedS"), "RightBraceMedS");
        }

        return this;
    }
    public void LinkSlots(List<string> slotNames, IHaveInventory inventoryObject)
    {
        foreach (string slotName in slotNames)
            transform.FindRecursively(slotName).GetComponent<ItemSlot>().Init(inventoryObject);
    }
    public Item GetItemInSlot(string slotName)
    {
        Transform targetSlot = transform.FindRecursively(slotName);
        return targetSlot.GetComponent<ItemSlot>().item;
    }
    public void RemoveItemIconInSlot(string slotName)
    {
        transform.FindRecursively(slotName).GetComponent<ItemSlot>().ClearItemIcon();
    }
    public void AddItemIconInSlot(Item item, string slotName)
    {
        if (item != null)
        {
            Transform targetSlot = transform.FindRecursively(slotName);
            targetSlot.GetComponent<ItemSlot>().AssignItemIcon(Instantiate(itemIconPrefab, targetSlot).GetComponent<ItemIcon>().Init(item));
        }   
    }
    private void Update()
    {
        DisplayAvailableSlots();
    }
    public void DisplayAvailableSlots()
    {
        //hide extra slots
        HideSlot("Backpack1");
        HideSlot("Backpack2");
        HideSlot("Backpack3");
        HideSlot("BackpackMedM");
        HideSlot("BackpackMedS");
        HideSlot("BArmour1");
        HideSlot("BArmour2");
        HideSlot("JArmour1");
        HideSlot("JArmour2");
        HideSlot("JArmour3");
        HideSlot("JArmour4");
        HideSlot("ArmourLine");
        HideSlot("LeftBrace");
        HideSlot("LeftBraceMedS");
        HideSlot("RightBrace");
        HideSlot("RightBraceMedS");
        HideSlot("BagMedM");

        //conditionally reveal
        if (CheckSlotContains("Back", "Backpack"))
        {
            RevealSlot("Backpack1");
            RevealSlot("Backpack2");
            RevealSlot("Backpack3");
            RevealSlot("BackpackMedM");
            RevealSlot("BackpackMedS");
        }
        if (CheckSlotContains("Chest", "Armour_Body"))
        {
            RevealSlot("BArmour1");
            RevealSlot("BArmour2");
            RevealSlot("ArmourLine");
        }
        if (CheckSlotContains("Head", "Armour_Juggernaut"))
        {
            RevealSlot("JArmour1");
            RevealSlot("JArmour2");
            RevealSlot("JArmour3");
            RevealSlot("JArmour4");
            RevealSlot("ArmourLine");
        }
        if (CheckSlotContains("LeftLeg", "Brace"))
        {
            RevealSlot("LeftBrace");
            RevealSlot("LeftBraceMedS");
        }
        if (CheckSlotContains("RightLeg", "Brace"))
        {
            RevealSlot("RightBrace");
            RevealSlot("RightBraceMedS");
        }
        if (CheckSlotContains("Posterior", "Bag"))
            RevealSlot("BagMedM");

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
