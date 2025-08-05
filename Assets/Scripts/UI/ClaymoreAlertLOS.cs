using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClaymoreAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Soldier soldier;
    public SoldierPortrait soldierPortrait;

    public Claymore claymore;
    public POIPortrait claymorePortrait;
    
    public ClaymoreAlertLOS Init(Soldier soldier, Claymore claymore)
    {
        SetSoldierAndClaymore(soldier, claymore);
        return this;
    }
    public void SetSoldierAndClaymore(Soldier soldier, Claymore claymore)
    {
        this.soldier = soldier;
        this.claymore = claymore;

        soldierPortrait.Init(soldier);
        claymorePortrait.Init(claymore);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.CreateLOSArrowPair(soldier, claymore);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MenuManager.Instance.DestroyLOSArrowPair(soldier, claymore);
    }
}
