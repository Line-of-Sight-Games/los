using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpyJamPopup : MonoBehaviour
{
    public GameObject useItemUI, jammedIndicator, inUseIndicator;
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
        if (ulfUsed.IsJamming() || ulfUsed.IsSpying())
        {
            spyButton.interactable = false;
            jamButton.interactable = false;
            inUseIndicator.SetActive(true);
        }
        else if (ulfUsed.IsJammed())
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
        inUseIndicator.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SpyButtonClick()
    {
        MenuManager.Instance.OpenUseULFUI("spy", ulfUsed);
        HideSpyJamPopup();
    }

    public void JamButtonClick()
    {
        MenuManager.Instance.OpenUseULFUI("jam", ulfUsed);
        HideSpyJamPopup();
    }
}