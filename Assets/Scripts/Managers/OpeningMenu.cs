using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningMenu : MonoBehaviour
{
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
        if (zombieButtonText.text.Equals("Z O M B I E"))
        {
            titleText.color = Color.white;
            zombieButtonText.text = "N O R M A L";
            activeLogo.GetComponent<Image>().sprite = lozLogo;
        }
        else
        {
            titleText.color = Color.black;
            zombieButtonText.text = "Z O M B I E";
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
        SceneManager.LoadScene("Create");
    }
    public void GoBack()
    {
        overwriteWarning.SetActive(false);
    }
}
