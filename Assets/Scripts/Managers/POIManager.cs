using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public MainGame game;

    public List<string> allPOIIds = new();
    public List<POI> allPOIs = new();
    public Terminal terminalPrefab;
    /*variables for GB required as well

    public void LoadData(GameData data)
    {
        //destroy existing soldiers ready to regenerate them
        IEnumerable<POI> allPOIsInst = FindObjectsOfType<POI>();
        foreach (POI poi in allPOIsInst)
            Destroy(poi.gameObject);

        allPOIIds = data.allPOIIds;
        foreach (string id in allPOIIds)
        {
            var newPOI = Instantiate(soldierPrefab);
            newSoldier.id = id;
            newSoldier.LoadData(data);

            if (game.displayPanel != null)
                newSoldier.LinkWithUI(game.displayPanel.gameObject);
        }

        RefreshSoldierList();
    }

    public void SaveData(ref GameData data)
    {
        IEnumerable<Soldier> allSoldiers = FindObjectsOfType<Soldier>();
        foreach (Soldier soldier in allSoldiers)
            if (!allSoldierIds.Contains(soldier.id))
                allSoldierIds.Add(soldier.id);

        data.allSoldiersIds = allSoldierIds;
    }

    public void RefreshSoldierList()
    {
        allSoldiers = FindObjectsOfType<Soldier>().ToList();
    }
    public Soldier FindSoldierByName(string name)
    {
        foreach (Soldier s in allSoldiers)
        {
            if (s.soldierName == name)
                return s;
        }
        return null;
    }
    public Soldier FindSoldierById(string id)
    {
        foreach (Soldier s in allSoldiers)
        {
            if (s.id == id)
                return s;
        }
        return null;
    }*/
}
