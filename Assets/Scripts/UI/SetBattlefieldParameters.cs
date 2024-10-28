using UnityEngine;
using TMPro;

public class SetBattlefieldParameters : MonoBehaviour, IDataPersistence
{
    public MainMenu menu;
    public MainGame game;
    public Camera cam;
    public Light sun;

    public WeatherGen weather;
    public DipelecGen dipelec;

    public GameObject battlefield, bottomPlane, outlineArea, setupMenuUI, gameMenuUI;
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
    }

    private void Start()
    {

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
        cam.transform.position = new Vector3(x / 2, y * 2, z / 2);
        sun.transform.position = new Vector3(0, y + 1, 0);
    }

    public void Confirm()
    {
        if (menu.ValidateIntInput(xSize, out int x) && menu.ValidateIntInput(ySize, out int y) && menu.ValidateIntInput(zSize, out int z) && menu.ValidateIntInput(maxRoundsInput, out int maxRounds) && menu.ValidateIntInput(turnTimeInput, out int maxTurnTime))
        {
            game.currentRound = 1;
            game.currentTeam = 1;
            game.maxRounds = maxRounds;
            game.maxTurnTime = maxTurnTime * 60;
            game.maxX = x;
            game.maxY = z;
            game.maxZ = y;

            weather.GenerateWeather();
            dipelec.GenerateDipelec();

            DataPersistenceManager.Instance.SaveGame();

            menu.UnfreezeTimer();
            setupMenuUI.SetActive(false);
            gameMenuUI.SetActive(true);
        }
    }
}
