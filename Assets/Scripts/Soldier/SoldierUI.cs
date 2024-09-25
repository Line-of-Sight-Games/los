using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public GameObject kia, resolveBroken, spotted;
    public Button actionButton, fieldButton;
    public SoldierPortrait soldierPotrait;
    public TextMeshProUGUI ap, mp, location; 

    private void Start()
    {
        noisePlayerSoldierUI = FindFirstObjectByType<AudioSource>();
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
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
            if (x >= 1 && x <= linkedSoldier.game.maxX && y >= 1 && y <= linkedSoldier.game.maxY && z >= 0 && z <= linkedSoldier.game.maxZ)
            {
                linkedSoldier.startX = x;
                linkedSoldier.startY = y;
                linkedSoldier.startZ = z;
                linkedSoldier.fielded = true;
                linkedSoldier.CheckSpecialityColor(linkedSoldier.soldierSpeciality);
                transform.Find("PopupBox").gameObject.SetActive(false);
                game.CheckDeploymentBeacons(linkedSoldier);

                //deploy the soldier
                game.PerformMove(linkedSoldier, 0, System.Tuple.Create(new Vector3(x, y, z), terrainDropdown.captionText.text), false, false, string.Empty, true);
                //patriot ability
                linkedSoldier.SetPatriotic();
            }
        }
    }

    public void OpenSoldierMenu(string type)
    {
        linkedSoldier.SetActiveSoldier();
        
        //print($"{Time.time}: Active Soldier: {game.activeSoldier.soldierName}|{menu.activeSoldier.soldierName}");
        menu.menuUI.transform.Find("GameMenu").Find("UnitDisplayPanel").gameObject.SetActive(false);
        menu.menuUI.transform.Find("GameMenu").Find("SoldierOptions").gameObject.SetActive(true);
        /*if (type == "frozen")
            menu.turnTitle.text = "<color=orange>F R O Z E N    T U R N</color>";
        else if (type == "moda")
            menu.turnTitle.text = "<color=purple>M O D A F I N I L    T U R N</color>";
        else
            menu.turnTitle.text = "N O R M A L    T U R N";*/

        //populate soldier loadout
        Transform soldierBanner = menu.soldierOptionsUI.transform.Find("SoldierBanner");
        soldierBanner.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(linkedSoldier);
    }
    public void DeathRoll()
    {
        if (menu.overrideView && menu.DeathKey())
        {
            if (game.DiceRoll() == 1)
            {
                print("Died from Deathroll");
                linkedSoldier.InstantKill(null, new() { "Deathroll" });
            } 
            else
                print("Survived Deathroll");
        }
    }
}
