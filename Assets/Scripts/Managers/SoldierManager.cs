using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierManager : MonoBehaviour, IDataPersistence
{
    public MainGame game;

    public List<string> allSoldierIds = new();
    public List<Soldier> allSoldiers = new();
    public Soldier soldierPrefab;

    public void LoadData(GameData data)
    {
        //destroy existing soldiers ready to regenerate them
        IEnumerable<Soldier> allSoldiersInst = FindObjectsOfType<Soldier>();
        foreach (Soldier soldier in allSoldiersInst)
        {
            Destroy(soldier.soldierUI);
            Destroy(soldier.gameObject);
        }

        allSoldierIds = data.allSoldiersIds;
        foreach (string id in allSoldierIds)
        {
            var newSoldier = Instantiate(soldierPrefab);
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
    }
}
