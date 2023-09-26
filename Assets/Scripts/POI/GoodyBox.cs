using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodyBox : POI, IDataPersistence, IHaveInventory
{
    public Dictionary<string, object> details;
    public string terrain;
    public Inventory inventory;
    public List<string> inventoryList;

    public MainGame game;
    public MainMenu menu;


    private void Start()
    {
        game = FindObjectOfType<MainGame>();
        menu = FindObjectOfType<MainMenu>();
    }

    public GoodyBox Init(int xpos, int ypos, int zpos, string terrain)
    {
        id = GenerateGuid();
        x = xpos;
        y = ypos;
        z = zpos;
        this.terrain = terrain;
        MapPhysicalPosition(x, y, z);

        return this;
    }

    public void LoadData(GameData data)
    {
        //load position
        x = System.Convert.ToInt32(details["x"]);
        y = System.Convert.ToInt32(details["y"]);
        z = System.Convert.ToInt32(details["z"]);
        terrainOn = (string)details["terrainOn"];
        MapPhysicalPosition(x, y, z);
    }

    public void SaveData(ref GameData data)
    {
        //save position
        details.Add("x", x);
        details.Add("y", y);
        details.Add("z", z);
        details.Add("terrainOn", terrainOn);

        //save inventory
        inventoryList = new List<string>();
        foreach (Item item in inventory.AllItems)
        {
            inventoryList.Add(item.id);
        }
        details.Add("inventory", inventoryList);
    }

    public Inventory Inventory
    {
        get { return inventory; }
    } 
}
