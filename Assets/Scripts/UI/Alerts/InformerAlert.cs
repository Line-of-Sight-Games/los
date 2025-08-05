using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformerAlert : SoldierAlert
{
    public void OpenSoldierSnapshot(TextMeshProUGUI snappedSoldierID)
    {
        Soldier snapshotSoldier = SoldierManager.Instance.FindSoldierById(snappedSoldierID.text);
        if (snapshotSoldier != null)
        {
            GameObject soldierSnapshot = Instantiate(MenuManager.Instance.soldierSnapshotPrefab, MenuManager.Instance.damageUI.transform);
            soldierSnapshot.transform.SetAsLastSibling();
            Transform soldierBanner = soldierSnapshot.transform.Find("SoldierBanner");

            soldierBanner.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(snapshotSoldier);
            soldierBanner.Find("HP").GetComponent<TextMeshProUGUI>().text = "HP: " + snapshotSoldier.GetFullHP().ToString();
            soldierBanner.Find("AP").GetComponent<TextMeshProUGUI>().text = "AP: " + snapshotSoldier.ap.ToString();
            soldierBanner.Find("MA").GetComponent<TextMeshProUGUI>().text = "MA: " + snapshotSoldier.mp.ToString();
            soldierBanner.Find("Speed").GetComponent<TextMeshProUGUI>().text = "Move: " + snapshotSoldier.InstantSpeed.ToString();
            soldierBanner.Find("XP").GetComponent<TextMeshProUGUI>().text = "XP: " + snapshotSoldier.xp.ToString();
            soldierBanner.Find("Status").GetComponent<TextMeshProUGUI>().text = "Status: " + snapshotSoldier.GetStatus();

            Transform soldierStatsUI = soldierSnapshot.transform.Find("SoldierStatsUI");

            soldier.PaintSpeciality(soldierStatsUI);

            foreach (string[] s in MenuManager.Instance.AllStats)
            {
                Color displayColor = Color.white;
                if (snapshotSoldier.stats.GetStat(s[0]).Val < snapshotSoldier.stats.GetStat(s[0]).BaseVal)
                    displayColor = Color.red;
                else if (snapshotSoldier.stats.GetStat(s[0]).Val > snapshotSoldier.stats.GetStat(s[0]).BaseVal)
                    displayColor = Color.green;

                soldierStatsUI.Find("Stats").Find("Base").Find(s[0]).GetComponent<TextMeshProUGUI>().text = snapshotSoldier.stats.GetStat(s[0].ToString()).BaseVal.ToString();
                soldierStatsUI.Find("Stats").Find("Active").Find(s[0]).GetComponent<TextMeshProUGUI>().text = snapshotSoldier.stats.GetStat(s[0].ToString()).Val.ToString();
                soldierStatsUI.Find("Stats").Find("Active").Find(s[0]).GetComponent<TextMeshProUGUI>().color = displayColor;
            }

            soldierStatsUI.Find("General").Find("Name").GetComponent<TextMeshProUGUI>().text = snapshotSoldier.soldierName;
            soldierStatsUI.Find("General").Find("Rank").GetComponent<TextMeshProUGUI>().text = snapshotSoldier.rank;
            soldierStatsUI.Find("General").Find("Terrain").GetComponent<TextMeshProUGUI>().text = snapshotSoldier.soldierTerrain;
            soldierStatsUI.Find("General").Find("Specialty").GetComponent<TextMeshProUGUI>().text = snapshotSoldier.PrintSoldierSpeciality();
            soldierStatsUI.Find("General").Find("Ability").GetComponent<TextMeshProUGUI>().text = HelperFunctions.PrintList(snapshotSoldier.soldierAbilities);
            soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().text = snapshotSoldier.RoundsWithoutFood.ToString();
            soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().text = snapshotSoldier.tp.ToString();

            soldierStatsUI.Find("SoldierLoadout").GetComponent<InventoryDisplayPanelSoldier>().Init(snapshotSoldier);
        }
    }
}
