using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformerAlert : SoldierAlert
{
    public GameObject soldierSnapshot, soldierSnapshotPrefab;
    public InformerAlert Init(Soldier informer, Soldier soldierInformedOn)
    {
        SetSoldier(informer);

        if (soldier != null)
        {
            soldierSnapshot = Instantiate(soldierSnapshotPrefab, MenuManager.Instance.damageUI.transform);
            soldierSnapshot.transform.SetAsLastSibling();

            Transform soldierBanner = soldierSnapshot.transform.Find("SoldierBanner");

            soldierBanner.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldierInformedOn);
            soldierBanner.Find("HP").GetComponent<TextMeshProUGUI>().text = "HP: " + soldierInformedOn.GetFullHP().ToString();
            soldierBanner.Find("AP").GetComponent<TextMeshProUGUI>().text = "AP: " + soldierInformedOn.ap.ToString();
            soldierBanner.Find("MA").GetComponent<TextMeshProUGUI>().text = "MA: " + soldierInformedOn.mp.ToString();
            soldierBanner.Find("Speed").GetComponent<TextMeshProUGUI>().text = "Move: " + soldierInformedOn.InstantSpeed.ToString();
            soldierBanner.Find("XP").GetComponent<TextMeshProUGUI>().text = "XP: " + soldierInformedOn.xp.ToString();
            soldierBanner.Find("Status").GetComponent<TextMeshProUGUI>().text = "Status: " + soldierInformedOn.GetStatus();

            Transform soldierStatsUI = soldierSnapshot.transform.Find("SoldierStatsUI");

            soldierInformedOn.PaintSpeciality(soldierStatsUI);

            foreach (string[] s in MenuManager.Instance.AllStats)
            {
                Color displayColor = Color.white;
                if (soldierInformedOn.stats.GetStat(s[0]).Val < soldier.stats.GetStat(s[0]).BaseVal)
                    displayColor = Color.red;
                else if (soldierInformedOn.stats.GetStat(s[0]).Val > soldier.stats.GetStat(s[0]).BaseVal)
                    displayColor = Color.green;

                soldierStatsUI.Find("Stats").Find("Base").Find(s[0]).GetComponent<TextMeshProUGUI>().text = soldierInformedOn.stats.GetStat(s[0].ToString()).BaseVal.ToString();
                soldierStatsUI.Find("Stats").Find("Active").Find(s[0]).GetComponent<TextMeshProUGUI>().text = soldierInformedOn.stats.GetStat(s[0].ToString()).Val.ToString();
                soldierStatsUI.Find("Stats").Find("Active").Find(s[0]).GetComponent<TextMeshProUGUI>().color = displayColor;
            }

            soldierStatsUI.Find("General").Find("Name").GetComponent<TextMeshProUGUI>().text = soldierInformedOn.soldierName;
            soldierStatsUI.Find("General").Find("Rank").GetComponent<TextMeshProUGUI>().text = soldierInformedOn.rank;
            soldierStatsUI.Find("General").Find("Terrain").GetComponent<TextMeshProUGUI>().text = soldierInformedOn.soldierTerrain;
            soldierStatsUI.Find("General").Find("Specialty").GetComponent<TextMeshProUGUI>().text = soldierInformedOn.PrintSoldierSpeciality();
            soldierStatsUI.Find("General").Find("Ability").GetComponent<TextMeshProUGUI>().text = HelperFunctions.PrintList(soldierInformedOn.soldierAbilities);
            soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().text = soldierInformedOn.RoundsWithoutFood.ToString();
            soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().text = soldierInformedOn.tp.ToString();

            soldierStatsUI.Find("SoldierLoadout").GetComponent<InventoryDisplayPanelSoldier>().Init(soldierInformedOn);
        }

        return this;
    }
    public void OpenSoldierSnapshot()
    {
        if (soldierSnapshot != null)
        {
            soldierSnapshot.SetActive(true);
            description.text = "Already used.";
        }
    }
}
