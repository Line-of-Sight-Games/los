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
}
