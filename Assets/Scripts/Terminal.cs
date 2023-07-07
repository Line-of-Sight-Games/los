using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : PhysicalObject, IDataPersistence
{
    public Dictionary<string, object> details;
    public string terrain;
    public List<string> soldierAlreadyInteracted;
    public JArray soldierAlreadyInteractedJArray;
    public string terminalType;

    public MainGame game;
    public MainMenu menu;

    private void Start()
    {
        game = FindObjectOfType<MainGame>();
        menu = FindObjectOfType<MainMenu>();
    }

    public Terminal Init(int xpos, int ypos, int zpos, string terrain, string type)
    {
        id = GenerateGuid();
        x = xpos;
        y = ypos;
        z = zpos;
        this.terrain = terrain;
        MapPhysicalPosition(x, y, z);
        terminalType = type;

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

        terminalType = (string)details["terminalType"];

        //load list of soldier already interacted
        soldierAlreadyInteracted = new();
        soldierAlreadyInteractedJArray = (JArray)details["soldierAlreadyInteracted"];
        foreach (string soldierId in soldierAlreadyInteractedJArray)
            soldierAlreadyInteracted.Add(soldierId);
    }

    public void SaveData(ref GameData data)
    {
        //save position
        details.Add("x", x);
        details.Add("y", y);
        details.Add("z", z);
        details.Add("terrainOn", terrainOn);
        details.Add("terminalType", terminalType);

        //save list of soldiers already interacted
        details.Add("soldierAlreadyInteracted", soldierAlreadyInteracted);
    }

    public List<string> SoldiersAlreadyInteracted
    {
        get { return soldierAlreadyInteracted; }
        set { soldierAlreadyInteracted = value; }
    }
}

