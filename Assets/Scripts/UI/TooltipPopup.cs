
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipPopup : MonoBehaviour
{
    public static TooltipPopup Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text locationText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple TacticalObjectPopup instances found!");
            enabled = false;
            return;
        }

        Instance = this;
    }


    public void Show(Soldier soldier)
    {
        if (soldier == null) return;

        portraitImage.sprite = soldier.soldierPortrait;
        portraitImage.enabled = soldier.soldierPortrait != null;

        nameText.text = soldier.soldierName;

        // Soldier coordinates are stored in battlefield coordinates.
        locationText.SetText($"X: {soldier.X}, Y: {soldier.Y}, Z: {soldier.Z}");

        panel.SetActive(true);
    }

    public void Show(POI poi)
    {
        if (poi == null) return;

        // Use the assigned portrait, or load the appropriate POI portrait.
        Sprite portrait = poi.poiPortrait != null
            ? poi.poiPortrait
            : poi.LoadPortrait(poi.poiType);

        portraitImage.sprite = portrait;
        portraitImage.enabled = portrait != null;

        // Convert the POI type into a readable display name.
        nameText.text = poi.poiType switch
        {
            "barrel" => "Explosive Barrel",
            "gb" => "Goody Box",
            "terminal" => "Terminal",
            "claymore" => "Claymore",
            "depbeacon" => "Deployment Beacon",
            "thermalcam" => "Thermal Camera",
            "drugcab" => "Drug Cabinet",
            "smokecloud" => "Smoke Cloud",
            "tabuncloud" => "Tabun Cloud",
            "binocularbeam" => "Binocular Beam",
            _ => poi.poiType
        };

        locationText.SetText($"X: {poi.X}, Y: {poi.Y}, Z: {poi.Z}");

        panel.SetActive(true);
    }

    public void Show(Item item)
    {
        if (item == null) return;

        // Display item portrait.
        portraitImage.sprite = item.itemImage;
        portraitImage.enabled = item.itemImage != null;

        // Display item name.
        nameText.text = item.itemName;

        // Display battlefield coordinates.
        locationText.SetText($"X: {item.X}, Y: {item.Y}, Z: {item.Z}");

        // Activate the tooltip panel, not the overall parent.
        panel.SetActive(true);
    }


    public void Close()
    {
        panel.SetActive(false);
    }
}

