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
    }

    public void PlayButtonPress()
    {
        //Debug.Log("played button press from soldier UI");
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
            //Debug.Log(x + ":" + linkedSoldier.game.maxX + " " + y + ":" + linkedSoldier.game.maxY + " " + z + ":" + linkedSoldier.game.maxZ);
            if (x >= 1 && x <= linkedSoldier.game.maxX && y >= 1 && y <= linkedSoldier.game.maxY && z >= 0 && z <= linkedSoldier.game.maxZ)
            {
                linkedSoldier.TerrainOn = terrainDropdown.options[terrainDropdown.value].text;
                linkedSoldier.X = x;
                linkedSoldier.Y = y;
                linkedSoldier.Z = z;
                linkedSoldier.fielded = true;
                linkedSoldier.CheckSpecialityColor(linkedSoldier.soldierSpeciality);
                transform.Find("PopupBox").gameObject.SetActive(false);
                StartCoroutine(linkedSoldier.game.DetectionAlertSingle(linkedSoldier, "moveChange", Vector3.zero, string.Empty));
            }
            else
            {
                Debug.Log("Create a popup which says their attempted move was outside of boundaries.");
            }
        }
        else
        {
            Debug.Log("Create a popup which says their formatting was wrong and to try again.");
        }
    }

    public void OpenSoldierMenu()
    {
        menu = FindObjectOfType<MainMenu>();
        game = FindObjectOfType<MainGame>();
        linkedSoldier.selected = true;
        menu.activeSoldier = linkedSoldier;
        game.activeSoldier = linkedSoldier;
        menu.menuUI.transform.Find("Options Panel").Find("GameOptions").gameObject.SetActive(false);
        menu.menuUI.transform.Find("Options Panel").Find("SoldierOptions").gameObject.SetActive(true);
    }
}
