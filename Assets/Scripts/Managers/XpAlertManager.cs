
using System.Collections.Generic;
using UnityEngine;

public class XpAlertManager : MonoBehaviour, IDataPersistence
{
    public static XpAlertManager Instance;

    public GameObject xpAlertPrefab;
    public Transform xpAlertContent;

    public List<string> allXpAlertIds = new();

    [SerializeField]
    private bool isDataLoaded;

    public bool IsDataLoaded { get { return isDataLoaded; } }

    private void Awake()
    {
        Instance = this;
    }

    // SAVE DATA
    public void SaveData(ref GameData data)
    {
        if (data.allXpAlertIds == null)
            data.allXpAlertIds = new List<string>();

        if (data.allXpAlertDetails == null)
            data.allXpAlertDetails =
                new Dictionary<string, Dictionary<string, object>>();

        data.allXpAlertIds.Clear();

        XpAlert[] alerts = xpAlertContent
            .GetComponentsInChildren<XpAlert>(true);

        Debug.Log($"XP Alert Manager found {alerts.Length} alerts.");

        foreach (XpAlert alert in alerts)
        {
            if (alert == null)
                continue;

            if (string.IsNullOrEmpty(alert.id))
            {
                Debug.LogWarning(
                    $"XP Alert on {alert.name} has no ID. Skipping."
                );
                continue;
            }

            alert.SaveData(ref data);

            data.allXpAlertIds.Add(alert.id);

            Debug.Log($"Saved XP Alert: {alert.id}");
        }

        Debug.Log($"Total XP Alert IDs: {data.allXpAlertIds.Count}");
        Debug.Log($"Total XP Alert Details: {data.allXpAlertDetails.Count}");
    }

    // LOAD DATA
    public void LoadData(GameData data)
    {
        // Remove alerts from the current game state first
        ClearExistingAlerts();

        Dictionary<string, XpAlert> existingXpAlerts = new();

        foreach (XpAlert xpAlert in FindObjectsByType<XpAlert>())
        {
            if (xpAlert != null && !string.IsNullOrEmpty(xpAlert.id))
                existingXpAlerts[xpAlert.id] = xpAlert;
        }

        allXpAlertIds = data.allXpAlertIds ?? new List<string>();

        foreach (string id in allXpAlertIds)
        {
            XpAlert newXpAlert;

            if (existingXpAlerts.TryGetValue(id, out XpAlert existingXpAlert)
                && existingXpAlert != null)
            {
                newXpAlert = existingXpAlert;
            }
            else
            {
                GameObject newObject = Instantiate(xpAlertPrefab, xpAlertContent);
                newXpAlert = newObject.GetComponent<XpAlert>();
                newXpAlert.id = id;
            }

            newXpAlert.LoadData(data);
        }

        isDataLoaded = true;
    }

    private void ClearExistingAlerts()
    {
        XpAlert[] existingAlerts =
            xpAlertContent.GetComponentsInChildren<XpAlert>(true);

        foreach (XpAlert alert in existingAlerts)
        {
            if (alert != null)
            {
                alert.gameObject.SetActive(false);
                Destroy(alert.gameObject);
            }
        }

        Debug.Log($"Cleared {existingAlerts.Length} existing XP alerts.");
    }
}
