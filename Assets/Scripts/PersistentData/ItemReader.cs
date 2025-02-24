using Newtonsoft.Json;
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
        public List<string> Traits;
        public int UsageAP;
        public int Weight;
        public List<string> EquippableSlots;
        public string WhereEquipped;
        public int HPGranted;
        public int AblativeHealth;
        public int LoudRadius;
        public int Charges;
        public int Ammo;
        public string PoisonedBy;
        public int MeleeDamage;
        public int JammingForTurns;
        public int SpyingForTurns;
        public int MaxClip;
        public int Damage;
        public int CritDamage;
        public int CQBU;
        public int CQBA;
        public int ShortU;
        public int ShortA;
        public int MedU;
        public int MedA;
        public int LongU;
        public int LongA;
        public int CoriolisU;
        public int CoriolisA;
        public int SuppressDrain;
        public int CQBSupPen;
        public int ShortSupPen;
        public int MedSupPen;
        public int LongSupPen;
        public int CorSupPen;
        public int CQBCovDamage;
        public int ShortCovDamage;
        public int MedCovDamage;
        public int LongCovDamage;
        public Dictionary<string, string> InventorySlots;
    }

    [System.Serializable]
    public class ItemList
    {
        public List<Item> items;
    }

    void Awake()
    {
        allItems = JsonConvert.DeserializeObject<ItemList>(itemsJSON.text);
    }
}
