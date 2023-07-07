using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReader : MonoBehaviour
{
    public TextAsset itemsJSON;
    public ItemList allItems = new();

    [System.Serializable]
    public class Item
    {
        public string Name;
        public string Type;
        public int Weight;
        public int Ammo;
        public int Max_Clip;
        public int Base_Max_Clip;
        public int Damage;
        public int Crit_Damage;
        public int CQB_U;
        public int CQB_A;
        public int Short_U;
        public int Short_A;
        public int Med_U;
        public int Med_A;
        public int Long_U;
        public int Long_A;
        public int Coriolis_U;
        public int Coriolis_A;
        public int Suppress_Drain;
        public int CQB_Sup_Pen;
        public int Short_Sup_Pen;
        public int Med_Sup_Pen;
        public int Long_Sup_Pen;
        public int CQB_Cov_Damage;
        public int Short_Cov_Damage;
        public int Med_Cov_Damage;
        public int Long_Cov_Damage;
        public string WhereEquipped;
        public bool Usable;
        public bool Consumable;
        public bool Fragile;
        public bool Unbreakable;
        public int AblativeHealth;
        public int HPGranted;
        public int LoudRadius;
        public int MeleeDamage;
        public int Charges;
        public bool Poisonable;
        public string PoisonedBy;
        public bool Shareable;
        public bool Tradeable;
        public string EquippableSlots;
        public string BlockedByLateral;
        public string BlockedByLeftBrace;
        public string BlockedByPosterior;
        public string BlockedByBackHole;
        public string BlockedByFullBody;
    }

    [System.Serializable]
    public class ItemList
    {
        public Item[] items;
    }

    void Awake()
    {
        allItems = JsonUtility.FromJson<ItemList>(itemsJSON.text);
    }
}
