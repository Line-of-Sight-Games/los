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
        public Dictionary<string, string> InventorySlots;
        public Dictionary<string, int> GunTraits;
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
