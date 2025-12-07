using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningMenu : MonoBehaviour
{
    public AudioSource menuMusic;
    public AudioClip losTheme, lozTheme;
    public GameObject startButton, overwriteWarning, activeLogo;
    public TextMeshProUGUI titleText, startButtonText, zombieButtonText;
    public Sprite losLogo, lozLogo;
    private FileDataHandler coreDataHandler;

    public void Start()
    {
        coreDataHandler = new FileDataHandler(Application.persistentDataPath, "LOSCore.json");

        if (ActiveSoldierList())
        {
            startButton.SetActive(true);
            if (ActiveGame())
                startButtonText.text = "C O N T I N U E";
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
        if (ActiveGame())
            overwriteWarning.SetActive(true);
        else
            PlayNewGame();
    }
    public void ZombieClicked()
    {
        if (zombieButtonText.text.Contains("L O Z"))
        {
            menuMusic.clip = lozTheme;
            menuMusic.Play();
            DataPersistenceManager.Instance.lozMode = true;
            titleText.color = Color.white;
            zombieButtonText.text = "L O S   M O D E";
            activeLogo.GetComponent<Image>().sprite = lozLogo;
        }
        else
        {
            menuMusic.clip = losTheme;
            menuMusic.Play();
            DataPersistenceManager.Instance.lozMode = false;
            titleText.color = Color.black;
            zombieButtonText.text = "L O Z   M O D E";
            activeLogo.GetComponent<Image>().sprite = losLogo;
        }
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
            SceneManager.LoadScene("CreateLOZ");
        else
            SceneManager.LoadScene("Create");
    }
    public void GoBack()
    {
        overwriteWarning.SetActive(false);
    }
}
