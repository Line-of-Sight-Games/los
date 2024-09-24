using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DropthrowPopup : MonoBehaviour
{
    public MainMenu menu;
    public GameObject dropThrowUI, noThrowIndicator;
    public TextMeshProUGUI noThrowIndicatorText;
    public Button throwButton, dropButton;
    public Item itemToDropThrow;
    public ItemIcon itemIconToDropThrow;

    void Update()
    {
        if (Input.anyKey && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            HideDropThrowPopup();
        }
    }
    public void ShowDropThrowPopup()
    {
        if (!menu.activeSoldier.IsAbleToSee())
        {
            throwButton.interactable = false;
            noThrowIndicatorText.text = "Blind";
            noThrowIndicator.SetActive(true);
        }
        else if (!menu.activeSoldier.HandsFreeToThrowItem(itemToDropThrow))
        {
            throwButton.interactable = false;
            noThrowIndicatorText.text = "Hands Full";
            noThrowIndicator.SetActive(true);
        }
        gameObject.SetActive(true);
    }
    public void HideDropThrowPopup()
    {
        throwButton.interactable = true;
        noThrowIndicator.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ThrowButtonClick()
    {
        menu.OpenDropThrowItemUI("throw", itemToDropThrow, itemToDropThrow.whereEquipped, itemIconToDropThrow);
        HideDropThrowPopup();
    }

    public void DropButtonClick()
    {
        menu.OpenDropThrowItemUI("drop", itemToDropThrow, itemToDropThrow.whereEquipped, itemIconToDropThrow);
        HideDropThrowPopup();
    }
}
