using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class POIManager : MonoBehaviour, IDataPersistence
{
    public List<string> allPOIIds = new();
    public List<POI> allPOIs = new();
    public POI poiPrefab;
    public GoodyBox gbPrefab;
    public Terminal terminalPrefab;
    public ExplosiveBarrel barrelPrefab;

    public void LoadData(GameData data)
    {
        //destroy existing POIs ready to regenerate them
        IEnumerable<POI> allPOIsInst = FindObjectsOfType<POI>();
        foreach (POI poi in allPOIsInst)
            Destroy(poi.gameObject);

        allPOIIds = data.allPOIIds;
        foreach (string id in allPOIIds)
        {
            POI newPOI = Instantiate(poiPrefab);
            newPOI.id = id;
            newPOI.LoadData(data);
            print($"{newPOI.id}: {newPOI.poiType}: {newPOI.GetType()}");
            if (newPOI.poiType == "terminal")
            {
                Destroy(newPOI.gameObject);
                newPOI = Instantiate(terminalPrefab);
                
            }
            else if (newPOI.poiType == "gb")
            {
                Destroy(newPOI.gameObject);
                newPOI = Instantiate(gbPrefab);
            }
            else if (newPOI.poiType == "barrel")
            {
                Destroy(newPOI.gameObject);
                newPOI = Instantiate(barrelPrefab);
            }
            print($"{newPOI.GetType()}");
            newPOI.id = id;
            newPOI.LoadData(data);
        }
    }

    public void SaveData(ref GameData data)
    {
        allPOIIds.Clear();
        data.allPOIIds.Clear();

        IEnumerable<POI> allPOIs = FindObjectsOfType<POI>();
        foreach (POI poi in allPOIs)
            if (!allPOIIds.Contains(poi.id))
                allPOIIds.Add(poi.id);

        data.allPOIIds = allPOIIds;
    }
    
    /*

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
