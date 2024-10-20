using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool gameIsPaused = false;
    public GameObject pauseMenuUI, timeStopIndicator;
    public MainMenu menu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
        menu.UnfreezeTimer();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
        menu.FreezeTimer();
    }
}
