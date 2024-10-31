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
    public TextMeshProUGUI hpDisplay;
    public Image soldierPosition;
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
            armourImage.gameObject.SetActive(false);
            brokenArmourImage.gameObject.SetActive(false);
            if (linkedSoldier.IsWearingBodyArmour(true) || linkedSoldier.IsWearingJuggernautArmour(true))
                armourImage.gameObject.SetActive(true);
            else if (linkedSoldier.IsWearingBodyArmour(false) || linkedSoldier.IsWearingJuggernautArmour(false))
                brokenArmourImage.gameObject.SetActive(true);
            hpDisplay.text = $"{linkedSoldier.GetFullHP()}";

            //load body position
            if (linkedSoldier.IsUnconscious() || linkedSoldier.IsPlayingDead())
                soldierPosition.sprite = linkedSoldier.LoadPosition("Unconscious");
            else if (linkedSoldier.IsLastStand())
                soldierPosition.sprite = linkedSoldier.LoadPosition("Last Stand");
            else
                soldierPosition.sprite = linkedSoldier.LoadPosition("Active");

            //load name
            soldierName.text = linkedSoldier.soldierName;

            //load location
            soldierLocation.text = $"X:{linkedSoldier.X} Y:{linkedSoldier.Y} Z:{linkedSoldier.Z}";
        }
    }
}
