using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIcon : MonoBehaviour
{
    public MainMenu menu;
    public GameObject linkedInventoryPanel;
    public Button linkedIconButton;
    public GameObject disabledReason;
    public TextMeshProUGUI disabledReasonText;

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

    public void Grey(string reason)
    {
        linkedIconButton.interactable = false;
        disabledReason.SetActive(true);
        disabledReasonText.text = reason;
    }
    public void UnGrey()
    {
        linkedIconButton.interactable = true;
        disabledReason.SetActive(false);
    }
}
