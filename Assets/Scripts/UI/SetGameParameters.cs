using UnityEngine;
using TMPro;

public class SetGameParameters : MonoBehaviour, IDataPersistence
{
    public Vector3 camPosition, sunPosition, mapPosition, mapDimensions, bottomPlanePosition, bottomPlaneDimensions, outlineAreaPosition, outlineAreaDimensions;
    public float camOrthoSize;

    public GameObject setupMenuUI, createSoldierMenuUI;
    public TMP_InputField xSize, ySize, zSize, maxRoundsInput, turnTimeInput, numberBasicZombiesInput, numberBruteZombiesInput;
    public TMP_Dropdown maxSoldierDropdown;
    public int x, y, z, maxRounds, maxTurnTime, maxTeams, maxSoldiers, numberBasicZombies, numberBruteZombies;

    public void LoadData(GameData data)
    {
        isDataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        data.mapPosition = mapPosition;
        data.mapDimensions = mapDimensions;
        data.bottomPlanePosition = bottomPlanePosition;
        data.bottomPlaneDimensions = bottomPlaneDimensions;
        data.outlineAreaPosition = outlineAreaPosition;
        data.outlineAreaDimensions = outlineAreaDimensions;
        data.camPosition = camPosition;
        data.camOrthoSize = camOrthoSize;
        data.sunPosition = sunPosition;

        data.currentRound = 1;
        data.currentTeam = 1;
        data.maxRounds = maxRounds;
        data.maxTurnTime = maxTurnTime;
        data.maxX = x;
        data.maxY = y;
        data.maxZ = z;
        data.maxTeams = maxTeams;

        if (DataPersistenceManager.Instance.lozMode)
        {
            data.numberBasicZombies = numberBasicZombies;
            data.numberBruteZombies = numberBruteZombies;
        }
    }
    public void SetField()
    {
        bottomPlaneDimensions = new Vector3(x, y, z);
        bottomPlanePosition = new Vector3(x / 2, 0, z / 2);

        outlineAreaDimensions = new Vector3(x, 0.1f, z);
        outlineAreaPosition = new Vector3(x / 2, 0, z / 2);

        mapDimensions = new Vector3(x, y, z);
        mapPosition = new Vector3(x / 2, y / 2, z / 2);
    }

    public void SetCam()
    {
        camOrthoSize = Mathf.Max(x, z) / 2;
        camPosition = new Vector3(x / 2, y * 2, z / 2);
        sunPosition = new Vector3(0, y + 1, 0);
    }

    public void Confirm()
    {
        // Validate non-LOZ fields
        bool baseValid =
            HelperFunctions.ValidateIntInput(xSize, out x) &&
            HelperFunctions.ValidateIntInput(ySize, out y) &&
            HelperFunctions.ValidateIntInput(zSize, out z) &&
            HelperFunctions.ValidateIntInput(maxRoundsInput, out maxRounds) &&
            HelperFunctions.ValidateIntInput(turnTimeInput, out maxTurnTime) &&
            int.TryParse(maxSoldierDropdown.captionText.text, out maxSoldiers);

        // Validate extra fields ONLY in LOZ mode
        bool zombieSetupValid = true; // default to true if not in LOZ mode

        if (DataPersistenceManager.Instance.lozMode)
        {
            zombieSetupValid =
                HelperFunctions.ValidateIntInput(numberBasicZombiesInput, out numberBasicZombies) &&
                HelperFunctions.ValidateIntInput(numberBruteZombiesInput, out numberBruteZombies);
        }

        // Only continue if ALL required fields are valid
        if (baseValid && zombieSetupValid)
        {
            maxTeams = 2;
            maxTurnTime *= 60;

            SetField();
            SetCam();

            WeatherManager.Instance.GenerateWeather(maxRounds);
            DataPersistenceManager.Instance.SaveGame();

            setupMenuUI.SetActive(false);
            createSoldierMenuUI.SetActive(true);
        }
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
