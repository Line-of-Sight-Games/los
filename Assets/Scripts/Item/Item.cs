using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEditor;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class Item : PhysicalObject, IDataPersistence
{
    public Dictionary<string, object> details;
    public MainGame game;
    public MainMenu menu; 
    public ItemReader reader;
    public ItemManager itemManager;
    public IHaveInventory owner;
    public string markedForAction;

    public Sprite itemImage;
    public int itemIndex;

    public string itemName;
    public List<string> traits;
    public int usageAP;
    public int weight;
    public int ammo;
    public int ablativeHealth;
    public int hpGranted;
    public int loudRadius;
    public int meleeDamage;
    public int charges;
    public string poisonedBy;
    public List<string> equippableSlots;
    public string whereEquipped;

    public ItemReader.GunTraits gunTraits;

    public BoxCollider bodyCollider;

    private void Awake()
    {
        game = FindObjectOfType<MainGame>();
        menu = FindObjectOfType<MainMenu>();
        reader = FindObjectOfType<ItemReader>();
        itemManager = FindObjectOfType<ItemManager>();
    }

    public Item Init(string name)
    {
        owner = null;
        itemIndex = GetIndex(name);
        itemImage = GetSprite(name);

        id = GenerateGuid();
        this.name = name;
        itemName = name;
        traits = reader.allItems.items[itemIndex].Traits;
        usageAP = reader.allItems.items[itemIndex].UsageAP;
        weight = reader.allItems.items[itemIndex].Weight;
        ammo = reader.allItems.items[itemIndex].Ammo;
        meleeDamage = reader.allItems.items[itemIndex].MeleeDamage;
        ablativeHealth = reader.allItems.items[itemIndex].AblativeHealth;
        hpGranted = reader.allItems.items[itemIndex].HPGranted;
        loudRadius = reader.allItems.items[itemIndex].LoudRadius;
        charges = reader.allItems.items[itemIndex].Charges;
        poisonedBy = reader.allItems.items[itemIndex].PoisonedBy;
        gunTraits = reader.allItems.items[itemIndex].GunTraits;
        equippableSlots = reader.allItems.items[itemIndex].EquippableSlots;
        whereEquipped = reader.allItems.items[itemIndex].WhereEquipped;

        itemManager.RefreshItemList();

        return this;
    }

    public Item SetParent(GameObject parent)
    {
        //make physical objects children of battlefield
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        transform.SetParent(parent.transform);

        return this;
    }

    public int GetIndex(string name)
    {
        return name switch
        {
            "Ammo_AR" => 0,
            "Ammo_LMG" => 1,
            "Ammo_Pi" => 2,
            "Ammo_Ri" => 3,
            "Ammo_Sh" => 4,
            "Ammo_SMG" => 5,
            "Ammo_Sn" => 6,
            "AR_ACOG_FAL" => 7,
            "AR_AK_47" => 8,
            "AR_M_16" => 9,
            "Armour_Body" => 10,
            "Armour_Exo" => 11,
            "Armour_Ghillie" => 12,
            "Armour_Juggernaut" => 13,
            "Armour_Stimulant" => 14,
            "Backpack" => 15,
            "Bag" => 16,
            "Binoculars" => 17,
            "Brace" => 18,
            "Claymore" => 19,
            "Deployment_Beacon" => 20,
            "E_Tool" => 21,
            "Food_Pack" => 22,
            "Grenade_Flashbang" => 23,
            "Grenade_Frag" => 24,
            "Grenade_Smoke" => 25,
            "Grenade_Tabun" => 26,
            "Knife" => 27,
            "Medkit_Large" => 28,
            "Medkit_Medium" => 29,
            "Medkit_Small" => 30,
            "LMG_LSW" => 31,
            "LMG_M_60" => 32,
            "LMG_SAW" => 33,
            "Logistics_Belt" => 34,
            "Pi_357" => 35,
            "Pi_Glock" => 36,
            "Pi_Sidearm" => 37,
            "Poison_Satchel" => 38,
            "Ri_Arisaka" => 39,
            "Ri_Carbine" => 40,
            "Ri_M1_Garand" => 41,
            "Riot_Shield" => 42,
            "Sh_Ithaca" => 43,
            "Sh_Olympus" => 44,
            "Sh_SPAS_12" => 45,
            "SMG_P_90" => 46,
            "SMG_Thompson" => 47,
            "SMG_UMP_40" => 48,
            "Sn_Barrett" => 49,
            "Sn_Dragunov" => 50,
            "Sn_Intervention" => 51,
            "Suppressor" => 52,
            "Syringe_Amphetamine" => 53,
            "Syringe_Androstenedione" => 54,
            "Syringe_Cannabinoid" => 55,
            "Syringe_Danazol" => 56,
            "Syringe_Glucocorticoid" => 57,
            "Syringe_Modafinil" => 58,
            "Syringe_Shard" => 59,
            "Syringe_Trenbolone" => 60,
            "Syringe_Unlabelled" => 61,
            "Thermal_Camera" => 62,
            "UHF_Radio" => 63,
            "ULF_Radio" => 64,
            "Water_Canteen" => 65,
            _ => -1,
        };
    }
    public Sprite GetSprite(string name)
    {
        return name switch
        {
            "Ammo_AR" => ItemAssets.Instance.Ammo_AR,
            "Ammo_LMG" => ItemAssets.Instance.Ammo_LMG,
            "Ammo_Pi" => ItemAssets.Instance.Ammo_Pi,
            "Ammo_Ri" => ItemAssets.Instance.Ammo_Ri,
            "Ammo_Sh" => ItemAssets.Instance.Ammo_Sh,
            "Ammo_SMG" => ItemAssets.Instance.Ammo_SMG,
            "Ammo_Sn" => ItemAssets.Instance.Ammo_Sn,
            "AR_ACOG_FAL" => ItemAssets.Instance.AR_ACOG_FAL,
            "AR_AK_47" => ItemAssets.Instance.AR_AK_47,
            "AR_M_16" => ItemAssets.Instance.AR_M_16,
            "Armour_Body" => ItemAssets.Instance.Armour_Body,
            "Armour_Exo" => ItemAssets.Instance.Armour_Exo,
            "Armour_Ghillie" => ItemAssets.Instance.Armour_Ghillie,
            "Armour_Juggernaut" => ItemAssets.Instance.Armour_Juggernaut,
            "Armour_Stimulant" => ItemAssets.Instance.Armour_Stimulant,
            "Backpack" => ItemAssets.Instance.Backpack,
            "Bag" => ItemAssets.Instance.Bag,
            "Binoculars" => ItemAssets.Instance.Binoculars,
            "Brace" => ItemAssets.Instance.Brace,
            "Claymore" => ItemAssets.Instance.Claymore,
            "Deployment_Beacon" => ItemAssets.Instance.Deployment_Beacon,
            "E_Tool" => ItemAssets.Instance.E_Tool,
            "Food_Pack" => ItemAssets.Instance.Food_Pack,
            "Grenade_Flashbang" => ItemAssets.Instance.Grenade_Flashbang,
            "Grenade_Frag" => ItemAssets.Instance.Grenade_Frag,
            "Grenade_Smoke" => ItemAssets.Instance.Grenade_Smoke,
            "Grenade_Tabun" => ItemAssets.Instance.Grenade_Tabun,
            "Knife" => ItemAssets.Instance.Knife,
            "Medkit_Large" => ItemAssets.Instance.Medkit_Large,
            "Medkit_Medium" => ItemAssets.Instance.Medkit_Medium,
            "Medkit_Small" => ItemAssets.Instance.Medkit_Small,
            "LMG_LSW" => ItemAssets.Instance.LMG_LSW,
            "LMG_M_60" => ItemAssets.Instance.LMG_M_60,
            "LMG_SAW" => ItemAssets.Instance.LMG_SAW,
            "Logistics_Belt" => ItemAssets.Instance.Logistics_Belt,
            "Pi_357" => ItemAssets.Instance.Pi_357,
            "Pi_Glock" => ItemAssets.Instance.Pi_Glock,
            "Pi_Sidearm" => ItemAssets.Instance.Pi_Sidearm,
            "Poison_Satchel" => ItemAssets.Instance.Poison_Satchel,
            "Ri_Arisaka" => ItemAssets.Instance.Ri_Arisaka,
            "Ri_Carbine" => ItemAssets.Instance.Ri_Carbine,
            "Ri_M1_Garand" => ItemAssets.Instance.Ri_M1_Garand,
            "Riot_Shield" => ItemAssets.Instance.Riot_Shield,
            "Sh_Ithaca" => ItemAssets.Instance.Sh_Ithaca,
            "Sh_Olympus" => ItemAssets.Instance.Sh_Olympus,
            "Sh_SPAS_12" => ItemAssets.Instance.Sh_SPAS_12,
            "SMG_P_90" => ItemAssets.Instance.SMG_P_90,
            "SMG_Thompson" => ItemAssets.Instance.SMG_Thompson,
            "SMG_UMP_40" => ItemAssets.Instance.SMG_UMP_40,
            "Sn_Barrett" => ItemAssets.Instance.Sn_Barrett,
            "Sn_Dragunov" => ItemAssets.Instance.Sn_Dragunov,
            "Sn_Intervention" => ItemAssets.Instance.Sn_Intervention,
            "Suppressor" => ItemAssets.Instance.Suppressor,
            "Syringe_Amphetamine" => ItemAssets.Instance.Syringe_Amphetamine,
            "Syringe_Androstenedione" => ItemAssets.Instance.Syringe_Androstenedione,
            "Syringe_Cannabinoid" => ItemAssets.Instance.Syringe_Cannabinoid,
            "Syringe_Danazol" => ItemAssets.Instance.Syringe_Danazol,
            "Syringe_Glucocorticoid" => ItemAssets.Instance.Syringe_Glucocorticoid,
            "Syringe_Modafinil" => ItemAssets.Instance.Syringe_Modafinil,
            "Syringe_Shard" => ItemAssets.Instance.Syringe_Shard,
            "Syringe_Trenbolone" => ItemAssets.Instance.Syringe_Trenbolone,
            "Syringe_Unlabelled" => ItemAssets.Instance.Syringe_Unlabelled,
            "Thermal_Camera" => ItemAssets.Instance.Thermal_Camera,
            "UHF_Radio" => ItemAssets.Instance.UHF_Radio,
            "ULF_Radio" => ItemAssets.Instance.ULF_Radio,
            _ => null,
        };
    }

    public void LoadData(GameData data)
    {
        //load basic item stats
        if (data.allItemDetails.TryGetValue(id, out details))
        {
            var tempId = id;
            itemName = (string)details["itemName"];
            Init(itemName);
            id = tempId;
            weight = System.Convert.ToInt32(details["weight"]);
            ammo = System.Convert.ToInt32(details["ammo"]);
            ablativeHealth = System.Convert.ToInt32(details["ablativeHealth"]);
            charges = System.Convert.ToInt32(details["charges"]);
            poisonedBy = (string)details["poisonedBy"];
            equippableSlots = (details["equippableSlots"] as JArray).Select(token => token.ToString()).ToList();
            whereEquipped = (string)details["whereEquipped"];

            //load position
            x = System.Convert.ToInt32(details["x"]);
            y = System.Convert.ToInt32(details["y"]);
            z = System.Convert.ToInt32(details["z"]);
            MapPhysicalPosition(x, y, z);
        }
    }

    public void SaveData(ref GameData data)
    {
        details = new()
        {
            //save basic information
            { "itemName", itemName },
            { "weight", weight },
            { "ammo", ammo },
            { "ablativeHealth", ablativeHealth },
            { "hpGranted", hpGranted },
            { "charges", charges },
            { "poisonedBy", poisonedBy },
            { "equippableSlots", equippableSlots },
            { "whereEquipped", whereEquipped },

            //save position
            { "x", x },
            { "y", y },
            { "z", z }
        };

        //add the item in
        if (data.allItemDetails.ContainsKey(id))
            data.allItemDetails.Remove(id);

        data.allItemDetails.Add(id, details);
    }

    public void RunPickupEffect(string slotName)
    {
        if (owner is Soldier owningSoldier)
        {
            //unset cover for JA wearers
            if (itemName == "Armour_Juggernaut")
                owningSoldier.UnsetCover();

            //take exo armour health
            if (itemName == "Armour_Exo")
            {
                owningSoldier.stats.H.BaseVal -= 3;
                owningSoldier.TakeDamage(null, 3, true, new List<string>() { "Exo" });
            }

            //reset sustenance for stim armour
            if (itemName == "Armour_Stimulant")
                owningSoldier.ResetRoundsWithoutFood();

            //spawn small medkit inside brace
            if (itemName == "Brace")
            {
                if (slotName == "LeftBrace")
                    owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Small"), "Misc5");
                else if (slotName == "RightBrace")
                    owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Small"), "Misc4");
            }

            //spawn med medkit in bag
            if (itemName == "Bag")
                owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Medium"), "Misc3");

            //spawn small & med medkit in backpack
            if (itemName == "Backpack")
            {
                owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Small"), "Misc1");
                owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Medium"), "Misc2");
            }

            //perform ability effects
            if (IsGun())
            {
                if (owningSoldier.IsGunner())
                {
                    //one time 1.5 bonus to max clip and ammo
                    if (gunTraits.MaxClip == gunTraits.BaseMaxClip)
                    {
                        gunTraits.MaxClip = Mathf.FloorToInt(gunTraits.MaxClip * 1.5f);
                        ammo = Mathf.FloorToInt(ammo * 1.5f);
                    }

                    //add 1 round to empty guns
                    if (ammo == 0)
                        ammo++;
                }

                if (owningSoldier.IsPlanner())
                {
                    ammo += game.DiceRoll();
                    if (ammo > gunTraits.MaxClip)
                        ammo = gunTraits.MaxClip;
                }
            }
        } 
    }
    public void RunDropEffect(string slotName)
    {
        print("running drop effect");
        if (owner is Soldier owningSoldier)
        {
            print("soldier is owner");
            //spawn small medkit inside brace
            if (itemName == "Brace")
            {
                if (slotName == "LeftBrace")
                    owningSoldier.Inventory.ConsumeItemInSlot(owningSoldier.Inventory.GetItemInSlot("Misc5"), "Misc5");
                else if (slotName == "RightBrace")
                    owningSoldier.Inventory.ConsumeItemInSlot(owningSoldier.Inventory.GetItemInSlot("Misc4"), "Misc4");
            }

            //spawn med medkit in bag
            if (itemName == "Bag")
                owningSoldier.Inventory.ConsumeItemInSlot(owningSoldier.Inventory.GetItemInSlot("Misc3"), "Misc3");

            //spawn small & med medkit in backpack
            if (itemName == "Backpack")
            {
                owningSoldier.Inventory.ConsumeItemInSlot(owningSoldier.Inventory.GetItemInSlot("Misc1"), "Misc1");
                owningSoldier.Inventory.ConsumeItemInSlot(owningSoldier.Inventory.GetItemInSlot("Misc2"), "Misc2");
            }
        }
    }
    public bool CheckAnyAmmo()
    {
        if (owner is Soldier owningSoldier)
        {
            if (ammo > 0)
                return true;
            else
            {
                owningSoldier.UnsetOverwatch();
                return false;
            }
        }
        return false;
    }

    public bool CheckSpecificAmmo(int ammo, bool fromSuppression)
    {
        if (owner is Soldier owningSoldier)
        {
            if (fromSuppression && owningSoldier.IsGunner())
                ammo--;

            if (this.ammo >= ammo)
                return true;
            else
                return false;
        }
        return false;
    }

    public void SpendSingleAmmo()
    {
        ammo--;
    }

    public void SpendSpecificAmmo(int ammo, bool fromSuppression)
    {
        if (owner is Soldier owningSoldier)
        {
            if (fromSuppression && owningSoldier.IsGunner())
                ammo--;

            this.ammo -= ammo;
        }
    }
    public int TakeAblativeDamage(int damage)
    {
        if (damage > ablativeHealth)
        {
            damage -= ablativeHealth;
            ablativeHealth = 0;
        }
        else
        {
            ablativeHealth -= damage;
            damage = 0;
        }

        return damage;
    }
    public string DisplayGunCoverDamage()
    {
        if (gunTraits.LongCovDamage.Equals(gunTraits.CQBCovDamage))
            return $"({gunTraits.CQBCovDamage})";
        else
            return $"({gunTraits.CQBCovDamage},{gunTraits.ShortCovDamage},{gunTraits.MedCovDamage},{gunTraits.LongCovDamage}-c,s,m,l)";
    }
    public void MoveItem(IHaveInventory fromOwner, string fromSlot, IHaveInventory toOwner, string toSlot)
    {
        if (fromOwner is Soldier fromOwnerSoldier)
            fromOwnerSoldier.DropItemFromSlot(this, fromSlot);
        else if (fromOwner == null) { }
        else
            fromOwner.Inventory.RemoveItem(this);

        if (toOwner is Soldier toOwnerSoldier)
            toOwnerSoldier.PickUpItemToSlot(this, toSlot);
        else if (toOwner == null) { }
        else
            toOwner.Inventory.AddItem(this);

        markedForAction = string.Empty;
    }
    public void UseULF()
    {
        if (owner is Soldier linkedSoldier)
        {
            int dip = linkedSoldier.stats.Dip.Val, elec = linkedSoldier.stats.Elec.Val;
            //locator and politician bonus
            if (linkedSoldier.IsLocator())
                elec++;
            if (linkedSoldier.IsPolitician())
                dip++;

            int avgDipElec = (dip + elec + 1) / 2;

            if (game.DiceRoll() <= avgDipElec)
            {
                menu.AddXpAlert(linkedSoldier, 2, $"{linkedSoldier.soldierName} successfully used a ULF radio.", true);
                menu.OpenULFResultUI("<color=green>Successful</color> ULF use!");
            }
            else
                menu.OpenULFResultUI("<color=red>Unsuccessful</color> ULF use.");
        }
    }
    public void UseItemInSlot(string slotName, ItemIcon linkedIcon, Item itemUsedOn)
    {
        if (owner is Soldier linkedSoldier)
        {
            switch (itemName)
            {
                case "Food_Pack":
                    linkedSoldier.roundsWithoutFood -= 10;
                    linkedSoldier.PerformLoudAction(8);
                    break;
                case "ULF_Radio":
                    UseULF();
                    linkedSoldier.PerformLoudAction(24);
                    break;
                case "Water_Canteen":
                    linkedSoldier.roundsWithoutFood -= 5;
                    weight--;
                    linkedSoldier.PerformLoudAction(8);
                    break;
                default:
                    break;
            }

            //decrement item charges and remove if consumable
            if (IsConsumable())
            {
                charges--;
                if (charges <= 0)
                {
                    linkedSoldier.Inventory.ConsumeItemInSlot(this, slotName);
                    Destroy(linkedIcon.gameObject);
                }
            }
        }
    }
    public bool IsDestructible()
    {
        if (!traits.Contains("Unbreakable"))
            return true;
        return false;
    }
    public bool IsUsable()
    {
        if (traits.Contains("Usable"))
            return true;
        return false;
    }
    public bool IsConsumable()
    {
        if (traits.Contains("Consumable"))
            return true;
        return false;
    }
    public bool IsPoisonable()
    {
        if (traits.Contains("Poisonable"))
            return true;
        return false;
    }
    public bool IsArmour()
    {
        if (traits.Contains("Armour"))
            return true;
        return false;
    }
    public bool IsGun()
    {
        if (traits.Contains("Gun"))
            return true;
        return false;
    }
    public bool IsAmmo()
    {
        if (traits.Contains("Ammo"))
            return true;
        return false;
    }
    public bool IsPoisonSatchel()
    {
        if (itemName == "Poison_Satchel")
            return true;
        return false;
    }
    public bool IsPistol()
    {
        if (IsGun())
            if (traits.Contains("Pi"))
                return true;
        return false;
    }
    public bool IsSMG()
    {
        if (IsGun())
            if (traits.Contains("SMG"))
                return true;
        return false;
    }
    public string SpecialityTag()
    {
        if (IsGun())
            return traits[1];
        return "";
    }
}
