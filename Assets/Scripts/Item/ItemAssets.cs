using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Sprite Ammo_AR;
    public Sprite Ammo_LMG;
    public Sprite Ammo_Pi;
    public Sprite Ammo_Ri;
    public Sprite Ammo_Sh;
    public Sprite Ammo_SMG;
    public Sprite Ammo_Sn;
    public Sprite AR_ACOG_FAL;
    public Sprite AR_AK_47;
    public Sprite AR_M_16;
    public Sprite Armour_Body;
    public Sprite Armour_Exo;
    public Sprite Armour_Ghillie;
    public Sprite Armour_Juggernaut;
    public Sprite Armour_Stimulant;
    public Sprite Backpack;
    public Sprite Bag;
    public Sprite Binoculars;
    public Sprite Brace;
    public Sprite Claymore;
    public Sprite Deployment_Beacon;
    public Sprite E_Tool;
    public Sprite Food_Pack;
    public Sprite Grenade_Flashbang;
    public Sprite Grenade_Frag;
    public Sprite Grenade_Smoke;
    public Sprite Grenade_Tabun;
    public Sprite Knife;
    public Sprite Medkit_Large;
    public Sprite Medkit_Medium;
    public Sprite Medkit_Small;
    public Sprite LMG_LSW;
    public Sprite LMG_M_60;
    public Sprite LMG_SAW;
    public Sprite Logistics_Belt;
    public Sprite Pi_357;
    public Sprite Pi_Glock;
    public Sprite Pi_Sidearm;
    public Sprite Poison_Satchel;
    public Sprite Ri_Arisaka;
    public Sprite Ri_Carbine;
    public Sprite Ri_M1_Garand;
    public Sprite Riot_Shield;
    public Sprite Sh_Ithaca;
    public Sprite Sh_Olympus;
    public Sprite Sh_SPAS_12;
    public Sprite SMG_P_90;
    public Sprite SMG_Thompson;
    public Sprite SMG_UMP_40;
    public Sprite Sn_Barrett;
    public Sprite Sn_Dragunov;
    public Sprite Sn_Intervention;
    public Sprite Suppressor;
    public Sprite Syringe_Amphetamine;
    public Sprite Syringe_Androstenedione;
    public Sprite Syringe_Cannabinoid;
    public Sprite Syringe_Danazol;
    public Sprite Syringe_Glucocorticoid;
    public Sprite Syringe_Modafinil;
    public Sprite Syringe_Shard;
    public Sprite Syringe_Trenbolone;
    public Sprite Syringe_Unlabelled;
    public Sprite Thermal_Camera;
    public Sprite UHF_Radio;
    public Sprite ULF_Radio;
    public Sprite Water_Canteen;

    public Sprite GetSprite(string name)
    {
        return (Sprite)GetType().GetField(name).GetValue(this);
    }
}
