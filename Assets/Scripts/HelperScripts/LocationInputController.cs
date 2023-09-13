using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LocationInputController : MonoBehaviour
{
    public TMP_InputField textInput;
    public bool initialiser;
    public MainGame game;

    private void Awake()
    {
        textInput = GetComponent<TMP_InputField>();
        game = FindObjectOfType<MainGame>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    public void CheckInput()
    {
        if (Regex.Match(textInput.text, @"^[0-9]+$").Success)
        {
            if (!initialiser)
            {
                if ((textInput.transform.name.Contains("X") && (int.Parse(textInput.text) > game.maxX || int.Parse(textInput.text) < 1)) || (textInput.transform.name.Contains("Y") && (int.Parse(textInput.text) > game.maxY || int.Parse(textInput.text) < 1)) || (textInput.transform.name.Contains("Z") && (int.Parse(textInput.text) > game.maxZ || int.Parse(textInput.text) < 0)))
                    textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
                else
                    textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(0.196f, 0.196f, 0.196f);
            }
            else
                textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(0.196f, 0.196f, 0.196f);
        }
        else
            textInput.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
    }
}
