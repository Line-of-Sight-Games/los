using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DipElecUI : MonoBehaviour
{
    public List<string> allTerminalIds = new();
    public TMP_Dropdown dipElecTerminalDropdown;
    public TMP_Dropdown dipElecTypeDropdown;
    public TMP_Dropdown dipElecLevelDropdown;
    public TextMeshProUGUI successChanceDisplay;
    public TextMeshProUGUI apCost;

    public GameObject levelUI;

    public string SelectedTerminalId => allTerminalIds[dipElecTerminalDropdown.value];
}
