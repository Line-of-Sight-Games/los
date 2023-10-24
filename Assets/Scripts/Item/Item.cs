using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

[System.Serializable]
public class Item : PhysicalObject, IDataPersistence
{
    public enum ItemType
    {
        Ammo_AR,
        Ammo_LMG,
        Ammo_Pi,
        Ammo_Ri,
        Ammo_Sh,
        Ammo_SMG,
        Ammo_Sn,
        AR_ACOG_FAL,
        AR_AK_47,
        AR_M_16,
        Armour_Body,
        Armour_Exo,
        Armour_Ghillie,
        Armour_Juggernaut,
        Armour_Stimulant,
        Backpack,
        Bag,
        Binoculars,
        Brace,
        Claymore,
        Deployment_Beacon,
        E_Tool,
        Food_Pack,
        Grenade_Flashbang,
        Grenade_Frag,
        Grenade_Smoke,
        Grenade_Tabun,
        Knife,
        Medkit_Large,
        Medkit_Medium,
        Medkit_Small,
        LMG_LSW,
        LMG_M_60,
        LMG_SAW,
        Logistics_Belt,
        Pi_357,
        Pi_Glock,
        Pi_Sidearm,
        Poison_Satchel,
        Ri_Arisaka,
        Ri_Carbine,
        Ri_M1_Garand,
        Riot_Shield,
        Sh_Ithaca,
        Sh_Olympus,
        Sh_SPAS_12,
        SMG_P_90,
        SMG_Thompson,
        SMG_UMP_40,
        Sn_Barrett,
        Sn_Dragunov,
        Sn_Intervention,
        Suppressor,
        Syringe_Amphetamine,
        Syringe_Androstenedione,
        Syringe_Cannabinoid,
        Syringe_Danazol,
        Syringe_Glucocorticoid,
        Syringe_Modafinil,
        Syringe_Shard,
        Syringe_Trenbolone,
        Syringe_Unlabelled,
        Thermal_Camera,
        UHF_Radio,
        ULF_Radio,
        Water_Canteen,
    }

