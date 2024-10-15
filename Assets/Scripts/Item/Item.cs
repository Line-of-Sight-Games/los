using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEditor;
using TMPro;
using Newtonsoft.Json;

[System.Serializable]
public class Item : PhysicalObject, IDataPersistence, IHaveInventory
{
    public Dictionary<string, object> details;
    public ItemReader reader;
    public ItemManager itemManager;
    public IHaveInventory owner;
    public string markedForAction;

    public Sprite itemImage;
    public int itemIndex;

    public string itemName;
    public List<string> traits;
    public List<string> equippableSlots;
    public string whereEquipped;
    public int usageAP;
    public int weight;
    public int ammo;
    public int ablativeHealth;
    public int hpGranted;
    public int loudRadius;
    public int meleeDamage;
    public int charges;
    public string poisonedBy;
    public int jammingForTurns;
    public int spyingForTurns;

    public Inventory inventory;
    public List<string> inventoryList;
    public Dictionary<string, string> inventorySlots;

    public Dictionary<string, int> gunTraits;

    public BoxCollider bodyCollider;

    private Soldier linkedSoldier;

    private void Awake()
    {
        game = FindFirstObjectByType<MainGame>();
        menu = FindFirstObjectByType<MainMenu>();
        reader = FindFirstObjectByType<ItemReader>();
        itemManager = FindFirstObjectByType<ItemManager>();
    }
    private void Update()
    {
        linkedSoldier = menu.activeSoldier;

        if (owner != null) 
        {
            X = owner.X;
            Y = owner.Y;
            Z = owner.Z;
        }
    }
    public Item Init(string name)
    {
        owner = null;
        itemIndex = GetIndex(name);
        itemImage = GetSprite(name);
        //print(itemIndex);

        id = GenerateGuid();
        this.name = name;
        itemName = name;
        traits = reader.allItems.items[itemIndex].Traits;
        equippableSlots = reader.allItems.items[itemIndex].EquippableSlots;
        whereEquipped = reader.allItems.items[itemIndex].WhereEquipped;
        usageAP = reader.allItems.items[itemIndex].UsageAP;
        weight = reader.allItems.items[itemIndex].Weight;
        ammo = reader.allItems.items[itemIndex].Ammo;
        meleeDamage = reader.allItems.items[itemIndex].MeleeDamage;
        ablativeHealth = reader.allItems.items[itemIndex].AblativeHealth;
        hpGranted = reader.allItems.items[itemIndex].HPGranted;
        loudRadius = reader.allItems.items[itemIndex].LoudRadius;
        charges = reader.allItems.items[itemIndex].Charges;
        poisonedBy = reader.allItems.items[itemIndex].PoisonedBy;
        jammingForTurns = reader.allItems.items[itemIndex].JammingForTurns;
        spyingForTurns = reader.allItems.items[itemIndex].SpyingForTurns;

        if (traits.Contains("Storage"))
        {
            inventory = new Inventory(this);
            inventorySlots = reader.allItems.items[itemIndex].InventorySlots;
        }

        if (traits.Contains("Gun"))
            gunTraits = reader.allItems.items[itemIndex].GunTraits;

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
            "Ammo_Ri" => 2,
            "Ammo_Sh" => 3,
            "Ammo_SMG_Pi" => 4,
            "Ammo_Sn" => 5,
            "AR_ACOG_FAL" => 6,
            "AR_AK_47" => 7,
            "AR_M_16" => 8,
            "Armour_Body" => 9,
            "Armour_Exo" => 10,
            "Armour_Ghillie" => 11,
            "Armour_Juggernaut" => 12,
            "Armour_Stimulant" => 13,
            "Backpack" => 14,
            "Bag" => 15,
            "Binoculars" => 16,
            "Brace" => 17,
            "Claymore" => 18,
            "Deployment_Beacon" => 19,
            "E_Tool" => 20,
            "Food_Pack" => 21,
            "Grenade_Flashbang" => 22,
            "Grenade_Frag" => 23,
            "Grenade_Smoke" => 24,
            "Grenade_Tabun" => 25,
            "Knife" => 26,
            "Medikit_Large" => 27,
            "Medikit_Medium" => 28,
            "Medikit_Small" => 29,
            "LMG_LSW" => 30,
            "LMG_M_60" => 31,
            "LMG_SAW" => 32,
            "Logistics_Belt" => 33,
            "Pi_357" => 34,
            "Pi_Glock" => 35,
            "Pi_Sidearm" => 36,
            "Poison_Satchel" => 37,
            "Ri_Arisaka" => 38,
            "Ri_Carbine" => 39,
            "Ri_M1_Garand" => 40,
            "Riot_Shield" => 41,
            "Sh_Ithaca" => 42,
            "Sh_Olympus" => 43,
            "Sh_SPAS_12" => 44,
            "SMG_P_90" => 45,
            "SMG_Thompson" => 46,
            "SMG_UMP_40" => 47,
            "Sn_Barrett" => 48,
            "Sn_Dragunov" => 49,
            "Sn_Intervention" => 50,
            "Suppressor" => 51,
            "Syringe_Amphetamine" => 52,
            "Syringe_Androstenedione" => 53,
            "Syringe_Cannabinoid" => 54,
            "Syringe_Danazol" => 55,
            "Syringe_Glucocorticoid" => 56,
            "Syringe_Modafinil" => 57,
            "Syringe_Shard" => 58,
            "Syringe_Trenbolone" => 59,
            "Syringe_Unlabelled" => 60,
            "Thermal_Camera" => 61,
            "Thermal_Goggles" => 62,
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
            "Ammo_Ri" => ItemAssets.Instance.Ammo_Ri,
            "Ammo_Sh" => ItemAssets.Instance.Ammo_Sh,
            "Ammo_SMG_Pi" => ItemAssets.Instance.Ammo_SMG_Pi,
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
            "Medikit_Large" => ItemAssets.Instance.Medikit_Large,
            "Medikit_Medium" => ItemAssets.Instance.Medikit_Medium,
            "Medikit_Small" => ItemAssets.Instance.Medikit_Small,
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
            "Thermal_Goggles" => ItemAssets.Instance.Thermal_Goggles,
            "UHF_Radio" => ItemAssets.Instance.UHF_Radio,
            "ULF_Radio" => ItemAssets.Instance.ULF_Radio,
            "Water_Canteen" => ItemAssets.Instance.Water_Canteen,
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
            equippableSlots = (details["equippableSlots"] as JArray).Select(token => token.ToString()).ToList();
            whereEquipped = (string)details["whereEquipped"];
            weight = Convert.ToInt32(details["weight"]);
            ammo = Convert.ToInt32(details["ammo"]);
            ablativeHealth = Convert.ToInt32(details["ablativeHealth"]);
            charges = Convert.ToInt32(details["charges"]);
            poisonedBy = (string)details["poisonedBy"];
            jammingForTurns = Convert.ToInt32(details["jammingForTurns"]);
            spyingForTurns = Convert.ToInt32(details["spyingForTurns"]);

            //load position
            x = Convert.ToInt32(details["x"]);
            y = Convert.ToInt32(details["y"]);
            z = Convert.ToInt32(details["z"]);
            MapPhysicalPosition(x, y, z);

            if (HasInventory())
            {
                //load inventory info
                inventory = new Inventory(this);
                inventoryList = (details["inventory"] as JArray).Select(token => token.ToString()).ToList();
                inventorySlots = JsonConvert.DeserializeObject<Dictionary<string, string>>(details["inventorySlots"].ToString());
            }

            if (IsGun())
                gunTraits = JsonConvert.DeserializeObject<Dictionary<string, int>>(details["gunTraits"].ToString());

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
            { "jammingForTurns", jammingForTurns },
            { "spyingForTurns", spyingForTurns },

            //save position
            { "x", x },
            { "y", y },
            { "z", z },
        };

