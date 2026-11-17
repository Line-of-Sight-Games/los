
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpAlert : SoldierAlert, IDataPersistence
{
    [NonSerialized] public Dictionary<string, object> details;

    public string id;
    public TextMeshProUGUI xpIndicator;
    public TextMeshProUGUI learnerXpIndicator;
    public TMP_InputField overrideXpIndicator;
    public Toggle toggle;
    public Button editButton;
    public bool editMode;

    public int xp;
    public bool learnerEnabled;

    [SerializeField]
    private bool isDataLoaded;

    public bool IsDataLoaded { get { return isDataLoaded; } }

    // INITIALISE
    public XpAlert Init(Soldier soldier, int xp, string xpDescription, bool learnerEnabled)
    {
        id = Guid.NewGuid().ToString();

        SetSoldier(soldier);

        this.xp = xp;
        description.text = xpDescription;
        this.learnerEnabled = learnerEnabled;

        RefreshDisplay();

        return this;
    }

    // REFRESH UI
    private void RefreshDisplay()
    {
        xpIndicator.text = $"{xp}";

        // Negative XP
        if (xp < 0)
        {
            xpIndicator.color = Color.red;
            toggle.interactable = false;
        }
        else
        {
            xpIndicator.color = Color.green;
            toggle.interactable = true;

            if (description.text.Contains("Override") ||
                description.text.Contains("Damage Event"))
                editButton.gameObject.SetActive(true);
            else
                editButton.gameObject.SetActive(false);
        }

        // Learner ability
        learnerXpIndicator.gameObject.SetActive(false);

        if (learnerEnabled && soldier.IsLearner())
        {
            learnerXpIndicator.gameObject.SetActive(true);
            learnerXpIndicator.SetText($"(+{Mathf.CeilToInt(0.5f * xp)})");
        }
    }

    // EDIT XP
    public void ToggleEditMode()
    {
        editMode = !editMode;
        overrideXpIndicator.gameObject.SetActive(editMode);
        overrideXpIndicator.placeholder.GetComponent<TextMeshProUGUI>().text = $"{xp}";
    }

    // RESOLVE XP
    public void Resolve()
    {
        if (!toggle.isOn)
            return;

        soldier.IncrementXP(xp, learnerEnabled);

        FileUtility.WriteToReport($"{soldier.soldierName} got {xp} xp for: {description.text}");
    }

    // CHANGE XP
    public void ChangeXp()
    {
        if (int.TryParse(overrideXpIndicator.text, out int newXp) && newXp >= 0)
        {
            xp = newXp;
            RefreshDisplay();
        }
    }

    // SAVE DATA
    public void SaveData(ref GameData data)
    {
        details = new Dictionary<string, object>
        {
            { "soldierId", soldier.id },
            { "xp", xp },
            { "description", description.text },
            { "learnerEnabled", learnerEnabled },
            { "toggleIsOn", toggle.isOn }
        };

        if (data.allXpAlertDetails.ContainsKey(id))
            data.allXpAlertDetails.Remove(id);

        data.allXpAlertDetails.Add(id, details);
    }

    // LOAD DATA
    public void LoadData(GameData data)
    {
        if (!data.allXpAlertDetails.TryGetValue(id, out details))
            return;

        // Reconnect to the correct soldier
        string soldierId = (string)details["soldierId"];

        foreach (Soldier s in FindObjectsByType<Soldier>())
        {
            if (s.id == soldierId)
            {
                SetSoldier(s);
                break;
            }
        }

        if (soldier == null)
        {
            Debug.LogWarning($"Could not find soldier {soldierId} for XP alert {id}.");
            return;
        }

        xp = Convert.ToInt32(details["xp"]);
        description.text = (string)details["description"];
        learnerEnabled = Convert.ToBoolean(details["learnerEnabled"]);
        toggle.isOn = Convert.ToBoolean(details["toggleIsOn"]);

        RefreshDisplay();

        isDataLoaded = true;
    }
}