    public Dictionary<string, ItemType> stringToItemType = new ()
    {
        { ItemType.Ammo_AR.ToString(), ItemType.Ammo_AR },
        { ItemType.Ammo_LMG.ToString(), ItemType.Ammo_LMG },
        { ItemType.Ammo_Pi.ToString(), ItemType.Ammo_Pi },
        { ItemType.Ammo_Ri.ToString(), ItemType.Ammo_Ri },
        { ItemType.Ammo_Sh.ToString(), ItemType.Ammo_Sh },
        { ItemType.Ammo_SMG.ToString(), ItemType.Ammo_SMG },
        { ItemType.Ammo_Sn.ToString(), ItemType.Ammo_Sn },
        { ItemType.AR_ACOG_FAL.ToString(), ItemType.AR_ACOG_FAL },
        { ItemType.AR_AK_47.ToString(), ItemType.AR_AK_47 },
        { ItemType.AR_M_16.ToString(), ItemType.AR_M_16 },
        { ItemType.Armour_Body.ToString(), ItemType.Armour_Body },
        { ItemType.Armour_Exo.ToString(), ItemType.Armour_Exo },
        { ItemType.Armour_Ghillie.ToString(), ItemType.Armour_Ghillie },
        { ItemType.Armour_Juggernaut.ToString(), ItemType.Armour_Juggernaut },
        { ItemType.Armour_Stimulant.ToString(), ItemType.Armour_Stimulant },
        { ItemType.Backpack.ToString(), ItemType.Backpack },
        { ItemType.Bag.ToString(), ItemType.Bag },
        { ItemType.Binoculars.ToString(), ItemType.Binoculars },
        { ItemType.Brace.ToString(), ItemType.Brace },
        { ItemType.Claymore.ToString(), ItemType.Claymore },
        { ItemType.Deployment_Beacon.ToString(), ItemType.Deployment_Beacon },
        { ItemType.E_Tool.ToString(), ItemType.E_Tool },
        { ItemType.Food_Pack.ToString(), ItemType.Food_Pack },
        { ItemType.Grenade_Flashbang.ToString(), ItemType.Grenade_Flashbang },
        { ItemType.Grenade_Frag.ToString(), ItemType.Grenade_Frag },
        { ItemType.Grenade_Smoke.ToString(), ItemType.Grenade_Smoke },
        { ItemType.Grenade_Tabun.ToString(), ItemType.Grenade_Tabun },
        { ItemType.Knife.ToString(), ItemType.Knife },
        { ItemType.Medkit_Large.ToString(), ItemType.Medkit_Large },
        { ItemType.Medkit_Medium.ToString(), ItemType.Medkit_Medium },
        { ItemType.Medkit_Small.ToString(), ItemType.Medkit_Small },
        { ItemType.LMG_LSW.ToString(), ItemType.LMG_LSW },
        { ItemType.LMG_M_60.ToString(), ItemType.LMG_M_60 },
        { ItemType.LMG_SAW.ToString(), ItemType.LMG_SAW },
        { ItemType.Logistics_Belt.ToString(), ItemType.Logistics_Belt },
        { ItemType.Pi_357.ToString(), ItemType.Pi_357 },
        { ItemType.Pi_Glock.ToString(), ItemType.Pi_Glock },
        { ItemType.Pi_Sidearm.ToString(), ItemType.Pi_Sidearm },
        { ItemType.Poison_Satchel.ToString(), ItemType.Poison_Satchel },
        { ItemType.Ri_Arisaka.ToString(), ItemType.Ri_Arisaka },
        { ItemType.Ri_Carbine.ToString(), ItemType.Ri_Carbine },
        { ItemType.Ri_M1_Garand.ToString(), ItemType.Ri_M1_Garand },
        { ItemType.Riot_Shield.ToString(), ItemType.Riot_Shield },
        { ItemType.Sh_Ithaca.ToString(), ItemType.Sh_Ithaca },
        { ItemType.Sh_Olympus.ToString(), ItemType.Sh_Olympus },
        { ItemType.Sh_SPAS_12.ToString(), ItemType.Sh_SPAS_12 },
        { ItemType.SMG_P_90.ToString(), ItemType.SMG_P_90 },
        { ItemType.SMG_Thompson.ToString(), ItemType.SMG_Thompson },
        { ItemType.SMG_UMP_40.ToString(), ItemType.SMG_UMP_40 },
        { ItemType.Sn_Barrett.ToString(), ItemType.Sn_Barrett },
        { ItemType.Sn_Dragunov.ToString(), ItemType.Sn_Dragunov },
        { ItemType.Sn_Intervention.ToString(), ItemType.Sn_Intervention },
        { ItemType.Suppressor.ToString(), ItemType.Suppressor },
        { ItemType.Syringe_Amphetamine.ToString(), ItemType.Syringe_Amphetamine },
        { ItemType.Syringe_Androstenedione.ToString(), ItemType.Syringe_Androstenedione },
        { ItemType.Syringe_Cannabinoid.ToString(), ItemType.Syringe_Cannabinoid },
        { ItemType.Syringe_Danazol.ToString(), ItemType.Syringe_Danazol },
        { ItemType.Syringe_Glucocorticoid.ToString(), ItemType.Syringe_Glucocorticoid },
        { ItemType.Syringe_Modafinil.ToString(), ItemType.Syringe_Modafinil },
        { ItemType.Syringe_Shard.ToString(), ItemType.Syringe_Shard },
        { ItemType.Syringe_Trenbolone.ToString(), ItemType.Syringe_Trenbolone },
        { ItemType.Syringe_Unlabelled.ToString(), ItemType.Syringe_Unlabelled },
        { ItemType.Thermal_Camera.ToString(), ItemType.Thermal_Camera },
        { ItemType.UHF_Radio.ToString(), ItemType.UHF_Radio },
        { ItemType.ULF_Radio.ToString(), ItemType.ULF_Radio },
        { ItemType.Water_Canteen.ToString(), ItemType.Water_Canteen },
    };
    public Dictionary<string, object> details;
    public MainGame game;
    public MainMenu menu; 
    public ItemReader reader;
    public ItemManager itemManager;
    public IHaveInventory owner;

