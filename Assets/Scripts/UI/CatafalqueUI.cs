using System.Linq;
using TMPro;
using UnityEngine;

public class CatafalqueUI : MonoBehaviour
{
    public TMP_Dropdown fallenSoldierDropdown;
    public TextMeshProUGUI apCost;

    public void ConfirmCatafalqueUI()
    {
        if (DataPersistenceManager.Instance.lozMode)
        {
            string catafalquedSoldierID = fallenSoldierDropdown.captionText.text;

            SoundManager.Instance.PlayCatafalqueUsed();

            ActiveSoldier.Instance.S.catafalquedThisTurn = true;
            ActiveSoldier.Instance.S.fallenSoldierList.Remove(catafalquedSoldierID);
            if (!ActiveSoldier.Instance.S.fallenSoldierList.Any())
                ActiveSoldier.Instance.S.catafalqueAvailable = false;
            ActiveSoldier.Instance.S.DrainAP();
            
            Soldier catafalquedSoldier = SoldierManager.Instance.FindSoldierById(catafalquedSoldierID);
            (string, int, int) xps = ("normal", 1, 2);
            if (catafalquedSoldier.IsBruteZombie()) //double xp for brute zombie kill
                xps = ("brute", xps.Item2 * 2, xps.Item3 * 2);

            //give 2 xp to catafalquer for zombie kill
            MenuManager.Instance.AddXpAlert(ActiveSoldier.Instance.S, xps.Item3, $"Initiated catafalque of fallen soldier. ({catafalquedSoldier.fallenSoldierName})(zombie)", false);
            //give 1 xp to all soldiers for catafalque
            foreach (Soldier s in GameManager.Instance.AllFieldedFriendlySoldiers())
                MenuManager.Instance.AddXpAlert(s, xps.Item2, $"Catafalque of fallen soldier. ({catafalquedSoldier.fallenSoldierName})(zombie)", false);
            
            
        }
        MenuManager.Instance.CloseCatafalqueUI();
    }
}
