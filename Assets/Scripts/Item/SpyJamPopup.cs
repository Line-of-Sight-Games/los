using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpyJamPopup : MonoBehaviour
{
    public MainMenu menu;
    public GameObject useItemUI, jammedIndicator;
    public Button spyButton, jamButton;
    public Item ulfUsed;

    void Update()
    {
        if (Input.anyKey && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            HideSpyJamPopup();
        }
    }
    public void ShowSpyJamPopup()
    {
        if (ulfUsed.IsJammed())
        {
            spyButton.interactable = false;
            jamButton.interactable = false;
            jammedIndicator.SetActive(true);
        }
        gameObject.SetActive(true);
    }
    public void HideSpyJamPopup()
    {
        spyButton.interactable = true;
        jamButton.interactable = true;
        jammedIndicator.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SpyButtonClick()
    {
        menu.OpenUseULFUI("spy", ulfUsed);
        HideSpyJamPopup();
    }

    public void JamButtonClick()
    {
        menu.OpenUseULFUI("jam", ulfUsed);
        HideSpyJamPopup();
    }
}