    public ItemType itemType;
    public Sprite itemImage;

    public string itemName;
    public int weight;
    public int ammo;
    public string whereEquipped;
    public bool isUsable;
    public bool isConsumable;
    public bool isFragile;
    public bool isUnbreakable;
    public int ablativeHealth;
    public int hpGranted;
    public int loudRadius;
    public int meleeDamage;
    public int charges;
    public bool isPoisonable;
    public string poisonedBy;
    public bool isShareable;
    public bool isTradeable;
    public List<string> equippableSlots;
    public string blockedByLateral;
    public string blockedByLeftBrace;
    public string blockedByPosterior;
    public string blockedByBackHole;
    public string blockedByFullBody;

    public string gunType;
    public int gunMaxClip;
    public int gunBaseMaxClip;
    public int gunDamage;
    public int gunCritDamage;
    public int gunCQBU;
    public int gunCQBA;
    public int gunShortU;
    public int gunShortA;
    public int gunMedU;
    public int gunMedA;
    public int gunLongU;
    public int gunLongA;
    public int gunCoriolisU;
    public int gunCoriolisA;
    public int gunSuppressionDrain;
    public int gunCQBSuppressionPenalty;
    public int gunShortSuppressionPenalty;
    public int gunMedSuppressionPenalty;
    public int gunLongSuppressionPenalty;
    public int gunCQBCoverDamage;
    public int gunShortCoverDamage;
    public int gunMedCoverDamage;
    public int gunLongCoverDamage;

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
        id = GenerateGuid();
        itemName = name;
        stringToItemType.TryGetValue(name, out itemType);
        itemImage = GetSprite();
        var itemIndex = GetIndex();
        weight = reader.allItems.items[itemIndex].Weight;
        ammo = reader.allItems.items[itemIndex].Ammo;
        whereEquipped = reader.allItems.items[itemIndex].WhereEquipped;
        isUsable = reader.allItems.items[itemIndex].Usable;
        isConsumable = reader.allItems.items[itemIndex].Consumable;
        isFragile = reader.allItems.items[itemIndex].Fragile;
        isFragile = reader.allItems.items[itemIndex].Fragile;
        isUnbreakable = reader.allItems.items[itemIndex].Unbreakable;
        ablativeHealth = reader.allItems.items[itemIndex].AblativeHealth;
        hpGranted = reader.allItems.items[itemIndex].HPGranted;
        loudRadius = reader.allItems.items[itemIndex].LoudRadius;
        meleeDamage = reader.allItems.items[itemIndex].MeleeDamage;
        charges = reader.allItems.items[itemIndex].Charges;
        isPoisonable = reader.allItems.items[itemIndex].Poisonable;
        poisonedBy = reader.allItems.items[itemIndex].PoisonedBy;
        isShareable = reader.allItems.items[itemIndex].Shareable;
        isTradeable = reader.allItems.items[itemIndex].Tradeable;
        equippableSlots = reader.allItems.items[itemIndex].EquippableSlots;
        blockedByLateral = reader.allItems.items[itemIndex].BlockedByLateral;
        blockedByLeftBrace = reader.allItems.items[itemIndex].BlockedByLeftBrace;
        blockedByPosterior = reader.allItems.items[itemIndex].BlockedByPosterior;
        blockedByBackHole = reader.allItems.items[itemIndex].BlockedByBackHole;
        blockedByFullBody = reader.allItems.items[itemIndex].BlockedByFullBody;

