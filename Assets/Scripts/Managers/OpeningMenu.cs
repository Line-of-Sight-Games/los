using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningMenu : MonoBehaviour
{
    public AudioSource menuMusic;
    public AudioClip losTheme, lozTheme;
    public GameObject startButton, overwriteWarning, activeLogo, gameModePopup;
    public TextMeshProUGUI titleText, startButtonText;
    public Sprite losLogo, lozLogo;
    private FileDataHandler coreDataHandler;

    public void Start()
    {
        coreDataHandler = new FileDataHandler(Application.persistentDataPath, "LOSCore.json");

        if (ActiveSoldierList())
        {
            startButton.SetActive(true);
            if (ActiveGame())
            {
                startButtonText.text = "C O N T I N U E";
                if (LOZGame())
                    SetLOZMode();
                else
                    SetLOSMode();
            }
            else
                startButtonText.text = "S T A R T";
        }
        else
            startButton.SetActive(false);
    }
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            overwriteWarning.SetActive(false);
    }
    public bool ActiveGame()
    {
        if (ActiveSoldierList() && coreDataHandler.Load().currentRound > 0)
            return true;
        return false;
    }
    public bool ActiveSoldierList()
    {
        if (coreDataHandler.Load() != null)
            return true;
        return false;
    }
    public void ContinueClicked()
    {
        DataPersistenceManager.Instance.LoadSceneWithData("Battlefield");
    }
    public void NewClicked()
    {
        gameModePopup.SetActive(true);
    }
    public void LOSButtonClick()
    {
        SetLOSMode();
        if (ActiveGame())
            overwriteWarning.SetActive(true);
        else
            PlayNewGame();
        gameModePopup.SetActive(false);
    }
    public void LOZButtonClick()
    {
        SetLOZMode();
        if (ActiveGame())
            overwriteWarning.SetActive(true);
        else
            PlayNewGame();
        gameModePopup.SetActive(false);
    }
    public void SetLOSMode()
    {
        DataPersistenceManager.Instance.lozMode = false;

        menuMusic.clip = losTheme;
        menuMusic.Play();
        titleText.color = Color.black;
        activeLogo.GetComponent<Image>().sprite = losLogo;
    }
    public void SetLOZMode()
    {
        DataPersistenceManager.Instance.lozMode = true;

        menuMusic.clip = lozTheme;
        menuMusic.Play();
        titleText.color = Color.white;
        activeLogo.GetComponent<Image>().sprite = lozLogo;
    }
    public bool LOZGame()
    {
        if (coreDataHandler.Load().lozMode)
            return true;
        return false;
    }
    public void QuitClicked()
    {
        Application.Quit();
    }
    public void PlayNewGame()
    {
        coreDataHandler.Delete();
        DataPersistenceManager.Instance.NewGame(); //delete gamedata

        if (DataPersistenceManager.Instance.lozMode)
        {
            DataPersistenceManager.Instance.gameData.lozMode = true;
            SceneManager.LoadScene("CreateLOZ");
        }
        else
            SceneManager.LoadScene("Create");
    }
    public void GoBack()
    {
        overwriteWarning.SetActive(false);
    }
}
