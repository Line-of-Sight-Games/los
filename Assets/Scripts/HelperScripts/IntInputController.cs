using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class IntInputController : MonoBehaviour
{
    public TMP_InputField textInput;
    public MainGame game;
    public Color normalColour;

    private void Awake()
    {
        textInput = GetComponent<TMP_InputField>();
        game = FindFirstObjectByType<MainGame>();
        normalColour = new(0.196f, 0.196f, 0.196f);
    }

    private void Update()
    {
        CheckIntInput();
    }
    public bool CheckIntInput()
    {
        if (Regex.Match(textInput.text, @"^-?\d+$").Success)
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