        gunType = reader.allItems.items[itemIndex].Type;
        if (gunType != null)
        {
            gunMaxClip = reader.allItems.items[itemIndex].Max_Clip;
            gunBaseMaxClip = reader.allItems.items[itemIndex].Base_Max_Clip;
            gunDamage = reader.allItems.items[itemIndex].Damage;
            gunCritDamage = reader.allItems.items[itemIndex].Crit_Damage;
            gunCQBU = reader.allItems.items[itemIndex].CQB_U;
            gunCQBA = reader.allItems.items[itemIndex].CQB_A;
            gunShortU = reader.allItems.items[itemIndex].Short_U;
            gunShortA = reader.allItems.items[itemIndex].Short_A;
            gunMedU = reader.allItems.items[itemIndex].Med_U;
            gunMedA = reader.allItems.items[itemIndex].Med_A;
            gunLongU = reader.allItems.items[itemIndex].Long_U;
            gunLongA = reader.allItems.items[itemIndex].Long_A;
            gunCoriolisU = reader.allItems.items[itemIndex].Coriolis_U;
            gunCoriolisA = reader.allItems.items[itemIndex].Coriolis_A;
            gunSuppressionDrain = reader.allItems.items[itemIndex].Suppress_Drain;
            gunCQBSuppressionPenalty = reader.allItems.items[itemIndex].CQB_Sup_Pen;
            gunShortSuppressionPenalty = reader.allItems.items[itemIndex].Short_Sup_Pen;
            gunMedSuppressionPenalty = reader.allItems.items[itemIndex].Med_Sup_Pen;
            gunLongSuppressionPenalty = reader.allItems.items[itemIndex].Long_Sup_Pen;
            gunCQBCoverDamage = reader.allItems.items[itemIndex].CQB_Cov_Damage;
            gunShortCoverDamage = reader.allItems.items[itemIndex].Short_Cov_Damage;
            gunMedCoverDamage = reader.allItems.items[itemIndex].Med_Cov_Damage;
            gunLongCoverDamage = reader.allItems.items[itemIndex].Long_Cov_Damage;
        }

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

