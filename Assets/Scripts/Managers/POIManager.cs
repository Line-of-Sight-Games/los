using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class POIManager : MonoBehaviour, IDataPersistence
{
    public List<string> allPOIIds = new();
    public List<POI> allPOIs = new();
    public POI poiPrefab;
    public Explosion explosionPrefab;
    public GoodyBox gbPrefab;
    public Terminal terminalPrefab;
    public ExplosiveBarrel barrelPrefab;
    public Claymore claymorePrefab;
    public SmokeCloud smokeCloudPrefab;
    public TabunCloud tabunCloudPrefab;
    public DeploymentBeacon deploymentBeaconPrefab;
    public ThermalCamera thermalCamPrefab;
    public DrugCabinet drugCabinetPrefab;
    public BinocularBeam binocularStripPrefab;

    public Sprite explosiveBarrelSprite, goodyBoxSprite, terminalSprite, claymoreSprite, deploymentBeaconSprite, thermalCameraSprite, drugCabinetSprite;

    public void LoadData(GameData data)
    {
        //destroy existing POIs ready to regenerate them
        IEnumerable<POI> allPOIsInst = FindObjectsByType<POI>(default);
        foreach (POI poi in allPOIsInst)
            Destroy(poi.gameObject);

        allPOIIds = data.allPOIIds;
        foreach (string id in allPOIIds)
        {
            string spawnType;
            if (data.allPOIDetails.TryGetValue(id, out var details))
            {
                spawnType = (string)details["poiType"];

                if (spawnType == "terminal")
                {
                    Terminal terminal = Instantiate(terminalPrefab);
                    terminal.id = id;
                    terminal.LoadData(data);
                }
                else if (spawnType == "gb")
                {
                    GoodyBox gb = Instantiate(gbPrefab);
                    gb.id = id;
                    gb.LoadData(data);
                }
                else if (spawnType == "barrel")
                {
                    ExplosiveBarrel barrel = Instantiate(barrelPrefab);
                    barrel.id = id;
                    barrel.LoadData(data);
                }
                else if (spawnType == "claymore")
                {
                    Claymore claymore = Instantiate(claymorePrefab);
                    claymore.id = id;
                    claymore.LoadData(data);
                }
                else if (spawnType == "smoke")
                {
                    SmokeCloud smoke = Instantiate(smokeCloudPrefab);
                    smoke.id = id;
                    smoke.LoadData(data);
                }
                else if (spawnType == "tabun")
                {
                    TabunCloud tabun = Instantiate(tabunCloudPrefab);
                    tabun.id = id;
                    tabun.LoadData(data);
                }
                else if (spawnType == "depbeacon")
                {
                    DeploymentBeacon depbeacon = Instantiate(deploymentBeaconPrefab);
                    depbeacon.id = id;
                    depbeacon.LoadData(data);
                }
                else if (spawnType == "thermalcam")
                {
                    ThermalCamera thermalcam = Instantiate(thermalCamPrefab);
                    thermalcam.id = id;
                    thermalcam.LoadData(data);
                }
                else if (spawnType == "drugcab")
                {
                    DrugCabinet drugcab = Instantiate(drugCabinetPrefab);
                    drugcab.id = id;
                    drugcab.LoadData(data);
                }
                else if (spawnType == "binocularBeam")
                {
                    BinocularBeam binocular = Instantiate(binocularStripPrefab);
                    binocular.id = id;
                    binocular.LoadData(data);
                }
            }
        }

        isDataLoaded = true;
    }
    public void SaveData(ref GameData data)
    {
        allPOIIds.Clear();
        data.allPOIIds.Clear();

        IEnumerable<POI> allPOIs = FindObjectsByType<POI>(default);
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
        allPOIs = FindObjectsByType<POI>(default).ToList();
    }
    public POI FindPOIById(string id)
    {
        foreach (POI poi in FindObjectsByType<POI>(default))
        {
            if (poi.id == id)
                return poi;
        }
        return null;
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
