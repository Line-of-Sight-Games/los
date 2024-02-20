using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoldierAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenu menu;
    public Soldier s1, s2;
    private void Awake()
    {
        menu = FindObjectOfType<MainMenu>();
    }
    public void SetSoldiers(Soldier initS1, Soldier initS2)
    {
        s1 = initS1;
        s2 = initS2;
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