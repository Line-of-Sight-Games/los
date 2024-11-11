using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class DetectionUI : MonoBehaviour
{
    public MainMenu menu;
    public Transform detectionAlertsPanel;
    public List<SoldierAlertLOS> allSoldierDetectionAlerts;
    public List<ClaymoreAlertLOS> allClaymoreDetectionAlerts;

    public SoldierAlertLOS soldierAlertLOSPrefab;
    public ClaymoreAlertLOS claymoreAlertLosPrefab;

    public bool LOSAlertAlreadyExists(Soldier s1, Soldier s2)
    {
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
        {
            if ((alert.s1 == s1 && alert.s2 == s2) || (alert.s2 == s1 && alert.s2 == s1)) //alert already exists
                return true;
        }
        return false;
    }
    public bool LOSAlertClaymoreAlreadyExists(Soldier s1, Claymore c)
    {
        foreach (ClaymoreAlertLOS alert  in allClaymoreDetectionAlerts)
        {
            if (alert.soldier == s1 && alert.claymore == c) //alert already exists
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
        menu.StartCoroutine(menu.OpenDetectionAlertUI());

        return detectionAlert;
    }
    public ClaymoreAlertLOS CreateLOSAlertClaymore(Soldier s1, Claymore c)
    {
        //create and add to list
        ClaymoreAlertLOS detectionAlert = Instantiate(claymoreAlertLosPrefab, detectionAlertsPanel).Init(s1, c);
        allClaymoreDetectionAlerts.Add(detectionAlert);

        //try to open detectionUI
        menu.StartCoroutine(menu.OpenDetectionAlertUI());

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
        detectionAlert.entered = true;
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
        detectionAlert.exited = true;
        detectionAlert.UpdateEndBoundary(detectee);
        detectionAlert.UpdateLabel(detectee, detecteeLabel);
    }
    public void LOSAlertSoldierClaymore(Soldier detector, Claymore claymore)
    {
        if (!LOSAlertClaymoreAlreadyExists(detector, claymore))
            CreateLOSAlertClaymore(detector, claymore);
    }
    public void ClearAllAlerts()
    {
        //destory physical objects
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
            Destroy(alert.gameObject);
        foreach (ClaymoreAlertLOS alert in allClaymoreDetectionAlerts)
            Destroy(alert.gameObject);

        //clear lists
        allSoldierDetectionAlerts.Clear();
        allClaymoreDetectionAlerts.Clear();
    }
}
