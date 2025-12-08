using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoldierUI : MonoBehaviour
{
    public Soldier linkedSoldier;
    public TMP_InputField xSize, ySize, zSize;
    public int x, y, z;
    public TMP_Dropdown terrainDropdown;
    public GameObject revealMessage, resolveBroken;
    public Button actionButton, fieldButton;
    public SoldierPortrait soldierPotrait;
    public TextMeshProUGUI ap, mp, location, revealMessageText; 

    public void DisplayInFriendlyColumn()
    {
        transform.SetParent(SoldierManager.Instance.friendlyDisplayColumn.transform);
    }
    public void DisplayInEnemyColumn()
    {
        transform.SetParent(SoldierManager.Instance.enemyDisplayColumn.transform);
    }
    public void FieldButtonClicked()
    {
        //play button press sfx
        SoundManager.Instance.PlayButtonPress();

        FieldSoldier();
    }
    public void ActionButtonClicked()
    {
        if (DataPersistenceManager.Instance.lozMode && linkedSoldier.IsZombie())
        {
            SoundManager.Instance.PlayZombieSelection(linkedSoldier);
        }
        else
        {
            //play button press sfx
            SoundManager.Instance.PlayButtonPress();
            //play select generic dialogue
            SoundManager.Instance.PlaySoldierSelection(linkedSoldier);
        }
            
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
            if (x >= 1 && x <= GameManager.Instance.maxX && y >= 1 && y <= GameManager.Instance.maxY && z >= 0 && z <= GameManager.Instance.maxZ)
            {
                //play move confirm dialogue
                SoundManager.Instance.PlaySoldierConfirmMove(linkedSoldier);

                //deploy the soldier
                GameManager.Instance.PerformSpawn(linkedSoldier, System.Tuple.Create(new Vector3(x, y, z), terrainDropdown.captionText.text));

                //confirm fielding
                linkedSoldier.fielded = true;
                transform.Find("PopupBox").gameObject.SetActive(false);
                linkedSoldier.CheckSpecialityColor(linkedSoldier.soldierSpeciality);
            }
        }
    }

    public void OpenSoldierMenu(string type)
    {
        ActiveSoldier.Instance.SetActiveSoldier(linkedSoldier);

        SoldierManager.Instance.enemyDisplayColumn.SetActive(false);
        SoldierManager.Instance.friendlyDisplayColumn.SetActive(false);
        MenuManager.Instance.menuUI.transform.Find("GameMenu").Find("SoldierOptions").gameObject.SetActive(true);
        /*if (type == "frozen")
            MenuManager.Instance.turnTitle.text = "<color=orange>F R O Z E N    T U R N</color>";
        else if (type == "moda")
            MenuManager.Instance.turnTitle.text = "<color=purple>M O D A F I N I L    T U R N</color>";
        else
            MenuManager.Instance.turnTitle.text = "N O R M A L    T U R N";*/

        //populate soldier loadout
        Transform soldierBanner = MenuManager.Instance.soldierOptionsUI.transform.Find("SoldierBanner");
        soldierBanner.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(linkedSoldier);
    }
    public void DeathRoll()
    {
        if (MenuManager.Instance.OverrideView && HelperFunctions.DeathKeyPressed())
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
