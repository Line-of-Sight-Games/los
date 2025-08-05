using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoldierPortrait : MonoBehaviour
{
    public Soldier linkedSoldier;
    public Image teamIndicator;
    public Image portrait;
    public Image soldierRank;
    public Image armourImage;
    public Image brokenArmourImage;
    public Image hpDisplayImage;
    public TextMeshProUGUI hpDisplay;
    public Image soldierPosition;
    public Image equipmentOnHead;
    public TextMeshProUGUI soldierName, soldierLocation;

    public void Init(Soldier s)
    {
        linkedSoldier = s;
    }

    private void Update()
    {
        if (linkedSoldier != null)
        {
            //load team colours
            if (linkedSoldier.soldierTeam == 1)
                teamIndicator.color = Color.red;
            else
                teamIndicator.color = Color.blue;

            //load portrait
            portrait.sprite = linkedSoldier.soldierPortrait;

            //load rank
            soldierRank.sprite = linkedSoldier.LoadInsignia(linkedSoldier.rank);

            //load hp
            if (linkedSoldier.IsOnturn() || MenuManager.Instance.OverrideView)
            {
                armourImage.gameObject.SetActive(false);
                brokenArmourImage.gameObject.SetActive(false);
                if (linkedSoldier.IsWearingBodyArmour(true) || linkedSoldier.IsWearingJuggernautArmour(true))
                    armourImage.gameObject.SetActive(true);
                else if (linkedSoldier.IsWearingBodyArmour(false) || linkedSoldier.IsWearingJuggernautArmour(false))
                    brokenArmourImage.gameObject.SetActive(true);
                hpDisplayImage.gameObject.SetActive(true);
                hpDisplay.text = $"{linkedSoldier.GetFullHP()}";
            }
            else //hide all hp details 
            {
                armourImage.gameObject.SetActive(false);
                brokenArmourImage.gameObject.SetActive(false);
                hpDisplayImage.gameObject.SetActive(false);
                hpDisplay.text = string.Empty;
            }

            //load body position
            if (linkedSoldier.IsUnconscious() || linkedSoldier.IsPlayingDead())
                soldierPosition.sprite = linkedSoldier.LoadPosition("Unconscious");
            else if (linkedSoldier.IsLastStand())
                soldierPosition.sprite = linkedSoldier.LoadPosition("Last Stand");
            else
                soldierPosition.sprite = linkedSoldier.LoadPosition("Active");

            //load equipment on head
            if (linkedSoldier.InventorySlots["Head"] != "")
                equipmentOnHead.sprite = linkedSoldier.LoadHeadEquipment(ItemManager.Instance.FindItemById(linkedSoldier.InventorySlots["Head"]).itemName);
            else
                equipmentOnHead.sprite = linkedSoldier.LoadHeadEquipment("Nothing");

            //load name
            soldierName.text = linkedSoldier.soldierName;

            //load location
            soldierLocation.text = $"X:{linkedSoldier.X} Y:{linkedSoldier.Y} Z:{linkedSoldier.Z}";
        }
    }
}
