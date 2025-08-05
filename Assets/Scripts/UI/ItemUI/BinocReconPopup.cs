using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BinocReconPopup : MonoBehaviour
{
    public Button relocateButton, stopButton;
    public Item binocsUsed;
    public ItemIcon binocsItemIcon;
    public GameObject notEnoughApIndicator;

    void Update()
    {
        if (Input.anyKey && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            HideBinocReconPopup();
        }
    }
    public void ShowBinocReconPopup()
    {
        if (MenuManager.Instance.activeSoldier.ap >= 2)
        {
            relocateButton.interactable = true;
            notEnoughApIndicator.SetActive(false);
        }
        else
        {
            relocateButton.interactable = false;
            notEnoughApIndicator.SetActive(true);
        }
        gameObject.SetActive(true);
    }
    public void HideBinocReconPopup()
    {
        gameObject.SetActive(false);
    }

    public void StopButtonClick()
    {
        binocsUsed.SoldierNestedOn().UnsetUsingBinoculars();
        HideBinocReconPopup();
    }

    public void RelocateButtonClick()
    {
        MenuManager.Instance.activeSoldier.DrainAP();
        MenuManager.Instance.OpenBinocularsUI(binocsUsed, binocsItemIcon, "Recon");
        HideBinocReconPopup();
    }
}
