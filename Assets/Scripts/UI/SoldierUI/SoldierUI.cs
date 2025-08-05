using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoldierUI : MonoBehaviour
{
    public MainMenu menu;
    public MainGame game;
    public SoldierManager soldierManager;
    public Soldier linkedSoldier;
    public TMP_InputField xSize, ySize, zSize;
    public int x, y, z;
    public TMP_Dropdown terrainDropdown;
    public GameObject revealMessage, resolveBroken;
    public Button actionButton, fieldButton;
    public SoldierPortrait soldierPotrait;
    public TextMeshProUGUI ap, mp, location, revealMessageText; 

    private void Start()
    {
        menu = FindFirstObjectByType<MainMenu>();
        game = FindFirstObjectByType<MainGame>();
        soldierManager = FindFirstObjectByType<SoldierManager>();
    }
    public void DisplayInFriendlyColumn()
    {
        transform.SetParent(soldierManager.friendlyDisplayColumn.transform);
    }
    public void DisplayInEnemyColumn()
    {
        transform.SetParent(soldierManager.enemyDisplayColumn.transform);
    }
    public void FieldButtonClicked()
    {
        //play button press sfx
        SoundManager.Instance.PlayButtonPress();

        FieldSoldier();
    }
    public void ActionButtonClicked()
    {
        //play button press sfx
        SoundManager.Instance.PlayButtonPress();
        //play select generic dialogue
        SoundManager.Instance.PlaySoldierSelection(linkedSoldier);

        OpenSoldierMenu("");
    }
    public void ConfirmFieldButtonClicked()
    {
        //play button press sfx
        SoundManager.Instance.PlayButtonPress();

        ConfirmFieldSoldier();
    }
    public void CancelFieldButtonClicked()
    {
        //play button press sfx
        SoundManager.Instance.PlayButtonPress();

        CancelFieldSoldier();
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
                //play move confirm dialogue
                SoundManager.Instance.PlaySoldierConfirmMove(linkedSoldier);

                //deploy the soldier
                game.PerformSpawn(linkedSoldier, System.Tuple.Create(new Vector3(x, y, z), terrainDropdown.captionText.text));

                //confirm fielding
                linkedSoldier.fielded = true;
                transform.Find("PopupBox").gameObject.SetActive(false);
                linkedSoldier.CheckSpecialityColor(linkedSoldier.soldierSpeciality);
            }
        }
    }

    public void OpenSoldierMenu(string type)
    {
        linkedSoldier.SetActiveSoldier();
        
        //print($"{Time.time}: Active Soldier: {game.activeSoldier.soldierName}|{menu.activeSoldier.soldierName}");
        soldierManager.enemyDisplayColumn.gameObject.SetActive(false);
        soldierManager.friendlyDisplayColumn.gameObject.SetActive(false);
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
        if (menu.OverrideView && menu.DeathKey())
        {
            if (HelperFunctions.DiceRoll() == 1)
            {
                print("Died from Deathroll");
                linkedSoldier.InstantKill(null, new() { "Deathroll" });
            } 
            else
                print("Survived Deathroll");
        }
    }
}
