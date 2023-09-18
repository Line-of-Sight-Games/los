using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public TMP_InputField textInput;
    public MainGame game;
    public int min, max;

    private void Awake()
    {
        textInput = GetComponent<TMP_InputField>();
        game = FindObjectOfType<MainGame>();
    }

    private void Update()
    {
        CheckInput();
    }
    public void CheckInput()
    {
        if (Regex.Match(textInput.text, @"^[0-9]+$").Success && int.Parse(textInput.text) >= min && int.Parse(textInput.text) <= max)
            textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(0.196f, 0.196f, 0.196f);
        else
            textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
    }
}
