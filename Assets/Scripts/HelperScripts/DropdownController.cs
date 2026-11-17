using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownController : MonoBehaviour
{
    public List<string> optionsToGrey = new();
    public TMP_Dropdown dropdown;

    private Coroutine dropdownCheck;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void OnEnable()
    {
        dropdownCheck = StartCoroutine(CheckDropdownOpened());
    }

    private void OnDisable()
    {
        if (dropdownCheck != null)
            StopCoroutine(dropdownCheck);
    }

    private IEnumerator CheckDropdownOpened()
    {
        while (true)
        {
            if (dropdown.transform.Find("Dropdown List") != null)
            {
                ApplyGreyedOptions();

                // Wait until dropdown closes before checking again
                yield return new WaitUntil(() =>
                    dropdown.transform.Find("Dropdown List") == null);
            }

            yield return null;
        }
    }

    public void ApplyGreyedOptions()
    {
        Transform dropdownList = dropdown.transform.Find("Dropdown List");

        if (dropdownList == null)
            return;

        Transform content = dropdownList.Find("Viewport/Content");

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            string cleanOptionText = Regex.Replace(
                dropdown.options[i].text, "<.*?>", "");

            foreach (string option in optionsToGrey)
            {
                if (option == cleanOptionText || (int.TryParse(option, out int index) && index == i))
                {
                    if (content.GetChild(i + 1).TryGetComponent<Toggle>(out var toggle))
                        toggle.interactable = false;

                    break;
                }
            }
        }
    }
}
