using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShotUI : MonoBehaviour
{
    public TextMeshProUGUI shooterID;
    public TMP_Dropdown shotTypeDropdown;
    public TMP_Dropdown aimTypeDropdown;
    public TMP_Dropdown targetDropdown;
    public TMP_Dropdown coverLevelDropdown;
    public TextMeshProUGUI barrelLocation;
    public TextMeshProUGUI suppressionValue;
    public TextMeshProUGUI apCost;

    public GameObject aimTypeUI;
    public GameObject suppressionValueUI;
    public GameObject coverLocationUI;
    public GameObject barrelLocationUI;
    public GameObject coverLevelUI;
}
