using UnityEngine;
using TMPro;

public class SoldierUI : MonoBehaviour
{
    public AudioSource noisePlayerSoldierUI;
    public AudioClip buttonPress;

    public MainMenu menu;
    public MainGame game;
    public Soldier linkedSoldier;
    public TMP_InputField xSize, ySize, zSize;
    public int x, y, z;
    public TMP_Dropdown terrainDropdown;

    private void Start()
    {
        noisePlayerSoldierUI = FindObjectOfType<AudioSource>();
        menu = FindObjectOfType<MainMenu>();
        game = FindObjectOfType<MainGame>();
    }

    public void PlayButtonPress()
    {
        //print("played button press from soldier UI");
        noisePlayerSoldierUI.PlayOneShot(buttonPress);
    }

    public void FieldSoldier()
    {
        transform.Find("PopupBox").gameObject.SetActive(true);
    }

    public void CancelFieldSoldier()
    {
        transform.Find("PopupBox").gameObject.SetActive(false);
    }

    public void ConfirmFieldSoldier()
    {
        if (int.TryParse(xSize.text, out x) && int.TryParse(ySize.text, out y) && int.TryParse(zSize.text, out z) && terrainDropdown.value != 0)
        {
            //print(x + ":" + linkedSoldier.game.maxX + " " + y + ":" + linkedSoldier.game.maxY + " " + z + ":" + linkedSoldier.game.maxZ);
            if (x >= 1 && x <= linkedSoldier.game.maxX && y >= 1 && y <= linkedSoldier.game.maxY && z >= 0 && z <= linkedSoldier.game.maxZ)
            {
                linkedSoldier.startX = x;
                linkedSoldier.startY = y;
                linkedSoldier.startZ = z;
                linkedSoldier.fielded = true;
                linkedSoldier.CheckSpecialityColor(linkedSoldier.soldierSpeciality);
                transform.Find("PopupBox").gameObject.SetActive(false);
                game.PerformMove(linkedSoldier, 0, System.Tuple.Create(new Vector3(x, y, z), terrainDropdown.options[terrainDropdown.value].text), false, false, string.Empty, true);
            }
        }
    }

    public void OpenSoldierMenu()
    {
        linkedSoldier.selected = true;
        menu.activeSoldier = linkedSoldier;
        game.activeSoldier = linkedSoldier;
        //print($"{Time.time}: Active Soldier: {game.activeSoldier.soldierName}|{menu.activeSoldier.soldierName}");
        menu.menuUI.transform.Find("Options Panel").Find("GameOptions").gameObject.SetActive(false);
        menu.menuUI.transform.Find("Options Panel").Find("SoldierOptions").gameObject.SetActive(true);
        menu.turnTitle.text = "N O R M A L    T U R N";

        //populate soldier loadout
        Transform soldierBanner = menu.soldierOptionsUI.transform.Find("SoldierBanner");
        soldierBanner.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(linkedSoldier);
    }
}
