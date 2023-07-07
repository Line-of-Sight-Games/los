using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking.Types;

public class DropdownController : MonoBehaviour
{
    public List<string> optionsToGrey;
    public TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Update()
    {
        if (dropdown.transform.Find("Dropdown List") != null)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (optionsToGrey.Contains(dropdown.options[i].text))
                {
                    Debug.Log($"Item {i}: {dropdown.options[i].text}");
                    dropdown.transform.Find("Dropdown List").Find("Viewport").Find("Content").Find($"Item {i}: {dropdown.options[i].text}").GetComponent<Toggle>().interactable = false;
                }
            }
        }
    }
}
