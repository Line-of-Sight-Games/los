using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShotUI : MonoBehaviour
{
    public TextMeshProUGUI shooterID;
    public TMP_Dropdown shotTypeDropdown;
    public TMP_Dropdown gunDropdown;
    public TMP_Dropdown gunsEmptyDropdown;
    public TMP_Dropdown comboGunsDropdown;
    public TMP_Dropdown comboGunsEmptyDropdown;
    public TMP_Dropdown aimTypeDropdown;
    public TMP_Dropdown targetDropdown;
    public TMP_Dropdown coverLevelDropdown;
    public TMP_InputField coverXPos;
    public TMP_InputField coverYPos;
    public TMP_InputField coverZPos;
    public TextMeshProUGUI barrelLocation;
    public TextMeshProUGUI suppressionValue;
    public TextMeshProUGUI apCost;
    public Button backButton;

    public GameObject aimTypeUI;
    public GameObject suppressionValueUI;
    public GameObject coverLocationUI;
    public GameObject invalidCoverLocationUI;
    public GameObject barrelLocationUI;
    public GameObject coverLevelUI;
}
