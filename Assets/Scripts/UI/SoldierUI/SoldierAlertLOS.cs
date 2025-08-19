using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoldierAlertLOS : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool s1Entered, s1Exited, s2Entered, s2Exited; 

    public Soldier s1;
    public TextMeshProUGUI s1Label, s1StartBoundary, s1EndBoundary;
    public SoldierPortrait s1Portrait;
    public GameObject s1BinocularImage;
    public Toggle s1Toggle;

    public Sprite detection1WayLeft, detection1WayRight, detection2Way;

    public Sprite avoidance2WayLeft, avoidance2WayRight;

    public Sprite avoidance1WayLeft, avoidance1WayRight, avoidance2Way;

    public Sprite detectionOverwatch2WayLeft, detectionOverwatch2WayRight, avoidanceOverwatch2WayLeft, avoidanceOverwatch2WayRight;

    public Sprite overwatch1WayLeft, overwatch1WayRight, overwatch2Way; 
        
    public Sprite noDetect2Way, nullArrow;

    public Image arrow;

    public Soldier s2;
    public TextMeshProUGUI s2Label, s2StartBoundary, s2EndBoundary;
    public SoldierPortrait s2Portrait;
    public GameObject s2BinocularImage;
    public Toggle s2Toggle;

    public GameObject causeOfLosCheckS1Object;
    public GameObject causeOfLosCheckS2Object;
    public TextMeshProUGUI causeOfLosCheckS1;
    public TextMeshProUGUI causeOfLosCheckS2;

    public TextMeshProUGUI causeOfToggleStateS1;
    public TextMeshProUGUI causeOfToggleStateS2;

    public SoldierAlertLOS Init(Soldier s1, Soldier s2)
    {
        SetSoldiers(s1, s2);
        return this;
    }
    public void SetSoldiers(Soldier s1, Soldier s2)
    {
        this.s1 = s1;
        this.s2 = s2;

        UpdateS1StartBoundary();
        UpdateS1EndBoundary();
        UpdateS2StartBoundary();
        UpdateS2EndBoundary();
        causeOfLosCheckS1.text = $"{s1.causeOfLosCheck}";
        causeOfLosCheckS2.text = $"{s2.causeOfLosCheck}";
        s1Portrait.Init(s1);
        s2Portrait.Init(s2);
        if (s1.IsUsingBinoculars())
            s1BinocularImage.SetActive(true);
        if (s2.IsUsingBinoculars())
            s2BinocularImage.SetActive(true);
    }
    public void UpdateS1Label(string label)
    {
        s1Label.text = ColourLabel(s1, label);
    }
    public void UpdateS2Label(string label)
    {
        s2Label.text = ColourLabel(s2, label);
    }
    public void UpdateLabel(Soldier s, string label)
    {
        if (s == s1)
            UpdateS1Label(label);
        else if (s == s2)
            UpdateS2Label(label);

        UpdateArrowType();
        UpdateToggles();
    }
    public void UpdateEntered(Soldier s)
    {
        if (s == s1)
            s1Entered = true;
        else if (s == s2)
            s2Entered = true;
    }
    public void UpdateExited(Soldier s)
    {
        if (s == s1)
            s1Exited = true;
        else if (s == s2)
            s2Exited = true;
    }
    public string ColourLabel(Soldier s, string label)
    {
        
        string prefix = "";
        string colorPrefix = "";
        string colorSuffix = "</color>";

        if (s == s1)
        {
            if (s1Entered && !s1Exited) //standard (ended in SR)
            { }
            else if (s1Entered && s1Exited) //glimpse
                prefix = "GLIMPSE ";
            else if (!s1Entered && s1Exited) //retreat
                prefix = "RETREAT ";
        }
        else if (s == s2)
        {
            if (s2Entered && !s2Exited) //standard (ended in SR)
            { }
            else if (s2Entered && s2Exited) //glimpse
                prefix = "GLIMPSE ";
            else if (!s2Entered && s2Exited) //retreat
                prefix = "RETREAT ";
        }

        if (label.Contains("DETECT"))
            colorPrefix = "<color=red>";
        else if (label.Contains("AVOID"))
            colorPrefix = "<color=green>";
        else if (label.Contains("OVERWATCH"))
            colorPrefix = "<color=yellow>";

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
            else if (s1Label.text.Contains("OUT OF SR"))
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
            else if (s1Label.text.Contains("OUT OF SR"))
                arrowType = "avoidance1WayRight";
            else if (s1Label.text.Contains("OVERWATCH"))
                arrowType = "avoidanceOverwatch2WayLeft";
        }
        else if (s2Label.text.Contains("OUT OF SR"))
        {
            if (s1Label.text.Contains("AVOID"))
                arrowType = "avoidance1WayLeft";
            else if (s1Label.text.Contains("DETECT"))
                arrowType = "detection1WayLeft";
            else if (s1Label.text.Contains("OUT OF SR"))
                arrowType = "noDetect2Way";
            else if (s1Label.text.Contains("OVERWATCH"))
                arrowType = "overwatch1WayLeft";
        }
        else if (s2Label.text.Contains("OVERWATCH"))
        {
            if (s1Label.text.Contains("AVOID"))
                arrowType = "avoidanceOverwatch2WayRight";
            else if (s1Label.text.Contains("DETECT"))
                arrowType = "detectionOverwatch2WayRight";
            else if (s1Label.text.Contains("OUT OF SR"))
                arrowType = "overwatch1WayRight";
            else if (s1Label.text.Contains("OVERWATCH"))
                arrowType = "overwatch2Way";
        }

        arrow.sprite = (Sprite)GetType().GetField(arrowType).GetValue(this);
    }
    public void UpdateToggles()
    {
        if (!s1.causeOfLosCheck.Contains("losChange") && !s2.causeOfLosCheck.Contains("losChange")) //no physical change to los at all
        {
            //s1 toggle
            if (s1Label.text.Equals("OUT OF SR")) //s1 is out of s2 SR
            {
                causeOfToggleStateS1.text = $"s1 is out of s2 SR";
                s1Toggle.interactable = false;
            }
            else if ((s1Label.text.Contains("DETECT") || s1Label.text.Contains("AVOID")) && (s2.LOSToTheseSoldiersAndRevealing.Contains(s1.Id) || s2.LOSToTheseSoldiersButHidden.Contains(s1.Id))) //s2 had LOS to s1 previously approved, and no change to s1 physical position
            {
                causeOfToggleStateS1.text = $"s2 had LOS to s1 previously approved, and no change to s1 physical position";
                s1Toggle.isOn = true;
                s1Toggle.interactable = false;
            }
            else if ((s1Label.text.Contains("DETECT") || s1Label.text.Contains("AVOID")) && (s2.NoLOSToTheseSoldiers.Contains(s1.Id))) //s2 had LOS to s1 previously denied, and no change to s1 physical position
            {
                causeOfToggleStateS1.text = $"s2 had LOS to s1 previously denied, and no change to s1 physical position";
                s1Toggle.interactable = false;
            }
            else
            {
                s1Toggle.interactable = true; 
                causeOfToggleStateS1.text = $"GM to evaluate LOS";
            }
                

            //s2 toggle
            if (s2Label.text.Equals("OUT OF SR")) //s2 is out of s1 SR
            {
                causeOfToggleStateS2.text = $"s2 is out of s1 SR";
                s2Toggle.interactable = false;
            }
            else if ((s2Label.text.Contains("DETECT") || s2Label.text.Contains("AVOID")) && (s1.LOSToTheseSoldiersAndRevealing.Contains(s2.Id) || s1.LOSToTheseSoldiersButHidden.Contains(s2.Id))) //s1 had LOS to s2 previously approved, and no change to s2 physical position
            {
                causeOfToggleStateS2.text = $"s1 had LOS to s2 previously approved, and no change to s2 physical position";
                s2Toggle.isOn = true;
                s2Toggle.interactable = false;
            }
            else if ((s2Label.text.Contains("DETECT") || s2Label.text.Contains("AVOID")) && (s1.NoLOSToTheseSoldiers.Contains(s2.Id))) //s1 had LOS to s2 previously denied, and no change to s2 physical position
            {
                causeOfToggleStateS2.text = $"s1 had LOS to s2 previously denied, and no change to s2 physical position";
                s2Toggle.interactable = false;
            }
            else
            {
                causeOfToggleStateS2.text = $"GM to evaluate LOS"; 
                s2Toggle.interactable = true;
            }
        }
        else
        {
            //locking options for s1 toggle
            if (s1Label.text.Equals("OUT OF SR")) //s1 is out of s2 SR
            {
                causeOfToggleStateS1.text = $"s1 is out of s2 SR";
                s1Toggle.interactable = false;
            }
            else if (s2.trenXRayEffect) //s2 has xray effect, guaranteeing LOS to s1
            {
                causeOfToggleStateS1.text = $"s2 has xray effect, guaranteeing LOS to s1";
                s1Toggle.isOn = true;
                s1Toggle.interactable = false;
            }
            else if (s1Label.text.Contains("RETREAT") && (s2.LOSToTheseSoldiersAndRevealing.Contains(s1.Id) || s2.LOSToTheseSoldiersButHidden.Contains(s1.Id))) //s1 was detected or avoided leaving s2 SR and had LOS pre-approved
            {
                causeOfToggleStateS1.text = $"s1 was detected or avoided leaving s2 SR and had LOS pre-approved";
                s1Toggle.isOn = true;
                s1Toggle.interactable = false;
            }
            else if (!s1Label.text.Contains("GLIMPSE") && s2.IsOnOverwatch() && !s2.PhysicalObjectWithinOverwatchCone(s1)) //s1 is not in the overwatch cone of s2 and the alert was NOT a glimpse
            {
                causeOfToggleStateS1.text = $"s1 is not in the overwatch cone of s2 and the alert was NOT a glimpse";
                s1Toggle.interactable = false;
            }
            else if (s1.IsInSmokeBlindZone() && !s2.IsWearingThermalGoggles()) //s1 is in a smoke blind zone and s2 does not have thermal goggles
            {
                causeOfToggleStateS1.text = $"s1 is in a smoke blind zone and s2 does not have thermal goggles";
                s1Toggle.interactable = false;
            }
            else
            {
                causeOfToggleStateS1.text = $"GM to evaluate LOS";
                s1Toggle.interactable = true;
            }
                

            //locking options for s2 toggle
            if (s2Label.text.Equals("OUT OF SR")) //s2 out of s1 SR
            {
                causeOfToggleStateS2.text = $"s2 out of s1 SR";
                s2Toggle.interactable = false;
            }
            else if (s1.trenXRayEffect) //s1 has xray effect, guaranteeing LOS to s2
            {
                causeOfToggleStateS2.text = $"s1 has xray effect, guaranteeing LOS to s2";
                s2Toggle.isOn = true;
                s2Toggle.interactable = false;
            }
            else if (s2Label.text.Contains("RETREAT") && (s1.LOSToTheseSoldiersAndRevealing.Contains(s2.Id) || s1.LOSToTheseSoldiersButHidden.Contains(s2.Id))) //s2 was detected or avoided leaving s1 SR and had LOS pre-approved
            {
                causeOfToggleStateS2.text = $"s2 was detected or avoided leaving s1 SR and had LOS pre-approved";
                s2Toggle.isOn = true;
                s2Toggle.interactable = false;
            }
            else if (!s2Label.text.Contains("GLIMPSE") && s1.IsOnOverwatch() && !s1.PhysicalObjectWithinOverwatchCone(s2)) //s2 is not in the overwatch cone of s1 and the alert was NOT a glimpse
            {
                causeOfToggleStateS2.text = $"s2 is not in the overwatch cone of s1 and the alert was NOT a glimpse";
                s2Toggle.interactable = false;
            }
            else if (s2.IsInSmokeBlindZone() && !s1.IsWearingThermalGoggles()) //s1 is in a smoke blind zone and s2 does not have thermal goggles
            {
                causeOfToggleStateS2.text = $"s1 is in a smoke blind zone and s2 does not have thermal goggles";
                s2Toggle.interactable = false;
            }
            else
            {
                causeOfToggleStateS2.text = $"GM to evaluate LOS";
                s2Toggle.interactable = true;
            }
        }
    }
    public bool LabelIndicatesDetectOrOverwatch(TextMeshProUGUI label)
    {
        if (label.text.Contains("DETECT") || label.text.Contains("OVERWATCH"))
            return true;
        return false;
    }
    public bool LabelIndicatesAvoidance(TextMeshProUGUI label)
    {
        if (label.text.Contains("AVOID"))
            return true;
        return false;
    }

    //pointer hover functions
    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.CreateLOSArrowPair(s1, s2);
        causeOfLosCheckS1Object.SetActive(true);
        causeOfLosCheckS2Object.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MenuManager.Instance.DestroyLOSArrowPair(s1, s2);
        causeOfLosCheckS1Object.SetActive(false);
        causeOfLosCheckS2Object.SetActive(false);
    }
}