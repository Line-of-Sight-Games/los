using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SoldierUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Soldier linkedSoldier;
    public TMP_InputField xSize, ySize, zSize;
    public int x, y, z;
    public TMP_Dropdown terrainDropdown;
    public GameObject revealMessage, resolveBroken;
    public Button actionButton, fieldButton;
    public SoldierPortrait soldierPotrait;
    public TextMeshProUGUI ap, mp, location, revealMessageText;
    public GameObject LOSCountPrefab;
    public GameObject arrowPrefab;
    public List<GameObject> activeArrows = new();

    public void DisplayInFriendlyColumn()
    {
        transform.SetParent(SoldierManager.Instance.friendlyDisplayColumn.transform);
    }
    public void DisplayInEnemyColumn()
    {
        transform.SetParent(SoldierManager.Instance.enemyDisplayColumn.transform);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (linkedSoldier == null)
            return;

        if (linkedSoldier.IsOnturnAndAlive())
            ShowLineOfSightArrows();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ClearArrows();
    }
    public void OnEnable()
    {
        ClearArrows();
    }
    public void OnDisable()
    {
        ClearArrows();
    }
    void ShowLineOfSightArrows()
    {
        ClearArrows();

        int count = 0;
        foreach (Soldier s in GameManager.Instance.AllFieldedEnemySoldiers())
        {
            if (linkedSoldier.CanSeeInOwnRight(s) && s.IsAlive())
            {
                GameObject arrow = Instantiate(arrowPrefab, MenuManager.Instance.UICanvas.transform);
                SetupArrow(arrow, GetComponent<RectTransform>(), s.soldierUI.GetComponent<RectTransform>());
                activeArrows.Add(arrow);
                count++;
            }
            continue;
        }
        GameObject counter = Instantiate(LOSCountPrefab, transform);
        counter.GetComponentInChildren<TextMeshProUGUI>().text = $"{count}";
        activeArrows.Add(counter);
    }
    void SetupArrow(GameObject arrow, RectTransform from, RectTransform to)
    {
        Vector2 start = new(from.position.x + (from.rect.width / 2) + 15, from.position.y + 35);
        Vector2 end = new(to.position.x - (to.rect.width / 4), to.position.y);
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        RectTransform rt = arrow.GetComponent<RectTransform>();
        rt.position = start;
        rt.sizeDelta = new Vector2(distance, rt.sizeDelta.y);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    void ClearArrows()
    {
        foreach (var arrow in activeArrows)
            Destroy(arrow);
        activeArrows.Clear();
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
            SoundManager.Instance.PlayZombieSelection(linkedSoldier);
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

                if (DataPersistenceManager.Instance.lozMode && linkedSoldier.IsZombie())
                    linkedSoldier.LeapIncrementStats(GameManager.Instance.currentRound / GameManager.Instance.roundsBetweenLeaps);
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
