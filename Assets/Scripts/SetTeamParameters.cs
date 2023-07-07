using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetTeamParameters : MonoBehaviour
{
    public GameObject setupMenuUI, createSoldierMenuUI;
    public TMP_Dropdown maxPlayerDropdown, activeMaxSoldierDropdown, maxSoldierDropdown2P, maxSoldierDropdown3P, maxSoldierDropdown4P;
    public GameObject activeMaxSoldierDropdownObj, maxSoldierDropdown2PObj, maxSoldierDropdown3PObj, maxSoldierDropdown4PObj;
    public int maxTeams, maxSoldiers;

    private void Start()
    {

    }
    public void Confirm()
    {
        int.TryParse(maxPlayerDropdown.options[maxPlayerDropdown.value].text, out maxTeams);
        int.TryParse(activeMaxSoldierDropdown.options[activeMaxSoldierDropdown.value].text, out maxSoldiers);
        setupMenuUI.SetActive(false);
        createSoldierMenuUI.SetActive(true);
    }

    private void Update()
    {
        if (maxPlayerDropdown.value == 0)
        {
            maxSoldierDropdown2PObj.SetActive(true);
            activeMaxSoldierDropdown = maxSoldierDropdown2P;
        }
        else
            maxSoldierDropdown2PObj.SetActive(false);

        if (maxPlayerDropdown.value == 1)
        {
            maxSoldierDropdown3PObj.SetActive(true);
            activeMaxSoldierDropdown = maxSoldierDropdown3P;
        }
        else
            maxSoldierDropdown3PObj.SetActive(false);

        if (maxPlayerDropdown.value == 2)
        {
            maxSoldierDropdown4PObj.SetActive(true);
            activeMaxSoldierDropdown = maxSoldierDropdown4P;
        }
        else
            maxSoldierDropdown4PObj.SetActive(false);
    }
}
