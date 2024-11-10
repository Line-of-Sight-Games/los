using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoldierAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenu menu;

    public bool entered, exited; 

    public Soldier s1;
    public TextMeshProUGUI s1Label, s1StartBoundary, s1EndBoundary;
    public SoldierPortrait s1Portrait;
    public int s1Xp;

    public Sprite detection1WayLeft, detection1WayRight, avoidance1WayLeft, avoidance1WayRight, detection2Way, avoidance2Way, avoidance2WayLeft, avoidance2WayRight, detectionOverwatch2WayLeft, detectionOverwatch2WayRight, avoidanceOverwatch2WayLeft, avoidanceOverwatch2WayRight, overwatch1WayLeft, overwatch1WayRight, noDetect2Way, nullArrow;

    public Image arrow;

    public Soldier s2;
    public TextMeshProUGUI s2Label, s2StartBoundary, s2EndBoundary;
    public SoldierPortrait s2Portrait;
    public int s2Xp;

    public GameObject causeOfLosCheckObject;
    public TextMeshProUGUI causeOfLosCheck;

    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
        causeOfLosCheck.text = menu.causeOfLosCheck;
    }
    public SoldierAlertLOS Init(Soldier s1, Soldier s2)
    {
        SetSoldiers(s1, s2);
        return this;
    }
    public void SetSoldiers(Soldier s1, Soldier s2)
    {
        this.s1 = s1;
        this.s2 = s2;
        s1Portrait.Init(s1);
        s2Portrait.Init(s2);
    }
    public void UpdateS1Label(string label)
    {
        s1Label.text = label;
    }
    public void UpdateS2Label(string label)
    {
        s2Label.text = label;
    }
    public void UpdateLabel(Soldier s, string label)
    {
        label = ColourLabel(label);

        if (s == s1)
            UpdateS1Label(label);
        else if (s == s2)
            UpdateS2Label(label);

        UpdateArrowType();
    }
    public string ColourLabel(string label)
    {
        string prefix = "";
        string colorPrefix = "";
        string colorSuffix = "</color>";

        if (entered && !exited) //standard (ended in SR)
        { }
        else if (entered && exited) //glimpse
            prefix = "GLIMPSE ";
        else if (!entered && exited) //retreat
            prefix = "RETREAT ";

        if (label.Contains("DETECT"))
            colorPrefix = "<color=red>";
        else if (label.Contains("AVOID"))
            colorPrefix = "<color=green>";
        else if (label.Contains("OVERWATCH"))
            colorPrefix = "<color=yellow>";
        else if (label.Contains("NO DETECT"))
            colorPrefix = "<color=grey>";

        return $"{colorPrefix}{prefix}{label}{colorSuffix}";
    }
    public void UpdateS1StartBoundary()
    {
        Vector3 s1Position = HelperFunctions.ConvertPhysicalPosToMathPos(s1.transform.position);
        string message = $"Start: X:{s1Position.x} Y:{s1Position.y} Z:{s1Position.z}";
        if (s1Position == new Vector3(s1.X, s1.Y, s1.Z))
            message += " (Current)";

        s1StartBoundary.text = message;
    }
    public void UpdateS2StartBoundary()
    {
        Vector3 s2Position = HelperFunctions.ConvertPhysicalPosToMathPos(s2.transform.position);
        string message = $"Start: X:{s2Position.x} Y:{s2Position.y} Z:{s2Position.z}";
        if (s2Position == new Vector3(s2.X, s2.Y, s2.Z))
            message += " (Current)";

        s2StartBoundary.text = message;
    }
    public void UpdateStartBoundary(Soldier s)
    {
        if (s == s1)
            UpdateS1StartBoundary();
        else if (s == s2)
            UpdateS2StartBoundary();
    }
    public void UpdateS1EndBoundary()
    {
        Vector3 s1Position = HelperFunctions.ConvertPhysicalPosToMathPos(s1.transform.position);
        string message = $"End: X:{s1Position.x} Y:{s1Position.y} Z:{s1Position.z}";
        if (s1Position == new Vector3(s1.X, s1.Y, s1.Z))
            message += " (Current)";

        s1EndBoundary.text = message;
    }
    public void UpdateS2EndBoundary()
    {
        Vector3 s2Position = HelperFunctions.ConvertPhysicalPosToMathPos(s2.transform.position);
        string message = $"End: X:{s2Position.x} Y:{s2Position.y} Z:{s2Position.z}";
        if (s2Position == new Vector3(s2.X, s2.Y, s2.Z))
            message += " (Current)";
        
        s2EndBoundary.text = message;
    }
    public void UpdateEndBoundary(Soldier s)
    {
        if (s == s1)
            UpdateS1EndBoundary();
        else if (s == s2)
            UpdateS2EndBoundary();
    }

    public void UpdateArrowType()
    {
        string arrowType = "nullArrow";
        if (s2Label.text.Contains("DETECT"))
        {
            if (s1Label.text.Contains("AVOID"))
                arrowType = "avoidance2WayLeft";
            else if (s1Label.text.Contains("DETECT"))
                arrowType = "detection2Way";
            else if (s1Label.text.Contains("NO DETECT"))
                arrowType = "detection1WayRight";
            else if (s1Label.text.Contains("OVERWATCH"))
                arrowType = "detectionOverwatch2WayLeft";
        }
        else if (s2Label.text.Contains("AVOID"))
        {
            if (s1Label.text.Contains("AVOID"))
                arrowType = "avoidance2Way";
            else if (s1Label.text.Contains("DETECT"))
                arrowType = "avoidance2WayRight";
            else if (s1Label.text.Contains("NO DETECT"))
                arrowType = "avoidance1WayRight";
            else if (s1Label.text.Contains("OVERWATCH"))
                arrowType = "avoidanceOverwatch2WayLeft";
        }
        else if (s2Label.text.Contains("NO DETECT"))
        {
            if (s1Label.text.Contains("AVOID"))
                arrowType = "avoidance1WayLeft";
            else if (s1Label.text.Contains("DETECT"))
                arrowType = "detection1WayLeft";
            else if (s1Label.text.Contains("NO DETECT"))
                arrowType = "noDetect2Way";
            else if (s1Label.text.Contains("OVERWATCH"))
                arrowType = "overwatch1WayLeft";
        }

        arrow.sprite = (Sprite)GetType().GetField(arrowType).GetValue(this);
    }

    //pointer hover functions
    public void OnPointerEnter(PointerEventData eventData)
    {
        menu.CreateLOSArrowPair(s1, s2);
        causeOfLosCheckObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        menu.DestroyLOSArrowPair(s1, s2);
        causeOfLosCheckObject.SetActive(false);

    }
}