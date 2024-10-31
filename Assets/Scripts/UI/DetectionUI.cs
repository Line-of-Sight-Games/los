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

    public void CreateDetectionAlertSoldierSoldier(Soldier detector, Soldier detectee, string detecteeLabel)
    {
        //block duplicate detection alerts being created, stops override mode creating multiple instances during overwriting detection stats
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
        {
            if ((alert.s1 == detector && alert.s2 == detectee) || (alert.s1 == detectee && alert.s2 == detector))
            {
                if (alert != null)
                    Destroy(alert.gameObject);
            }
        }

        SoldierAlertLOS detectionAlert = Instantiate(soldierAlertLOSPrefab, detectionAlertsPanel).Init(detector, detectee);
        allSoldierDetectionAlerts.Add(detectionAlert); 
        
        detectionAlert.UpdateS1StartBoundary();
        detectionAlert.UpdateS2StartBoundary();
        detectionAlert.UpdateS1Label(detecteeLabel);

        /*
        //block invalid selections
        if (detectorLabel.Contains("Not detected"))
            detectionAlert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().interactable = false;
        if (counterLabel.Contains("Not detected"))
            detectionAlert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().interactable = false;

        //force reveal for trenbolone
        if (detector.trenXRayEffect)
        {
            detectionAlert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().isOn = true;
            detectionAlert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().interactable = false;
        }
        if (counter.trenXRayEffect)
        {
            detectionAlert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().isOn = true;
            detectionAlert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().interactable = false;
        }

        //detectionAlert.transform.Find("DetectionArrow").GetComponent<Image>().sprite = (Sprite)GetType().GetField(arrowType).GetValue(this);
        */
       menu.StartCoroutine(menu.OpenDetectionAlertUI());
    }
    public void UpdateDetectionAlertSoldierSoldier(Soldier detector, Soldier counter)
    {
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
        {
            if ((alert.s1 == detector && alert.s2 == counter) || (alert.s1 == counter && alert.s2 == detector))
            {
                alert.UpdateS1EndBoundary();
                alert.UpdateS2EndBoundary();

                /*
                //block invalid selections
                if (detectorLabel.Contains("Not detected"))
                    alert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().interactable = false;
                if (counterLabel.Contains("Not detected"))
                    alert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().interactable = false;

                //force reveal for trenbolone
                if (detector.trenXRayEffect)
                {
                    alert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().isOn = true;
                    alert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().interactable = false;
                }
                if (counter.trenXRayEffect)
                {
                    alert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().isOn = true;
                    alert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().interactable = false;
                }

                //detectionAlert.transform.Find("DetectionArrow").GetComponent<Image>().sprite = (Sprite)GetType().GetField(arrowType).GetValue(this);

                detectionAlert.transform.Find("Detector").Find("DetectorSR").GetComponent<TextMeshProUGUI>().text = "(SR=" + detector.stats.SR.Val + ")";
                detectionAlert.transform.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text = counterLabel;
                //detectionAlert.transform.Find("Detector").Find("DetectorLocation").GetComponent<TextMeshProUGUI>().text = "X:" + detector.X + "\nY:" + detector.Y + "\nZ:" + detector.Z;

                detectionAlert.transform.Find("Counter").Find("CounterSR").GetComponent<TextMeshProUGUI>().text = "(SR=" + counter.stats.SR.Val + ")";
                detectionAlert.transform.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text = detectorLabel;
                //detectionAlert.transform.Find("Counter").Find("CounterLocation").GetComponent<TextMeshProUGUI>().text = "X:" + counter.X + "\nY:" + counter.Y + "\nZ:" + counter.Z;*/
            }
        }
    }
    public void CreateDetectionAlertSoldierClaymore(Soldier detector, Claymore claymore, string detectorLabel, string counterLabel, string arrowType)
    {
        print($"Tried to add detection alert {detector.soldierName} to claymore {claymore.X}, {claymore.Y}, {claymore.Z} with {arrowType} arrow");
        //block duplicate detection alerts being created, stops override mode creating multiple instances during overwriting detection stats
        foreach (ClaymoreAlertLOS alert in allClaymoreDetectionAlerts)
        {
            if (alert.soldier == detector && alert.claymore == claymore)
                Destroy(alert.gameObject);
        }

        ClaymoreAlertLOS claymoreAlert = Instantiate(claymoreAlertLosPrefab, detectionAlertsPanel);
        allClaymoreDetectionAlerts.Add(claymoreAlert);

        //block invalid selections
        claymoreAlert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().interactable = false;

        //force reveal for claymores
        claymoreAlert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().isOn = true;
        claymoreAlert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().interactable = false;

        claymoreAlert.GetComponent<ClaymoreAlertLOS>().SetSoldierAndClaymore(detector, claymore);
        claymoreAlert.transform.Find("DetectionArrow").GetComponent<Image>().sprite = (Sprite)GetType().GetField(arrowType).GetValue(this);

        claymoreAlert.transform.Find("Detector").Find("DetectorSR").GetComponent<TextMeshProUGUI>().text = "(SR=" + detector.stats.SR.Val + ")";
        claymoreAlert.transform.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text = counterLabel;
        claymoreAlert.transform.Find("Detector").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(detector);
        claymoreAlert.transform.Find("Detector").Find("DetectorLocation").GetComponent<TextMeshProUGUI>().text = "X:" + detector.X + "\nY:" + detector.Y + "\nZ:" + detector.Z;

        claymoreAlert.transform.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text = detectorLabel;
        claymoreAlert.transform.Find("Counter").Find("POIPortrait").GetComponent<POIPortrait>().Init(claymore);
        claymoreAlert.transform.Find("Counter").Find("CounterLocation").GetComponent<TextMeshProUGUI>().text = "X:" + claymore.X + "\nY:" + claymore.Y + "\nZ:" + claymore.Z;
    }
    public void ClearAllAlerts()
    {
        foreach (SoldierAlertLOS alert in allSoldierDetectionAlerts)
            Destroy(alert.gameObject);
    }
}
