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
        IEnumerable<Soldier> allSoldiersInst = FindObjectsByType<Soldier>(default);
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

            if (game.soldierDisplayPanelUI != null)
                newSoldier.LinkWithUI(game.soldierDisplayPanelUI);
        }

        //order soldiers by display priority
        SetSiblingIndexByPriority();

        RefreshSoldierList();
    }

    public void SaveData(ref GameData data)
    {
        IEnumerable<Soldier> allSoldiers = FindObjectsByType<Soldier>(default);
        foreach (Soldier soldier in allSoldiers)
            if (!allSoldierIds.Contains(soldier.id))
                allSoldierIds.Add(soldier.id);

        data.allSoldiersIds = allSoldierIds;
    }

    public void RefreshSoldierList()
    {
        allSoldiers = FindObjectsByType<Soldier>(default).ToList();
    }

    public void SetSiblingIndexByPriority()
    {
        // Get all sibling components
        Soldier[] allSoldiers = FindObjectsByType<Soldier>(default);

        // Sort siblings by soldierDisplayPriority in ascending order
        System.Array.Sort(allSoldiers, (x, y) => x.soldierDisplayPriority.CompareTo(y.soldierDisplayPriority));

        // Set the sibling index of the soldiers and their linkedUI based on the sorted order
        for (int i = 0; i < allSoldiers.Length; i++)
        {
            allSoldiers[i].transform.SetSiblingIndex(i);
            allSoldiers[i].soldierUI.transform.SetSiblingIndex(i);
        }
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
