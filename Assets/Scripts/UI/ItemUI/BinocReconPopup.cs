using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BinocReconPopup : MonoBehaviour
{
    public MainMenu menu;
    public Button relocateButton, stopButton;
    public Item binocsUsed;
    public ItemIcon binocsItemIcon;

    void Update()
    {
        if (Input.anyKey && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            HideBinocReconPopup();
        }
    }
    public void ShowBinocReconPopup()
    {
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
        binocsUsed.SoldierNestedOn().UnsetUsingBinoculars();
        menu.OpenBinocularsUI(binocsUsed, binocsItemIcon);
        
        HideBinocReconPopup();
    }
}
