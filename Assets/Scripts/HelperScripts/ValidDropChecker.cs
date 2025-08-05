using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ValidDropChecker : MonoBehaviour
{
    public TMP_InputField XPos, YPos, ZPos;
    public GameObject invalidThrow, catcher, itemWillBreak;
    public TMP_Dropdown catcherDropdown;
    public UseItemUI useItemUI;

    private void Update()
    {
        CheckDroppingRange();
        CheckForCatchers();
        CheckForItemBreak();
    }
    public void CheckDroppingRange()
    {
        invalidThrow.SetActive(false);

        if (GetThrowLocation(out Vector3 throwLocation) && (Vector2.Distance(new(throwLocation.x, throwLocation.y), new(MenuManager.Instance.activeSoldier.X, MenuManager.Instance.activeSoldier.Y)) > 3 || throwLocation.z > MenuManager.Instance.activeSoldier.Z + 3))
            invalidThrow.SetActive(true);
    }
    public bool GetThrowLocation(out Vector3 throwLocation)
    {
        throwLocation = default;
        if (HelperFunctions.ValidateIntInput(XPos, out int x) && HelperFunctions.ValidateIntInput(YPos, out int y) && HelperFunctions.ValidateIntInput(ZPos, out int z))
        {
            throwLocation = new Vector3(x, y, z);
            return true;
        }
        return false;
    }
    public void CheckForCatchers()
    {
        catcher.SetActive(false);
        catcherDropdown.ClearOptions();

        if (GetThrowLocation(out Vector3 throwLocation))
        {
            foreach (Soldier s in GameManager.Instance.AllFieldedSoldiers())
            {
                if (s.IsAbleToSee() && s.IsSameTeamAs(MenuManager.Instance.activeSoldier) && s.PointWithinRadius(throwLocation, 3) && s.HasAHandFree(true))
                {
                    if (!catcherDropdown.options.Any(option => option.text == s.soldierName))
                        catcherDropdown.AddOptions(new List<TMP_Dropdown.OptionData> { new(s.soldierName, s.soldierPortrait, Color.white) });
                }
            }

            if (catcherDropdown.options.Count > 0)
                catcher.SetActive(true);
        }
    }
    public void CheckForItemBreak()
    {
        itemWillBreak.SetActive(false);

        if (GetThrowLocation(out Vector3 throwLocation) && MenuManager.Instance.activeSoldier.Z - throwLocation.z > 8 && useItemUI.itemUsed.IsFragile() && !catcher.activeInHierarchy && !invalidThrow.activeInHierarchy)
            itemWillBreak.SetActive(true);
    }
}

