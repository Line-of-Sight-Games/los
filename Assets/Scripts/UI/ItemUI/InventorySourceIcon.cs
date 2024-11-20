using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIcon : MonoBehaviour
{
    public MainMenu menu;
    public InventorySourcePanel linkedInventoryPanel;
    public Button linkedIconButton;
    public GameObject disabledReason;
    public TextMeshProUGUI disabledReasonText;

    public InventorySourceIcon Init(InventorySourcePanel linkedPanel)
    {
        menu = FindFirstObjectByType<MainMenu>();
        linkedInventoryPanel = linkedPanel;

        return this;
    }

    public void OpenItemPanel()
    {
        //locater ability
        if (this is InventorySourceIconGoodyBox gbIcon)
            menu.inventorySourceViewOnly = gbIcon.inventorySourceViewOnly;
        else if (this is InventorySourceIconDrugCabinet dcIcon)
            menu.inventorySourceViewOnly = dcIcon.inventorySourceViewOnly;

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
