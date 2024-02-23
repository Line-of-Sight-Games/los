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
    public Claymore claymorePrefab;
    public SmokeCloud smokeCloudPrefab;
    public TabunCloud tabunCloudPrefab;
    public DeploymentBeacon deploymentBeaconPrefab;

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
            string spawnType = newPOI.poiType;
            Destroy(newPOI.gameObject);

            if (spawnType == "terminal")
                newPOI = Instantiate(terminalPrefab);
            else if (spawnType == "gb")
                newPOI = Instantiate(gbPrefab);
            else if (spawnType == "barrel")
                newPOI = Instantiate(barrelPrefab);
            else if (spawnType == "claymore")
                newPOI = Instantiate(claymorePrefab);
            else if (spawnType == "smoke")
                newPOI = Instantiate(smokeCloudPrefab);
            else if (spawnType == "tabun")
                newPOI = Instantiate(tabunCloudPrefab);
            else if (spawnType == "depbeacon")
                newPOI = Instantiate(deploymentBeaconPrefab);

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
    public void DestroyPOI(POI poi)
    {
        Destroy(poi.gameObject);
        RefreshPOIList();
    }
    public void RefreshPOIList()
    {
        allPOIs = FindObjectsOfType<POI>().ToList();
    }
    public POI FindPOIById(string id)
    {
        foreach (POI poi in FindObjectsOfType<POI>())
        {
            if (poi.id == id)
                return poi;
        }
        return null;
    }
}
