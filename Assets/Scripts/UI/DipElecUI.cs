using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DipElecUI : MonoBehaviour
{
    public bool clearDipelecFlag = false;
    public List<string> allTerminalIds = new();
    public TMP_Dropdown dipElecTerminalDropdown;
    public TMP_Dropdown dipElecTypeDropdown;
    public TMP_Dropdown dipElecLevelDropdown;
    public TextMeshProUGUI successChanceDisplay;
    public TextMeshProUGUI apCost;

    public GameObject levelUI, dipelecResultUI;

    //dipelec functions
    public void OpenDipElecUI()
    {
        //generate terminal list
        List<TMP_Dropdown.OptionData> terminalDetailsList = new();
        foreach (Terminal t in GameManager.Instance.AllTerminals())
        {
            TMP_Dropdown.OptionData terminalDetails;
            if (t.terminalEnabled && ActiveSoldier.Instance.S.PhysicalObjectWithinMeleeRadius(t))
            {
                allTerminalIds.Add(t.Id);
                terminalDetails = new($"X:{t.X} Y:{t.Y} Z:{t.Z}", t.poiPortrait, Color.white);
                terminalDetailsList.Add(terminalDetails);
            }
        }
        dipElecTerminalDropdown.AddOptions(terminalDetailsList);

        UpdateDipElecUI();
        gameObject.SetActive(true);
    }
    public void CloseDipElecUI()
    {
        ClearDipElecUI();
        gameObject.SetActive(false);
    }
    public void ClearDipElecUI()
    {
        clearDipelecFlag = true;
        allTerminalIds.Clear();

        dipElecTerminalDropdown.value = 0;
        dipElecTerminalDropdown.ClearOptions();

        dipElecTypeDropdown.value = 0;
        dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();

        dipElecLevelDropdown.value = 0;
        successChanceDisplay.text = "";
        clearDipelecFlag = false;
    }
    public void OpenDipelecResultUI()
    {
        dipelecResultUI.SetActive(true);
    }
    public void ClearDipelecResultUI()
    {
        foreach (Transform child in dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"))
            Destroy(child.gameObject);
    }
    public void CloseDipelecResultUI()
    {
        if (MenuManager.Instance.OverrideKey())
        {
            if (dipelecResultUI.transform.Find("OptionPanel").Find("Rewards").childCount > 1)
                Destroy(dipelecResultUI.transform.Find("OptionPanel").Find("Rewards").GetChild(dipelecResultUI.transform.Find("OptionPanel").Find("Rewards").childCount - 1).gameObject);
            else
            {
                MenuManager.Instance.UnfreezeTimer();
                ClearDipelecResultUI();
                dipelecResultUI.SetActive(false);
                ActiveSoldier.Instance.S.PerformLoudAction(30);
            }
        }
    }

    //dipelec functions
    public void UpdateDipElecUI()
    {
        if (!clearDipelecFlag)
        {
            //read terminal
            Terminal terminal = POIManager.Instance.FindPOIById(SelectedTerminalId) as Terminal;

            //set dipelec type
            dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();

            if (terminal.terminalType == "Dip Only" || terminal.SoldiersAlreadyHacked.Contains(ActiveSoldier.Instance.S.Id))
                dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Hack");
            if (terminal.terminalType == "Elec Only" || terminal.SoldiersAlreadyNegotiated.Contains(ActiveSoldier.Instance.S.Id))
                dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Negotiation");

            if (dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Contains("Hack") && dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Contains("Negotiation"))
                dipElecTypeDropdown.value = 2;
            else if (dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Contains("Negotiation"))
                dipElecTypeDropdown.value = 1;
        }
        UpdateDipElecRewardAndChance();
    }
    public void UpdateDipElecRewardAndChance()
    {
        //set level dropdown and success chance
        dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();

        if (dipElecTypeDropdown.value == 0)
        {
            levelUI.SetActive(true);
            for (int i = 1; i <= 6; i++)
                if (ActiveSoldier.Instance.S.stats.Dip.Val + ActiveSoldier.Instance.S.TacticianBonus() < i)
                    dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Add($"{i}");
            if (dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Contains($"{dipElecLevelDropdown.value + 1}"))
                dipElecLevelDropdown.value = 0;

            successChanceDisplay.text = Mathf.FloorToInt(HelperFunctions.CumulativeBinomialProbability(ActiveSoldier.Instance.S.stats.Dip.Val + ActiveSoldier.Instance.S.TacticianBonus(), dipElecLevelDropdown.value + 1, 0.5f, 0.5f) * 100f).ToString() + "%";
        }
        else if (dipElecTypeDropdown.value == 1)
        {
            levelUI.SetActive(true);
            for (int i = 1; i <= 6; i++)
                if (ActiveSoldier.Instance.S.stats.Elec.Val + ActiveSoldier.Instance.S.CalculatorBonus() < i)
                    dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Add($"{i}");
            if (dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Contains($"{dipElecLevelDropdown.value + 1}"))
                dipElecLevelDropdown.value = 0;
            successChanceDisplay.text = Mathf.FloorToInt(HelperFunctions.CumulativeBinomialProbability(ActiveSoldier.Instance.S.stats.Elec.Val + ActiveSoldier.Instance.S.CalculatorBonus(), dipElecLevelDropdown.value + 1, 0.5f, 0.5f) * 100f).ToString() + "%";
        }
        else
        {
            levelUI.SetActive(false);
            successChanceDisplay.text = "100%";
        }
    }
    public void ConfirmDipElec()
    {
        if (ActiveSoldier.Instance.S.CheckAP(3))
        {
            MenuManager.Instance.FreezeTimer();
            ActiveSoldier.Instance.S.DeductAP(3);
            bool terminalDisabled = false;
            int passCount = 0;
            string resultString = "";

            Terminal terminal = POIManager.Instance.FindPOIById(SelectedTerminalId) as Terminal;

            FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} attempts to interact with terminal at ({terminal.X}, {terminal.Y}, {terminal.Z})."); //write to report

            if (dipElecTypeDropdown.value == 0)
            {
                resultString += "Negotiation";
                terminal.SoldiersAlreadyNegotiated.Add(ActiveSoldier.Instance.S.Id);
                for (int i = 0; i < ActiveSoldier.Instance.S.stats.Dip.Val; i++)
                {
                    if (HelperFunctions.RandomDipelecCoinFlip())
                        passCount++;
                }
            }
            else if (dipElecTypeDropdown.value == 1)
            {
                resultString += "Hack";
                terminal.SoldiersAlreadyHacked.Add(ActiveSoldier.Instance.S.Id);
                for (int i = 0; i < ActiveSoldier.Instance.S.stats.Elec.Val; i++)
                {
                    if (HelperFunctions.RandomDipelecCoinFlip())
                        passCount++;
                }
            }
            else
                terminalDisabled = true;

            if (!terminalDisabled)
            {
                int targetLevel = dipElecLevelDropdown.value + 1;
                if (passCount >= targetLevel)
                {
                    FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} succeeds at level {targetLevel} {resultString}."); //write to report

                    for (int i = targetLevel; i >= 1; i--)
                    {
                        dipelecResultUI.transform.Find("OptionPanel").Find("Title").GetComponentInChildren<TextMeshProUGUI>().text = $"<color=green>{resultString} successful</color>";
                        GameObject dipelecReward = Instantiate(MenuManager.Instance.dipelecRewardPrefab, dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"));
                        dipelecReward.GetComponentInChildren<TextMeshProUGUI>().text = (resultString == "Hack") ? DipelecManager.Instance.GetLevelElec(i) : DipelecManager.Instance.GetLevelDip(i);
                    }

                    //add xp for successful dipelec
                    MenuManager.Instance.AddXpAlert(ActiveSoldier.Instance.S, (int)Mathf.Pow(2, targetLevel - 1), $"Successful level {targetLevel} {resultString}", true);

                    resultString += $"SuccessL{targetLevel}";
                }
                else
                {
                    FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} fails to {resultString}."); //write to report

                    dipelecResultUI.transform.Find("OptionPanel").Find("Title").GetComponentInChildren<TextMeshProUGUI>().text = $"<color=red>{resultString} failed</color>";
                    GameObject dipelecReward = Instantiate(MenuManager.Instance.dipelecRewardPrefab, dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"));
                    dipelecReward.GetComponentInChildren<TextMeshProUGUI>().text = $"No reward";

                    resultString += $"Fail";
                }
            }
            else
            {
                FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} disables terminal at ({terminal.X}, {terminal.Y}, {terminal.Z})."); //write to report

                GameObject dipelecReward = Instantiate(MenuManager.Instance.dipelecRewardPrefab, dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"));
                dipelecReward.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=red>Terminal disabled</color>";

                terminal.terminalEnabled = false;
                if (ActiveSoldier.Instance.S.hp > 3)
                    ActiveSoldier.Instance.S.TakeDamage(ActiveSoldier.Instance.S, ActiveSoldier.Instance.S.hp - 3, true, new() { "Dipelec" }, Vector3.zero);
            }
            //play dipelec result sfx
            SoundManager.Instance.PlayDipelecResolution(resultString);

            OpenDipelecResultUI();
            CloseDipElecUI();
        }
    }

    public string SelectedTerminalId => allTerminalIds[dipElecTerminalDropdown.value];
}
