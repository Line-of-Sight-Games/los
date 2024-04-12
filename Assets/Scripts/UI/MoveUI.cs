using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour
{
    public TMP_Dropdown moveTypeDropdown;
    public TMP_InputField xPos;
    public TMP_InputField yPos;
    public TMP_InputField zPos;
    public TextMeshProUGUI startX;
    public TextMeshProUGUI startY;
    public TextMeshProUGUI startZ;
    public TMP_Dropdown terrainDropdown;
    public Toggle coverToggle;
    public Toggle meleeToggle;
    public TMP_InputField fallInput;
    public TextMeshProUGUI moveDonated;
    public TextMeshProUGUI apCost;

    public GameObject locationUI;
    public GameObject startlocationUI;
    public GameObject terrainDropdownUI;
    public GameObject coverToggleUI;
    public GameObject meleeToggleUI;
    public GameObject fallInputUI;
    public GameObject closestAllyUI;
    public GameObject moveDonatedUI;
    public GameObject backButton;
}
