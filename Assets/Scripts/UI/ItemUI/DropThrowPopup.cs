using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DropthrowPopup : MonoBehaviour
{
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
        if (ActiveSoldier.Instance.S.stats.Str.Val == 0)
        {
            throwButton.interactable = false;
            noThrowIndicatorText.text = "Too Weak";
            noThrowIndicator.SetActive(true);
        }
        if (ActiveSoldier.Instance.S.IsBlind())
        {
            throwButton.interactable = false;
            noThrowIndicatorText.text = "Blind";
            noThrowIndicator.SetActive(true);
        }
        else if (!ActiveSoldier.Instance.S.HandsFreeToThrowItem(itemToDropThrow))
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
        MenuManager.Instance.OpenDropThrowItemUI("throw", itemToDropThrow, itemToDropThrow.whereEquipped, itemIconToDropThrow);
        HideDropThrowPopup();
    }

    public void DropButtonClick()
    {
        MenuManager.Instance.OpenDropThrowItemUI("drop", itemToDropThrow, itemToDropThrow.whereEquipped, itemIconToDropThrow);
        HideDropThrowPopup();
    }
}
