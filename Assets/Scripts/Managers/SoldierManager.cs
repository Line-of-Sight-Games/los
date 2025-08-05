using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierManager : MonoBehaviour, IDataPersistence
{
    public static SoldierManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public List<string> allSoldierIds = new();
    public List<Soldier> allSoldiers = new();
    public Soldier soldierPrefab;

    public GameObject friendlyDisplayColumn, enemyDisplayColumn;
    private void Update()
    {
        foreach (SoldierUI soldierUI in FindObjectsByType<SoldierUI>(default))
        {
            if (soldierUI.linkedSoldier != null)
            {
                if (soldierUI.linkedSoldier.IsOnturn())
                    soldierUI.DisplayInFriendlyColumn();
                else
                    soldierUI.DisplayInEnemyColumn();
            }
        }
        
        //order soldiers by display priority
        SetSiblingIndexByPriority();
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

            if (newSoldier.IsOnturn())
                newSoldier.LinkWithUI(friendlyDisplayColumn.transform);
            else
                newSoldier.LinkWithUI(enemyDisplayColumn.transform);
        }
        RefreshSoldierList();

        isDataLoaded = true;
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

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
