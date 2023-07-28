using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyItemsPanel : MonoBehaviour
{
    public MainMenu menu;
    public Soldier linkedSoldier;

    public AllyItemsPanel Init(Soldier soldier)
    {
        menu = FindObjectOfType<MainMenu>();
        linkedSoldier = soldier;

        return this;
    }

    public void CloseAllyItemPanel()
    {
        menu.CloseItemPanel(gameObject);
    }
}
