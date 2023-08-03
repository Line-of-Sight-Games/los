using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyItemsPanel : MonoBehaviour
{
    public MainMenu menu;
    public Soldier linkedSoldier;
    public SoundManager soundManager;

    public AllyItemsPanel Init(Soldier soldier)
    {
        soundManager = FindObjectOfType<SoundManager>();
        menu = FindObjectOfType<MainMenu>();
        linkedSoldier = soldier;

        return this;
    }

    public void CloseAllyItemPanel()
    {
        menu.CloseItemPanel(gameObject);
    }

    public void ButtonSoundTrigger()
    {
        soundManager.PlayButtonPress();
    }
}
