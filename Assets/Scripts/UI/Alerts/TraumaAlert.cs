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
                if (description.text.Contains("Commander"))
                    FileUtility.WriteToReport($"{soldier.soldierName} suffers automatic trauma due to commander death ({trauma}tp)"); //write to report
                else if (description.text.Contains("Lastandicide"))
                    FileUtility.WriteToReport($"{soldier.soldierName} suffers automatic trauma due to ally Lastandicide ({trauma}tp)"); //write to report

                //automatic trauma
                title.text = "TRAUMA GAINED";
                description.text = $"{soldier.soldierName} took {trauma} trauma.";
            }
            else
            {
                if (soldier.IsResilient())
                    resisted = true;
                else
                {
                    //do the trauma check
                    for (int i = 0; i < rolls; i++)
                    {
                        if (soldier.ResilienceCheck())
                            resisted = true;
                    }
                }

                if (resisted)
                {
                    if (description.text.Contains("Tabun"))
                    {
                        MenuManager.Instance.AddXpAlert(soldier, xpOnResist, $"Resisted trauma from tabun gas.", true);
                        FileUtility.WriteToReport($"{soldier.soldierName} resists trauma from tabun gas ({trauma}tp)"); //write to report
                    }
                    else
                    {
                        MenuManager.Instance.AddXpAlert(soldier, xpOnResist, $"Resisted trauma from death witnessed at {range} range.", true);
                        FileUtility.WriteToReport($"{soldier.soldierName} resists trauma from death witnessed at {range} range ({trauma}tp)"); //write to report
                    }

                    title.text = "<color=green>TRAUMA RESISTED</color>";
                    description.text = $"{soldier.soldierName} resisted the trauma.";
                }
                else
                {
                    if (description.text.Contains("Tabun"))
                        FileUtility.WriteToReport($"{soldier.soldierName} suffers trauma from tabun gas ({trauma}tp). He is {soldier.GetTraumaState().Replace(", ", "").Trim()}"); //write to report
                    else
                        FileUtility.WriteToReport($"{soldier.soldierName} suffers trauma from death witnessed at {range} range ({trauma}tp). He is {soldier.GetTraumaState().Replace(", ", "").Trim()}"); //write to report

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

        traumaIndicator.gameObject.SetActive(false);
        testButton.SetActive(false);
        traumaToggle.gameObject.SetActive(false);
    }
}