    public int GetIndex()
    {
        return itemType switch
        {
            ItemType.Ammo_AR => 0,
            ItemType.Ammo_LMG => 1,
            ItemType.Ammo_Pi => 2,
            ItemType.Ammo_Ri => 3,
            ItemType.Ammo_Sh => 4,
            ItemType.Ammo_SMG => 5,
            ItemType.Ammo_Sn => 6,
            ItemType.AR_ACOG_FAL => 7,
            ItemType.AR_AK_47 => 8,
            ItemType.AR_M_16 => 9,
            ItemType.Armour_Body => 10,
            ItemType.Armour_Exo => 11,
            ItemType.Armour_Ghillie => 12,
            ItemType.Armour_Juggernaut => 13,
            ItemType.Armour_Stimulant => 14,
            ItemType.Backpack => 15,
            ItemType.Bag => 16,
            ItemType.Binoculars => 17,
            ItemType.Brace => 18,
            ItemType.Claymore => 19,
            ItemType.Deployment_Beacon => 20,
            ItemType.E_Tool => 21,
            ItemType.Food_Pack => 22,
            ItemType.Grenade_Flashbang => 23,
            ItemType.Grenade_Frag => 24,
            ItemType.Grenade_Smoke => 25,
            ItemType.Grenade_Tabun => 26,
            ItemType.Knife => 27,
            ItemType.Medkit_Large => 28,
            ItemType.Medkit_Medium => 29,
            ItemType.Medkit_Small => 30,
            ItemType.LMG_LSW => 31,
            ItemType.LMG_M_60 => 32,
            ItemType.LMG_SAW => 33,
            ItemType.Logistics_Belt => 34,
            ItemType.Pi_357 => 35,
            ItemType.Pi_Glock => 36,
            ItemType.Pi_Sidearm => 37,
            ItemType.Poison_Satchel => 38,
            ItemType.Ri_Arisaka => 39,
            ItemType.Ri_Carbine => 40,
            ItemType.Ri_M1_Garand => 41,
            ItemType.Riot_Shield => 42,
            ItemType.Sh_Ithaca => 43,
            ItemType.Sh_Olympus => 44,
            ItemType.Sh_SPAS_12 => 45,
            ItemType.SMG_P_90 => 46,
            ItemType.SMG_Thompson => 47,
            ItemType.SMG_UMP_40 => 48,
            ItemType.Sn_Barrett => 49,
            ItemType.Sn_Dragunov => 50,
            ItemType.Sn_Intervention => 51,
            ItemType.Suppressor => 52,
            ItemType.Syringe_Amphetamine => 53,
            ItemType.Syringe_Androstenedione => 54,
            ItemType.Syringe_Cannabinoid => 55,
            ItemType.Syringe_Danazol => 56,
            ItemType.Syringe_Glucocorticoid => 57,
            ItemType.Syringe_Modafinil => 58,
            ItemType.Syringe_Shard => 59,
            ItemType.Syringe_Trenbolone => 60,
            ItemType.Syringe_Unlabelled => 61,
            ItemType.Thermal_Camera => 62,
            ItemType.UHF_Radio => 63,
            ItemType.ULF_Radio => 64,
            ItemType.Water_Canteen => 65,
            _ => -1,
        };
    }
    public Sprite GetSprite()
    {
        return itemType switch
        {
            ItemType.Ammo_AR => ItemAssets.Instance.Ammo_AR,
            ItemType.Ammo_LMG => ItemAssets.Instance.Ammo_LMG,
            ItemType.Ammo_Pi => ItemAssets.Instance.Ammo_Pi,
            ItemType.Ammo_Ri => ItemAssets.Instance.Ammo_Ri,
            ItemType.Ammo_Sh => ItemAssets.Instance.Ammo_Sh,
            ItemType.Ammo_SMG => ItemAssets.Instance.Ammo_SMG,
            ItemType.Ammo_Sn => ItemAssets.Instance.Ammo_Sn,
            ItemType.AR_ACOG_FAL => ItemAssets.Instance.AR_ACOG_FAL,
            ItemType.AR_AK_47 => ItemAssets.Instance.AR_AK_47,
            ItemType.AR_M_16 => ItemAssets.Instance.AR_M_16,
            ItemType.Armour_Body => ItemAssets.Instance.Armour_Body,
            ItemType.Armour_Exo => ItemAssets.Instance.Armour_Exo,
            ItemType.Armour_Ghillie => ItemAssets.Instance.Armour_Ghillie,
            ItemType.Armour_Juggernaut => ItemAssets.Instance.Armour_Juggernaut,
            ItemType.Armour_Stimulant => ItemAssets.Instance.Armour_Stimulant,
            ItemType.Backpack => ItemAssets.Instance.Backpack,
            ItemType.Bag => ItemAssets.Instance.Bag,
            ItemType.Binoculars => ItemAssets.Instance.Binoculars,
            ItemType.Brace => ItemAssets.Instance.Brace,
            ItemType.Claymore => ItemAssets.Instance.Claymore,
            ItemType.Deployment_Beacon => ItemAssets.Instance.Deployment_Beacon,
            ItemType.E_Tool => ItemAssets.Instance.E_Tool,
            ItemType.Food_Pack => ItemAssets.Instance.Food_Pack,
            ItemType.Grenade_Flashbang => ItemAssets.Instance.Grenade_Flashbang,
            ItemType.Grenade_Frag => ItemAssets.Instance.Grenade_Frag,
            ItemType.Grenade_Smoke => ItemAssets.Instance.Grenade_Smoke,
            ItemType.Grenade_Tabun => ItemAssets.Instance.Grenade_Tabun,
            ItemType.Knife => ItemAssets.Instance.Knife,
            ItemType.Medkit_Large => ItemAssets.Instance.Medkit_Large,
            ItemType.Medkit_Medium => ItemAssets.Instance.Medkit_Medium,
            ItemType.Medkit_Small => ItemAssets.Instance.Medkit_Small,
            ItemType.LMG_LSW => ItemAssets.Instance.LMG_LSW,
            ItemType.LMG_M_60 => ItemAssets.Instance.LMG_M_60,
            ItemType.LMG_SAW => ItemAssets.Instance.LMG_SAW,
            ItemType.Logistics_Belt => ItemAssets.Instance.Logistics_Belt,
            ItemType.Pi_357 => ItemAssets.Instance.Pi_357,
            ItemType.Pi_Glock => ItemAssets.Instance.Pi_Glock,
            ItemType.Pi_Sidearm => ItemAssets.Instance.Pi_Sidearm,
            ItemType.Poison_Satchel => ItemAssets.Instance.Poison_Satchel,
            ItemType.Ri_Arisaka => ItemAssets.Instance.Ri_Arisaka,
            ItemType.Ri_Carbine => ItemAssets.Instance.Ri_Carbine,
            ItemType.Ri_M1_Garand => ItemAssets.Instance.Ri_M1_Garand,
            ItemType.Riot_Shield => ItemAssets.Instance.Riot_Shield,
            ItemType.Sh_Ithaca => ItemAssets.Instance.Sh_Ithaca,
            ItemType.Sh_Olympus => ItemAssets.Instance.Sh_Olympus,
            ItemType.Sh_SPAS_12 => ItemAssets.Instance.Sh_SPAS_12,
            ItemType.SMG_P_90 => ItemAssets.Instance.SMG_P_90,
            ItemType.SMG_Thompson => ItemAssets.Instance.SMG_Thompson,
            ItemType.SMG_UMP_40 => ItemAssets.Instance.SMG_UMP_40,
            ItemType.Sn_Barrett => ItemAssets.Instance.Sn_Barrett,
            ItemType.Sn_Dragunov => ItemAssets.Instance.Sn_Dragunov,
            ItemType.Sn_Intervention => ItemAssets.Instance.Sn_Intervention,
            ItemType.Suppressor => ItemAssets.Instance.Suppressor,
            ItemType.Syringe_Amphetamine => ItemAssets.Instance.Syringe_Amphetamine,
            ItemType.Syringe_Androstenedione => ItemAssets.Instance.Syringe_Androstenedione,
            ItemType.Syringe_Cannabinoid => ItemAssets.Instance.Syringe_Cannabinoid,
            ItemType.Syringe_Danazol => ItemAssets.Instance.Syringe_Danazol,
            ItemType.Syringe_Glucocorticoid => ItemAssets.Instance.Syringe_Glucocorticoid,
            ItemType.Syringe_Modafinil => ItemAssets.Instance.Syringe_Modafinil,
            ItemType.Syringe_Shard => ItemAssets.Instance.Syringe_Shard,
            ItemType.Syringe_Trenbolone => ItemAssets.Instance.Syringe_Trenbolone,
            ItemType.Syringe_Unlabelled => ItemAssets.Instance.Syringe_Unlabelled,
            ItemType.Thermal_Camera => ItemAssets.Instance.Thermal_Camera,
            ItemType.UHF_Radio => ItemAssets.Instance.UHF_Radio,
            ItemType.ULF_Radio => ItemAssets.Instance.ULF_Radio,
            ItemType.Water_Canteen => ItemAssets.Instance.Water_Canteen,
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
            whereEquipped = (string)details["whereEquipped"];
            isUsable = (bool)details["isUsable"];
            isConsumable = (bool)details["isConsumable"];
            isFragile = (bool)details["isFragile"];
            isUnbreakable = (bool)details["isUnbreakable"];
            ablativeHealth = System.Convert.ToInt32(details["ablativeHealth"]);
            hpGranted = System.Convert.ToInt32(details["hpGranted"]);
            loudRadius = System.Convert.ToInt32(details["loudRadius"]);
            meleeDamage = System.Convert.ToInt32(details["meleeDamage"]);
            charges = System.Convert.ToInt32(details["charges"]);
            isPoisonable = (bool)details["isPoisonable"];
            poisonedBy = (string)details["poisonedBy"];
            isShareable = (bool)details["isShareable"];
            isTradeable = (bool)details["isTradeable"];
            equippableSlots = (details["equippableSlots"] as JArray).Select(token => token.ToString()).ToList();
            blockedByLateral = (string)details["blockedByLateral"];
            blockedByLeftBrace = (string)details["blockedByLeftBrace"];
            blockedByPosterior = (string)details["blockedByPosterior"];
            blockedByBackHole = (string)details["blockedByBackHole"];
            blockedByFullBody = (string)details["blockedByFullBody"];

            //load gun stats
            gunType = (string)details["gunType"];
            if (gunType != null)
            {
                gunMaxClip = System.Convert.ToInt32(details["gunMaxClip"]);
                gunBaseMaxClip = System.Convert.ToInt32(details["gunBaseMaxClip"]);
                gunDamage = System.Convert.ToInt32(details["gunDamage"]);
                gunCritDamage = System.Convert.ToInt32(details["gunCritDamage"]);
                gunCQBU = System.Convert.ToInt32(details["gunCQBU"]);
                gunCQBA = System.Convert.ToInt32(details["gunCQBA"]);
                gunShortU = System.Convert.ToInt32(details["gunShortU"]);
                gunShortA = System.Convert.ToInt32(details["gunShortA"]);
                gunMedU = System.Convert.ToInt32(details["gunMedU"]);
                gunMedA = System.Convert.ToInt32(details["gunMedA"]);
                gunLongU = System.Convert.ToInt32(details["gunLongU"]);
                gunLongA = System.Convert.ToInt32(details["gunLongA"]);
                gunCoriolisU = System.Convert.ToInt32(details["gunCoriolisU"]);
                gunCoriolisA = System.Convert.ToInt32(details["gunCoriolisA"]);
                gunSuppressionDrain = System.Convert.ToInt32(details["gunSuppressionDrain"]);
                gunCQBSuppressionPenalty = System.Convert.ToInt32(details["gunCQBSuppressionPenalty"]);
                gunShortSuppressionPenalty = System.Convert.ToInt32(details["gunShortSuppressionPenalty"]);
                gunMedSuppressionPenalty = System.Convert.ToInt32(details["gunMedSuppressionPenalty"]);
                gunLongSuppressionPenalty = System.Convert.ToInt32(details["gunLongSuppressionPenalty"]);
                gunCQBCoverDamage = System.Convert.ToInt32(details["gunCQBCoverDamage"]);
                gunShortCoverDamage = System.Convert.ToInt32(details["gunShortCoverDamage"]);
                gunMedCoverDamage = System.Convert.ToInt32(details["gunMedCoverDamage"]);
                gunLongCoverDamage = System.Convert.ToInt32(details["gunLongCoverDamage"]);
            }

            //load position
            x = System.Convert.ToInt32(details["x"]);
            y = System.Convert.ToInt32(details["y"]);
            z = System.Convert.ToInt32(details["z"]);
            MapPhysicalPosition(x, y, z);
        }
    }

