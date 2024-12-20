using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BinocInHandPopup : MonoBehaviour
{
    public MainMenu menu;
    public Button reconButton, flashButton;
    public Item binocsUsed;
    public ItemIcon binocsItemIcon;
    public GameObject notEnoughApIndicator, handsFullIndicator;

    void Update()
    {
        if (Input.anyKey && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            HideBinocInHandPopup();
        }
    }
    public void ShowBinocInHandPopup()
    {
        if (menu.activeSoldier.ap >= 2)
        {
            reconButton.interactable = true;
            flashButton.interactable = true;
            notEnoughApIndicator.SetActive(false);

            if (binocsUsed.whereEquipped.Contains("Hand") && !menu.activeSoldier.HasAHandFree(true))
            {
                reconButton.interactable = false;
                handsFullIndicator.SetActive(true);
            }
            else
            {
                reconButton.interactable = true;
                handsFullIndicator.SetActive(false);
            }
        }
        else
        {
            reconButton.interactable = false;
            flashButton.interactable = false;
            notEnoughApIndicator.SetActive(true);
        }
        gameObject.SetActive(true);
    }
    public void HideBinocInHandPopup()
    {
        gameObject.SetActive(false);
    }

    public void ReconButtonClick()
    {
        menu.activeSoldier.DrainAP();
        menu.OpenBinocularsUI(binocsUsed, binocsItemIcon, "Recon");
        HideBinocInHandPopup();
    }

    public void FlashButtonClick()
    {
        menu.activeSoldier.DeductAP(2);
        menu.OpenBinocularsUI(binocsUsed, binocsItemIcon, "Flash");
        HideBinocInHandPopup();
    }
}
