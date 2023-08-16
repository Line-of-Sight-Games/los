using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
                foreach (string option in optionsToGrey)
                {
                    if (option == dropdown.options[i].text || int.Parse(option) == i)
                    {
                        Debug.Log($"Item {i}: {dropdown.options[i].text}");
                        dropdown.transform.Find("Dropdown List").Find("Viewport").Find("Content").GetChild(i + 1).GetComponent<Toggle>().interactable = false;
                    }
                }
            }
        }
    }
}
