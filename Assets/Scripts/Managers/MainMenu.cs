using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IDataPersistence
{
    //secret override key
    public KeyCode overrideKey = KeyCode.LeftShift;
    public KeyCode secondOverrideKey = KeyCode.Space;

    public SoldierManager soldierManager;
    public ItemManager itemManager;
    public MainGame game;
    public WeatherGen weather;
    public DipelecGen dipelec;
    public POIManager poiManager;
    public SoundManager soundManager;

    public TextMeshProUGUI gameTimer, turnTimer, roundIndicator, teamTurnIndicator, weatherIndicator;
    public GameObject menuUI, teamTurnOverUI, teamTurnStartUI, setupMenuUI, gameTimerUI, gameMenuUI, soldierOptionsUI, soldierStatsUI, shotUI, flankersShotUI, shotConfirmUI, overwatchShotUI,
        shotResultUI, moveUI, overmoveUI, suppressionMoveUI, moveToSameSpotUI, meleeUI, noMeleeTargetsUI, meleeBreakEngagementRequestUI, meleeResultUI, meleeConfirmUI,
        configureUI, soldierOptionsAdditionalUI, dipelecUI, dipelecResultUI, damageEventUI, overrideUI, detectionAlertUI, detectionUI, lostLosUI, damageUI, 
        traumaAlertUI, traumaUI, explosionUI, inspirerUI, xpAlertUI, xpLogUI, promotionUI, lastandicideConfirmUI, brokenFledUI, endSoldierTurnAlertUI, playdeadAlertUI, 
        coverAlertUI, overwatchUI, externalItemSourcesUI, inventorySourceIconsUI, flankersMeleeAttackerUI, flankersMeleeDefenderUI, detectionAlertPrefab, 
        lostLosAlertPrefab, losGlimpseAlertPrefab, damageAlertPrefab, traumaAlertPrefab, inspirerAlertPrefab, xpAlertPrefab, promotionAlertPrefab, 
        allyInventoryIconPrefab, groundInventoryIconPrefab, gbInventoryIconPrefab, inventoryPanelGroundPrefab, inventoryPanelAllyPrefab, inventoryPanelGoodyBoxPrefab, soldierSnapshotPrefab, soldierPortraitPrefab, possibleFlankerPrefab, 
        meleeAlertPrefab, overwatchShotUIPrefab, dipelecRewardPrefab, explosionAlertPrefab, endTurnButton, overrideButton, overrideTimeStopIndicator, overrideVersionDisplay, overrideVisibilityDropdown, 
        overrideInsertObjectsButton, overrideInsertObjectsUI, overrideMuteButton, undoButton, blockingScreen, itemSlotPrefab, itemIconPrefab, useItemUI, ULFResultUI;
    public ItemIconGB gbItemIconPrefab;
    public LOSArrow LOSArrowPrefab;
    public OverwatchArc overwatchArcPrefab;
    public SightRadiusCircle sightRadiusCirclePrefab;
    public List<Button> actionButtons;
    public List<Sprite> insignia;
    public Button shotButton, moveButton, meleeButton, configureButton, lastandicideButton, dipElecButton, overwatchButton, coverButton, playdeadButton, additionalOptionsButton;
    private float playTimeTotal;
    public float turnTime;
    public string meleeChargeIndicator;
    public Soldier activeSoldier;
    public bool overrideView, displayLOSArrows, clearShotFlag, clearMeleeFlag, clearDipelecFlag, clearMoveFlag, detectionResolvedFlag, meleeResolvedFlag, shotResolvedFlag, inspirerResolvedFlag, clearDamageEventFlag, 
        xpResolvedFlag, teamTurnOverFlag, teamTurnStartFlag, onItemUseScreen;
    public TMP_InputField LInput, HInput, RInput, SInput, EInput, FInput, PInput, CInput, SRInput, RiInput, ARInput, LMGInput, SnInput, SMGInput, ShInput, MInput, StrInput, DipInput, ElecInput, HealInput;
    public Sprite detection1WayLeft, detection1WayRight, avoidance1WayLeft, avoidance1WayRight, detection2Way, avoidance2Way, avoidance2WayLeft, avoidance2WayRight, 
        detectionOverwatch2WayLeft, detectionOverwatch2WayRight, avoidanceOverwatch2WayLeft, avoidanceOverwatch2WayRight, overwatch1WayLeft, overwatch1WayRight, noDetect2Way, fist, explosiveBarrel, covermanSprite;
    public Color normalTextColour = new(0.196f, 0.196f, 0.196f);

    private readonly string[][] allStats =
    {
        new string[] { "L", "Leadership" },
        new string[] { "H", "Health" },
        new string[] { "R", "Resilience" },
        new string[] { "S", "Speed" },
        new string[] { "E", "Evasion" },
        new string[] { "F", "Stealth" },
        new string[] { "P", "Perceptiveness" },
        new string[] { "C", "Camouflage" },
        new string[] { "SR", "Sight Radius" },
        new string[] { "Ri", "Rifle" },
        new string[] { "AR", "Assault Rifle" },
        new string[] { "LMG", "Light Machine Gun" },
        new string[] { "Sn", "Sniper Rifle" },
        new string[] { "SMG", "Sub-Machine Gun" },
        new string[] { "Sh", "Shotgun" },
        new string[] { "M", "Melee" },
        new string[] { "Str", "Strength" },
        new string[] { "Dip", "Diplomacy" },
        new string[] { "Elec", "Electronics" },
        new string[] { "Heal", "Healing"},
    };

    private readonly string[][] abilitiesUpgradedAbilities =
{
        new string[] { "Adept", "Aficionado" },
        new string[] { "Avenger", "Exactor" },
        new string[] { "Bloodletter", "Masochist" },
        new string[] { "Bull", "Colossus" },
        new string[] { "Calculator", "Supercomputer" },
        new string[] { "Daredevil", "Spider" },
        new string[] { "Dissuader", "Omen of Death" },
        new string[] { "Experimentalist", "Chemist" },
        new string[] { "Fighter", "Pugilist" },
        new string[] { "Guardsman", "Sentinel" },
        new string[] { "Gunner", "Cannoneer" },
        new string[] { "Illusionist", "Ghost" },
        new string[] { "Informer", "Double Agent" },
        new string[] { "Inspirer", "Galvaniser" },
        new string[] { "Insulator", "Absorber" },
        new string[] { "Jammer", "Corrupter" },
        new string[] { "Learner", "Mastermind" },
        new string[] { "Locater", "Logistician" },
        new string[] { "Patriot", "Zealot" },
        new string[] { "Planner", "Prophet" },
        new string[] { "Politician", "Master's Ally" },
        new string[] { "Revoker", "Pacifier" },
        new string[] { "Shadow", "Shapeshifter" },
        new string[] { "Sharpshooter", "Deadeye" },
        new string[] { "Spotter", "Tracker" },
        new string[] { "Sprinter", "Olympian" },
        new string[] { "Tactician", "Creator" },
        new string[] { "Tranquiliser", "Anaesthetist" },
        new string[] { "Vaulter", "Acrobat" },
        new string[] { "Witness", "Hypnotist" },
    };
    public readonly string[,] specialtiesStats =
    {
        { "Commander (L)", "Leadership", "L"},
        { "Spartan (H)", "Health", "H" },
        { "Survivor (R)", "Resilience", "R"},
        { "Runner (S)", "Speed", "S" },
        { "Evader (E)", "Evasion", "E" },
        { "Assassin (F)", "Stealth", "F" },
        { "Seeker (P)", "Perceptiveness", "P" },
        { "Chameleon (C)", "Camouflage", "C" },
        { "Scout (SR)", "Sight Radius", "SR" },
        { "Infantryman (Ri)", "Rifle", "Ri" },
        { "Operator (AR)", "Assault Rifle", "AR" },
        { "Earthquake (LMG)", "Light Machine Gun", "LMG" },
        { "Hunter (Sn)", "Sniper Rifle", "Sn" },
        { "Cyclone (SMG)", "Sub-Machine Gun", "SMG" },
        { "Hammer (Sh)", "Shotgun", "Sh" },
        { "Wolf (M)", "Melee", "M" },
        { "Hercules (Str)", "Strength", "Str" },
        { "Diplomat (Dip)", "Diplomacy", "Dip" },
        { "Technician (Elec)", "Electronics", "Elec" },
        { "Medic (Heal)", "Healing", "Heal" }
    };

    public List<string> terrainList = new() { "Urban", "Desert", "Jungle", "Alpine" };
    public void LoadData(GameData data)
    {
        playTimeTotal = data.playTimeTotal;
        turnTime = data.turnTime;
    }

    public void SaveData(ref GameData data)
    {
        data.playTimeTotal = playTimeTotal;
        data.turnTime = turnTime;
    }
    public string FindStringInColXReturnStringInColYInMatrix(string[,] matrix, string searchString, int x, int y)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
            if (matrix[i, x] == searchString)
                return matrix[i, y];
        return null;
    }
    void Start()
    {
        //check you are in corrrect scene
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Battlefield"))
        {
            //check the game has started and weather exists
            if (game.currentRound > 0 && weather.savedWeather.Count > 0)
            {
                setupMenuUI.SetActive(false);
                gameTimerUI.SetActive(true);
                gameMenuUI.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Battlefield"))
        {
            //check the game has started and weather exists
            if (game.currentRound > 0 && weather.savedWeather.Count > 0)
            {
                if (game.currentRound <= game.maxRounds)
                {
                    playTimeTotal += Time.deltaTime;
                    turnTime += Time.deltaTime;
                    weatherIndicator.text = DisplayWeather();
                }
                if (!game.gameOver)
                {
                    DisplaySoldiers();
                    RenderSoldierVisuals();
                    CheckWinConditions();
                }
                
                DisplayItems();
                gameTimer.text = FormatFloatTime(playTimeTotal);
                turnTimer.text = FormatFloatTime(game.maxTurnTime - turnTime);
                TurnTimerColour();
                ChangeRoundIndicators();

                //show LOS gizmos
                DisplayLOSGizmos();

                if (activeSoldier != null)
                {
                    DisplayActiveSoldier();
                    DisplayActionMenu();
                }

                //hard turn cap
                /*if (turnTime > setBattlefieldParameters.maxTurnTime * 1.2)
                {
                    game.EndTurn();
                }*/
            }
        }
    }











    //helper functions - menu
    public bool OverrideKey()
    {
        if (Input.GetKey(overrideKey))
            return true;
        else
            return false;
    }
    public bool SecondOverrideKey()
    {
        if (Input.GetKey(secondOverrideKey))
            return true;
        else
            return false;
    }
    public bool SecondOverrideKeyDown()
    {
        if (Input.GetKeyDown(secondOverrideKey))
            return true;
        else
            return false;
    }
    public bool SecondOverrideKeyUp()
    {
        if (Input.GetKeyUp(secondOverrideKey))
            return true;
        else
            return false;
    }
    public void DisplayLOSGizmos()
    {
        if (OverrideKey())
        {
            //create gizmos upon key press
            if (SecondOverrideKeyDown())
            {
                CreateLOSArrows();
                if (activeSoldier != null)
                {
                    CreateSightRadiusCircle();
                    CreateOverwatchArc();
                }
            }

            //reveal GM objects while held
            if (SecondOverrideKey())
            {
                DisplayGMObjects();
            }
            else
            {
                HideGMObjects();
            }

            //destroy gizmos upon key release
            if (SecondOverrideKeyUp())
            {
                DestroySightRadiusCircle();
                DestroyLOSArrows();
                DestroyOverwatchArc();
            }
        }
    }
    public void DisplayGMObjects()
    {
        var GMObjects = FindObjectsOfType<GMObject>(true);

        foreach (GMObject obj in GMObjects)
            obj.gameObject.SetActive(true);
    }
    public void HideGMObjects()
    {
        var GMObjects = FindObjectsOfType<GMObject>(true);

        foreach (GMObject obj in GMObjects)
            obj.gameObject.SetActive(false);
    }
    public void CreateLOSArrows()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            foreach (string id in s.RevealingSoldiers)
            {
                LOSArrow arrow = Instantiate(LOSArrowPrefab).Init(s, soldierManager.FindSoldierById(id));
                arrow.transform.SetAsLastSibling();
            }
        }
    }
    public void DestroyLOSArrows()
    {
        var LOSArrows = FindObjectsOfType<LOSArrow>(true);
        foreach (LOSArrow arrow in LOSArrows)
            Destroy(arrow.gameObject);
    }
    public void CreateOverwatchArc()
    {
        if (activeSoldier.IsOnOverwatch())
        {
            OverwatchArc overwatchArc = Instantiate(overwatchArcPrefab).Init(activeSoldier);
            overwatchArc.transform.SetAsLastSibling();
        }
    }
    public void DestroyOverwatchArc()
    {
        var overwatchArcs = FindObjectsOfType<OverwatchArc>(true);
        foreach (OverwatchArc overwatchArc in overwatchArcs)
            Destroy(overwatchArc.gameObject);
    }
    public void CreateSightRadiusCircle()
    {
        SightRadiusCircle sightRadiusCircle = Instantiate(sightRadiusCirclePrefab).Init(activeSoldier);
        sightRadiusCircle.transform.SetAsLastSibling();
    }
    public void DestroySightRadiusCircle()
    {
        var sightRadiusCircles = FindObjectsOfType<SightRadiusCircle>(true);
        foreach (SightRadiusCircle sightRadiusCircle in sightRadiusCircles)
            Destroy(sightRadiusCircle.gameObject);
    }
    public string PrintArray(Array array)
    {
        string str = "[";
        foreach (object obj in array)
        {
            str += obj.ToString();
            str += ", ";
        }
        str += "]";
        return str;
    }
    public string PrintList<T>(List<T> list)
    {
        string str = "";
        //str += "[";
        foreach (object obj in list)
        {
            str += obj.ToString();
            if (list.Count > 1 && obj != (object)list.Last())
                str += ", ";
        }
        //str += "]";
        return str;
    }
    public List<string> ReadStringToList(string stringToList)
    {
        return Regex.Split(stringToList, @"(?:,\s+)|(['""].+['""])(?:,\s+)").ToList();
    }
    public string IdToName(string id)
    {
        Soldier s = soldierManager.FindSoldierById(id);
        if (s != null)
            return s.soldierName;
        else
            return id;
    }
    public void FreezeTime()
    {
        Time.timeScale = 0f;
    }
    public void UnfreezeTime()
    {
        Time.timeScale = 1.0f;
    }
    public void SetXpResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTime();
        else
            FreezeTime();

        xpResolvedFlag = value;
    }
    public void SetMeleeResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTime();
        else
            FreezeTime();

        meleeResolvedFlag = value;
    }
    public void SetShotResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTime();
        else
            FreezeTime();

        shotResolvedFlag = value;
    }
    public void SetInspirerResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTime();
        else
            FreezeTime();

        inspirerResolvedFlag = value;
    }
    public void SetTeamTurnOverFlagTo(bool value)
    {
        if (value)
            UnfreezeTime();
        else
        {
            FreezeTime();
            OpenPlayerTurnOverUI();
        }

        teamTurnOverFlag = value;
    }
    public void SetTeamTurnStartFlagTo(bool value)
    {
        if (value)
            UnfreezeTime();
        else
        {
            FreezeTime();
            OpenPlayerTurnStartUI();
        }

        teamTurnStartFlag = value;
    }













    //override functions - menu
    public void OpenOverrideMenu()
    {
        if (overrideView)
        {
            //exit override
            UnfreezeTime();
            ToggleOverrideView();
            endTurnButton.SetActive(true);
            overrideButton.GetComponentInChildren<TextMeshProUGUI>().text = "Override";
            overrideTimeStopIndicator.SetActive(false);
            overrideVersionDisplay.SetActive(false);
            overrideVisibilityDropdown.SetActive(false);
            overrideInsertObjectsButton.SetActive(false);
            overrideMuteButton.SetActive(false);
        }
        else
            overrideUI.SetActive(true);
    }
    public void CloseOverrideMenu()
    {
        overrideUI.SetActive(false);
    }
    public void ToggleOverrideView()
    {
        overrideView = !overrideView;
    }
    public void ConfirmOverride()
    {
        if (OverrideKey())
        {
            //enter override
            FreezeTime();
            ToggleOverrideView();
            CloseOverrideMenu();
            soundManager.PlayOverrideAlarm();
            endTurnButton.SetActive(false);
            overrideButton.GetComponentInChildren<TextMeshProUGUI>().text = "Resume";
            overrideTimeStopIndicator.SetActive(true);
            overrideVersionDisplay.SetActive(true);
            overrideVisibilityDropdown.SetActive(true);
            overrideInsertObjectsButton.SetActive(true);
            overrideMuteButton.SetActive(true);
            GetOverrideVisibility();
        }
    }
    public void GetOverrideHealthState(Transform soldierStatsUI)
    {
        TMP_Dropdown dropdown = soldierStatsUI.Find("General").Find("OverrideHealthState").Find("HealthStateDropdown").GetComponent<TMP_Dropdown>();

        if (activeSoldier.IsUnconscious())
            dropdown.value = 2;
        else if (activeSoldier.IsLastStand())
            dropdown.value = 1;
        else
            dropdown.value = 0;
    }
    public void SetOverrideHealthState()
    {
        TMP_Dropdown dropdown = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideHealthState").Find("HealthStateDropdown").GetComponent<TMP_Dropdown>();

        if (dropdown.value == 2)
            activeSoldier.MakeUnconscious();
        else if (dropdown.value == 1)
            activeSoldier.MakeLastStand();
        else
            activeSoldier.MakeActive();
    }
    public void GetOverrideVisibility()
    {
        TMP_Dropdown dropdown = overrideVisibilityDropdown.GetComponent<TMP_Dropdown>();

        dropdown.value = weather.CurrentVis switch
        {
            "Full" => 0,
            "Good" => 1,
            "Moderate" => 2,
            "Poor" => 3,
            "Zero" => 4,
            _ => 0,
        };
    }
    public void SetOverrideVisibility()
    {
        //print("setoverridevis");
        TMP_Dropdown dropdown = overrideVisibilityDropdown.GetComponent<TMP_Dropdown>();
        string oldVis = weather.CurrentVis;

        weather.CurrentVis = dropdown.value switch
        {
            0 => "Full visibility",
            1 => "Good visibility",
            2 => "Moderate visibility",
            3 => "Poor visibility",
            4 => "Zero visibility",
            _ => "",
        };

        if (game.CheckWeatherChange(oldVis, weather.CurrentVis) != "false")
            StartCoroutine(game.DetectionAlertAll("statChange", false));
    }
    public void ChangeHP()
    {
        TMP_InputField hpInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("OverrideHP").GetComponent<TMP_InputField>();
        
        if (int.TryParse(hpInput.text, out int newHp))
        {
            if (newHp > 0)
            {
                if (activeSoldier.hp == 0)
                    activeSoldier.Resurrect(newHp);
                else
                {
                    if (newHp < activeSoldier.hp)
                        activeSoldier.TakeDamage(null, activeSoldier.hp - newHp, true, new List<string> { "Override" });
                    else if (newHp > activeSoldier.hp)
                        activeSoldier.TakeHeal(null, newHp - activeSoldier.hp, 0, true, false);
                }
            }
            else if (newHp == 0)
                activeSoldier.Kill(null, new List<string> { "Override" });
                
        }

        hpInput.text = "";
    }
    public void ChangeAP()
    {
        TMP_InputField apInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("OverrideAP").GetComponent<TMP_InputField>();
        
        if (int.TryParse(apInput.text, out int newAp))
        {
            if (newAp >= 0)
            {
                activeSoldier.usedAP = false;
                activeSoldier.ap = newAp; 
            }
        }

        apInput.text = "";
    }
    public void ChangeMP()
    {
        TMP_InputField mpInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("OverrideMP").GetComponent<TMP_InputField>();
        
        if (int.TryParse(mpInput.text, out int newMp))
            if (newMp >= 0)
            {
                activeSoldier.usedMP = false;
                activeSoldier.mp = newMp;
            }


        mpInput.text = "";
    }
    public void ChangeXP()
    {
        TMP_InputField xpInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("OverrideXP").GetComponent<TMP_InputField>();
        
        if (int.TryParse(xpInput.text, out int newXp))
        {
            if (newXp >= 0)
            {
                if (newXp > activeSoldier.xp)
                    AddXpAlert(activeSoldier, newXp - activeSoldier.xp, "(Override) Extra xp added.", false);

                activeSoldier.xp = newXp;
                
            }
        }

        xpInput.text = "";
    }
    public void ChangeBaseStat(string code)
    {
        if (int.TryParse((GetType().GetField(code + "Input").GetValue(this) as TMP_InputField).text, out int newBaseVal) && newBaseVal >= 0)
        {
            activeSoldier.stats.SetStat(code, newBaseVal);

            //recalculate stats
            activeSoldier.CalculateActiveStats();

            //run detection alert if SR, C, F, P is changed
            if (code == "SR" || code == "C" || code == "F" || code == "P")
                StartCoroutine(game.DetectionAlertSingle(activeSoldier, "statChange", Vector3.zero, string.Empty, false));

            //run melee control re-eval if R, Str, M is changed
            if (activeSoldier.IsMeleeEngaged() && (code == "R" || code == "Str" || code == "M"))
                StartCoroutine(game.DetermineMeleeControllerMultiple(activeSoldier));
        }

        //clear override input
        (GetType().GetField(code + "Input").GetValue(this) as TMP_InputField).text = "";
    }
    public void ChangeAbilities()
    {
        TMP_InputField abilityInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideAbility").GetComponent<TMP_InputField>();
        bool invalid = true;
        List<string> abilityList = ReadStringToList(abilityInput.text);

        if (abilityList.Count > 0)
        {
            foreach (string str in abilityList)
                foreach (string[] abilityTuple in abilitiesUpgradedAbilities)
                    if (abilityTuple[0] == str || abilityTuple[1] == str)
                        invalid = false;

            if (!invalid)
                activeSoldier.soldierAbilities = abilityList;
        }

        abilityInput.text = "";
    }
    public void ChangeLocation(string xyz)
    {
        string overrideLocation = "OverrideLocation" + xyz;
        TMP_InputField locationInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideLocation").Find(overrideLocation).GetComponent<TMP_InputField>();
        
        if (int.TryParse(locationInput.text, out int newlocationInput))
        {
            if ((xyz.Equals("X") && newlocationInput >= 1 && newlocationInput <= game.maxX) || (xyz.Equals("Y") && newlocationInput >= 1 && newlocationInput <= game.maxY) || (xyz.Equals("Z") && newlocationInput >= 0 && newlocationInput <= game.maxZ))
            {
                if (xyz.Equals("X"))
                    activeSoldier.X = newlocationInput;
                else if (xyz.Equals("Y"))
                    activeSoldier.Y = newlocationInput;
                else
                    activeSoldier.Z = newlocationInput;

                StartCoroutine(game.DetectionAlertSingle(activeSoldier, "losChange", Vector3.zero, string.Empty, true));
            }
        }

        locationInput.text = string.Empty;
    }
    public void ChangeTerrainOn()
    {
        TMP_InputField terrainOnInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideTerrainOn").GetComponent<TMP_InputField>();
        string terrainOnString = terrainOnInput.text;

        if (terrainList.Contains(terrainOnString) && terrainOnString != "")
            activeSoldier.TerrainOn = terrainOnString;

        terrainOnInput.text = "";
    }
    public void ChangeRoundsWithoutFood()
    {
        TMP_InputField roundsWithoutFoodInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideRoundsWithoutFood").GetComponent<TMP_InputField>();
        
        if (int.TryParse(roundsWithoutFoodInput.text, out int newRoundsWithoutFood))
            if (newRoundsWithoutFood >= 0)
                activeSoldier.RoundsWithoutFood = newRoundsWithoutFood;

        roundsWithoutFoodInput.text = "";
    }
    public void ChangeTraumaPoints()
    {
        TMP_InputField traumaInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideTraumaPoints").GetComponent<TMP_InputField>();
        
        if (int.TryParse(traumaInput.text, out int newTrauma))
            if (newTrauma >= 0)
            {
                activeSoldier.tp = 0;
                activeSoldier.TakeTrauma(newTrauma);
            }

        traumaInput.text = "";
    }







    //display functions - menu
    public string DisplayWeather()
    {
        string displayWeather = "";

        displayWeather += weather.CurrentWeather;

        foreach (Soldier s in game.AllSoldiers())
            if (s.IsOnturnAndAlive() && s.IsExperimentalist())
                displayWeather += "\n<color=green>" + weather.NextTurnWeather + "</color>";

        return displayWeather;
    }
    
    public void DisplaySoldiers()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            if (overrideView)
            {
                s.soldierUI.SetActive(true);
                s.soldierUI.transform.Find("ActionButton").GetComponent<Button>().interactable = true;
            }
            else
            {
                if (s.soldierTeam == game.currentTeam || s.IsSpotted())
                    s.soldierUI.SetActive(true);
                else
                    s.soldierUI.SetActive(false);

                if (s.soldierTeam != game.currentTeam && s.IsSpotted())
                    s.soldierUI.transform.Find("ActionButton").GetComponent<Button>().interactable = false;
                else
                    s.soldierUI.transform.Find("ActionButton").GetComponent<Button>().interactable = true;
            }
        }
    }
    public void RenderSoldierVisuals()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            if (overrideView)
                s.GetComponent<Renderer>().enabled = true;
            else
            {
                if (s.soldierTeam == game.currentTeam || s.IsRevealed() || s.IsDead() || s.IsPlayingDead() || s.IsSpotted())
                    s.GetComponent<Renderer>().enabled = true;
                else
                    s.GetComponent<Renderer>().enabled = false;
            }
            s.PaintColor();
        }
    }
    public void CheckWinConditions()
    {
        int p1DeadCount = 0, p2DeadCount = 0;

        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsDead() || s.IsUnconscious())
            {
                if (s.soldierTeam == 1)
                    p1DeadCount++;
                else
                    p2DeadCount++;
            }    
        }

        if (p1DeadCount == game.AllSoldiers().Count / 2)
            game.GameOver("<color=blue>Team 2</color> Victory");

        if (p2DeadCount == game.AllSoldiers().Count / 2)
            game.GameOver("<color=red>Team 1</color> Victory");
    }
    public void DisplayItems()
    {
        var itemList = FindObjectsOfType<Item>();
        foreach (Item i in itemList)
        {
            if (overrideView)
                i.GetComponent<Renderer>().enabled = true;
            else
            {
                if (i.transform.parent == null)
                    i.GetComponent<Renderer>().enabled = true;
                else
                    i.GetComponent<Renderer>().enabled = false;
            }
        }
    }
    public void DisplaySoldiersGameOver()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            s.soldierUI.SetActive(true);
            s.fielded = true;
            s.GetComponent<Renderer>().enabled = true;
            s.CheckSpecialityColor(s.soldierSpeciality);
            blockingScreen.SetActive(true);
        }
    }
    public void ChangeRoundIndicators()
    {
        if (!game.gameOver)
        {
            roundIndicator.text = "Round " + game.currentRound;
            if (game.currentTeam == 1)
                teamTurnIndicator.text = "<color=red>";
            else
                teamTurnIndicator.text = "<color=blue>";
            teamTurnIndicator.text += "Team " + game.currentTeam + "</color> Turn";
        }
    }
    public string FormatFloatTime(float time)
    {
        int hours = Mathf.Abs((int)time / 3600);
        int minutes = Mathf.Abs((int)time % 3600 / 60);
        int seconds = Mathf.Abs((int)time % 3600 % 60);

        if (time < 0)
            return string.Format("+{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        else
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    public void TurnTimerColour()
    {
        if (turnTime > game.maxTurnTime)
        {
            turnTimer.color = Color.red;
            soundManager.PlayBanzai();
        }
        else if (turnTime > game.maxTurnTime * 0.75)
        {
            turnTimer.color = new Color(1f, 0.5f, 0f);
        }
        else if (turnTime > game.maxTurnTime * 0.5)
        {
            turnTimer.color = Color.yellow;
        }
        else
        {
            turnTimer.color = Color.green;
        }
    }
    public void ToggleSoldierStatsUI()
    {
        if (soldierStatsUI.activeInHierarchy)
            CloseSoldierStatsUI();
        else
            OpenSoldierStatsUI();
    }
    public void OpenSoldierStatsUI()
    {
        onItemUseScreen = true;
        soldierStatsUI.transform.Find("SoldierLoadout").GetComponent<InventoryDisplayPanelSoldier>().Init(activeSoldier);
        soldierStatsUI.SetActive(true);
    }
    public void CloseSoldierStatsUI()
    {
        onItemUseScreen = false;
        soldierStatsUI.SetActive(false);
    }
    public void ToggleAdditionalActionMenu()
    {
        if (soldierOptionsAdditionalUI.activeSelf)
            soldierOptionsAdditionalUI.SetActive(false);
        else
            soldierOptionsAdditionalUI.SetActive(true);
    }
    public void CloseAdditionalActionMenu()
    {
        soldierOptionsAdditionalUI.SetActive(false);
    }
    public Dictionary<Button, string> AddAllButtons(Dictionary<Button, string> buttonStates)
    {
        foreach (Button b in actionButtons)
            buttonStates.Add(b, "");

        return buttonStates;
    }

    public Dictionary<Button, string> ExceptButton(Dictionary<Button, string> buttonStates, Button activeButton)
    {
        buttonStates.Remove(activeButton);
        return buttonStates;
    }

    public void DisplayActionMenu()
    {
        Dictionary<Button, string> buttonStates = new();

        //display lastandicide button
        if (activeSoldier.IsLastStand())
            lastandicideButton.gameObject.SetActive(true);
        else
            lastandicideButton.gameObject.SetActive(false);

        if (game.gameOver)
            GreyAll("Game Over");
        else if (overrideView)
            GreyOutButtons(AddAllButtons(buttonStates), "Override");
        else if (activeSoldier.IsDead())
            GreyOutButtons(AddAllButtons(buttonStates), "Dead");
        else if (activeSoldier.IsUnconscious())
            GreyOutButtons(AddAllButtons(buttonStates), "<color=blue>Unconscious</color>");
        else if (activeSoldier.IsStunned())
            GreyOutButtons(AddAllButtons(buttonStates), "Stunned");
        else if (activeSoldier.IsPlayingDead())
            GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), playdeadButton), "<color=yellow>Playdead</color>");
        else if (activeSoldier.ap == 0)
            GreyOutButtons(AddAllButtons(buttonStates), "No AP");
        else if (activeSoldier.tp == 4)
        {
            //if in last stand regain control
            if (activeSoldier.IsLastStand())
            {
                buttonStates.Add(moveButton, "Last Stand");
                GreyOutButtons(buttonStates, "");
            }
            else
                GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), moveButton), "Broken");
        }
        else if (activeSoldier.IsMeleeControlled())
        {
            if (activeSoldier.HasSMGOrPistolEquipped())
                GreyOutButtons(ExceptButton(ExceptButton(AddAllButtons(buttonStates), meleeButton), shotButton), "<color=red>Melee Controlled</color>");
            else
                GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), meleeButton), "<color=red>Melee Controlled</color>");
        }
        else
        {
            //block move button
            if (activeSoldier.IsLastStand())
                buttonStates.Add(moveButton, "Last Stand");
            else if (activeSoldier.mp == 0)
                buttonStates.Add(moveButton, "No MA");
            else if (activeSoldier.IsMeleeControlling())
                buttonStates.Add(moveButton, "Melee Controlling");

            //block cover button
            if (activeSoldier.IsWearingJuggernautArmour(false))
                buttonStates.Add(coverButton, "<color=green>Juggernaut</color>");
            else if (activeSoldier.IsInCover())
                buttonStates.Add(coverButton, "<color=green>Taking Cover</color>");


            //block shot and overwatch buttons
            if (!activeSoldier.HasGunEquipped())
            {
                buttonStates.Add(shotButton, "No Gun");
                buttonStates.Add(overwatchButton, "No Gun");
            }
            else if (!activeSoldier.IsAbleToSee())
            {
                buttonStates.Add(shotButton, "Blind");
                buttonStates.Add(overwatchButton, "Blind");
            }
            else if (activeSoldier.IsMeleeControlling())
            {
                if (!activeSoldier.HasSMGOrPistolEquipped())
                    buttonStates.Add(shotButton, "Melee Controlling");
                buttonStates.Add(overwatchButton, "Melee Controlling");
            }
            else if (activeSoldier.IsDualWielding())
            {
                buttonStates.Add(shotButton, "Dual Wield"); //TO BE REMOVED AFTER DUAL WIELD RULING
                buttonStates.Add(overwatchButton, "Dual Wield");
            }
            else if (!activeSoldier.EquippedGun.CheckAnyAmmo())
            {
                buttonStates.Add(shotButton, "Gun Empty");
                buttonStates.Add(overwatchButton, "Gun Empty");
            }

            //block melee button
            if (!activeSoldier.FindMeleeTargets())
                buttonStates.Add(meleeButton, "No Target");
            else if (activeSoldier.stats.SR.Val == 0)
                buttonStates.Add(meleeButton, "Blind");

            //block dipelec button
            if (activeSoldier.stats.SR.Val == 0)
                buttonStates.Add(dipElecButton, "Blind");
            else if (!activeSoldier.TerminalInRange())
                buttonStates.Add(dipElecButton, "No Terminal");
            else if (!activeSoldier.ClosestTerminal().terminalEnabled)
                buttonStates.Add(dipElecButton, "Terminal Disabled");

            //block playdead button
            if (activeSoldier.IsWearingJuggernautArmour(false))
                buttonStates.Add(playdeadButton, "Juggernaut");

            GreyOutButtons(buttonStates, "");
        }

        //change config button text if first ap use
        if (activeSoldier.roundsFielded == 0 && !activeSoldier.usedAP)
            configureButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Spawn Config";
        else
            configureButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Config";

        /*if (activeSoldier.usedAP)
            undoButton.SetActive(true);
        else
            undoButton.SetActive(false);*/
    }
    public void GreyAll(string reason)
    {
        soldierOptionsUI.transform.Find("SoldierActions").Find("AllReason").GetComponent<TextMeshProUGUI>().text = reason;

        foreach (Button b in actionButtons)
            GreyOut(b, "");

        GreyOut(additionalOptionsButton, "");
    }

    public void GreyOutButtons(Dictionary<Button, string> buttonStates, string multiButtonReason)
    {
        soldierOptionsUI.transform.Find("SoldierActions").Find("AllReason").GetComponent<TextMeshProUGUI>().text = multiButtonReason;

        if (buttonStates.Count > 0)
        {
            foreach (Button b in actionButtons)
            {
                if (buttonStates.ContainsKey(b))
                {
                    buttonStates.TryGetValue(b, out string bValue);
                    GreyOut(b, bValue);
                }
                else
                {
                    UnGrey(b, "");
                    UnGrey(additionalOptionsButton, "");
                }
            }
        }
        else
            UnGreyAll();
    }

    public void UnGreyAll()
    {
        soldierOptionsUI.transform.Find("SoldierActions").Find("AllReason").GetComponent<TextMeshProUGUI>().text = "";

        foreach (Button b in actionButtons)
            UnGrey(b, "");

        UnGrey(additionalOptionsButton, "");
    }

    public void UnGrey(Button button, string reason)
    {
        button.interactable = true;
        button.transform.Find("Reason").GetComponent<TextMeshProUGUI>().text = reason;
    }

    public void GreyOut(Button button, string reason)
    {
        button.interactable = false;
        button.transform.Find("Reason").GetComponent<TextMeshProUGUI>().text = reason;
    }
    public void DisplayActiveSoldier()
    {
        Transform soldierBanner = soldierOptionsUI.transform.Find("SoldierBanner");
        soldierBanner.Find("HP").GetComponent<TextMeshProUGUI>().text = "HP: " + activeSoldier.GetFullHP().ToString();
        soldierBanner.Find("AP").GetComponent<TextMeshProUGUI>().text = "AP: " + activeSoldier.ap.ToString();
        soldierBanner.Find("MP").GetComponent<TextMeshProUGUI>().text = "MA: " + activeSoldier.mp.ToString();
        soldierBanner.Find("Speed").GetComponent<TextMeshProUGUI>().text = "Max Move: " + activeSoldier.InstantSpeed.ToString();
        soldierBanner.Find("XP").GetComponent<TextMeshProUGUI>().text = "XP: " + activeSoldier.xp.ToString();
        soldierBanner.Find("Status").GetComponent<TextMeshProUGUI>().text = "Status: " + activeSoldier.GetStatus();

        if (overrideView)
        {
            soldierBanner.Find("OverrideHP").gameObject.SetActive(true);
            soldierBanner.Find("OverrideHP").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.hp.ToString();
            soldierBanner.Find("OverrideAP").gameObject.SetActive(true);
            soldierBanner.Find("OverrideAP").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.ap.ToString();
            soldierBanner.Find("OverrideMP").gameObject.SetActive(true);
            soldierBanner.Find("OverrideMP").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.mp.ToString();
            soldierBanner.Find("OverrideXP").gameObject.SetActive(true);
            soldierBanner.Find("OverrideXP").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.xp.ToString();
        }
        else
        {
            soldierBanner.Find("OverrideHP").gameObject.SetActive(false);
            soldierBanner.Find("OverrideAP").gameObject.SetActive(false);
            soldierBanner.Find("OverrideMP").gameObject.SetActive(false);
            soldierBanner.Find("OverrideXP").gameObject.SetActive(false);
        }

        Transform soldierStatsUI = soldierBanner.Find("SoldierStatsUI");
        if (soldierStatsUI.gameObject.activeInHierarchy)
        {
            activeSoldier.PaintSpeciality(soldierStatsUI);

            foreach (string[] s in allStats)
            {
                Color displayColor = Color.white;
                if (activeSoldier.stats.GetStat(s[0]).Val < activeSoldier.stats.GetStat(s[0]).BaseVal)
                    displayColor = Color.red;
                else if (activeSoldier.stats.GetStat(s[0]).Val > activeSoldier.stats.GetStat(s[0]).BaseVal)
                    displayColor = Color.green;

                if (overrideView)
                {
                    soldierStatsUI.Find("Stats").Find("OverrideBase").gameObject.SetActive(true);
                    soldierStatsUI.Find("Stats").Find("OverrideBase").Find(s[0]).GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.stats.GetStat(s[0].ToString()).BaseVal.ToString();
                    soldierStatsUI.Find("General").Find("OverrideAbility").gameObject.SetActive(true);
                    soldierStatsUI.Find("General").Find("OverrideAbility").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = PrintList(activeSoldier.soldierAbilities);
                    soldierStatsUI.Find("General").Find("OverrideLocation").gameObject.SetActive(true);
                    soldierStatsUI.Find("General").Find("OverrideLocation").Find("OverrideLocationX").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
                    soldierStatsUI.Find("General").Find("OverrideLocation").Find("OverrideLocationY").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
                    soldierStatsUI.Find("General").Find("OverrideLocation").Find("OverrideLocationZ").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();
                    soldierStatsUI.Find("General").Find("OverrideTerrainOn").gameObject.SetActive(true);
                    soldierStatsUI.Find("General").Find("OverrideTerrainOn").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.terrainOn;
                    soldierStatsUI.Find("General").Find("OverrideRoundsWithoutFood").gameObject.SetActive(true);
                    soldierStatsUI.Find("General").Find("OverrideRoundsWithoutFood").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.RoundsWithoutFood.ToString();
                    soldierStatsUI.Find("General").Find("OverrideTraumaPoints").gameObject.SetActive(true);
                    soldierStatsUI.Find("General").Find("OverrideTraumaPoints").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.tp.ToString();
                    soldierStatsUI.Find("General").Find("OverrideHealthState").gameObject.SetActive(true);
                    GetOverrideHealthState(soldierStatsUI);
                }
                else
                {
                    soldierStatsUI.Find("Stats").Find("OverrideBase").gameObject.SetActive(false);
                    soldierStatsUI.Find("General").Find("OverrideAbility").gameObject.SetActive(false);
                    soldierStatsUI.Find("General").Find("OverrideLocation").gameObject.SetActive(false);
                    soldierStatsUI.Find("General").Find("OverrideTerrainOn").gameObject.SetActive(false);
                    soldierStatsUI.Find("General").Find("OverrideRoundsWithoutFood").gameObject.SetActive(false);
                    soldierStatsUI.Find("General").Find("OverrideTraumaPoints").gameObject.SetActive(false);
                    soldierStatsUI.Find("General").Find("OverrideHealthState").gameObject.SetActive(false);
                }

                soldierStatsUI.Find("Stats").Find("Base").Find(s[0]).GetComponent<TextMeshProUGUI>().text = activeSoldier.stats.GetStat(s[0].ToString()).BaseVal.ToString();
                soldierStatsUI.Find("Stats").Find("Active").Find(s[0]).GetComponent<TextMeshProUGUI>().text = activeSoldier.stats.GetStat(s[0].ToString()).Val.ToString();
                soldierStatsUI.Find("Stats").Find("Active").Find(s[0]).GetComponent<TextMeshProUGUI>().color = displayColor;
            }

            soldierStatsUI.Find("General").Find("Name").GetComponent<TextMeshProUGUI>().text = activeSoldier.soldierName;
            soldierStatsUI.Find("General").Find("Rank").GetComponent<TextMeshProUGUI>().text = activeSoldier.rank;
            soldierStatsUI.Find("General").Find("Terrain").GetComponent<TextMeshProUGUI>().text = activeSoldier.soldierTerrain;
            soldierStatsUI.Find("General").Find("Specialty").GetComponent<TextMeshProUGUI>().text = activeSoldier.PrintSoldierSpeciality();
            soldierStatsUI.Find("General").Find("Ability").GetComponent<TextMeshProUGUI>().text = PrintList(activeSoldier.soldierAbilities);
            soldierStatsUI.Find("General").Find("Location").Find("LocationX").GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
            soldierStatsUI.Find("General").Find("Location").Find("LocationY").GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
            soldierStatsUI.Find("General").Find("Location").Find("LocationZ").GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();
            soldierStatsUI.Find("General").Find("TerrainOn").GetComponent<TextMeshProUGUI>().text = activeSoldier.TerrainOn;
            soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().text = activeSoldier.RoundsWithoutFood.ToString();
            soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().text = activeSoldier.tp.ToString();
        }
    }
    
        
        


        /*

         def engaged_state(id)

             ans = ""

             controlled_by = ""

             controlling = ""


             engage_array = @soldier_array[id][236].split(' ')

             for i in 1..engage_array.length - 1

                 if engage_array[i].end_with ? ("c")

                     controlled_by += "#{engage_array[i].to_i}"

                 else
                 controlling += "#{engage_array[i].to_i}"

                 end
             end


             if controlled_by.length > 0

                 controlled_by.prepend("Engaged (Controlled By: ")

                 ans += ", " + "#{controlled_by})".red

             end

             if controlling.length > 0

                 controlling.prepend("Engaged (Controlling: ")

                 ans += ", " + "#{controlling})".green

             end
             ans

         end

         def armour_state(id)

             armour = ""


             if @soldier_array[id][54] > 0 #check for J armour
                 armour += ", " + "Juggernaut Armour".green

             elsif @soldier_array[id][56] > 0 #check for X armour
                 armour += ", " + "Exo Suit".green

             elsif @soldier_array[id][55] > 0 #check for G armour
                 armour += ", " + "Ghillie Suit".green

             elsif @soldier_array[id][57] > 0 #check for stim armour
                 armour += ", " + "Stimulant Armour".green

             elsif @soldier_array[id][53] > 0 #check for B armour
                 armour += ", " + "Body Armour".green

             end
             armour

         end

         def carrying(id)

             carry = ""


             if @soldier_array[id][215] > 0

                 carry += ", " + "Carrying #{name_call(@soldier_array[id][215])}".yellow

             end
             carry

         end

         def spotting(id)

             spot = ""


             if @soldier_array[id][233] > 0

                 spot += ", " + "Spotting #{name_call(@soldier_array[id][233])}".green

             end
             spot

         end*/

    public void CloseSoldierMenu()
    {
        if (activeSoldier.usedAP && activeSoldier.ap > 0 && !overrideView)
            OpenEndSoldierTurnAlertUI();
        else
        {
            activeSoldier.selected = false;
            activeSoldier = null;
            soldierOptionsUI.SetActive(false);
            menuUI.transform.Find("Options Panel").Find("GameOptions").gameObject.SetActive(true);
        }
    }
    public void CloseSoldierMenuUndo()
    {
        activeSoldier.usedAP = false;
        activeSoldier.selected = false;
        activeSoldier = null;
        soldierOptionsUI.SetActive(false);
        menuUI.transform.Find("Options Panel").Find("GameOptions").gameObject.SetActive(true);
    }
    public void OpenEndSoldierTurnAlertUI()
    {
        endSoldierTurnAlertUI.SetActive(true);
    }

    public void CloseEndSoldierTurnAlertUI()
    {
        endSoldierTurnAlertUI.SetActive(false);
    }















    //navigation functions - menu
    public void CreateClicked()
    {
        SceneManager.LoadScene("Create");
    }
    public void QuitClicked()
    {
        Application.Quit();
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }





    


    //detection functions - menu
    public void OpenGMAlertDetectionUI()
    {
        int childCount = 0, overwatchCount = 0;
        foreach (Transform child in detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
        {
            childCount++;
            if (child.Find("DetectionArrow").GetComponent<Image>().sprite.ToString().Contains("verwatch"))
                overwatchCount++;
        }

        if (childCount > 0)
        {
            detectionResolvedFlag = false;
            FreezeTime();

            if (overwatchCount > 1) //more than a single overwatch line detected
                detectionUI.transform.Find("MultiOverwatchAlert").gameObject.SetActive(true);
            else
                detectionUI.transform.Find("MultiOverwatchAlert").gameObject.SetActive(false);
            
            //illusionist ability
            if (activeSoldier != null)
            {
                if (activeSoldier.IsIllusionist() && activeSoldier.IsHidden() && !activeSoldier.illusionedThisMove) //check if it's active in heirarchy
                    detectionAlertUI.transform.Find("OptionPanel").Find("IllusionistAlert").gameObject.SetActive(true);
                else
                    detectionAlertUI.transform.Find("OptionPanel").Find("IllusionistAlert").gameObject.SetActive(false);
            }
            else
                detectionAlertUI.transform.Find("OptionPanel").Find("IllusionistAlert").gameObject.SetActive(false);

            detectionAlertUI.SetActive(true);
            soundManager.PlayDetectionAlarm();
        }
    }
    public void CloseGMAlertDetectionUI()
    {
        detectionAlertUI.SetActive(false);
    }
    public void IllusionistMoveUndo()
    {
        //reset soldier location and ap
        activeSoldier.illusionedThisMove = true;
        activeSoldier.X = (int)game.tempMove.Item1.x;
        activeSoldier.Y = (int)game.tempMove.Item1.y;
        activeSoldier.Z = (int)game.tempMove.Item1.z;
        activeSoldier.TerrainOn = game.tempMove.Item2;
        activeSoldier.ap += game.tempMove.Item3;
        activeSoldier.mp += game.tempMove.Item4;

        //destory detection alerts
        foreach (Transform child in detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            Destroy(child.gameObject);

        CloseGMAlertDetectionUI();
    }
    public bool AllDetectionsMutual()
    {
        foreach (Transform child in detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
        {
            if (!child.Find("DetectionArrow").GetComponent<Image>().sprite.ToString().Contains("Detection2Way"))
                return false;
        }
        return true;
    }
    public void OpenDetectionUI()
    {
        if (OverrideKey())
        {
            //actually open the alert log
            if (detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content").childCount > 0)
            {
                detectionUI.SetActive(true);
                detectionUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

                //show payall/denyall buttons if all detections mutual
                if (AllDetectionsMutual())
                {
                    detectionUI.transform.Find("OptionPanel").Find("PayAll").gameObject.SetActive(true);
                    detectionUI.transform.Find("OptionPanel").Find("DenyAll").gameObject.SetActive(true);
                }
                else
                {
                    detectionUI.transform.Find("OptionPanel").Find("PayAll").gameObject.SetActive(false);
                    detectionUI.transform.Find("OptionPanel").Find("DenyAll").gameObject.SetActive(false);
                }
            }

            if (activeSoldier != null)
                activeSoldier.illusionedThisMove = false;
            
            CloseGMAlertDetectionUI();
        }
        else
            soundManager.PlayOverrideAlarm();
    }
    public void AddDetectionAlert(Soldier detector, Soldier counter, string detectorLabel, string counterLabel, string arrowType)
    {
        print($"Tried to add detection alert {detector.soldierName} to {counter.soldierName} with {arrowType} arrow");
        //block duplicate detection alerts being created, stops override mode creating multiple instances during overwriting detection stats
        foreach (Transform child in detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if ((child.GetComponent<SoldierAlertDouble>().s1 == detector && child.GetComponent<SoldierAlertDouble>().s2 == counter) || (child.GetComponent<SoldierAlertDouble>().s1 == counter && child.GetComponent<SoldierAlertDouble>().s2 == detector))
                Destroy(child.gameObject);

        GameObject detectionAlert = Instantiate(detectionAlertPrefab, detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        if (detectorLabel.Contains("Not detected"))
            detectionAlert.transform.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().interactable = false;

        if (counterLabel.Contains("Not detected"))
            detectionAlert.transform.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().interactable = false;

        detectionAlert.GetComponent<SoldierAlertDouble>().SetSoldiers(detector, counter);
        detectionAlert.transform.Find("DetectionArrow").GetComponent<Image>().sprite = (Sprite)GetType().GetField(arrowType).GetValue(this);

        detectionAlert.transform.Find("Detector").Find("DetectorSR").GetComponent<TextMeshProUGUI>().text = "(SR=" + detector.stats.SR.Val + ")";
        detectionAlert.transform.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text = counterLabel;
        detectionAlert.transform.Find("Detector").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(detector);
        detectionAlert.transform.Find("Detector").Find("DetectorLocation").GetComponent<TextMeshProUGUI>().text = "X:" + detector.X + "\nY:" + detector.Y + "\nZ:" + detector.Z;

        detectionAlert.transform.Find("Counter").Find("CounterSR").GetComponent<TextMeshProUGUI>().text = "(SR=" + counter.stats.SR.Val + ")";
        detectionAlert.transform.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text = detectorLabel;
        detectionAlert.transform.Find("Counter").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(counter);
        detectionAlert.transform.Find("Counter").Find("CounterLocation").GetComponent<TextMeshProUGUI>().text = "X:" + counter.X + "\nY:" + counter.Y + "\nZ:" + counter.Z;
    }
    public void PayAllDetections()
    {
        if (OverrideKey())
        {
            Transform detectionAlert = detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
            ScrollRect detectionScroller = detectionUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();

            detectionScroller.verticalNormalizedPosition = 0.05f;
            foreach (Transform child in detectionAlert)
            {
                child.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().isOn = true;
                child.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().isOn = true;
            }

            ConfirmDetections();
        }
    }
    public void DenyAllDetections()
    {
        if (OverrideKey())
        {
            Transform detectionAlert = detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
            ScrollRect detectionScroller = detectionUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();

            detectionScroller.verticalNormalizedPosition = 0.05f;
            foreach (Transform child in detectionAlert)
            {
                child.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().isOn = false;
                child.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().isOn = false;
            }

            ConfirmDetections();
        }
    }
    public void ConfirmDetections()
    {
        ScrollRect detectionScroller = detectionUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        if (detectionScroller.verticalNormalizedPosition <= 0.05f)
        {
            //create list of soldiers and who they're revealing
            Dictionary<string, List<string>> allSoldiersRevealing = new();
            Dictionary<string, List<string>> allSoldiersRevealedBy = new();
            Dictionary<string, List<string>> allSoldiersNotRevealing = new();
            Dictionary<string, List<string>> allSoldiersNotRevealedBy = new();
            Dictionary<string, List<string>> allSoldiersRevealingFinal = new();

            foreach (Soldier s in game.AllSoldiers())
            {
                allSoldiersRevealing.Add(s.id, new List<string>());
                allSoldiersRevealedBy.Add(s.id, new List<string>());
                allSoldiersNotRevealing.Add(s.id, new List<string>());
                allSoldiersNotRevealedBy.Add(s.id, new List<string>());
                allSoldiersRevealingFinal.Add(s.id, new List<string>());

                //add soldiers that can't possibly be seen cause out of radius
                foreach (Soldier s2 in game.AllSoldiers())
                    if (s.IsOppositeTeamAs(s2) && !s.PhysicalObjectWithinMaxRadius(s2))
                        allSoldiersNotRevealing[s.id].Add(s2.id);
            }

            Transform detectionAlert = detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
            foreach (Transform child in detectionAlert)
            {
                Soldier detector = child.GetComponent<SoldierAlertDouble>().s1;
                Soldier counter = child.GetComponent<SoldierAlertDouble>().s2;

                if (child.Find("Detector").Find("DetectorToggle").GetComponent<Toggle>().isOn == true)
                {
                    if (child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text.Contains("DETECTED") || child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text.Contains("OVERWATCH"))
                    {
                        //if not a glimpse or a retreat detection, add soldier to revealing list
                        if (!child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text.Contains("GLIMPSE") && !child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text.Contains("RETREAT"))
                            allSoldiersRevealing[counter.id].Add(detector.id);
                        else
                        {
                            AddLosGlimpseAlert(detector, child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text);
                            StartCoroutine(OpenLostLOSList());
                            allSoldiersNotRevealing[counter.id].Add(detector.id);
                        }

                        if (detector.IsOnturnAndAlive())
                        {
                            if (detector.stats.F.Val > 2)
                                AddXpAlert(counter, detector.stats.F.Val, "Detected stealthed soldier (" + detector.soldierName + ") with F > 2.", true);
                        }
                        else
                        {
                            if (detector.stats.C.Val > 2)
                                AddXpAlert(counter, detector.stats.C.Val, "Detected camouflaged soldier (" + detector.soldierName + ") with C > 2.", true);
                        }

                        //check for overwatch shot
                        if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("verwatch") && child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("Left"))
                        {
                            StartCoroutine(CreateOverwatchShotUI(counter, detector));
                            StartCoroutine(OpenOverwatchShotUI());
                        }
                    }
                    else
                    {
                        allSoldiersNotRevealing[counter.id].Add(detector.id);
                        if (detector.IsOnturnAndAlive())
                            AddXpAlert(detector, 2, "Avoided stealth detection (" + counter.soldierName + ").", true);
                        else
                            AddXpAlert(detector, 1, "Avoided camouflage detection (" + counter.soldierName + ").", true);
                    }
                }
                else
                    allSoldiersNotRevealing[counter.id].Add(detector.id);

                if (child.Find("Counter").Find("CounterToggle").GetComponent<Toggle>().isOn == true)
                {
                    if (child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text.Contains("DETECTED") || child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text.Contains("OVERWATCH"))
                    {
                        //if not a glimpse or a retreat detection, add soldier to revealing list
                        if (!child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text.Contains("GLIMPSE") && !child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text.Contains("RETREAT"))
                            allSoldiersRevealing[detector.id].Add(counter.id);
                        else
                        {
                            AddLosGlimpseAlert(counter, child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text);
                            StartCoroutine(OpenLostLOSList());
                            allSoldiersNotRevealing[detector.id].Add(counter.id);
                        }

                        if (counter.IsOnturnAndAlive())
                        {
                            if (counter.stats.F.Val > 2)
                                AddXpAlert(detector, counter.stats.F.Val, "Detected stealthed soldier (" + counter.soldierName + ") with F > 2.", true);
                        }
                        else
                        {
                            if (counter.stats.C.Val > 2)
                                AddXpAlert(detector, counter.stats.C.Val, "Detected camouflaged soldier (" + counter.soldierName + ") with C > 2.", true);
                        }

                        //check for overwatch shot
                        /*if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("verwatch") && child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("Left"))
                        {
                            StartCoroutine(CreateOverwatchShotUI(detector, counter));
                            StartCoroutine(OpenOverwatchShotUI());
                        }*/
                    }
                    else
                    {
                        allSoldiersNotRevealing[detector.id].Add(counter.id);
                        if (counter.IsOnturnAndAlive())
                            AddXpAlert(counter, 1, "Avoided stealth detection (" + detector.soldierName + ").", true);
                        else
                            AddXpAlert(counter, 1, "Avoided camouflage detection (" + detector.soldierName + ").", true);
                    }
                }
                else
                    allSoldiersNotRevealing[detector.id].Add(counter.id);
            }

            //combine old revealing list with fresh revealing list and not revealing list
            foreach (KeyValuePair<string, List<string>> keyValuePair in allSoldiersRevealing)
            {
                //print(keyValuePair.Key + " " + game.FindSoldierById(keyValuePair.Key).soldierName);
                List<string> arrayOfRevealingList = allSoldiersRevealing[keyValuePair.Key];
                List<string> arrayOfOldRevealingList = soldierManager.FindSoldierById(keyValuePair.Key).RevealingSoldiers;
                List<string> arrayOfNotRevealingList = allSoldiersNotRevealing[keyValuePair.Key];

                //populate final reveal list using fresh reveals plus old reveals minus not reveals
                List<string> arrayFinalRevealingList = new();
                foreach (string soldierId in arrayOfRevealingList)
                    arrayFinalRevealingList.Add(soldierId);
                foreach (string soldierId in arrayOfOldRevealingList)
                {
                    arrayFinalRevealingList.Remove(soldierId);
                    arrayFinalRevealingList.Add(soldierId);
                }
                foreach (string soldierId in arrayOfNotRevealingList)
                    arrayFinalRevealingList.Remove(soldierId);

                //print(IdToName(keyValuePair.Key) + " : " + PrintList(arrayOfRevealingList) + " + " + PrintList(arrayOfOldRevealingList) + " - " + PrintList(arrayOfNotRevealingList) + " = " + PrintList(arrayFinalRevealingList));

                foreach (string soldierId in arrayFinalRevealingList)
                    allSoldiersRevealingFinal[keyValuePair.Key].Add(soldierId);
            }

            //populate revealedby list for each soldier
            foreach (KeyValuePair<string, List<string>> keyValuePair in allSoldiersRevealingFinal)
            {
                if (keyValuePair.Value.Any())
                {
                    List<string> revealingNames = keyValuePair.Value;
                    foreach (string soldierId in revealingNames)
                        allSoldiersRevealedBy[soldierId].Add(keyValuePair.Key);
                }
            }

            //repopulate each soldier's revealing and revealedby lists with all reveals
            foreach (Soldier s in game.AllSoldiers())
            {
                s.RevealingSoldiers = allSoldiersRevealingFinal.GetValueOrDefault(s.id);
                s.RevealedBySoldiers = allSoldiersRevealedBy.GetValueOrDefault(s.id);
            }

            //only close the detectionUI if it's open
            if (detectionUI.activeInHierarchy)
            {
                //destroy all detection alerts after done
                foreach (Transform child in detectionAlert)
                    Destroy(child.gameObject);

                CloseDetectionUI();
            }
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }
    public void CloseDetectionUI()
    {
        detectionResolvedFlag = true;
        UnfreezeTime();
        detectionUI.SetActive(false);
    }
    public void AddLostLosAlert(Soldier soldier)
    {
        //block duplicate lostlos alerts being created
        foreach (Transform child in lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if (child.GetComponent<SoldierAlert>().soldier == soldier && child.Find("LostLosTitle") != null)
                Destroy(child.gameObject);

        GameObject lostLosAlert = Instantiate(lostLosAlertPrefab, lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        lostLosAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
        lostLosAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
        lostLosAlert.transform.Find("LostLosDescription").GetComponent<TextMeshProUGUI>().text = soldier.soldierName + " is now hidden, remove him from the board.";
    }
    public void AddLosGlimpseAlert(Soldier soldier, string description)
    {
        GameObject losGlimpseAlert = Instantiate(losGlimpseAlertPrefab, lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        losGlimpseAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
        losGlimpseAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
        //losGlimpseAlert.transform.Find("LosGlimpseTitle").GetComponent<TextMeshProUGUI>().text = ;
        losGlimpseAlert.transform.Find("LosGlimpseDescription").GetComponent<TextMeshProUGUI>().text = $"{soldier.soldierName} was glimpsed {description}.";
    }
    public IEnumerator OpenLostLOSList()
    {
        //print("OpenLostLosList(start)");
        //yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => meleeResolvedFlag == true && overrideView == false);
        //print("OpenLostLosList(passedmeleeflag)");
        bool display = false;
        foreach (Transform child in lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
        {
            //destroy false alerts
            if (child.GetComponent<SoldierAlert>().soldier.RevealedBySoldiers.Any())
                Destroy(child.gameObject);
            else if (child.GetComponent<SoldierAlert>().soldier.IsOnturnAndAlive())
                child.gameObject.SetActive(false);
            else
            {
                child.gameObject.SetActive(true);
                display = true;
            }
        }   

        if (display)
            lostLosUI.SetActive(true);
    }
    public void ConfirmLostLosList()
    {
        ScrollRect lostLosScroller = lostLosUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        if (lostLosScroller.verticalNormalizedPosition <= 0.05f)
            CloseLostLOSList();
        else
            print("Haven't scrolled all the way to the bottom");
    }

    public void ClearLostLOSList()
    {
        foreach (Transform child in lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if (child.GetComponent<SoldierAlert>().soldier.IsOffturn())
                Destroy(child.gameObject);
    }
    public void CloseLostLOSList()
    {
        ClearLostLOSList();
        lostLosUI.SetActive(false);
    }



    //damage ui functions
    public void AddDamageAlert(Soldier soldier, string description, bool resisted, bool nonDamage)
    {
        GameObject damageAlert = Instantiate(damageAlertPrefab, damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        if (resisted)
        {
            if (nonDamage)
                damageAlert.transform.Find("DamageTitle").GetComponent<TextMeshProUGUI>().text = "<color=green>EFFECT RESISTED</color>";
            else
                damageAlert.transform.Find("DamageTitle").GetComponent<TextMeshProUGUI>().text = "<color=green>DAMAGE RESISTED</color>";
        }
        else
        {
            if (nonDamage)
                damageAlert.transform.Find("DamageTitle").GetComponent<TextMeshProUGUI>().text = "<color=red>EFFECT SUFFERED</color>";
            else
                damageAlert.transform.Find("DamageTitle").GetComponent<TextMeshProUGUI>().text = "<color=red>DAMAGE SUFFERED</color>";
        }

        damageAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
        damageAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
        damageAlert.transform.Find("DamageDescription").GetComponent<TextMeshProUGUI>().text = description;

        //try and open damagealert
        StartCoroutine(OpenDamageList());
    }
    public IEnumerator OpenDamageList()
    {
        //print("OpenLostLosList(start)");
        //yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => shotResolvedFlag == true && meleeResolvedFlag == true && overrideView == false);
        //print("OpenLostLosList(passedmeleeflag)");
        bool display = false;
        foreach (Transform child in damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
        {
            if (child.GetComponent<SoldierAlert>().soldier.IsOffturn())
                child.gameObject.SetActive(false);
            else
            {
                child.gameObject.SetActive(true);
                display = true;
            }
        }

        if (display)
            damageUI.SetActive(true);
    }
    public void ConfirmDamageList()
    {
        ScrollRect lostLosScroller = damageUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        if (lostLosScroller.verticalNormalizedPosition <= 0.05f)
        {
            CloseDamageList();
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }

    public void ClearDamageList()
    {
        foreach (Transform child in damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if (child.GetComponent<SoldierAlert>().soldier.IsOnturn())
                Destroy(child.gameObject); 
    }
    public void CloseDamageList()
    {
        ClearDamageList();
        damageUI.SetActive(false);
    }








    //inspirer functions
    public void OpenInspirerUI()
    {
        inspirerUI.SetActive(true);
    }
    public void AddInspirerAlert(Soldier friendly, string reason)
    {
        GameObject inspirerAlert = Instantiate(inspirerAlertPrefab, inspirerUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));
        inspirerAlert.GetComponent<SoldierAlert>().SetSoldier(friendly);

        string inspirerTitle = "Potential Inspiration: ";
        inspirerTitle += friendly.soldierSpeciality switch
        {
            "Leadership" => "+1 L",
            "Health" => "+1 H",
            "Resilience" => "+1 R",
            "Speed" => "+6 S",
            "Evasion" => "+1 E",
            "Stealth" => "+1 F",
            "Perceptiveness" => "+1 P",
            "Camouflage" => "+1 C",
            "Sight Radius" => "+10 SR",
            "Rifle" => "+5% Ri Aim",
            "Assault Rifle" => "+5% AR Aim",
            "Light Machine Gun" => "+5% LMG Aim",
            "Sniper Rifle" => "+5% Sn Aim",
            "Sub-Machine Gun" => "+5% SMG Aim",
            "Shotgun" => "+5% Sh Aim",
            "Melee" => "+0.5 M",
            _ => "+1 AP",
        };

        inspirerAlert.transform.Find("InspirerTitle").GetComponent<TextMeshProUGUI>().text = inspirerTitle;
        inspirerAlert.transform.Find("InspirerDescription").GetComponent<TextMeshProUGUI>().text = reason;
        inspirerAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(friendly);
    }
    public void CloseInspirerUI()
    {
        SetInspirerResolvedFlagTo(true);
        inspirerUI.SetActive(false);
    }







    //trauma functions - menu
    public void OpenTraumaUI()
    {
        if (OverrideKey())
        {
            traumaUI.SetActive(true);
            CloseTraumaAlertUI();
        }
    }

    public void CloseTraumaUI()
    {
        traumaUI.SetActive(false);
    }
    public void AddTraumaAlert(Soldier friendly, int trauma, string reason, int rolls, int xpOnResist, string range)
    {
        GameObject traumaAlert = Instantiate(traumaAlertPrefab, traumaUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));
        traumaAlert.GetComponent<SoldierAlert>().SetSoldier(friendly);

        traumaAlert.transform.Find("TraumaIndicator").GetComponent<TextMeshProUGUI>().text = $"{trauma}";
        traumaAlert.transform.Find("TraumaDescription").GetComponent<TextMeshProUGUI>().text = reason;
        traumaAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(friendly);
        traumaAlert.transform.Find("Rolls").GetComponent<TextMeshProUGUI>().text = rolls.ToString();
        traumaAlert.transform.Find("XpOnResist").GetComponent<TextMeshProUGUI>().text = xpOnResist.ToString();
        traumaAlert.transform.Find("Distance").GetComponent<TextMeshProUGUI>().text = range;

        //block invalid trauma alerts being created
        if (friendly.tp >= 5)
        {
            traumaAlert.transform.Find("TraumaToggle").GetComponent<Toggle>().interactable = false;
            traumaAlert.transform.Find("TraumaIndicator").gameObject.SetActive(false);
            traumaAlert.transform.Find("ConfirmButton").gameObject.SetActive(false);
            traumaAlert.transform.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text = "<color=blue>DESENSITISED</color>";
        }
        else
        {
            if (reason.Contains("Commander") || reason.Contains("Lastandicide"))
            {
                traumaAlert.transform.Find("TraumaToggle").GetComponent<Toggle>().isOn = true;
                traumaAlert.transform.Find("TraumaToggle").GetComponent<Toggle>().interactable = false;
            }
            else if (friendly.IsResilient())
            {
                traumaAlert.transform.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text = "<color=green>TRAUMA RESISTED</color>";
                traumaAlert.transform.Find("TraumaIndicator").GetComponent<TextMeshProUGUI>().text = $"<color=green>{trauma}</color>";
            }
        }

        
    }

    public void OpenTraumaAlertUI()
    {
        traumaAlertUI.SetActive(true);
    }
    public void CloseTraumaAlertUI()
    {
        traumaAlertUI.SetActive(false);
    }
    public void OpenBrokenFledUI()
    {
        FreezeTime();
        brokenFledUI.SetActive(true);
    }

    public void CloseBrokenFledUI()
    {
        UnfreezeTime();
        brokenFledUI.SetActive(false);
    }













    //explosion functions
    public void OpenExplosionUI()
    {
        explosionUI.SetActive(true);
    }

    public void CloseExplosionUI()
    {
        explosionUI.SetActive(false);
    }
    public void AddExplosionAlert(Soldier hitByExplosion, Soldier explodedBy, int damage)
    {
        GameObject explosionAlert = Instantiate(explosionAlertPrefab, explosionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));
        explosionAlert.transform.Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text = $"{damage}";
        explosionAlert.GetComponent<SoldierAlertDouble>().SetSoldiers(hitByExplosion, explodedBy);

        explosionAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(hitByExplosion);
    }






    //shot functions - menu
    public void OpenShotUI()
    {
        //set shooter details
        Soldier shooter = activeSoldier;
        shotUI.transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text = shooter.id;

        TMP_Dropdown shotTypeDropdown = shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>();
        shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        Image gunImage = shotUI.transform.Find("Gun").Find("GunImage").GetComponent<Image>();
        TMP_Dropdown aimDropdown = shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>();
        shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("InvalidLocation").gameObject.SetActive(false);

        //generate gun image
        gunImage.sprite = shooter.EquippedGun.itemImage;

        //block suppression option if gun does not have enough ammo
        if (!shooter.EquippedGun.CheckSpecificAmmo(shooter.EquippedGun.gunTraits.SuppressDrain, true))
            shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression Shot");

        //if soldier engaged in melee block force unaimed shot
        if (shooter.IsMeleeEngaged())
            aimDropdown.value = 1;
        else
            aimDropdown.value = 0;

        BlockShotOptions();
        game.UpdateShotType(shooter);
        game.UpdateShotUI(shooter);

        shotUI.SetActive(true);
    }
    public void BlockShotOptions()
    {
        if (activeSoldier.IsMeleeEngaged())
        {
            shotUI.transform.Find("BackButton").GetComponent<Button>().interactable = true;
            shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>().interactable = false;
            shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>().interactable = false;
            shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().interactable = true;
            shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>().interactable = false;
        }
        else
        {
            shotUI.transform.Find("BackButton").GetComponent<Button>().interactable = true;
            shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>().interactable = true;
            shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>().interactable = true;
            shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().interactable = true;
            shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>().interactable = true;
        }
    }
    public IEnumerator OpenOverwatchShotUI()
    {
        yield return new WaitForSeconds(0.05f);
        overwatchShotUI.SetActive(true);
    }

    public IEnumerator CreateOverwatchShotUI(Soldier shooter, Soldier target)
    {
        yield return new WaitForSeconds(0.05f);
        bool shotAlready = false;

        //check if the overwatch has not been taken already
        foreach (Transform child in overwatchShotUI.transform)
            if (child.Find("Shooter").GetComponent<TextMeshProUGUI>().text == shooter.id)
                shotAlready = true;

        //only proceed if it doesn't exist already
        if (!shotAlready)
        {
            GameObject overwatchUI = Instantiate(overwatchShotUIPrefab, overwatchShotUI.transform);

            //set shooter
            overwatchUI.transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text = shooter.id;

            TMP_Dropdown shotTypeDropdown = overwatchUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>();
            shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
            Image gunImage = overwatchUI.transform.Find("Gun").Find("GunImage").GetComponent<Image>();
            TMP_Dropdown aimDropdown = overwatchUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>();
            TMP_Dropdown targetDropdown = overwatchUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
            TMP_Dropdown coverDropdown = overwatchUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>();
            List<TMP_Dropdown.OptionData> targetDetails = new();

            //display shooter
            if (shooter.IsBeingRevealedBy(target.id))
            {
                overwatchUI.transform.Find("ShooterPortrait").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(shooter);
                overwatchUI.transform.Find("ShooterPortrait").gameObject.SetActive(true);
            }
            else
                overwatchUI.transform.Find("ShooterPortrait").gameObject.SetActive(false);

            //set as regular shot
            shotTypeDropdown.value = 0;
            shotTypeDropdown.interactable = false;

            //set as aimed shot
            aimDropdown.value = 0;
            aimDropdown.interactable = false;

            //determine if in cover
            if (target.IsInCover())
                overwatchUI.transform.Find("TargetPanel").Find("CoverLevel").gameObject.SetActive(true);
            else
                overwatchUI.transform.Find("TargetPanel").Find("CoverLevel").gameObject.SetActive(false);
            coverDropdown.value = 0;

            //generate gun image
            gunImage.sprite = shooter.EquippedGun.itemImage;

            //set target
            TMP_Dropdown.OptionData option = new(target.soldierName, target.soldierPortrait);
            targetDetails.Add(option);
            targetDropdown.AddOptions(targetDetails);
            targetDropdown.interactable = false;

            overwatchUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "0";

            //prefill with target's current position
            overwatchUI.transform.Find("TargetPanel").Find("OverwatchLocation").Find("XPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = target.X.ToString();
            overwatchUI.transform.Find("TargetPanel").Find("OverwatchLocation").Find("YPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = target.Y.ToString();
            overwatchUI.transform.Find("TargetPanel").Find("OverwatchLocation").Find("ZPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = target.Z.ToString();
        }
    }
    public void GuardsmanOverwatchRetry()
    {
        Soldier shooter = game.tempShooterTarget.Item1;
        StartCoroutine(CreateOverwatchShotUI(shooter, game.tempShooterTarget.Item2 as Soldier));
        StartCoroutine(OpenOverwatchShotUI());
    }
    public void OpenShotResultUI()
    {
        shotResultUI.SetActive(true);
    }
    public void CloseShotResultUI()
    {
        SetShotResolvedFlagTo(true);
        shotResultUI.SetActive(false);
    }
    public void ClearShotUI()
    {
        clearShotFlag = true;
        shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("Gun").Find("GunImage").GetComponent<Image>().sprite = null;
        shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("XPos").GetComponent<TMP_InputField>().text = "";
        shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("YPos").GetComponent<TMP_InputField>().text = "";
        shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        ClearFlankersUI(flankersShotUI);
        clearShotFlag = false;
    }

    public void CloseShotUI()
    {
        ClearShotUI();
        ClearShotConfirmUI();
        shotUI.SetActive(false);
        shotConfirmUI.SetActive(false);
    }
    public void OpenShotConfirmUI()
    {
        if (int.TryParse(shotUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap))
        {
            if (game.CheckAP(ap))
            {
                Soldier shooter = soldierManager.FindSoldierById(shotUI.transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text);
                IAmShootable target = game.FindShootableById(game.targetDropdown.options[game.targetDropdown.value].text);
                Item gun = shooter.EquippedGun;
                Tuple<int, int, int> chances = null;
                bool validDetails = true;

                //check cover location is valid
                if (target is Coverman targetCover)
                    if (!shooter.PhysicalObjectIsRevealed(targetCover))
                        validDetails = false;

                if (validDetails)
                {
                    if (game.shotTypeDropdown.value == 1)
                        chances = Tuple.Create(100, 0, 100);
                    else
                        chances = game.CalculateHitPercentage(shooter, target, gun);

                    //only continue if shot is valid
                    if (chances != null)
                    {
                        //only shot suppression hit chance if suppressed
                        if (shooter.IsSuppressed() && game.shotTypeDropdown.value != 1)
                            shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(true);
                        else
                            shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(false);

                        shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").Find("SuppressedHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item3.ToString() + "%";
                        shotConfirmUI.transform.Find("OptionPanel").Find("HitChance").Find("HitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item1.ToString() + "%";
                        shotConfirmUI.transform.Find("OptionPanel").Find("CritHitChance").Find("CritHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item2.ToString() + "%";

                        //enable back button only if shot is aimed and under 25%
                        if (game.aimTypeDropdown.value != 1 && chances.Item1 <= 2)
                            shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = true;
                        else
                            shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = false;

                        //add parameter to equation view
                        shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text =
                            $"{game.shotParameters.Find(tuple => tuple.Item1 == "accuracy")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "sharpshooter")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "inspired")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "WS")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "juggernaut")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "stim")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "trauma")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "sustenance")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "tE")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "cover")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "vis")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "rain")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "wind")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "HP")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "tHP")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "Ter")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "tTer")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "elevation")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "kd")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "overwatch")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "flank")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "stealth")} " +
                            $"| {game.shotParameters.Find(tuple => tuple.Item1 == "suppression")}";

                        shotConfirmUI.SetActive(true);
                    }
                }
            }
        }
    }
    public void ExitShotConfirmUI()
    {
        int.TryParse(shotUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap);
        //deduct ap for aiming if leaving shot
        game.DeductAP(ap - 1);

        CloseShotUI();
    }
    public void ClearShotConfirmUI()
    {
        //if (shotConfirmUI.activeInHierarchy)
        {
            clearShotFlag = true;
            shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").Find("SuppressedHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("HitChance").Find("HitChanceDisplay").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("CritHitChance").Find("CritHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = "";
            clearShotFlag = false;
        }       
    }







    //cross functional shot functions melee functions
    public void OpenFlankersUI(GameObject flankersUI)
    {
        flankersUI.SetActive(true);
    }










    //melee functions - menu
    public void OpenMeleeUINonCoroutine(string meleeCharge)
    {
        StartCoroutine(OpenMeleeUI(meleeCharge));
    }
    public IEnumerator OpenMeleeUI(string meleeCharge)
    {
        yield return new WaitForSeconds(0.05f);
        
        //set attacker
        Soldier attacker = activeSoldier;
        meleeUI.transform.Find("Attacker").GetComponent<TextMeshProUGUI>().text = attacker.id;

        meleeChargeIndicator = meleeCharge;
        TMP_Dropdown meleeTypeDropdown = meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>();
        meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        Image attackerWeaponImage = meleeUI.transform.Find("AttackerWeapon").Find("WeaponImage").GetComponent<Image>();
        Image defenderWeaponImage = meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").Find("WeaponImage").GetComponent<Image>();
        TMP_Dropdown targetDropdown = meleeUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        List<TMP_Dropdown.OptionData> defenderDetails = new();

        //generate melee type dropdown
        List<TMP_Dropdown.OptionData> meleeTypeDetails = new()
        {
            new TMP_Dropdown.OptionData(meleeCharge),
            new TMP_Dropdown.OptionData("Engagement Only"),
        };
        meleeTypeDropdown.AddOptions(meleeTypeDetails);

        //block engagement only if melee controlled
        if (attacker.IsMeleeEngaged())
            meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Engagement Only");

        //display best attacker weapon
        if (attacker.BestMeleeWeapon != null)
            attackerWeaponImage.sprite = attacker.BestMeleeWeapon.itemImage;
        else
            attackerWeaponImage.sprite = fist;



        //generate target list
        foreach (Soldier s in game.AllSoldiers())
        {
            TMP_Dropdown.OptionData defender = null;
            if (s.IsAlive() && attacker.IsOppositeTeamAs(s) && s.IsRevealed() && attacker.PhysicalObjectWithinMeleeRadius(s))
            {
                if (attacker.CanSeeInOwnRight(s))
                    defender = new(s.soldierName, s.soldierPortrait);
                else
                    defender = new(s.soldierName, s.LoadPortraitTeamsight(s.soldierPortraitText));

                defenderDetails.Add(defender);
            }

            if (defender != null)
            {
                //remove option if soldier is engaged and this soldier is not on the engagement list
                if (attacker.IsMeleeEngaged() && !attacker.IsMeleeEngagedWith(s))
                    defenderDetails.Remove(defender);
            }
        }

        if (defenderDetails.Count > 0)
        {
            targetDropdown.AddOptions(defenderDetails);

            Soldier defender = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);

            if (defender.controlledBySoldiersList.Contains(activeSoldier.id))
                meleeTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("<color=green>Disengage</color>") });
            else if (defender.controllingSoldiersList.Contains(activeSoldier.id))
                meleeTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("<color=red>Request Disengage</color>") });

            //show defender weapon
            if (defender.BestMeleeWeapon != null)
                defenderWeaponImage.sprite = defender.BestMeleeWeapon.itemImage;
            else
                defenderWeaponImage.sprite = fist;

            CheckMeleeType();
            game.UpdateMeleeUI();

            meleeUI.SetActive(true);
        }
        else
        {
            ClearMeleeUI();
            OpenNoMeleeTargetsUI();
        }
    }
    public void ClearFlankersUI(GameObject flankersUI)
    {
        flankersUI.SetActive(false);
        foreach (Transform child in flankersUI.transform.Find("FlankersPanel"))
            Destroy(child.gameObject);
    }
    public void CheckMeleeType()
    {
        TMP_Dropdown meleeTypeDropdown = meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>();
        meleeUI.transform.Find("AttackerWeapon").gameObject.SetActive(false);
        meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").gameObject.SetActive(false);
        flankersMeleeAttackerUI.SetActive(false);
        flankersMeleeDefenderUI.SetActive(false);

        if (meleeTypeDropdown.value == 0)
        {
            meleeUI.transform.Find("AttackerWeapon").gameObject.SetActive(true);
            meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").gameObject.SetActive(true);
            flankersMeleeAttackerUI.SetActive(true);
            flankersMeleeDefenderUI.SetActive(true);
        }

        if (meleeTypeDropdown.options[meleeTypeDropdown.value].text.Contains("Request"))
            OpenMeleeBreakEngagementRequestUI();
    }
    public void ClearMeleeUI()
    {
        clearMeleeFlag = true;
        meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        meleeUI.transform.Find("AttackerWeapon").Find("WeaponImage").GetComponent<Image>().sprite = null;
        meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").Find("WeaponImage").GetComponent<Image>().sprite = null;
        meleeUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        meleeUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().value = 0;
        ClearFlankersUI(flankersMeleeAttackerUI);
        ClearFlankersUI(flankersMeleeDefenderUI);
        clearMeleeFlag = false;
    }
    public void CloseMeleeUI()
    {
        ClearMeleeUI();
        ClearMeleeConfirmUI();
        meleeUI.SetActive(false);
        meleeConfirmUI.SetActive(false);
    }
    public void OpenMeleeConfirmUI()
    {
        if (int.TryParse(meleeUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap))
        {
            if (game.CheckAP(ap))
            {
                //find attacker and defender
                Soldier attacker = soldierManager.FindSoldierById(meleeUI.transform.Find("Attacker").GetComponent<TextMeshProUGUI>().text);
                Soldier defender = soldierManager.FindSoldierByName(game.meleeTargetDropdown.options[game.meleeTargetDropdown.value].text);

                int meleeDamage = game.CalculateMeleeResult(attacker, defender);

                meleeConfirmUI.transform.Find("OptionPanel").Find("Damage").Find("DamageDisplay").GetComponent<TextMeshProUGUI>().text = meleeDamage.ToString();

                //add parameters to equation view
                meleeConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text =
                    $"{game.meleeParameters.Find(tuple => tuple.Item1 == "aM")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aJuggernaut")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aInspirer")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aSustenance")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aWep")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aHP")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aTer")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aFlank")} " +
                    $"| {game.shotParameters.Find(tuple => tuple.Item1 == "kd")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "suppression")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "aStr")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dM")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dJuggernaut")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dInspirer")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dSustenance")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dWep")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "charge")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dHP")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dTer")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dFlank")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "suppression")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dStr")} " +
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "bloodrage")}";

                //add rounding to equation view
                meleeConfirmUI.transform.Find("EquationPanel").Find("Rounding").GetComponent<TextMeshProUGUI>().text = $"Rounding: {game.meleeParameters.Find(tuple => tuple.Item1 == "rounding").Item2}";

                meleeConfirmUI.SetActive(true);
            }
        }
    }
    public void ClearMeleeConfirmUI()
    {
        //if (shotConfirmUI.activeInHierarchy)
        {
            clearMeleeFlag = true;
            meleeConfirmUI.transform.Find("OptionPanel").Find("Damage").Find("DamageDisplay").GetComponent<TextMeshProUGUI>().text = "";
            clearMeleeFlag = false;
        }
    }
    public void OpenNoMeleeTargetsUI()
    {
        noMeleeTargetsUI.SetActive(true);
    }
    public void CloseNoMeleeTargetsUI()
    {
        noMeleeTargetsUI.SetActive(false);
    }
    public void AddMeleeAlert(Soldier attacker, Soldier defender, string damageResult, string controlResult)
    {
        //block duplicate detection alerts being created, stops override mode creating multiple instances during overwriting detection stats
        foreach (Transform child in meleeResultUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if ((child.GetComponent<SoldierAlertDouble>().s1 == attacker && child.GetComponent<SoldierAlertDouble>().s2 == defender) || (child.GetComponent<SoldierAlertDouble>().s1 == defender && child.GetComponent<SoldierAlertDouble>().s2 == attacker))
                Destroy(child.gameObject);

        GameObject meleeAlert = Instantiate(meleeAlertPrefab, meleeResultUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        meleeAlert.GetComponent<SoldierAlertDouble>().SetSoldiers(attacker, defender);
        meleeAlert.transform.Find("Results").Find("DamageResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = damageResult;
        meleeAlert.transform.Find("Results").Find("ControlResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = controlResult;
        meleeAlert.transform.Find("Attacker").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(attacker);
        meleeAlert.transform.Find("Defender").Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(defender);
    }
    public void OpenMeleeResultUI()
    {
        meleeResultUI.SetActive(true);
    }
    public void ConfirmMeleeResult()
    {
        ScrollRect meleeScroller = meleeResultUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        Transform meleeAlertList = meleeResultUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
        if (meleeScroller.verticalNormalizedPosition <= 0.05f)
        {
            //destroy all detection alerts after done
            foreach (Transform child in meleeAlertList)
                Destroy(child.gameObject);

            CloseMeleeResultUI();
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }
    public void CloseMeleeResultUI()
    {
        SetMeleeResolvedFlagTo(true);
        meleeResultUI.SetActive(false);
    }
    public void OpenMeleeBreakEngagementRequestUI()
    {
        meleeBreakEngagementRequestUI.SetActive(true);
    }
    public void DenyBreakEngagementRequest()
    {
        meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        game.UpdateMeleeUI();
        CloseMeleeBreakEngagementRequestUI();
    }
    public void AcceptBreakEngagementRequest()
    {
        if (OverrideKey())
        {
            game.UpdateMeleeUI();
            CloseMeleeBreakEngagementRequestUI();
        }
    }
    public void CloseMeleeBreakEngagementRequestUI()
    {
        meleeBreakEngagementRequestUI.SetActive(false);
    }



    //move functions - menu
    public void ShowClosestAlly()
    {
        GameObject closestAllyUI = moveUI.transform.Find("ClosestAlly").gameObject;
        ClearClosestAllyUI(closestAllyUI);

        Soldier closestAlly = activeSoldier.ClosestAllyMobile();

        if (closestAlly != null)
        {
            GameObject closestAllyPortrait = Instantiate(soldierPortraitPrefab, closestAllyUI.transform.Find("ClosestAllyPanel"));
            closestAllyPortrait.GetComponent<SoldierPortrait>().Init(closestAlly);
        }
    }
    public void ClearClosestAllyUI(GameObject closestAllyUI)
    {
        closestAllyUI.SetActive(false);
        foreach (Transform child in closestAllyUI.transform.Find("ClosestAllyPanel"))
            Destroy(child.gameObject);
    }
    public void CheckMoveType()
    {
        TMP_Dropdown moveTypeDropdown = moveUI.transform.Find("MoveType").Find("MoveTypeDropdown").GetComponent<TMP_Dropdown>();

        if (moveTypeDropdown.options[moveTypeDropdown.value].text.Contains("Planner"))
        {
            moveUI.transform.Find("Location").gameObject.SetActive(false);
            moveUI.transform.Find("Terrain").gameObject.SetActive(false);
            moveUI.transform.Find("Cover").gameObject.SetActive(false);
            moveUI.transform.Find("Melee").gameObject.SetActive(false);
            moveUI.transform.Find("Fall").gameObject.SetActive(false);
            ShowClosestAlly();
            moveUI.transform.Find("ClosestAlly").gameObject.SetActive(true);
            moveUI.transform.Find("MoveDonated").gameObject.SetActive(true);
        }
        else if (moveTypeDropdown.options[moveTypeDropdown.value].text.Contains("Exo"))
        {
            moveUI.transform.Find("Location").gameObject.SetActive(true);
            moveUI.transform.Find("Terrain").gameObject.SetActive(true);
            moveUI.transform.Find("Cover").gameObject.SetActive(true);
            moveUI.transform.Find("Melee").gameObject.SetActive(true);
            moveUI.transform.Find("Fall").gameObject.SetActive(false);
            moveUI.transform.Find("ClosestAlly").gameObject.SetActive(false);
            moveUI.transform.Find("MoveDonated").gameObject.SetActive(false);
        }
        else
        {
            moveUI.transform.Find("Location").gameObject.SetActive(true);
            moveUI.transform.Find("Terrain").gameObject.SetActive(true);
            moveUI.transform.Find("Cover").gameObject.SetActive(true);
            moveUI.transform.Find("Melee").gameObject.SetActive(true);
            moveUI.transform.Find("Fall").gameObject.SetActive(true);
            moveUI.transform.Find("ClosestAlly").gameObject.SetActive(false);
            moveUI.transform.Find("MoveDonated").gameObject.SetActive(false);
        }
    }
    public void OpenMoveUI(bool suppressed)
    {
        TMP_Dropdown moveTypeDropdown = moveUI.transform.Find("MoveType").Find("MoveTypeDropdown").GetComponent<TMP_Dropdown>();
        Toggle coverToggle = moveUI.transform.Find("Cover").Find("CoverToggle").GetComponent<Toggle>();
        Toggle meleeToggle = moveUI.transform.Find("Melee").Find("MeleeToggle").GetComponent<Toggle>();
        GameObject backButton = moveUI.transform.Find("BackButton").gameObject;

        List<TMP_Dropdown.OptionData> moveTypeDetails;
        moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        moveTypeDropdown.ClearOptions();
        moveTypeDropdown.interactable = true;
        coverToggle.interactable = true;
        meleeToggle.interactable = true;

        //add suppression indicators if suppressed
        string fullMoveSuppressed = "", halfMoveSuppressed = "";
        if (activeSoldier.IsSuppressed())
        {
            fullMoveSuppressed = " <color=orange>(" + activeSoldier.FullMoveSuppressed + ")</color>";
            halfMoveSuppressed = " <color=orange>(" + activeSoldier.HalfMoveSuppressed + ")</color>";
        }

        //generate move type dropdown
        if (suppressed)
        {
            moveTypeDetails = new() 
            {
                new TMP_Dropdown.OptionData("Full: " + fullMoveSuppressed),
                new TMP_Dropdown.OptionData("Half: " + halfMoveSuppressed),
                new TMP_Dropdown.OptionData("Tile: " + activeSoldier.TileMove),
            };
            moveTypeDropdown.interactable = false;
            coverToggle.interactable = false;
            meleeToggle.interactable = false;
            backButton.SetActive(false);
        }
        else
        {
            moveTypeDetails = new()
            {
                new TMP_Dropdown.OptionData("Full: " + activeSoldier.FullMove + fullMoveSuppressed),
                new TMP_Dropdown.OptionData("Half: " + activeSoldier.HalfMove + halfMoveSuppressed),
                new TMP_Dropdown.OptionData("Tile: " + activeSoldier.TileMove),
            };
            backButton.SetActive(true);
        }
        
        //add extra move options for planner/exo
        if (activeSoldier.IsPlanner() && activeSoldier.ClosestAllyMobile() != null && !activeSoldier.usedMP)
            moveTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Planner Donation</color>"));
        if (activeSoldier.IsWearingExoArmour())
            moveTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Exo Jump</color>"));
        moveTypeDropdown.AddOptions(moveTypeDetails);

        //grey options according to AP
        if (activeSoldier.ap < 3)
        {
            moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("0");
            moveTypeDropdown.value = 1;
        }
        if (activeSoldier.ap < 2)
        {
            moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("1");
            moveTypeDropdown.value = 2;
        }

        //block cover for JA
        if (activeSoldier.IsWearingJuggernautArmour(false))
            coverToggle.interactable = false;

        //block melee toggle if within engage distance of enemy
        if (activeSoldier.ClosestEnemyVisible() != null && activeSoldier.PhysicalObjectWithinMeleeRadius(activeSoldier.ClosestEnemyVisible()) || suppressed)
            meleeToggle.interactable = false;

        /*//block planner if already moved
        if (!activeSoldier.usedMP)
            moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Planner Donate");
        else
            moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();*/

        //prefill movement position inputs with current position
        moveUI.transform.Find("Location").Find("XPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
        moveUI.transform.Find("Location").Find("YPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
        moveUI.transform.Find("Location").Find("ZPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();
        
        //block options and show start location if broken
        if (activeSoldier.IsBroken())
        {
            moveUI.transform.Find("Location").Find("StartLocation").Find("StartX").GetComponent<TextMeshProUGUI>().text = activeSoldier.startX.ToString();
            moveUI.transform.Find("Location").Find("StartLocation").Find("StartY").GetComponent<TextMeshProUGUI>().text = activeSoldier.startY.ToString();
            moveUI.transform.Find("Location").Find("StartLocation").Find("StartZ").GetComponent<TextMeshProUGUI>().text = activeSoldier.startZ.ToString();
            coverToggle.interactable = false;
            meleeToggle.interactable = false;
            moveUI.transform.Find("Location").Find("StartLocation").gameObject.SetActive(true);
        }
        else
            moveUI.transform.Find("Location").Find("StartLocation").gameObject.SetActive(false);

        game.UpdateMoveUI();
        
        moveUI.SetActive(true);
    }
    public void OpenMoveToSameSpotUI()
    {
        moveToSameSpotUI.SetActive(true);
    }
    public void CloseMoveToSameSpotUI()
    {
        moveToSameSpotUI.SetActive(false);
    }
    public void OpenOvermoveUI(string message)
    {
        soundManager.PlayOvermoveAlarm();
        overmoveUI.transform.Find("Warning").GetComponent<TextMeshProUGUI>().text = message;
        overmoveUI.SetActive(true);
    }
    public void CloseOvermoveUI()
    {
        overmoveUI.SetActive(false);
    }
    public void OpenSuppressionMoveUI()
    {
        soundManager.PlayOvermoveAlarm();
        suppressionMoveUI.SetActive(true);
    }
    public void CloseSuppressionMoveUI()
    {
        suppressionMoveUI.SetActive(false);
    }
    public void ClearMoveUI()
    {
        clearMoveFlag = true;
        moveUI.transform.Find("MoveType").Find("MoveTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        moveUI.transform.Find("Location").Find("XPos").GetComponent<TMP_InputField>().text = "";
        moveUI.transform.Find("Location").Find("YPos").GetComponent<TMP_InputField>().text = "";
        moveUI.transform.Find("Location").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        moveUI.transform.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value = 0;
        moveUI.transform.Find("Cover").Find("CoverToggle").GetComponent<Toggle>().isOn = false;
        moveUI.transform.Find("Melee").Find("MeleeToggle").GetComponent<Toggle>().isOn = false;
        moveUI.transform.Find("Fall").Find("FallInputZ").GetComponent<TMP_InputField>().text = "";
        clearMoveFlag = false;
    }
    public void CloseMoveUI()
    {
        ClearMoveUI();
        moveUI.SetActive(false);
        overmoveUI.SetActive(false);
    }
    public void ConfirmOvermove()
    {
        if (OverrideKey())
        {
            game.ConfirmMove(true);
            CloseOvermoveUI();
        }
        else
            soundManager.PlayOverrideAlarm();
    }
    public void ConfirmSuppressionMove()
    {
        if (OverrideKey())
        {
            OpenMoveUI(true);
            CloseSuppressionMoveUI();
        }
        else
            soundManager.PlayOverrideAlarm();
    }










    //configure functions - menu
    public void AddGroundInventorySourceButton()
    {
        GameObject groundInventoryPanel = Instantiate(inventoryPanelGroundPrefab, configureUI.transform).GetComponent<InventorySourcePanel>().Init(null).gameObject;
        Instantiate(groundInventoryIconPrefab.GetComponent<InventorySourceIcon>().Init(groundInventoryPanel), inventorySourceIconsUI.transform);
        foreach (Item i in game.FindNearbyItems())
        {
            if (i.transform.parent == null)
            {
                ItemSlot itemSlot = Instantiate(itemSlotPrefab, groundInventoryPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>();
                itemSlot.AssignItemIcon(Instantiate(itemIconPrefab, itemSlot.transform).GetComponent<ItemIcon>().Init(i));
            }
        }
    }
    public void AddAllyInventorySourceButtons()
    {
        foreach (Soldier s in game.AllSoldiers())
            if (s.IsFielded() && activeSoldier.PhysicalObjectWithinItemRadius(s) && (activeSoldier.IsSameTeamAs(s) || s.IsUnconscious() || s.IsDead()) && s.IsInteractable())
                Instantiate(allyInventoryIconPrefab.GetComponent<InventorySourceIconAlly>().Init(s, Instantiate(inventoryPanelAllyPrefab, configureUI.transform).GetComponent<InventorySourcePanel>().Init(s).gameObject), inventorySourceIconsUI.transform);
    }
    public void AddPOIInventorySourceButtons()
    {
        foreach (GoodyBox gb in game.AllGoodyBoxes())
            if (activeSoldier.PhysicalObjectWithinItemRadius(gb))
                Instantiate(gbInventoryIconPrefab.GetComponent<InventorySourceIconGoodyBox>().Init(gb, Instantiate(inventoryPanelGoodyBoxPrefab, configureUI.transform).GetComponent<InventorySourcePanel>().Init(gb).gameObject), inventorySourceIconsUI.transform);
    }
    public void OpenConfigureUI()
    {
        //populate active soldier inventory
        configureUI.transform.Find("SoldierInventory").Find("SoldierLoadout").GetComponent<InventoryDisplayPanelSoldier>().Init(activeSoldier);

        //populate ground item icons
        AddGroundInventorySourceButton();
        
        //populate ally icons
        AddAllyInventorySourceButtons();
        
        //populate gb icons
        AddPOIInventorySourceButtons();

        configureUI.SetActive(true);
    }
    public void ClearConfigureUI()
    {
        //reset ap counter to 0
        configureUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "0";

        //clear all display panels
        foreach (Transform child in configureUI.transform.Find("ExternalItemSources").Find("InventorySourceIconPanel").Find("Viewport").Find("Contents"))
            Destroy(child.gameObject);

        //destroy all item source panels
        foreach (Transform child in configureUI.transform)
            if (child.GetComponent<InventorySourcePanel>() != null)
                Destroy(child.gameObject);
    }
    public void OpenInventoryPanel(GameObject itemPanel)
    {
        itemPanel.SetActive(true);

    }
    public void ClearInventoryPanel(GameObject itemPanel)
    {
        foreach (Transform child in itemPanel.transform.Find("Viewport").Find("Contents"))
            Destroy(child.gameObject);
    }
    public void CloseInventoryPanel(GameObject itemPanel)
    {
        itemPanel.SetActive(false);
    }
    public void CloseConfigureUI()
    {
        ClearConfigureUI();
        if (configureButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text == "Spawn Config")
            activeSoldier.usedAP = true;
        configureUI.SetActive(false);
    }











    //dipelec functions
    public void OpenDipElecUI()
    {
        Terminal terminal = activeSoldier.ClosestTerminal();
        dipelecUI.transform.Find("Terminal").GetComponent<TextMeshProUGUI>().text = terminal.id;
        TMP_Dropdown dipElecDropdown = dipelecUI.transform.Find("DipElecType").GetComponentInChildren<TMP_Dropdown>();
        dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        if (terminal.terminalType == "Dip Only")
            dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Hack");
        else if (terminal.terminalType == "Elec Only")
            dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Negotiation");

        if (terminal.SoldiersAlreadyHacked.Contains(activeSoldier.id))
            dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Hack");
        if (terminal.SoldiersAlreadyNegotiated.Contains(activeSoldier.id))
            dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Negotiation");

        if (dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Contains("Hack") && dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Contains("Negotiation"))
            dipElecDropdown.value = 2;
        else if (dipElecDropdown.GetComponent<DropdownController>().optionsToGrey.Contains("Negotiation"))
            dipElecDropdown.value = 1;

        dipelecUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "3";
        game.UpdateDipElecUI();
        dipelecUI.SetActive(true);
    }
    public void CloseDipElecUI()
    {
        ClearDipElecUI();
        dipelecUI.SetActive(false);
    }
    public void ClearDipElecUI()
    {
        clearDipelecFlag = true;
        dipelecUI.transform.Find("DipElecType").Find("DipElecTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        dipelecUI.transform.Find("Level").Find("LevelDropdown").GetComponent<TMP_Dropdown>().value = 0;
        dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = "";
        dipelecUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "";
        dipelecUI.transform.Find("SuccessChance").Find("SuccessChanceDisplay").GetComponent<TextMeshProUGUI>().text = "";
        foreach (Transform child in dipelecResultUI.transform.Find("RewardPanel").Find("Scroll").Find("View").Find("Content"))
            Destroy(child.gameObject);
        clearDipelecFlag = false;
    }
    public void OpenDipelecResultUI()
    {
        dipelecResultUI.SetActive(true);
    }
    public void CloseDipelecResultUI()
    {
        dipelecResultUI.SetActive(false);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    //cover functions/playdead functions/overwatch functions/lastandicide functions - menu
    public void OpenTakeCoverUI()
    {
        coverAlertUI.SetActive(true);
    }

    public void CloseTakeCoverUI()
    {
        coverAlertUI.SetActive(false);
    }
    public void OpenPlaydeadAlertUI()
    {
        playdeadAlertUI.SetActive(true);
    }
    public void ClosePlaydeadAlertUI()
    {
        playdeadAlertUI.SetActive(false);
    }
    public void OpenOverwatchUI()
    {
        
        overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Radius").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = $"Max {activeSoldier.SRColliderMax.radius}";
        overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Radius").GetComponent<InputController>().max = Mathf.RoundToInt(activeSoldier.SRColliderMax.radius);
        
        //allow guardsman to overwatch up to 180 degrees
        if (activeSoldier.IsGuardsman())
        {
            overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Angle").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = $"Max 180";
            overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Angle").GetComponent<InputController>().max = 180;
        }
        else
        {
            overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Angle").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = $"Max 90";
            overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Angle").GetComponent<InputController>().max = 90;
        }

        overwatchUI.SetActive(true);
    }
    public void ClearOverwatchUI()
    {
        overwatchUI.transform.Find("OptionPanel").Find("Location").Find("XPos").GetComponent<TMP_InputField>().text = "";
        overwatchUI.transform.Find("OptionPanel").Find("Location").Find("YPos").GetComponent<TMP_InputField>().text = "";
        overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Radius").GetComponent<TMP_InputField>().text = "";
        overwatchUI.transform.Find("OptionPanel").Find("Location").Find("Angle").GetComponent<TMP_InputField>().text = "";
    }
    public void CloseOverwatchUI()
    {
        ClearOverwatchUI();
        overwatchUI.SetActive(false);
    }
    public void OpenLastandicideConfirmUI()
    {
        lastandicideConfirmUI.SetActive(true);
    }
    public void CloseLastandicideConfirmUI()
    {
        lastandicideConfirmUI.SetActive(false);
    }







    //damage event functions - menu
    public void OpenDamageEventUI()
    {
        TMP_Dropdown damageEventTypeDropdown = damageEventUI.transform.Find("DamageEventType").Find("DamageEventTypeDropdown").GetComponent<TMP_Dropdown>();
        damageEventTypeDropdown.ClearOptions();

        //generate damage event type dropdown
        List<TMP_Dropdown.OptionData> damageEventTypeDetails = new()
        {
        new TMP_Dropdown.OptionData("Fall Damage"),
        new TMP_Dropdown.OptionData("Structural Collapse"),
        new TMP_Dropdown.OptionData("Other"),
        };
        if (activeSoldier.IsBloodletter() && !activeSoldier.bloodLettedThisTurn)
            damageEventTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Bloodletting</color>"));
        damageEventTypeDropdown.AddOptions(damageEventTypeDetails);

        //prefill movement position inputs with current position
        damageEventUI.transform.Find("Location").Find("XPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
        damageEventUI.transform.Find("Location").Find("YPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
        damageEventUI.transform.Find("Location").Find("ZPos").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();

        /*//block bloodletter if already bloodletted this turn
        if (activeSoldier.IsBloodletter() && activeSoldier.bloodLettedThisTurn)
            damageEventTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Bloodletting");
        else
            damageEventTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();*/

        damageEventUI.SetActive(true);
    }
    public void ClearDamageEventUI()
    {
        clearDamageEventFlag = true;
        damageEventUI.transform.Find("DamageEventType").Find("DamageEventTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        damageEventUI.transform.Find("FallDistance").Find("FallInputZ").GetComponent<TMP_InputField>().text = "";
        damageEventUI.transform.Find("StructureHeight").Find("StructureHeightInputZ").GetComponent<TMP_InputField>().text = "";
        damageEventUI.transform.Find("Other").Find("OtherInput").GetComponent<TMP_InputField>().text = "";
        damageEventUI.transform.Find("Location").Find("XPos").GetComponent<TMP_InputField>().text = "";
        damageEventUI.transform.Find("Location").Find("YPos").GetComponent<TMP_InputField>().text = "";
        damageEventUI.transform.Find("Location").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        damageEventUI.transform.Find("Location").Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value = 0;
        damageEventUI.transform.Find("DamageSource").Find("DamageSourceInput").GetComponent<TMP_InputField>().text = "";
        clearDamageEventFlag = false;
    }
    public void CloseDamageEventUI()
    {
        ClearDamageEventUI();
        damageEventUI.SetActive(false);
    }
    public void CheckDamageEventType()
    {
        TMP_Dropdown damageEventTypeDropdown = damageEventUI.transform.Find("DamageEventType").Find("DamageEventTypeDropdown").GetComponent<TMP_Dropdown>();

        if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Fall"))
        {
            damageEventUI.transform.Find("FallDistance").gameObject.SetActive(true);
            damageEventUI.transform.Find("StructureHeight").gameObject.SetActive(false);
            damageEventUI.transform.Find("Other").gameObject.SetActive(false);
            damageEventUI.transform.Find("DamageSource").gameObject.SetActive(false);
            damageEventUI.transform.Find("Location").gameObject.SetActive(true);
        }
        else if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Collapse"))
        {
            damageEventUI.transform.Find("FallDistance").gameObject.SetActive(false);
            damageEventUI.transform.Find("StructureHeight").gameObject.SetActive(true);
            damageEventUI.transform.Find("Other").gameObject.SetActive(false);
            damageEventUI.transform.Find("DamageSource").gameObject.SetActive(false);
            damageEventUI.transform.Find("Location").gameObject.SetActive(true);
        }
        else if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Other"))
        {
            damageEventUI.transform.Find("FallDistance").gameObject.SetActive(false);
            damageEventUI.transform.Find("StructureHeight").gameObject.SetActive(false);
            damageEventUI.transform.Find("Other").gameObject.SetActive(true);
            damageEventUI.transform.Find("DamageSource").gameObject.SetActive(true);
            damageEventUI.transform.Find("Location").gameObject.SetActive(false);
        }
        else if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Bloodletting"))
        {
            damageEventUI.transform.Find("FallDistance").gameObject.SetActive(false);
            damageEventUI.transform.Find("Other").gameObject.SetActive(false);
            damageEventUI.transform.Find("DamageSource").gameObject.SetActive(false);
            damageEventUI.transform.Find("Location").gameObject.SetActive(false);
        }
    }










    //xp functions - menu
    public void OpenXpAlertUI()
    {
        SetXpResolvedFlagTo(false);
        xpAlertUI.SetActive(true);
    }
    public void CloseXpAlertUI()
    {
        xpAlertUI.SetActive(false);
    }
    public void CloseXpLogUI()
    {
        xpLogUI.SetActive(false);
    }

    public void OpenPromotionUI()
    {
        promotionUI.SetActive(true);
    }

    public void ClosePromotionUI()
    {
        SetXpResolvedFlagTo(true);
        promotionUI.SetActive(false);
    }
    public void AddXpAlert(Soldier soldier, int xp, string xpDescription, bool learnerEnabled)
    {
        if (soldier != null && xp > 0)
        {
            //block duplicate xp alerts being created, made to obey the rule that xp for avoidance or detection can only be one per soldier per turn
            foreach (Transform child in xpLogUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            {
                //destroy duplicate avoidances/detections against same detecting/avoiding soldier
                if ((child.Find("XpDescription").GetComponent<TextMeshProUGUI>().text.Contains("Avoided") && child.Find("XpDescription").GetComponent<TextMeshProUGUI>().text == xpDescription) || (child.Find("XpDescription").GetComponent<TextMeshProUGUI>().text.Contains("Detected") && child.Find("XpDescription").GetComponent<TextMeshProUGUI>().text == xpDescription))
                    Destroy(child.gameObject);
            }

            if (soldier.IsConscious())
            {
                GameObject xpAlert = Instantiate(xpAlertPrefab, xpLogUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

                xpAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
                xpAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
                xpAlert.transform.Find("XpIndicator").GetComponent<TextMeshProUGUI>().text = xp.ToString();
                xpAlert.transform.Find("XpDescription").GetComponent<TextMeshProUGUI>().text = xpDescription;
                xpAlert.transform.Find("LearnerEnabled").GetComponent<Toggle>().isOn = learnerEnabled;

                //force xp event if made in override mode
                if (xpDescription.Contains("Override"))
                {
                    xpAlert.transform.Find("XpToggle").GetComponent<Toggle>().isOn = true;
                    xpAlert.transform.Find("XpToggle").GetComponent<Toggle>().interactable = false;
                }

                if (learnerEnabled && soldier.IsLearner())
                {
                    xpAlert.transform.Find("LearnerIndicator").gameObject.SetActive(true);
                    xpAlert.transform.Find("LearnerIndicator").GetComponent<TextMeshProUGUI>().text = "(+" + (int)((1.5f * xp) + 1 - xp) + ")";
                }
            }
            else
                print(soldier.soldierName + " cannot recieve xp unconscious.");
        }
    }

    public void OpenXpLogUI()
    {
        if (OverrideKey())
        {
            Transform xpAlerts = xpLogUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
            foreach (Transform child in xpAlerts)
            {
                if (child.GetComponent<SoldierAlert>().soldier.soldierTeam == game.currentTeam)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
            CloseXpAlertUI();
            xpLogUI.SetActive(true);
            xpLogUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        }
    }

    public void ConfirmXp()
    {
        ScrollRect xpScroller = xpLogUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        if (xpScroller.verticalNormalizedPosition <= 0.05f)
        {
            Transform xpAlerts = xpLogUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
            List<Transform> xpAlertsList = new();
            foreach (Transform child in xpAlerts)
            {
                bool learnerEnabled = child.Find("LearnerEnabled").GetComponent<Toggle>().isOn;
                Soldier soldier = child.GetComponent<SoldierAlert>().soldier;

                if (child.Find("XpToggle").GetComponent<Toggle>().isOn)
                {
                    int.TryParse(child.Find("XpIndicator").GetComponent<TextMeshProUGUI>().text, out int xp);
                    //block override xp from double incrementing
                    if (child.Find("XpDescription").GetComponent<TextMeshProUGUI>().text.Contains("Override"))
                        soldier.xp -= xp;
                    soldier.IncrementXP(xp, learnerEnabled);
                }
                xpAlertsList.Add(child);
            }

            CheckPromotion(xpAlertsList);

            //destroy all xp alerts for given team after done
            foreach (Transform child in xpAlerts)
                if (child.GetComponent<SoldierAlert>().soldier.IsOnturn())
                    Destroy(child.gameObject);

            CloseXpLogUI();
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }

    public void CheckXP()
    {
        bool display = false;
        Transform xpAlerts = xpLogUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");

        if (xpAlerts.childCount > 0)
        {
            foreach (Transform child in xpAlerts)
            {
                if (child.GetComponent<SoldierAlert>().soldier.IsOnturnAndAlive())
                {
                    //destroy xp alerts going to unconscious soldiers
                    if (!child.GetComponent<SoldierAlert>().soldier.IsConscious())
                        Destroy(child.gameObject);
                    else
                        display = true;

                }
            }
        }

        if (display)
            OpenXpAlertUI();
        else
            SetXpResolvedFlagTo(true);
    }

    public void CheckPromotion(List<Transform> potentialPromotions)
    {
        List<Soldier> xpRecievers = new();
        bool showPromotionUI = false;

        foreach (Transform child in potentialPromotions)
        {
            Soldier s = child.GetComponent<SoldierAlert>().soldier;
            if (!xpRecievers.Contains(s))
                xpRecievers.Add(s);
        }

        foreach (Soldier s in xpRecievers)
        {
            if (s.IsOnturnAndAlive() && s.xp >= s.MinXPForRank() * 2 && s.NextRank() != "")
            {
                AddPromotionAlert(s);
                showPromotionUI = true;
            }
        }

        if (showPromotionUI)
            OpenPromotionUI();
        else
            SetXpResolvedFlagTo(true);
    }
    public void AddPromotionAlert(Soldier soldier)
    {
        GameObject promotionAlert = Instantiate(promotionAlertPrefab, promotionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        promotionAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
        promotionAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
        promotionAlert.transform.Find("NextRankIndicator").GetComponent<Image>().sprite = soldier.LoadInsignia(soldier.NextRank());
        promotionAlert.transform.Find("PromotionRank").GetComponent<TextMeshProUGUI>().text = soldier.NextRank();

        if (soldier.NextRank() == "Captain")
        {
            promotionAlert.transform.Find("CaptaincyTitle").gameObject.SetActive(true);
            promotionAlert.transform.Find("CaptaincyToggle").gameObject.SetActive(true);
        }
    }
    public void ConfirmPromotion()
    {
        bool confirm = true;
        Transform promotionAlerts = promotionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
        List<Transform> promotionAlertsList = new();
        foreach (Transform child in promotionAlerts)
        {
            if (!child.GetComponent<SoldierAlert>().promotionComplete)
                confirm = false;

            promotionAlertsList.Add(child);
        }

        if (confirm)
        {
            //destroy all promotion alerts for given team after done
            foreach (Transform child in promotionAlerts)
                if (child.GetComponent<SoldierAlert>().soldier.IsOnturn())
                    Destroy(child.gameObject);

            ClosePromotionUI();
            CheckPromotion(promotionAlertsList);
        }
    }





    //item functions
    public void OpenUseItemUI(Item itemUsed, string itemUsedFromSlotName, ItemIcon linkedIcon)
    {
        useItemUI.transform.Find("OptionPanel").Find("Target").gameObject.SetActive(true);
        TMP_Dropdown targetDropdown = useItemUI.transform.Find("OptionPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        targetDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> targetOptionDataList = new();
        useItemUI.GetComponent<ConfirmUseItemUI>().itemUsed = itemUsed;
        useItemUI.GetComponent<ConfirmUseItemUI>().itemUsedIcon = linkedIcon;
        useItemUI.GetComponent<ConfirmUseItemUI>().itemUsedFromSlotName = itemUsedFromSlotName;
        useItemUI.transform.Find("OptionPanel").Find("Message").Find("Text").GetComponent<TextMeshProUGUI>().text = itemUsed.itemName switch
        {
            "Ammo_AR" => "Reload Assault Rifle?",
            "Ammo_LMG" => "Reload Light Machine Gun?",
            "Ammo_Pi" => "Reload Pistol?",
            "Ammo_Ri" => "Reload Rifle?",
            "Ammo_Sh" => "Reload Shotgun?",
            "Ammo_SMG" => "Reload Sub-Machine Gun?",
            "Ammo_Sn" => "Reload Sniper?",
            "Food_Pack" => "Consume food pack?",
            "ULF_Radio" => "Attempt to use ULF radio?",
            "Water_Canteen" => "Drink water?",
            "Medkit_Large" => "Use Large Medkit?",
            "Medkit_Medium" => "Use Medium Medkit?",
            "Medkit_Small" => "Use Small Medkit?",
            "Poison_Satchel" => "Administer Posion?",
            "Syringe_Amphetamine" => "Administer Amphetamine?",
            "Syringe_Androstenedione" => "Administer Androstenedione?",
            "Syringe_Cannabinoid" => "Administer Cannabinoid?",
            "Syringe_Danazol" => "Administer Danazol?",
            "Syringe_Glucocorticoid" => "Administer Glucocorticoid?",
            "Syringe_Modafinil" => "Administer Modafinil?",
            "Syringe_Shard" => "Administer Shard?",
            "Syringe_Trenbolone" => "Administer Trenbolone?",
            "Syringe_Unlabelled" => "Administer Unlabelled Syringe?",
            _ => "Unrecognised item",
        };

        if (itemUsed.itemName.Contains("Medkit"))
        {
            foreach (Soldier s in game.AllSoldiers())
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (activeSoldier.IsSameTeamAsIncludingSelf(s) && (s.IsInjured() || s.IsTraumatised()) && activeSoldier.PhysicalObjectWithinMeleeRadius(s))
                    targetOptionData = new(s.Id, s.soldierPortrait);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }
            targetDropdown.AddOptions(targetOptionDataList);

            if (targetOptionDataList.Count > 0)
            {
                game.UpdateSoldierUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                useItemUI.SetActive(true);
            }
        }
        else if (itemUsed.itemName.Contains("Syringe"))
        {
            foreach (Soldier s in game.AllSoldiers())
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (s.IsAlive() && activeSoldier.PhysicalObjectWithinMeleeRadius(s))
                    targetOptionData = new(s.Id, s.soldierPortrait);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }
            targetDropdown.AddOptions(targetOptionDataList);

            if (targetOptionDataList.Count > 0)
            {
                game.UpdateSoldierUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                useItemUI.SetActive(true);
            }
        }
        else if (itemUsed.itemName.Equals("Poison_Satchel"))
        {
            foreach (Item i in game.FindNearbyItems())
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (i.IsPoisonable())
                    targetOptionData = new(i.id, i.itemImage);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }
            targetDropdown.AddOptions(targetOptionDataList);

            if (targetOptionDataList.Count > 0)
            {
                game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                useItemUI.SetActive(true);
            }
        }
        else if (itemUsed.traits.Contains("Ammo"))
        {
            if (itemUsed.itemName.Contains("AR"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsAssaultRifle())
                        targetOptionData = new(i.id, i.itemImage);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("LMG"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsLMG())
                        targetOptionData = new(i.id, i.itemImage);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("Pi"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsPistol())
                        targetOptionData = new(i.id, i.itemImage);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("Ri"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsRifle())
                        targetOptionData = new(i.id, i.itemImage);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("Sh"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsShotgun())
                        targetOptionData = new(i.id, i.itemImage);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("SMG"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsSMG())
                        targetOptionData = new(i.id, i.itemImage);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("Sn"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsSniper())
                        targetOptionData = new(i.id, i.itemImage);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<ConfirmUseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
        }
        else
        {
            useItemUI.transform.Find("OptionPanel").Find("Target").gameObject.SetActive(false);
            useItemUI.SetActive(true);
        }
    }
    public void CloseUseItemUI()
    {
        useItemUI.SetActive(false);
    }
    public void OpenULFResultUI(string message)
    {
        ULFResultUI.transform.Find("OptionPanel").Find("Result").Find("Text").GetComponent<TextMeshProUGUI>().text = message;
        ULFResultUI.SetActive(true);
    }
    public void CloseULFResultUI()
    {
        ULFResultUI.SetActive(false);
    }








    //insert game objects function
    public void OpenOverrideInsertObjectsUI()
    {
        overrideInsertObjectsUI.SetActive(true);
        game.UpdateInsertGameObjectsUI();
    }
    public void ClearOverrideInsertObjectsUI()
    {
        overrideInsertObjectsUI.transform.Find("OptionPanel").Find("ObjectType").Find("ObjectTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Location").Find("XPos").GetComponent<TMP_InputField>().text = string.Empty;
        overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Location").Find("YPos").GetComponent<TMP_InputField>().text = string.Empty;
        overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Location").Find("ZPos").GetComponent<TMP_InputField>().text = string.Empty;
        overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value = 0;
    }
    public void CloseOverrideInsertObjectsUI()
    {
        ClearOverrideInsertObjectsUI();
        overrideInsertObjectsUI.SetActive(false);
    }








    //end of turn functions
    public void OpenPlayerTurnOverUI()
    {
        string displayText;
        if (game.currentTeam == 1)
            displayText = "<color=red>";
        else
            displayText = "<color=blue>";
        displayText += "Team " + game.currentTeam + "</color>: Leave Command Zone.";
        teamTurnOverUI.transform.Find("OptionPanel").Find("Title").Find("TitleText").GetComponent<TextMeshProUGUI>().text = displayText;
        teamTurnOverUI.SetActive(true);
    }
    public void ClosePlayerTurnOverUI()
    {
        SetTeamTurnOverFlagTo(true);
        teamTurnOverUI.SetActive(false);
    }
    public void OpenPlayerTurnStartUI()
    {
        string displayText;
        if (game.currentTeam == 1)
            displayText = "<color=red>";
        else
            displayText = "<color=blue>";
        displayText += "Team " + game.currentTeam + "</color>: Enter Command Zone.";
        teamTurnStartUI.transform.Find("OptionPanel").Find("Title").Find("TitleText").GetComponent<TextMeshProUGUI>().text = displayText;
        teamTurnStartUI.SetActive(true);
    }
    public void ClosePlayerTurnStartUI()
    {
        SetTeamTurnStartFlagTo(true);
        teamTurnStartUI.SetActive(false);
    }







    //properties
    public string[][] AllStats
    {
        get
        {
            return allStats;
        }
    }
}
