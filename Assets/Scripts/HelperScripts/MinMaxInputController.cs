using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MinMaxInputController : IntInputController
{
    public int min, max;

    private void Update()
    {
        CheckMinMaxInput();
    }
    public bool CheckMinMaxInput()
    {
        if (CheckIntInput() && int.Parse(textInput.text) >= min && int.Parse(textInput.text) <= max)
        {
            textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = normalColour;
            return true;
        }
        else
        {
            textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
            return false;
        }
    }
}
