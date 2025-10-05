using UnityEngine;


public class RiotShieldPopup : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKey && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            HideRiotShieldPopup();
        }
    }
    public void ShowRiotShieldPopup()
    {
        if (ActiveSoldier.Instance.S.HasActiveRiotShield() && !ActiveSoldier.Instance.S.HasActiveAndAngledRiotShield())
            MenuManager.Instance.OpenRiotShieldUI();
        else
            gameObject.SetActive(true);
    }
    public void HideRiotShieldPopup()
    {
        gameObject.SetActive(false);
    }
    public void ReorientButtonClick()
    {
        MenuManager.Instance.OpenRiotShieldUI();
        HideRiotShieldPopup();
    }
    public void UnorientButtonClick()
    {
        ActiveSoldier.Instance.S.riotXPoint = 0;
        ActiveSoldier.Instance.S.riotYPoint = 0;
        HideRiotShieldPopup();
    }
}
