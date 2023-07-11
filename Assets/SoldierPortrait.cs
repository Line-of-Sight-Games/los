using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoldierPortrait : MonoBehaviour
{
    public void Init(Soldier s)
    {
        GetComponent<Image>().sprite = s.soldierPortrait;
        transform.Find("SoldierName").GetComponent<TextMeshProUGUI>().text = s.soldierName;
        transform.Find("TeamIndicator").GetComponent<TextMeshProUGUI>().text = s.soldierTeam.ToString();
        transform.Find("RankIndicator").GetComponent<Image>().sprite = s.LoadInsignia(s.rank);
    }
}
