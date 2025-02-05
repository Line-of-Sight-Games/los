using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpAlert : SoldierAlert
{
    public TextMeshProUGUI xpIndicator;
    public TextMeshProUGUI learnerXpIndicator;
    public Toggle toggle;

    public int xp;
    public bool learnerEnabled;

    public XpAlert Init(Soldier soldier, int xp, string xpDescription, bool learnerEnabled)
    {
        SetSoldier(soldier);
        this.xp = xp;
        xpIndicator.text = $"{xp}";
        description.text = xpDescription;
        this.learnerEnabled = learnerEnabled;

        //force xp event if made in override mode
        if (description.text.Contains("Override"))
        {
            toggle.isOn = true;
            toggle.interactable = false;
        }

        //learner ability
        if (learnerEnabled && soldier.IsLearner())
        {
            learnerXpIndicator.gameObject.SetActive(true);
            learnerXpIndicator.text = $"(+{Mathf.CeilToInt(0.5f * xp)})";
        }

        return this;
    }

    public void Resolve()
    {
        if (toggle.isOn)
        {
            //block override xp from double incrementing
            if (description.text.Contains("Override"))
                soldier.xp -= xp;
            soldier.IncrementXP(xp, learnerEnabled);
            FileUtility.WriteToReport($"{soldier.soldierName} got {xp} xp for: {description}"); //write to report
        }
    }
}
