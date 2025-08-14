using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class DropUI : MonoBehaviour
{
    public UseItemUI useItemUIComponent;
    public GameObject groundOrAlly;
    public GameObject catcher;
    public GameObject invalidThrow;
    public GameObject itemWillBreak;
    public GameObject noAlliesPresent;

    public TextMeshProUGUI headerText;

    public TMP_InputField XPos, YPos, ZPos;

    public TMP_Dropdown groundOrAllyDropdown;
    public TMP_Dropdown catcherDropdown;

    public void Update()
    {
        if (CheckThrowLocation())
            CheckForCatchers();
        else
        {
            catcher.SetActive(false);
            noAlliesPresent.SetActive(false);
            catcherDropdown.ClearOptions();
        }
    }
    public bool CheckThrowLocation()
    {
        if (GetThrowLocation(out Vector3 throwLocation))
        {
            if (IsWithinDropBounds(ActiveSoldier.Instance.S, throwLocation))
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
        else
        {
            invalidThrow.SetActive(false);
            groundOrAlly.SetActive(false);
        }
        return false;
    }
    public bool IsWithinDropBounds(Soldier throwingSoldier, Vector3 throwLocation)
    {
        if (Vector2.Distance(throwLocation, new(throwingSoldier.X, throwingSoldier.Y)) <= 3 && throwLocation.z <= (throwingSoldier.Z + 3))
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
    public void OpenDropUI(UseItemUI useItemUI)
    {
        useItemUIComponent.itemUsed = useItemUI.itemUsed;
        useItemUIComponent.itemUsedIcon = useItemUI.itemUsedIcon;
        useItemUIComponent.itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        headerText.text = $"Dropping {useItemUI.itemUsed.itemName} from {useItemUI.itemUsedFromSlotName} slot.";
        gameObject.SetActive(true);
    }
    public void ConfirmDrop(UseItemUI dropItemUI)
    {
        if (GetThrowLocation(out Vector3 throwLocation) && !invalidThrow.activeInHierarchy)
        {
            FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} drops {dropItemUI.itemUsed.itemName}."); //write to report

            if (itemWillBreak.activeInHierarchy)
            {
                dropItemUI.itemUsed.X = (int)throwLocation.x;
                dropItemUI.itemUsed.Y = (int)throwLocation.y;
                dropItemUI.itemUsed.Z = (int)throwLocation.z;
                FileUtility.WriteToReport($"{dropItemUI.itemUsed.itemName} breaks."); //write to report
                dropItemUI.itemUsed.TakeDamage(ActiveSoldier.Instance.S, 1, new() { "Fall" }); //destroy item
            }
            else if (catcher.activeInHierarchy)
            {
                if (dropItemUI.itemUsed.IsCatchable())
                {
                    Soldier catchingSoldier = SoldierManager.Instance.FindSoldierByName(catcherDropdown.captionText.text);

                    FileUtility.WriteToReport($"{dropItemUI.itemUsed.itemName} is caught by {catchingSoldier.soldierName}."); //write to report

                    //if soldier has left hand free catch it there, otherwise catch in right hand
                    if (catchingSoldier.LeftHandItem == null)
                        dropItemUI.itemUsed.MoveItem(ActiveSoldier.Instance.S, dropItemUI.itemUsedFromSlotName, catchingSoldier, "LeftHand");
                    else
                        dropItemUI.itemUsed.MoveItem(ActiveSoldier.Instance.S, dropItemUI.itemUsedFromSlotName, catchingSoldier, "RightHand");
                }
                else
                {
                    dropItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(dropItemUI.itemUsed, dropItemUI.itemUsedFromSlotName); //move item to ground
                    dropItemUI.itemUsed.X = (int)throwLocation.x;
                    dropItemUI.itemUsed.Y = (int)throwLocation.y;
                    dropItemUI.itemUsed.Z = (int)throwLocation.z;
                }
            }
            else
            {
                dropItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(dropItemUI.itemUsed, dropItemUI.itemUsedFromSlotName); //move item to ground
                dropItemUI.itemUsed.X = (int)throwLocation.x;
                dropItemUI.itemUsed.Y = (int)throwLocation.y;
                dropItemUI.itemUsed.Z = (int)throwLocation.z;
            }
            ActiveSoldier.Instance.S.PerformLoudAction(5);
            CloseDropUI();
        }
    }
    public void ClearDropUI()
    {
        invalidThrow.SetActive(false);
        itemWillBreak.SetActive(false);
        groundOrAlly.SetActive(false);
        noAlliesPresent.SetActive(false);
        catcher.SetActive(false);
        
        XPos.interactable = true;
        YPos.interactable = true;
        ZPos.interactable = true;
        XPos.text = "";
        YPos.text = "";
        ZPos.text = "";
        groundOrAllyDropdown.value = 0;
        catcherDropdown.ClearOptions();
    }
    public void CloseDropUI()
    {
        ClearDropUI();
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
