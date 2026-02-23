using TMPro;
using UnityEngine;

public class GeneralAlertUI : MonoBehaviour
{
    public TextMeshProUGUI message;

    public void Init(string message)
    {
        this.message.text = message;
        gameObject.SetActive(true);
        transform.SetParent(FindFirstObjectByType<Canvas>().transform, false);
    }
    public void Deactivate()
    {
        Destroy(gameObject);
    }
}
