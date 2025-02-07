using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningMenu : MonoBehaviour
{
    public GameObject startButton, overwriteWarning;
    public TextMeshProUGUI startButtonText;
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
        if (Input.GetKeyDown(KeyCode.Escape))
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
        SceneManager.LoadScene("LoadingScreen");
        LoadingScreen.Instance.LoadSceneWithData("Battlefield");
    }
    public void NewClicked()
    {
        if (ActiveGame())
            overwriteWarning.SetActive(true);
        else
            PlayNewGame();
    }
    public void QuitClicked()
    {
        Application.Quit();
    }
    public void PlayNewGame()
    {
        coreDataHandler.Delete();
        SceneManager.LoadScene("Create");
    }
    public void GoBack()
    {
        overwriteWarning.SetActive(false);
    }
}
