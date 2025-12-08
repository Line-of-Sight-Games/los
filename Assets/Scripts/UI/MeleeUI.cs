using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeleeUI : MonoBehaviour
{
    public TextMeshProUGUI attackerID;
    public TMP_Dropdown meleeTypeDropdown;
    public Image attackerWeaponImage;
    public TMP_Dropdown targetDropdown;
    public Image defenderWeaponImage;
    public TextMeshProUGUI apCost;

    public GameObject attackerWeaponUI;
    public GameObject defenderWeaponUI;
    public GameObject flankersMeleeAttackerUI;
    public GameObject flankersMeleeDefenderUI;

    public Sprite fist;
    public string meleeChargeIndicator;
    public GameObject meleeConfirmUI, meleeResultUI, meleeBreakEngagementRequestUI;
    public GameObject meleeAlertPrefab;

    public bool clearMeleeFlag;

    public List<Tuple<string, string>> meleeParameters = new();
    public void OpenMeleeUINonCoroutine(string meleeCharge)
    {
        MenuManager.Instance.StartCoroutine(OpenMeleeUI(meleeCharge));
    }
    public IEnumerator OpenMeleeUI(string meleeCharge)
    {
        yield return new WaitUntil(() => MenuManager.Instance.MovementResolvedFlag() && MenuManager.Instance.detectionResolvedFlag);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => MenuManager.Instance.detectionResolvedFlag && MenuManager.Instance.shotResolvedFlag);

        //set attacker
        Soldier attacker = ActiveSoldier.Instance.S;
        attackerID.text = attacker.id;

        meleeChargeIndicator = meleeCharge;

        List<TMP_Dropdown.OptionData> defenderDetails = new();

        //display best attacker weapon
        if (attacker.BestMeleeWeapon != null)
            attackerWeaponImage.sprite = attacker.BestMeleeWeapon.itemImage;
        else
            attackerWeaponImage.sprite = fist;

        //generate target list
        foreach (Soldier s in GameManager.Instance.AllSoldiers())
        {
            TMP_Dropdown.OptionData defender = null;
            if (s.IsAlive() && attacker.IsOppositeTeamAs(s) && s.IsRevealed() && attacker.PhysicalObjectWithinMeleeRadius(s))
            {
                if (attacker.CanSeeInOwnRight(s))
                    defender = new(s.soldierName, s.soldierPortrait, Color.white);
                else
                    defender = new(s.soldierName, s.LoadPortraitTeamsight(s.soldierPortraitText), Color.white);

                defenderDetails.Add(defender);
            }

            if (defender != null)
            {
                //remove option if soldier is engaged and this soldier is not on the engagement list
                if (attacker.IsMeleeEngaged() && !attacker.IsMeleeEngagedWith(s))
                    defenderDetails.Remove(defender);
            }
        }

        if (defenderDetails.Count > 0)
        {
            targetDropdown.AddOptions(defenderDetails);

            Soldier defender = SoldierManager.Instance.FindSoldierByName(targetDropdown.captionText.text);

            //show defender weapon
            if (defender.BestMeleeWeapon != null)
                defenderWeaponImage.sprite = defender.BestMeleeWeapon.itemImage;
            else
                defenderWeaponImage.sprite = fist;

            UpdateMeleeTypeOptions();
            gameObject.SetActive(true);
        }
        else
        {
            ClearMeleeUI();
            MenuManager.Instance.generalAlertUI.Activate("No melee targets found");
        }
    }


    public void ClearMeleeUI()
    {
        clearMeleeFlag = true;
        meleeTypeDropdown.ClearOptions();
        meleeTypeDropdown.value = 0;
        attackerWeaponImage.sprite = null;
        defenderWeaponImage.sprite = null;
        targetDropdown.ClearOptions();
        targetDropdown.value = 0;
        MenuManager.Instance.ClearFlankersUI(flankersMeleeAttackerUI);
        MenuManager.Instance.ClearFlankersUI(flankersMeleeDefenderUI);
        clearMeleeFlag = false;
    }
    public void CloseMeleeUI()
    {
        ClearMeleeUI();
        ClearMeleeConfirmUI();
        gameObject.SetActive(false);
        meleeConfirmUI.SetActive(false);
    }
    public void OpenMeleeConfirmUI()
    {
        if (!meleeTypeDropdown.captionText.text.Contains("Select"))
        {
            if (int.TryParse(apCost.text, out int ap) && ActiveSoldier.Instance.S.CheckAP(ap))
            {
                //find attacker and defender
                Soldier attacker = SoldierManager.Instance.FindSoldierById(attackerID.text);
                Soldier defender = SoldierManager.Instance.FindSoldierByName(targetDropdown.captionText.text);

                int meleeDamage = CalculateMeleeResult(attacker, defender);

                meleeConfirmUI.transform.Find("OptionPanel").Find("Damage").Find("DamageDisplay").GetComponent<TextMeshProUGUI>().text = meleeDamage.ToString();

                //add parameters to equation view
                meleeConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = DisplayMeleeParameters();

                //add rounding to equation view
                meleeConfirmUI.transform.Find("EquationPanel").Find("Rounding").GetComponent<TextMeshProUGUI>().text = $"Rounding: {meleeParameters.Find(tuple => tuple.Item1 == "rounding").Item2}";

                meleeConfirmUI.SetActive(true);
            }
        }
        else
            MenuManager.Instance.generalAlertUI.Activate("Select a melee type");
    }
    public void ClearMeleeConfirmUI()
    {
        //if (shotConfirmUI.activeInHierarchy)
        {
            clearMeleeFlag = true;
            meleeConfirmUI.transform.Find("OptionPanel").Find("Damage").Find("DamageDisplay").GetComponent<TextMeshProUGUI>().text = "";
            clearMeleeFlag = false;
        }
    }
    public void AddMeleeAlert(Soldier attacker, Soldier defender, string damageResult, string controlResult)
    {
        //block duplicate detection alerts being created, stops override mode creating multiple instances during overwriting detection stats
        foreach (Transform child in meleeResultUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if ((child.GetComponent<SoldierAlertDouble>().s1 == attacker && child.GetComponent<SoldierAlertDouble>().s2 == defender) || (child.GetComponent<SoldierAlertDouble>().s1 == defender && child.GetComponent<SoldierAlertDouble>().s2 == attacker))
                Destroy(child.gameObject);

        GameObject meleeAlert = Instantiate(meleeAlertPrefab, meleeResultUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        meleeAlert.GetComponent<SoldierAlertDouble>().SetSoldiers(attacker, defender);
        meleeAlert.transform.Find("Results").Find("DamageResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = damageResult;
        meleeAlert.transform.Find("Results").Find("ControlResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = controlResult;
        meleeAlert.transform.Find("Attacker").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(attacker);
        meleeAlert.transform.Find("Defender").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(defender);
    }
    public IEnumerator OpenMeleeResultUI()
    {
        yield return new WaitUntil(() => MenuManager.Instance.shotResolvedFlag == true);

        meleeResultUI.SetActive(true);
    }
    public void ConfirmMeleeResult()
    {
        ScrollRect meleeScroller = meleeResultUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        Transform meleeAlertList = meleeResultUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
        if (meleeScroller.verticalNormalizedPosition <= 0.05f)
        {
            //destroy all detection alerts after done
            foreach (Transform child in meleeAlertList)
                Destroy(child.gameObject);

            CloseMeleeResultUI();
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }
    public void CloseMeleeResultUI()
    {
        SetMeleeResolvedFlagTo(true);
        meleeResultUI.SetActive(false);
    }
    public void OpenMeleeBreakEngagementRequestUI()
    {
        meleeBreakEngagementRequestUI.SetActive(true);
    }
    public void DenyBreakEngagementRequest()
    {
        meleeTypeDropdown.value = 0;
        CloseMeleeBreakEngagementRequestUI();
    }
    public void AcceptBreakEngagementRequest()
    {
        if (HelperFunctions.OverrideKeyPressed())
            CloseMeleeBreakEngagementRequestUI();
    }
    public void CloseMeleeBreakEngagementRequestUI()
    {
        meleeBreakEngagementRequestUI.SetActive(false);
    }


    public void MeleeTargetDropdownChanged()
    {
        Soldier attacker = SoldierManager.Instance.FindSoldierById(attackerID.text);

        if (!clearMeleeFlag)
            UpdateMeleeTypeOptions();
    }
    public void MeleeTypeDropdownChanged()
    {
        Soldier attacker = SoldierManager.Instance.FindSoldierById(attackerID.text);
        Soldier defender = SoldierManager.Instance.FindSoldierByName(targetDropdown.captionText.text);

        if (!clearMeleeFlag)
        {
            CheckMeleeType();
            if (meleeTypeDropdown.captionText.text.Contains("Attack")) //If it's an actual attack
            {
                UpdateMeleeAP(attacker);
                UpdateMeleeDefenderWeapon(defender);
                UpdateMeleeFlankingAgainstAttacker(attacker, defender);
                UpdateMeleeFlankingAgainstDefender(attacker, defender);
            }
        }
    }
    public void UpdateMeleeAP(Soldier attacker)
    {
        if (meleeTypeDropdown.captionText.text.Contains("Charge"))
            apCost.text = "0";
        else
        {
            if (attacker.IsBrawler())
                apCost.text = "1";
            else
                apCost.text = "2";
        }
    }
    public void UpdateMeleeDefenderWeapon(Soldier defender)
    {
        //show defender weapon
        if (defender.BestMeleeWeapon != null)
            defenderWeaponImage.sprite = defender.BestMeleeWeapon.itemImage;
        else
            defenderWeaponImage.sprite = fist;
    }
    public void UpdateMeleeFlankingAgainstAttacker(Soldier attacker, Soldier defender)
    {
        //clear the flanker ui
        MenuManager.Instance.ClearFlankersUI(flankersMeleeAttackerUI);
        int flankersCount = 0;
        foreach (Soldier s in GameManager.Instance.AllSoldiers())
        {
            if (s.IsAbleToSee() && s.IsOppositeTeamAs(attacker) && !s.IsSelf(defender) && s.PhysicalObjectWithinMeleeRadius(attacker) && s.IsRevealing(attacker))
            {
                //add flanker to ui to visualise
                flankersCount++;
                GameObject flankerPortrait = Instantiate(MenuManager.Instance.possibleFlankerPrefab, flankersMeleeAttackerUI.transform.Find("FlankersPanel"));
                flankerPortrait.GetComponentInChildren<SoldierPortrait>().Init(s);
            }
        }

        //display flankers if there are any
        if (flankersCount > 0)
            MenuManager.Instance.OpenFlankersUI(flankersMeleeAttackerUI);
    }
    public void UpdateMeleeFlankingAgainstDefender(Soldier attacker, Soldier defender)
    {
        //clear the flanker ui
        MenuManager.Instance.ClearFlankersUI(flankersMeleeDefenderUI);
        int flankersCount = 0;
        if (!defender.IsTactician() || attacker.IsRevoker())
        {
            foreach (Soldier s in GameManager.Instance.AllSoldiers())
            {
                if (s.IsAbleToSee() && s.IsOppositeTeamAs(defender) && !s.IsSelf(attacker) && s.PhysicalObjectWithinMeleeRadius(defender) && s.IsRevealing(defender) && flankersCount < 3)
                {
                    flankersCount++;

                    //add flanker to ui to visualise
                    GameObject flankerPortrait = Instantiate(MenuManager.Instance.possibleFlankerPrefab, flankersMeleeDefenderUI.transform.Find("FlankersPanel"));
                    flankerPortrait.GetComponentInChildren<SoldierPortrait>().Init(s);
                }
            }

            //display flankers if there are any
            if (flankersCount > 0)
                MenuManager.Instance.OpenFlankersUI(flankersMeleeDefenderUI);
        }
    }
    public void UpdateMeleeTypeOptions()
    {
        Soldier attacker = SoldierManager.Instance.FindSoldierById(attackerID.text);
        Soldier defender = SoldierManager.Instance.FindSoldierByName(targetDropdown.captionText.text);

        //reset dropdown
        meleeTypeDropdown.ClearOptions();
        meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();

        List<TMP_Dropdown.OptionData> meleeTypeDetails = new()
        {
            new TMP_Dropdown.OptionData("Select action..."),
            new TMP_Dropdown.OptionData(meleeChargeIndicator),
            new TMP_Dropdown.OptionData("Engagement Only"),
        };

        //block engagement if melee engaged
        if (attacker.IsMeleeEngaged())
            meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Engagement Only");

        //add options for disengage or request disengage if melee engaged
        if (defender.controlledBySoldiersList.Contains(attacker.id))
            meleeTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Disengage</color>"));
        else if (defender.controllingSoldiersList.Contains(attacker.id))
            meleeTypeDetails.Add(new TMP_Dropdown.OptionData("<color=red>Request Disengage</color>"));

        meleeTypeDropdown.AddOptions(meleeTypeDetails);

        //block attack option if attacker has riot shield - force only disengage or disengage request
        if (attacker.HasActiveRiotShield())
            meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add(meleeChargeIndicator);

        //block attacks against a soldier with a riot shield
        if (defender.HasActiveRiotShield())
            meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("1"); //grey out attack option
        else
            meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Remove("1");

        CheckMeleeType();
    }
    public void CheckMeleeType()
    {
        if (meleeTypeDropdown.captionText.text.Contains("Attack"))
        {
            attackerWeaponUI.SetActive(true);
            defenderWeaponUI.SetActive(true);
            flankersMeleeAttackerUI.SetActive(true);
            flankersMeleeDefenderUI.SetActive(true);
        }
        else
        {
            attackerWeaponUI.SetActive(false);
            defenderWeaponUI.SetActive(false);
            flankersMeleeAttackerUI.SetActive(false);
            flankersMeleeDefenderUI.SetActive(false);
        }

        if (meleeTypeDropdown.captionText.text.Contains("Request"))
            OpenMeleeBreakEngagementRequestUI();
    }
    public float AttackerMeleeSkill(Soldier attacker)
    {
        int juggernautBonus = 0;
        float inspirerBonus, attackerMeleeSkill = attacker.stats.M.Val;

        //apply JA debuff
        if (attacker.IsWearingJuggernautArmour(false))
            juggernautBonus = -1;
        attackerMeleeSkill += juggernautBonus;

        //check for inspirer
        inspirerBonus = attacker.InspirerBonusWeaponMelee();
        attackerMeleeSkill += inspirerBonus;

        //apply sustenance debuff
        attackerMeleeSkill *= AttackerSustenanceMod(attacker);

        //correct negatives
        if (attackerMeleeSkill < 0)
            attackerMeleeSkill = 0;

        meleeParameters.Add(Tuple.Create("aM", $"{attacker.stats.M.Val}"));
        meleeParameters.Add(Tuple.Create("aJuggernaut", $"{juggernautBonus}"));
        meleeParameters.Add(Tuple.Create("aInspirer", $"{inspirerBonus}"));
        return attackerMeleeSkill;
    }
    public float AttackerSustenanceMod(Soldier attacker)
    {
        float sustenanceMod = 0;

        if (attacker.roundsWithoutFood >= 20)
            sustenanceMod = 0.5f;

        meleeParameters.Add(Tuple.Create("aSustenance", $"{1 - sustenanceMod}"));
        return 1 - sustenanceMod;
    }
    public int AttackerWeaponDamage(Item weapon)
    {
        int attackerWeaponDamage;

        //if fist selected
        if (weapon == null)
            attackerWeaponDamage = 1;
        else
            attackerWeaponDamage = weapon.meleeDamage;

        meleeParameters.Add(Tuple.Create("aWep", $"{attackerWeaponDamage}"));
        return attackerWeaponDamage;
    }
    public float AttackerHealthMod(Soldier attacker)
    {
        float attackerHealthMod;
        if (attacker.IsLastStand())
            attackerHealthMod = 0.6f;
        else if (attacker.hp <= attacker.stats.H.Val / 2)
            attackerHealthMod = 0.20f;
        else if (attacker.hp < attacker.stats.H.Val)
            attackerHealthMod = 0.06f;
        else
            attackerHealthMod = 0f;

        meleeParameters.Add(Tuple.Create("aHP", $"{1 - attackerHealthMod}"));
        return 1 - attackerHealthMod;
    }
    public float AttackerTerrainMod(Soldier attacker)
    {
        float attackerTerrainMod;
        if (attacker.IsOnNativeTerrain())
            attackerTerrainMod = -0.4f;
        else if (attacker.IsOnOppositeTerrain())
            attackerTerrainMod = 0.2f;
        else
            attackerTerrainMod = 0f;

        meleeParameters.Add(Tuple.Create("aTer", $"{1 - attackerTerrainMod}"));
        return 1 - attackerTerrainMod;
    }
    public float FlankingAgainstAttackerMod()
    {
        int flankersCount = 0;
        foreach (Transform child in flankersMeleeAttackerUI.transform.Find("FlankersPanel"))
            if (child.GetComponentInChildren<Toggle>().isOn)
                flankersCount++;

        float attackerFlankingMod = flankersCount switch
        {
            0 => 0f,
            1 => 0.16f,
            2 => 0.46f,
            3 or _ => 0.8f
        };

        meleeParameters.Add(Tuple.Create("aFlank", $"{1 - attackerFlankingMod}"));
        return 1 - attackerFlankingMod;
    }
    public float AttackerKdMod(Soldier attacker)
    {
        float kdMod;
        int kd = attacker.GetKd();

        if (kd != 0)
            kdMod = -(2 * kd * 0.01f);
        else
            kdMod = 0;

        //report parameters
        meleeParameters.Add(Tuple.Create("kd", $"{1 - kdMod}"));

        return 1 - kdMod;
    }
    public float AttackerStrengthMod(Soldier attacker)
    {
        float strengthMod = attacker.stats.Str.Val;
        strengthMod *= 0.2f;

        meleeParameters.Add(Tuple.Create("aStr", $"{attacker.stats.Str.Val}"));
        return strengthMod;
    }
    public float DefenderMeleeSkill(Soldier defender)
    {
        int juggernautBonus = 0;
        float defenderMeleeSkill = defender.stats.M.Val;

        //apply JA debuff
        if (defender.IsWearingJuggernautArmour(false))
            juggernautBonus = -1;
        defenderMeleeSkill += juggernautBonus;

        //apply sustenance debuff
        defenderMeleeSkill *= DefenderSustenanceMod(defender);

        //correct negatives
        if (defenderMeleeSkill < 0)
            defenderMeleeSkill = 0;

        meleeParameters.Add(Tuple.Create("dM", $"{defender.stats.M.Val}"));
        meleeParameters.Add(Tuple.Create("dJuggernaut", $"{juggernautBonus}"));
        return defenderMeleeSkill;
    }
    public float DefenderSustenanceMod(Soldier defender)
    {
        float sustenanceMod = 0;

        if (defender.roundsWithoutFood >= 20)
            sustenanceMod = 0.5f;

        meleeParameters.Add(Tuple.Create("dSustenance", $"{1 - sustenanceMod}"));
        return 1 - sustenanceMod;
    }
    public int DefenderWeaponDamage(Item weapon)
    {
        int defenderWeaponDamage;

        //if fist selected
        if (weapon == null)
            defenderWeaponDamage = 1;
        else
            defenderWeaponDamage = weapon.meleeDamage;

        meleeParameters.Add(Tuple.Create("dWep", $"{defenderWeaponDamage}"));
        return defenderWeaponDamage;
    }
    public float ChargeModifier()
    {
        float chargeMod;

        chargeMod = meleeTypeDropdown.captionText.text switch
        {
            "Full Charge Attack" => 1.9f,
            "Half Charge Attack" => 1.4f,
            "3cm Charge Attack" => 1.1f,
            "Static Attack" or _ => 0f,
        };

        meleeParameters.Add(Tuple.Create("charge", $"{chargeMod}"));
        return chargeMod;
    }
    public float DefenderHealthMod(Soldier defender)
    {
        float defenderHealthMod;
        if (defender.IsLastStand())
            defenderHealthMod = 0.8f;
        else if (defender.hp <= defender.stats.H.Val / 2)
            defenderHealthMod = 0.4f;
        else if (defender.hp < defender.stats.H.Val)
            defenderHealthMod = 0.2f;
        else
            defenderHealthMod = 0f;

        meleeParameters.Add(Tuple.Create("dHP", $"{1 - defenderHealthMod}"));
        return 1 - defenderHealthMod;
    }
    public float DefenderTerrainMod(Soldier defender)
    {
        float defenderTerrainMod;
        if (defender.IsOnNativeTerrain())
            defenderTerrainMod = -0.4f;
        else if (defender.IsOnOppositeTerrain())
            defenderTerrainMod = 0.4f;
        else
            defenderTerrainMod = 0f;

        meleeParameters.Add(Tuple.Create("dTer", $"{1 - defenderTerrainMod}"));
        return 1 - defenderTerrainMod;
    }
    public float FlankingAgainstDefenderMod(Soldier defender)
    {
        float defenderFlankingMod = 0;
        if (!defender.IsTactician())
        {
            int flankersCount = 0;
            foreach (Transform child in flankersMeleeDefenderUI.transform.Find("FlankersPanel"))
                if (child.GetComponentInChildren<Toggle>().isOn)
                    flankersCount++;

            defenderFlankingMod = flankersCount switch
            {
                0 => 0f,
                1 => 0.26f,
                2 => 0.56f,
                3 or _ => 0.86f
            };
        }

        meleeParameters.Add(Tuple.Create("dFlank", $"{1 - defenderFlankingMod}"));
        return 1 - defenderFlankingMod;
    }
    public float DefenderStrengthMod(Soldier defender)
    {
        float strengthMod = defender.stats.Str.Val;
        strengthMod *= 0.2f;

        meleeParameters.Add(Tuple.Create("dStr", $"{defender.stats.Str.Val}"));
        return strengthMod;
    }
    public float AttackerSuppressionMod(Soldier soldier)
    {
        float suppressionMod = soldier.GetSuppression() / 100f;

        meleeParameters.Add(Tuple.Create("aSuppression", $"{1 - suppressionMod}"));
        return 1 - suppressionMod;
    }
    public float DefenderSuppressionMod(Soldier soldier)
    {
        float suppressionMod = soldier.GetSuppression() / 100f;

        meleeParameters.Add(Tuple.Create("dSuppression", $"{1 - suppressionMod}"));
        return 1 - suppressionMod;
    }
    public float AttackerFightMod(Soldier soldier)
    {
        float fightMod = 0;

        if (soldier.FightActive())
            fightMod += 0.5f * soldier.stats.F.Val;
        else if (soldier.AvengingActive()) //avenger ability
            fightMod += 0.5f * (soldier.stats.F.Val - 1);

        meleeParameters.Add(Tuple.Create("aFight", $"{fightMod}"));
        return fightMod;
    }
    public float DefenderFightMod(Soldier soldier)
    {
        float fightMod = 0;

        if (soldier.FightActive())
            fightMod += 0.5f * soldier.stats.F.Val;
        else if (soldier.AvengingActive()) //avenger ability
            fightMod += 0.5f * (soldier.stats.F.Val - 1);

        meleeParameters.Add(Tuple.Create("dFight", $"{fightMod}"));
        return fightMod;
    }
    public int CalculateMeleeResult(Soldier attacker, Soldier defender)
    {
        //destroy old melee parameters
        meleeParameters.Clear();

        float meleeDamage;
        int meleeDamageFinal;
        Item attackerWeapon = attacker.BestMeleeWeapon;
        Item defenderWeapon = defender.BestMeleeWeapon;
        int bloodrageMultiplier = 1;

        //if it's a normal attack
        if (meleeTypeDropdown.captionText.text.Contains("Attack"))
        {
            meleeDamage = (AttackerMeleeSkill(attacker) + AttackerWeaponDamage(attackerWeapon)) * AttackerHealthMod(attacker) * AttackerTerrainMod(attacker) * AttackerKdMod(attacker) * FlankingAgainstAttackerMod() * AttackerSuppressionMod(attacker) + AttackerStrengthMod(attacker) - ((DefenderMeleeSkill(defender) + DefenderWeaponDamage(defenderWeapon) + ChargeModifier()) * DefenderHealthMod(defender) * DefenderTerrainMod(defender) * FlankingAgainstDefenderMod(defender) * DefenderSuppressionMod(defender) + DefenderStrengthMod(defender)) - DefenderFightMod(defender) + AttackerFightMod(attacker);

            //check bloodletter damage bonus
            if (meleeDamage > 0 && attacker.IsBloodRaged() && !defender.IsRevoker())
                bloodrageMultiplier = 2;

            meleeParameters.Add(Tuple.Create("bloodrage", $"{bloodrageMultiplier}"));
            meleeDamage *= bloodrageMultiplier;

            //rounding based on R
            if (attacker.stats.R.Val > defender.stats.R.Val)
            {
                meleeParameters.Add(Tuple.Create("rounding", "Attacker favoured."));
                meleeDamageFinal = Mathf.CeilToInt(meleeDamage);
            }
            else if (attacker.stats.R.Val < defender.stats.R.Val)
            {
                meleeParameters.Add(Tuple.Create("rounding", "Defender favoured."));
                meleeDamageFinal = Mathf.FloorToInt(meleeDamage);
            }
            else
            {
                meleeParameters.Add(Tuple.Create("rounding", "Neither favoured."));
                meleeDamageFinal = Mathf.RoundToInt(meleeDamage);
            }
        }
        else
        {
            meleeParameters.Add(Tuple.Create("rounding", "N/A"));
            meleeDamageFinal = 0;
        }

        return meleeDamageFinal;
    }
    public void EstablishController(Soldier controller, Soldier s2)
    {
        BreakMeleeEngagement(controller, s2);
        controller.controllingSoldiersList.Add(s2.id);
        controller.controlledBySoldiersList.Remove(s2.id);
        s2.controlledBySoldiersList.Add(controller.id);
        s2.controllingSoldiersList.Remove(controller.id);

        MenuManager.Instance.StartCoroutine(OpenMeleeResultUI());
    }
    public void EstablishNoController(Soldier s1, Soldier s2)
    {
        BreakMeleeEngagement(s1, s2);

        MenuManager.Instance.StartCoroutine(OpenMeleeResultUI());
    }
    public string DetermineMeleeController(Soldier attacker, Soldier defender, bool counterattack, bool disengage)
    {
        string controlResult;

        if (defender.IsDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=red>No-one Controlling\n(" + defender.soldierName + " Dead)</color>";
        }
        else if (attacker.IsDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=red>No-one Controlling\n(" + attacker.soldierName + " Dead)</color>";
        }
        else if (defender.IsUnconscious())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=blue>No-one Controlling\n(" + defender.soldierName + " Unconscious)</color>";
        }
        else if (attacker.IsUnconscious())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=blue>No-one Controlling\n(" + attacker.soldierName + " Unconscious)</color>";
        }
        else if (defender.IsPlayingDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + defender.soldierName + " Playdead)</color>";
        }
        else if (attacker.IsPlayingDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + attacker.soldierName + " Playdead)</color>";
        }
        else if (defender.IsBroken())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + defender.soldierName + " Broken)</color>";
        }
        else if (counterattack)
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + defender.soldierName + " Counterattacked)</color>";
        }
        else if (disengage)
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + attacker.soldierName + " Disengaged)</color>";
        }
        else
        {
            if (attacker.stats.R.Val > defender.stats.R.Val)
            {
                EstablishController(attacker, defender);
                controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher R)</color>";
            }
            else if (attacker.stats.R.Val < defender.stats.R.Val)
            {
                EstablishController(defender, attacker);
                controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher R)</color>";
            }
            else
            {
                if (attacker.stats.Str.Val > defender.stats.Str.Val)
                {
                    EstablishController(attacker, defender);
                    controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher Str)</color>";
                }
                else if (attacker.stats.Str.Val < defender.stats.Str.Val)
                {
                    EstablishController(defender, attacker);
                    controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher Str)</color>";
                }
                else
                {
                    if (attacker.stats.M.Val > defender.stats.M.Val)
                    {
                        EstablishController(attacker, defender);
                        controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher M)</color>";
                    }
                    else if (attacker.stats.M.Val < defender.stats.M.Val)
                    {
                        EstablishController(defender, attacker);
                        controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher M)</color>";
                    }
                    else
                    {
                        if (attacker.stats.F.Val > defender.stats.F.Val)
                        {
                            EstablishController(attacker, defender);
                            controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher F)</color>";
                        }
                        else if (attacker.stats.F.Val < defender.stats.F.Val)
                        {
                            EstablishController(defender, attacker);
                            controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher )</color>";
                        }
                        else
                        {
                            EstablishNoController(attacker, defender);
                            controlResult = "<color=orange>No-one Controlling\n(Evenly Matched)</color>";
                        }
                    }
                }
            }
        }
        return controlResult;
    }
    public void BreakAllControllingMeleeEngagments(Soldier s1)
    {
        List<string> engagedSoldiersList = new();

        foreach (string soldierId in s1.controllingSoldiersList)
            engagedSoldiersList.Add(soldierId);

        foreach (string soldierId in engagedSoldiersList)
            BreakMeleeEngagement(s1, SoldierManager.Instance.FindSoldierById(soldierId));
    }
    public void BreakMeleeEngagement(Soldier s1, Soldier s2)
    {
        s1.controllingSoldiersList.Remove(s2.id);
        s1.controlledBySoldiersList.Remove(s2.id);
        s2.controllingSoldiersList.Remove(s1.id);
        s2.controlledBySoldiersList.Remove(s1.id);
    }
    public void ConfirmMelee()
    {
        Soldier attacker = SoldierManager.Instance.FindSoldierById(attackerID.text);
        Soldier defender = SoldierManager.Instance.FindSoldierByName(targetDropdown.captionText.text);

        if (int.TryParse(apCost.text, out int ap) && ActiveSoldier.Instance.S.CheckAP(ap))
        {
            SetMeleeResolvedFlagTo(false);
            ActiveSoldier.Instance.S.DeductAP(ap);

            FileUtility.WriteToReport($"{attacker.soldierName} starting melee attack on {defender.soldierName}"); //write to report

            //determine if damage is from melee or melee charge
            List<string> damageType = new() { "Melee" };
            if (ap == 0)
                damageType.Add("Charge");

            string damageMessage;
            bool counterattack = false, instantKill = false, loudAction = true, disengage = false;

            //engagement only options
            if (meleeTypeDropdown.captionText.text.Equals("Engagement Only"))
            {
                damageMessage = "<color=orange>No Damage\n(Engagement Only)</color>";
                //loudAction = false;
            }
            else if (meleeTypeDropdown.captionText.text.Contains("Disengage"))
            {
                damageMessage = "<color=orange>No Damage\n(Disengagement)</color>";
                disengage = true;
                //loudAction = false;
            }
            else
            {
                //instant kill scenarios
                if (attacker.IsHidden() && attacker.stats.M.Val > defender.stats.M.Val) //stealth kill
                {
                    damageMessage = "<color=green>INSTANT KILL\n(Stealth Attack)</color>";
                    instantKill = true;
                    loudAction = false;
                }
                else if (defender.IsOnOverwatch() && attacker.stats.M.Val > defender.stats.M.Val) //overwatcher kill
                {
                    damageMessage = "<color=green>INSTANT KILL\n(Overwatcher)</color>";
                    instantKill = true;
                    loudAction = false;
                }
                else if (defender.IsUnconscious()) //unconscious kill
                {
                    damageMessage = "<color=green>INSTANT KILL\n(Unconscious)</color>";
                    instantKill = true;
                    loudAction = false;
                }
                else if (defender.IsPlayingDead()) //playdead kill
                {
                    damageMessage = "<color=green>INSTANT KILL\n(Playdead)</color>";
                    instantKill = true;
                    loudAction = false;
                }
                else
                {
                    //calculate melee result
                    int meleeDamage = CalculateMeleeResult(attacker, defender);

                    //drop impractical handheld items
                    attacker.DropWeakerHandheldItem();
                    defender.DropWeakerHandheldItem();

                    //play melee success sfx
                    if (DataPersistenceManager.Instance.lozMode && attacker.IsZombie())
                    {
                        SoundManager.Instance.PlayZombieAttack(attacker); //play loz melee success sfx
                    }

                    if (meleeDamage > 0)
                    {
                        if (damageType.Contains("Charge"))
                            SoundManager.Instance.PlayMeleeResolution("successCharge"); //play melee success charge sfx
                        else
                            SoundManager.Instance.PlayMeleeResolution("successStatic"); //play melee success static sfx

                        if (attacker.IsWearingExoArmour() && !defender.IsWearingJuggernautArmour(false)) //exo kill on standard man
                        {
                            damageMessage = "<color=green>INSTANT KILL\n(Exo Armour)</color>";
                            instantKill = true;
                        }
                        else
                        {
                            if (defender.IsWearingJuggernautArmour(false) && !attacker.IsWearingExoArmour())
                                damageMessage = "<color=orange>No Damage\n(Juggernaut Immune)</color>";
                            else
                                damageMessage = "<color=green>Successful Attack\n(" + meleeDamage + " Damage)</color>";
                            defender.TakeDamage(attacker, meleeDamage, false, damageType, Vector3.zero);
                            attacker.BrawlerMeleeHitReward();
                        }

                        //add xp to attacker for successful melee attack
                        MenuManager.Instance.AddXpAlert(attacker, 1, $"Successful melee attack on {defender.soldierName}.", false);
                    }
                    else if (meleeDamage < 0)
                    {
                        damageType.Add("Counter");

                        //play melee counterattack sfx
                        SoundManager.Instance.PlayMeleeResolution("counter");

                        counterattack = true;
                        meleeDamage *= -1;
                        damageMessage = "<color=red>Counterattacked\n(" + meleeDamage + " Damage)</color>";
                        attacker.TakeDamage(defender, meleeDamage, false, damageType, Vector3.zero);

                        //push a zero damage attack to the defender even if counterattcking to trigger abilities
                        defender.TakeDamage(attacker, 0, true, damageType, Vector3.zero);

                        //add xp to defender for successful melee counterattack
                        MenuManager.Instance.AddXpAlert(defender, 2 + meleeDamage, $"Melee counterattack attack on {attacker.soldierName} for {meleeDamage} damage.", false);
                    }
                    else
                    {
                        //play melee breakeven sfx
                        SoundManager.Instance.PlayMeleeResolution("breakeven");
                        //play melee breakeven dialogue
                        SoundManager.Instance.PlaySoldierMeleeBreakeven(ActiveSoldier.Instance.S);

                        damageMessage = "<color=orange>No Damage\n(Evenly Matched)</color>";

                        //push a zero damage attack to the defender even if counterattcking to trigger abilities
                        defender.TakeDamage(attacker, 0, true, damageType, Vector3.zero);

                        //add xp to defender for successful melee block
                        MenuManager.Instance.AddXpAlert(defender, 2, $"Melee block against {attacker.soldierName}.", false);
                    }
                }

                //reset bloodrage even if non-successful attack
                attacker.UnsetBloodRage();
            }

            //kill if instantKill
            if (instantKill)
                defender.InstantKill(attacker, new List<string>() { "Melee" });

            //attacker and defender exit cover
            attacker.UnsetCover();
            defender.UnsetCover();

            //attacker and defender exit overwatch
            attacker.UnsetOverwatch();
            defender.UnsetOverwatch();

            //add melee alert
            AddMeleeAlert(attacker, defender, damageMessage, DetermineMeleeController(attacker, defender, counterattack, disengage));

            //trigger loud action
            if (loudAction)
                attacker.PerformLoudAction();

            MenuManager.Instance.StartCoroutine(OpenMeleeResultUI());
            CloseMeleeUI();
        }
    }
    public IEnumerator DetermineMeleeControllerMultiple(Soldier s1)
    {
        if (s1.EngagedSoldiers.Count > 0)
        {
            SetMeleeResolvedFlagTo(false);
            yield return new WaitUntil(() => MenuManager.Instance.MovementResolvedFlag() && MenuManager.Instance.detectionResolvedFlag);
            foreach (Soldier s in s1.EngagedSoldiers)
                AddMeleeAlert(s1, s, "No Damage\n(Engagement Change)", DetermineMeleeController(s1, s, false, false));
        }
    }

    public string DisplayMeleeParameters()
    {
        List<string> colouredParameters = new();
        foreach (Tuple<string, string> param in meleeParameters)
        {

            if (param.Item1 == "aM" || param.Item1 == "aJuggernaut" || param.Item1 == "aInspirer" || param.Item1 == "aWep" || param.Item1 == "aStr" || param.Item1 == "aFight")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "dM" || param.Item1 == "dJuggernaut" || param.Item1 == "dWep" || param.Item1 == "charge" || param.Item1 == "dStr" || param.Item1 == "dFight")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "aSustenance" || param.Item1 == "aHP" || param.Item1 == "aTer" || param.Item1 == "aFlank" || param.Item1 == "kd" || param.Item1 == "aSuppression" || param.Item1 == "bloodrage")
            {
                if (float.Parse(param.Item2) > 1)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 1)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "dSustenance" || param.Item1 == "dHP" || param.Item1 == "dTer" || param.Item1 == "dFlank" || param.Item1 == "dSuppression")
            {
                if (float.Parse(param.Item2) < 1)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) > 1)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
        }

        return $"{colouredParameters.Find(str => str.Contains("aM"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aJuggernaut"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aInspirer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aSustenance"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aWep"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aHP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aTer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aFlank"))} " +
                $"| {colouredParameters.Find(str => str.Contains("kd"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aSuppression"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aStr"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dM"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dJuggernaut"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dInspirer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dSustenance"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dWep"))} " +
                $"| {colouredParameters.Find(str => str.Contains("charge"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dHP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dTer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dFlank"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dSuppression"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dStr"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dFight"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aFight"))} " +
                $"| {colouredParameters.Find(str => str.Contains("bloodrage"))}";
    }
    public void SetMeleeResolvedFlagTo(bool value)
    {
        if (value)
            MenuManager.Instance.UnfreezeTimer();
        else
            MenuManager.Instance.FreezeTimer();

        MenuManager.Instance.meleeResolvedFlag = value;
    }
}