    public void SaveData(ref GameData data)
    {
        details = new();

        //save basic information
        details.Add("itemName", itemName);
        details.Add("weight", weight);
        details.Add("ammo", ammo);
        details.Add("whereEquipped", whereEquipped);
        details.Add("isUsable", isUsable);
        details.Add("isConsumable", isConsumable);
        details.Add("isFragile", isFragile);
        details.Add("isUnbreakable", isUnbreakable);
        details.Add("ablativeHealth", ablativeHealth);
        details.Add("hpGranted", hpGranted);
        details.Add("loudRadius", loudRadius);
        details.Add("meleeDamage", meleeDamage);
        details.Add("charges", charges);
        details.Add("isPoisonable", isPoisonable);
        details.Add("poisonedBy", poisonedBy);
        details.Add("isShareable", isShareable);
        details.Add("isTradeable", isTradeable);
        details.Add("equippableSlots", equippableSlots);
        details.Add("blockedByLateral", blockedByLateral);
        details.Add("blockedByLeftBrace", blockedByLeftBrace);
        details.Add("blockedByPosterior", blockedByPosterior);
        details.Add("blockedByBackHole", blockedByBackHole);
        details.Add("blockedByFullBody", blockedByFullBody);

        //save gun information
        details.Add("gunType", gunType);
        if (gunType != null)
        {
            details.Add("gunMaxClip", gunMaxClip);
            details.Add("gunBaseMaxClip", gunBaseMaxClip);
            details.Add("gunDamage", gunDamage);
            details.Add("gunCritDamage", gunCritDamage);
            details.Add("gunCQBU", gunCQBU);
            details.Add("gunCQBA", gunCQBA);
            details.Add("gunShortU", gunShortU);
            details.Add("gunShortA", gunShortA);
            details.Add("gunMedU", gunMedU);
            details.Add("gunMedA", gunMedA);
            details.Add("gunLongU", gunLongU);
            details.Add("gunLongA", gunLongA);
            details.Add("gunCoriolisU", gunCoriolisU);
            details.Add("gunCoriolisA", gunCoriolisA);
            details.Add("gunSuppressionDrain", gunSuppressionDrain);
            details.Add("gunCQBSuppressionPenalty", gunCQBSuppressionPenalty);
            details.Add("gunShortSuppressionPenalty", gunShortSuppressionPenalty);
            details.Add("gunMedSuppressionPenalty", gunMedSuppressionPenalty);
            details.Add("gunLongSuppressionPenalty", gunLongSuppressionPenalty);
            details.Add("gunCQBCoverDamage", gunCQBCoverDamage);
            details.Add("gunShortCoverDamage", gunShortCoverDamage);
            details.Add("gunMedCoverDamage", gunMedCoverDamage);
            details.Add("gunLongCoverDamage", gunLongCoverDamage);
        }

        //save position
        details.Add("x", x);
        details.Add("y", y);
        details.Add("z", z);

        //add the item in
        if (data.allItemDetails.ContainsKey(id))
            data.allItemDetails.Remove(id);

        data.allItemDetails.Add(id, details);
    }

