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

    public InventoryDisplayPanelSoldier Init(Soldier s)
    {
        ClearAllSlots();
        
        linkedSoldier = s;
        LinkSlots(new() { "Head" , "Chest", "Back", "Posterior", "Lateral", "LeftLeg", "RightLeg", "LeftHand", "RightHand" }, s);

        //head slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Head"), "Head");
        if (s.Inventory.GetItemInSlot("Head") is Item headItem && headItem.IsJuggernautArmour())
        {
            LinkSlots(new() { "JArmour1", "JArmour2", "JArmour3", "JArmour4" }, headItem);
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour1"), "JArmour1");
            if (headItem.Inventory.GetItemInSlot("JArmour1") is Item gunItem1 && gunItem1.IsSuppressibleGun())
            {
                LinkSlots(new() { $"JArmour1Suppressor" }, gunItem1);
                AddItemIconInSlot(gunItem1.Inventory.GetItemInSlot("Suppressor"), $"JArmour1Suppressor");
            }
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour2"), "JArmour2");
            if (headItem.Inventory.GetItemInSlot("JArmour2") is Item gunItem2 && gunItem2.IsSuppressibleGun())
            {
                LinkSlots(new() { $"JArmour2Suppressor" }, gunItem2);
                AddItemIconInSlot(gunItem2.Inventory.GetItemInSlot("Suppressor"), $"JArmour2Suppressor");
            }
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour3"), "JArmour3");
            if (headItem.Inventory.GetItemInSlot("JArmour3") is Item gunItem3 && gunItem3.IsSuppressibleGun())
            {
                LinkSlots(new() { $"JArmour3Suppressor" }, gunItem3);
                AddItemIconInSlot(gunItem3.Inventory.GetItemInSlot("Suppressor"), $"JArmour3Suppressor");
            }
            AddItemIconInSlot(headItem.Inventory.GetItemInSlot("JArmour4"), "JArmour4");
            if (headItem.Inventory.GetItemInSlot("JArmour4") is Item gunItem4 && gunItem4.IsSuppressibleGun())
            {
                LinkSlots(new() { $"JArmour3Suppressor" }, gunItem4);
                AddItemIconInSlot(gunItem4.Inventory.GetItemInSlot("Suppressor"), $"JArmour4Suppressor");
            }
        }


        //chest slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Chest"), "Chest");
        if (s.Inventory.GetItemInSlot("Chest") is Item chestItem && chestItem.IsBodyArmour())
        {
            LinkSlots(new() { "BArmour1", "BArmour2" }, chestItem);
            AddItemIconInSlot(chestItem.Inventory.GetItemInSlot("BArmour1"), "BArmour1");
            if (chestItem.Inventory.GetItemInSlot("BArmour1") is Item gunItem5 && gunItem5.IsSuppressibleGun())
            {
                LinkSlots(new() { $"BArmour1Suppressor" }, gunItem5);
                AddItemIconInSlot(gunItem5.Inventory.GetItemInSlot("Suppressor"), $"BArmour1Suppressor");
            }
            AddItemIconInSlot(chestItem.Inventory.GetItemInSlot("BArmour2"), "BArmour2");
            if (chestItem.Inventory.GetItemInSlot("BArmour2") is Item gunItem6 && gunItem6.IsSuppressibleGun())
            {
                LinkSlots(new() { $"BArmour2Suppressor" }, gunItem6);
                AddItemIconInSlot(gunItem6.Inventory.GetItemInSlot("Suppressor"), $"BArmour2Suppressor");
            }
        }

        
        //back slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Back"), "Back");
        if (s.Inventory.GetItemInSlot("Back") is Item backItem && backItem.IsBackpack())
        {
            LinkSlots(new() { "Backpack1", "Backpack2", "Backpack3", "BackpackMedM", "BackpackMedS" }, backItem);
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack1"), "Backpack1");
            if (backItem.Inventory.GetItemInSlot("Backpack1") is Item gunItem7 && gunItem7.IsSuppressibleGun())
            {
                LinkSlots(new() { $"Backpack1Suppressor" }, gunItem7);
                AddItemIconInSlot(gunItem7.Inventory.GetItemInSlot("Suppressor"), $"Backpack1Suppressor");
            }
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack2"), "Backpack2");
            if (backItem.Inventory.GetItemInSlot("Backpack2") is Item gunItem8 && gunItem8.IsSuppressibleGun())
            {
                LinkSlots(new() { $"Backpack2Suppressor" }, gunItem8);
                AddItemIconInSlot(gunItem8.Inventory.GetItemInSlot("Suppressor"), $"Backpack2Suppressor");
            }
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("Backpack3"), "Backpack3");
            if (backItem.Inventory.GetItemInSlot("Backpack3") is Item gunItem9 && gunItem9.IsSuppressibleGun())
            {
                LinkSlots(new() { $"Backpack3Suppressor" }, gunItem9);
                AddItemIconInSlot(gunItem9.Inventory.GetItemInSlot("Suppressor"), $"Backpack3Suppressor");
            }
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("BackpackMedM"), "BackpackMedM");
            AddItemIconInSlot(backItem.Inventory.GetItemInSlot("BackpackMedS"), "BackpackMedS");
        }


        //posterior slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Posterior"), "Posterior");
        if (s.Inventory.GetItemInSlot("Posterior") is Item posteriorItem && posteriorItem.IsBag())
        {
            LinkSlots(new() { "BagMedM" }, posteriorItem);
            AddItemIconInSlot(posteriorItem.Inventory.GetItemInSlot("BagMedM"), "BagMedM");
        }


        //lateral slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("Lateral"), "Lateral");


        //left leg slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("LeftLeg"), "LeftLeg");
        if (s.Inventory.GetItemInSlot("LeftLeg") is Item leftBrace && leftBrace.IsBrace())
        {
            LinkSlots(new() { "LeftBrace", "LeftBraceMedS" }, leftBrace);
            AddItemIconInSlot(leftBrace.Inventory.GetItemInSlot("Brace1"), "LeftBrace");
            AddItemIconInSlot(leftBrace.Inventory.GetItemInSlot("BraceMedS"), "LeftBraceMedS");
        }

        //right leg slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("RightLeg"), "RightLeg");
        if (s.Inventory.GetItemInSlot("RightLeg") is Item rightBrace && rightBrace.IsBrace())
        {
            LinkSlots(new() { "RightBrace", "RightBraceMedS" }, rightBrace);
            AddItemIconInSlot(rightBrace.Inventory.GetItemInSlot("Brace1"), "RightBrace");
            AddItemIconInSlot(rightBrace.Inventory.GetItemInSlot("BraceMedS"), "RightBraceMedS");
        }

        //left hand slot
        AddItemIconInSlot(s.Inventory.GetItemInSlot("LeftHand"), "LeftHand");
        if (s.Inventory.GetItemInSlot("LeftHand") is Item gunItem10 && gunItem10.IsSuppressibleGun())
        {
            LinkSlots(new() { $"LeftHandSuppressor" }, gunItem10);
            AddItemIconInSlot(gunItem10.Inventory.GetItemInSlot("Suppressor"), $"LeftHandSuppressor");
        }


        //right hand item
        AddItemIconInSlot(s.Inventory.GetItemInSlot("RightHand"), "RightHand");
        if (s.Inventory.GetItemInSlot("RightHand") is Item gunItem11 && gunItem11.IsSuppressibleGun())
        {
            LinkSlots(new() { $"RightHandSuppressor" }, gunItem11);
            AddItemIconInSlot(gunItem11.Inventory.GetItemInSlot("Suppressor"), $"RightHandSuppressor");
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
    public void ClearAllSlots()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ItemSlot>() != null)
                RemoveItemIconInSlot(child.name);
        }
    }
    public void AddItemIconInSlot(Item item, string slotName)
    {
        if (item != null)
        {
            print($"trying to add {item.itemName} to {slotName}");
            Transform targetSlot = transform.FindRecursively(slotName);
            targetSlot.GetComponent<ItemSlot>().AssignItemIcon(Instantiate(itemIconPrefab, targetSlot).GetComponent<ItemIcon>().Init(item));
            print($"successfully added {item.itemName} to {targetSlot.name}");
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
            RevealSlot("BackpackMedM");
            RevealSlot("BackpackMedS");
        }
        else
        {
            HideSlot("Backpack1");
            HideSlot("Backpack2");
            HideSlot("Backpack3");
            HideSlot("BackpackMedM");
            HideSlot("BackpackMedS");
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
        {
            RevealSlot("LeftBrace");
            RevealSlot("LeftBraceMedS");
        }
        else
        {
            HideSlot("LeftBrace");
            HideSlot("LeftBraceMedS");
        }

        if (CheckSlotContains("RightLeg", "Brace"))
        {
            RevealSlot("RightBrace");
            RevealSlot("RightBraceMedS");
        }
        else
        {
            HideSlot("RightBrace");
            HideSlot("RightBraceMedS");
        }

        if (CheckSlotContains("Posterior", "Bag"))
            RevealSlot("BagMedM");
        else
            HideSlot("BagMedM");

        string[] gunSlots = { "LeftHand", "RightHand", "Backpack1", "Backpack2", "Backpack3", "BArmour1", "BArmour2", "JArmour1", "JArmour2", "JArmour3", "JArmour4" };
        foreach (string gunSlot in gunSlots)
        {
            if (CheckSlotContainsSuppressibleGun(gunSlot))
                RevealSlot($"{gunSlot}Suppressor");
            else
                HideSlot($"{gunSlot}Suppressor");
        }

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
    public bool CheckSlotContainsSuppressibleGun(string slotName)
    {
        string[] gunNameList = { "AR_ACOG_FAL", "AR_AK_47", "AR_M_16", "LMG_LSW", "LMG_M_60", "LMG_SAW", "Pi_357", "Pi_Glock", "Pi_Sidearm", "Sh_Ithaca", "Sh_Olympus", "Sh_SPAS_12", "SMG_P_90", "SMG_Thompson", "SMG_UMP_40", "Sn_Barrett", "Sn_Dragunov", "Sn_Intervention" };
        print($"{slotName}");
        Transform targetTransform = transform.FindRecursively(slotName);
        if (targetTransform != null)
            foreach (Transform child in targetTransform)
                if (gunNameList.Contains(child.name))
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
