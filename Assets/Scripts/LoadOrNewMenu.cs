using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOrNewMenu : MonoBehaviour
{
    public GameObject loadOrNewMenuUI, confirmMenuUI, createSoldiersUI, startGameUI, soldiersAlreadyCreatedUI;
    private bool menuShowing;
    private FileDataHandler coreDataHandler;

    // Update is called once per frame

    public void Start()
    {
        coreDataHandler = new FileDataHandler(Application.persistentDataPath, "LOSCore.json");
    }

    public void PlayCurrentGame()
    {
        SceneManager.LoadScene("Battlefield");
    }

    public void ConfirmNewGame()
    {
        confirmMenuUI.SetActive(true);
    }

    public void GoBack()
    {
        loadOrNewMenuUI.SetActive(false);
        confirmMenuUI.SetActive(false);
        createSoldiersUI.SetActive(false);
        startGameUI.SetActive(false);
        soldiersAlreadyCreatedUI.SetActive(false);
    } 

    public void PlayNewGame()
    {
        coreDataHandler.Delete();
        SceneManager.LoadScene("Create");
    }

    public void ShowMenu()
    {
        if (coreDataHandler.Load() != null)
        {
            if (coreDataHandler.Load().currentRound > 0)
                loadOrNewMenuUI.SetActive(true); 
            else
                startGameUI.SetActive(true);
        }
        else
        {

            createSoldiersUI.SetActive(true);
        }
        menuShowing = true;
    }

    public void CreateSoldiersDetected()
    {
        if (coreDataHandler.Load() != null)
            soldiersAlreadyCreatedUI.SetActive(true);
        else
            PlayNewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuShowing)
            {
                loadOrNewMenuUI.SetActive(false);
                confirmMenuUI.SetActive(false);
                createSoldiersUI.SetActive(false);
                startGameUI.SetActive(false);
                soldiersAlreadyCreatedUI.SetActive(false);
                menuShowing = false;
            }
        }
    }
}
