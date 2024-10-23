using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LocationInputController : MinMaxInputController
{
    private void OnEnable()
    {
        if (textInput.transform.name.Contains("X"))
        {
            min = 1;
            max = game.maxX;
        }
        else if (textInput.transform.name.Contains("Y"))
        {
            min = 1;
            max = game.maxY;
        }
        else if (textInput.transform.name.Contains("Z"))
        {
            min = 0;
            max = game.maxZ;
        }
    }

    public void SetMin(int min)
    {
        this.min = min;
    }

    public void SetMax(int max)
    {
        this.max = max;
    }
}
