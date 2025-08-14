using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ThrowUI : MonoBehaviour
{
    public UseItemUI useItemUIComponent;

    public GameObject scatteredOffMap;
    public GameObject invalidThrow;
    public GameObject itemWillBreak;
    public GameObject pressedOnce;
    public GameObject preciseThrow;
    public GameObject finalPosition;
    public GameObject groundOrAlly;
    public GameObject noAlliesPresent;
    public GameObject catcher;

    public TextMeshProUGUI headerText;

    public TMP_InputField XPos, YPos, ZPos;

    public TMP_Dropdown groundOrAllyDropdown;
    public TMP_Dropdown catcherDropdown;

    private void Update()
    {
        if (CheckThrowLocation())
            CheckForCatchers();
        else
        {
            catcher.SetActive(false);
            noAlliesPresent.SetActive(false);
            catcherDropdown.ClearOptions();
        }
        
        CheckOffMap();
    }
    public bool CheckThrowLocation()
    {
        if (GetThrowLocation(out Vector3 throwLocation))
        {
            if (ActiveSoldier.Instance.S.IsAbleToSee() && ActiveSoldier.Instance.S.HasStrength())
            {
                if (IsWithinBounds(ActiveSoldier.Instance.S, throwLocation))
                {
                    invalidThrow.SetActive(false);
                    groundOrAlly.SetActive(true);
                    return true;
                }
                else
                {
                    invalidThrow.SetActive(true);
                    groundOrAlly.SetActive(false);
                }
            }
        }
        else
        {
            invalidThrow.SetActive(false);
            groundOrAlly.SetActive(false);
        }
        return false;
    }
    public void CheckOffMap()
    {
        scatteredOffMap.SetActive(false);

        if (GetThrowLocation(out Vector3 throwLocation) && (throwLocation.x <= 0 || throwLocation.x > GameManager.Instance.maxX || throwLocation.y <= 0 || throwLocation.y > GameManager.Instance.maxY)) //is scattering off map
            scatteredOffMap.SetActive(true);
    }
    public bool IsWithinBounds(Soldier throwingSoldier, Vector3 throwLocation)
    {
        int deltaX = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow((int)throwLocation.x - throwingSoldier.X, 2)));
        int deltaY = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow((int)throwLocation.y - throwingSoldier.Y, 2)));
        int deltaZ = Mathf.Min((int)throwLocation.z - throwingSoldier.Z, Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow((int)throwLocation.z - throwingSoldier.Z, 2))));
        int s = throwingSoldier.stats.Str.Val;

        print($"s={s} | deltaX={deltaX} | deltaY={deltaY} | deltaZ={deltaZ} | rhs={(100 * Mathf.Pow(s, 2) - (Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2))) / (20 * s)}");

        if (deltaZ <= (100 * Mathf.Pow(s, 2) - (Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2))) / (20 * s))
            return true;
        return false;
    }
    public void CheckForCatchers()
    {
        if (GetThrowLocation(out Vector3 throwLocation) && groundOrAllyDropdown.captionText.text.Equals("Ally"))
        {
            catcherDropdown.ClearOptions();
            foreach (Soldier s in GameManager.Instance.AllFieldedSoldiers())
            {
                if (s.IsAbleToSee() && s.IsSameTeamAs(ActiveSoldier.Instance.S) && s.PointWithinRadius(throwLocation, 3) && s.HasAHandFree(true))
                {
                    if (!catcherDropdown.options.Any(option => option.text == s.soldierName))
                        catcherDropdown.AddOptions(new List<TMP_Dropdown.OptionData> { new(s.soldierName, s.soldierPortrait, Color.white) });
                }
            }

            if (catcherDropdown.options.Count > 0)
            {
                noAlliesPresent.SetActive(false);
                catcher.SetActive(true);
            }
            else
            {
                noAlliesPresent.SetActive(true);
                catcher.SetActive(false);
            }
        }
        else
        {
            catcher.SetActive(false);
            noAlliesPresent.SetActive(false);
            catcherDropdown.ClearOptions();
        }
    }
    public void OpenThrowUI(UseItemUI useItemUI)
    {
        useItemUIComponent.itemUsed = useItemUI.itemUsed;
        useItemUIComponent.itemUsedIcon = useItemUI.itemUsedIcon;
        useItemUIComponent.itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        headerText.text = $"Throwing {useItemUI.itemUsed.itemName} from {useItemUI.itemUsedFromSlotName} slot.";
        gameObject.SetActive(true);

        //set minimum input values to allow targeting negative coordinates
        XPos.GetComponent<LocationInputController>().SetMin(-ActiveSoldier.Instance.S.ThrowRadius);
        YPos.GetComponent<LocationInputController>().SetMin(-ActiveSoldier.Instance.S.ThrowRadius);
    }

    public void ConfirmThrow(UseItemUI throwItemUI)
    {
        if (!pressedOnce.activeInHierarchy) //first press
        {
            if (GetThrowLocation(out Vector3 throwLocation) && !invalidThrow.activeInHierarchy)
            {
                int newX, newY;
                int throwDistance = Mathf.RoundToInt(Vector3.Distance(new(ActiveSoldier.Instance.S.X, ActiveSoldier.Instance.S.Y, ActiveSoldier.Instance.S.Z), throwLocation));
                int scatterDegree = HelperFunctions.RandomNumber(0, 360);
                int scatterDistance = ActiveSoldier.Instance.S.StrengthCheck() switch
                {
                    false => Mathf.CeilToInt(HelperFunctions.DiceRoll() * ActiveSoldier.Instance.S.stats.Str.Val / 2.0f),
                    _ => -1,
                };

                if (scatterDistance == -1 || throwDistance <= 3)
                    preciseThrow.SetActive(true);
                else
                {
                    (newX, newY) = HelperFunctions.CalculateScatteredCoordinates((int)throwLocation.x, (int)throwLocation.y, scatterDegree, scatterDistance);

                    XPos.text = $"{newX}";
                    YPos.text = $"{newY}";

                    finalPosition.SetActive(true);
                }

                pressedOnce.SetActive(true);
            }
        }
        else //second press
        {
            if (scatteredOffMap.activeInHierarchy)
            {
                ActiveSoldier.Instance.S.Inventory.ConsumeItemInSlot(throwItemUI.itemUsed, throwItemUI.itemUsedFromSlotName); //destroy item
                CloseThrowUI();
            }
            else
            {
                if (GetThrowLocation(out Vector3 throwLocation))
                {
                    if (itemWillBreak.activeInHierarchy)
                    {
                        throwItemUI.itemUsed.X = (int)throwLocation.x;
                        throwItemUI.itemUsed.Y = (int)throwLocation.y;
                        throwItemUI.itemUsed.Z = (int)throwLocation.z;
                        FileUtility.WriteToReport($"{throwItemUI.itemUsed.itemName} breaks."); //write to report
                        throwItemUI.itemUsed.TakeDamage(ActiveSoldier.Instance.S, 1, new() { "Fall" }); //destroy item
                    }
                    else if (catcher.activeInHierarchy)
                    {
                        if (throwItemUI.itemUsed.IsCatchable())
                        {
                            Soldier catchingSoldier = SoldierManager.Instance.FindSoldierByName(catcherDropdown.captionText.text);

                            //if soldier has left hand free catch it there, otherwise catch in right hand
                            if (catchingSoldier.LeftHandItem == null)
                                throwItemUI.itemUsed.MoveItem(ActiveSoldier.Instance.S, throwItemUI.itemUsedFromSlotName, catchingSoldier, "LeftHand");
                            else
                                throwItemUI.itemUsed.MoveItem(ActiveSoldier.Instance.S, throwItemUI.itemUsedFromSlotName, catchingSoldier, "RightHand");
                        }
                        else
                        {
                            throwItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(throwItemUI.itemUsed, throwItemUI.itemUsedFromSlotName); //move item to ground
                            throwItemUI.itemUsed.X = (int)throwLocation.x;
                            throwItemUI.itemUsed.Y = (int)throwLocation.y;
                            throwItemUI.itemUsed.Z = (int)throwLocation.z;
                        }
                    }
                    else
                    {
                        throwItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(throwItemUI.itemUsed, throwItemUI.itemUsedFromSlotName); //move item to ground
                        throwItemUI.itemUsed.X = (int)throwLocation.x;
                        throwItemUI.itemUsed.Y = (int)throwLocation.y;
                        throwItemUI.itemUsed.Z = (int)throwLocation.z;
                    }
                    ActiveSoldier.Instance.S.PerformLoudAction(5);
                    CloseThrowUI();
                }
            }

            //reset minimum input values
            throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("XPos").GetComponent<LocationInputController>().SetMin(1);
            throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("YPos").GetComponent<LocationInputController>().SetMin(1);
        }
    }
    public void ClearThrowUI()
    {
        invalidThrow.SetActive(false);
        scatteredOffMap.SetActive(false);
        itemWillBreak.SetActive(false);
        pressedOnce.SetActive(false);
        preciseThrow.SetActive(false);
        finalPosition.SetActive(false);
        groundOrAlly.SetActive(false);
        catcher.SetActive(false);
        noAlliesPresent.SetActive(false);

        XPos.interactable = true;
        YPos.interactable = true;
        ZPos.interactable = true;
        XPos.text = "";
        YPos.text = "";
        ZPos.text = "";

        groundOrAllyDropdown.value = 0;
        catcherDropdown.ClearOptions();
    }
    public void CloseThrowUI()
    {
        ClearThrowUI();
        gameObject.SetActive(false);
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
}
