using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClaymoreAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenu menu;
    public Soldier soldier;
    public Claymore claymore;
    private void Awake()
    {
        menu = FindObjectOfType<MainMenu>();
    }
    public void SetSoldierAndClaymore(Soldier initSoldier, Claymore initClaymore)
    {
        soldier = initSoldier;
        claymore = initClaymore;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.CreateLOSArrowPair(soldier, claymore);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menu.DestroyLOSArrowPair(soldier, claymore);
    }
}
