using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TraumaAlert : SoldierAlert
{
    public int trauma, rolls, xpOnResist;
    public string range;

    public TextMeshProUGUI traumaIndicator;
    public Toggle traumaToggle;
    public GameObject testButton;
    public TextMeshProUGUI traumaButtonText;

    public TraumaAlert Init(Soldier soldier, int trauma, string description, int rolls, int xpOnResist, string range)
    {
        SetSoldier(soldier);
        this.trauma = trauma;
        traumaIndicator.text = $"{trauma}";
        this.description.text = description;
        this.rolls = rolls;
        this.xpOnResist = xpOnResist;
        this.range = range;

        //block invalid trauma alerts being created
        if (description.Contains("automatic") || description.Contains("Tabun"))
        {
            traumaToggle.isOn = true;
            traumaToggle.interactable = false;
        }
        else if (soldier.IsDesensitised())
        {
            title.text = "<color=blue>DESENSITISED</color>";
            traumaIndicator.gameObject.SetActive(false);
            traumaToggle.interactable = false;
            testButton.SetActive(false);
        }
        else if (soldier.IsResilient())
        {
            title.text = "<color=green>RESILIENT</color>";
            traumaIndicator.gameObject.SetActive(false);
            traumaButtonText.text = "<color=green>Test</color>";
        }

        return this;
    }
    public void TraumatiseSoldier()
    {
        bool resisted = false;

        if (traumaToggle.isOn)
        {
            if (description.text.Contains("automatic"))
            {
                //automatic trauma
                title.text = "TRAUMA GAINED";
                description.text = $"{soldier.soldierName} took {trauma} trauma.";
            }
            else
            {
                //do the trauma check
                for (int i = 0; i < rolls; i++)
                {
                    if (soldier.ResilienceCheck())
                        resisted = true;
                }

                if (resisted)
                {
                    if (description.text.Contains("Tabun"))
                        MenuManager.Instance.AddXpAlert(soldier, xpOnResist, $"Resisted tabun trauma.", true);
                    else
                        MenuManager.Instance.AddXpAlert(soldier, xpOnResist, $"Resisted trauma from death witnessed at {range} range.", true);

                    title.text = "<color=green>TRAUMA RESISTED</color>";
                    description.text = $"{soldier.soldierName} resisted the trauma.";
                }
                else
                {
                    title.text = "TRAUMA GAINED";
                    description.text = $"{soldier.soldierName} failed to resist and took {trauma} trauma.";
                }
            }
        }
        else
        {
            title.text = "<color=white>NO LOS</color>";
            description.text = "No trauma points accrued.";
        }

        testButton.SetActive(false);
        traumaToggle.gameObject.SetActive(false);
    }
}
