using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageEventUI : MonoBehaviour
{
    public TMP_Dropdown damageEventTypeDropdown;
    public TMP_Dropdown causedByDropdown;
    public TMP_InputField fallInput;
    public TMP_InputField structureHeight;
    public TMP_InputField otherInput;
    public TMP_InputField damageSource;
    public TMP_InputField xPos;
    public TMP_InputField yPos;
    public TMP_InputField zPos;
    public TMP_Dropdown terrainDropdown;
    public GameObject fallDistanceUI;
    public GameObject structureHeightUI;
    public GameObject damageSourceUI;
    public GameObject locationUI;
    public bool clearDamageEventFlag;

    //damage event functions - menu
    public void OpenDamageEventUI()
    {
        MenuManager.Instance.FreezeTimer();

        //generate damage event type dropdown
        List<string> damageEventTypeDetails = new()
        {
            "Fall Damage",
            "Structural Collapse",
            "Other"
        };
        if (ActiveSoldier.Instance.S.IsBloodletter())
        {
            damageEventTypeDetails.Add("<color=green>Bloodletting</color>");

            //block bloodletter if already bloodletted this turn
            if (ActiveSoldier.Instance.S.bloodLettedThisTurn)
                damageEventTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Bloodletting");
        }
        HelperFunctions.PopulateDropdown(damageEventTypeDropdown, damageEventTypeDetails);

        //generate caused by dropdown
        List<string> causedByOptions = GameManager.Instance
            .AllFieldedSoldiers()
            .Select(soldier => soldier.soldierName)
            .ToList();
        HelperFunctions.PopulateDropdown(causedByDropdown, causedByOptions);

        //prefill movement position inputs with current position
        xPos.placeholder.GetComponent<TextMeshProUGUI>().text = ActiveSoldier.Instance.S.X.ToString();
        yPos.placeholder.GetComponent<TextMeshProUGUI>().text = ActiveSoldier.Instance.S.Y.ToString();
        zPos.placeholder.GetComponent<TextMeshProUGUI>().text = ActiveSoldier.Instance.S.Z.ToString();

        gameObject.SetActive(true);
    }
    public void ClearDamageEventUI()
    {
        clearDamageEventFlag = true;
        damageEventTypeDropdown.value = 0;
        causedByDropdown.value = 0;
        fallInput.text = "";
        structureHeight.text = "";
        otherInput.text = "";
        damageSource.text = "";
        xPos.text = "";
        yPos.text = "";
        zPos.text = "";
        terrainDropdown.value = 0;
        clearDamageEventFlag = false;
    }
    public void CloseDamageEventUI()
    {
        MenuManager.Instance.UnfreezeTimer();
        ClearDamageEventUI();
        gameObject.SetActive(false);
    }
    public void CheckDamageEventType()
    {
        transform.Find("FallDistance").gameObject.SetActive(false);
        transform.Find("Other").gameObject.SetActive(false);
        transform.Find("StructureHeight").gameObject.SetActive(false);
        transform.Find("DamageSource").gameObject.SetActive(false);
        transform.Find("Location").gameObject.SetActive(false);

        if (damageEventTypeDropdown.captionText.text.Contains("Fall"))
        {
            fallDistanceUI.SetActive(true);
            locationUI.SetActive(true);
        }
        else if (damageEventTypeDropdown.captionText.text.Contains("Collapse"))
        {
            transform.Find("StructureHeight").gameObject.SetActive(true);
            transform.Find("Location").gameObject.SetActive(true);
        }
        else if (damageEventTypeDropdown.captionText.text.Contains("Other"))
        {
            transform.Find("Other").gameObject.SetActive(true);
            transform.Find("DamageSource").gameObject.SetActive(true);
        }
        else if (damageEventTypeDropdown.captionText.text.Contains("Bloodletting")) { }
    }


    public void ConfirmDamageEvent()
    {
        Soldier target = ActiveSoldier.Instance.S;

        // Resolve caused-by soldier
        Soldier causedBy = SoldierManager.Instance.FindSoldierByName(causedByDropdown.captionText.text);

        // Bloodletting (no XP event)
        if (damageEventTypeDropdown.captionText.text.Contains("Bloodletting"))
        {
            FileUtility.WriteToReport($"{target.soldierName} bloodlets.");

            target.TakeBloodlettingDamage();
            CloseDamageEventUI();
            return;
        }
        else if (damageEventTypeDropdown.captionText.text.Contains("Other") && int.TryParse(otherInput.text, out int otherDamage))
        {
            int damageTaken = target.TakeDamage(causedBy, otherDamage, false, new() { damageSource.text }, Vector3.zero);

            MenuManager.Instance.GenerateFringeDamageXp(causedBy, damageTaken, $"(Damage Event) Fringe damage caused ({damageSource.text}) on {target.soldierName}.");

            CloseDamageEventUI();
            return;
        }
        else if (GetFallOrCollapseLocation(out Tuple<Vector3, string> fallCollapseLocation))
        {
            if (damageEventTypeDropdown.captionText.text.Contains("Fall"))
            {
                int fallDamage = GameManager.Instance.CalculateFallDamage(target, int.Parse(fallInput.text));

                int damageTaken = target.TakeDamage(causedBy, fallDamage, false, new() { "Fall" }, Vector3.zero);

                MenuManager.Instance.GenerateFringeDamageXp(causedBy, damageTaken, $"(Damage Event) Damage caused by a fall on {target.soldierName}.");
            }
            else if (damageEventTypeDropdown.captionText.text.Contains("Collapse"))
            {
                int structureHeightInt = int.Parse(structureHeight.text);

                // Check whether soldier survives
                if (target.StructuralCollapseCheck(structureHeightInt))
                {
                    MenuManager.Instance.AddXpAlert(target, target.stats.R.Val, $"Survived a {structureHeightInt}cm structural collapse.", true);

                    MenuManager.Instance.AddSoldierAlert(target, "EVENT SURVIVED", Color.green, $"Survives a {structureHeightInt}cm structural collapse.", -1, -1);
                }
                else
                {
                    int damageBeforeKill = target.GetFullHP();

                    target.InstantKill(causedBy, new() { "Structural Collapse" });
                    target.SetCrushed();

                    MenuManager.Instance.GenerateFringeDamageXp(causedBy, damageBeforeKill, $"(Damage Event) Damage caused by a structural collapse on {target.soldierName}.");
                }
            }

            // Move actually proceeds
            GameManager.Instance.PerformMove(target, 0, fallCollapseLocation, false, false, string.Empty, true);

            CloseDamageEventUI();
        }
        else
        {
            print("Invalid Input");
        }
    }

    public bool GetFallOrCollapseLocation(out Tuple<Vector3, string> fallCollapseLocation)
    {
        fallCollapseLocation = default;
        if (HelperFunctions.ValidateIntInput(xPos, out int x) && HelperFunctions.ValidateIntInput(yPos, out int y) && HelperFunctions.ValidateIntInput(zPos, out int z) && terrainDropdown.value != 0)
        {
            fallCollapseLocation = Tuple.Create(new Vector3(x, y, z), terrainDropdown.captionText.text);

            return true;
        }

        return false;
    }
}
