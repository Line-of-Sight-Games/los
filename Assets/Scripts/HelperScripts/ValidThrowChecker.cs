using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ValidThrowChecker : MonoBehaviour
{
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

        if (GetThrowLocation(out Vector3 throwLocation) && (throwLocation.x <= 0 || throwLocation.x > MenuManager.Instance.game.maxX || throwLocation.y <= 0 || throwLocation.y > MenuManager.Instance.game.maxY)) //is scattering off map
            scatteredOffMap.SetActive(true);
    }
    public void CheckThrowingRange()
    {
        throwBeyondRadius.SetActive(false);
        throwBeyondBlindRadius.SetActive(false);

        if (!pressedOnce.activeInHierarchy)
        {
            if (GetThrowLocation(out Vector3 throwLocation))
            {
                if (MenuManager.Instance.activeSoldier.IsAbleToSee() && MenuManager.Instance.activeSoldier.HasStrength())
                {
                    if (!IsWithinBounds(MenuManager.Instance.activeSoldier, throwLocation))
                        throwBeyondRadius.SetActive(true);
                }
                else
                {
                    if (!IsWithinDropBounds(MenuManager.Instance.activeSoldier, throwLocation)) //dropping allowed while blind within 3 or 0 strength
                        throwBeyondBlindRadius.SetActive(true);
                }
            }
            
        }
    }
    public bool IsWithinBounds(Soldier throwingSoldier, Vector3 throwLocation)
    {
        int deltaX = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow((int)throwLocation.x - throwingSoldier.X, 2)));
        int deltaY = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow((int)throwLocation.y - throwingSoldier.Y, 2)));
        int deltaZ = Mathf.Min((int)throwLocation.z - throwingSoldier.Z, Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow((int)throwLocation.z - throwingSoldier.Z, 2))));
        int s = throwingSoldier.stats.Str.Val;

        print($"s={s} | deltaX={deltaX} | deltaY={deltaY} | Z={deltaZ} | rhs={(100 * Mathf.Pow(s, 2) - (Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2))) / (20 * s)}");

        if (deltaZ <= (100 * Mathf.Pow(s, 2) - (Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2))) / (20 * s))
            return true;
        return false;
    }
    public bool IsWithinDropBounds(Soldier throwingSoldier, Vector3 throwLocation)
    {
        if (Vector2.Distance(throwLocation, new(throwingSoldier.X, throwingSoldier.Y)) <= 3 && throwLocation.z <= (throwingSoldier.Z + 3))
            return true;
        return false;
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
        print("running");
        catcher.SetActive(false);
        catcherDropdown.ClearOptions();

        if (GetThrowLocation(out Vector3 throwLocation))
        {
            foreach (Soldier s in MenuManager.Instance.game.AllSoldiers())
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

        if (GetThrowLocation(out Vector3 throwLocation) && MenuManager.Instance.activeSoldier.Z - throwLocation.z > 8 && useItemUI.itemUsed.IsFragile() && !catcher.activeInHierarchy && !throwBeyondRadius.activeInHierarchy && !throwBeyondBlindRadius.activeInHierarchy)
            itemWillBreak.SetActive(true);
    }
}
