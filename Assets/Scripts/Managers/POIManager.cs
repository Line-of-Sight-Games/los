using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class POIManager : MonoBehaviour, IDataPersistence
{
    public static POIManager Instance { get; private set; }

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
        // Destroy existing POIs ready to regenerate them.
        POI[] existingPOIs = FindObjectsByType<POI>();

        foreach (POI poi in existingPOIs)
        {
            if (poi != null)
                Destroy(poi.gameObject);
        }

        // Clear existing lists.
        allPOIs.Clear();

        // Restore saved POI IDs.
        allPOIIds = data.allPOIIds;

        foreach (string id in allPOIIds)
        {
            if (!data.allPOIDetails.TryGetValue(id, out var details))
                continue;

            string spawnType = (string)details["poiType"];

            switch (spawnType)
            {
                case "terminal":
                    {
                        Terminal terminal = Instantiate(terminalPrefab);
                        terminal.id = id;
                        terminal.LoadData(data);
                        break;
                    }
                case "gb":
                    {
                        GoodyBox gb = Instantiate(gbPrefab);
                        gb.id = id;
                        gb.LoadData(data);
                        break;
                    }
                case "barrel":
                    {
                        ExplosiveBarrel barrel = Instantiate(barrelPrefab);
                        barrel.id = id;
                        barrel.LoadData(data);
                        break;
                    }
                case "claymore":
                    {
                        Claymore claymore = Instantiate(claymorePrefab);
                        claymore.id = id;
                        claymore.LoadData(data);
                        break;
                    }
                case "smoke":
                    {
                        SmokeCloud smoke = Instantiate(smokeCloudPrefab);
                        smoke.id = id;
                        smoke.LoadData(data);
                        break;
                    }
                case "tabun":
                    {
                        TabunCloud tabun = Instantiate(tabunCloudPrefab);
                        tabun.id = id;
                        tabun.LoadData(data);
                        break;
                    }
                case "depbeacon":
                    {
                        DeploymentBeacon beacon = Instantiate(deploymentBeaconPrefab);
                        beacon.id = id;
                        beacon.LoadData(data);
                        break;
                    }
                case "thermalcam":
                    {
                        ThermalCamera camera = Instantiate(thermalCamPrefab);
                        camera.id = id;
                        camera.LoadData(data);
                        break;
                    }
                case "drugcab":
                    {
                        DrugCabinet cabinet = Instantiate(drugCabinetPrefab);
                        cabinet.id = id;
                        cabinet.LoadData(data);
                        break;
                    }
                case "binocularBeam":
                    {
                        BinocularBeam binocular = Instantiate(binocularStripPrefab);
                        binocular.id = id;
                        binocular.LoadData(data);
                        break;
                    }
                default:
                    Debug.LogWarning($"Unknown POI type '{spawnType}' for ID '{id}'.");
                    break;
            }
        }

        RefreshPOIList();

        isDataLoaded = true;
    }
    public void SaveData(ref GameData data)
    {
        allPOIIds.Clear();
        data.allPOIIds.Clear();

        IEnumerable<POI> allPOIs = FindObjectsByType<POI>();
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
        allPOIs = FindObjectsByType<POI>().ToList();
    }
    public POI FindPOIById(string id)
    {
        foreach (POI poi in FindObjectsByType<POI>())
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
