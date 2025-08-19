using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectionAlertToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject causeOfToggleState;
    public void OnPointerEnter(PointerEventData eventData)
    {
        causeOfToggleState.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        causeOfToggleState.SetActive(false);
    }
}
