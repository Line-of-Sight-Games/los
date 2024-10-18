using TMPro;
using UnityEngine;

public class GeneralAlertUI : MonoBehaviour
{
    public TextMeshProUGUI message;

    public void Activate(string message)
    {
        this.message.text = message;
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
