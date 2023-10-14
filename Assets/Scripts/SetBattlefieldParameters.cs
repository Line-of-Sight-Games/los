using UnityEngine;
using TMPro;

public class SetBattlefieldParameters : MonoBehaviour, IDataPersistence
{
    public DataPersistenceManager dataPersistenceManager;
    public MainMenu menu;
    public WeatherGen weather;
    public MainGame game;
    public Camera cam;
    public Light sun;
    public GameObject battlefield, bottomPlane, outlineArea, setupMenuUI, gameTimer, gameMenuUI;
    public TMP_InputField xSize, ySize, zSize, maxRoundsInput, turnTimeInput;
    public int x, y, z, maxRounds, maxTurnTime;

    public void LoadData(GameData data)
    {
        
    }

    public void SaveData(ref GameData data)
    {
        data.mapPosition = battlefield.transform.position;
        data.mapDimensions = battlefield.transform.localScale;
        data.bottomPlanePosition = bottomPlane.transform.position;
        data.bottomPlaneDimensions = bottomPlane.transform.localScale;
        data.outlineAreaPosition = outlineArea.transform.position;
        data.outlineAreaDimensions = outlineArea.transform.localScale;
        data.camPosition = cam.transform.position;
        data.camOrthoSize = cam.orthographicSize;
        data.sunPosition = sun.transform.position;
        data.maxRounds = maxRounds;
        data.maxTurnTime = maxTurnTime;
    }

    private void Start()
    {
        dataPersistenceManager = FindObjectOfType<DataPersistenceManager>();
        if (dataPersistenceManager == null)
        {
            print("Found the data persistence mnanager");
        }
    }

    public void ChangeXSize()
    {
        int.TryParse(xSize.text, out x);
        SetField();
        SetCam();
    }

    public void ChangeYSize()
    {
        int.TryParse(ySize.text, out z);
        SetField();
        SetCam();
    }

    public void ChangeZSize()
    {
        int.TryParse(zSize.text, out y);
        SetField();
        SetCam();
    }

    public void SetField()
    {
        bottomPlane.transform.localScale = new Vector3(x, y, z);
        bottomPlane.transform.position = new Vector3(x / 2, 0, z / 2);

        outlineArea.transform.localScale = new Vector3(x, 0.1f, z);
        outlineArea.transform.position = new Vector3(x / 2, 0, z / 2);

        battlefield.transform.localScale = new Vector3(x, y, z);
        battlefield.transform.position = new Vector3(x / 2, y / 2, z / 2);
    }

    public void SetCam()
    {
        cam.orthographicSize = Mathf.Max(x, z) / 2;
        cam.transform.position = new Vector3(x / 2, y + 1, z / 2);
        sun.transform.position = new Vector3(0, y + 1, 0);
    }

    public void Confirm()
    {
        if (int.TryParse(xSize.text, out x) && int.TryParse(ySize.text, out z) && int.TryParse(zSize.text, out y) && int.TryParse(maxRoundsInput.text, out maxRounds) && int.TryParse(turnTimeInput.text, out maxTurnTime))
        {
            if (x > 0 && y > 0 && z >= 0)
            {
                game.currentRound = 1;
                game.currentTeam = 1;
                game.maxRounds = maxRounds;
                game.maxTurnTime = maxTurnTime * 60;
                game.maxX = x;
                game.maxY = z;
                game.maxZ = y;

                DataPersistenceManager.Instance.SaveGame();

                setupMenuUI.SetActive(false);
                gameTimer.SetActive(true);
                gameMenuUI.SetActive(true);
            }
            else
            {
                print("Create a popup which says their x, y, z values must not be negative.");
            }
        }
        else
        {
            print("Create a popup which says their formatting was wrong and to try again.");
        }
    }
}