    public void RunPickupEffect()
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
                owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Small"), "Misc1");

            //spawn med medkit in bag
            if (itemName == "Bag")
                owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Medium"), "Misc2");

            //spawn small & med medkit in backpack
            if (itemName == "Backpack")
            {
                owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Small"), "Misc3");
                owningSoldier.PickUpItemToSlot(itemManager.SpawnItem("Medkit_Medium"), "Misc4");
            }

            //perform ability effects
            if (gunType != null)
            {
                if (owningSoldier.IsGunner())
                {
                    //one time 1.5 bonus to max clip and ammo
                    if (gunMaxClip == gunBaseMaxClip)
                    {
                        gunMaxClip = Mathf.FloorToInt(gunMaxClip * 1.5f);
                        ammo = Mathf.FloorToInt(ammo * 1.5f);
                    }

                    //add 1 round to empty guns
                    if (ammo == 0)
                        ammo++;
                }

                if (owningSoldier.IsPlanner())
                {
                    ammo += game.DiceRoll();
                    if (ammo > gunMaxClip)
                        ammo = gunMaxClip;
                }
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
        if (gunLongCoverDamage.Equals(gunCQBCoverDamage))
            return $"({gunCQBCoverDamage})";
        else
            return $"({gunCQBCoverDamage},{gunShortCoverDamage},{gunMedCoverDamage},{gunLongCoverDamage}-c,s,m,l)";
    }
    public void ChangeOwner(IHaveInventory from, IHaveInventory to)
    {
        if (from.Inventory.AllItemIds.Contains(id))
            from.Inventory.AllItemIds.Remove(id);
        if (!to.Inventory.AllItemIds.Contains(id))
            to.Inventory.AllItemIds.Add(id);
    }
}
