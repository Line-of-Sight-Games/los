using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIcon : MonoBehaviour
{
    public MainMenu menu;
    public GameObject linkedInventoryPanel;

    public void Start()
    {
        menu = FindObjectOfType<MainMenu>();
    }

    public void OpenItemPanel()
    {
        menu.OpenInventoryPanel(linkedInventoryPanel);
    }
}
