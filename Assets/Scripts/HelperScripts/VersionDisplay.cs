using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
    public TextMeshProUGUI versionDisplay;
    // Start is called before the first frame update
    void Start()
    {
        versionDisplay.text = "Version: " + Application.version.ToString();
    }
}
