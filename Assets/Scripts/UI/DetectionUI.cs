using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DetectionUI : MonoBehaviour
{
    public Transform detectionAlertsPanel;
    public List<SoldierAlertLOS> allSoldierDetectionAlerts;
    public List<ClaymoreAlertLOS> allClaymoreDetectionAlerts;
    public List<ThermalCamAlertLOS> allThermalCamDetectionAlerts;

    public SoldierAlertLOS soldierAlertLOSPrefab;
    public ClaymoreAlertLOS claymoreAlertLosPrefab;
    public ThermalCamAlertLOS thermalCamAlertLosPrefab;

    public GameObject illusionistButton;

    public bool illusionistMoveTriggered;

    public void Update()
    {
        if (illusionistMoveTriggered)
        {
            bool illusionistButtonActive = false;
            foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
            {
                if ((ActiveSoldier.Instance.S.Equals(alert.s1) && alert.s1Toggle.isOn && alert.s1Label.text.Contains("DETECT")) || (ActiveSoldier.Instance.S.Equals(alert.s2) && alert.s2Toggle.isOn && alert.s2Label.text.Contains("DETECT")))
                    illusionistButtonActive = true;
            }

            if (illusionistButtonActive)
                illusionistButton.SetActive(true);
            else
                illusionistButton.SetActive(false);
        }
        else
            illusionistButton.SetActive(false);
    }

    //los alerts soldiers
    public bool LOSAlertAlreadyExists(Soldier s1, Soldier s2)
    {
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
        {
            if ((alert.s1 == s1 && alert.s2 == s2) || (alert.s1 == s2 && alert.s2 == s1)) //alert already exists
                return true;
        }
        return false;
    }
    public SoldierAlertLOS ExistingLOSAlert(Soldier s1, Soldier s2)
    {
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
        {
            if ((alert.s1 == s1 && alert.s2 == s2) || (alert.s1 == s2 && alert.s2 == s1)) //alert already exists
                return alert;
        }
        return null;
    }
    public SoldierAlertLOS CreateLOSAlert(Soldier s1, Soldier s2)
    {
        //create and add to list
        SoldierAlertLOS detectionAlert = Instantiate(soldierAlertLOSPrefab, detectionAlertsPanel).Init(s1, s2);
        allSoldierDetectionAlerts.Add(detectionAlert);

        //try to open detectionUI
        MenuManager.Instance.StartCoroutine(MenuManager.Instance.OpenDetectionAlertUI());

        return detectionAlert;
    }
    public void LOSAlertSoldierSoldierStart(Soldier detector, Soldier detectee, string detecteeLabel)
    {
        SoldierAlertLOS detectionAlert;
        if (LOSAlertAlreadyExists(detector, detectee))
            detectionAlert = ExistingLOSAlert(detector, detectee);
        else
            detectionAlert = CreateLOSAlert(detector, detectee);

        //update alert
        detectionAlert.UpdateEntered(detectee);
        detectionAlert.UpdateStartBoundary(detectee);
        detectionAlert.UpdateLabel(detectee, detecteeLabel);
    }
    public void LOSAlertSoldierSoldierStay(Soldier detector, Soldier detectee, string detecteeLabel)
    {
        SoldierAlertLOS detectionAlert;
        if (LOSAlertAlreadyExists(detector, detectee))
            detectionAlert = ExistingLOSAlert(detector, detectee);
        else
            detectionAlert = CreateLOSAlert(detector, detectee);

        //update alert
        detectionAlert.UpdateEndBoundary(detectee);
        detectionAlert.UpdateLabel(detectee, detecteeLabel);
    }
    public void LOSAlertSoldierSoldierEnd(Soldier detector, Soldier detectee, string detecteeLabel)
    {
        SoldierAlertLOS detectionAlert;
        if (LOSAlertAlreadyExists(detector, detectee))
            detectionAlert = ExistingLOSAlert(detector, detectee);
        else
            detectionAlert = CreateLOSAlert(detector, detectee);

        //update alert
        detectionAlert.UpdateExited(detectee);
        detectionAlert.UpdateEndBoundary(detectee);
        detectionAlert.UpdateLabel(detectee, detecteeLabel);
    }







    //los alarts claymore
    public bool LOSAlertClaymoreAlreadyExists(Soldier s1, Claymore c)
    {
        foreach (ClaymoreAlertLOS alert in allClaymoreDetectionAlerts)
        {
            if (alert.soldier == s1 && alert.claymore == c) //alert already exists
                return true;
        }
        return false;
    }
    public ClaymoreAlertLOS CreateLOSAlertClaymore(Soldier s1, Claymore c)
    {
        //create and add to list
        ClaymoreAlertLOS detectionAlert = Instantiate(claymoreAlertLosPrefab, detectionAlertsPanel).Init(s1, c);
        allClaymoreDetectionAlerts.Add(detectionAlert);

        //try to open detectionUI
        MenuManager.Instance.StartCoroutine(MenuManager.Instance.OpenDetectionAlertUI());

        return detectionAlert;
    }
    public void LOSAlertSoldierClaymore(Soldier detector, Claymore claymore)
    {
        if (!LOSAlertClaymoreAlreadyExists(detector, claymore))
            CreateLOSAlertClaymore(detector, claymore);
    }









    //thermal cam alerts
    public bool LOSAlertThermalAlreadyExists(ThermalCamera tc, Soldier s1)
    {
        foreach (ThermalCamAlertLOS alert in allThermalCamDetectionAlerts)
        {
            if (alert.soldier == s1 && alert.themalCam == tc) //alert already exists
                return true;
        }
        return false;
    }
    public ThermalCamAlertLOS ExistingThermalLOSAlert(ThermalCamera tc, Soldier s1)
    {
        foreach (ThermalCamAlertLOS alert in allThermalCamDetectionAlerts)
        {
            if ((alert.soldier == s1 && alert.themalCam == tc)) //alert already exists
                return alert;
        }
        return null;
    }
    public ThermalCamAlertLOS CreateLOSAlertThermal(ThermalCamera tc, Soldier s1)
    {
        //create and add to list
        ThermalCamAlertLOS detectionAlert = Instantiate(thermalCamAlertLosPrefab, detectionAlertsPanel).Init(s1, tc);
        allThermalCamDetectionAlerts.Add(detectionAlert);

        //try to open detectionUI
        MenuManager.Instance.StartCoroutine(MenuManager.Instance.OpenDetectionAlertUI());

        return detectionAlert;
    }
    public void LOSAlertThermalCamSoldierStart(ThermalCamera thermalCam, Soldier detectee)
    {
        ThermalCamAlertLOS detectionAlert;
        if (LOSAlertThermalAlreadyExists(thermalCam, detectee))
            detectionAlert = ExistingThermalLOSAlert(thermalCam, detectee);
        else
            detectionAlert = CreateLOSAlertThermal(thermalCam, detectee);

        //update alert
        detectionAlert.entered = true;
        detectionAlert.UpdateStartBoundary();
        detectionAlert.UpdateLabel();
    }
    public void LOSAlertThermalCamSoldierStay(ThermalCamera thermalCam, Soldier detectee)
    {
        ThermalCamAlertLOS detectionAlert;
        if (LOSAlertThermalAlreadyExists(thermalCam, detectee))
            detectionAlert = ExistingThermalLOSAlert(thermalCam, detectee);
        else
            detectionAlert = CreateLOSAlertThermal(thermalCam, detectee);

        //update alert
        detectionAlert.UpdateEndBoundary();
        detectionAlert.UpdateLabel();
    }
    public void LOSAlertThermalCamSoldierEnd(ThermalCamera thermalCam, Soldier detectee)
    {
        ThermalCamAlertLOS detectionAlert;
        if (LOSAlertThermalAlreadyExists(thermalCam, detectee))
            detectionAlert = ExistingThermalLOSAlert(thermalCam, detectee);
        else
            detectionAlert = CreateLOSAlertThermal(thermalCam, detectee);

        //update alert
        detectionAlert.exited = true;
        detectionAlert.UpdateEndBoundary();
        detectionAlert.UpdateLabel();
    }










    
    //clear alerts
    public void ClearAllAlerts()
    {
        //destory physical objects
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
            Destroy(alert.gameObject);
        foreach (ClaymoreAlertLOS alert in allClaymoreDetectionAlerts)
            Destroy(alert.gameObject);
        foreach (ThermalCamAlertLOS alert in allThermalCamDetectionAlerts)
            Destroy(alert.gameObject);

        //clear lists
        allSoldierDetectionAlerts.Clear();
        allClaymoreDetectionAlerts.Clear();
        allThermalCamDetectionAlerts.Clear();
    }
}
