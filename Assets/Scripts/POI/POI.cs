using System.Collections.Generic;

public class POI : PhysicalObject, IDataPersistence
{
    public POIManager poiManager;
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
