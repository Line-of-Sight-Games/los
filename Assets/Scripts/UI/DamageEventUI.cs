using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageEventUI : MonoBehaviour
{
    public TMP_Dropdown damageEventTypeDropdown;
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
}
