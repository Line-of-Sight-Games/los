using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class POI : PhysicalObject, IDataPersistence
{
    public POIManager poiManager;
    public Dictionary<string, object> details;
    public string poiType;
    public Sprite poiPortrait;

    public virtual void LoadData(GameData data)
    {
        if (data.allPOIDetails.TryGetValue(id, out details))
        {
            poiType = (string)details["poiType"];
        }
    }

    public virtual void SaveData(ref GameData data)
    {

    }

    public Sprite LoadPortrait(string portraitType)
    {
        TMP_Dropdown allPortraits = FindFirstObjectByType<AllPortraits>().allPortraitsPOIDropdown;
        return portraitType switch
        {
            "barrel" => allPortraits.options[0].image,
            "gb" => allPortraits.options[1].image,
            "terminal" => allPortraits.options[2].image,
            "claymore" => allPortraits.options[3].image,
            "depbeacon" => allPortraits.options[4].image,
            "thermalcam" => allPortraits.options[5].image,
            "drugcab" => allPortraits.options[6].image,
            _ => allPortraits.options[0].image,
        };
    }
}
