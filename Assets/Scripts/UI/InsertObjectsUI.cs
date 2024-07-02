using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InsertObjectsUI : MonoBehaviour
{
    public TMP_Dropdown objectTypeDropdown;
    public TMP_InputField xPos;
    public TMP_InputField yPos;
    public TMP_InputField zPos;
    public TMP_Dropdown terrainDropdown;
    public TMP_Dropdown terminalTypeDropdown;

    public Transform gbItemsPanel;
    public Transform dcItemsPanel;

    public GameObject terminalTypeUI;
    public GameObject allItemsPanelUI;
    public GameObject allDrugsPanelUI;
}
