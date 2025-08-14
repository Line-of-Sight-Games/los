using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisarmUI : MonoBehaviour
{
    public List<string> allDisarmableIds = new();
    public TMP_Dropdown disarmableDropdown;

    //disarm functions
    public void OpenDisarmUI()
    {
        //generate disarmable list
        List<TMP_Dropdown.OptionData> disarmOptionDataList = new();
        foreach (IAmDisarmable disarmable in GameManager.Instance.AllDisarmable())
        {
            TMP_Dropdown.OptionData disarmOptionData;
            if (ActiveSoldier.Instance.S.PhysicalObjectWithinMeleeRadius((PhysicalObject)disarmable))
            {
                allDisarmableIds.Add(disarmable.Id);
                disarmOptionData = new($"X:{disarmable.X} Y:{disarmable.Y} Z:{disarmable.Z}", disarmable.DisarmImage, Color.white);
                disarmOptionDataList.Add(disarmOptionData);
            }
        }
        disarmableDropdown.AddOptions(disarmOptionDataList);

        gameObject.SetActive(true);
    }
    public void CloseDisarmUI()
    {
        ClearDisarmUI();
        gameObject.SetActive(false);
    }
    public void ClearDisarmUI()
    {
        allDisarmableIds.Clear();
        disarmableDropdown.ClearOptions();
    }
    public void ConfirmDisarm()
    {
        if (ActiveSoldier.Instance.S.CheckAP(1))
        {
            ActiveSoldier.Instance.S.DeductAP(1);

            POI poiToDisarm = POIManager.Instance.FindPOIById(SelectedDisarmableId);
            Item disarmedItem = null;
            Soldier placedBy = null;

            if (poiToDisarm is Claymore claymoreToDisarm)
            {
                disarmedItem = ItemManager.Instance.SpawnItem("Claymore");
                placedBy = claymoreToDisarm.placedBy;
            }
            else if (poiToDisarm is DeploymentBeacon depbeaconToDisarm)
            {
                disarmedItem = ItemManager.Instance.SpawnItem("Deployment_Beacon");
                placedBy = depbeaconToDisarm.placedBy;
            }
            else if (poiToDisarm is ThermalCamera thermalcamToDisarm)
            {
                GameManager.Instance.SetLosCheckAllEnemies("losChange|thermalCamDeactive"); //loscheckallenemies
                disarmedItem = ItemManager.Instance.SpawnItem("Thermal_Camera");
                placedBy = thermalcamToDisarm.placedBy;
            }

            //xp for disarming enemy objects
            if (placedBy != null && ActiveSoldier.Instance.S.IsOppositeTeamAs(placedBy))
                MenuManager.Instance.AddXpAlert(ActiveSoldier.Instance.S, 2, "Disarmed enemy device.", true);

            //set item to same position as poi and destroy poi
            disarmedItem.X = poiToDisarm.X;
            disarmedItem.Y = poiToDisarm.Y;
            disarmedItem.Z = poiToDisarm.Z;
            POIManager.Instance.DestroyPOI(poiToDisarm);

            ActiveSoldier.Instance.S.PerformLoudAction(6);
            CloseDisarmUI();
        }
    }

    public string SelectedDisarmableId => allDisarmableIds[disarmableDropdown.value];
}
