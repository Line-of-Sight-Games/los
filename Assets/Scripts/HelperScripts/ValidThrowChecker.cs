using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValidThrowChecker : MonoBehaviour
{
    public MainMenu menu;
    public TMP_InputField XPos, YPos, ZPos;
    public GameObject invalidThrow, pressedOnce;

    private void Update()
    {
        if (!pressedOnce.activeInHierarchy)
        {
            if (GetThrowLocation(out Vector3 throwLocation) && menu.soldierManager.FindSoldierById(menu.activeSoldier.Id).PhysicalObjectWithinRadius(throwLocation, menu.activeSoldier.ThrowRadius()))
                invalidThrow.SetActive(false);
            else
                invalidThrow.SetActive(true);
        }
        else
            invalidThrow.SetActive(false);
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
}
