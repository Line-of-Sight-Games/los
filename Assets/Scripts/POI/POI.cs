using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : PhysicalObject, IDataPersistence
{
    public Dictionary<string, object> details;
    public string poiType;

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
}
