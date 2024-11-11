using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThermalCamAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenu menu;

    public bool entered, exited;

    public ThermalCamera themalCam;
    public POIPortrait themalCamPortrait;

    public Soldier soldier;
    public SoldierPortrait soldierPortrait;
    public TextMeshProUGUI startBoundary, endBoundary, label;
    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
    }
    public ThermalCamAlertLOS Init(Soldier soldier, ThermalCamera themalCam)
    {
        SetSoldierAndCamera(soldier, themalCam);
        return this;
    }
    public void SetSoldierAndCamera(Soldier soldier, ThermalCamera themalCam)
    {
        this.soldier = soldier;
        this.themalCam = themalCam;

        UpdateLabel();
        UpdateStartBoundary();
        UpdateEndBoundary();
        soldierPortrait.Init(soldier);
        themalCamPortrait.Init(themalCam);
    }
    public void UpdateLabel()
    {
        string prefix = "";
        if (entered && !exited) //standard (ended in SR)
        { }
        else if (entered && exited) //glimpse
            prefix = "GLIMPSE ";
        else if (!entered && exited) //retreat
            prefix = "RETREAT ";

        this.label.text = $"{prefix}DETECT";
    }
    public void UpdateStartBoundary()
    {
        Vector3 soldierPosition = HelperFunctions.ConvertPhysicalPosToMathPos(soldier.transform.position);
        string message = $"Start: X:{soldierPosition.x} Y:{soldierPosition.y} Z:{soldierPosition.z}";
        if (soldierPosition == new Vector3(soldier.X, soldier.Y, soldier.Z))
            message += " (Current)";

        startBoundary.text = message;
    }
    public void UpdateEndBoundary()
    {
        Vector3 soldierPosition = HelperFunctions.ConvertPhysicalPosToMathPos(soldier.transform.position);
        string message = $"End: X:{soldierPosition.x} Y:{soldierPosition.y} Z:{soldierPosition.z}";
        if (soldierPosition == new Vector3(soldier.X, soldier.Y, soldier.Z))
            message += " (Current)";

        endBoundary.text = message;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.CreateLOSArrowPair(soldier, themalCam);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        menu.DestroyLOSArrowPair(soldier, themalCam);
    }
}
