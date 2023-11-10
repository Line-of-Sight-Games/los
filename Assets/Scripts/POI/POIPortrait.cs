using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class POIPortrait : MonoBehaviour
{
    public Sprite explosiveBarrelSprite, goodyBoxSprite, terminalSprite;
    public void Init(POI poi)
    {
        print(poi.poiType);
        GetComponent<Image>().sprite = poi.poiType switch
        {
            "barrel" => explosiveBarrelSprite,
            "gb" => goodyBoxSprite,
            "terminal" => terminalSprite,
            _ => null,
        };
        transform.Find("POILocation").GetComponent<TextMeshProUGUI>().text = $"X:{poi.X} Y:{poi.Y} Z:{poi.Z}";
    }
}