        if (HasInventory())
        {
            //save inventory
            details.Add("inventory", Inventory.AllItemIds);
            details.Add("inventorySlots", inventorySlots);
        }

        if (IsGun())
            details.Add("gunTraits", gunTraits);

        //add the item in
        if (data.allItemDetails.ContainsKey(id))
            data.allItemDetails.Remove(id);

        data.allItemDetails.Add(id, details);
    }
    public void RunPickupEffect(Soldier linkedSoldier)
    {
        if (linkedSoldier != null)
        {
            //rerun detection alert for thermal goggles
            if (itemName.Equals("Thermal_Goggles"))
                StartCoroutine(game.DetectionAlertSingle(linkedSoldier, "statChange(SR)|thermalEquipped", Vector3.zero, string.Empty));

            //unset cover for JA wearers
            if (IsJuggernautArmour())
            {
                //play pickup JA dialogue
                game.soundManager.PlaySoldierEquipArmour(linkedSoldier.soldierSpeciality);

                linkedSoldier.UnsetCover();
            }

            //play pickup BA dialogue
            if (IsBodyArmour())
                game.soundManager.PlaySoldierEquipArmour(linkedSoldier.soldierSpeciality);

            //play pickup UHF dialogue
            if (IsUHF())
                game.soundManager.PlaySoldierPickupUHF(linkedSoldier.soldierSpeciality);

            //play pickup ULF dialogue
            if (IsULF())
                game.soundManager.PlaySoldierPickupULF(linkedSoldier.soldierSpeciality);

            //take exo armour health
            if (itemName.Equals("Armour_Exo"))
            {
                linkedSoldier.stats.H.BaseVal -= 3;
                linkedSoldier.TakeDamage(null, 3, true, new() { "Exo" });
            }

            //reset sustenance for stim armour
            if (itemName.Equals("Armour_Stimulant"))
            {
                linkedSoldier.ResetRoundsWithoutFood();
                for (int i = 0; i < itemManager.drugTable.Length; i++)
                    linkedSoldier.UnsetOnDrug(itemManager.drugTable[i]);
                linkedSoldier.UnsetTabun();
            }

            //add ap for logistics belt
            if (itemName.Equals("Logistics_Belt"))
                linkedSoldier.ap++;

            //label unlabelled syringes
            if (linkedSoldier.IsMedic() && itemName.Equals("Syringe_Unlabelled"))
                itemName = $"Syringe_{itemManager.drugTable[HelperFunctions.RandomNumber(0, itemManager.drugTable.Length - 1)]}";

            //perform ability effects
            if (IsGun())
            {
                if (linkedSoldier.IsGunner())
                {
                    //gunner ability
                    if (!linkedSoldier.gunnerGunsBlessed.Contains(this.Id))
                    {
                        //one time 1.5 bonus to max clip
                        gunTraits["MaxClip"] = Mathf.FloorToInt(gunTraits["MaxClip"] * 1.5f);

                        //get ammo increase minimum 1
                        int ammoIncrease = Mathf.FloorToInt(ammo * 0.5f);
                        if (ammoIncrease == 0)
                            ammoIncrease = 1;

                        ammo += ammoIncrease;

                        linkedSoldier.gunnerGunsBlessed.Add(this.Id);
                    }
                }

                //planner ability
                if (linkedSoldier.IsPlanner())
                {
                    if (!linkedSoldier.plannerGunsBlessed.Contains(this.Id))
                    {
                        ammo += game.DiceRoll();
                        if (ammo > gunTraits["MaxClip"])
                            ammo = gunTraits["MaxClip"];

                        linkedSoldier.plannerGunsBlessed.Add(this.Id);
                    }
                }
            }
        }
    }
    public void RunDropEffect(Soldier linkedSoldier)
    {
        //minus ap for logistics belt
        if (itemName.Equals("Logistics_Belt"))
            linkedSoldier.DeductAP(1);
    }
    public bool CheckAnyAmmo()
    {
        if (linkedSoldier != null)
        {
            if (ammo > 0)
                return true;
            else
            {
                linkedSoldier.UnsetOverwatch();
                return false;
            }
        }
        return false;
    }
    public bool CheckSpecificAmmo(int ammo, bool fromSuppression)
    {
        if (linkedSoldier != null)
        {
            if (fromSuppression && linkedSoldier.IsGunner())
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
        if (linkedSoldier != null)
        {
            if (fromSuppression && linkedSoldier.IsGunner())
                ammo--;

            this.ammo -= ammo;
        }
    }
    public int TakeAblativeDamage(Soldier damagedBy, int damage, List<string> damageSource)
    {
        if (linkedSoldier != null)
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

            //uncon check if wearer in LS 
            if (damage == 0)
                if (linkedSoldier.IsLastStand() && !linkedSoldier.ResilienceCheck())
                    linkedSoldier.MakeUnconscious(damagedBy, damageSource);

            return damage;
        }
        return 0;
    }
    public string DisplayGunCoverDamage()
    {
        if (gunTraits["LongCovDamage"].Equals(gunTraits["CQBCovDamage"]))
            return $"({gunTraits["CQBCovDamage"]})";
        else
            return $"({gunTraits["CQBCovDamage"]},{gunTraits["ShortCovDamage"]},{gunTraits["MedCovDamage"]},{gunTraits["LongCovDamage"]}-c,s,m,l)";
    }
    public void MoveItem(IHaveInventory fromOwner, string fromSlot, IHaveInventory toOwner, string toSlot)
    {
        if (fromOwner == null) { }
        else if (fromOwner is GoodyBox goodyBox)
            goodyBox.Inventory.RemoveItem(this);
        else
            fromOwner.Inventory.RemoveItemFromSlot(this, fromSlot);

        if (toOwner == null) { }
        else if (toOwner is GoodyBox goodyBox)
            goodyBox.Inventory.AddItem(this); 
        else
            toOwner.Inventory.AddItemToSlot(this, toSlot);

        markedForAction = string.Empty;
    }
    public List<string> GetUHFStrikes()
    {
        if (linkedSoldier != null)
        {
            int dip = linkedSoldier.stats.Dip.Val, elec = linkedSoldier.stats.Elec.Val;
            //locator and politician bonus
            if (linkedSoldier.IsLocator())
                elec++;
            if (linkedSoldier.IsPolitician())
                dip++;

            //keep values within bounds
            if (dip > 9)
                dip = 9;
            if (elec > 9)
                elec = 9;

            int dipelecScore = itemManager.scoreTable[dip, elec];
            List<string> strike = itemManager.GetStrikeAndLowerNames(dipelecScore);
            return strike;
        }
        return null;
    }
    public void UseULF(string effect)
    {
        if (linkedSoldier != null)
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
                if (effect.Equals("spy"))
                {
                    SetSpying();
                    menu.OpenULFResultUI("<color=green>Spying successful!</color>");
                }
                else
                {
                    SetJamming();
                    menu.OpenULFResultUI("<color=green>Jamming successful!</color>");
                }
                menu.AddXpAlert(linkedSoldier, 2, $"{linkedSoldier.soldierName} successfully used a ULF radio to {effect}.", true);
            }
            else
                menu.OpenULFResultUI("<color=red>Unsuccessful</color> ULF use.");
        }
    }
    public void UseItem(ItemIcon linkedIcon, Item itemUsedOn, Soldier soldierUsedOn)
    {
        if (linkedSoldier != null)
        {
            switch (itemName)
            {
                case "E_Tool":
                    print("tried to use etool");
                    menu.OpenEtoolResultUI();
                    break;
                case "Ammo_AR":
                case "Ammo_LMG":
                case "Ammo_Ri":
                case "Ammo_Sh":
                case "Ammo_SMG_Pi":
                case "Ammo_Sn":
                    (this.ammo, itemUsedOn.ammo) = (itemUsedOn.ammo, this.ammo);
                    if (this.ammo != 0)
                        charges++; //add a "charge" to ensure ammo is not deleted
                    break;
                case "Food_Pack":
                    if (poisonedBy == null || poisonedBy == "")
                        linkedSoldier.roundsWithoutFood -= 10;
                    else
                        linkedSoldier.TakePoisoning(poisonedBy, true);
                    break;
                case "Medikit_Small":
                case "Medikit_Medium":
                case "Medikit_Large":
                    if (poisonedBy == null || poisonedBy == "")
                    {
                        //play heal ally dialogue
                        if (linkedSoldier != soldierUsedOn) //only if it's an ally you're healing
                            game.soundManager.PlaySoldierHealAlly(linkedSoldier.soldierSpeciality);

                        soldierUsedOn.TakeHeal(linkedSoldier, hpGranted + linkedSoldier.stats.Heal.Val, linkedSoldier.stats.Heal.Val, false, false);
                    }
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Poison_Satchel":
                    itemUsedOn.poisonedBy = linkedSoldier.Id;
                    break;
                case "Syringe_Amphetamine":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Amphetamine", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Androstenedione":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Androstenedione", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Cannabinoid":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Cannabinoid", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Danazol":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Danazol", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Glucocorticoid":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Glucocorticoid", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Modafinil":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Modafinil", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Shard":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Shard", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Trenbolone":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug("Trenbolone", linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Syringe_Unlabelled":
                    if (poisonedBy == null || poisonedBy == "")
                        soldierUsedOn.TakeDrug(itemManager.drugTable[HelperFunctions.RandomNumber(0, itemManager.drugTable.Length - 1)], linkedSoldier);
                    else
                        soldierUsedOn.TakePoisoning(poisonedBy, true);
                    break;
                case "Water_Canteen":
                    if (poisonedBy == null || poisonedBy == "")
                        linkedSoldier.roundsWithoutFood -= 5;
                    else
                        linkedSoldier.TakePoisoning(poisonedBy, true);
                    weight--;
                    break;
                default:
                    break;
            }

            //perform loud action
            linkedSoldier.PerformLoudAction(loudRadius);

            //decrement item charges and remove if consumable
            if (IsConsumable())
            {
                charges--;
                if (charges <= 0)
                {
                    ConsumeItem();
                    Destroy(linkedIcon.gameObject);
                }
            }
        }
    }
    public void CheckExplosionGrenade(Soldier explodedBy, Vector3 position)
    {
        GameObject explosionList = Instantiate(menu.explosionListPrefab, menu.explosionUI.transform).GetComponent<ExplosionList>().Init($"{itemName} | Detonated: {position.x},{position.y},{position.z}").gameObject;
        explosionList.transform.Find("ExplodedBy").GetComponent<TextMeshProUGUI>().text = explodedBy.Id;

        if (IsFrag())
        {
            foreach (PhysicalObject obj in FindObjectsByType<PhysicalObject>(default))
            {
                int damage = 0;
                if (obj.PhysicalObjectWithinRadius(position, 3))
                    damage = 8;
                else if (obj.PhysicalObjectWithinRadius(position, 8))
                    damage = 4;
                else if (obj.PhysicalObjectWithinRadius(position, 15))
                    damage = 2;

                if (damage > 0)
                {
                    if (obj is Item hitItem)
                        menu.AddExplosionAlertItem(explosionList, hitItem, position, explodedBy, damage);
                    else if (obj is POI hitPoi)
                        menu.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, damage);
                    else if (obj is Soldier hitSoldier)
                        menu.AddExplosionAlert(explosionList, hitSoldier, position, explodedBy, damage, 1);
                }
            }
        }
        else if (IsFlashbang())
        {
            foreach (PhysicalObject obj in FindObjectsByType<PhysicalObject>(default))
            {
                int damage = 0, stun = 0;
                if (obj.PhysicalObjectWithinRadius(position, 0))
                {
                    damage = game.DiceRoll();
                    stun = 4;
                }
                else if (obj.PhysicalObjectWithinRadius(position, 3))
                    stun = 4;
                else if (obj.PhysicalObjectWithinRadius(position, 8))
                    stun = 3;
                else if (obj.PhysicalObjectWithinRadius(position, 15))
                    stun = 2;

                if (stun > 0 || damage > 0)
                {
                    if (obj is Soldier hitSoldier)
                    {
                        //calculate final damage
                        damage -= hitSoldier.stats.R.Val;
                        if (damage < 0)
                            damage = 0;

                        menu.AddExplosionAlert(explosionList, hitSoldier, position, explodedBy, damage, stun);
                    }
                    else if (obj is POI hitPoi && damage > 0)
                        menu.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, damage);
                    else if (obj is Item hitItem && damage > 0)
                        menu.AddExplosionAlertItem(explosionList, hitItem, position, explodedBy, damage);
                }
            }
        }
        else if (IsSmoke())
            Instantiate(game.poiManager.smokeCloudPrefab).Init(Tuple.Create(new Vector3(position.x, position.y, position.z), string.Empty), explodedBy.Id);
        else if (IsTabun())
            Instantiate(game.poiManager.tabunCloudPrefab).Init(Tuple.Create(new Vector3(position.x, position.y, position.z), string.Empty), explodedBy.Id);

        //show explosion ui
        menu.OpenExplosionUI();

        DestroyItem(explodedBy);
    }
    public void ConsumeItem()
    {
        owner?.Inventory.RemoveItemFromSlot(this, whereEquipped);
        itemManager.DestroyItem(this);
    }
    public void DamageItem(Soldier damagedBy, int damage)
    {
        if ((damage >= 5 && IsBreakable()) || (damage > 0 && IsFragile()))
            DestroyItem(damagedBy);
    }
    public void DestroyItem(Soldier destroyedBy)
    {
        if (owner != null && owner is Soldier linkedSoldier)
            menu.AddDamageAlert(linkedSoldier, $"{linkedSoldier.soldierName} had {this.itemName} ({this.X},{this.Y},{this.Z}) destroyed.", false, true);

        ConsumeItem();
    }
    public bool IsCatchable()
    {
        if (equippableSlots.Contains("Hand"))
            return true;
        return false;
    }
    public bool IsBreakable()
    {
        if (!traits.Contains("Unbreakable"))
            return true;
        return false;
    }
    public bool IsNestedInGoodyBox()
    {
        if (owner != null)
        {
            if (owner is GoodyBox)
                return true;
            else if (owner is Item owningItem)
            {
                if (owningItem.owner is GoodyBox)
                    return true;
            }
        }
        return false;
    }
    public bool IsNestedOnSoldier()
    {
        if (owner != null)
        {
            if (owner is Soldier)
                return true;
            else if (owner is Item owningItem)
            {
                if (owningItem.owner is Soldier)
                    return true;
            }
        }
        return false;
    }
    public Soldier SoldierNestedOn()
    {
        if (owner != null)
        {
            if (owner is Soldier owningSoldier)
                return owningSoldier;
            else if (owner is Item owningItem)
            {
                if (owningItem.owner is Soldier owningSoldier2)
                    return owningSoldier2;
            }
        }
        return null;
    }
    public bool IsFragile()
    {
        if (traits.Contains("Fragile") && !(owner != null && owner is Item parentItem && parentItem.IsProtecting()))
            return true;
        return false;
    }
    public bool IsUnbreakable()
    {
        if (traits.Contains("Unbreakable"))
            return true;
        return false;
    }
    public bool IsUsable()
    {
        if (traits.Contains("Usable"))
            return true;
        return false;
    }
    public bool IsProtecting()
    {
        if (traits.Contains("Protecting"))
            return true;
        return false;
    }
    public bool IsConsumable()
    {
        if (traits.Contains("Consumable"))
            return true;
        return false;
    }
    public bool IsTriggered()
    {
        if (id.Contains("t"))
            return true;
        return false;
    }
    public void SetTriggered()
    {
        print($"Setting triggered {itemName} ({X},{Y},{Z}) | {id}");
        if (!IsTriggered())
            id = $"t{id}";
    }
    public bool IsPoisonable()
    {
        if (traits.Contains("Poisonable"))
            return true;
        return false;
    }
    public bool IsThrowable()
    {
        if (traits.Contains("Throwable"))
            return true;
        return false;
    }
    public bool IsArmour()
    {
        if (traits.Contains("Armour"))
            return true;
        return false;
    }
    public bool HasInventory()
    {
        if (traits.Contains("Storage"))
            return true;
        return false;
    }
    public bool IsGun()
    {
        if (traits.Contains("Gun"))
            return true;
        return false;
    }
    public bool IsGrenade()
    {
        if (name.Contains("Grenade"))
            return true;
        return false;
    }
    public bool IsClaymore()
    {
        if (name.Contains("Claymore"))
            return true;
        return false;
    }
    public bool IsFrag()
    {
        if (IsGrenade() && name.Contains("Frag"))
            return true;
        return false;
    }
    public bool IsFlashbang()
    {
        if (IsGrenade() && name.Contains("Flashbang"))
            return true;
        return false;
    }
    public bool IsSmoke()
    {
        if (IsGrenade() && name.Contains("Smoke"))
            return true;
        return false;
    }
    public bool IsTabun()
    {
        if (IsGrenade() && name.Contains("Tabun"))
            return true;
        return false;
    }
    public bool IsRiotShield()
    {
        if (name.Equals("Riot_Shield"))
            return true;
        return false;
    }
    public bool IsLargeMedikit()
    {
        if (name.Equals("Medikit_Large"))
            return true;
        return false;
    }
    public bool IsWeapon()
    {
        if (traits.Contains("Weapon"))
            return true;
        return false;
    }
    public bool IsAssaultRifle()
    {
        if (IsGun() && traits.Contains("Assault Rifle"))
            return true;
        return false;
    }
    public bool IsLMG()
    {
        if (IsGun() && traits.Contains("Light Machine Gun"))
            return true;
        return false;
    }
    public bool IsPistol()
    {
        if (IsGun() && traits.Contains("Pistol"))
            return true;
        return false;
    }
    public bool IsRifle()
    {
        if (IsGun() && traits.Contains("Rifle"))
            return true;
        return false;
    }
    public bool IsShotgun()
    {
        if (IsGun() && traits.Contains("Shotgun"))
            return true;
        return false;
    }
    public bool IsSMG()
    {
        if (IsGun() && traits.Contains("Sub-Machine Gun"))
            return true;
        return false;
    }
    public bool IsSniper()
    {
        if (IsGun() && traits.Contains("Sniper Rifle"))
            return true;
        return false;
    }
    public bool SuppressorAttached()
    {
        if (IsGun() && false)
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
        if (itemName.Equals("Poison_Satchel"))
            return true;
        return false;
    }
    public bool IsUHF()
    {
        if (itemName.Equals("UHF_Radio"))
            return true;
        return false;
    }
    public bool IsULF()
    {
        if (itemName.Equals("ULF_Radio"))
            return true;
        return false;
    }
    public void SetSpying()
    {
        spyingForTurns = 2;
    }
    public bool IsSpying()
    {
        if (IsULF() && spyingForTurns > 0)
            return true;
        return false;
    }
    public void SetJamming()
    {
        jammingForTurns = 2;
    }
    public bool IsJamming()
    {
        if (IsULF() && jammingForTurns > 0)
            return true;
        return false;
    }
    public bool IsJammed()
    {
        if (this.owner is Soldier owningSoldier)
        {
            foreach (Item i in itemManager.allItems)
                if (i.IsJamming() && i.owner is Soldier linkedSoldier && linkedSoldier.IsOppositeTeamAs(owningSoldier))
                    return true;
        }
        return false;
    }
    public bool IsBackpack()
    {
        if (name.Contains("Backpack"))
            return true;
        return false;
    }
    public bool IsBag()
    {
        if (name.Contains("Bag"))
            return true;
        return false;
    }
    public bool IsBrace()
    {
        if (name.Contains("Brace"))
            return true;
        return false;
    }
    public bool IsBodyArmour()
    {
        if (name.Contains("Armour_Body"))
            return true;
        return false;
    }
    public bool IsJuggernautArmour()
    {
        if (name.Contains("Armour_Juggernaut"))
            return true;
        return false;
    }
    public string SpecialityTag()
    {
        if (IsGun())
            return traits[1];
        return "";
    }













    public Inventory Inventory { get { return inventory; } }
    public GameObject GameObject { get { return gameObject; } }
    public List<string> InventoryList { get { return inventoryList; } }
    public Dictionary<string, string> InventorySlots { get { return inventorySlots; } }
}
