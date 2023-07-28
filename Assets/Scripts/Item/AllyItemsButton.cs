using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyItemsButton : MonoBehaviour
{
    public Soldier linkedSoldier;
    public AllyItemsPanel linkedItemPanel;
    public MainMenu menu;

    public void Start()
    {
        menu = FindObjectOfType<MainMenu>();
    }

    public AllyItemsButton Init(Soldier soldier, AllyItemsPanel allyItemPanel)
    {
        linkedSoldier = soldier;
        linkedItemPanel = allyItemPanel;
        transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(linkedSoldier);

        return this;
    }

    public void OpenItemPanel()
    {
        menu.OpenAllyItemPanel(linkedItemPanel.gameObject, linkedSoldier);
    }
}
