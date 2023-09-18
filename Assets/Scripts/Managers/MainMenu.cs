using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
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
    public SoundManager soundManager;

    public TextMeshProUGUI gameTimer, turnTimer, roundIndicator, teamTurnIndicator, weatherIndicator;
    public GameObject menuUI, teamTurnOverUI, teamTurnStartUI, setupMenuUI, gameTimerUI, gameMenuUI, soldierOptionsUI, soldierStatsUI, shotUI, flankersShotUI, shotConfirmUI,
        shotResultUI, moveUI, overmoveUI, suppressionMoveUI, moveToSameSpotUI, meleeUI, noMeleeTargetsUI, meleeBreakEngagementRequestUI, meleeResultUI, meleeConfirmUI,
        configureUI, soldierOptionsAdditionalUI, dipelecUI, dipelecResultUI, damageEventUI, overrideUI, detectionAlertUI, detectionUI, lostLosUI, damageUI, 
        traumaAlertUI, traumaUI, inspirerUI, xpAlertUI, xpLogUI, promotionUI, lastandicideConfirmUI, brokenFledUI, endSoldierTurnAlertUI, playdeadAlertUI, 
        coverAlertUI, overwatchUI, externalItemSourcesUI, allyItemButtonUI, flankersMeleeAttackerUI, flankersMeleeDefenderUI, allyInventoryPanelPrefab, detectionAlertPrefab, 
        lostLosAlertPrefab, damageAlertPrefab, traumaAlertPrefab, inspirerAlertPrefab, xpAlertPrefab, promotionAlertPrefab, allyItemsButtonPrefab, 
        soldierPortraitPrefab, meleeAlertPrefab, endTurnButton, overrideButton, overrideTimeStopIndicator, overrideVersionDisplay, overrideVisibilityDropdown, 
        overrideInsertObjectsButton, overrideInsertObjectsUI, undoButton, blockingScreen;
    public ItemIcon itemIconPrefab;
    public LOSArrow LOSArrowPrefab;
    public OverwatchArc overwatchArcPrefab;
    public SightRadiusCircle sightRadiusCirclePrefab;
    public List<Button> actionButtons;
    public List<Sprite> insignia;
    public Button shotButton, moveButton, meleeButton, configureButton, lastandicideButton, dipElecButton, overwatchButton, coverButton, playdeadButton, additionalOptionsButton;
    private float playTimeTotal;
    public float turnTime;
    public string meleeChargeIndicator;
    public Soldier[] allSoldiers;
    public Soldier activeSoldier;
    public bool overrideView, displayLOSArrows, clearShotFlag, clearMeleeFlag, clearDipelecFlag, clearMoveFlag, meleeResolvedFlag, inspirerResolvedFlag, clearDamageEventFlag, xpResolvedFlag, teamTurnOverFlag, teamTurnStartFlag;
    public TMP_InputField LInput, HInput, RInput, SInput, EInput, FInput, PInput, CInput, SRInput, RiInput, ARInput, LMGInput, SnInput, SMGInput, ShInput, MInput, StrInput, DipInput, ElecInput, HealInput;
    public Sprite detection1WayLeft, detection1WayRight, avoidance1WayLeft, avoidance1WayRight, detection2Way, avoidance2Way, avoidance2WayLeft, avoidance2WayRight, 
        detectionOverwatch2WayLeft, detectionOverwatch2WayRight, avoidanceOverwatch2WayLeft, avoidanceOverwatch2WayRight, overwatch1WayLeft, overwatch1WayRight, fist;

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
    private readonly string[][] abilities =
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
                allSoldiers = FindObjectsOfType<Soldier>();
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
        foreach (Soldier s in allSoldiers)
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
            print("Overwatch registered");
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
            GetOverrideVisibility();
        }
    }
    public void GetOverrideHealthState(Transform soldierStatsUI)
    {
        TMP_Dropdown dropdown = soldierStatsUI.Find("General").Find("OverrideHealthState").Find("HealthStateDropdown").GetComponent<TMP_Dropdown>();

        if (activeSoldier.state.Contains("Unconscious"))
            dropdown.value = 2;
        else if (activeSoldier.state.Contains("Last Stand"))
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
        //Debug.Log("setoverridevis");
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
            StartCoroutine(game.DetectionAlertAll("statChange"));
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
                StartCoroutine(game.DetectionAlertSingle(activeSoldier, "statChange", Vector3.zero, string.Empty));

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
                foreach (string[] abilityTuple in abilities)
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

                StartCoroutine(game.DetectionAlertSingle(activeSoldier, "losChange", Vector3.zero, string.Empty));
            }
        }

        locationInput.text = "";
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
                activeSoldier.tp = newTrauma;

        traumaInput.text = "";
    }







    //display functions - menu
    public string DisplayWeather()
    {
        string displayWeather = "";

        displayWeather += weather.CurrentWeather;

        foreach (Soldier s in allSoldiers)
            if (s.IsOnturnAndAlive() && s.IsExperimentalist())
                displayWeather += "\n<color=green>" + weather.NextTurnWeather + "</color>";

        return displayWeather;
    }
    
    public void DisplaySoldiers()
    {
        foreach (Soldier s in allSoldiers)
        {
            if (overrideView)
                s.soldierUI.SetActive(true);
            else
            {
                if (s.soldierTeam == game.currentTeam)
                    s.soldierUI.SetActive(true);
                else
                    s.soldierUI.SetActive(false);
            }
        }
    }
    public void RenderSoldierVisuals()
    {
        foreach (Soldier s in allSoldiers)
        {
            if (overrideView)
                s.GetComponent<Renderer>().enabled = true;
            else
            {
                if (s.soldierTeam == game.currentTeam || s.IsRevealed() || s.IsDead() || s.IsPlayingDead())
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

        foreach (Soldier s in allSoldiers)
        {
            if (s.IsDead() || s.IsUnconscious())
            {
                if (s.soldierTeam == 1)
                    p1DeadCount++;
                else
                    p2DeadCount++;
            }    
        }

        if (p1DeadCount == allSoldiers.Length / 2)
            game.GameOver("<color=blue>Team 2</color> Victory");

        if (p2DeadCount == allSoldiers.Length / 2)
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
                {
                    if (i.transform.parent.GetComponent<Soldier>().soldierTeam == game.currentTeam)
                        i.GetComponent<Renderer>().enabled = true;
                    else
                        i.GetComponent<Renderer>().enabled = false;
                }
            }
        }
    }
    public void DisplaySoldiersGameOver()
    {
        var allSoldiers = FindObjectsOfType<Soldier>();
        foreach (Soldier s in allSoldiers)
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
        Transform inventoryUI = soldierStatsUI.transform.Find("ItemDisplayPanel").Find("Inventory").Find("InventoryPanel").Find("Viewport").Find("DisplayInventoryContent");

        foreach (Item i in activeSoldier.inventory.Items)
            Instantiate(itemIconPrefab, inventoryUI).Init(i.itemName, 1, i);

        soldierStatsUI.SetActive(true);
    }
    public void CloseSoldierStatsUI()
    {
        Transform inventoryUI = soldierStatsUI.transform.Find("ItemDisplayPanel").Find("Inventory").Find("InventoryPanel").Find("Viewport").Find("DisplayInventoryContent");

        foreach (Transform child in inventoryUI)
            Destroy(child.gameObject);

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
        if (activeSoldier.state.Contains("Last Stand"))
            lastandicideButton.gameObject.SetActive(true);
        else
            lastandicideButton.gameObject.SetActive(false);

        if (game.gameOver)
            GreyAll("Game Over");
        else if (overrideView)
            GreyOutButtons(AddAllButtons(buttonStates), "Override");
        else if (activeSoldier.state.Contains("Dead"))
            GreyOutButtons(AddAllButtons(buttonStates), "Dead");
        else if (activeSoldier.state.Contains("Unconscious"))
            GreyOutButtons(AddAllButtons(buttonStates), "<color=blue>Unconscious</color>");
        else if (activeSoldier.IsStunned())
            GreyOutButtons(AddAllButtons(buttonStates), "Stunned");
        else if (activeSoldier.state.Contains("Playdead"))
            GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), playdeadButton), "<color=yellow>Playdead</color>");
        else if (activeSoldier.ap == 0)
            GreyOutButtons(AddAllButtons(buttonStates), "No AP");
        else if (activeSoldier.tp == 4)
        {
            //if in last stand regain control
            if (activeSoldier.state.Contains("Last Stand"))
            {
                buttonStates.Add(moveButton, "Last Stand");
                GreyOutButtons(buttonStates, "");
            }
            else
                GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), moveButton), "Broken");
        }
        else if (activeSoldier.IsMeleeControlled())
        {
            //if has pistol/smg add shot button
            bool canShoot = false;
            foreach (Item i in activeSoldier.inventory.Items)
                if (i.gunType == "SMG" || i.gunType == "Pistol")
                    canShoot = true;

            if (canShoot)
                GreyOutButtons(ExceptButton(ExceptButton(AddAllButtons(buttonStates), meleeButton), shotButton), "<color=red>Melee Controlled</color>");
            else
                GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), meleeButton), "<color=red>Melee Controlled</color>");
        }
        else
        {
            //block move button
            if (activeSoldier.state.Contains("Last Stand"))
                buttonStates.Add(moveButton, "Last Stand");
            else if (activeSoldier.mp == 0)
                buttonStates.Add(moveButton, "No MA");
            else if (activeSoldier.IsMeleeControlling())
                buttonStates.Add(moveButton, "Melee Controlling");

            //block cover button
            if (activeSoldier.inventory.IsWearingJuggernautArmour())
                buttonStates.Add(coverButton, "<color=green>Juggernaut</color>");
            else if (activeSoldier.IsInCover())
                buttonStates.Add(coverButton, "<color=green>Taking Cover</color>");


            //block shot and overwatch buttons
            bool hasGun = false;
            foreach (Item i in activeSoldier.inventory.Items)
                if (i.gunType != null)
                    hasGun = true;
            bool canShootInMelee = false;
            foreach (Item i in activeSoldier.inventory.Items)
                if (i.gunType == "SMG" || i.gunType == "Pistol")
                    canShootInMelee = true;
            if (!hasGun)
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
                if (!canShootInMelee)
                    buttonStates.Add(shotButton, "Melee Controlling");
                buttonStates.Add(overwatchButton, "Melee Controlling");
            }

            //block melee button
            if (!activeSoldier.FindMeleeTargets())
            {
                buttonStates.Add(meleeButton, "No Target");
            }
            else if (activeSoldier.stats.SR.Val == 0)
            {
                buttonStates.Add(meleeButton, "Blind");
            }

            //block dipelec button
            if (activeSoldier.stats.SR.Val == 0)
            {
                buttonStates.Add(dipElecButton, "Blind");
            }

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
        var soldierBanner = soldierOptionsUI.transform.Find("SoldierBanner");

        soldierBanner.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(activeSoldier);
        soldierBanner.Find("HP").GetComponent<TextMeshProUGUI>().text = "HP: " + activeSoldier.GetFullHP().ToString();
        soldierBanner.Find("AP").GetComponent<TextMeshProUGUI>().text = "AP: " + activeSoldier.ap.ToString();
        soldierBanner.Find("MP").GetComponent<TextMeshProUGUI>().text = "MA: " + activeSoldier.mp.ToString();
        soldierBanner.Find("Speed").GetComponent<TextMeshProUGUI>().text = "Max Move: " + activeSoldier.InstantSpeed.ToString();
        soldierBanner.Find("XP").GetComponent<TextMeshProUGUI>().text = "XP: " + activeSoldier.xp.ToString();
        soldierBanner.Find("Status").GetComponent<TextMeshProUGUI>().text = "Status: " + GetStatus();

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

        var soldierStatsUI = soldierBanner.Find("SoldierStatsUI");
        if (soldierStatsUI.gameObject.activeInHierarchy)
        {
            PaintSpeciality(soldierStatsUI);

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
            soldierStatsUI.Find("General").Find("Specialty").GetComponent<TextMeshProUGUI>().text = activeSoldier.soldierSpeciality;
            soldierStatsUI.Find("General").Find("Ability").GetComponent<TextMeshProUGUI>().text = PrintList(activeSoldier.soldierAbilities);
            soldierStatsUI.Find("General").Find("Location").Find("LocationX").GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
            soldierStatsUI.Find("General").Find("Location").Find("LocationY").GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
            soldierStatsUI.Find("General").Find("Location").Find("LocationZ").GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();
            soldierStatsUI.Find("General").Find("TerrainOn").GetComponent<TextMeshProUGUI>().text = activeSoldier.TerrainOn;
            soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().text = activeSoldier.RoundsWithoutFood.ToString();
            soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().text = activeSoldier.tp.ToString();
        }
    }
    public void PaintSpeciality(Transform soldierStatsUI)
    {
        TextMeshProUGUI[] statLabels = soldierStatsUI.Find("Stats").Find("Labels").GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI t in statLabels)
        {
            foreach (string[] s in allStats)
            {
                Color displayColor = Color.white;
                if (t.text == s[0] && s[1] == activeSoldier.soldierSpeciality)
                {
                    displayColor = Color.green;
                    t.color = displayColor;
                    break;
                }
                else
                    t.color = displayColor;
            }
        }
    }
    public string GetHealthState()
    {
        int hp = activeSoldier.hp;

        if (hp > activeSoldier.stats.H.Val)
            return "<color=green>Overhealth</color>";
        else if (hp == activeSoldier.stats.H.Val)
            return "Full Health";
        else if (hp <= 0)
            return "<color=red>Dead</color>";
        else if (hp <= activeSoldier.stats.H.Val / 3)
            return "<color=red>Critically Injured</color>";
        else if (hp <= activeSoldier.stats.H.Val / 2)
            return "<color=orange>Severely Injured</color>";
        else
            return "<color=yellow>Injured</color>";
    }

    public string GetConsciousState()
    {
        if (activeSoldier.state.Contains("Unconscious"))
            return ", <color=blue>Unconscious</color>";
        else if (activeSoldier.state.Contains("Last Stand"))
            return ", <color=red>Last Stand</color>";
        else if (activeSoldier.state.Contains("Active"))
            return ", Active";
        else
            return "";
    }
    public string GetArmourState()
    {
        if (activeSoldier.GetArmourHP() > 0)
            return ", <color=green>Armoured(" + activeSoldier.GetArmourHP() + ")</color>";
        else
            return "";
    }
    public string GetTraumaState()
    {
        return activeSoldier.tp switch
        {
            0 => ", Committed",
            1 => ", <color=yellow>Wavering</color>",
            2 => ", <color=yellow>Shaken</color>",
            3 => ", <color=orange>Frozen</color>",
            4 => ", <color=red>Broken</color>",
            _ => ", <color=blue>Desensitised</color>",
        };
    }

    public string GetStunnedState()
    {
        if (activeSoldier.IsStunned())
            return ", <color=red>Stunned(" + activeSoldier.stunnedRoundsVulnerable + ")</color>";
        else
            return "";
    }

    public string GetHungerState()
    {
        if (activeSoldier.RoundsWithoutFood >= 30)
            return ", <color=red>Starving</color>";
        else if (activeSoldier.RoundsWithoutFood >= 20)
            return ", <color=orange>Very Hungry</color>";
        else if (activeSoldier.RoundsWithoutFood >= 10)
            return ", <color=yellow>Hungry</color>";
        else
            return "";
    }
    public string GetMeleeControlState()
    {
        string controlString = "";

        if (activeSoldier.controllingSoldiersList.Count > 0)
        {
            controlString += ", <color=green>Controlling (";

            for (int i = 0; i < activeSoldier.controllingSoldiersList.Count; i++)
            {
                if (i > 0)
                    controlString += ", " + soldierManager.FindSoldierById(activeSoldier.controllingSoldiersList[i]).soldierName;
                else
                    controlString += soldierManager.FindSoldierById(activeSoldier.controllingSoldiersList[i]).soldierName;
            }
            controlString += ")</color>";
        }
            
        if (activeSoldier.controlledBySoldiersList.Count > 0)
        {
            controlString += ", <color=red>Controlled By (";

            for (int i = 0; i < activeSoldier.controlledBySoldiersList.Count; i++)
            {
                if (i > 0)
                    controlString += ", " + soldierManager.FindSoldierById(activeSoldier.controlledBySoldiersList[i]).soldierName;
                else
                    controlString += soldierManager.FindSoldierById(activeSoldier.controlledBySoldiersList[i]).soldierName;
            }
            controlString += ")</color>";
        }

        return controlString;
    }
    public string GetCoverState()
    {
        if (activeSoldier.IsInCover())
            return ", <color=green>Taking Cover</color>";
        else
            return "";
    }
    public string GetOverwatchState()
    {
        if (activeSoldier.IsOnOverwatch())
            return ", <color=green>Overwatch</color>";
        else
            return "";
    }
    public string GetLoudDetectedState()
    {
        if (activeSoldier.loudActionRoundsVulnerable > 0)
            return ", <color=red>Vulnerable(" + activeSoldier.loudActionRoundsVulnerable + ")</color>";
        else
            return "";
    }

    public string GetPoisonedState()
    {
        if (activeSoldier.state.Contains("Poisoned"))
            return ", <color=red>Poisoned</color>";
        else
            return "";
    }

    public string GetSuppressionState()
    {
        if (activeSoldier.GetSuppression() > 0)
            return ", <color=orange>Suppressed (" + activeSoldier.GetSuppression() + ")</color>";
        else
            return "";
    }

    public string GetPlaydeadState()
    {
        if (activeSoldier.state.Contains("Playdead"))
            return ", <color=yellow>Playdead</color>";
        else
            return "";
    }

    public string GetDrugState()
    {
        if (activeSoldier.state.Contains("Modafinil"))
            return ", <color=purple>Modafinil</color>";
        else if (activeSoldier.state.Contains("Amphetamine"))
            return ", <color=purple>Amphetamine</color>";
        else if (activeSoldier.state.Contains("Androstenedione"))
            return ", <color=purple>Androstenedione</color>";
        else if (activeSoldier.state.Contains("Cannabinoid"))
            return ", <color=purple>Cannabinoid</color>";
        else if (activeSoldier.state.Contains("Shard"))
            return ", <color=purple>Shard</color>";
        else if (activeSoldier.state.Contains("Glucocorticoid"))
            return ", <color=purple>Glucocorticoid</color>";
        else if (activeSoldier.state.Contains("Danazol"))
            return ", <color=purple>Danazol</color>";
        else if (activeSoldier.state.Contains("Trenbolone"))
            return ", <color=purple>Trenbolone</color>";
        else
            return "";
    }
    public string GetPatriotState()
    {
        if (activeSoldier.TerrainOn == activeSoldier.soldierTerrain && activeSoldier.IsPatriot())
            return ", <color=green>Patriotic</color>";
        else
            return "";
    }
    public string GetInspiredState()
    {
        if (activeSoldier.IsInspired())
            return ", <color=green>Inspired</color>";
        else
            return "";
    }
    public string GetDissuadedState()
    {
        if (activeSoldier.IsDissuaded())
            return ", <color=red>Dissuaded</color>";
        else
            return "";
    }
    public string GetBloodRageState()
    {
        if (activeSoldier.IsBloodRaged())
            return ", <color=green>Blood Rage</color>";
        else
            return "";
    }
    public string GetStatus()
    {
        string status = "";
        status += GetHealthState();

        if (activeSoldier.IsAlive())
        {
            status += GetConsciousState();
            status += GetArmourState();
            status += GetTraumaState();
            status += GetStunnedState();
            status += GetHungerState();
            status += GetLoudDetectedState();
            status += GetMeleeControlState();
            status += GetOverwatchState();
            status += GetCoverState();
            status += GetPoisonedState();
            status += GetSuppressionState();
            status += GetPlaydeadState();
            status += GetDrugState();

            status += GetPatriotState();
            status += GetInspiredState();
            status += GetDissuadedState();
            status += GetBloodRageState();
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

        return status;
    }

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
        if (detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content").childCount > 0)
        {
            FreezeTime();
            detectionAlertUI.SetActive(true);
            soundManager.PlayDetectionAlarm();
        }
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

            detectionAlertUI.SetActive(false);
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
        //Debug.Log("ConfirmDetections()");
        ScrollRect detectionScroller = detectionUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        if (detectionScroller.verticalNormalizedPosition <= 0.05f)
        {
            //Debug.Log("ConfirmDetections() passed into the detectionscroller block");

            //create list of soldiers and who they're revealing
            Dictionary<string, List<string>> allSoldiersRevealing = new();
            Dictionary<string, List<string>> allSoldiersRevealedBy = new();
            Dictionary<string, List<string>> allSoldiersNotRevealing = new();
            Dictionary<string, List<string>> allSoldiersNotRevealedBy = new();
            Dictionary<string, List<string>> allSoldiersRevealingFinal = new();

            foreach (Soldier s in allSoldiers)
            {
                allSoldiersRevealing.Add(s.id, new List<string>());
                allSoldiersRevealedBy.Add(s.id, new List<string>());
                allSoldiersNotRevealing.Add(s.id, new List<string>());
                allSoldiersNotRevealedBy.Add(s.id, new List<string>());
                allSoldiersRevealingFinal.Add(s.id, new List<string>());

                //add soldiers that can't possibly be seen cause out of radius
                foreach (Soldier s2 in allSoldiers)
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
                    if (child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text.Contains("DETECTED"))
                    {
                        //if not a glimpse or a retreat detection, add soldier to revealing list
                        if (!child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text.Contains("GLIMPSE") && !child.Find("Detector").Find("CounterLabel").GetComponent<TextMeshProUGUI>().text.Contains("RETREAT"))
                            allSoldiersRevealing[counter.id].Add(detector.id);
                        else
                            allSoldiersNotRevealing[counter.id].Add(detector.id);

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
                        print(child.Find("DetectionArrow").GetComponent<Image>().sprite.name);
                        if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("verwatch"))
                        {
                            if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("Left"))
                                StartCoroutine(OpenOverwatchShotUI(counter, detector));
                            else if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("Right"))
                                StartCoroutine(OpenOverwatchShotUI(detector, counter));
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
                    if (child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text.Contains("DETECTED"))
                    {
                        //if not a glimpse or a retreat detection, add soldier to revealing list
                        if (!child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text.Contains("GLIMPSE") && !child.Find("Counter").Find("DetectorLabel").GetComponent<TextMeshProUGUI>().text.Contains("RETREAT"))
                            allSoldiersRevealing[detector.id].Add(counter.id);
                        else
                            allSoldiersNotRevealing[detector.id].Add(counter.id);

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
                        print(child.Find("DetectionArrow").GetComponent<Image>().sprite.name);
                        if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("verwatch"))
                        {
                            if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("Left"))
                                StartCoroutine(OpenOverwatchShotUI(counter, detector));
                            else if (child.Find("DetectionArrow").GetComponent<Image>().sprite.name.Contains("Right"))
                                StartCoroutine(OpenOverwatchShotUI(detector, counter));
                        }
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
                //Debug.Log(keyValuePair.Key + " " + game.FindSoldierById(keyValuePair.Key).soldierName);
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

                //Debug.Log(IdToName(keyValuePair.Key) + " : " + PrintList(arrayOfRevealingList) + " + " + PrintList(arrayOfOldRevealingList) + " - " + PrintList(arrayOfNotRevealingList) + " = " + PrintList(arrayFinalRevealingList));

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
            foreach (Soldier s in allSoldiers)
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
            Debug.Log("Haven't scrolled all the way to the bottom");
    }
    public void CloseDetectionUI()
    {
        detectionUI.SetActive(false);
        UnfreezeTime();
    }
    public void AddLostLosAlert(Soldier soldier)
    {
        //block duplicate lostlos alerts being created
        foreach (Transform child in lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if (child.GetComponent<SoldierAlert>().soldier == soldier)
                Destroy(child.gameObject);

        GameObject lostLosAlert = Instantiate(lostLosAlertPrefab, lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        lostLosAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
        lostLosAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
        lostLosAlert.transform.Find("LostLosDescription").GetComponent<TextMeshProUGUI>().text = soldier.soldierName + " is now hidden, remove him from the board.";
    }
    public IEnumerator OpenLostLOSList()
    {
        //Debug.Log("OpenLostLosList(start)");
        //yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => meleeResolvedFlag == true && overrideView == false);
        //Debug.Log("OpenLostLosList(passedmeleeflag)");
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
            Debug.Log("Haven't scrolled all the way to the bottom");
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
    public void AddDamageAlert(Soldier soldier, string description, bool resisted)
    {
        GameObject damageAlert = Instantiate(damageAlertPrefab, damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        if (resisted)
            damageAlert.transform.Find("DamageTitle").GetComponent<TextMeshProUGUI>().text = "<color=green>DAMAGE RESISTED</color>";

        damageAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
        damageAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
        damageAlert.transform.Find("DamageDescription").GetComponent<TextMeshProUGUI>().text = description;

        //try and open damagealert
        StartCoroutine(OpenDamageList());
    }
    public IEnumerator OpenDamageList()
    {
        //Debug.Log("OpenLostLosList(start)");
        //yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => meleeResolvedFlag == true && overrideView == false);
        //Debug.Log("OpenLostLosList(passedmeleeflag)");
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
            Debug.Log("Haven't scrolled all the way to the bottom");
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
            "LMG" => "+5% LMG Aim",
            "Sniper" => "+5% Sn Aim",
            "SMG" => "+5% SMG Aim",
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
        traumaUI.SetActive(true);
    }

    public void CloseTraumaUI()
    {
        traumaUI.SetActive(false);
    }
    public void AddTraumaAlert(Soldier friendly, int trauma, string reason, int rolls, int xpOnResist, string range)
    {
        GameObject traumaAlert = Instantiate(traumaAlertPrefab, traumaUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));
        traumaAlert.GetComponent<SoldierAlert>().SetSoldier(friendly);

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
                traumaAlert.transform.Find("TraumaToggle").GetComponent<Toggle>().interactable = false;
                traumaAlert.transform.Find("TraumaIndicator").gameObject.SetActive(false);
                traumaAlert.transform.Find("ConfirmButton").gameObject.SetActive(false);
                traumaAlert.transform.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text = "<color=green>RESILIENT</color>";
            }
        }

        traumaAlert.transform.Find("TraumaIndicator").GetComponent<TextMeshProUGUI>().text = trauma.ToString();
        traumaAlert.transform.Find("TraumaDescription").GetComponent<TextMeshProUGUI>().text = reason;
        traumaAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(friendly);
        traumaAlert.transform.Find("Rolls").GetComponent<TextMeshProUGUI>().text = rolls.ToString();
        traumaAlert.transform.Find("XpOnResist").GetComponent<TextMeshProUGUI>().text = xpOnResist.ToString();
        traumaAlert.transform.Find("Distance").GetComponent<TextMeshProUGUI>().text = range;
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
        brokenFledUI.SetActive(true);
    }

    public void CloseBrokenFledUI()
    {
        UnfreezeTime();
        brokenFledUI.SetActive(false);
    }









    //shot functions - menu
    public void OpenShotUI()
    {
        //set shooter details
        Soldier shooter = activeSoldier;
        shotUI.transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text = shooter.id;

        TMP_Dropdown shotTypeDropdown = shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>();
        shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        TMP_Dropdown gunDropdown = shotUI.transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown aimDropdown = shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown targetDropdown = shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown coverDropdown = shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>();
        List<TMP_Dropdown.OptionData> targetDetails = new(), gunDetails = new();
        List<string> noTargets = new() { "No Targets" };

        //generate guns list
        foreach (Item i in activeSoldier.inventory.Items)
        {
            if (i.gunType != null)
                gunDetails.Add(new TMP_Dropdown.OptionData(i.id, i.itemImage));
        }
        if (gunDetails.Count > 0)
            gunDropdown.AddOptions(gunDetails);

        //generate target list
        foreach (Soldier s in allSoldiers)
        {
            TMP_Dropdown.OptionData target = null;
            if (s.IsAlive() && activeSoldier.IsOppositeTeamAs(s) && s.IsRevealed())
            {
                if (activeSoldier.CanSeeInOwnRight(s))
                    target = new(s.soldierName, s.soldierPortrait);
                else
                    target = new(s.soldierName, s.LoadPortraitTeamsight(s.soldierPortraitText));
            }
            
            if (target != null)
            {
                targetDetails.Add(target);

                //remove option if target is jammer and shooter can't see in own right
                if (s.IsJammer() && !activeSoldier.CanSeeInOwnRight(s))
                    targetDetails.Remove(target);

                //remove option if soldier is engaged and this soldier is not on the engagement list
                if (activeSoldier.IsMeleeEngaged() && !activeSoldier.IsMeleeEngagedWith(s))
                    targetDetails.Remove(target);
            }
        }

        //check target list
        if (targetDetails.Count > 0)
        {
            Item gun = game.itemManager.FindItemById(game.gunTypeDropdown.options[game.gunTypeDropdown.value].text);
            targetDropdown.AddOptions(targetDetails);
            targetDropdown.interactable = true;
            //block suppression option if gun does not have enough ammo
            if (!gun.CheckSpecificAmmo(gun.gunSuppressionDrain, true))
                shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression Shot");

            shotTypeDropdown.value = 0;
            CheckTargetInCover();
        }
        else
        {
            targetDropdown.AddOptions(noTargets);
            targetDropdown.interactable = false;
            shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Standard Shot");
            shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression Shot");
            shotTypeDropdown.value = 2;
        }

        //if soldier engaged in melee block force unaimed shot
        if (activeSoldier.IsMeleeEngaged())
            aimDropdown.value = 1;
        else
            aimDropdown.value = 0;

        BlockShotOptions(false);

        game.UpdateShotUI(shooter);

        shotUI.SetActive(true);
    }
    public void BlockShotOptions(bool overwatch)
    {
        if (overwatch)
        {
            shotUI.transform.Find("BackButton").GetComponent<Button>().interactable = false;
            shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>().interactable = false;
            shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>().interactable = false;
            shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().interactable = false;
            shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>().interactable = false;
        }
        else if (activeSoldier.IsMeleeEngaged())
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
    public IEnumerator OpenOverwatchShotUI(Soldier shooter, Soldier target)
    {
        yield return new WaitForSeconds(0.05f);

        //set shooter
        shotUI.transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text = shooter.id;

        TMP_Dropdown shotTypeDropdown = shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>();
        shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        TMP_Dropdown gunDropdown = shotUI.transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown aimDropdown = shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown targetDropdown = shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown coverDropdown = shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>();
        List<TMP_Dropdown.OptionData> targetDetails = new(), gunDetails = new();

        //set as regular shot
        shotTypeDropdown.value = 0;
        
        //set as aimed shot
        aimDropdown.value = 0;

        //set no cover
        coverDropdown.value = 0;

        //generate guns list
        gunDropdown.ClearOptions();
        foreach (Item i in shooter.inventory.Items)
        {
            if (i.gunType != null)
                gunDetails.Add(new TMP_Dropdown.OptionData(i.id, i.itemImage));
        }
        if (gunDetails.Count > 0)
            gunDropdown.AddOptions(gunDetails);

        //set target
        TMP_Dropdown.OptionData option = new(target.soldierName, target.soldierPortrait);
        targetDetails.Add(option);
        targetDropdown.AddOptions(targetDetails);
        CheckTargetInCover();

        BlockShotOptions(true);
        
        game.UpdateShotUI(shooter);

        shotUI.SetActive(true);
    }

    public void CheckTargetInCover()
    {
        TMP_Dropdown targetDropdown = shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown coverDropdown = shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>();
        Soldier targetSoldier = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);

        //reset selection to no cover
        coverDropdown.value = 0;

        //show the cover level only if man is in cover
        if (targetSoldier != null)
        {
            if (targetSoldier.IsInCover())
                shotUI.transform.Find("TargetPanel").Find("CoverLevel").gameObject.SetActive(true);
            else
                shotUI.transform.Find("TargetPanel").Find("CoverLevel").gameObject.SetActive(false);
        }
    }
    public void OpenShotResultUI()
    {
        FreezeTime();
        shotResultUI.SetActive(true);
    }
    public void CloseShotResultUI()
    {
        UnfreezeTime();
        shotResultUI.SetActive(false);
    }
    public void ClearShotUI()
    {
        clearShotFlag = true;
        shotUI.transform.Find("ShotType").Find("ShotTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        shotUI.transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("Aim").Find("AimDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        shotUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("TargetPanel").Find("CoverLevel").Find("CoverLevelDropdown").GetComponent<TMP_Dropdown>().value = 0;
        shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("XPos").GetComponent<TMP_InputField>().text = "";
        shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("YPos").GetComponent<TMP_InputField>().text = "";
        shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        game.ClearFlankersUI(flankersShotUI);
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
                //find shooter
                Soldier shooter = soldierManager.FindSoldierById(shotUI.transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text);
                Soldier target = soldierManager.FindSoldierByName(game.targetDropdown.options[game.targetDropdown.value].text);
                Tuple<int, int, int> chances;

                //if suppression shot hit chance is always 100
                if (game.shotTypeDropdown.value == 1)
                    chances = Tuple.Create(100, 0, 100);
                else
                    chances = game.CalculateHitPercentage(shooter, target);

                //only shot suppression hit chance if suppressed
                if (shooter.IsSuppressed() && game.shotTypeDropdown.value != 1)
                    shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(true);
                else
                    shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(false);

                shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").Find("SuppressedHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item3.ToString() + "%";
                shotConfirmUI.transform.Find("OptionPanel").Find("HitChance").Find("HitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item1.ToString() + "%";
                shotConfirmUI.transform.Find("OptionPanel").Find("CritHitChance").Find("CritHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item2.ToString() + "%";

                //enable back button only if shot is aimed and under 25%
                if (chances.Item1 <= 25 && game.aimTypeDropdown.value == 0 && !shooter.IsOnOverwatch())
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
        TMP_Dropdown attackerWeaponDropdown = meleeUI.transform.Find("AttackerWeapon").Find("WeaponDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown defenderWeaponDropdown = meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").Find("WeaponDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown targetDropdown = meleeUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        List<TMP_Dropdown.OptionData> targetDetails = new(), attackerWeaponDetails = new(), defenderWeaponDetails = new();

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

        //generate attacker weapon list
        attackerWeaponDetails.Add(new TMP_Dropdown.OptionData("Fist", fist));
        foreach (Item i in attacker.inventory.Items)
        {
            if (i.equippableSlots.Contains("Hand_Slot"))
                attackerWeaponDetails.Add(new TMP_Dropdown.OptionData(i.id, i.itemImage));
        }
        attackerWeaponDropdown.AddOptions(attackerWeaponDetails);

        //generate target list
        foreach (Soldier s in allSoldiers)
        {
            TMP_Dropdown.OptionData target = null;
            if (s.IsAlive() && attacker.IsOppositeTeamAs(s) && s.IsRevealed() && attacker.PhysicalObjectWithinMeleeRadius(s))
            {
                if (attacker.CanSeeInOwnRight(s))
                    target = new(s.soldierName, s.soldierPortrait);
                else
                    target = new(s.soldierName, s.LoadPortraitTeamsight(s.soldierPortraitText));

                targetDetails.Add(target);
            }

            if (target != null)
            {
                //remove option if soldier is engaged and this soldier is not on the engagement list
                if (attacker.IsMeleeEngaged() && !attacker.IsMeleeEngagedWith(s))
                    targetDetails.Remove(target);
            }
        }

        if (targetDetails.Count > 0)
        {
            targetDropdown.AddOptions(targetDetails);

            Soldier target = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);

            if (target.controlledBySoldiersList.Contains(activeSoldier.id))
                meleeTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("<color=green>Disengage</color>") });
            else if (target.controllingSoldiersList.Contains(activeSoldier.id))
                meleeTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("<color=red>Request Disengage</color>") });

            //generate defender weapon list
            defenderWeaponDetails.Add(new TMP_Dropdown.OptionData("Fist", fist));
            foreach (Item i in target.inventory.Items)
            {
                if (i.equippableSlots.Contains("Hand_Slot"))
                    defenderWeaponDetails.Add(new TMP_Dropdown.OptionData(i.id, i.itemImage));
            }
            defenderWeaponDropdown.AddOptions(defenderWeaponDetails);

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
    public void CheckTargetWeapon()
    {
        TMP_Dropdown defenderWeaponDropdown = meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").Find("WeaponDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown targetDropdown = meleeUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        Soldier defender = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);
        List<TMP_Dropdown.OptionData> defenderWeaponDetails = new();

        //generate defender weapon list
        defenderWeaponDropdown.ClearOptions();
        defenderWeaponDropdown.value = 0;
        defenderWeaponDetails.Add(new TMP_Dropdown.OptionData("Fist", fist));
        foreach (Item i in defender.inventory.Items)
        {
            if (i.equippableSlots.Contains("Hand_Slot"))
                defenderWeaponDetails.Add(new TMP_Dropdown.OptionData(i.id, i.itemImage));
        }
        defenderWeaponDropdown.AddOptions(defenderWeaponDetails);
    }
    public void CheckMeleeType()
    {
        TMP_Dropdown meleeTypeDropdown = meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>();

        if (meleeTypeDropdown.value == 0)
        {
            meleeUI.transform.Find("AttackerWeapon").gameObject.SetActive(true);
            meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").gameObject.SetActive(true);
        }
        else
        {
            meleeUI.transform.Find("AttackerWeapon").gameObject.SetActive(false);
            meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").gameObject.SetActive(false);
        }

        if (meleeTypeDropdown.options[meleeTypeDropdown.value].text.Contains("Request"))
            OpenMeleeBreakEngagementRequestUI();
    }
    public void ClearMeleeUI()
    {
        clearMeleeFlag = true;
        meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        meleeUI.transform.Find("MeleeType").Find("MeleeTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        meleeUI.transform.Find("AttackerWeapon").Find("WeaponDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        meleeUI.transform.Find("AttackerWeapon").Find("WeaponDropdown").GetComponent<TMP_Dropdown>().value = 0;
        meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").Find("WeaponDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        meleeUI.transform.Find("TargetPanel").Find("DefenderWeapon").Find("WeaponDropdown").GetComponent<TMP_Dropdown>().value = 0;
        meleeUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        meleeUI.transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().value = 0;
        game.ClearFlankersUI(flankersMeleeAttackerUI);
        game.ClearFlankersUI(flankersMeleeDefenderUI);
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
                    $"| {game.meleeParameters.Find(tuple => tuple.Item1 == "dStr")}";

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
            Debug.Log("Haven't scrolled all the way to the bottom");
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

        Soldier closestAlly = game.FindClosestAlly(true);

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
        if (activeSoldier.IsPlanner() && game.FindClosestAlly(true) != null && !activeSoldier.usedMP)
            moveTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Planner Donate</color>"));
        if (activeSoldier.inventory.IsWearingExoArmour())
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
        if (activeSoldier.inventory.IsWearingJuggernautArmour())
            coverToggle.interactable = false;

        //block melee toggle if within engage distance of enemy
        if (game.FindClosestEnemy(true) != null && activeSoldier.PhysicalObjectWithinMeleeRadius(game.FindClosestEnemy(true)) || suppressed)
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
        moveUI.transform.Find("Fall").Find("FallInput").GetComponent<TMP_InputField>().text = "";
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
    public void AddAllyItemButtons()
    {
        foreach (Soldier s in allSoldiers)
        {
            if (activeSoldier.IsSameTeamAs(s) && s.IsFielded() && (!s.IsMeleeControlled() || s.IsUnconscious()))
            {
                if (activeSoldier.PhysicalObjectWithinItemRadius(s) && s.IsInteractable())
                    Instantiate(allyItemsButtonPrefab.GetComponent<AllyItemsButton>().Init(s, Instantiate(allyInventoryPanelPrefab.GetComponent<AllyItemsPanel>().Init(s), externalItemSourcesUI.transform).GetComponent<AllyItemsPanel>()).gameObject, allyItemButtonUI.transform);
            }
        }
    }
    public void OpenConfigureUI()
    {
        Transform inventoryUI = configureUI.transform.Find("ItemDisplayPanel").Find("Inventory").Find("InventoryPanel").Find("Viewport").Find("InventoryContent");
        Transform groundItemsUI = configureUI.transform.Find("ExternalItemSources").Find("GroundItemsPanel").Find("Viewport").Find("GroundItemsContent");

        //populate inventory icons
        foreach (Item i in activeSoldier.inventory.Items)
            Instantiate(itemIconPrefab, inventoryUI).Init(i.itemName, 1, i);

        //populate ground item icons
        foreach (Item i in game.FindNearbyItems())
            Instantiate(itemIconPrefab, groundItemsUI).Init(i.itemName, 1, i);

        AddAllyItemButtons();
        configureUI.SetActive(true);
    }
    public void ClearConfigureUI()
    {
        //reset ap counter to 0
        configureUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "0";
        
        //reset all pickup counters to 0 for all items menu
        Transform allItemsUI = configureUI.transform.Find("ExternalItemSources").Find("AllItemsPanel").Find("Viewport").Find("AllItemsContent");
        
        foreach (Transform child in allItemsUI)
            if (child.GetComponent<ItemIcon>() != null)
                child.GetComponent<ItemIcon>().pickupNumber = 0;

        //clear all display panels
        Transform inventoryItemsUI = configureUI.transform.Find("ItemDisplayPanel").Find("Inventory").Find("InventoryPanel").Find("Viewport").Find("InventoryContent");
        Transform groundItemsUI = configureUI.transform.Find("ExternalItemSources").Find("GroundItemsPanel").Find("Viewport").Find("GroundItemsContent");
        Transform allyButtonPanel = configureUI.transform.Find("ExternalItemSources").Find("AllyButtonPanel");
        List<AllyItemsPanel> allyInventoryPanels = configureUI.transform.Find("ExternalItemSources").GetComponentsInChildren<AllyItemsPanel>().ToList();

        ClearItemPanel(inventoryItemsUI);
        ClearItemPanel(groundItemsUI);
        ClearItemPanel(allyButtonPanel);
        foreach (AllyItemsPanel allyPanel in allyInventoryPanels)
            Destroy(allyPanel.gameObject);

        //close all open display panels
        GameObject allItemsPanelUI = configureUI.transform.Find("ExternalItemSources").Find("AllItemsPanel").gameObject;
        GameObject groundItemsPanelUI = configureUI.transform.Find("ExternalItemSources").Find("GroundItemsPanel").gameObject;

        CloseItemPanel(allItemsPanelUI);
        CloseItemPanel(groundItemsPanelUI);
    }
    public void ClearItemPanel(Transform itemPanel)
    {
        foreach (Transform child in itemPanel)
            Destroy(child.gameObject);
    }
    public void OpenItemPanel(GameObject itemPanel)
    {
        itemPanel.SetActive(true);
    }
    public void OpenAllyItemPanel(GameObject itemPanel, Soldier ally)
    {
        game.activeItemPanel = itemPanel.transform;

        if (itemPanel.transform.Find("Viewport").Find("AllyInventoryContent").childCount == 0)
        {
            foreach (Item i in ally.inventory.Items)
                Instantiate(itemIconPrefab, itemPanel.transform.Find("Viewport").Find("AllyInventoryContent")).Init(i.itemName, 1, i);
        }       

        itemPanel.SetActive(true);
    }
    public void CloseItemPanel(GameObject itemPanel)
    {
        game.activeItemPanel = null;
        itemPanel.SetActive(false);
    }
    public void CloseConfigureUI()
    {
        ClearConfigureUI();
        configureUI.SetActive(false);
    }











    //dipelec functions
    public void OpenDipElecUI()
    {
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
        if (OverrideKey())
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
    }
    public void ClearDamageEventUI()
    {
        clearDamageEventFlag = true;
        damageEventUI.transform.Find("DamageEventType").Find("DamageEventTypeDropdown").GetComponent<TMP_Dropdown>().value = 0;
        damageEventUI.transform.Find("FallDistance").Find("FallInput").GetComponent<TMP_InputField>().text = "";
        damageEventUI.transform.Find("StructureHeight").Find("StructureHeightInput").GetComponent<TMP_InputField>().text = "";
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
                Debug.Log(soldier.soldierName + " cannot recieve xp unconscious.");
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
            Debug.Log("Haven't scrolled all the way to the bottom");
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









    //insert game objects function
    public void OpenOverrideInsertObjectsUI()
    {
        overrideInsertObjectsUI.SetActive(true);
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
}
