using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySourceIcon : MonoBehaviour
{
    public InventorySourcePanel linkedInventoryPanel;
    public Button linkedIconButton;
    public GameObject disabledReason;
    public TextMeshProUGUI disabledReasonText;

    public InventorySourceIcon Init(InventorySourcePanel linkedPanel)
    {
        linkedInventoryPanel = linkedPanel;

        return this;
    }

    public void OpenItemPanel()
    {
        MenuManager.Instance.OpenInventoryPanel(linkedInventoryPanel);
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
