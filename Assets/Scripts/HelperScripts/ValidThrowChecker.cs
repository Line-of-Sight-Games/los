using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ValidThrowChecker : MonoBehaviour
{
    public MainMenu menu;
    public TMP_InputField XPos, YPos, ZPos;
    public GameObject invalidThrow, pressedOnce, catcher, itemWillBreak;
    public TMP_Dropdown catcherDropdown;
    public UseItemUI useItemUI;

    private void Update()
    {
        CheckThrowingRange();
        if (!useItemUI.name.Contains("Grenade")) //don't run when throwing grenade in anger
        {
            CheckForCatchers();
            CheckForItemBreak();
        }
    }
    public void CheckThrowingRange()
    {
        invalidThrow.SetActive(false);

        if (!pressedOnce.activeInHierarchy)
        {
            if (GetThrowLocation(out Vector3 throwLocation) && !menu.soldierManager.FindSoldierById(menu.activeSoldier.Id).PhysicalObjectWithinRadius(throwLocation, menu.activeSoldier.ThrowRadius()))
                invalidThrow.SetActive(true);
        }
    }
    public bool GetThrowLocation(out Vector3 throwLocation)
    {
        throwLocation = default;
        if (menu.ValidateIntInput(XPos, out int x) && menu.ValidateIntInput(YPos, out int y) && menu.ValidateIntInput(ZPos, out int z))
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
            foreach (Soldier s in menu.game.AllSoldiers())
            {
                if (s.IsAbleToSee() && s.IsSameTeamAs(menu.activeSoldier) && s.PhysicalObjectWithinRadius(throwLocation, 3))
                {
                    if (!catcherDropdown.options.Any(option => option.text == s.soldierName))
                        catcherDropdown.AddOptions(new List<TMP_Dropdown.OptionData> { new(s.soldierName, s.soldierPortrait) });
                }
            }

            if (catcherDropdown.options.Count > 0)
                catcher.SetActive(true);
        }
    }
    public void CheckForItemBreak()
    {
        itemWillBreak.SetActive(false);

        if (GetThrowLocation(out Vector3 throwLocation) && menu.activeSoldier.Z - throwLocation.z > 8 && useItemUI.itemUsed.IsFragile() && !catcher.activeInHierarchy && !invalidThrow.activeInHierarchy)
            itemWillBreak.SetActive(true);
    }
}
