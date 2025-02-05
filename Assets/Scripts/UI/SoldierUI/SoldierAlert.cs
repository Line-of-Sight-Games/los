using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoldierAlert : MonoBehaviour
{
    public SoundManager soundManager;
    public Soldier soldier;
    public SoldierPortrait soldierPortrait;

    public TextMeshProUGUI title;
    public TextMeshProUGUI damageIndicator;
    public TextMeshProUGUI description;

    public SoldierAlert Init(Soldier soldier, string title, Color titleColour, string description, int preDamage, int postDamage)
    {
        SetSoldier(soldier);
        this.title.text = title;
        this.title.color = titleColour;
        damageIndicator.color = titleColour;
        this.description.text = description;

        if (preDamage >= 0)
        {
            damageIndicator.text = $"{preDamage}";
            if (postDamage >= 0)
                damageIndicator.text += $" -> {postDamage}";
        }

        FileUtility.WriteToReport($"{soldier.soldierName}: {description}"); //write to report
        return this;
    }
    private void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
    }

    public void PlayButtonPress()
    {
        soundManager.PlayButtonPress();
    }

    public void SetSoldier(Soldier initSoldier)
    {
        soldier = initSoldier;
        soldierPortrait.Init(initSoldier);
    }
}


