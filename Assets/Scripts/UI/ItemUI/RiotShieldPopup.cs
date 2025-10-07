using UnityEngine;

public class RiotShieldPopup : MonoBehaviour
{
    void Update()
    {
        if (HelperFunctions.AnyKeyPressed() && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), HelperFunctions.MousePosition(), null))
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
