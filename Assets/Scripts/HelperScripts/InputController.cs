using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public TMP_InputField textInput;
    public MainGame game;
    public int min, max;
    public Color normalColour;

    private void Awake()
    {
        textInput = GetComponent<TMP_InputField>();
        game = FindObjectOfType<MainGame>();
        normalColour = game.menu.normalTextColour;
    }

    private void Update()
    {
        CheckInput();
    }
    public void CheckInput()
    {
        if (Regex.Match(textInput.text, @"^-?\d+$").Success && int.Parse(textInput.text) >= min && int.Parse(textInput.text) <= max)
            textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = normalColour;
        else
            textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
    }
}
