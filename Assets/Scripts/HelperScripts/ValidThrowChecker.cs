using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ValidThrowChecker : MonoBehaviour
{
    public MainMenu menu;
    public TMP_InputField XPos, YPos, ZPos;
    public GameObject throwBeyondRadius, throwBeyondBlindRadius, pressedOnce, catcher, itemWillBreak, scatteredOffMap;
    public TMP_Dropdown catcherDropdown;
    public UseItemUI useItemUI;

    private void Update()
    {
        CheckThrowingRange();
        CheckOffMap();
        if (!useItemUI.name.Contains("Grenade")) //don't run when throwing grenade in anger
        {
            CheckForCatchers();
            CheckForItemBreak();
        }
    }
    public void CheckOffMap()
    {
        scatteredOffMap.SetActive(false);

        if (GetThrowLocation(out Vector3 throwLocation) && (throwLocation.x <= 0 || throwLocation.x > menu.game.maxX || throwLocation.y <= 0 || throwLocation.y > menu.game.maxY)) //is scattering off map
            scatteredOffMap.SetActive(true);
    }
    public void CheckThrowingRange()
    {
        throwBeyondRadius.SetActive(false);
        throwBeyondBlindRadius.SetActive(false);

        if (!pressedOnce.activeInHierarchy)
        {
            if (menu.activeSoldier.IsAbleToSee()) 
            {
                if (GetThrowLocation(out Vector3 throwLocation) && Vector3.Distance(throwLocation, new(menu.activeSoldier.X, menu.activeSoldier.Y, menu.activeSoldier.Z)) > menu.activeSoldier.ThrowRadius)
                    throwBeyondRadius.SetActive(true);
            }
            else
            {
                if (GetThrowLocation(out Vector3 throwLocation) && Vector3.Distance(throwLocation, new(menu.activeSoldier.X, menu.activeSoldier.Y, menu.activeSoldier.Z)) > 3) //dropping allowed while blid within 3
                    throwBeyondBlindRadius.SetActive(true);
            }
        }
    }
    public bool GetThrowLocation(out Vector3 throwLocation)
    {
        throwLocation = default;
        if (menu.ValidateIntInput(XPos, out int x) && menu.ValidateIntInput(YPos, out int y) && menu.ValidateIntInput(ZPos, out int z))
        {
            throwLocation = new Vector3(x, y, z);
            print($"{throwLocation}");
            return true;
        }
        return false;
    }
    public void CheckForCatchers()
    {
        print("running");
        catcher.SetActive(false);
        catcherDropdown.ClearOptions();

        if (GetThrowLocation(out Vector3 throwLocation))
        {
            foreach (Soldier s in menu.game.AllSoldiers())
            {
                if (s.IsAbleToSee() && s.IsSameTeamAs(menu.activeSoldier) && s.PhysicalObjectWithinRadius(throwLocation, 3) && s.HasAHandFree(true))
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

        if (GetThrowLocation(out Vector3 throwLocation) && menu.activeSoldier.Z - throwLocation.z > 8 && useItemUI.itemUsed.IsFragile() && !catcher.activeInHierarchy && !throwBeyondRadius.activeInHierarchy && !throwBeyondBlindRadius.activeInHierarchy)
            itemWillBreak.SetActive(true);
    }
}
