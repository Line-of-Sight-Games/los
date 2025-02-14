using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        //{"Food_Pack", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Flashbang", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Frag", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Smoke", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Grenade_Tabun", "", "", "", "", "", "", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        {"Knife", "", "", "", "", "", "LeftBrace", "Back", "", "", "", "", "", "", "", "", "", "", ""},
        //{"Medikit_Large", "", "", "", "RightHand", "LeftHand", "", "", "", "", "", "", "", "", "", "", "", "", ""},
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
    public ItemSlot head, chest, back, posterior, lateral, leftLeg, rightLeg, leftHand, rightHand;
    public ItemSlot jArmour1, jArmour2, jArmour3, jArmour4, bArmour1, bArmour2, backpack1, backpack2, backpack3, leftBrace, rightBrace, daredevil;

    public InventoryDisplayPanelSoldier Init(Soldier s)
    {
        ClearAllSlots();
        
        linkedSoldier = s;
        LinkSlots(new() { "Head" , "Chest", "Back", "Posterior", "Lateral", "LeftLeg", "RightLeg", "LeftHand", "RightHand" }, s);

        //head slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Head"), head);
        if (s.Inventory.GetItemInSlot("Head") is Item headItem && headItem.IsJuggernautArmour())
        {
            LinkSlots(new() { "JArmour1", "JArmour2", "JArmour3", "JArmour4" }, headItem);
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour1"), jArmour1);
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour2"), jArmour2);
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour3"), jArmour3);
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour4"), jArmour4);
        }

        //chest slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Chest"), chest);
        if (s.Inventory.GetItemInSlot("Chest") is Item chestItem && chestItem.IsBodyArmour())
        {
            LinkSlots(new() { "BArmour1", "BArmour2" }, chestItem);
            AddItemIconInSlot(chestItem.Inventory.GetItemInSlot("BArmour1"), bArmour1);
            AddItemIconInSlot(chestItem.Inventory.GetItemInSlot("BArmour2"), bArmour2);
        }

        //back slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Back"), back);
        if (s.Inventory.GetItemInSlot("Back") is Item backItem && backItem.IsBackpack())
        {
            LinkSlots(new() { "Backpack1", "Backpack2", "Backpack3" }, backItem);
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack1"), backpack1);
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack2"), backpack2);
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack3"), backpack3);
        }

        //posterior slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Posterior"), posterior);

        //lateral slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Lateral"), lateral);

        //left leg slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("LeftLeg"), leftLeg);
        if (s.Inventory.GetItemInSlot("LeftLeg") is Item leftBrace && leftBrace.IsBrace())
        {
            LinkSlots(new() { "LeftBrace" }, leftBrace);
            AddItemIconInSlot(leftBrace.Inventory.GetItemInSlot("Brace1"), this.leftBrace);
        }

        //right leg slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("RightLeg"), rightLeg);
        if (s.Inventory.GetItemInSlot("RightLeg") is Item rightBrace && rightBrace.IsBrace())
        {
            LinkSlots(new() { "RightBrace" }, rightBrace);
            AddItemIconInSlot(rightBrace.Inventory.GetItemInSlot("Brace1"), this.rightBrace);
        }

        //left hand slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("LeftHand"), leftHand);

        //right hand item
        AddItemIconInSlot(s.Inventory.GetItemInSlot("RightHand"), rightHand);

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
        print($"removing item from {slotName}");
        transform.FindRecursively(slotName).GetComponent<ItemSlot>().ClearItemIcon();
    }
    public void ClearAllSlots()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ItemSlot>() != null)
                RemoveItemIconInSlot(child.name);
        }
    }
    public void AddItemIconInSlot(Item item, ItemSlot slot)
    {
        if (item != null && slot != null)
        {
            if (slot.item == null)
            {
                ItemIcon newItemIcon = Instantiate(itemIconPrefab, slot.transform).GetComponent<ItemIcon>().Init(item, slot);
                slot.AssignItemIcon(newItemIcon);
            }
        }
    }
    private void Update()
    {
        DisplayAvailableSlots();
    }
    public void DisplayAvailableSlots()
    {
        //hide extra slots
        HideSlot("ArmourLine");
        
        //conditionally reveal
        if (CheckSlotContains("Back", "Backpack"))
        {
            RevealSlot("Backpack1");
            RevealSlot("Backpack2");
            RevealSlot("Backpack3");
        }
        else
        {
            HideSlot("Backpack1");
            HideSlot("Backpack2");
            HideSlot("Backpack3");
        }

        if (CheckSlotContains("Chest", "Armour_Body"))
        {
            RevealSlot("BArmour1");
            RevealSlot("BArmour2");
            RevealSlot("ArmourLine");
        }
        else
        {
            HideSlot("BArmour1");
            HideSlot("BArmour2");
        }

        if (CheckSlotContains("Head", "Armour_Juggernaut"))
        {
            RevealSlot("JArmour1");
            RevealSlot("JArmour2");
            RevealSlot("JArmour3");
            RevealSlot("JArmour4");
            RevealSlot("ArmourLine");
        }
        else
        {
            HideSlot("JArmour1");
            HideSlot("JArmour2");
            HideSlot("JArmour3");
            HideSlot("JArmour4");
        }

        if (CheckSlotContains("LeftLeg", "Brace"))
            RevealSlot("LeftBrace");
        else
            HideSlot("LeftBrace");


        if (CheckSlotContains("RightLeg", "Brace"))
            RevealSlot("RightBrace");
        else
            HideSlot("RightBrace");

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
