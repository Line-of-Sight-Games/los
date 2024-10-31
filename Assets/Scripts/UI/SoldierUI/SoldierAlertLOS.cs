using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoldierAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenu menu;

    public Soldier s1;
    public TextMeshProUGUI s1Label, s1StartBoundary, s1EndBoundary;
    public SoldierPortrait s1Portrait;
    public int s1Xp;

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
    public void UpdateS1StartBoundary()
    {
        Vector3 s1Position = HelperFunctions.ConvertPhysicalPosToMathPos(s1.transform.position);
        if (s1Position == new Vector3(s1.X, s1.Y, s1.Z))
            s1StartBoundary.text = $"Start: Current Location";
        else
            s1StartBoundary.text = $"Start: X:{s1Position.x} Y:{s1Position.y} Z:{s1Position.z}";
    }
    public void UpdateS1EndBoundary()
    {
        Vector3 s1Position = HelperFunctions.ConvertPhysicalPosToMathPos(s1.transform.position);
        if (s1Position == new Vector3(s1.X, s1.Y, s1.Z))
            s1EndBoundary.text = $"End: Current Location";
        else
            s1EndBoundary.text = $"End: X:{s1Position.x} Y:{s1Position.y} Z:{s1Position.z}";
    }
    public void UpdateS2StartBoundary()
    {
        Vector3 s2Position = HelperFunctions.ConvertPhysicalPosToMathPos(s2.transform.position);
        if (s2Position == new Vector3(s2.X, s2.Y, s2.Z))
            s2StartBoundary.text = $"Start: Current Location";
        else
            s2StartBoundary.text = $"Start: X:{s2Position.x} Y:{s2Position.y} Z:{s2Position.z}";
    }
    public void UpdateS2EndBoundary()
    {
        Vector3 s2Position = HelperFunctions.ConvertPhysicalPosToMathPos(s2.transform.position);
        if (s2Position == new Vector3(s2.X, s2.Y, s2.Z))
            s2EndBoundary.text = $"End: Current Location";
        else
            s2EndBoundary.text = $"End: X:{s2Position.x} Y:{s2Position.y} Z:{s2Position.z}";
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