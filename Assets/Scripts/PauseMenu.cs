using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI, timeStopIndicator;
    public MainMenu menu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        if (!menu.overrideView)
        {
            timeStopIndicator.SetActive(false);
            menu.UnfreezeTime();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        if (!menu.overrideView)
        {
            timeStopIndicator.SetActive(true);
            menu.FreezeTime();
        }
    }
}
