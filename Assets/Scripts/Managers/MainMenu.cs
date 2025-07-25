using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IDataPersistence
{
    //secret override key
    public KeyCode overrideKey = KeyCode.LeftShift;
    public KeyCode secondOverrideKey = KeyCode.Space;
    public KeyCode deathKey = KeyCode.D;

    public SoldierManager soldierManager;
    public ItemManager itemManager;
    public MainGame game;
    public WeatherGen weather;
    public DipelecGen dipelec;
    public POIManager poiManager;
    public SoundManager soundManager;
    public TextMeshProUGUI gameTimer, turnTimer, roundIndicator, teamTurnIndicator, turnTitle;

    public DetectionUI detectionUI;
    public MoveUI moveUI;
    public ShotUI shotUI;
    public MeleeUI meleeUI;
    public ConfigureUI configUI;
    public DipElecUI dipelecUI;
    public DamageEventUI damageEventUI;
    public OverwatchUI overwatchUI;
    public DragUI dragUI;
    public InsertObjectsUI insertObjectsUI;
    public OverwatchShotUI overwatchShotUI;
    public GeneralAlertUI generalAlertUI;
    public BinocularsUI binocularsUI;

    public GameObject menuUI, weatherUI, teamTurnOverUI, teamTurnStartUI, soldierOptionsUI, soldierStatsUI, flankersShotUI, shotConfirmUI, shotResultUI, overmoveUI, suppressionMoveUI, meleeBreakEngagementRequestUI, meleeResultUI, meleeConfirmUI, dipelecResultUI, overrideUI, detectionAlertUI, lostLosUI, damageUI, traumaAlertUI, traumaUI, explosionUI, inspirerUI, xpAlertUI, xpLogUI, promotionUI, lastandicideConfirmUI, brokenFledUI, endSoldierTurnAlertUI, playdeadAlertUI, coverAlertUI, inventorySourceIconsUI, lostLosAlertPrefab, losGlimpseAlertPrefab, inspirerAlertPrefab, allyInventoryIconPrefab, groundInventoryIconPrefab, gbInventoryIconPrefab, dcInventoryIconPrefab, globalInventoryIconPrefab, soldierSnapshotPrefab, soldierPortraitPrefab, possibleFlankerPrefab, meleeAlertPrefab, dipelecRewardPrefab, explosionListPrefab, explosionAlertPrefab, explosionAlertPOIPrefab, explosionAlertItemPrefab, endTurnButton, enterOverrideButton, exitOverrideButton, overrideVersionDisplay, overrideVisibilityDropdown, overrideWindSpeedDropdown, overrideWindDirectionDropdown, overrideRainDropdown, overrideInsertObjectsButton, muteIcon, timeStopIcon, undoButton, blockingScreen, itemSlotPrefab, itemIconPrefab, cannotUseItemUI, useItemUI, dropThrowItemUI, dropUI, throwUI, etoolResultUI, grenadeUI, claymoreUI, deploymentBeaconUI, thermalCamUI, useULFUI, ULFResultUI, UHFUI, riotShieldUI, disarmUI, politicianUI, cloudDissipationAlertPrefab;
    
    public SoldierAlert soldierAlertPrefab;
    public XpAlert xpAlertPrefab;
    public PromotionAlert promotionAlertPrefab;
    public TraumaAlert traumaAlertPrefab;

    public InventorySourcePanel inventoryPanelGroundPrefab, inventoryPanelAllyPrefab, inventoryPanelGoodyBoxPrefab;
    public ItemIconGB gbItemIconPrefab;
    public LOSArrow LOSArrowPrefab;
    public OverwatchSectorSphere overwatchSectorSpherePrefab;
    public List<Button> actionButtons;
    public Button shotButton, moveButton, meleeButton, configureButton, dipElecButton, overwatchButton, coverButton, playdeadButton, disarmButton, dragButton, lastandicideButton, politicsButton;
    private float playTimeTotal;
    public float turnTime;
    public string meleeChargeIndicator;
    public Soldier activeSoldier;
    public bool timerStop, overrideView, clearShotFlag, clearMeleeFlag, clearDipelecFlag, clearMoveFlag, detectionResolvedFlag, meleeResolvedFlag, shotResolvedFlag, binocularsFlashResolvedFlag, explosionResolvedFlag, inspirerResolvedFlag, xpResolvedFlag, clearDamageEventFlag, teamTurnOverFlag, teamTurnStartFlag, onItemUseScreen, inventorySourceViewOnly;
    public TMP_InputField LInput, HInput, RInput, SInput, EInput, FInput, PInput, CInput, SRInput, RiInput, ARInput, LMGInput, SnInput, SMGInput, ShInput, MInput, StrInput, DipInput, ElecInput, HealInput;
    public Sprite fist, explosiveBarrelSprite, goodyBoxSprite, terminalSprite, drugCabinetSprite, covermanSprite;

    private readonly string[][] allStats =
    {
        new string[] { "L", "Leadership" },
        new string[] { "H", "Health" },
        new string[] { "R", "Resilience" },
        new string[] { "S", "Speed" },
        new string[] { "E", "Evasion" },
        new string[] { "F", "Fight" },
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
        new string[] { "Brawler", "Pugilist" },
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
        { "Reservist (F)", "Fight", "F" },
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

        isDataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        data.playTimeTotal = playTimeTotal;
        data.turnTime = turnTime;
    }
    //navigation functions - menu
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        //check the game is running
        if (game.GameRunning)
        {
            //check if game has not run out of turns
            if (game.currentRound <= game.maxRounds)
            {
                //always count play time
                playTimeTotal += Time.unscaledDeltaTime;

                //don't count turn time if timerstopped
                if (!TimerStop)
                    turnTime += Time.deltaTime;

                DisplayWeather();
            }
            if (!game.gameOver)
            {
                DisplaySoldiers();
                RenderSoldierVisuals();
                CheckWinConditions();
            }

            //check for game mute
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (OverrideKey())
                    ToggleMute();
            }

            if (!OverrideView)
            {
                //show/hide end turn button
                if (activeSoldier == null)
                    endTurnButton.SetActive(true);
                else
                    endTurnButton.SetActive(false);
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
        }
    }
    












    //helper functions - menu
    public bool OverrideKey()
    {
        if (Input.GetKey(overrideKey))
            return true; 
        return false;
    }
    public bool SecondOverrideKey()
    {
        if (Input.GetKey(secondOverrideKey))
            return true;
        return false;
    }
    public bool SecondOverrideKeyDown()
    {
        if (Input.GetKeyDown(secondOverrideKey))
            return true;
        return false;
    }
    public bool SecondOverrideKeyUp()
    {
        if (Input.GetKeyUp(secondOverrideKey))
            return true;
        return false;
    }
    public bool DeathKey()
    {
        if (Input.GetKey(deathKey))
            return true;
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
                RevealSightRadiusSpheres();
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
                DestroyLOSArrows();
                HideSightRadiusSphere();
                
            }
        }
    }
    public void DisplayGMObjects()
    {
        var GMObjects = FindObjectsByType<GMObject>(FindObjectsInactive.Include, default);

        foreach (GMObject obj in GMObjects)
            obj.gameObject.SetActive(true);
    }
    public void HideGMObjects()
    {
        var GMObjects = FindObjectsByType<GMObject>(FindObjectsInactive.Include, default);

        foreach (GMObject obj in GMObjects)
            obj.gameObject.SetActive(false);
    }
    public void CreateLOSArrows()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            foreach (string id in s.LOSToTheseSoldiersAndRevealing)
            {
                LOSArrow arrow = Instantiate(LOSArrowPrefab).Init(s, soldierManager.FindSoldierById(id));
                arrow.transform.SetAsLastSibling();
            }
        }
    }
    public void CreateLOSArrowPair(Soldier s1, Soldier s2)
    {
        LOSArrow arrow = Instantiate(LOSArrowPrefab).Init(s1, s2);
        arrow.transform.SetAsLastSibling();
    }
    public void CreateLOSArrowPair(Soldier s1, POI p1)
    {
        LOSArrow arrow = Instantiate(LOSArrowPrefab).Init(s1, p1);
        arrow.transform.SetAsLastSibling();
    }
    public void DestroyLOSArrowPair(Soldier s1, Soldier s2)
    {
        var LOSArrows = FindObjectsByType<LOSArrow>(FindObjectsInactive.Include, default);
        foreach (LOSArrow arrow in LOSArrows)
            if (arrow.from == s1 && arrow.to == s2)
                Destroy(arrow.gameObject);
    }
    public void DestroyLOSArrowPair(Soldier s1, POI p1)
    {
        var LOSArrows = FindObjectsByType<LOSArrow>(FindObjectsInactive.Include, default);
        foreach (LOSArrow arrow in LOSArrows)
            if (arrow.from == s1 && arrow.to == p1)
                Destroy(arrow.gameObject);
    }
    public void DestroyLOSArrows()
    {
        var LOSArrows = FindObjectsByType<LOSArrow>(FindObjectsInactive.Include, default);
        foreach (LOSArrow arrow in LOSArrows)
            Destroy(arrow.gameObject);
    }
    public void DestroyOverwatchSectors()
    {
        var overwatchSectors = FindObjectsByType<OverwatchSectorSphere>(FindObjectsInactive.Include, default);
        foreach (OverwatchSectorSphere overwatchSector in overwatchSectors)
            Destroy(overwatchSector.gameObject);
    }
    public void RevealSightRadiusSpheres()
    {
        if (activeSoldier != null)
        {
            activeSoldier.SRColliderMinRenderer.enabled = true;
            activeSoldier.SRColliderHalfRenderer.enabled = true;
            activeSoldier.SRColliderFullRenderer.enabled = true;

            if (activeSoldier.IsOnOverwatch())
                Instantiate(overwatchSectorSpherePrefab, activeSoldier.transform).Init(activeSoldier);
        }
    }
    public void HideSightRadiusSphere()
    {
        if (activeSoldier != null)
        {
            activeSoldier.SRColliderMinRenderer.enabled = false;
            activeSoldier.SRColliderHalfRenderer.enabled = false;
            activeSoldier.SRColliderFullRenderer.enabled = false;

            DestroyOverwatchSectors();
        }
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
    public void FreezeTimer(bool force = false)
    {
        if (force || !OverrideView)
        {
            TimerStop = true;
            timeStopIcon.SetActive(true);
        }
    }
    public void UnfreezeTimer(bool force = false)
    {
        if (force || !OverrideView)
        {
            TimerStop = false;
            timeStopIcon.SetActive(false);
        }
    }
    public void SetDetectionResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
            FreezeTimer();

        detectionResolvedFlag = value;
    }
    public void SetXpResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
            FreezeTimer();

        xpResolvedFlag = value;
    }
    public void SetMeleeResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
            FreezeTimer();

        meleeResolvedFlag = value;
    }
    public void SetShotResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
            FreezeTimer();

        shotResolvedFlag = value;
    }
    public void SetExplosionResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
            FreezeTimer();

        explosionResolvedFlag = value;
    }
    public void SetInspirerResolvedFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
            FreezeTimer();

        inspirerResolvedFlag = value;
    }
    public void SetTeamTurnOverFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
        {
            FreezeTimer();
            OpenPlayerTurnOverUI();
        }

        teamTurnOverFlag = value;
    }
    public void SetTeamTurnStartFlagTo(bool value)
    {
        if (value)
            UnfreezeTimer();
        else
        {
            FreezeTimer();
            OpenPlayerTurnStartUI();
        }

        teamTurnStartFlag = value;
    }
    public bool MovementResolvedFlag()
    {
        foreach (Soldier s in game.AllFieldedSoldiers())
        {
            if (!s.moveResolvedFlag)
                return false;
        }
        return true;
    }
    public void ToggleMute()
    {
        muteIcon.SetActive(!muteIcon.activeSelf);
        soundManager.isMute = !soundManager.isMute;
    }
    public void Mute()
    {
        muteIcon.SetActive(true);
        soundManager.isMute = true;
    }
    public void UnMute()
    {
        muteIcon.SetActive(false);
        soundManager.isMute = false;
    }
    
    public string DisplayShotParameters()
    {
        List<string> colouredParameters = new();
        foreach (Tuple<string,string> param in game.shotParameters)
        {

            if (param.Item1 == "accuracy" || param.Item1 == "sharpshooter" || param.Item1 == "inspired" || param.Item1 == "WS" || param.Item1 == "stim" || param.Item1 == "juggernaut" || param.Item1 == "fight")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "tE" || param.Item1 == "suppression")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else
            {
                if (float.Parse(param.Item2) > 1)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 1)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
        }

        return $"{colouredParameters.Find(str => str.Contains("accuracy"))} " +
                $"| {colouredParameters.Find(str => str.Contains("sharpshooter"))} " +
                $"| {colouredParameters.Find(str => str.Contains("inspired"))} " +
                $"| {colouredParameters.Find(str => str.Contains("WS"))} " +
                $"| {colouredParameters.Find(str => str.Contains("juggernaut"))} " +
                $"| {colouredParameters.Find(str => str.Contains("stim"))} " +
                $"| {colouredParameters.Find(str => str.Contains("trauma"))} " +
                $"| {colouredParameters.Find(str => str.Contains("sustenance"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tE"))} " +
                $"| {colouredParameters.Find(str => str.Contains("cover"))} " +
                $"| {colouredParameters.Find(str => str.Contains("vis"))} " +
                $"| {colouredParameters.Find(str => str.Contains("rain"))} " +
                $"| {colouredParameters.Find(str => str.Contains("wind"))} " +
                $"| {colouredParameters.Find(str => str.Contains("HP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tHP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("Ter"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tTer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("elevation"))} " +
                $"| {colouredParameters.Find(str => str.Contains("kd"))} " +
                $"| {colouredParameters.Find(str => str.Contains("overwatch"))} " +
                $"| {colouredParameters.Find(str => str.Contains("flank"))} " +
                $"| {colouredParameters.Find(str => str.Contains("stealth"))} " +
                $"| {colouredParameters.Find(str => str.Contains("smoke"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tabun"))} " +
                $"| {colouredParameters.Find(str => str.Contains("suppression"))} " +
                $"| {colouredParameters.Find(str => str.Contains("fight"))}";
    }
    public string DisplayMeleeParameters()
    {
        List<string> colouredParameters = new();
        foreach (Tuple<string, string> param in game.meleeParameters)
        {

            if (param.Item1 == "aM" || param.Item1 == "aJuggernaut" || param.Item1 == "aInspirer" || param.Item1 == "aWep" || param.Item1 == "aStr" || param.Item1 == "aFight")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "dM" || param.Item1 == "dJuggernaut" || param.Item1 == "dWep" || param.Item1 == "charge" || param.Item1 == "dStr" || param.Item1 == "dFight")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "aSustenance" || param.Item1 == "aHP" || param.Item1 == "aTer" || param.Item1 == "aFlank" || param.Item1 == "kd" || param.Item1 == "aSuppression" || param.Item1 == "bloodrage")
            {
                if (float.Parse(param.Item2) > 1)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 1)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "dSustenance" || param.Item1 == "dHP" || param.Item1 == "dTer" || param.Item1 == "dFlank" || param.Item1 == "dSuppression")
            {
                if (float.Parse(param.Item2) < 1)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) > 1)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
        }

        return $"{colouredParameters.Find(str => str.Contains("aM"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aJuggernaut"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aInspirer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aSustenance"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aWep"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aHP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aTer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aFlank"))} " +
                $"| {colouredParameters.Find(str => str.Contains("kd"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aSuppression"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aStr"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dM"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dJuggernaut"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dInspirer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dSustenance"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dWep"))} " +
                $"| {colouredParameters.Find(str => str.Contains("charge"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dHP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dTer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dFlank"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dSuppression"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dStr"))} " +
                $"| {colouredParameters.Find(str => str.Contains("dFight"))} " +
                $"| {colouredParameters.Find(str => str.Contains("aFight"))} " +
                $"| {colouredParameters.Find(str => str.Contains("bloodrage"))}";
    }











    //override functions - menu
    public void OpenOverrideMenu()
    {
        overrideUI.SetActive(true);
    }
    public void CloseOverrideMenu()
    {
        overrideUI.SetActive(false);
    }
    public void ConfirmEnterOverride()
    {
        if (OverrideKey())
        {
            CloseOverrideMenu();
            SetOverrideView();
        }
    }
    public void ConfirmExitOverride()
    {
        UnsetOverrideView();
    }
    public void SetOverrideView()
    {
        if (!OverrideView)
        {
            //enter override
            OverrideView = true;

            soundManager.PlayOverrideAlarm(); //play override alarm sfx

            endTurnButton.SetActive(false);
            enterOverrideButton.SetActive(false);

            exitOverrideButton.SetActive(true);
            overrideVersionDisplay.SetActive(true);
            overrideInsertObjectsButton.SetActive(true);
            GetOverrideWeather();
            Mute();
        }
    }
    public void UnsetOverrideView()
    {
        if (OverrideView)
        {
            //exit override
            OverrideView = false;

            exitOverrideButton.SetActive(false);
            overrideVersionDisplay.SetActive(false);
            overrideInsertObjectsButton.SetActive(false);
            HideOverrideWeather();

            endTurnButton.SetActive(true);
            enterOverrideButton.SetActive(true);
            
            UnMute();
        }
    }
    public void GetOverrideHealthState(Transform soldierStatsUI)
    {
        TMP_Dropdown dropdown = soldierStatsUI.Find("General").Find("OverrideHealthState").Find("HealthStateDropdown").GetComponent<TMP_Dropdown>();

        if (activeSoldier.IsDead())
            dropdown.value = 3;
        else if (activeSoldier.IsUnconscious())
            dropdown.value = 2;
        else if (activeSoldier.IsLastStand())
            dropdown.value = 1;
        else
            dropdown.value = 0;
    }
    public void SetOverrideHealthState()
    {
        TMP_Dropdown dropdown = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideHealthState").Find("HealthStateDropdown").GetComponent<TMP_Dropdown>();
        FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} health state changed"); //write to report

        if (dropdown.value == 3)
        {
            if (!activeSoldier.IsDead())
                activeSoldier.InstantKill(null, new() { "Override" });
        }
        else if (dropdown.value == 2)
        {
            if (!activeSoldier.IsUnconscious())
                activeSoldier.MakeUnconscious(null, new() { "Override" });
        }
        else if (dropdown.value == 1)
        {
            if (!activeSoldier.IsLastStand())
                activeSoldier.MakeLastStand();
        }
        else if (dropdown.value == 0)
        {
            if (!activeSoldier.IsActive())
                activeSoldier.MakeActive();
        }
    }
    public void GetOverrideTerrainOn(Transform soldierStatsUI)
    {
        TMP_Dropdown dropdown = soldierStatsUI.Find("General").Find("OverrideTerrainOn").Find("TerrainDropdown").GetComponent<TMP_Dropdown>();

        if (activeSoldier.TerrainOn.Equals("Alpine"))
            dropdown.value = 0;
        else if (activeSoldier.TerrainOn.Equals("Desert"))
            dropdown.value = 1;
        else if (activeSoldier.TerrainOn.Equals("Jungle"))
            dropdown.value = 2;
        else if(activeSoldier.TerrainOn.Equals("Urban"))
            dropdown.value = 3;
    }
    public void SetOverrideTerrainOn()
    {
        TMP_Dropdown dropdown = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideTerrainOn").Find("TerrainDropdown").GetComponent<TMP_Dropdown>();
        string terrainOnString = dropdown.value switch
        {
            0 => "Alpine",
            1 => "Desert",
            2 => "Jungle",
            3 => "Urban",
            _ => "Unknown",
        };
        FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} terrain on changed from {activeSoldier.TerrainOn} to {terrainOnString}"); //write to report
        
        activeSoldier.TerrainOn = terrainOnString;
    }
    public void HideOverrideWeather()
    {
        overrideVisibilityDropdown.SetActive(false);
        overrideWindSpeedDropdown.SetActive(false);
        overrideWindDirectionDropdown.SetActive(false);
        overrideRainDropdown.SetActive(false);
    }
    public void GetOverrideWeather()
    {
        overrideVisibilityDropdown.SetActive(true);
        overrideWindSpeedDropdown.SetActive(true);
        overrideWindDirectionDropdown.SetActive(true);
        overrideRainDropdown.SetActive(true);

        GetOverrideVisibility();
        GetOverrideWindSpeed();
        GetOverrideWindDirection();
        GetOverrideRain();
    }
    public void GetOverrideVisibility()
    {
        TMP_Dropdown dropdown = overrideVisibilityDropdown.GetComponent<TMP_Dropdown>();
        dropdown.captionText.text = weather.CurrentVis;
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
    public void GetOverrideWindSpeed()
    {
        TMP_Dropdown dropdown = overrideWindSpeedDropdown.GetComponent<TMP_Dropdown>();
        dropdown.captionText.text = weather.CurrentWindSpeed;
        dropdown.value = weather.CurrentWindSpeed switch
        {
            "Strong" => 0,
            "Moderate" => 1,
            "Light" => 2,
            "Zero" or _ => 3,
        };

    }
    public void GetOverrideWindDirection()
    {
        TMP_Dropdown dropdown = overrideWindDirectionDropdown.GetComponent<TMP_Dropdown>();
        dropdown.captionText.text = weather.CurrentWindDirection;
        dropdown.value = weather.CurrentWindDirection switch
        {
            "North-Eastern" => 0,
            "South-Eastern" => 1,
            "Eastern" => 2,
            "North-Western" => 3,
            "South-Western" => 4,
            "Western" => 5,
            "Northern" => 6,
            "Southern" => 7,
            "Zero" or _ => 8,
        };
    }
    public void GetOverrideRain()
    {
        TMP_Dropdown dropdown = overrideRainDropdown.GetComponent<TMP_Dropdown>();
        dropdown.captionText.text = weather.CurrentRain;
        dropdown.value = weather.CurrentRain switch
        {
            "Torrential" => 0,
            "Heavy" => 1,
            "Moderate" => 2,
            "Light" => 3,
            "Zero" or _ => 4,
        };
    }
    public void SetOverrideVisibility()
    {
        TMP_Dropdown dropdown = overrideVisibilityDropdown.GetComponent<TMP_Dropdown>();
        FileUtility.WriteToReport($"(Override) Weather changed: Visibility changed from {weather.CurrentVis} to {dropdown.captionText.text}"); //write to report

        string oldVis = weather.CurrentVis;
        weather.CurrentVis = dropdown.captionText.text;
        
        if (!weather.CurrentVis.Equals(oldVis))
            game.SetLosCheckAll("statChange(SR)|weatherChange(override)"); //loscheckall
    }
    public void SetOverrideWindSpeed()
    {
        TMP_Dropdown dropdown = overrideWindSpeedDropdown.GetComponent<TMP_Dropdown>();
        FileUtility.WriteToReport($"(Override) Weather changed: Wind speed changed from {weather.CurrentWindSpeed} to {dropdown.captionText.text}"); //write to report
        weather.CurrentWindSpeed = dropdown.captionText.text;
    }
    public void SetOverrideWindDirection()
    {
        TMP_Dropdown dropdown = overrideWindDirectionDropdown.GetComponent<TMP_Dropdown>();
        FileUtility.WriteToReport($"(Override) Weather changed: Wind direction changed from {weather.CurrentWindDirection} to {dropdown.captionText.text}"); //write to report
        weather.CurrentWindDirection = dropdown.captionText.text;
    }
    public void SetOverrideRain()
    {
        TMP_Dropdown dropdown = overrideRainDropdown.GetComponent<TMP_Dropdown>();
        FileUtility.WriteToReport($"(Override) Weather changed: Rain intensity changed from {weather.CurrentRain} to {dropdown.captionText.text}"); //write to report
        weather.CurrentRain = dropdown.captionText.text;
    }
    public void ChangeHP()
    {
        TMP_InputField hpInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("OverrideHP").GetComponent<TMP_InputField>();
        
        if (int.TryParse(hpInput.text, out int newHp) && newHp >= 0)
        {
            FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} hp changed from {activeSoldier.hp} to {newHp}"); //write to report

            if (newHp > 0)
            {
                if (activeSoldier.hp == 0)
                    activeSoldier.Resurrect(newHp);
                else
                {
                    if (newHp < activeSoldier.hp)
                        activeSoldier.TakeDamage(null, activeSoldier.hp - newHp, true, new() { "Override" }, Vector3.zero);
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
        
        if (int.TryParse(apInput.text, out int newAp) && newAp >= 0)
        {
            FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} ap changed from {activeSoldier.ap} to {newAp}"); //write to report

            activeSoldier.usedAP = false;
            activeSoldier.ap = newAp; 
        }

        apInput.text = "";
    }
    public void ChangeMP()
    {
        TMP_InputField mpInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("OverrideMP").GetComponent<TMP_InputField>();
        
        if (int.TryParse(mpInput.text, out int newMp) && newMp >= 0)
        {
            FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} mp changed from {activeSoldier.mp} to {newMp}"); //write to report

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
                FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} xp changed from {activeSoldier.xp} to {newXp}"); //write to report

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
            FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} base {code} changed from {activeSoldier.stats.GetStat(code).BaseVal} to {newBaseVal}"); //write to report

            activeSoldier.stats.SetStat(code, newBaseVal);

            //recalculate stats
            activeSoldier.CalculateActiveStats();

            //set los check if P, C, SR is changed
            if (code == "P" || code == "C" || code == "SR")
                activeSoldier.SetLosCheck($"statChange({code})|baseStatChange(override)"); //losCheck

            //run melee control re-eval if R, Str, M, F is changed
            if (activeSoldier.IsMeleeEngaged() && (code == "R" || code == "Str" || code == "M" || code == "F"))
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
            FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} abilities changed from {HelperFunctions.PrintList(activeSoldier.soldierAbilities)} to {HelperFunctions.PrintList(abilityList)}"); //write to report

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
                activeSoldier.SetLosCheck("losChange|move(override)"); //losCheck

                if (xyz.Equals("X"))
                {
                    FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} location changed from {activeSoldier.X}, {activeSoldier.Y}, {activeSoldier.Z} to {newlocationInput}, {activeSoldier.Y}, {activeSoldier.Z}"); //write to report
                    activeSoldier.X = newlocationInput;
                }
                else if (xyz.Equals("Y"))
                {
                    FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} location changed from {activeSoldier.X}, {activeSoldier.Y}, {activeSoldier.Z} to {activeSoldier.X}, {newlocationInput}, {activeSoldier.Z}"); //write to report
                    activeSoldier.Y = newlocationInput;
                }
                else
                {
                    FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} location changed from {activeSoldier.X}, {activeSoldier.Y}, {activeSoldier.Z} to {activeSoldier.X}, {activeSoldier.Z}, {newlocationInput}"); //write to report
                    activeSoldier.Z = newlocationInput;
                }
            }
        }

        locationInput.text = string.Empty;
    }
    public void ChangeRoundsWithoutFood()
    {
        TMP_InputField roundsWithoutFoodInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideRoundsWithoutFood").GetComponent<TMP_InputField>();
        
        if (int.TryParse(roundsWithoutFoodInput.text, out int newRoundsWithoutFood) && newRoundsWithoutFood >= 0)
        {
            FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} rounds without food changed from {activeSoldier.RoundsWithoutFood} to {newRoundsWithoutFood}"); //write to report

            activeSoldier.RoundsWithoutFood = newRoundsWithoutFood;
        }

        roundsWithoutFoodInput.text = "";
    }
    public void ChangeTraumaPoints()
    {
        TMP_InputField traumaInput = soldierOptionsUI.transform.Find("SoldierBanner").Find("SoldierStatsUI").Find("General").Find("OverrideTraumaPoints").GetComponent<TMP_InputField>();
        
        if (int.TryParse(traumaInput.text, out int newTrauma) && newTrauma >= 0)
        {
            FileUtility.WriteToReport($"(Override) {activeSoldier.soldierName} trauma points changed from {activeSoldier.tp} to {newTrauma}"); //write to report

            activeSoldier.tp = 0;
            activeSoldier.TakeTrauma(newTrauma);
        }

        traumaInput.text = "";
    }







    //display functions - menu
    public void DisplayWeather()
    {
        TMP_Dropdown allVis = FindFirstObjectByType<AllWeatherIcons>().allVisIcons;
        TMP_Dropdown allWind = FindFirstObjectByType<AllWeatherIcons>().allWindIcons;
        TMP_Dropdown allWindDirection = FindFirstObjectByType<AllWeatherIcons>().allWindDirectionIcons;
        TMP_Dropdown allRain = FindFirstObjectByType<AllWeatherIcons>().allRainIcons;

        Transform weatherIcons = weatherUI.transform.Find("WeatherIcons");
        Transform experimentalistWeatherIcons = weatherUI.transform.Find("ExperimentalistWeatherIcons");

        weatherIcons.Find("Vis").GetComponent<Image>().sprite = weather.CurrentVis switch
        {
            "Zero" => allVis.options[0].image,
            "Poor" => allVis.options[1].image,
            "Moderate" => allVis.options[2].image,
            "Good" => allVis.options[3].image,
            "Full" or _ => allVis.options[4].image,
        };

        weatherIcons.Find("Wind").Find("Direction").gameObject.SetActive(true);
        weatherIcons.Find("Wind").GetComponent<Image>().sprite = weather.CurrentWindSpeed switch
        {
            "Strong" => allWind.options[0].image,
            "Moderate" => allWind.options[1].image,
            "Light" => allWind.options[2].image,
            "Zero" or _ => allWind.options[3].image,
        };
        if (weather.CurrentWindSpeed.Equals("Zero"))
            weatherIcons.Find("Wind").Find("Direction").gameObject.SetActive(false);
        else
        {
            weatherIcons.Find("Wind").Find("Direction").GetComponent<Image>().sprite = weather.CurrentWindDirection switch
            {
                "North-Eastern" => allWindDirection.options[0].image,
                "South-Eastern" => allWindDirection.options[1].image,
                "Eastern" => allWindDirection.options[2].image,
                "North-Western" => allWindDirection.options[3].image,
                "South-Western" => allWindDirection.options[4].image,
                "Western" => allWindDirection.options[5].image,
                "Northern" => allWindDirection.options[6].image,
                "Southern" => allWindDirection.options[7].image,
                "Zero" or _ => allWindDirection.options[8].image,
            };
        }

        weatherIcons.Find("Rain").GetComponent<Image>().sprite = weather.CurrentRain switch
        {
            "Torrential" => allRain.options[0].image,
            "Heavy" => allRain.options[1].image,
            "Moderate" => allRain.options[2].image,
            "Light" => allRain.options[3].image,
            "Zero" or _ => allRain.options[4].image,
        };

        experimentalistWeatherIcons.gameObject.SetActive(false);
        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsOnturnAndAlive() && s.IsExperimentalist())
            {
                experimentalistWeatherIcons.Find("Vis").GetComponent<Image>().sprite = weather.NextTurnVis switch
                {
                    "Zero" => allVis.options[0].image,
                    "Poor" => allVis.options[1].image,
                    "Moderate" => allVis.options[2].image,
                    "Good" => allVis.options[3].image,
                    "Full" or _ => allVis.options[4].image,
                };

                experimentalistWeatherIcons.Find("Wind").GetComponent<Image>().sprite = weather.NextTurnWindSpeed switch
                {
                    "Strong" => allWind.options[0].image,
                    "Moderate" => allWind.options[1].image,
                    "Light" => allWind.options[2].image,
                    "Zero" or _ => allWind.options[3].image,
                };

                experimentalistWeatherIcons.Find("Wind").Find("Direction").GetComponent<Image>().sprite = weather.NextTurnWindDirection switch
                {
                    "North-Eastern" => allWindDirection.options[0].image,
                    "South-Eastern" => allWindDirection.options[1].image,
                    "Eastern" => allWindDirection.options[2].image,
                    "North-Western" => allWindDirection.options[3].image,
                    "South-Western" => allWindDirection.options[4].image,
                    "Western" => allWindDirection.options[5].image,
                    "Northern" => allWindDirection.options[6].image,
                    "Southern" => allWindDirection.options[7].image,
                    "Zero" or _ => allWindDirection.options[8].image,
                };

                experimentalistWeatherIcons.Find("Rain").GetComponent<Image>().sprite = weather.NextTurnRain switch
                {
                    "Torrential" => allRain.options[0].image,
                    "Heavy" => allRain.options[1].image,
                    "Moderate" => allRain.options[2].image,
                    "Light" => allRain.options[3].image,
                    "Zero" or _ => allRain.options[4].image,
                };

                experimentalistWeatherIcons.gameObject.SetActive(true);
            }
        }
    }
    
    public void DisplaySoldiers()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            if (OverrideView)
            {
                s.soldierUI.gameObject.SetActive(true);
                s.soldierUI.actionButton.interactable = true;
                s.soldierUI.revealMessage.SetActive(false);
                s.soldierUI.revealMessageText.text = string.Empty;
            }
            else
            {
                //set visibility of button
                if (s.IsOnturn() || (s.IsOffturn() && (s.IsRevealed() || s.IsSpotted()) || s.IsDead() || s.IsPlayingDead())) 
                    s.soldierUI.gameObject.SetActive(true);
                else
                    s.soldierUI.gameObject.SetActive(false);

                //set interactability of buttons 
                if (s.IsOnturn() && (s.IsBroken() || !s.HasUnresolvedBrokenAllies()))
                    s.soldierUI.actionButton.interactable = true;
                else
                    s.soldierUI.actionButton.interactable = false;

                //set message
                if (s.IsDead() || s.IsPlayingDead())
                {
                    s.soldierUI.revealMessageText.text = "<color=red>Dead</color>";
                    s.soldierUI.revealMessage.SetActive(true);
                }
                else if (s.IsOffturn() && s.IsRevealed())
                {
                    s.soldierUI.revealMessage.SetActive(true);
                    s.soldierUI.revealMessageText.text = "<color=green>Revealed</color>";
                }
                else if (s.IsOffturn() && s.IsSpotted())
                {
                    s.soldierUI.revealMessage.SetActive(true);
                    s.soldierUI.revealMessageText.text = "<color=green>Spotting</color>";
                }
                else
                    s.soldierUI.revealMessage.SetActive(false);

                //set broken message
                if (s.IsOnturnAndAlive() && !s.IsBroken() && s.HasUnresolvedBrokenAllies())
                    s.soldierUI.resolveBroken.SetActive(true);
                else
                    s.soldierUI.resolveBroken.SetActive(false);

                
            }
        }
    }
    public void RenderSoldierVisuals()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            if (OverrideView)
                s.renderer.enabled = true;
            else
            {
                if (s.IsOnturn() || s.IsRevealed() || s.IsDead() || s.IsPlayingDead() || s.IsSpotted())
                    s.renderer.enabled = true;
                else
                    s.renderer.enabled = false;
            }
            s.PaintColor();
        }
        foreach (Claymore c in FindObjectsByType<Claymore>(default))
        {
            if (OverrideView)
                c.renderer.enabled = true;
            else
            {
                if (c.revealed || (c.placedBy != null && c.placedBy.soldierTeam == game.currentTeam))
                    c.renderer.enabled = true;
                else
                    c.renderer.enabled = false;
            }
        }
        foreach (ThermalCamera tc in FindObjectsByType<ThermalCamera>(default))
        {
            if (OverrideView)
                tc.beam.renderer.enabled = true;
            else
            {
                if (tc.placedBy != null && tc.placedBy.soldierTeam == game.currentTeam)
                    tc.beam.renderer.enabled = true;
                else
                    tc.beam.renderer.enabled = false;
            }
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
        var itemList = FindObjectsByType<Item>(default);
        foreach (Item i in itemList)
        {
            if (OverrideView)
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
            s.soldierUI.gameObject.SetActive(true);
            s.fielded = true;
            s.renderer.enabled = true;
            s.CheckSpecialityColor(s.soldierSpeciality);
            blockingScreen.SetActive(true);
        }
    }
    public void ChangeRoundIndicators()
    {
        if (!game.gameOver)
        {
            roundIndicator.text = $"Round {game.currentRound}";
            if (game.currentTeam == 1)
                teamTurnIndicator.text = "<color=red>";
            else
                teamTurnIndicator.text = "<color=blue>";
            teamTurnIndicator.text += $"Team {game.currentTeam}</color> Turn";
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
            //play banzai sfx
            soundManager.PlayBanzai();

            turnTimer.color = Color.red;
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

        //display politics button - politician ability
        if (activeSoldier.IsPolitician())
            politicsButton.gameObject.SetActive(true);
        else
            politicsButton.gameObject.SetActive(false);

        if (game.gameOver)
            GreyAll("Game Over");
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
        else if (activeSoldier.IsUsingBinocularsInReconMode())
            GreyOutButtons(AddAllButtons(buttonStates), "<color=green>Binoculars (Recon)</color>");
        else if (activeSoldier.IsFrozen() && game.frozenTurn)
        {
            GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), shotButton), "<color=orange>Frozen</color>");
            if (!activeSoldier.HasAnyAmmo())
            {
                buttonStates.Add(shotButton, "No Ammo");
                GreyOutButtons(buttonStates, "");
            }
        }
        else if (activeSoldier.IsBroken())
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
            if (activeSoldier.HasSMGsOrPistolsEquipped())
                GreyOutButtons(ExceptButton(ExceptButton(AddAllButtons(buttonStates), meleeButton), shotButton), "Melee Controlled");
            else
                GreyOutButtons(ExceptButton(AddAllButtons(buttonStates), meleeButton), "Melee Controlled");
        }
        else
        {
            //block move button
            if (activeSoldier.IsLastStand())
                buttonStates.Add(moveButton, "Last Stand");
            else if (activeSoldier.mp == 0)
                buttonStates.Add(moveButton, "No MA");
            else if (activeSoldier.IsMeleeControlling())
                buttonStates.Add(moveButton, "<color=green>Melee Controlling</color>");

            //block shot button
            if (!activeSoldier.HasGunsEquipped())
                buttonStates.Add(shotButton, "No Gun");
            else if (activeSoldier.IsBlind())
                buttonStates.Add(shotButton, "Blind");
            else if (!activeSoldier.IsValidLoadout())
                buttonStates.Add(shotButton, "Hands Full");
            else if (!activeSoldier.HasAnyAmmo())
                buttonStates.Add(shotButton, "No Ammo");
            else if (activeSoldier.IsMeleeControlling())
            {
                if (!activeSoldier.HasSMGsOrPistolsEquipped())
                    buttonStates.Add(shotButton, "<color=green>Melee Controlling</color>");
            }

            //block melee button
            if (!activeSoldier.FindMeleeTargets())
                buttonStates.Add(meleeButton, "No Target");
            else if (activeSoldier.stats.SR.Val == 0)
                buttonStates.Add(meleeButton, "Blind");

            //block dipelec button
            if (!activeSoldier.TerminalInRange(default))
                buttonStates.Add(dipElecButton, "No Terminal");
            else if (activeSoldier.IsBlind())
                buttonStates.Add(dipElecButton, "Blind");
            else if (!activeSoldier.TerminalInRange(true))
                buttonStates.Add(dipElecButton, "Terminal Disabled");
            else if (activeSoldier.IsMeleeControlling())
                buttonStates.Add(dipElecButton, "<color=green>Melee Controlling</color>");

            //block overwatch button
            if (!activeSoldier.HasGunsEquipped())
                buttonStates.Add(overwatchButton, "No Gun");
            else if (activeSoldier.IsBlind())
                buttonStates.Add(overwatchButton, "Blind");
            else if (!activeSoldier.IsValidLoadout())
                buttonStates.Add(overwatchButton, "Hands Full");
            else if (activeSoldier.HasTwoGunsEquipped())
                buttonStates.Add(overwatchButton, "Dual Wield");
            else if (!activeSoldier.HasAnyAmmo())
                buttonStates.Add(overwatchButton, "No Ammo");
            else if (activeSoldier.IsMeleeControlling())
                buttonStates.Add(overwatchButton, "<color=green>Melee Controlling</color>");

            //block cover button
            if (activeSoldier.IsWearingJuggernautArmour(false))
                buttonStates.Add(coverButton, "<color=green>Juggernaut</color>");
            else if (activeSoldier.IsInCover())
                buttonStates.Add(coverButton, "<color=green>Taking Cover</color>");
            else if (activeSoldier.IsMeleeControlling())
                buttonStates.Add(coverButton, "<color=green>Melee Controlling</color>");

            //block playdead button
            if (activeSoldier.IsWearingJuggernautArmour(false))
                buttonStates.Add(playdeadButton, "Juggernaut");
            else if (activeSoldier.IsMeleeControlling())
                buttonStates.Add(playdeadButton, "<color=green>Melee Controlling</color>");

            //block disarm button
            if (!activeSoldier.DisarmableInRange())
                buttonStates.Add(disarmButton, "No Devices");
            else if (activeSoldier.IsBlind())
                buttonStates.Add(disarmButton, "Blind");

            //block drag button
            if (!activeSoldier.DraggableInRange())
                buttonStates.Add(dragButton, "No Targets");
            else if (activeSoldier.IsBlind())
                buttonStates.Add(dragButton, "Blind");
            else if (activeSoldier.IsLastStand())
                buttonStates.Add(dragButton, "Last Stand");

            //block politics button
            if (activeSoldier.politicianUsed)
                buttonStates.Add(politicsButton, "Already Used");

            GreyOutButtons(buttonStates, "");
        }

        //change config button text if first ap use
        if (activeSoldier.roundsFielded == 0 && !activeSoldier.usedAP)
            configureButton.GetComponentInChildren<TextMeshProUGUI>().text = "Spawn Config";
        else
            configureButton.GetComponentInChildren<TextMeshProUGUI>().text = "Config";

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
    }

    public void GreyOutButtons(Dictionary<Button, string> buttonStates, string multiButtonReason)
    {
        soldierOptionsUI.transform.Find("AllReason").GetComponent<TextMeshProUGUI>().text = multiButtonReason;

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
                    UnGrey(b, "");
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
    }

    public void UnGrey(Button button, string reason)
    {
        button.interactable = true;
        button.transform.Find("Reason").gameObject.SetActive(false);
    }

    public void GreyOut(Button button, string reason)
    {
        button.interactable = false;
        button.transform.Find("Reason").gameObject.SetActive(true);
        button.transform.Find("Reason").GetComponentInChildren<TextMeshProUGUI>().text = reason;
    }
    public void DisplayActiveSoldier()
    {
        Transform soldierBanner = soldierOptionsUI.transform.Find("SoldierBanner");
        soldierBanner.Find("HP").GetComponent<TextMeshProUGUI>().text = $"HP: {activeSoldier.GetFullHP()}";
        soldierBanner.Find("AP").GetComponent<TextMeshProUGUI>().text = $"AP: {activeSoldier.ap}";
        soldierBanner.Find("MA").GetComponent<TextMeshProUGUI>().text = $"MA: {activeSoldier.mp}";
        soldierBanner.Find("Speed").GetComponent<TextMeshProUGUI>().text = $"Move: {activeSoldier.InstantSpeed}";
        soldierBanner.Find("XP").GetComponent<TextMeshProUGUI>().text = $"XP: {activeSoldier.xp}";
        soldierBanner.Find("Status").GetComponent<TextMeshProUGUI>().text = activeSoldier.GetStatus();

        if (OverrideView)
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

                if (OverrideView)
                {
                    soldierStatsUI.Find("Stats").Find("OverrideBase").gameObject.SetActive(true);
                    soldierStatsUI.Find("Stats").Find("OverrideBase").Find(s[0]).GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.stats.GetStat(s[0].ToString()).BaseVal.ToString();
                    soldierStatsUI.Find("General").Find("OverrideAbility").gameObject.SetActive(true);
                    soldierStatsUI.Find("General").Find("OverrideAbility").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = HelperFunctions.PrintList(activeSoldier.soldierAbilities);
                    soldierStatsUI.Find("General").Find("OverrideLocation").gameObject.SetActive(true);
                    soldierStatsUI.Find("General").Find("OverrideLocation").Find("OverrideLocationX").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
                    soldierStatsUI.Find("General").Find("OverrideLocation").Find("OverrideLocationY").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
                    soldierStatsUI.Find("General").Find("OverrideLocation").Find("OverrideLocationZ").GetComponent<TMP_InputField>().placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();
                    soldierStatsUI.Find("General").Find("OverrideTerrainOn").gameObject.SetActive(true);
                    GetOverrideTerrainOn(soldierStatsUI);
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
            soldierStatsUI.Find("General").Find("Specialty").GetComponent<TextMeshProUGUI>().text = activeSoldier.PrintSoldierSpeciality();
            soldierStatsUI.Find("General").Find("Ability").GetComponent<TextMeshProUGUI>().text = HelperFunctions.PrintList(activeSoldier.soldierAbilities);
            soldierStatsUI.Find("General").Find("Location").Find("LocationX").GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
            soldierStatsUI.Find("General").Find("Location").Find("LocationY").GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
            soldierStatsUI.Find("General").Find("Location").Find("LocationZ").GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();

            //terrain and terrain on
            soldierStatsUI.Find("General").Find("TerrainOn").GetComponent<TextMeshProUGUI>().text = activeSoldier.TerrainOn;
            soldierStatsUI.Find("General").Find("Terrain").GetComponent<TextMeshProUGUI>().text = activeSoldier.soldierTerrain;
            if (activeSoldier.IsOnNativeTerrain())
            {
                soldierStatsUI.Find("General").Find("TerrainOn").GetComponent<TextMeshProUGUI>().color = Color.green;
                soldierStatsUI.Find("General").Find("Terrain").GetComponent<TextMeshProUGUI>().color = Color.green;
            }
            else if (activeSoldier.IsOnOppositeTerrain())
            {
                soldierStatsUI.Find("General").Find("TerrainOn").GetComponent<TextMeshProUGUI>().color = Color.red;
                soldierStatsUI.Find("General").Find("Terrain").GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                soldierStatsUI.Find("General").Find("TerrainOn").GetComponent<TextMeshProUGUI>().color = Color.white;
                soldierStatsUI.Find("General").Find("Terrain").GetComponent<TextMeshProUGUI>().color = Color.white;
            }

            //rounds without food
            soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().text = activeSoldier.RoundsWithoutFood.ToString();
            if (activeSoldier.RoundsWithoutFood < 0)
                soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().color = Color.green;
            else if (activeSoldier.tp >= 10)
                soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().color = Color.red;
            else
                soldierStatsUI.Find("General").Find("RoundsWithoutFood").GetComponent<TextMeshProUGUI>().color = Color.white;

            //trauma
            soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().text = activeSoldier.tp.ToString();
            if (activeSoldier.IsDesensitised())
                soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().color = Color.green;
            else if (activeSoldier.tp > 0)
                soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().color = Color.red;
            else
                soldierStatsUI.Find("General").Find("TraumaPoints").GetComponent<TextMeshProUGUI>().color = Color.white;
        }
    }

    public void CloseSoldierMenu()
    {
        if (activeSoldier.usedAP && activeSoldier.ap > 0 && !OverrideView)
            OpenEndSoldierTurnAlertUI();
        else
        {
            if (game.modaTurn)
                game.EndModaTurn();
            if (game.frozenTurn)
                game.EndFrozenTurn();
            activeSoldier.UnsetActiveSoldier();
            soldierOptionsUI.SetActive(false);
            soldierManager.enemyDisplayColumn.SetActive(true);
            soldierManager.friendlyDisplayColumn.SetActive(true);
            
            //save game
            DataPersistenceManager.Instance.SaveGame();
        }
    }
    public void CloseSoldierMenuUndo()
    {
        activeSoldier.usedAP = false;
        activeSoldier.selected = false;
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















    





    


    //detection functions - menu
    public IEnumerator OpenDetectionAlertUI()
    {
        //wait for everything else to resolve
        yield return new WaitUntil(() => MovementResolvedFlag() && explosionResolvedFlag && shotResolvedFlag && meleeResolvedFlag && inspirerResolvedFlag && binocularsFlashResolvedFlag);
        CloseSoldierStatsUI();

        detectionAlertUI.transform.Find("OptionPanel").Find("IllusionistAlert").gameObject.SetActive(false);
        detectionUI.transform.Find("OptionPanel").Find("IllusionistButton").gameObject.SetActive(false);
        int childCount = 0, overwatchCount = 0;
        foreach (Transform child in detectionUI.detectionAlertsPanel)
        {
            childCount++;
            if (child.Find("DetectionArrow").GetComponent<Image>().sprite.ToString().Contains("verwatch"))
                overwatchCount++;
        }

        if (childCount > 0)
        {
            SetDetectionResolvedFlagTo(false);

            if (overwatchCount > 1) //more than a single overwatch line detected
                detectionUI.transform.Find("MultiOverwatchAlert").gameObject.SetActive(true);
            else
                detectionUI.transform.Find("MultiOverwatchAlert").gameObject.SetActive(false);

            //illusionist ability
            if (activeSoldier != null)
            {
                if (activeSoldier.IsIllusionist() && activeSoldier.IsHidden() && !activeSoldier.illusionedThisMove)
                {
                    //FIX illusionist triggering on non-move actions
                    detectionAlertUI.transform.Find("OptionPanel").Find("IllusionistAlert").gameObject.SetActive(true);
                    detectionUI.transform.Find("OptionPanel").Find("IllusionistButton").gameObject.SetActive(true);
                }
            }

            detectionAlertUI.SetActive(true);

            //play detection alarm sfx
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

        //destroy detection alerts
        foreach (Transform child in detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            Destroy(child.gameObject);

        CloseDetectionUI();
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
                detectionUI.gameObject.SetActive(true);
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
            Dictionary<string, List<string>> allSoldiersNotRevealingOutOfSR = new();
            Dictionary<string, List<string>> allSoldiersNotRevealingNoLos = new();
            Dictionary<string, List<string>> allSoldiersNotRevealingHidden = new();
            Dictionary<string, List<string>> allSoldiersRevealingFinal = new();

            foreach (Soldier s in game.AllSoldiers())
            {
                allSoldiersRevealing.Add(s.id, new List<string>());
                allSoldiersRevealedBy.Add(s.id, new List<string>());
                allSoldiersNotRevealingOutOfSR.Add(s.id, new List<string>());
                allSoldiersNotRevealingNoLos.Add(s.id, new List<string>());
                allSoldiersNotRevealingHidden.Add(s.id, new List<string>());
                allSoldiersRevealingFinal.Add(s.id, new List<string>());

                //add soldiers that can't possibly be seen cause out of any collider
                foreach (Soldier s2 in game.AllSoldiers())
                {
                    if (s.IsOppositeTeamAs(s2) && s2.IsAlive())
                    {
                        if (!s.soldiersWithinAnyCollider.Contains(s2.Id)) //if (!s.PhysicalObjectWithinMaxRadius(s2))
                            allSoldiersNotRevealingOutOfSR[s.Id].Add(s2.Id);
                    } 
                }
            }

            Transform detectionAlert = detectionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
            foreach (Transform child in detectionAlert)
            {
                if (child.GetComponent<SoldierAlertLOS>() != null)
                {
                    SoldierAlertLOS sAlert = child.GetComponent<SoldierAlertLOS>();
                    Soldier detector = sAlert.s1;
                    Soldier counter = sAlert.s2;

                    //do checks for s1 (detector)(left side)
                    if (sAlert.s1Label.text.Equals("OUT OF SR")) //formally out of sr
                        allSoldiersNotRevealingOutOfSR[counter.Id].Add(detector.Id);
                    else
                    {
                        if (sAlert.s1Label.text.Contains("GLIMPSE") || sAlert.s1Label.text.Contains("RETREAT")) //if it's a glimpse or retreat alert
                        {
                            allSoldiersNotRevealingOutOfSR[counter.Id].Add(detector.Id);

                            if (sAlert.s1Toggle.isOn == true) //LOS paid
                            {
                                if (sAlert.s1Label.text.Contains("DETECT") || sAlert.s1Label.text.Contains("OVERWATCH")) //detection
                                {
                                    //create the glimpse alert for team
                                    AddLosGlimpseAlert(detector, sAlert.s1Label.text);
                                    StartCoroutine(OpenLostLOSList());

                                    //check for overwatch shot
                                    if (sAlert.s1Label.text.Contains("OVERWATCH"))
                                        StartCoroutine(OpenOverwatchShotUI(counter, detector));

                                    //pay xp for binoc detection (only if it's a "new" detection)
                                    if (counter.IsUsingBinoculars() && detector.IsHidden())
                                        AddXpAlert(counter, 4, $"Detected {detector.soldierName} with binoculars ({counter.BinocularUseMode()}).", true);
                                }
                                else //avoidance
                                    AddXpAlert(detector, 1 + detector.ShadowXpBonus(counter.IsRevoker()), $"Avoided detection.", true); //xp
                            }
                        }
                        else //if it's a standard SR alert
                        {
                            if (sAlert.s1Toggle.isOn == true) //paid by GM
                            {
                                if (sAlert.s1Label.text.Contains("DETECT") || sAlert.s1Label.text.Contains("OVERWATCH"))
                                {
                                    allSoldiersRevealing[counter.Id].Add(detector.Id);

                                    //check for overwatch shot
                                    if (sAlert.s1Label.text.Contains("OVERWATCH"))
                                        StartCoroutine(OpenOverwatchShotUI(counter, detector));

                                    //pay xp for binoc detection (only if it's a "new" detection)
                                    if (counter.IsUsingBinoculars() && detector.IsHidden())
                                        AddXpAlert(counter, 4, $"Detected {detector.soldierName} with binoculars ({counter.BinocularUseMode()}).", true);
                                }
                                else //avoidance
                                {
                                    allSoldiersNotRevealingHidden[counter.Id].Add(detector.Id);
                                    AddXpAlert(detector, 1 + detector.ShadowXpBonus(counter.IsRevoker()), $"Avoided detection.", true); //xp
                                }
                            }
                            else
                                allSoldiersNotRevealingNoLos[counter.Id].Add(detector.Id);

                        }
                    }
                    

                    //do checks for s2 (counter)(right side)
                    if (sAlert.s2Label.text.Equals("OUT OF SR")) //formally out of sr
                        allSoldiersNotRevealingOutOfSR[detector.Id].Add(counter.Id);
                    else
                    {
                        if (sAlert.s2Label.text.Contains("GLIMPSE") || sAlert.s2Label.text.Contains("RETREAT")) //if it's a glimpse or retreat alert
                        {
                            allSoldiersNotRevealingOutOfSR[detector.Id].Add(counter.Id);

                            if (sAlert.s2Toggle.isOn == true) //LOS paid
                            {
                                if (sAlert.s2Label.text.Contains("DETECT") || sAlert.s2Label.text.Contains("OVERWATCH")) //detection
                                {
                                    //create glimpse alert for team
                                    AddLosGlimpseAlert(counter, sAlert.s2Label.text);
                                    StartCoroutine(OpenLostLOSList());

                                    //check for overwatch shot
                                    if (sAlert.s2Label.text.Contains("OVERWATCH"))
                                        StartCoroutine(OpenOverwatchShotUI(detector, counter));

                                    //pay xp for binoc detection (only if it's a "new" detection)
                                    if (detector.IsUsingBinoculars() && counter.IsHidden())
                                        AddXpAlert(detector, 4, $"Detected {counter.soldierName} with binoculars ({detector.BinocularUseMode()}).", true);
                                }
                                else //avoidance
                                    AddXpAlert(counter, 1 + counter.ShadowXpBonus(detector.IsRevoker()), $"Avoided detection.", true); //xp
                            }
                        }
                        else //if it's a standard SR alert
                        {
                            if (sAlert.s2Toggle.isOn == true) //LOS paid
                            {
                                if (sAlert.s2Label.text.Contains("DETECT") || sAlert.s2Label.text.Contains("OVERWATCH"))
                                {
                                    allSoldiersRevealing[detector.Id].Add(counter.Id);

                                    //check for overwatch shot
                                    if (sAlert.s2Label.text.Contains("OVERWATCH"))
                                        StartCoroutine(OpenOverwatchShotUI(detector, counter));

                                    //pay xp for binoc detection (only if it's a "new" detection)
                                    if (detector.IsUsingBinoculars() && counter.IsHidden())
                                        AddXpAlert(detector, 4, $"Detected {counter.soldierName} with binoculars ({detector.BinocularUseMode()}).", true);
                                }
                                else
                                {
                                    allSoldiersNotRevealingHidden[detector.Id].Add(counter.Id);
                                    AddXpAlert(counter, 1 + counter.ShadowXpBonus(detector.IsRevoker()), $"Avoided detection.", true); //xp
                                }
                            }
                            else
                                allSoldiersNotRevealingNoLos[detector.Id].Add(counter.Id);
                        }
                    }

                    //if detector revealed either by glimpse or in SR a soldier with c > 2 xp check
                    if (sAlert.s2Toggle.isOn == true && (sAlert.s2Label.text.Contains("DETECT") || sAlert.s2Label.text.Contains("OVERWATCH")))
                    {
                        if (counter.ActiveC > 2)
                            AddXpAlert(detector, counter.ActiveC + detector.ShadowXpBonus(counter.IsRevoker()), $"Detected soldier ({counter.soldierName}) with C > 2.", true); //xp
                    }

                    //if counter revealed either by glimpse or in SR a soldier with c > 2 xp check
                    if (sAlert.s1Toggle.isOn == true && (sAlert.s1Label.text.Contains("DETECT") || sAlert.s1Label.text.Contains("OVERWATCH")))
                    {
                        if (detector.ActiveC > 2)
                            AddXpAlert(counter, detector.ActiveC + counter.ShadowXpBonus(detector.IsRevoker()), $"Detected soldier ({detector.soldierName}) with C > 2.", true); //xp
                    }
                }
                else if (child.GetComponent<ClaymoreAlertLOS>() != null)
                {
                    ClaymoreAlertLOS cAlert = child.GetComponent<ClaymoreAlertLOS>();
                    Soldier detector = cAlert.soldier;
                    Claymore claymore = cAlert.claymore;

                    //play claymore detect dialogue
                    soundManager.PlaySoldierDetectClaymore(detector);

                    claymore.revealed = true;
                }
                else if (child.GetComponent<ThermalCamAlertLOS>() != null)
                {
                    ThermalCamAlertLOS tcAlert = child.GetComponent<ThermalCamAlertLOS>();
                    ThermalCamera thermalCam = tcAlert.themalCam;
                    Soldier detector = thermalCam.placedBy;
                    Soldier detectee = tcAlert.soldier;

                    //if not a glimpse or a retreat detection, add soldier to revealing list
                    if (!tcAlert.label.text.Contains("GLIMPSE") && !tcAlert.label.text.Contains("RETREAT"))
                    {
                        if (tcAlert.toggle.isOn == true)
                            allSoldiersRevealing[detector.Id].Add(detectee.Id);
                        else
                            allSoldiersNotRevealingNoLos[detector.Id].Add(detectee.Id);
                    }
                    else
                    {
                        allSoldiersNotRevealingOutOfSR[detector.Id].Add(detectee.Id);
                        AddLosGlimpseAlert(detectee, tcAlert.label.text);
                        StartCoroutine(OpenLostLOSList());
                    }

                    //if soldier was paid revealed either by glimpse or in SR xp for thermal cam
                    if (tcAlert.toggle.isOn == true)
                    {
                        AddXpAlert(detector, 1, $"Thermal camera at ({thermalCam.X},{thermalCam.Y},{thermalCam.Z}) revealed soldier ({detectee.soldierName}).", true); //xp
                    }
                }
            }

            //combine old revealing list with fresh revealing list and not revealing list
            foreach (KeyValuePair<string, List<string>> keyValuePair in allSoldiersRevealing)
            {
                //print(keyValuePair.Key + " " + game.FindSoldierById(keyValuePair.Key).soldierName);
                List<string> arrayOfRevealingList = allSoldiersRevealing[keyValuePair.Key];
                List<string> arrayOfOldRevealingList = soldierManager.FindSoldierById(keyValuePair.Key).LOSToTheseSoldiersAndRevealing;
                List<string> arrayOfNotRevealingList = new();
                arrayOfNotRevealingList.AddRange(allSoldiersNotRevealingOutOfSR[keyValuePair.Key]);
                arrayOfNotRevealingList.AddRange(allSoldiersNotRevealingNoLos[keyValuePair.Key]);
                arrayOfNotRevealingList.AddRange(allSoldiersNotRevealingHidden[keyValuePair.Key]);

                //populate final reveal list = fresh reveals plus old reveals minus not reveals
                List<string> arrayFinalRevealingList = arrayOfRevealingList.Concat(arrayOfOldRevealingList).Distinct().Except(arrayOfNotRevealingList).ToList();
                //print(IdToName(keyValuePair.Key) + "--->New:[" + PrintList(arrayOfRevealingList) + "] + Existing:[" + PrintList(arrayOfOldRevealingList) + "] - NotRevealing:[" + PrintList(arrayOfNotRevealingList) + "] = Final:[" + PrintList(arrayFinalRevealingList) + "]");

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

            //repopulate each soldier's los lists
            foreach (Soldier s in game.AllSoldiers())
            {
                foreach (string soldierOutOfSR in allSoldiersNotRevealingOutOfSR.GetValueOrDefault(s.Id))
                    s.AddSoldierOutOfSR(soldierOutOfSR);

                foreach (string noLosToThisSoldier in allSoldiersNotRevealingNoLos.GetValueOrDefault(s.Id))
                    s.AddNoLOSToThisSoldier(noLosToThisSoldier);

                foreach (string losToThisSoldierButHidden in allSoldiersNotRevealingHidden.GetValueOrDefault(s.Id))
                    s.AddLOSToThisSoldierButHidden(losToThisSoldierButHidden);

                foreach (string losToThisSoldierAndRevealing in allSoldiersRevealingFinal.GetValueOrDefault(s.Id))
                    s.AddLOSToThisSoldierAndRevealing(losToThisSoldierAndRevealing);

                foreach (string soldierRevealingThisSoldier in allSoldiersRevealedBy.GetValueOrDefault(s.Id))
                    s.AddSoldierRevealingThisSoldier(soldierRevealingThisSoldier);

                s.UnsetLosCheck(); //clear the losCheck trigger

                //deactivate flash mode binoculars
                if (s.IsUsingBinocularsInFlashMode())
                    s.UnsetUsingBinoculars();
            }
            
            //close detection UI
            CloseDetectionUI();
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }
    public void CloseDetectionUI()
    {
        SetDetectionResolvedFlagTo(true);
        detectionUI.ClearAllAlerts();
        detectionUI.gameObject.SetActive(false);
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
        //block duplicate lostlos alerts being created
        foreach (Transform child in lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
            if (child.GetComponent<SoldierAlert>().soldier == soldier && child.Find("LosGlimpseTitle") != null)
                Destroy(child.gameObject);

        GameObject losGlimpseAlert = Instantiate(losGlimpseAlertPrefab, lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        losGlimpseAlert.GetComponent<SoldierAlert>().SetSoldier(soldier);
        losGlimpseAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(soldier);
        losGlimpseAlert.transform.Find("LosGlimpseDescription").GetComponent<TextMeshProUGUI>().text = $"{soldier.soldierName} was glimpsed {description}.";
    }
    public IEnumerator OpenLostLOSList()
    {
        yield return new WaitUntil(() => meleeResolvedFlag == true);

        bool display = false;
        foreach (Transform child in lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
        {
            //destroy false alerts
            if (child.GetComponent<SoldierAlert>().soldier.SoldiersRevealingThisSoldier.Any())
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
    public void AddSoldierAlert(Soldier soldier, string title, Color titleColour, string description, int preDamage, int postDamage)
    {
        Instantiate(soldierAlertPrefab, damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content")).Init(soldier, title, titleColour, description, preDamage, postDamage);

        //try and open damagealert
        StartCoroutine(OpenDamageList());
    }
    public IEnumerator OpenDamageList()
    {
        yield return new WaitUntil(() => shotResolvedFlag == true && meleeResolvedFlag == true);

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
            "Fight" => "+1 F",
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
            foreach (Transform child in traumaUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"))
                if (child.GetComponent<SoldierAlert>().soldier.IsDead())
                    Destroy(child.gameObject);

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
        Instantiate(traumaAlertPrefab, traumaUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content")).Init(friendly, trauma, reason, rolls, xpOnResist, range);
    }

    public IEnumerator OpenTraumaAlertUI()
    {
        yield return new WaitUntil(() => meleeResolvedFlag == true && shotResolvedFlag == true && explosionResolvedFlag == true);

        traumaAlertUI.SetActive(true);
    }
    public void CloseTraumaAlertUI()
    {
        traumaAlertUI.SetActive(false);
    }
    public void OpenBrokenFledUI()
    {
        FreezeTimer();
        brokenFledUI.SetActive(true);
    }

    public void CloseBrokenFledUI()
    {
        UnfreezeTimer();
        brokenFledUI.SetActive(false);
    }













    //explosion functions
    public void OpenExplosionUI()
    {
        SetExplosionResolvedFlagTo(false);
        explosionUI.SetActive(true);
    }
    public void CloseExplosionUI()
    {
        if (explosionUI.transform.childCount == 1) //check if this is the last explosion list
        {
            //delete relevant explosions
            foreach (Explosion explosion in FindObjectsByType<Explosion>(default))
                Destroy(explosion.gameObject);

            SetExplosionResolvedFlagTo(true);
            explosionUI.SetActive(false);
        }
    }
    public void AddExplosionAlert(GameObject explosionList, Soldier hitByExplosion, Vector3 explosionLocation, Soldier explodedBy, int damage, int stunRounds)
    {
        if (hitByExplosion.IsAlive())
        {
            GameObject explosionAlert = Instantiate(explosionAlertPrefab, explosionList.transform.Find("Scroll").Find("View").Find("Content"));

            explosionAlert.GetComponent<ExplosiveAlert>().SetObjects(explodedBy, hitByExplosion);
            explosionAlert.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(hitByExplosion);

            //riot shield block
            if (hitByExplosion.HasActiveAndCorrectlyAngledRiotShield(explosionLocation))
                explosionAlert.transform.Find("RiotShield").gameObject.SetActive(true);

            //JA block
            if (hitByExplosion.IsWearingJuggernautArmour(true))
                explosionAlert.transform.Find("JA").gameObject.SetActive(true);

            //display item destroyed indicators
            if (damage > 0)
                explosionAlert.transform.Find("FragileDestroyed").gameObject.SetActive(true);
            if (damage >= 5)
                explosionAlert.transform.Find("BreakableDestroyed").gameObject.SetActive(true);

            //display damage and stun
            explosionAlert.transform.Find("Damage").Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text = $"{damage}";
            explosionAlert.transform.Find("Stun").Find("StunDamageIndicator").GetComponent<TextMeshProUGUI>().text = $"{stunRounds}";

            //delete if there's no damage and no stun
            if (damage <= 0 && stunRounds <= 0)
                Destroy(explosionAlert);  
        }
    }
    public void AddExplosionAlertPOI(GameObject explosionList, POI poiHit, Soldier explodedBy, int damage)
    {
        if (poiHit is not GoodyBox)
        {
            if ((poiHit is ExplosiveBarrel barrel && !barrel.triggered) || (poiHit is Claymore claymore && !claymore.triggered) || poiHit is Terminal || poiHit is DeploymentBeacon || poiHit is ThermalCamera)
            {
                GameObject explosionAlert = Instantiate(explosionAlertPOIPrefab, explosionList.transform.Find("Scroll").Find("View").Find("Content"));
                explosionAlert.GetComponent<ExplosiveAlert>().SetObjects(explodedBy, poiHit);

                explosionAlert.transform.Find("POIPortrait").GetComponent<POIPortrait>().Init(poiHit);
                explosionAlert.transform.Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text = $"{damage}";
            }
        }
    }
    public void AddExplosionAlertItem(GameObject explosionList, Item itemHit, Vector3 explosionLocation, Soldier explodedBy, int damage)
    {
        if (!itemHit.IsNestedInGoodyBox())
        {
            if (!itemHit.IsTriggered())
            {
                //riot shield block
                if (itemHit.IsNestedOnSoldier() && itemHit.SoldierNestedOn().HasActiveAndCorrectlyAngledRiotShield(explosionLocation))
                    damage /= 2;

                if ((itemHit.IsBreakable() && damage >= 5) || (itemHit.IsFragile() && damage > 0))
                {
                    GameObject explosionAlert = Instantiate(explosionAlertItemPrefab, explosionList.transform.Find("Scroll").Find("View").Find("Content"));

                    explosionAlert.GetComponent<ExplosiveAlert>().SetObjects(explodedBy, itemHit);
                    explosionAlert.transform.Find("ItemPortrait").GetComponent<ItemPortrait>().Init(itemHit);
                    explosionAlert.transform.Find("Damage").Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text = $"{damage}";

                    //show riot shield block
                    if (itemHit.IsNestedOnSoldier() && itemHit.SoldierNestedOn().HasActiveAndCorrectlyAngledRiotShield(explosionLocation))
                        explosionAlert.transform.Find("RiotShield").gameObject.SetActive(true);
                }
            }
        }
    }






    //shot functions - menu
    public void OpenShotUI()
    {
        //clear old data
        ClearShotUI();
        ClearShotConfirmUI();

        //set shooter details
        Soldier shooter = activeSoldier;
        shotUI.shooterID.text = shooter.Id;

        //generate gun dropdown
        List<TMP_Dropdown.OptionData> gunOptionDataList = new();
        TMP_Dropdown.OptionData gunOptionData;
        bool leftGrey = false, rightGrey = false;
        if (shooter.LeftHandItem != null)
        {
            if (shooter.LeftHandItem.IsGun())
            {
                if (shooter.LeftHandItem.CheckAnyAmmo())
                    gunOptionData = new(shooter.LeftHandItem.itemName, shooter.LeftHandItem.itemImage, Color.white);
                else
                {
                    gunOptionData = shotUI.gunsEmptyDropdown.options[shotUI.gunsEmptyDropdown.options.FindIndex(option => option.text.Contains($"{shooter.LeftHandItem.itemName}"))];
                    leftGrey = true;
                }
                gunOptionDataList.Add(gunOptionData);
            }
        }
        if (shooter.RightHandItem != null) 
        {
            if (shooter.RightHandItem.IsGun())
            {
                if (shooter.RightHandItem.CheckAnyAmmo())
                    gunOptionData = new(shooter.RightHandItem.itemName, shooter.RightHandItem.itemImage, Color.white);
                else
                {
                    gunOptionData = shotUI.gunsEmptyDropdown.options[shotUI.gunsEmptyDropdown.options.FindIndex(option => option.text.Contains($"{shooter.RightHandItem.itemName}"))];
                    rightGrey = true;
                }
                gunOptionDataList.Add(gunOptionData);
            }
        }
        if (gunOptionDataList.Count > 1)
        {
            foreach (TMP_Dropdown.OptionData option in shotUI.comboGunsDropdown.options)
            {
                if (option.text.Contains(shooter.LeftHandItem.itemName) && option.text.Contains(shooter.RightHandItem.itemName))
                {
                    if (leftGrey || rightGrey)
                    {
                        gunOptionData = shotUI.comboGunsEmptyDropdown.options[shotUI.comboGunsEmptyDropdown.options.FindIndex(option => option.text.Contains($"{shooter.RightHandItem.itemName}") && option.text.Contains($"{ shooter.RightHandItem.itemName}"))];
                        gunOptionDataList.Add(gunOptionData);
                    }
                    else 
                    {
                        gunOptionData = option;
                        gunOptionDataList.Add(gunOptionData);
                    }
                }
            }

            shotUI.aimTypeDropdown.value = 1;
            shotUI.aimTypeDropdown.interactable = false;
        }
        shotUI.gunDropdown.AddOptions(gunOptionDataList);

        if (leftGrey)
        {
            shotUI.gunDropdown.GetComponent<DropdownController>().optionsToGrey.Add(shooter.LeftHandItem.itemName);
            shotUI.gunDropdown.value = 1;
        }
        if (rightGrey)
            shotUI.gunDropdown.GetComponent<DropdownController>().optionsToGrey.Add(shooter.RightHandItem.itemName);
        if (leftGrey || rightGrey)
            shotUI.gunDropdown.GetComponent<DropdownController>().optionsToGrey.Add("2");

        //block suppression option if gun does not have enough ammo
        int gunsWithoutEnoughAmmoToSuppress = 0;
        foreach (Item gun in activeSoldier.EquippedGuns)
        {
            print($"{gun.itemName}|{gun.ammo}|{gun.suppressDrain}");
            if (!gun.CheckGreaterThanSpecificAmmo(gun.suppressDrain, true))
                gunsWithoutEnoughAmmoToSuppress++;
        }
        print(gunsWithoutEnoughAmmoToSuppress);
        if (gunsWithoutEnoughAmmoToSuppress == activeSoldier.EquippedGuns.Count)
            shotUI.shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression");

        //if soldier engaged in melee block force unaimed shot
        if (shooter.IsMeleeEngaged())
        {
            shotUI.aimTypeDropdown.value = 1;
            shotUI.shotTypeDropdown.interactable = false;
            shotUI.aimTypeDropdown.interactable = false;
            shotUI.coverLevelDropdown.interactable = false;
        }

        game.UpdateShotType(shooter);
        game.UpdateShotUI(shooter);

        shotUI.gameObject.SetActive(true);
    }

    public IEnumerator OpenOverwatchShotUI(Soldier shooter, Soldier target)
    {
        yield return new WaitUntil(() => MovementResolvedFlag() && detectionResolvedFlag);

        overwatchShotUI.Init(shooter, target);
        overwatchShotUI.gameObject.SetActive(true);
    }
    public void GuardsmanOverwatchRetry()
    {
        overwatchShotUI.ConfirmShotOverwatch(true);
    }
    public IEnumerator OpenShotResultUI(bool runSecondShot)
    {
        print("trying to open shot result");
        yield return new WaitUntil(() => explosionResolvedFlag);
        print("explosion flag is resolved, opening shot result");

        if (runSecondShot)
            shotResultUI.transform.Find("RunSecondShot").gameObject.SetActive(true);
        else
            shotResultUI.transform.Find("RunSecondShot").gameObject.SetActive(false);

        shotResultUI.SetActive(true);
    }
    public void CloseShotResultUI()
    {
        if (OverrideKey())
        {
            if (shotResultUI.transform.Find("RunSecondShot").gameObject.activeInHierarchy)
                game.ConfirmShot(false);
            else
            {
                SetShotResolvedFlagTo(true);
                shotResultUI.SetActive(false);
            }
        }
    }
    public void ClearShotUI()
    {
        clearShotFlag = true;
        shotUI.shotTypeDropdown.interactable = true;
        shotUI.shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        shotUI.shotTypeDropdown.value = 0;

        shotUI.gunDropdown.interactable = true;
        shotUI.gunDropdown.value = 0;
        shotUI.gunDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        shotUI.gunDropdown.ClearOptions();

        shotUI.aimTypeDropdown.interactable = true;
        shotUI.aimTypeDropdown.value = 0;

        shotUI.targetDropdown.interactable = true;
        shotUI.targetDropdown.ClearOptions();
        shotUI.targetDropdown.value = 0;

        shotUI.coverLevelDropdown.interactable = true;
        shotUI.coverLevelDropdown.value = 0;

        shotUI.coverXPos.text = "";
        shotUI.coverYPos.text = "";
        shotUI.coverZPos.text = "";
        shotUI.invalidCoverLocationUI.SetActive(false);
        ClearFlankersUI(flankersShotUI);
        clearShotFlag = false;
    }

    public void CloseShotUI()
    {
        shotUI.gameObject.SetActive(false);
        shotConfirmUI.SetActive(false);
    }
    public void OpenShotConfirmUI()
    {
        if (int.TryParse(shotUI.apCost.text, out int ap))
        {
            if (activeSoldier.CheckAP(ap))
            {
                Soldier shooter = soldierManager.FindSoldierById(shotUI.shooterID.text);
                IAmShootable target = game.FindShootableById(shotUI.targetDropdown.captionText.text);
                Item gun1 = null, gun2 = null;
                Tuple<int, int, int> chances1 = null, chances2 = null;

                //if shooting with two guns
                if (shotUI.gunDropdown.value == 2)
                {
                    gun1 = shooter.EquippedGuns[0];
                    gun2 = shooter.EquippedGuns[1];
                }
                else
                    gun1 = shooter.EquippedGuns[shotUI.gunDropdown.value];
                shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text = shotUI.gunDropdown.captionText.text;

                //if gun is valid, get chance for first shot
                if (gun1 != null)
                {
                    if (shotUI.shotTypeDropdown.value == 1)
                        chances1 = Tuple.Create(100, 0, 100);
                    else
                        chances1 = game.CalculateHitPercentage(shooter, target, gun1);
                }
                
                //if first shot is valid, display details
                if (chances1 != null)
                {
                    //show gun image
                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("GunImage").GetComponent<Image>().sprite = gun1.itemImage;

                    //only shot suppression hit chance if suppressed
                    if (shooter.IsSuppressed() && shotUI.shotTypeDropdown.value != 1)
                    {
                        shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").gameObject.SetActive(true);
                        shotConfirmUI.transform.Find("OptionPanel").Find("HitChanceLabels").Find("SuppressedHitChance").gameObject.SetActive(true);
                    }
                    else
                    {
                        shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").gameObject.SetActive(false);
                        shotConfirmUI.transform.Find("OptionPanel").Find("HitChanceLabels").Find("SuppressedHitChance").gameObject.SetActive(false);
                    }

                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = chances1.Item3.ToString() + "%";
                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = chances1.Item1.ToString() + "%";
                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = chances1.Item2.ToString() + "%";

                    //enable back button only if shot is aimed and under 25%
                    if (shotUI.aimTypeDropdown.captionText.text.Contains("Aimed") && chances1.Item1 <= 25)
                        shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = true;
                    else
                        shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = false;

                    //add parameter to equation view
                    shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = DisplayShotParameters();


                    shotConfirmUI.SetActive(true);
                }

                //if shooting with two guns
                if (shotUI.gunDropdown.value == 2)
                {
                    shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").gameObject.SetActive(true);

                    //show gun image
                    shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("GunImage").GetComponent<Image>().sprite = gun2.itemImage;

                    //if gun is valid
                    if (gun2 != null)
                    {
                        if (shotUI.shotTypeDropdown.value == 1)
                            chances2 = Tuple.Create(100, 0, 100);
                        else
                            chances2 = game.CalculateHitPercentage(shooter, target, gun2);
                    }

                    //only continue if shot is valid
                    if (chances2 != null)
                    {
                        //only shot suppression hit chance if suppressed
                        if (shooter.IsSuppressed() && shotUI.shotTypeDropdown.value != 1)
                            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").gameObject.SetActive(true);
                        else
                            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").gameObject.SetActive(false);

                        shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = chances2.Item3.ToString() + "%";
                        shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = chances2.Item1.ToString() + "%";
                        shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = chances2.Item2.ToString() + "%";

                        //back button always disabled for unaimed shot
                        shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = false;

                        //add parameter to equation view
                        //shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = DisplayShotParameters();

                        shotConfirmUI.SetActive(true);
                    }
                }
                else
                    shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").gameObject.SetActive(false);
            }
        }
    }
    public void ExitShotConfirmUI()
    {
        FileUtility.WriteToReport($"{activeSoldier.soldierName} bails out of aimed shot ({shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text}|{shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text})"); //write to report

        int.TryParse(shotUI.apCost.text, out int ap);
        //deduct ap for aiming if leaving shot
        activeSoldier.DeductAP(ap - 1);

        CloseShotUI();
    }
    public void ClearShotConfirmUI()
    {
        //if (shotConfirmUI.activeInHierarchy)
        {
            clearShotFlag = true;
            shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text = "";
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
        yield return new WaitUntil(() => MovementResolvedFlag() && detectionResolvedFlag);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => detectionResolvedFlag && shotResolvedFlag);

        //set attacker
        Soldier attacker = activeSoldier;
        meleeUI.attackerID.text = attacker.id;

        meleeChargeIndicator = meleeCharge;
        meleeUI.meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();

        List<TMP_Dropdown.OptionData> defenderDetails = new();

        //generate melee type dropdown
        List<TMP_Dropdown.OptionData> meleeTypeDetails = new()
        {
            new TMP_Dropdown.OptionData(meleeCharge),
            new TMP_Dropdown.OptionData("Engagement Only"),
        };
        meleeUI.meleeTypeDropdown.AddOptions(meleeTypeDetails);

        //block engagement only if melee controlled
        if (attacker.IsMeleeEngaged())
            meleeUI.meleeTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Engagement Only");

        //display best attacker weapon
        if (attacker.BestMeleeWeapon != null)
            meleeUI.attackerWeaponImage.sprite = attacker.BestMeleeWeapon.itemImage;
        else
            meleeUI.attackerWeaponImage.sprite = fist;



        //generate target list
        foreach (Soldier s in game.AllSoldiers())
        {
            TMP_Dropdown.OptionData defender = null;
            if (s.IsAlive() && attacker.IsOppositeTeamAs(s) && s.IsRevealed() && attacker.PhysicalObjectWithinMeleeRadius(s))
            {
                if (attacker.CanSeeInOwnRight(s))
                    defender = new(s.soldierName, s.soldierPortrait, Color.white);
                else
                    defender = new(s.soldierName, s.LoadPortraitTeamsight(s.soldierPortraitText), Color.white);

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
            meleeUI.targetDropdown.AddOptions(defenderDetails);

            Soldier defender = soldierManager.FindSoldierByName(meleeUI.targetDropdown.captionText.text);

            if (defender.controlledBySoldiersList.Contains(activeSoldier.id))
                meleeUI.meleeTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new ("<color=green>Disengage</color>") });
            else if (defender.controllingSoldiersList.Contains(activeSoldier.id))
                meleeUI.meleeTypeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>() { new ("<color=red>Request Disengage</color>") });

            //show defender weapon
            if (defender.BestMeleeWeapon != null)
                meleeUI.defenderWeaponImage.sprite = defender.BestMeleeWeapon.itemImage;
            else
                meleeUI.defenderWeaponImage.sprite = fist;

            CheckMeleeType();
            game.UpdateMeleeUI();

            meleeUI.gameObject.SetActive(true);
        }
        else
        {
            ClearMeleeUI();
            generalAlertUI.Activate("No melee targets found");
        }
    }
    public void ClearFlankersUI(GameObject flankersUI)
    {
        foreach (Transform child in flankersUI.transform.Find("FlankersPanel"))
            Destroy(child.gameObject);
        flankersUI.SetActive(false);
    }
    public void CheckMeleeType()
    {
        if (meleeUI.meleeTypeDropdown.value == 0)
        {
            meleeUI.attackerWeaponUI.SetActive(true);
            meleeUI.defenderWeaponUI.SetActive(true);
            meleeUI.flankersMeleeAttackerUI.SetActive(true);
            meleeUI.flankersMeleeDefenderUI.SetActive(true);
        }
        else
        {
            meleeUI.attackerWeaponUI.SetActive(false);
            meleeUI.defenderWeaponUI.SetActive(false);
            meleeUI.flankersMeleeAttackerUI.SetActive(false);
            meleeUI.flankersMeleeDefenderUI.SetActive(false);
        }
        
        if (meleeUI.meleeTypeDropdown.captionText.text.Contains("Request"))
            OpenMeleeBreakEngagementRequestUI();
    }
    public void ClearMeleeUI()
    {
        clearMeleeFlag = true;
        meleeUI.meleeTypeDropdown.ClearOptions();
        meleeUI.meleeTypeDropdown.value = 0;
        meleeUI.attackerWeaponImage.sprite = null;
        meleeUI.defenderWeaponImage.sprite = null;
        meleeUI.targetDropdown.ClearOptions();
        meleeUI.targetDropdown.value = 0;
        ClearFlankersUI(meleeUI.flankersMeleeAttackerUI);
        ClearFlankersUI(meleeUI.flankersMeleeDefenderUI);
        clearMeleeFlag = false;
    }
    public void CloseMeleeUI()
    {
        ClearMeleeUI();
        ClearMeleeConfirmUI();
        meleeUI.gameObject.SetActive(false);
        meleeConfirmUI.SetActive(false);
    }
    public void OpenMeleeConfirmUI()
    {
        if (int.TryParse(meleeUI.apCost.text, out int ap))
        {
            if (activeSoldier.CheckAP(ap))
            {
                //find attacker and defender
                Soldier attacker = soldierManager.FindSoldierById(meleeUI.attackerID.text);
                Soldier defender = soldierManager.FindSoldierByName(meleeUI.targetDropdown.captionText.text);

                int meleeDamage = game.CalculateMeleeResult(attacker, defender);

                meleeConfirmUI.transform.Find("OptionPanel").Find("Damage").Find("DamageDisplay").GetComponent<TextMeshProUGUI>().text = meleeDamage.ToString();

                //add parameters to equation view
                meleeConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = DisplayMeleeParameters();

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
    public IEnumerator OpenMeleeResultUI()
    {
        yield return new WaitUntil(() => shotResolvedFlag == true);

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
        meleeUI.meleeTypeDropdown.value = 0;
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
        ClearClosestAllyUI(moveUI.closestAllyUI);

        Soldier closestAlly = activeSoldier.ClosestAllyForPlannerBuff();

        if (closestAlly != null)
        {
            GameObject closestAllyPortrait = Instantiate(soldierPortraitPrefab, moveUI.closestAllyUI.transform.Find("ClosestAllyPanel"));
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
        //set default
        moveUI.locationUI.SetActive(true);
        moveUI.terrainDropdownUI.SetActive(true);
        moveUI.coverToggleUI.SetActive(true);
        moveUI.meleeToggleUI.SetActive(true);
        moveUI.fallInputUI.SetActive(true);
        moveUI.closestAllyUI.SetActive(false);
        moveUI.moveDonatedUI.SetActive(false);

        if (moveUI.moveTypeDropdown.captionText.text.Contains("Planner"))
        {
            moveUI.locationUI.SetActive(false);
            moveUI.terrainDropdownUI.SetActive(false);
            moveUI.coverToggleUI.SetActive(false);
            moveUI.meleeToggleUI.SetActive(false);
            moveUI.fallInputUI.SetActive(false);
            ShowClosestAlly();
            moveUI.closestAllyUI.SetActive(true);
            moveUI.moveDonatedUI.SetActive(true);
        }
        else if (moveUI.moveTypeDropdown.captionText.text.Contains("Exo"))
            moveUI.fallInputUI.SetActive(false);

    }
    public void OpenMoveUI(bool suppressed)
    {
        List<TMP_Dropdown.OptionData> moveTypeDetails;
        moveUI.moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        moveUI.moveTypeDropdown.ClearOptions();
        moveUI.moveTypeDropdown.interactable = true;
        moveUI.coverToggle.interactable = true;
        moveUI.meleeToggle.interactable = true;

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
            moveUI.moveTypeDropdown.interactable = false;
            moveUI.coverToggle.interactable = false;
            moveUI.meleeToggle.interactable = false;
            moveUI.backButton.SetActive(false);
        }
        else
        {
            moveTypeDetails = new()
            {
                new TMP_Dropdown.OptionData("Full: " + activeSoldier.FullMove + fullMoveSuppressed),
                new TMP_Dropdown.OptionData("Half: " + activeSoldier.HalfMove + halfMoveSuppressed),
                new TMP_Dropdown.OptionData("Tile: " + activeSoldier.TileMove),
            };
            moveUI.backButton.SetActive(true);
        }
        
        //add extra move options for planner/exo
        if (activeSoldier.IsPlanner() && activeSoldier.ClosestAllyForPlannerBuff() != null && !activeSoldier.usedMP)
            moveTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Planner Donation</color>"));
        if (activeSoldier.IsWearingExoArmour())
            moveTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Exo Jump</color>"));
        moveUI.moveTypeDropdown.AddOptions(moveTypeDetails);

        if (activeSoldier.IsSmokeBlinded())
        {
            moveUI.moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("0");
            moveUI.moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("1");
            moveUI.moveTypeDropdown.value = 2;
        }
        else
        {
            //grey options according to AP
            if (activeSoldier.ap < 3)
            {
                moveUI.moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("0");
                moveUI.moveTypeDropdown.value = 1;
            }
            if (activeSoldier.ap < 2)
            {
                moveUI.moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("1");
                moveUI.moveTypeDropdown.value = 2;
            }
        }

        //block cover for JA
        if (activeSoldier.IsWearingJuggernautArmour(false))
            moveUI.coverToggle.interactable = false;

        //block melee toggle if within engage distance of enemy
        if (activeSoldier.ClosestEnemyVisible() != null && activeSoldier.PhysicalObjectWithinMeleeRadius(activeSoldier.ClosestEnemyVisible()) || suppressed)
            moveUI.meleeToggle.interactable = false;

        /*//block planner if already moved
        if (!activeSoldier.usedMP)
            moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Planner Donate");
        else
            moveTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();*/

        //prefill movement position inputs with current position
        moveUI.xPos.placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
        moveUI.yPos.placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
        moveUI.zPos.placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();
        
        //block options and show start location if broken
        if (activeSoldier.IsBroken())
        {
            moveUI.startX.text = activeSoldier.startX.ToString();
            moveUI.startY.text = activeSoldier.startY.ToString();
            moveUI.startZ.text = activeSoldier.startZ.ToString();
            moveUI.coverToggle.interactable = false;
            moveUI.meleeToggle.interactable = false;
            moveUI.startlocationUI.SetActive(true);
        }
        else
            moveUI.startlocationUI.SetActive(false);

        game.UpdateMoveUI();
        
        moveUI.gameObject.SetActive(true);
    }
    public void OpenOvermoveUI(string message)
    {
        //play overmove alarm sfx
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
        //play overmove alarm sfx
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
        moveUI.moveTypeDropdown.value = 0;
        moveUI.xPos.text = "";
        moveUI.yPos.text = "";
        moveUI.zPos.text = "";
        moveUI.terrainDropdown.value = 0;
        moveUI.coverToggle.isOn = false;
        moveUI.meleeToggle.isOn = false;
        moveUI.fallInput.text = "";
        clearMoveFlag = false;
    }
    public void CloseMoveUI()
    {
        ClearMoveUI();
        moveUI.gameObject.SetActive(false);
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
            soundManager.PlayOverrideAlarm(); //play override alarm sfx
    }
    public void ConfirmSuppressionMove()
    {
        if (OverrideKey())
        {
            OpenMoveUI(true);
            CloseSuppressionMoveUI();
        }
        else
            soundManager.PlayOverrideAlarm(); //play override alarm sfx
    }










    //configure functions - menu
    public void AddGroundInventorySourceButton()
    {
        InventorySourcePanel groundInventoryPanel = Instantiate(inventoryPanelGroundPrefab, configUI.externalItemSourcesPanel.transform).Init(null);
        InventorySourceIcon groundInventoryButton = Instantiate(groundInventoryIconPrefab.GetComponent<InventorySourceIcon>().Init(groundInventoryPanel), inventorySourceIconsUI.transform);
        foreach (Item i in game.FindNearbyItems())
        {
            if (i.transform.parent == null)
            {
                ItemSlot itemSlot = Instantiate(itemSlotPrefab, groundInventoryPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>();
                itemSlot.AssignItemIcon(Instantiate(itemIconPrefab, itemSlot.transform).GetComponent<ItemIcon>().Init(i, itemSlot));
            }
        }

        if (activeSoldier.roundsFielded == 0 && !activeSoldier.usedAP)
            groundInventoryButton.Grey("Spawn Config");
        else
            groundInventoryButton.UnGrey();
    }
    public void AddAllyInventorySourceButtons()
    {
        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsFielded() && activeSoldier.PhysicalObjectWithinItemRadius(s) && (activeSoldier.IsSameTeamAs(s) || s.IsUnconscious() || s.IsDead()) && s.IsInteractable())
            {
                InventorySourceIcon allyInventoryButton = Instantiate(allyInventoryIconPrefab.GetComponent<InventorySourceIconAlly>().Init(s, Instantiate(inventoryPanelAllyPrefab, configUI.externalItemSourcesPanel.transform).Init(s)), inventorySourceIconsUI.transform);

                if (activeSoldier.IsBlind())
                    allyInventoryButton.Grey("Blind");
                else if (activeSoldier.roundsFielded == 0 && !activeSoldier.usedAP)
                    allyInventoryButton.Grey("Spawn Config");
                else
                    allyInventoryButton.UnGrey();
            }
        }
    }
    public void AddPOIInventorySourceButtons()
    {
        bool poisNearby = false;

        foreach (GoodyBox gb in game.AllGoodyBoxes())
        {
            if (activeSoldier.PhysicalObjectWithinItemRadius(gb))
            {
                poisNearby = true;
                InventorySourceIcon gbInventoryButton = Instantiate(gbInventoryIconPrefab.GetComponent<InventorySourceIconGoodyBox>().Init(gb, Instantiate(inventoryPanelGoodyBoxPrefab, configUI.externalItemSourcesPanel.transform).Init(gb)), inventorySourceIconsUI.transform);

                if (activeSoldier.IsBlind())
                    gbInventoryButton.Grey("Blind");
                else
                    gbInventoryButton.UnGrey();
            }
            else if (activeSoldier.IsLocater()) //locater ability
            {
                bool gbIsRevealed = false;
                foreach (Soldier s in game.AllFieldedFriendlySoldiers())
                {
                    if (s.IsAbleToSee() && s.PhysicalObjectWithinMaxRadius(gb))
                        gbIsRevealed = true;
                }

                if (gbIsRevealed)
                {
                    InventorySourceIcon gbInventoryButton = Instantiate(gbInventoryIconPrefab.GetComponent<InventorySourceIconGoodyBox>().Init(gb, Instantiate(inventoryPanelGoodyBoxPrefab, configUI.externalItemSourcesPanel.transform).Init(gb)), inventorySourceIconsUI.transform);

                    (gbInventoryButton as InventorySourceIconGoodyBox).SetLocated();
                }
                    
            }
        }
                
        foreach (DrugCabinet dc in game.AllDrugCabinets())
        {
            if (activeSoldier.PhysicalObjectWithinItemRadius(dc))
            {
                poisNearby = true;
                InventorySourceIcon dcInventoryButton = Instantiate(dcInventoryIconPrefab.GetComponent<InventorySourceIconDrugCabinet>().Init(dc, Instantiate(inventoryPanelGoodyBoxPrefab, configUI.externalItemSourcesPanel.transform).Init(dc)), inventorySourceIconsUI.transform);
                
                if (activeSoldier.IsBlind())
                    dcInventoryButton.Grey("Blind");
                else
                    dcInventoryButton.UnGrey();
            }
            else if (activeSoldier.IsLocater()) //locater ability
            {
                bool dcIsRevealed = false;
                foreach (Soldier s in game.AllFieldedFriendlySoldiers())
                {
                    if (s.IsAbleToSee() && s.PhysicalObjectWithinMaxRadius(dc))
                        dcIsRevealed = true;
                }

                if (dcIsRevealed)
                {
                    InventorySourceIcon dcInventoryButton = Instantiate(dcInventoryIconPrefab.GetComponent<InventorySourceIconDrugCabinet>().Init(dc, Instantiate(inventoryPanelGoodyBoxPrefab, configUI.externalItemSourcesPanel.transform).Init(dc)), inventorySourceIconsUI.transform);

                    (dcInventoryButton as InventorySourceIconDrugCabinet).SetLocated();
                }

            }
        }

        if (activeSoldier.IsAbleToSee() && poisNearby)
        {
            //play configure near GB dialogue
            soundManager.PlaySoldierConfigNearGB(activeSoldier);
        }
    }
    public void AddGlobalInventorySourceButton()
    {
        Instantiate(globalInventoryIconPrefab.GetComponent<InventorySourceIcon>().Init(configUI.allItemsPanel), inventorySourceIconsUI.transform);
    }
    public void OpenConfigureUI()
    {
        //populate active soldier inventory
        configUI.activeSoldierInventory.Init(activeSoldier);

        if (configureButton.GetComponentInChildren<TextMeshProUGUI>().text.Equals("Spawn Config"))
        {
            //add global button
            AddGlobalInventorySourceButton();
        }
        else
        {
            if (OverrideView)
            {
                //add global button
                AddGlobalInventorySourceButton();
            }

            //populate ground item icons
            AddGroundInventorySourceButton();

            //populate ally icons
            AddAllyInventorySourceButtons();

            //populate gb and dc icons
            AddPOIInventorySourceButtons();
        }

        configUI.gameObject.SetActive(true);
    }
    public void ClearConfigureUI()
    {
        //reset ap counter to 0
        configUI.apCost.text = "0";

        //clear all display panels
        foreach (Transform child in configUI.inventorySourceIconPanel.transform.Find("Viewport").Find("Contents"))
            Destroy(child.gameObject);

        //destroy all item source panels
        foreach (Transform child in configUI.externalItemSourcesPanel.transform)
            if (child.GetComponent<InventorySourcePanel>() != null)
            {
                if (child.name.Equals("AllItemsPanel"))
                    child.gameObject.SetActive(false);
                else
                    Destroy(child.gameObject);
            }
    }
    public void OpenInventoryPanel(InventorySourcePanel inventoryPanel)
    {
        inventoryPanel.gameObject.SetActive(true);
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
        configUI.gameObject.SetActive(false);
    }











    //dipelec functions
    public void OpenDipElecUI()
    {
        //generate terminal list
        List<TMP_Dropdown.OptionData> terminalDetailsList = new();
        foreach (Terminal t in game.AllTerminals())
        {
            TMP_Dropdown.OptionData terminalDetails = null;
            if (t.terminalEnabled && activeSoldier.PhysicalObjectWithinMeleeRadius(t))
            {
                dipelecUI.allTerminalIds.Add(t.id);
                terminalDetails = new($"X:{t.X} Y:{t.Y} Z:{t.Z}", t.poiPortrait, Color.white);
                terminalDetailsList.Add(terminalDetails);
            }
        }
        dipelecUI.dipElecTerminalDropdown.AddOptions(terminalDetailsList);

        game.UpdateDipElecUI();
        dipelecUI.gameObject.SetActive(true);
    }
    public void CloseDipElecUI()
    {
        ClearDipElecUI();
        dipelecUI.gameObject.SetActive(false);
    }
    public void ClearDipElecUI()
    {
        clearDipelecFlag = true;
        dipelecUI.allTerminalIds.Clear();

        dipelecUI.dipElecTerminalDropdown.value = 0;
        dipelecUI.dipElecTerminalDropdown.ClearOptions();

        dipelecUI.dipElecTypeDropdown.value = 0;
        dipelecUI.dipElecTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();

        dipelecUI.dipElecLevelDropdown.value = 0;
        dipelecUI.successChanceDisplay.text = "";
        clearDipelecFlag = false;
    }
    public void OpenDipelecResultUI()
    {
        dipelecResultUI.SetActive(true);
    }
    public void ClearDipelecResultUI()
    {
        foreach (Transform child in dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"))
            Destroy(child.gameObject);
    }
    public void CloseDipelecResultUI()
    {
        if (OverrideKey())
        {
            if (dipelecResultUI.transform.Find("OptionPanel").Find("Rewards").childCount > 1)
                Destroy(dipelecResultUI.transform.Find("OptionPanel").Find("Rewards").GetChild(dipelecResultUI.transform.Find("OptionPanel").Find("Rewards").childCount - 1).gameObject);
            else
            {
                UnfreezeTimer();
                ClearDipelecResultUI();
                dipelecResultUI.SetActive(false);
                activeSoldier.PerformLoudAction(30);
            }
        }
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
        
        overwatchUI.radius.placeholder.GetComponent<TextMeshProUGUI>().text = $"Max {activeSoldier.SRColliderFull.radius}";
        overwatchUI.radius.GetComponent<MinMaxInputController>().max = Mathf.RoundToInt(activeSoldier.SRColliderFull.radius);
        
        //allow guardsman to overwatch up to 180 degrees
        if (activeSoldier.IsGuardsman())
        {
            overwatchUI.arc.placeholder.GetComponent<TextMeshProUGUI>().text = $"Max 180";
            overwatchUI.arc.GetComponent<MinMaxInputController>().max = 180;
        }
        else
        {
            overwatchUI.arc.placeholder.GetComponent<TextMeshProUGUI>().text = $"Max 90";
            overwatchUI.arc.GetComponent<MinMaxInputController>().max = 90;
        }

        //set ap cost
        if (activeSoldier.ap < 2)
            overwatchUI.apCost.text = "2";
        else
            overwatchUI.apCost.text = $"{activeSoldier.ap}";

        overwatchUI.gameObject.SetActive(true);
    }
    public void ClearOverwatchUI()
    {
        overwatchUI.xPos.text = "";
        overwatchUI.yPos.text = "";
        overwatchUI.radius.text = "";
        overwatchUI.arc.text = "";
    }
    public void CloseOverwatchUI()
    {
        ClearOverwatchUI();
        overwatchUI.gameObject.SetActive(false);
    }
    public void OpenLastandicideConfirmUI()
    {
        lastandicideConfirmUI.SetActive(true);
    }
    public void CloseLastandicideConfirmUI()
    {
        lastandicideConfirmUI.SetActive(false);
    }







    //disarm functions
    public void OpenDisarmUI()
    {
        List<TMP_Dropdown.OptionData> disarmOptionDataList = new();
        TMP_Dropdown.OptionData disarmOptionData;

        foreach (IAmDisarmable disarmable in game.AllDisarmable())
        {
            if (activeSoldier.PhysicalObjectWithinMeleeRadius((PhysicalObject)disarmable))
            {
                disarmOptionData = new(disarmable.Id, disarmable.DisarmImage, Color.white);
                disarmOptionDataList.Add(disarmOptionData);
            }
        }

        disarmUI.transform.Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().AddOptions(disarmOptionDataList);
        disarmUI.SetActive(true);
    }
    public void CloseDisarmUI()
    {
        ClearDisarmUI();
        disarmUI.gameObject.SetActive(false);
    }
    public void ClearDisarmUI()
    {
        disarmUI.transform.Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
    }










    //drag functions
    public void OpenDragUI()
    {
        List<TMP_Dropdown.OptionData> dragOptionDataList = new();
        TMP_Dropdown.OptionData dragOptionData;

        foreach (Soldier soldier in game.AllSoldiers())
        {
            if (activeSoldier.PhysicalObjectWithinMeleeRadius(soldier) && !soldier.IsWearingJuggernautArmour(false) && (soldier.IsSameTeamAs(activeSoldier) || soldier.IsDead() || soldier.IsPlayingDead() || soldier.IsUnconscious() || soldier.IsMeleeControlledBy(activeSoldier)))
            {
                dragOptionData = new(soldier.Id, soldier.soldierPortrait, Color.white);
                dragOptionDataList.Add(dragOptionData);
            }
        }

        dragUI.targetDropdown.AddOptions(dragOptionDataList);
        dragUI.gameObject.SetActive(true);
    }
    public void CloseDragUI()
    {
        ClearDragUI();
        dragUI.gameObject.SetActive(false);
    }
    public void ClearDragUI()
    {
        dragUI.pressCount = 0;
        dragUI.legitMove = false;
        dragUI.legitDrop = false;
        dragUI.targetDropdown.ClearOptions();
        dragUI.targetDropdown.interactable = true;
        dragUI.xPos.text = "";
        dragUI.yPos.text = "";
        dragUI.zPos.text = "";
        dragUI.terrainDropdown.value = 0;
        dragUI.xPosD.text = "";
        dragUI.yPosD.text = "";
        dragUI.zPosD.text = "";
        dragUI.terrainDropdownD.value = 0;
        dragUI.moveObjects.SetActive(false);
        dragUI.dropObjects.SetActive(false);
        dragUI.backButton.SetActive(true);
        dragUI.moveOutOfRange.SetActive(false);
        dragUI.dropOutOfRange.SetActive(false);
    }












    //damage event functions - menu
    public void OpenDamageEventUI()
    {
        FreezeTimer();
        damageEventUI.damageEventTypeDropdown.ClearOptions();

        //generate damage event type dropdown
        List<TMP_Dropdown.OptionData> damageEventTypeDetails = new()
        {
        new TMP_Dropdown.OptionData("Fall Damage"),
        new TMP_Dropdown.OptionData("Structural Collapse"),
        new TMP_Dropdown.OptionData("Other"),
        };
        if (activeSoldier.IsBloodletter() && !activeSoldier.bloodLettedThisTurn)
            damageEventTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Bloodletting</color>"));
        damageEventUI.damageEventTypeDropdown.AddOptions(damageEventTypeDetails);

        //prefill movement position inputs with current position
        damageEventUI.xPos.placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.X.ToString();
        damageEventUI.yPos.placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Y.ToString();
        damageEventUI.zPos.placeholder.GetComponent<TextMeshProUGUI>().text = activeSoldier.Z.ToString();

        /*//block bloodletter if already bloodletted this turn
        if (activeSoldier.IsBloodletter() && activeSoldier.bloodLettedThisTurn)
            damageEventTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Bloodletting");
        else
            damageEventTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();*/

        damageEventUI.gameObject.SetActive(true);
    }
    public void ClearDamageEventUI()
    {
        clearDamageEventFlag = true;
        damageEventUI.damageEventTypeDropdown.value = 0;
        damageEventUI.fallInput.text = "";
        damageEventUI.structureHeight.text = "";
        damageEventUI.otherInput.text = "";
        damageEventUI.damageSource.text = "";
        damageEventUI.xPos.text = "";
        damageEventUI.yPos.text = "";
        damageEventUI.zPos.text = "";
        damageEventUI.terrainDropdown.value = 0;
        clearDamageEventFlag = false;
    }
    public void CloseDamageEventUI()
    {
        UnfreezeTimer();
        ClearDamageEventUI();
        damageEventUI.gameObject.SetActive(false);
    }
    public void CheckDamageEventType()
    {
        damageEventUI.transform.Find("FallDistance").gameObject.SetActive(false);
        damageEventUI.transform.Find("Other").gameObject.SetActive(false);
        damageEventUI.transform.Find("StructureHeight").gameObject.SetActive(false);
        damageEventUI.transform.Find("DamageSource").gameObject.SetActive(false);
        damageEventUI.transform.Find("Location").gameObject.SetActive(false);

        if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Fall"))
        {
            damageEventUI.fallDistanceUI.SetActive(true);
            damageEventUI.locationUI.SetActive(true);
        }
        else if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Collapse"))
        {
            damageEventUI.transform.Find("StructureHeight").gameObject.SetActive(true);
            damageEventUI.transform.Find("Location").gameObject.SetActive(true);
        }
        else if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Other"))
        {
            damageEventUI.transform.Find("Other").gameObject.SetActive(true);
            damageEventUI.transform.Find("DamageSource").gameObject.SetActive(true);
        }
        else if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Bloodletting")) {}
    }













    //politician functions
    public void OpenPoliticianUI()
    {
        politicianUI.SetActive(true);
    }
    public void ClosePoliticianUI()
    {
        politicianUI.SetActive(false);
    }
    public void ConfirmPoliticianUI()
    {
        activeSoldier.politicianUsed = true;
        ClosePoliticianUI();
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
                XpAlert xpAlert = child.GetComponent<XpAlert>();

                //destroy duplicate avoidances/detections against same detecting/avoiding soldier
                if ((xpAlert.description.text.Contains("Avoided") && xpAlert.description.text == xpDescription && xpAlert.soldier == soldier) || (xpAlert.description.text.Contains("Detected") && xpAlert.description.text == xpDescription && xpAlert.soldier == soldier))
                {
                    print($"destroying {xpDescription}");
                    Destroy(xpAlert.gameObject);
                }
            }

            if (soldier.IsConscious())
            {
                Instantiate(xpAlertPrefab, xpLogUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content")).Init(soldier, xp, xpDescription, learnerEnabled);
            }
            else
                print($"{soldier.soldierName} cannot receive xp unconscious.");
        }
    }

    public void OpenXpLogUI()
    {
        if (OverrideKey())
        {
            Transform xpAlerts = xpLogUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");
            foreach (Transform child in xpAlerts)
            {
                if (child.GetComponent<SoldierAlert>().soldier.IsOnturn())
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
                xpAlertsList.Add(child);
                child.GetComponent<XpAlert>().Resolve();
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
                Soldier xpReciever = child.GetComponent<SoldierAlert>().soldier;
                //destroy xp alerts for soldiers who are dead or unconscious
                if (!xpReciever.IsConscious())
                    Destroy(child.gameObject);
                else if (xpReciever.IsOnturnAndAlive())
                    display = true;
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
        PromotionAlert promotionAlert = Instantiate(promotionAlertPrefab, promotionUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        promotionAlert.SetSoldier(soldier);
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
            if (!child.GetComponent<PromotionAlert>().promotionComplete)
                confirm = false;

            promotionAlertsList.Add(child);
            FileUtility.WriteToReport($"{child.GetComponent<SoldierAlert>().soldier.soldierName} promotion: {child.GetComponent<SoldierAlert>().soldier.rank}"); //write to report
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
    public void OpenCannotUseItemUI(string message)
    {
        cannotUseItemUI.transform.Find("OptionPanel").Find("Message").Find("Text").GetComponent<TextMeshProUGUI>().text = message;
        cannotUseItemUI.SetActive(true);
    }
    public void CloseCannotUseItemUI()
    {
        cannotUseItemUI.SetActive(false);
    }
    public void OpenThrowUI(UseItemUI useItemUI)
    {
        throwUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        throwUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        throwUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        throwUI.transform.Find("OptionPanel").Find("ItemName").Find("Text").GetComponent<TextMeshProUGUI>().text = $"Throwing {useItemUI.itemUsed.itemName} from {useItemUI.itemUsedFromSlotName} slot.";
        throwUI.SetActive(true);

        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("XPos").GetComponent<LocationInputController>().SetMin(-activeSoldier.ThrowRadius);
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("YPos").GetComponent<LocationInputController>().SetMin(-activeSoldier.ThrowRadius);
    }
    public void ClearThrowUI()
    {
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("XPos").GetComponent<TMP_InputField>().interactable = true;
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("YPos").GetComponent<TMP_InputField>().interactable = true;
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("ZPos").GetComponent<TMP_InputField>().interactable = true;
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("XPos").GetComponent<TMP_InputField>().text = "";
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("YPos").GetComponent<TMP_InputField>().text = "";
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        throwUI.transform.Find("OptionPanel").Find("ScatteredOffMap").gameObject.SetActive(false);
        throwUI.transform.Find("PressedOnce").gameObject.SetActive(false);
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("FinalPosition").gameObject.SetActive(false);
        throwUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("PreciseThrow").gameObject.SetActive(false);
    }
    public void CloseThrowUI()
    {
        ClearThrowUI();
        throwUI.SetActive(false);
    }
    public void OpenDropUI(UseItemUI useItemUI)
    {
        dropUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        dropUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        dropUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        dropUI.transform.Find("OptionPanel").Find("ItemName").Find("Text").GetComponent<TextMeshProUGUI>().text = $"Dropping {useItemUI.itemUsed.itemName} from {useItemUI.itemUsedFromSlotName} slot.";
        dropUI.SetActive(true);
    }
    public void ClearDropUI()
    {
        dropUI.transform.Find("OptionPanel").Find("DropTarget").Find("XPos").GetComponent<TMP_InputField>().interactable = true;
        dropUI.transform.Find("OptionPanel").Find("DropTarget").Find("YPos").GetComponent<TMP_InputField>().interactable = true;
        dropUI.transform.Find("OptionPanel").Find("DropTarget").Find("ZPos").GetComponent<TMP_InputField>().interactable = true;
        dropUI.transform.Find("OptionPanel").Find("DropTarget").Find("XPos").GetComponent<TMP_InputField>().text = "";
        dropUI.transform.Find("OptionPanel").Find("DropTarget").Find("YPos").GetComponent<TMP_InputField>().text = "";
        dropUI.transform.Find("OptionPanel").Find("DropTarget").Find("ZPos").GetComponent<TMP_InputField>().text = "";
    }
    public void CloseDropUI()
    {
        ClearDropUI();
        dropUI.SetActive(false);
    }
    public void OpenUseItemUI(Item itemUsed, string itemUsedFromSlotName, ItemIcon linkedIcon, int ap)
    {
        useItemUI.transform.Find("OptionPanel").Find("Target").gameObject.SetActive(true);

        TMP_Dropdown targetDropdown = useItemUI.transform.Find("OptionPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>();
        targetDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> targetOptionDataList = new();
        useItemUI.GetComponent<UseItemUI>().itemUsed = itemUsed;
        useItemUI.GetComponent<UseItemUI>().itemUsedIcon = linkedIcon;
        useItemUI.GetComponent<UseItemUI>().itemUsedFromSlotName = itemUsedFromSlotName;
        useItemUI.transform.Find("OptionPanel").Find("Message").Find("Text").GetComponent<TextMeshProUGUI>().text = itemUsed.itemName switch
        {
            "Ammo_AR" => "Reload Assault Rifle?",
            "Ammo_LMG" => "Reload Light Machine Gun?",
            "Ammo_Ri" => "Reload Rifle?",
            "Ammo_Sh" => "Reload Shotgun?",
            "Ammo_SMG_Pi" => "Reload Sub-Machine Gun or Pistol?",
            "Ammo_Sn" => "Reload Sniper?",
            "Binoculars" => $"Use binoculars ({itemUsed.GetBinocularMode(itemUsedFromSlotName)} Mode)",
            "Claymore" => "Place Claymore?",
            "Deployment_Beacon" => "Place Deployment Beacon?",
            "E_Tool" => "Dig?",
            "Food_Pack" => "Consume food pack?",
            "Grenade_Flashbang" => "Remove pin and throw flashbang?",
            "Grenade_Frag" => "Remove pin and throw frag?",
            "Grenade_Smoke" => "Remove pin and throw smoke?",
            "Grenade_Tabun" => "Remove pin and throw tabun?",
            "Medikit_Large" => "Use Large Medikit?",
            "Medikit_Medium" => "Use Medium Medikit?",
            "Medikit_Small" => "Use Small Medikit?",
            "Poison_Satchel" => "Administer Posion?",
            "Riot_Shield" => "Orient riot shield?",
            "Syringe_Amphetamine" => "Administer Amphetamine?",
            "Syringe_Androstenedione" => "Administer Androstenedione?",
            "Syringe_Cannabinoid" => "Administer Cannabinoid?",
            "Syringe_Danazol" => "Administer Danazol?",
            "Syringe_Glucocorticoid" => "Administer Glucocorticoid?",
            "Syringe_Modafinil" => "Administer Modafinil?",
            "Syringe_Shard" => "Administer Shard?",
            "Syringe_Trenbolone" => "Administer Trenbolone?",
            "Syringe_Unlabelled" => "Administer Unlabelled Syringe?",
            "Thermal_Camera" => "Place Thermal Camera?",
            "UHF_Radio" => "Call UHF strike?",
            "ULF_Radio" => "Attempt to use ULF radio?",
            "Water_Canteen" => "Drink water?",
            _ => $"Unrecognised item ({itemUsed.itemName})",
        };
        useItemUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = ap.ToString();

        if (itemUsed.itemName.Contains("Medikit"))
        {
            foreach (Soldier s in game.AllSoldiers())
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (activeSoldier.IsSameTeamAsIncludingSelf(s) && (s.IsInjured() || s.IsTraumatised()) && activeSoldier.PhysicalObjectWithinMeleeRadius(s))
                    targetOptionData = new(s.Id, s.soldierPortrait, Color.white);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }
            targetDropdown.AddOptions(targetOptionDataList);

            if (targetOptionDataList.Count > 0)
            {
                game.UpdateSoldierUsedOn(useItemUI.GetComponent<UseItemUI>());
                useItemUI.SetActive(true);
            }
        }
        else if (itemUsed.itemName.Contains("Syringe"))
        {
            foreach (Soldier s in game.AllSoldiers())
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (s.IsAlive() && activeSoldier.PhysicalObjectWithinMeleeRadius(s))
                    targetOptionData = new(s.Id, s.soldierPortrait, Color.white);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }
            targetDropdown.AddOptions(targetOptionDataList);

            if (targetOptionDataList.Count > 0)
            {
                game.UpdateSoldierUsedOn(useItemUI.GetComponent<UseItemUI>());
                useItemUI.SetActive(true);
            }
        }
        else if (itemUsed.itemName.Equals("Poison_Satchel"))
        {
            foreach (Item i in game.FindNearbyItems())
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (i.IsPoisonable())
                    targetOptionData = new(i.id, i.itemImage, Color.white);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }
            targetDropdown.AddOptions(targetOptionDataList);

            if (targetOptionDataList.Count > 0)
            {
                game.UpdateItemUsedOn(useItemUI.GetComponent<UseItemUI>());
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
                        targetOptionData = new(i.id, i.itemImage, Color.white);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<UseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("LMG"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsLMG())
                        targetOptionData = new(i.id, i.itemImage, Color.white);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<UseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("Ri"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsRifle())
                        targetOptionData = new(i.id, i.itemImage, Color.white);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<UseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("Sh"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsShotgun())
                        targetOptionData = new(i.id, i.itemImage, Color.white);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<UseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("SMG"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsSMG() || i.IsPistol())
                        targetOptionData = new(i.id, i.itemImage, Color.white);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<UseItemUI>());
                    useItemUI.SetActive(true);
                }
            }
            else if (itemUsed.itemName.Contains("Sn"))
            {
                foreach (Item i in itemUsed.owner.Inventory.AllItems)
                {
                    TMP_Dropdown.OptionData targetOptionData = null;
                    if (i.IsSniper())
                        targetOptionData = new(i.id, i.itemImage, Color.white);

                    if (targetOptionData != null)
                        targetOptionDataList.Add(targetOptionData);
                }
                targetDropdown.AddOptions(targetOptionDataList);

                if (targetOptionDataList.Count > 0)
                {
                    game.UpdateItemUsedOn(useItemUI.GetComponent<UseItemUI>());
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
    public void OpenDropThrowItemUI(string throwOrDrop, Item itemThrown, string itemThrownFromSlotName, ItemIcon linkedIcon)
    {
        int ap = 0;
        if (throwOrDrop.Equals("throw") || (throwOrDrop.Equals("drop") && !itemThrown.whereEquipped.Contains("Hand")))
            ap++;

        if (activeSoldier.CheckAP(ap))
        {
            if (throwOrDrop.Equals("throw"))
            {
                if (activeSoldier.IsAbleToSee())
                    dropThrowItemUI.transform.Find("OptionPanel").Find("Message").GetComponentInChildren<TextMeshProUGUI>().text = $"Throw item up to {activeSoldier.ThrowRadius}cm?";
            }
            else if (throwOrDrop.Equals("drop"))
            {
                dropThrowItemUI.transform.Find("OptionPanel").Find("Message").GetComponentInChildren<TextMeshProUGUI>().text = $"Drop item up to 3cm?";
            }

            dropThrowItemUI.GetComponent<UseItemUI>().itemUsed = itemThrown;
            dropThrowItemUI.GetComponent<UseItemUI>().itemUsedIcon = linkedIcon;
            dropThrowItemUI.GetComponent<UseItemUI>().itemUsedFromSlotName = itemThrownFromSlotName;
            dropThrowItemUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = ap.ToString();
            dropThrowItemUI.SetActive(true);
        }
    }
    public void CloseDropThrowItemUI()
    {
        dropThrowItemUI.SetActive(false);
    }
    public void OpenEtoolResultUI()
    {
        etoolResultUI.SetActive(true);
    }
    public void CloseEtoolResultUI()
    {
        etoolResultUI.SetActive(false);
    }
    public void OpenUseULFUI(string effect, Item ulfUsed)
    {
        if (activeSoldier.CheckAP(3))
        {
            if (activeSoldier.HandsFreeToUseItem(ulfUsed))
            {
                FileUtility.WriteToReport($"{activeSoldier.soldierName} attempts to {effect} with ulf."); //write to report

                useULFUI.GetComponent<UseItemUI>().itemUsed = ulfUsed;
                useULFUI.GetComponent<UseItemUI>().itemUsedFromSlotName = effect;
                useULFUI.transform.Find("OptionPanel").Find("Message").Find("Text").GetComponent<TextMeshProUGUI>().text = effect switch
                {
                    "spy" => "Attempt to spy with ULF radio?",
                    "jam" => "Attempt to jam communications with ULF radio?",
                    _ => $"Unrecognised action ({effect})",
                };
                useULFUI.SetActive(true);
            }
        }
    }
    public void CloseUseULFUI()
    {
        useULFUI.SetActive(false);
    }
    public void OpenULFResultUI(string message)
    {
        //play ulf dialogue
        soundManager.PlayULFResult(message);

        ULFResultUI.transform.Find("OptionPanel").Find("Result").Find("Text").GetComponent<TextMeshProUGUI>().text = message;
        ULFResultUI.SetActive(true);
    }
    public void CloseULFResultUI()
    {
        ULFResultUI.SetActive(false);
    }
    public void CloseUHFUI()
    {
        ClearUHFUI();
        UHFUI.SetActive(false);
    }
    public void ClearUHFUI()
    {
        UHFUI.transform.Find("OptionPanel").Find("StrikeOptions").Find("StrikeOptionsDropdown").GetComponent<TMP_Dropdown>().ClearOptions();
        UHFUI.transform.Find("OptionPanel").Find("StrikeOptions").Find("StrikeOptionsDropdown").GetComponent<TMP_Dropdown>().interactable = true;
        UHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("XPos").GetComponent<TMP_InputField>().text = "";
        UHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("YPos").GetComponent<TMP_InputField>().text = "";
        UHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        UHFUI.transform.Find("OptionPanel").Find("TotalMiss").Find("Text").GetComponent<TextMeshProUGUI>().text = "";
        UHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("XPos").GetComponent<TMP_InputField>().interactable = true;
        UHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("YPos").GetComponent<TMP_InputField>().interactable = true;
        UHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("ZLabel").gameObject.SetActive(false);
        UHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("ZPos").gameObject.SetActive(false);
        UHFUI.transform.Find("OptionPanel").Find("TotalMiss").gameObject.SetActive(false);
        UHFUI.transform.Find("PressedOnce").gameObject.SetActive(false);
    }
    public void OpenUHFUI(UseItemUI useItemUI)
    {
        //play uhf dial up dialogue
        soundManager.PlayUHFDialUp();

        UHFUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        UHFUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        UHFUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        //Get strike options on and below dipelec score
        List<TMP_Dropdown.OptionData> strikeOptions = new();
        foreach (string strikeName in useItemUI.itemUsed.GetUHFStrikes())
        {
            TMP_Dropdown.OptionData optionData = new(strikeName);
            strikeOptions.Add(optionData);
        }
        UHFUI.transform.Find("OptionPanel").Find("StrikeOptions").Find("StrikeOptionsDropdown").GetComponent<TMP_Dropdown>().AddOptions(strikeOptions);

        UHFUI.SetActive(true);
    }
    public void CloseRiotShieldUI()
    {
        ClearRiotShieldUI();
        riotShieldUI.SetActive(false);
    }
    public void ClearRiotShieldUI()
    {
        riotShieldUI.transform.Find("OptionPanel").Find("RiotShieldTarget").Find("XPos").GetComponent<TMP_InputField>().text = "";
        riotShieldUI.transform.Find("OptionPanel").Find("RiotShieldTarget").Find("YPos").GetComponent<TMP_InputField>().text = "";
    }
    public void OpenRiotShieldUI(UseItemUI useItemUI)
    {
        riotShieldUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        riotShieldUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        riotShieldUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        riotShieldUI.SetActive(true);
    }
    public void OpenGrenadeUI(UseItemUI useItemUI)
    {
        //play grenade use dialogue
        if (useItemUI.itemUsed.IsTabun())
            soundManager.PlaySoldierUseTabun(activeSoldier);
        else if (useItemUI.itemUsed.IsSmoke())
            soundManager.PlaySoldierUseSmoke(activeSoldier);
        else
            soundManager.PlaySoldierUseGrenade(activeSoldier);


        grenadeUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        grenadeUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        grenadeUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;
        grenadeUI.transform.Find("OptionPanel").Find("GrenadeType").GetComponentInChildren<TextMeshProUGUI>().text = useItemUI.itemUsed.itemName;

        grenadeUI.SetActive(true);
    }
    public void ClearGrenadeUI()
    {
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("XPos").GetComponent<TMP_InputField>().interactable = true;
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("YPos").GetComponent<TMP_InputField>().interactable = true;
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("ZPos").GetComponent<TMP_InputField>().interactable = true;
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("XPos").GetComponent<TMP_InputField>().text = "";
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("YPos").GetComponent<TMP_InputField>().text = "";
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("FinalPosition").gameObject.SetActive(false);
        grenadeUI.transform.Find("OptionPanel").Find("Target").Find("PreciseThrow").gameObject.SetActive(false);
        grenadeUI.transform.Find("OptionPanel").Find("ScatteredOffMap").gameObject.SetActive(false);
        grenadeUI.transform.Find("OptionPanel").Find("PressedOnce").gameObject.SetActive(false);
    }
    public void CloseGrenadeUI()
    {
        ClearGrenadeUI();
        grenadeUI.SetActive(false);
    }
    public void OpenClaymoreUI(UseItemUI useItemUI)
    {
        //play claymore placement sfx
        soundManager.PlayPlaceClaymore();
        //play claymore placement dialogue
        soundManager.PlaySoldierPlaceClaymore(activeSoldier);

        claymoreUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        claymoreUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        claymoreUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        claymoreUI.SetActive(true);
    }
    public void ClearClaymoreUI()
    {
        claymoreUI.transform.Find("OptionPanel").Find("OutOfRange").gameObject.SetActive(false);
        claymoreUI.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("XPos").GetComponent<TMP_InputField>().text = "";
        claymoreUI.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("YPos").GetComponent<TMP_InputField>().text = "";
        claymoreUI.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        claymoreUI.transform.Find("OptionPanel").Find("ClaymoreFacing").Find("XPos").GetComponent<TMP_InputField>().text = "";
        claymoreUI.transform.Find("OptionPanel").Find("ClaymoreFacing").Find("YPos").GetComponent<TMP_InputField>().text = "";
    }
    public void CloseClaymoreUI()
    {
        ClearClaymoreUI();
        claymoreUI.SetActive(false);
    }
    public void OpenDeploymentBeaconUI(UseItemUI useItemUI)
    {
        deploymentBeaconUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        deploymentBeaconUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        deploymentBeaconUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        deploymentBeaconUI.SetActive(true);
    }
    public void ClearDeploymentBeaconUI()
    {
        deploymentBeaconUI.transform.Find("OptionPanel").Find("OutOfRange").gameObject.SetActive(false);
        deploymentBeaconUI.transform.Find("OptionPanel").Find("BeaconPlacing").Find("XPos").GetComponent<TMP_InputField>().text = "";
        deploymentBeaconUI.transform.Find("OptionPanel").Find("BeaconPlacing").Find("YPos").GetComponent<TMP_InputField>().text = "";
        deploymentBeaconUI.transform.Find("OptionPanel").Find("BeaconPlacing").Find("ZPos").GetComponent<TMP_InputField>().text = "";
    }
    public void CloseDeploymentBeaconUI()
    {
        ClearDeploymentBeaconUI();
        deploymentBeaconUI.SetActive(false);
    }
    public void OpenBinocularsUI(Item binocs, ItemIcon binocsIcon, string useMode)
    {
        binocularsUI.binocularsUsed = binocs;
        binocularsUI.binocularsUsedIcon = binocsIcon;
        binocularsUI.binocularMode = useMode;

        CloseSoldierStatsUI();
        binocularsUI.gameObject.SetActive(true);
    }
    public void ClearBinocularsUI()
    {
        binocularsUI.xPos.text = string.Empty;
        binocularsUI.yPos.text = string.Empty;
    }
    public void CloseBinocularsUI()
    {
        ClearBinocularsUI();
        binocularsUI.gameObject.SetActive(false);
    }
    public void OpenThermalCamUI(UseItemUI useItemUI)
    {
        thermalCamUI.GetComponent<UseItemUI>().itemUsed = useItemUI.itemUsed;
        thermalCamUI.GetComponent<UseItemUI>().itemUsedIcon = useItemUI.itemUsedIcon;
        thermalCamUI.GetComponent<UseItemUI>().itemUsedFromSlotName = useItemUI.itemUsedFromSlotName;

        thermalCamUI.SetActive(true);
    }
    public void ClearThermalCamUI()
    {
        thermalCamUI.transform.Find("OptionPanel").Find("OutOfRange").gameObject.SetActive(false);
        thermalCamUI.transform.Find("OptionPanel").Find("CamPlacing").Find("XPos").GetComponent<TMP_InputField>().text = "";
        thermalCamUI.transform.Find("OptionPanel").Find("CamPlacing").Find("YPos").GetComponent<TMP_InputField>().text = "";
        thermalCamUI.transform.Find("OptionPanel").Find("CamPlacing").Find("ZPos").GetComponent<TMP_InputField>().text = "";
        thermalCamUI.transform.Find("OptionPanel").Find("CamFacing").Find("XPos").GetComponent<TMP_InputField>().text = "";
        thermalCamUI.transform.Find("OptionPanel").Find("CamFacing").Find("YPos").GetComponent<TMP_InputField>().text = "";
    }
    public void CloseThermalCamUI()
    {
        ClearThermalCamUI();
        thermalCamUI.SetActive(false);
    }
    public void CreateCloudDissipationAlert(POI cloud)
    {
        GameObject cloudAlert = Instantiate(cloudDissipationAlertPrefab, menuUI.transform);
        cloudAlert.transform.SetSiblingIndex(31);
        cloudAlert.transform.Find("OptionPanel").Find("Message").GetComponentInChildren<TextMeshProUGUI>().text = $"{char.ToUpper(cloud.poiType[0])}{cloud.poiType[1..]} cloud at\nX:{cloud.X}, Y:{cloud.Y}, Z:{cloud.Z}\nhas dissipated.";
    }







    //insert game objects function
    public void OpenOverrideInsertObjectsUI()
    {
        insertObjectsUI.gameObject.SetActive(true);
        game.UpdateInsertGameObjectsUI();
    }
    public void ClearOverrideInsertObjectsUI()
    {
        insertObjectsUI.objectTypeDropdown.value = 0;
        insertObjectsUI.xPos.text = string.Empty;
        insertObjectsUI.yPos.text = string.Empty;
        insertObjectsUI.zPos.text = string.Empty;

        foreach (Transform child in insertObjectsUI.gbItemsPanel) //clear gb
        {
            ItemIconGB itemIcon = child.GetComponent<ItemIconGB>();
            if (itemIcon != null && itemIcon.pickupNumber > 0)
                itemIcon.pickupNumber = 0;
        }
        foreach (Transform child in insertObjectsUI.dcItemsPanel) //clear dc
        {
            ItemIconGB itemIcon = child.GetComponent<ItemIconGB>();
            if (itemIcon != null && itemIcon.pickupNumber > 0)
                itemIcon.pickupNumber = 0;
        }
    }
    public void CloseOverrideInsertObjectsUI()
    {
        ClearOverrideInsertObjectsUI();
        insertObjectsUI.gameObject.SetActive(false);
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
    public bool OverrideView
    {
        get { return overrideView; }
        set 
        { 
            overrideView = value;
            if (overrideView)
                FreezeTimer(true);
            else
                UnfreezeTimer(true);
        }
    }
    public bool TimerStop
    {
        get { return timerStop; }
        set { timerStop = value; }
    }
    public string[][] AllStats
    {
        get
        {
            return allStats;
        }
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
