using System.Collections.Generic;
using UnityEngine;

public interface IHaveInventory
{
    public GameObject GameObject { get; }
    public Inventory Inventory { get; }
    public List<string> InventoryList { get; }

    public int X { get; }
    public int Y { get; }
    public int Z { get; }
}