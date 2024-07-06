using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIcon : MonoBehaviour
{
    public MainMenu menu;
    public GameObject linkedInventoryPanel;

    public void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
    }

    public InventorySourceIcon Init(GameObject linkedPanel)
    {
        linkedInventoryPanel = linkedPanel;

        return this;
    }

    public void OpenItemPanel()
    {
        menu.OpenInventoryPanel(linkedInventoryPanel);
    }
}
