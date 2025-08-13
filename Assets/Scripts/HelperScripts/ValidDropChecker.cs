using TMPro;
using UnityEngine;

public class ValidDropChecker : MonoBehaviour
{
    public TMP_InputField XPos, YPos, ZPos;
    public GameObject invalidThrow, groundOrAlly, catcher, itemWillBreak;
    public TMP_Dropdown groundOrAllyDropdown, catcherDropdown;
    public UseItemUI useItemUI;

    private void Update()
    {
        CheckForItemBreak();
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
    public void CheckForItemBreak()
    {
        itemWillBreak.SetActive(false);

        if (GetThrowLocation(out Vector3 throwLocation) && ActiveSoldier.Instance.S.Z - throwLocation.z > 8 && useItemUI.itemUsed.IsFragile() && !catcher.activeInHierarchy && !invalidThrow.activeInHierarchy)
            itemWillBreak.SetActive(true);
    }
}

