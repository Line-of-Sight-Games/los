using System;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DragUI : MonoBehaviour
{
    public MainMenu menu;
    public int pressCount = 0;
    public bool legitMove, legitDrop;
    public Vector3 moveLocation, dropLocation;
    public Soldier drager, dragee;

    public TMP_Dropdown targetDropdown;
    public TextMeshProUGUI maxDragRange;
    public TMP_InputField xPos, yPos, zPos, xPosD, yPosD, zPosD;
    public TMP_Dropdown terrainDropdown, terrainDropdownD;
    public TextMeshProUGUI apCost;
    public GameObject moveObjects, moveOutOfRange, dropObjects, dropOutOfRange, backButton;

    private void Update()
    {
        if (pressCount.Equals(1))
        {
            if (HelperFunctions.ValidateIntInput(xPos, out int x) && HelperFunctions.ValidateIntInput(yPos, out int y) && HelperFunctions.ValidateIntInput(zPos, out int z) && terrainDropdown.value != 0)
            {
                moveLocation = new(x, y, z);
                if (menu.activeSoldier.PointWithinRadius(moveLocation, menu.activeSoldier.GetMaxDragRange()))
                {
                    legitMove = true;
                    apCost.text = GetDragAPCost().ToString();
                    moveOutOfRange.SetActive(false);
                }
                else
                {
                    legitMove = false;
                    moveOutOfRange.SetActive(true);
                }
            }
            else
            {
                legitMove = false;
                moveOutOfRange.SetActive(true);
            }
        }
        else if (pressCount.Equals(2))
        {
            apCost.text = "0";
            if (HelperFunctions.ValidateIntInput(xPosD, out int x) && HelperFunctions.ValidateIntInput(yPosD, out int y) && HelperFunctions.ValidateIntInput(zPosD, out int z) && terrainDropdownD.value != 0)
            {
                dropLocation = new(x, y, z);
                if (IsWithinDropBounds())
                {
                    legitDrop = true;
                    dropOutOfRange.SetActive(false);
                }
                else
                {
                    legitDrop = false;
                    dropOutOfRange.SetActive(true);
                }
            }
            else
            {
                legitDrop = false;
                dropOutOfRange.SetActive(true);
            }
        }
    }
    public void IncrementPressCount()
    {
        pressCount++;
    }
    public int GetDragAPCost()
    {
        return Mathf.Max(1, Mathf.CeilToInt(Vector3.Distance(new(menu.activeSoldier.X, menu.activeSoldier.Y, menu.activeSoldier.Z), new(moveLocation.x, moveLocation.y, moveLocation.z))) / menu.activeSoldier.stats.Str.Val);
    }
    public int GetDropDistance()
    {
        return Mathf.RoundToInt(dragee.Z - dropLocation.z);
    }
    public bool IsWithinDropBounds()
    {
        if (Vector2.Distance(dropLocation, new(drager.X, drager.Y)) <= 3 && dropLocation.z <= (drager.Z + 3))
            return true;
        return false;
    }
    public void ConfirmDrag()
    {
        if (pressCount.Equals(0)) //first press
        {
            drager = menu.activeSoldier;
            dragee = menu.soldierManager.FindSoldierById(targetDropdown.captionText.text);
            targetDropdown.interactable = false;
            maxDragRange.text = drager.GetMaxDragRange().ToString();
            moveObjects.SetActive(true);
            IncrementPressCount();
        }
        else if (pressCount.Equals(1)) //second press
        {
            if (legitMove)
            {
                dragee.beingDraggedBy = drager.Id;
                menu.game.BreakAllControllingMeleeEngagments(drager); //break melee engagement when commencing drag
                if (drager.CheckAP(GetDragAPCost()))
                {
                    drager.DeductAP(GetDragAPCost());
                    menu.game.PerformMove(drager, 0, Tuple.Create(moveLocation, terrainDropdown.captionText.text), false, false, string.Empty, true); 
                    menu.game.PerformMove(dragee, 0, Tuple.Create(moveLocation, terrainDropdown.captionText.text), false, false, string.Empty, true);
                    moveObjects.SetActive(false);
                    backButton.SetActive(false);
                    dropObjects.SetActive(true);
                    IncrementPressCount();
                }
            }
        }
        else if (pressCount.Equals(2))
        {
            if (IsWithinDropBounds())
            {
                dragee.beingDraggedBy = string.Empty;
                menu.game.PerformMove(dragee, 0, Tuple.Create(dropLocation, terrainDropdownD.captionText.text), false, false, GetDropDistance().ToString(), true);
                menu.CloseDragUI();
            }
        }
    }
}
