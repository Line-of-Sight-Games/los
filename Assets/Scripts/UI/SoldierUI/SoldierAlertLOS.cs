using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoldierAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenu menu;
    public Soldier s1, s2;

    public TextMeshProUGUI detectorLocation, counterLocation;
    private void Awake()
    {
        menu = FindFirstObjectByType<MainMenu>();
    }
    public SoldierAlertLOS Init(Soldier s1, Soldier s2)
    {
        SetSoldiers(s1, s2);
        Vector3 s1Position = HelperFunctions.ConvertPhysicalPosToMathPos(s1.transform.position);
        Vector3 s2Position = HelperFunctions.ConvertPhysicalPosToMathPos(s2.transform.position);
        detectorLocation.text = $"X:{s1Position.x}\nY:{s1Position.y}\nZ:{s1Position.z}";
        counterLocation.text = $"X:{s2Position.x}\nY:{s2Position.y}\nZ:{s2Position.z}";

        return this;
    }
    public void SetSoldiers(Soldier s1, Soldier s2)
    {
        this.s1 = s1;
        this.s2 = s2;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.CreateLOSArrowPair(s1, s2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menu.DestroyLOSArrowPair(s1, s2);
    }
}