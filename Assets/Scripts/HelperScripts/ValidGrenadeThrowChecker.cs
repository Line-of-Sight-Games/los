using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ValidGrenadeThrowChecker : MonoBehaviour
{
    public TMP_InputField XPos, YPos, ZPos;
    public GameObject throwBeyondRadius, throwBeyondBlindRadius, pressedOnce, scatteredOffMap;
    public UseItemUI useItemUI;

    private void Update()
    {
        CheckThrowingRange();
        CheckOffMap();
    }
    public void CheckOffMap()
    {
        scatteredOffMap.SetActive(false);

        if (GetThrowLocation(out Vector3 throwLocation) && (throwLocation.x <= 0 || throwLocation.x > GameManager.Instance.maxX || throwLocation.y <= 0 || throwLocation.y > GameManager.Instance.maxY)) //is scattering off map
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
                if (ActiveSoldier.Instance.S.IsAbleToSee() && ActiveSoldier.Instance.S.HasStrength())
                {
                    if (!IsWithinBounds(ActiveSoldier.Instance.S, throwLocation))
                        throwBeyondRadius.SetActive(true);
                }
                else
                {
                    if (!IsWithinDropBounds(ActiveSoldier.Instance.S, throwLocation)) //dropping allowed while blind within 3 or 0 strength
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
}
