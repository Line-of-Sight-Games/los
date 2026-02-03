using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    [SerializeField]
    private bool gameRunning;

    public MoveUI moveUI;
    public ShotUI shotUI;
    public MeleeUI meleeUI;
    public ConfigureUI configUI;
    public DamageEventUI damageEventUI;
    public OverwatchUI overwatchUI;
    public InsertObjectsUI insertObjectsUI;

    public bool gameOver, modaTurn, frozenTurn;
    public int maxX, maxY, maxZ;
    public int currentRound, currentTeam, currentTurn, maxRounds, maxTeams, maxTurnTime, tempTeam;
    public int roundsBetweenLeaps = 3;
    public Camera cam;
    public Light sun;
    public GameObject battlefield, bottomPlane, outlineArea;
    public Tuple<Vector3, string, int, int> tempMove;
    public Soldier tempSoldier;
    public List<string> tempDamageSource;

    public Transform allItemsContentUI, inventoryItemsContentUI, groundItemsContentUI, activeItemPanel, allyButtonContentUI;

    public GameManager Init()
    {
        gameRunning = true;
        return this;
    }

    //helper functions - game
    public List<PhysicalObject> AllBattlefieldObjects()
    {
        List<PhysicalObject> battlefieldObjects = new();
        foreach (PhysicalObject obj in FindObjectsByType<PhysicalObject>(default))
        {
            if (obj.OnBattlefield())
                battlefieldObjects.Add(obj);
        }

        return battlefieldObjects;
    }
    public List<Soldier> AllSoldiers()
    {
        return SoldierManager.Instance.allSoldiers;
    }
    public List<Soldier> AllTeam1Soldiers()
    {
        List<Soldier> team1Soldiers = new();
        foreach (Soldier s in AllSoldiers())
        {
            if (s.soldierTeam.Equals(1))
                team1Soldiers.Add(s);
        }

        return team1Soldiers;
    }
    public List<Soldier> AllTeam2Soldiers()
    {
        List<Soldier> team1Soldiers = new();
        foreach (Soldier s in AllSoldiers())
        {
            if (s.soldierTeam.Equals(2))
                team1Soldiers.Add(s);
        }

        return team1Soldiers;
    }
    public List<Soldier> AllFieldedSoldiers()
    {
        return AllBattlefieldObjects().OfType<Soldier>().ToList();
    }
    public List<Soldier> AllFieldedFriendlySoldiers()
    {
        List<Soldier> fieldedAllies = new();
        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.soldierTeam == currentTeam)
                fieldedAllies.Add(s);
        }

        return fieldedAllies;
    }
    public List<Soldier> AllFieldedEnemySoldiers()
    {
        List<Soldier> fieldedAllies = new();
        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.soldierTeam != currentTeam)
                fieldedAllies.Add(s);
        }

        return fieldedAllies;
    }
    public List<IAmDetectable> AllDetectable()
    {
        return AllBattlefieldObjects().OfType<IAmDetectable>().ToList();
    }
    public List<IAmShootable> AllShootable()
    {
        return AllBattlefieldObjects().OfType<IAmShootable>().ToList();
    }
    public List<IHaveInventory> AllHasInventory()
    {
        return AllBattlefieldObjects().OfType<IHaveInventory>().ToList();
    }
    public List<IAmDisarmable> AllDisarmable()
    {
        return AllBattlefieldObjects().OfType<IAmDisarmable>().ToList();
    }
    public List<GoodyBox> AllGoodyBoxes()
    {
        return AllBattlefieldObjects().OfType<GoodyBox>().ToList();
    }
    public List<DrugCabinet> AllDrugCabinets()
    {
        return AllBattlefieldObjects().OfType<DrugCabinet>().ToList();
    }
    public List<Terminal> AllTerminals()
    {
        return AllBattlefieldObjects().OfType<Terminal>().ToList();
    }
    public string RandomShotMissString()
    {
        return $"{RandomShotScatterDistance()}cm {RandomShotScatterHorizontal()}, {RandomShotScatterDistance()}cm {RandomShotScatterVertical()}";
    }
    public string RandomShotScatterDistance()
    {
        return decimal.Round((decimal)UnityEngine.Random.Range(0.5f, 5.0f), 1).ToString();
    }
    public string RandomShotScatterHorizontal()
    {
        if (HelperFunctions.CoinFlip())
            return "left";
        else
            return "right";
    }
    public string RandomShotScatterVertical()
    {
        if (HelperFunctions.CoinFlip())
            return "up";
        else
            return "down";
    }
    public IAmShootable FindShootableById(string id)
    {
        foreach (IAmShootable shootable in AllShootable())
            if (shootable.Id == id)
                return shootable;
        return null;
    }
    public IHaveInventory FindHasInventoryById(string id)
    {
        foreach (IHaveInventory hasInventory in AllHasInventory())
            if (hasInventory.Id == id)
                return hasInventory;
        return null;
    }
    public IAmDisarmable FindDisarmableById(string id)
    {
        foreach (IAmDisarmable disarmable in AllDisarmable())
            if (disarmable.Id == id)
                return disarmable;
        return null;
    }
    public void SetLosCheckAllEnemies(string causeOfLosCheck)
    {
        foreach (Soldier s in AllFieldedEnemySoldiers())
            s.SetLosCheck(causeOfLosCheck); //loscheck
    }
    public void SetLosCheckAll(string causeOfLosCheck)
    {
        foreach (Soldier s in AllFieldedSoldiers())
            s.SetLosCheck(causeOfLosCheck); //loscheck
    }








    //turn resolutions
    public void GameOver(string result)
    {
        gameOver = true;
        print("GameOver");

        if (MenuManager.Instance.OverrideView)
            MenuManager.Instance.UnsetOverrideView();

        MenuManager.Instance.FreezeTimer();
        MenuManager.Instance.DisplaySoldiersGameOver();
        MenuManager.Instance.roundIndicator.text = "Game Over";
        MenuManager.Instance.teamTurnIndicator.text = result;

        //play game over music
        if (DataPersistenceManager.Instance.lozMode)
        {
            if (result.Contains("Team 1"))
                SoundManager.Instance.PlayZombiesWiped();
            else
                SoundManager.Instance.PlayOperatorsWiped();
        }
        else
            SoundManager.Instance.PlayGameOverMusic();
    }
    public void SwitchTeam(int team)
    {
        tempTeam = currentTeam;
        currentTeam = team;
    }
    public void StartModaTurn(Soldier modaSoldier, Soldier killedBy, List<string> damageSource)
    {
        MenuManager.Instance.FreezeTimer();

        //set temp parameters
        tempSoldier = killedBy;
        tempDamageSource = damageSource;

        //activate modafinil turn
        modaTurn = true;
        SwitchTeam(modaSoldier.soldierTeam);
        modaSoldier.soldierUI.GetComponent<SoldierUI>().OpenSoldierMenu("moda");
        modaSoldier.GenerateAP();
        modaSoldier.modaProtect = false;
    }
    public void EndModaTurn()
    {
        MenuManager.Instance.UnfreezeTimer();

        //update temp parameters
        tempDamageSource.Add("Modafinil");

        //deactivate modafinil turn
        ActiveSoldier.Instance.S.Kill(tempSoldier, tempDamageSource);
        modaTurn = false;
        SwitchTeam(tempTeam);
    }
    public void StartFrozenTurn(Soldier frozenSoldier)
    {
        MenuManager.Instance.SetOverrideView();

        //change game parameters
        frozenTurn = true;
        SwitchTeam(frozenSoldier.soldierTeam);

        //activate the surviving soldier
        frozenSoldier.soldierUI.GetComponent<SoldierUI>().OpenSoldierMenu("frozen");
        shotUI.OpenShotUI();
    }
    public void EndFrozenTurn()
    {
        MenuManager.Instance.UnsetOverrideView();

        frozenTurn = false;
        SwitchTeam(tempTeam);
    }
    public void EndTurnNonCoroutine()
    {
        StartCoroutine(EndTurn());
    }
    public IEnumerator EndTurn()
    {
        if (!gameOver)
        {
            if (currentRound <= maxRounds)
            {
                MenuManager.Instance.SetTeamTurnOverFlagTo(false);

                StartCoroutine(EndTeamTurn());

                yield return new WaitUntil(() => MenuManager.Instance.teamTurnOverFlag == true);

                //set los flags only if weather changes
                if (WeatherManager.Instance.CheckVisChange(out string increaseOrDecrease))
                    SetLosCheckAll($"statChange(SR)|weatherChange({increaseOrDecrease})"); //loscheckall

                currentTurn++;
                if (currentTeam < maxTeams)
                    currentTeam++;
                else
                {
                    if (currentRound == maxRounds)
                        GameOver("Resolve Victory Conditions");
                    else
                    {
                        currentTeam = 1;
                        EndRound();
                        currentRound++;
                    }
                }

                //run los lost for intra-turn changes
                if (MenuManager.Instance.lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content").childCount > 0)
                    StartCoroutine(MenuManager.Instance.OpenLostLOSList());

                MenuManager.Instance.SetTeamTurnStartFlagTo(false);

                yield return new WaitUntil(() => MenuManager.Instance.teamTurnStartFlag == true);

                MenuManager.Instance.turnTime = 0;
                SoundManager.Instance.banzaiPlayed = false;
                StartTurn();
            }
        }
    }
    public IEnumerator EndTeamTurn()
    {
        //pre xp check stuff
        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.IsOnturnAndAlive()) //run things that trigger at the end of friendly team turn
            {
                //reset usedap flag
                s.usedAP = false;
                s.usedMP = false;

                //reset bloodletter used on turn flag
                s.bloodLettedThisTurn = false;

                //reset inspired
                s.UnsetInspired();

                //remove donated planner movement
                s.plannerDonatedMove = 0;

                //remove dissuaded
                s.UnsetDissuaded();

                //reset patriot
                s.UnsetPatriotic();

                //increase rounds fielded
                s.roundsFielded++;
                if (s.IsConscious())
                {
                    //only give xp for every 4 rounds conscious on the field
                    s.roundsFieldedConscious++;
                    if (s.roundsFieldedConscious % 4 == 0)
                        MenuManager.Instance.AddXpAlert(s, 1, $"Survived {s.roundsFieldedConscious} rounds.", true);
                }

                //increase rounds without food
                s.IncreaseRoundsWithoutFood();

                //unset suppression
                s.UnsetSuppression();

                //unset politicianUsed
                s.politicianUsed = false;

                //toggle catafalque
                if (!s.lastZombieKilled.Equals(string.Empty))
                {
                    if (s.catafalqueReady) //turn off catafalque
                    {
                        s.lastZombieKilled = string.Empty;
                        s.catafalqueReady = false;
                    }
                    else
                    {
                        if (SoldierManager.Instance.FindSoldierById(s.lastZombieKilled).IsNamedZombie())
                            s.catafalqueReady = true;
                    }
                }
            }
            else if (s.IsOffturnAndAlive()) //run things that trigger at the end of enemy team turn
            {
                //unset overwatch
                if (s.IsOnOverwatch())
                    s.UnsetOverwatch();
            }
            //run things that trigger at the end of any team turn

            //decrement turns of avenger fight bonus
            if (s.IsAvenger())
            {
                if (s.turnsAvenging > 0)
                {
                    s.turnsAvenging--;
                    if (s.turnsAvenging == 0)
                        s.UnsetAvenging();
                }
            }

            //decrement stunnage
            if (s.stunnedTurnsVulnerable > 0)
            {
                s.stunnedTurnsVulnerable--;
                if (s.stunnedTurnsVulnerable == 0)
                    s.UnsetStunned();
            }

            //decrement loud action counter
            if (s.loudActionTurnsVulnerable > 0)
            {
                s.loudActionTurnsVulnerable--;
                if (s.loudActionTurnsVulnerable == 0)
                    s.UnsetLoudRevealed();
            }

            //decrement loud action worst case counter
            if (s.lastLoudActionCounter > 0)
            {
                s.lastLoudActionCounter--;
                if (s.lastLoudActionCounter == 0)
                    s.lastLoudRadius = 0;
            }
                
        }

        MenuManager.Instance.CheckXP();

        yield return new WaitUntil(() => MenuManager.Instance.xpResolvedFlag);

        //post xp stuff
        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.IsOnturnAndAlive()) //run things that trigger at the end of friendly team turn
            {
                //dish out poison damage
                if (s.IsPoisoned())
                    s.TakePoisonDamage();
            }
            else if (s.IsOffturnAndAlive()) //run things that trigger at the end of enemy team turn
            {

            }
            //run things that trigger at the end of any team turn

            //decrement bleedout timer
            if (s.IsUnconscious())
            {
                s.bleedoutTurns--;
                if (s.bleedoutTurns <= 0)
                    s.BleedoutKill();
            }

            //increment recon binoculars
            if (s.IsUsingBinocularsInReconMode())
            {
                if (POIManager.Instance.FindPOIById(s.binocularBeamId.Split("|")[0]) is BinocularBeam binocBeam)
                {
                        binocBeam.turnsActive++;
                        if (binocBeam.turnsActive % 2 == 0)
                            s.SetLosCheck("statChange(P)|binocIncrease|Recon"); //loscheck
                }
            }
        }
    }
    public void EndRound()
    {
        //run things that only happen at the end of a round
    }
    public void StartTurn()
    {
        if (currentRound <= maxRounds)
        {
            FileUtility.WriteToReport($"\nRound {currentRound} | Team: {currentTeam}"); //write to report
            if (currentTeam == 1)
                StartRound();

            IncreaseTurnsActiveAllClouds();
            DecreaseTurnsSpyingJammingAllULFs();
            StartCoroutine(StartTeamTurn());
        }
    }
    public IEnumerator StartTeamTurn()
    {
        if (MenuManager.Instance.damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content").childCount > 0)
            StartCoroutine(MenuManager.Instance.OpenDamageList());

        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.IsOnturnAndAlive()) //run things that trigger at the start of actvive team turn
            {
                if (DataPersistenceManager.Instance.lozMode)
                {
                    if (s.IsZombie() && currentRound % roundsBetweenLeaps == 0)
                    {
                        SoundManager.Instance.PlayLeapEvent();
                        s.LeapIncrementStats(currentRound / roundsBetweenLeaps);
                    }
                }

                //patriot ability
                s.SetPatriotic();

                //inspirer ability
                if (s.IsInspirer())
                    CheckInspirer(s);

                //dissuader ability
                if (s.IsDissuader()) //set initial dissuasions
                {
                    foreach (string revealedSoldierId in s.LOSToTheseSoldiersAndRevealing)
                    {
                        Soldier revealedSoldier = SoldierManager.Instance.FindSoldierById(revealedSoldierId);
                        if (!revealedSoldier.IsRevoker())
                            revealedSoldier.SetDissuaded();
                    }
                }

                yield return new WaitUntil(() => MenuManager.Instance.inspirerResolvedFlag);
            }
            else //run things that trigger at the start of opposition team turn
            {
                
            }
        }

        MenuManager.Instance.CheckXP();

        yield return new WaitUntil(() => MenuManager.Instance.xpResolvedFlag);

        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.IsOnturnAndAlive())
            {
                s.GenerateAP();
            }
        }
    }
    public void StartRound()
    {

    }





    //draining soldier AP and MA (MP below) after use during turn
    public void EndSoldierTurn()
    {
        ActiveSoldier.Instance.S.DrainAP();
        ActiveSoldier.Instance.S.DrainMP();
    }

    //playdead functions
    public void CheckPlaydead()
    {
        if (ActiveSoldier.Instance.S.IsPlayingDead())
            ActiveSoldier.Instance.S.UnsetPlaydead();
        else
            MenuManager.Instance.OpenPlaydeadAlertUI();
    }
    public void ConfirmPlaydead()
    {
        FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} plays dead"); //write to report

        ActiveSoldier.Instance.S.SetPlaydead();
        MenuManager.Instance.ClosePlaydeadAlertUI();
    }

    //overwatch functions
    public void ConfirmOverwatch()
    {
        if (HelperFunctions.ValidateIntInput(overwatchUI.xPos, out int x) && HelperFunctions.ValidateIntInput(overwatchUI.yPos, out int y) && HelperFunctions.ValidateIntInput(overwatchUI.radius, out int r) && HelperFunctions.ValidateIntInput(overwatchUI.arc, out int a))
        {
            if (ActiveSoldier.Instance.S.CheckAP(2))
            {
                //play overwatch confirm dialogue
                SoundManager.Instance.PlaySoldierEnterOverwatch(ActiveSoldier.Instance.S);

                ActiveSoldier.Instance.S.DrainAP();
                ActiveSoldier.Instance.S.SetOverwatch(x, y, r, a);
            }
            MenuManager.Instance.CloseOverwatchUI();
        }
    }


    //move functions
    public void FleeSoldier()
    {
        ActiveSoldier.Instance.S.InstantKill(ActiveSoldier.Instance.S, new List<string>() { "Flee" });
    }
    public int CalculateFallDamage(Soldier soldier, int fallDistance)
    {
        //no fall damage for zombies
        if (DataPersistenceManager.Instance.lozMode && soldier.IsZombie())
            return 0;
        else
        {
            int damage = Mathf.CeilToInt(Mathf.Pow(fallDistance / 4.0f, 2) / 2.0f - soldier.stats.R.Val);
            //play fall damage sfx
            if (damage > 0)
                SoundManager.Instance.PlayFallFromHeight();

            return Mathf.CeilToInt(Mathf.Pow(fallDistance / 4.0f, 2) / 2.0f - soldier.stats.R.Val);
        }
    }
    public void UpdateMoveAP()
    {
        int ap = 0;
        string move = moveUI.moveTypeDropdown.captionText.text;

        if (move.Contains("Full"))
            ap = 3;
        else if (move.Contains("Half"))
            ap = 2;
        else if (move.Contains("Tile") || move.Contains("Exo"))
            ap = 1;

        if (ap > 0)
        {
            if (ap > 1 && ActiveSoldier.Instance.S.InstantSpeed <= 6)
                ap--;

            if (ap > 1 && ActiveSoldier.Instance.S.IsSprinter())
                ap--;

            if (moveUI.coverToggle.isOn)
                ap++;
        }

        moveUI.apCost.text = ap.ToString();
    }
    public void UpdateMoveDonated()
    {
        if (moveUI.moveTypeDropdown.captionText.text.Contains("Planner"))
            moveUI.moveDonated.text = ActiveSoldier.Instance.S.HalfMove.ToString();
    }
    public void UpdateMoveUI()
    {
        if (!MenuManager.Instance.clearMoveFlag)
        {
            UpdateMoveAP();
            UpdateMoveDonated();
        }
    }
    public bool GetMoveLocation(out Tuple<Vector3, string> moveLocation)
    {
        moveLocation = default;
        if (HelperFunctions.ValidateIntInput(moveUI.xPos, out int x) && HelperFunctions.ValidateIntInput(moveUI.yPos, out int y) && HelperFunctions.ValidateIntInput(moveUI.zPos, out int z) && moveUI.terrainDropdown.value != 0)
        {
            moveLocation = Tuple.Create(new Vector3(x, y, z), moveUI.terrainDropdown.captionText.text);
            return true;
        }

        return false;
    }
    public int CalculateMoveDistance(Vector3 moveToLocation)
    {
        return Mathf.RoundToInt(Vector3.Distance(new Vector3(ActiveSoldier.Instance.S.X, ActiveSoldier.Instance.S.Y, ActiveSoldier.Instance.S.Z), moveToLocation));
    }
    public void ConfirmMove(bool force)
    {
        int.TryParse(moveUI.apCost.text, out int ap);

        if (moveUI.moveTypeDropdown.captionText.text.Contains("Planner"))
        {
            if (ActiveSoldier.Instance.S.CheckMP(1) && ActiveSoldier.Instance.S.CheckAP(ap))
            {
                //planner donation proceeds
                ActiveSoldier.Instance.S.DeductAP(ap);
                ActiveSoldier.Instance.S.DrainMP();
                foreach (Transform child in moveUI.closestAllyUI.transform.Find("ClosestAllyPanel"))
                    SoldierManager.Instance.FindSoldierByName(child.Find("SoldierName").GetComponent<TextMeshProUGUI>().text).plannerDonatedMove += ActiveSoldier.Instance.S.HalfMove;
            }
            MenuManager.Instance.CloseMoveUI();
        }
        else if (moveUI.moveTypeDropdown.captionText.text.Contains("Exo"))
        {
            if (GetMoveLocation(out Tuple<Vector3, string> moveToLocation))
            {
                if (ActiveSoldier.Instance.S.X != moveToLocation.Item1.x || ActiveSoldier.Instance.S.Y != moveToLocation.Item1.y || ActiveSoldier.Instance.S.Z != moveToLocation.Item1.z)
                {
                    if (force || (moveToLocation.Item1.x <= ActiveSoldier.Instance.S.X + 3 && moveToLocation.Item1.x >= ActiveSoldier.Instance.S.X - 3 && moveToLocation.Item1.y <= ActiveSoldier.Instance.S.Y + 3 && moveToLocation.Item1.y >= ActiveSoldier.Instance.S.Y - 3))
                    {
                        if (ActiveSoldier.Instance.S.CheckAP(ap))
                        {
                            if (DataPersistenceManager.Instance.lozMode && ActiveSoldier.Instance.S.IsZombie())
                                SoundManager.Instance.PlayZombieMove(ActiveSoldier.Instance.S);
                            else 
                            {
                                //play move dialogue
                                if (moveUI.meleeToggle.isOn)
                                    SoundManager.Instance.PlaySoldierMeleeMove(ActiveSoldier.Instance.S); //play melee move dialogue
                                else
                                    SoundManager.Instance.PlaySoldierConfirmMove(ActiveSoldier.Instance.S); //play standard move dialogue
                            }
                            

                            PerformMove(ActiveSoldier.Instance.S, ap, moveToLocation, moveUI.meleeToggle.isOn, moveUI.coverToggle.isOn, moveUI.fallInput.text, true);

                            //trigger loud action
                            ActiveSoldier.Instance.S.PerformLoudAction(10);
                        }
                        MenuManager.Instance.CloseMoveUI();
                    }
                    else
                        MenuManager.Instance.OpenOvermoveUI("Warning: Landing is further than 3cm away from jump point.");
                }
                else
                    MenuManager.Instance.generalAlertUI.Activate("Cannot move to same location, use OVERRIDE if terrain change is required");
            }
        }
        else
        {
            //get maxmove
            float maxMove;
            if (moveUI.moveTypeDropdown.captionText.text.Contains("Full"))
                maxMove = ActiveSoldier.Instance.S.FullMove;
            else if (moveUI.moveTypeDropdown.captionText.text.Contains("Half"))
                maxMove = ActiveSoldier.Instance.S.HalfMove;
            else
                maxMove = ActiveSoldier.Instance.S.TileMove;

            if (GetMoveLocation(out Tuple<Vector3, string> moveToLocation))
            {
                if (ActiveSoldier.Instance.S.X != moveToLocation.Item1.x || ActiveSoldier.Instance.S.Y != moveToLocation.Item1.y || ActiveSoldier.Instance.S.Z != moveToLocation.Item1.z)
                {
                    //skip supression check if it's already happened before, otherwise run it
                    if (!ActiveSoldier.Instance.S.IsSuppressed() || (ActiveSoldier.Instance.S.IsSuppressed() && (moveUI.moveTypeDropdown.interactable == false || ActiveSoldier.Instance.S.SuppressionCheck())))
                    {
                        int distance = CalculateMoveDistance(moveToLocation.Item1);
                        if (force || distance <= maxMove)
                        {
                            if (ActiveSoldier.Instance.S.CheckMP(1) && ActiveSoldier.Instance.S.CheckAP(ap)) 
                            {
                                //play move dialogue
                                if (moveUI.meleeToggle.isOn)
                                    SoundManager.Instance.PlaySoldierMeleeMove(ActiveSoldier.Instance.S); //play melee move dialogue
                                else
                                {
                                    if (!moveUI.moveTypeDropdown.captionText.text.Contains("Tile"))
                                        SoundManager.Instance.PlaySoldierConfirmMove(ActiveSoldier.Instance.S); //play standard move dialogue
                                }

                                PerformMove(ActiveSoldier.Instance.S, ap, moveToLocation, moveUI.meleeToggle.isOn, moveUI.coverToggle.isOn, moveUI.fallInput.text, false);
                            }
                            MenuManager.Instance.CloseMoveUI();
                        }
                        else
                            MenuManager.Instance.OpenOvermoveUI($"Warning: Proposed move is {distance - maxMove} cm over max ({distance}/{maxMove})");
                    }
                    else
                        MenuManager.Instance.OpenSuppressionMoveUI();
                }
                else
                    MenuManager.Instance.generalAlertUI.Activate("Cannot move to same location, use OVERRIDE if terrain change is required");
            }
            else
                print("Invalid Input");
        }
    }
    public void PerformSpawn(Soldier movingSoldier, Tuple<Vector3, string> moveToLocation)
    {
        movingSoldier.moveResolvedFlag = false;
        FileUtility.WriteToReport($"{movingSoldier.soldierName} spawned at ({(int)moveToLocation.Item1.x}, {(int)moveToLocation.Item1.y}, {(int)moveToLocation.Item1.z}) {moveToLocation.Item2}"); //write to report

        //save start position
        movingSoldier.startX = (int)moveToLocation.Item1.x;
        movingSoldier.startY = (int)moveToLocation.Item1.y;
        movingSoldier.startZ = (int)moveToLocation.Item1.z;

        //perform the move
        movingSoldier.transform.position = HelperFunctions.ConvertMathPosToPhysicalPos(moveToLocation.Item1);
        movingSoldier.x = (int)moveToLocation.Item1.x;
        movingSoldier.y = (int)moveToLocation.Item1.y;
        movingSoldier.z = (int)moveToLocation.Item1.z;
        movingSoldier.TerrainOn = moveToLocation.Item2;

        //check deployment beacons
        CheckDeploymentBeacons(movingSoldier);

        //patriot ability
        movingSoldier.SetPatriotic();

        //generate starting ap
        movingSoldier.GenerateAP();

        //check for smoke clouds
        if (!movingSoldier.CheckSmokeClouds() && movingSoldier.IsInSmoke())
            movingSoldier.UnsetSmoked();

        //check for tabun clouds
        if (!movingSoldier.CheckTabunClouds() && movingSoldier.IsInTabun())
            movingSoldier.UnsetTabun();

        movingSoldier.SetLosCheck("losChange|move|spawn"); //losCheck
    }
    public void PerformMove(Soldier movingSoldier, int ap, Tuple<Vector3, string> moveToLocation, bool meleeToggle, bool coverToggle, string fallDistance, bool freeMove)
    {
        movingSoldier.moveResolvedFlag = false;
        FileUtility.WriteToReport($"{movingSoldier.soldierName} moved to ({(int)moveToLocation.Item1.x}, {(int)moveToLocation.Item1.y}, {(int)moveToLocation.Item1.z}) {moveToLocation.Item2}"); //write to report

        int.TryParse(fallDistance, out int fallDistanceInt);
        string launchMelee = string.Empty;
        movingSoldier.DeductAP(ap);
        if (!freeMove)
            movingSoldier.DeductMP(1);

        //fill the tempMove variable in case move needs to be reverted
        Vector3 oldPos = new(movingSoldier.X, movingSoldier.Y, movingSoldier.Z);
        tempMove = Tuple.Create(oldPos, movingSoldier.TerrainOn, ap, 1);

        //perform the move
        movingSoldier.x = (int)moveToLocation.Item1.x;
        movingSoldier.y = (int)moveToLocation.Item1.y;
        movingSoldier.z = (int)moveToLocation.Item1.z;
        movingSoldier.TerrainOn = moveToLocation.Item2;

        //activate in cover
        if (coverToggle)
            movingSoldier.SetCover();
        else
            movingSoldier.UnsetCover();

        //break melee control
        meleeUI.BreakAllControllingMeleeEngagments(movingSoldier);

        //unset overwatch
        movingSoldier.UnsetOverwatch();

        //break suppression
        movingSoldier.UnsetSuppression();

        //check for smoke clouds
        if (!movingSoldier.CheckSmokeClouds() && movingSoldier.IsInSmoke())
            movingSoldier.UnsetSmoked();

        //check for tabun clouds
        if (!movingSoldier.CheckTabunClouds() && movingSoldier.IsInTabun())
            movingSoldier.UnsetTabun();

        //check for fall damage
        if (fallDistanceInt > 0)
            movingSoldier.TakeDamage(movingSoldier, CalculateFallDamage(movingSoldier, fallDistanceInt), false, new() { "Fall" }, Vector3.zero);

        //launch melee if melee toggle is on
        if (meleeToggle)
        {
            if (moveUI.moveTypeDropdown.value == 0)
                launchMelee = "Full Charge Attack";
            else if (moveUI.moveTypeDropdown.value == 1)
                launchMelee = "Half Charge Attack";
            else if (moveUI.moveTypeDropdown.value == 2)
                launchMelee = "3cm Charge Attack";
            StartCoroutine(meleeUI.OpenMeleeUI(launchMelee));
        }

        //check broken soldier leaving field
        CheckBrokenFlee(movingSoldier);

        movingSoldier.SetLosCheck("losChange|move"); //losCheck
    }
    public void CheckBrokenFlee(Soldier movingSoldier)
    {
        if (movingSoldier.IsBroken())
            if (movingSoldier.X == movingSoldier.startX && movingSoldier.Y == movingSoldier.startY && movingSoldier.Z == movingSoldier.startZ)
                MenuManager.Instance.OpenBrokenFledUI();
    }
    public void CheckDeploymentBeacons(Soldier fieldedSoldier)
    {
        foreach (DeploymentBeacon beacon in FindObjectsByType<DeploymentBeacon>(default))
            if (beacon.placedBy.IsSameTeamAs(fieldedSoldier) && fieldedSoldier.X == beacon.X && fieldedSoldier.Y == beacon.Y && fieldedSoldier.Z == beacon.Z)
                MenuManager.Instance.AddXpAlert(beacon.placedBy, 1, $"Ally ({fieldedSoldier.soldierName}) deployed through beacon at ({beacon.X}, {beacon.Y}, {beacon.Z})", true);
    }














    //configure functions
    public int UpdateConfigureAP()
    {
        int totalDrop = 0, totalPickup = 0, totalSwap = 0, ap = 0;

        foreach (ItemIcon itemIcon in MenuManager.Instance.configUI.GetComponentsInChildren<ItemIcon>(true))
        {
            Item item = itemIcon.item;
            if (item.markedForAction != string.Empty)
            {
                string[] instructions = item.markedForAction.Split('|');
                if (instructions[0] == instructions[2] && instructions[1] != instructions[3])
                    totalSwap++;
                else if (instructions[2] == "none")
                {
                    if (!instructions[1].Contains("Hand"))
                        totalDrop++;
                }
                else
                    totalPickup++;

                if (item.equippableSlots.Contains("Head")) //is a special armour
                    ap = 3;
            }
        }
        print($"totaldrop|{totalDrop} totalswap|{totalSwap} totalpickup|{totalPickup}");

        //calculate ap required for action
        if (totalDrop > 2)
            ap += 2;
        else
            ap += totalDrop;

        ap += totalPickup;
        ap += totalSwap;

        if (ap > 3)
            ap = 3;

        //apply adept ap reduction
        if (ActiveSoldier.Instance.S.IsAdept() && ap > 1)
            ap -= 1;

        //configure free on first soldier turn is they haven't used ap
        if (ActiveSoldier.Instance.S.roundsFielded == 0 && !ActiveSoldier.Instance.S.usedAP)
            ap = 0;

        configUI.apCost.text = ap.ToString();
        return ap;
    }
    public List<Item> FindNearbyItems()
    {
        List<Item> nearbyItems = new();
        foreach (Item i in FindObjectsByType<Item>(default))
            if (ActiveSoldier.Instance.S.PhysicalObjectWithinItemRadius(i))
                nearbyItems.Add(i);

        return nearbyItems;
    }
    public void ConfirmConfigure()
    {
        int ap = UpdateConfigureAP();
        if (ActiveSoldier.Instance.S.CheckAP(ap))
        {
            FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} configures."); //write to report

            ActiveSoldier.Instance.S.DeductAP(ap);
            foreach (ItemIcon itemIcon in configUI.GetComponentsInChildren<ItemIcon>(true))
            {
                Item item = itemIcon.item;
                if (item.markedForAction != string.Empty)
                {
                    string[] instructions = item.markedForAction.Split('|');
                    print(item.markedForAction);
                    item.MoveItem(FindHasInventoryById(instructions[0]), instructions[1], FindHasInventoryById(instructions[2]), instructions[3]);
                }
            }

            if (MenuManager.Instance.configureButton.GetComponentInChildren<TextMeshProUGUI>().text.Equals("Spawn Config")) { } //if spawn config, make it silent
            else
                ActiveSoldier.Instance.S.PerformLoudAction(5);

            MenuManager.Instance.CloseConfigureUI();
        }
    }













    















    //item functions
    public void ConfirmUseItem(UseItemUI useItemUI)
    {
        int.TryParse(useItemUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap);
        Item itemUsed = useItemUI.itemUsed;
        Item itemUsedOn = useItemUI.itemUsedOn;
        Soldier soldierUsedOn = useItemUI.soldierUsedOn;
        ItemIcon linkedIcon = useItemUI.itemUsedIcon;

        FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} uses {itemUsed.itemName}."); //write to report

        switch (itemUsed.itemName)
        {
            case "E_Tool":
            case "Food_Pack":
            case "Water_Canteen":
                itemUsed.UseItem(linkedIcon, null, null);
                break;
            case "Ammo_AR":
            case "Ammo_LMG":
            case "Ammo_Ri":
            case "Ammo_Sh":
            case "Ammo_SMG_Pi":
            case "Ammo_Sn":
            case "Poison_Satchel":
                itemUsed.UseItem(linkedIcon, itemUsedOn, null);
                break;
            case "Medikit_Small":
            case "Medikit_Medium":
            case "Medikit_Large":
            case "Syringe_Amphetamine":
            case "Syringe_Androstenedione":
            case "Syringe_Cannabinoid":
            case "Syringe_Danazol":
            case "Syringe_Glucocorticoid":
            case "Syringe_Modafinil":
            case "Syringe_Shard":
            case "Syringe_Trenbolone":
            case "Syringe_Unlabelled":
                itemUsed.UseItem(linkedIcon, null, soldierUsedOn);
                break;
            case "Grenade_Flashbang":
            case "Grenade_Frag":
            case "Grenade_Smoke":
            case "Grenade_Tabun":
                MenuManager.Instance.OpenGrenadeUI(useItemUI);
                break;
            case "Binoculars":
                MenuManager.Instance.OpenBinocularsUI(itemUsed, linkedIcon, "Flash");
                break;
            case "Claymore":
                MenuManager.Instance.OpenClaymoreUI(useItemUI);
                break;
            case "Deployment_Beacon":
                MenuManager.Instance.OpenDeploymentBeaconUI(useItemUI);
                break;
            case "Thermal_Camera":
                MenuManager.Instance.OpenThermalCamUI(useItemUI);
                break;
            case "UHF_Radio":
                MenuManager.Instance.OpenUHFUI(useItemUI);
                break;
            default:
                break;

        }
        ActiveSoldier.Instance.S.DeductAP(ap);
        MenuManager.Instance.CloseUseItemUI();
    }
    public void ConfirmDropThrowItem(UseItemUI useItemUI)
    {
        int.TryParse(useItemUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap);
        if (useItemUI.transform.Find("OptionPanel").Find("Message").Find("Text").GetComponent<TextMeshProUGUI>().text.Contains("Throw"))
            MenuManager.Instance.throwUI.OpenThrowUI(useItemUI);
        else
            MenuManager.Instance.dropUI.OpenDropUI(useItemUI);

        ActiveSoldier.Instance.S.DeductAP(ap);
        MenuManager.Instance.CloseDropThrowItemUI();
    }
    public void UpdateSoldierUsedOn(UseItemUI useItemUI)
    {
        useItemUI.soldierUsedOn = SoldierManager.Instance.FindSoldierById(MenuManager.Instance.useItemUI.transform.Find("OptionPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().captionText.text);
    }
    public void UpdateItemUsedOn(UseItemUI useItemUI)
    {
        useItemUI.itemUsedOn = ItemManager.Instance.FindItemById(MenuManager.Instance.useItemUI.transform.Find("OptionPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().captionText.text);
    }
    public void ConfirmUHF(UseItemUI useUHFUI)
    {
        TMP_InputField targetX = useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField targetY = useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("YPos").GetComponent<TMP_InputField>();
        TMP_InputField targetZ = useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("ZPos").GetComponent<TMP_InputField>();
        GameObject totalMiss = useUHFUI.transform.Find("OptionPanel").Find("TotalMiss").gameObject;
        TMP_Dropdown strikeOption = useUHFUI.transform.Find("OptionPanel").Find("StrikeOptions").Find("StrikeOptionsDropdown").GetComponent<TMP_Dropdown>();

        // get dipelec score
        int functionalDip = ActiveSoldier.Instance.S.stats.Dip.Val;
        int functionalElec = ActiveSoldier.Instance.S.stats.Elec.Val;
        if (functionalDip > 9)
            functionalDip = 9;
        if (functionalElec > 9)
            functionalElec = 9;
        int dipelecScore = ItemManager.Instance.scoreTable[functionalDip, functionalElec];

        // get dipelec strike parameters
        Tuple<int, string, int, int, int> strike = ItemManager.Instance.GetStrike(strikeOption.captionText.text);
        int rolls = strike.Item4;
        int radius = strike.Item3;
        int damage = strike.Item5;

        if (!useUHFUI.transform.Find("PressedOnce").gameObject.activeInHierarchy) //first press
        {

            if (HelperFunctions.ValidateIntInput(targetX, out int x) && HelperFunctions.ValidateIntInput(targetY, out int y))
            {
                int highestRoll = 0, newX, newY;
                float scatterDistance;
                int scatterDegree = HelperFunctions.RandomNumber(0, 360);
                for (int i = 0; i < rolls; i++)
                {
                    int roll = HelperFunctions.DiceRoll();
                    if (roll > highestRoll)
                        highestRoll = roll;
                }

                //play uhf result dialogue
                SoundManager.Instance.PlayUHFResult(highestRoll);

                //calculate scatter
                scatterDistance = highestRoll switch
                {
                    2 => radius + 1,
                    3 => 0.75f * radius + 1,
                    4 => 0.5f * radius + 1,
                    5 => 0.25f * radius + 1,
                    6 => 0,
                    _ => -1,
                };

                if (scatterDistance != -1)
                {
                    (newX, newY) = HelperFunctions.CalculateScatteredCoordinates(x, y, scatterDegree, scatterDistance);

                    if (newX > 0 && newX <= maxX && newY > 0 && newY <= maxY)
                    {
                        strikeOption.interactable = false;
                        targetX.text = $"{newX}";
                        targetX.interactable = false;
                        targetY.text = $"{newY}";
                        targetY.interactable = false;
                        useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("ZLabel").gameObject.SetActive(true);
                        useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("ZPos").gameObject.SetActive(true);
                    }
                    else
                    {
                        useUHFUI.transform.Find("OptionPanel").Find("TotalMiss").Find("Text").GetComponent<TextMeshProUGUI>().text = "Scattered off map";
                        useUHFUI.transform.Find("OptionPanel").Find("TotalMiss").gameObject.SetActive(true);
                    }
                }
                else
                {
                    useUHFUI.transform.Find("OptionPanel").Find("TotalMiss").Find("Text").GetComponent<TextMeshProUGUI>().text = "Total Misfire";
                    useUHFUI.transform.Find("OptionPanel").Find("TotalMiss").gameObject.SetActive(true);
                }
                useUHFUI.transform.Find("PressedOnce").gameObject.SetActive(true);
            }
        }
        else //second press
        {
            if (totalMiss.activeInHierarchy)
            {
                MenuManager.Instance.CloseUHFUI();
                useUHFUI.itemUsed.UseItem(useUHFUI.itemUsedIcon, useUHFUI.itemUsedOn, useUHFUI.soldierUsedOn);
            }
            else
            {
                if (HelperFunctions.ValidateIntInput(targetX, out int x) && HelperFunctions.ValidateIntInput(targetY, out int y) && HelperFunctions.ValidateIntInput(targetZ, out int z))
                {
                    MenuManager.Instance.CloseUHFUI();
                    useUHFUI.itemUsed.UseItem(useUHFUI.itemUsedIcon, useUHFUI.itemUsedOn, useUHFUI.soldierUsedOn);
                    CheckExplosionUHF(ActiveSoldier.Instance.S, new Vector3(x, y, z), radius, damage);
                }
            }

            //perform loud action
            ActiveSoldier.Instance.S.PerformLoudAction(14);

            //set sound flags after enemy use UHF
            foreach (Soldier s in AllSoldiers())
            {
                if (s.IsSameTeamAs(ActiveSoldier.Instance.S))
                    SoundManager.Instance.SetSoldierSelectionSoundFlagAfterAllyUseUHF(s);
                else
                    SoundManager.Instance.SetSoldierSelectionSoundFlagAfterEnemyUseUHF(s);
            }
        }
    }
    public void ConfirmULF(UseItemUI useULFUI)
    {
        if (ActiveSoldier.Instance.S.CheckAP(3))
        {
            ActiveSoldier.Instance.S.DeductAP(3);
            useULFUI.itemUsed.UseULF(useULFUI.itemUsedFromSlotName);
            MenuManager.Instance.CloseUseULFUI();
        }
    }
    public void ConfirmRiotShield()
    {
        if (HelperFunctions.ValidateIntInput(MenuManager.Instance.riotShieldUI.xPos, out int x) && HelperFunctions.ValidateIntInput(MenuManager.Instance.riotShieldUI.yPos, out int y))
        {
            FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} orients riot shield ({x}, {y})."); //write to report

            //set riot shield facing
            ActiveSoldier.Instance.S.riotXPoint = x;
            ActiveSoldier.Instance.S.riotYPoint = y;

            MenuManager.Instance.CloseRiotShieldUI();
        }
    }
    public void ConfirmGrenade(UseItemUI useGrenade)
    {
        string grenadeName = useGrenade.transform.Find("OptionPanel").Find("GrenadeType").Find("Text").GetComponent<TextMeshProUGUI>().text;
        ValidGrenadeThrowChecker throwTarget = useGrenade.transform.Find("OptionPanel").Find("Target").GetComponent<ValidGrenadeThrowChecker>();
        GameObject throwBeyondRadius = useGrenade.transform.Find("OptionPanel").Find("Target").Find("ThrowBeyondRadius").gameObject;
        GameObject throwBeyondBlindRadius = useGrenade.transform.Find("OptionPanel").Find("Target").Find("ThrowBeyondBlindRadius").gameObject;
        GameObject scatteredOffMap = useGrenade.transform.Find("OptionPanel").Find("ScatteredOffMap").gameObject;

        if (!throwTarget.pressedOnce.activeInHierarchy) //first press
        {
            if (HelperFunctions.ValidateIntInput(throwTarget.XPos, out int x) && HelperFunctions.ValidateIntInput(throwTarget.YPos, out int y) && HelperFunctions.ValidateIntInput(throwTarget.ZPos, out int z) && !throwBeyondRadius.activeInHierarchy && !throwBeyondBlindRadius.activeInHierarchy)
            {
                int newX, newY;
                throwTarget.GetThrowLocation(out Vector3 throwLocation);
                int throwDistance = Mathf.RoundToInt(Vector3.Distance(new(ActiveSoldier.Instance.S.X, ActiveSoldier.Instance.S.Y, ActiveSoldier.Instance.S.Z), throwLocation));
                int scatterDegree = HelperFunctions.RandomNumber(0, 360);
                int scatterDistance = ActiveSoldier.Instance.S.StrengthCheck() switch
                {
                    false => Mathf.CeilToInt(HelperFunctions.DiceRoll() * ActiveSoldier.Instance.S.stats.Str.Val / 2.0f),
                    _ => -1,
                };

                if (scatterDistance == -1 || throwDistance <= 3)
                    useGrenade.transform.Find("OptionPanel").Find("Target").Find("PreciseThrow").gameObject.SetActive(true);
                else
                {
                    (newX, newY) = HelperFunctions.CalculateScatteredCoordinates(x, y, scatterDegree, scatterDistance);

                    throwTarget.XPos.text = $"{newX}";
                    throwTarget.YPos.text = $"{newY}";
                    useGrenade.transform.Find("OptionPanel").Find("Target").Find("FinalPosition").gameObject.SetActive(true);
                }

                throwTarget.pressedOnce.SetActive(true);
            }
        }
        else //second press
        {
            if (scatteredOffMap.activeInHierarchy)
            {
                FileUtility.WriteToReport($"{useGrenade.itemUsed.itemName} falls off map."); //write to report
                useGrenade.itemUsed.TakeDamage(ActiveSoldier.Instance.S, 1, new() { "Fall" }); //destroy item
                MenuManager.Instance.CloseGrenadeUI();
            }
            else
            {
                if (HelperFunctions.ValidateIntInput(throwTarget.XPos, out int x) && HelperFunctions.ValidateIntInput(throwTarget.YPos, out int y) && HelperFunctions.ValidateIntInput(throwTarget.ZPos, out int z))
                {
                    useGrenade.itemUsed.UseItem(useGrenade.itemUsedIcon, useGrenade.itemUsedOn, useGrenade.soldierUsedOn);
                    useGrenade.itemUsed.CheckExplosionGrenade(ActiveSoldier.Instance.S, new Vector3(x, y, z));
                    MenuManager.Instance.CloseGrenadeUI();
                }
            }
        }
    }
    public void ConfirmBinoculars()
    {
        if (HelperFunctions.ValidateIntInput(MenuManager.Instance.binocularsUI.xPos, out int x) && HelperFunctions.ValidateIntInput(MenuManager.Instance.binocularsUI.yPos, out int y))
        {
            FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} uses binoculars ({x}, {y})."); //write to report

            MenuManager.Instance.binocularsUI.binocularsUsed.UseItem(MenuManager.Instance.binocularsUI.binocularsUsedIcon, null, null);
            StartCoroutine(ActiveSoldier.Instance.S.SetUsingBinoculars(new(x, y), MenuManager.Instance.binocularsUI.binocularMode));

            MenuManager.Instance.CloseBinocularsUI();
        }
    }
    public void ConfirmClaymore(UseItemUI useClaymore)
    {
        TMP_InputField placedX = useClaymore.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField placedY = useClaymore.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("YPos").GetComponent<TMP_InputField>();
        TMP_InputField placedZ = useClaymore.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("ZPos").GetComponent<TMP_InputField>();
        TMP_InputField facingX = useClaymore.transform.Find("OptionPanel").Find("ClaymoreFacing").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField facingY = useClaymore.transform.Find("OptionPanel").Find("ClaymoreFacing").Find("YPos").GetComponent<TMP_InputField>();


        if (HelperFunctions.ValidateIntInput(placedX, out int x) && HelperFunctions.ValidateIntInput(placedY, out int y) && HelperFunctions.ValidateIntInput(placedZ, out int z) && HelperFunctions.ValidateIntInput(facingX, out int fx) && HelperFunctions.ValidateIntInput(facingY, out int fy))
        {
            if (CalculateRange(ActiveSoldier.Instance.S, new Vector3(x, y, z)) <= ActiveSoldier.Instance.S.SRColliderMin.radius)
            {
                FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} places claymore at ({x}, {y}, {z})."); //write to report

                useClaymore.itemUsed.UseItem(useClaymore.itemUsedIcon, useClaymore.itemUsedOn, useClaymore.soldierUsedOn);
                Instantiate(POIManager.Instance.claymorePrefab).Init(new(x, y, z), Tuple.Create(ActiveSoldier.Instance.S.ActiveC, fx, fy, false, ActiveSoldier.Instance.S.Id));

                ActiveSoldier.Instance.S.PerformLoudAction(10);
                MenuManager.Instance.CloseClaymoreUI();
            }
            else
                useClaymore.transform.Find("OptionPanel").Find("OutOfRange").gameObject.SetActive(true);
        }
    }
    public void ConfirmDeploymentBeacon(UseItemUI useDeploymentBeacon)
    {
        TMP_InputField placedX = useDeploymentBeacon.transform.Find("OptionPanel").Find("BeaconPlacing").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField placedY = useDeploymentBeacon.transform.Find("OptionPanel").Find("BeaconPlacing").Find("YPos").GetComponent<TMP_InputField>();
        TMP_InputField placedZ = useDeploymentBeacon.transform.Find("OptionPanel").Find("BeaconPlacing").Find("ZPos").GetComponent<TMP_InputField>();

        if (HelperFunctions.ValidateIntInput(placedX, out int x) && HelperFunctions.ValidateIntInput(placedY, out int y) && HelperFunctions.ValidateIntInput(placedZ, out int z))
        {
            if (CalculateRange(ActiveSoldier.Instance.S, new Vector3(x, y, z)) <= ActiveSoldier.Instance.S.SRColliderMin.radius)
            {
                FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} places deployment beacon at ({x}, {y}, {z})."); //write to report

                //play use deployment beacon
                SoundManager.Instance.PlayUseDepBeacon();

                useDeploymentBeacon.itemUsed.UseItem(useDeploymentBeacon.itemUsedIcon, useDeploymentBeacon.itemUsedOn, useDeploymentBeacon.soldierUsedOn);
                Instantiate(POIManager.Instance.deploymentBeaconPrefab).Init(new(x, y, z), ActiveSoldier.Instance.S.Id);

                ActiveSoldier.Instance.S.PerformLoudAction(10);
                MenuManager.Instance.CloseDeploymentBeaconUI();
            }
            else
                useDeploymentBeacon.transform.Find("OptionPanel").Find("OutOfRange").gameObject.SetActive(true);
        }
    }
    public void ConfirmThermalCam(UseItemUI useThermalCam)
    {
        TMP_InputField placedX = useThermalCam.transform.Find("OptionPanel").Find("CamPlacing").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField placedY = useThermalCam.transform.Find("OptionPanel").Find("CamPlacing").Find("YPos").GetComponent<TMP_InputField>();
        TMP_InputField placedZ = useThermalCam.transform.Find("OptionPanel").Find("CamPlacing").Find("ZPos").GetComponent<TMP_InputField>();
        TMP_InputField facingX = useThermalCam.transform.Find("OptionPanel").Find("CamFacing").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField facingY = useThermalCam.transform.Find("OptionPanel").Find("CamFacing").Find("YPos").GetComponent<TMP_InputField>();

        if (HelperFunctions.ValidateIntInput(placedX, out int x) && HelperFunctions.ValidateIntInput(placedY, out int y) && HelperFunctions.ValidateIntInput(placedZ, out int z) && HelperFunctions.ValidateIntInput(facingX, out int fx) && HelperFunctions.ValidateIntInput(facingY, out int fy))
        {
            if (CalculateRange(ActiveSoldier.Instance.S, new Vector3(x, y, z)) <= ActiveSoldier.Instance.S.SRColliderMin.radius)
            {
                FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} places thermal cam at ({x}, {y}, {z})."); //write to report

                useThermalCam.itemUsed.UseItem(useThermalCam.itemUsedIcon, useThermalCam.itemUsedOn, useThermalCam.soldierUsedOn);
                
                SetLosCheckAllEnemies("losChange|thermalCamActive"); //loscheckallenemies
                Instantiate(POIManager.Instance.thermalCamPrefab).Init(new(x, y, z), Tuple.Create(fx, fy, ActiveSoldier.Instance.S.Id));

                ActiveSoldier.Instance.S.PerformLoudAction(10);
                MenuManager.Instance.CloseThermalCamUI();
            }
            else
                useThermalCam.transform.Find("OptionPanel").Find("OutOfRange").gameObject.SetActive(true);
        }
    }
    public void CheckExplosionUHF(Soldier explodedBy, Vector3 position, int radius, int damage)
    {
        //play explosion sfx
        SoundManager.Instance.PlayExplosion();

        GameObject explosionList = Instantiate(MenuManager.Instance.explosionListPrefab, MenuManager.Instance.explosionUI.transform).GetComponent<ExplosionList>().Init($"UHF | Detonated: {position.x},{position.y},{position.z} | Radius: {radius}cm | Damage: {damage}", position).gameObject;
        explosionList.transform.Find("ExplodedBy").GetComponent<TextMeshProUGUI>().text = explodedBy.id;

        //create explosion objects
        Explosion explosion1 = Instantiate(POIManager.Instance.explosionPrefab, position, default).Init(radius / 2, position);
        Explosion explosion2 = Instantiate(POIManager.Instance.explosionPrefab, position, default).Init(radius, position);

        foreach (PhysicalObject obj in AllBattlefieldObjects())
        {
            float damagef = 0;
            if (obj.IsWithinSphere(explosion1.BodyCollider))
                damagef = damage;
            else if (obj.IsWithinSphere(explosion2.BodyCollider))
                damagef = damage / 2.0f;
                
            if (damagef > 0)
            {
                if (obj is Item hitItem)
                    MenuManager.Instance.AddExplosionAlertItem(explosionList, hitItem, position, explodedBy, Mathf.RoundToInt(damagef));
                else if (obj is POI hitPoi)
                    MenuManager.Instance.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, Mathf.RoundToInt(damagef));
                else if (obj is Soldier hitSoldier)
                    MenuManager.Instance.AddExplosionAlert(explosionList, hitSoldier, position, explodedBy, hitSoldier.RoundByResilience(damagef) - hitSoldier.stats.R.Val, 1);
            }
        }

        //show explosion ui
        MenuManager.Instance.OpenExplosionUI();
    }
    public void CheckAllSmokeClouds()
    {
        foreach (Soldier s in AllSoldiers())
            if (!s.CheckSmokeClouds() && s.IsInSmoke())
                s.UnsetSmoked();
    }
    public void CheckAllTabunClouds()
    {
        foreach (Soldier s in AllSoldiers())
            if (!s.CheckTabunClouds() && s.IsInTabun())
                s.UnsetTabun();
    }
    public void IncreaseTurnsActiveAllClouds()
    {
        foreach (SmokeCloud cloud in FindObjectsByType<SmokeCloud>(default))
            cloud.TurnsUntilDissipation--;
        foreach (TabunCloud cloud in FindObjectsByType<TabunCloud>(default))
            cloud.TurnsUntilDissipation--;
    }
    public void DecreaseTurnsSpyingJammingAllULFs()
    {
        foreach (Item item in ItemManager.Instance.allItems)
        {
            if (item.IsJamming())
                item.jammingForTurns--;
            if (item.IsSpying())
                item.spyingForTurns--;
        }
    }





    

















    //lastandicide functions
    public void Lastandicide()
    {
        if (ActiveSoldier.Instance.S.CheckAP(1))
        {
            FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} lastandicides"); //write to report

            ActiveSoldier.Instance.S.DeductAP(1);
            ActiveSoldier.Instance.S.InstantKill(ActiveSoldier.Instance.S, new List<string> { "Lastandicide" });
        }
    }

    //trauma functions
    public void TraumaCheck(Soldier deadSoldier, int tp, bool commander, bool lastandicide)
    {
        if (deadSoldier.IsDead())
        {
            foreach (Soldier friendly in AllSoldiers())
            {
                //print(friendly.soldierName + " trauma check attempting to run");

                if (friendly.IsSameTeamAs(deadSoldier) && friendly.IsAlive())
                {
                    //print(friendly.soldierName + " trauma check actually running");
                    //desensitised
                    if (friendly.IsDesensitised())
                    {
                        MenuManager.Instance.AddTraumaAlert(friendly, tp, $"{friendly.soldierName} is {friendly.GetTraumaState()}. He is immune to trauma.", 0, 0, "");
                        FileUtility.WriteToReport($"{friendly.soldierName} is {friendly.GetTraumaState().Replace(",", "").Trim()}, he is immune to trauma ({tp}tp)"); //write to report
                    }
                    else
                    {
                        //guaranteed trauma from commander death and/or lastandicide
                        if (commander)
                            MenuManager.Instance.AddTraumaAlert(friendly, 1, "Commander died, an automatic trauma point has been accrued.", 0, 0, "");
                        if (lastandicide)
                            MenuManager.Instance.AddTraumaAlert(friendly, 1, $"{deadSoldier.soldierName} committed Lastandicide, an automatic trauma point has been accrued.", 0, 0, "");

                        if (friendly.IsAbleToSee())
                        {
                            int rolls = 0, xpOnResist = 0;
                            if (friendly.PhysicalObjectWithinMaxRadius(deadSoldier))
                            {
                                string range = CalculateRangeBracket(CalculateRange(deadSoldier, friendly));
                                switch (range)
                                {
                                    case "Melee":
                                        rolls = 0;
                                        xpOnResist = 5;
                                        break;
                                    case "CQB":
                                        rolls = 0;
                                        xpOnResist = 4;
                                        break;
                                    case "Short":
                                        rolls = 1;
                                        xpOnResist = 3;
                                        break;
                                    case "Medium":
                                        rolls = 2;
                                        xpOnResist = 2;
                                        break;
                                    case "Long":
                                        rolls = 3;
                                        xpOnResist = 1;
                                        break;
                                    default:
                                        rolls = 3;
                                        xpOnResist = 1;
                                        break;
                                }

                                //add rolls for rank differential
                                if (deadSoldier.MinXPForRank() > friendly.MinXPForRank())
                                    rolls += 0;
                                else if (deadSoldier.MinXPForRank() == friendly.MinXPForRank())
                                    rolls += 1;
                                else
                                    rolls += 2;

                                if (friendly.IsResilient())
                                    MenuManager.Instance.AddTraumaAlert(friendly, tp, $"{friendly.soldierName} is Resilient. Within {range} range of {deadSoldier.soldierName}. Check for LOS?", rolls, xpOnResist, range);
                                else
                                    MenuManager.Instance.AddTraumaAlert(friendly, tp, $"{friendly.soldierName} is within {range} range of {deadSoldier.soldierName}. Check for LOS?", rolls, xpOnResist, range);
                            }
                        }
                    }
                }
            }
        }
    }
    public void ConfirmTrauma()
    {
        ScrollRect traumaScroller = MenuManager.Instance.traumaUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        bool unresolved = false;

        if (traumaScroller.verticalNormalizedPosition <= 0.05f)
        {
            Transform traumaAlerts = MenuManager.Instance.traumaUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");

            foreach (Transform child in traumaAlerts)
            {
                if (child.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text.Contains("POTENTIAL"))
                    unresolved = true;
            }

            //block confirm if all buttons haven't been clicked
            if (!unresolved)
            {
                Dictionary<Soldier, int> soldiersToTraumatise = new();
                foreach (Transform child in traumaAlerts)
                {
                    if (child.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text.Contains("GAINED"))
                    {
                        Soldier traumatisedSoldier = child.GetComponent<SoldierAlert>().soldier;
                        int.TryParse(child.Find("TraumaIndicator").GetComponent<TextMeshProUGUI>().text, out int trauma);

                        // Check if the soldier is already in the dictionary
                        if (soldiersToTraumatise.ContainsKey(traumatisedSoldier))
                            soldiersToTraumatise[traumatisedSoldier] += trauma;
                        else
                            soldiersToTraumatise.Add(traumatisedSoldier, trauma);
                    }
                    Destroy(child.gameObject);
                }

                //iterate through the list 
                foreach (KeyValuePair<Soldier, int> kvp in soldiersToTraumatise)
                    kvp.Key.TakeTrauma(kvp.Value);

                MenuManager.Instance.CloseTraumaUI();
            }
            else
                print("Haven't traumatised everyone.");
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }












    //damage event functions - game
    public void ConfirmDamageEvent()
    {
        if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Bloodletting"))
        {
            FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} bloodlets."); //write to report

            ActiveSoldier.Instance.S.TakeBloodlettingDamage();
            MenuManager.Instance.CloseDamageEventUI();
        }
        else if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Other") && int.TryParse(damageEventUI.otherInput.text, out int otherDamage))
        {
            ActiveSoldier.Instance.S.TakeDamage(null, otherDamage, false, new() { damageEventUI.damageSource.text }, Vector3.zero);
            MenuManager.Instance.CloseDamageEventUI();
        }
        else
        {
            //check input
            if (GetFallOrCollapseLocation(out Tuple<Vector3, string> fallCollapseLocation))
            {
                if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Fall"))
                    ActiveSoldier.Instance.S.TakeDamage(null, CalculateFallDamage(ActiveSoldier.Instance.S, int.Parse(damageEventUI.fallInput.text)), false, new() { "Fall" }, Vector3.zero);
                else if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Collapse"))
                {
                    int structureHeight = int.Parse(damageEventUI.structureHeight.text);
                    //add xp if survives, otherwise kill
                    if (ActiveSoldier.Instance.S.StructuralCollapseCheck(structureHeight))
                    {
                        MenuManager.Instance.AddXpAlert(ActiveSoldier.Instance.S, ActiveSoldier.Instance.S.stats.R.Val, $"Survived a {structureHeight}cm structural collapse.", true);
                        MenuManager.Instance.AddSoldierAlert(ActiveSoldier.Instance.S, "EVENT SURVIVED", Color.green, $"Survives a {structureHeight}cm structural collapse.", -1, -1);
                    }
                    else
                    {
                        if (ActiveSoldier.Instance.S.IsWearingJuggernautArmour(false))
                        {
                            ActiveSoldier.Instance.S.MakeUnconscious(null, new() { "Structural Collapse" });
                            MenuManager.Instance.AddSoldierAlert(ActiveSoldier.Instance.S, "EVENT SURVIVED", Color.green, $"Survives a {structureHeight}cm structural collapse with Juggernaut Armour.", -1, -1);
                        }
                        else
                        {
                            ActiveSoldier.Instance.S.InstantKill(null, new() { "Structural Collapse" });
                            ActiveSoldier.Instance.S.SetCrushed();
                        }
                    }
                }

                //move actually proceeds
                PerformMove(ActiveSoldier.Instance.S, 0, fallCollapseLocation, false, false, string.Empty, true);

                MenuManager.Instance.CloseDamageEventUI();

            }
            else
                print("Invalid Input");
        }
    }
    public bool GetFallOrCollapseLocation(out Tuple<Vector3, string> fallCollapseLocation)
    {
        fallCollapseLocation = default;
        if (HelperFunctions.ValidateIntInput(damageEventUI.xPos, out int x) && HelperFunctions.ValidateIntInput(damageEventUI.yPos, out int y) && HelperFunctions.ValidateIntInput(damageEventUI.zPos, out int z) && damageEventUI.terrainDropdown.value != 0)
        {
            fallCollapseLocation = Tuple.Create(new Vector3(x, y, z), damageEventUI.terrainDropdown.captionText.text);

            return true;
        }

        return false;
    }






    //inspirer functions
    public void CheckInspirer(Soldier inspirer)
    {
        MenuManager.Instance.SetInspirerResolvedFlagTo(false);
        bool openInspirerUI = false;

        foreach (Soldier friendly in AllSoldiers())
        {
            if (friendly.IsAbleToSee() && inspirer.IsSameTeamAs(friendly) && friendly.PhysicalObjectWithinMaxRadius(inspirer) && !friendly.IsRevoker())
            {
                MenuManager.Instance.AddInspirerAlert(friendly, "An Inspirer (" + inspirer.soldierName + ") is within SR range of " + friendly.soldierName + ". Is LOS present?");
                openInspirerUI = true;
            }
        }

        if (openInspirerUI)
            MenuManager.Instance.OpenInspirerUI();
        else
            MenuManager.Instance.SetInspirerResolvedFlagTo(true);
    }
    public void ConfirmInspirer()
    {
        ScrollRect inspireScroller = MenuManager.Instance.inspirerUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();

        if (inspireScroller.verticalNormalizedPosition <= 0.05f)
        {
            Transform inspirerAlerts = MenuManager.Instance.inspirerUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");

            foreach (Transform child in inspirerAlerts)
            {
                if (child.Find("InspirerToggle").GetComponent<Toggle>().isOn)
                    child.GetComponent<SoldierAlert>().soldier.SetInspired();
            }

            //destroy inspirer alerts
            foreach (Transform child in inspirerAlerts)
                Destroy(child.gameObject);

            MenuManager.Instance.CloseInspirerUI();
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }




    //xp functions - game
    public int CalculateShotKillXp(Soldier killer, Soldier deadMan)
    {
        int xp = 0;
        if (killer.IsOppositeTeamAs(deadMan)) //only pay xp if killing opponent
        {
            int rankDifference = killer.RankDifferenceTo(deadMan);

            if (deadMan.IsHigherRankThan(killer))
                xp = 6 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
            else
                xp = 6 - rankDifference;

            if (xp < 0)
                xp = 0;
        }
        return xp;
    }
    public int CalculateMeleeKillXp(Soldier killer, Soldier deadMan)
    {
        int xp = 0;
        if (killer.IsOppositeTeamAs(deadMan)) //only pay xp if killing opponent
        {
            int rankDifference = killer.RankDifferenceTo(deadMan);

            if (deadMan.IsHigherRankThan(killer))
                xp = 10 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
            else
                xp = 10 - rankDifference;

            if (xp < 0)
                xp = 0;
        }
        return xp;
    }
    public int CalculateMeleeCounterKillXp(Soldier killer, Soldier deadMan)
    {
        int xp = 0;
        if (killer.IsOppositeTeamAs(deadMan)) //only pay xp if killing opponent
        {
            int rankDifference = killer.RankDifferenceTo(deadMan);

            if (deadMan.IsHigherRankThan(killer))
                xp = 20 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
            else
                xp = 20 - rankDifference;

            if (xp < 0)
                xp = 0;
        }
        return xp;
    }





    //detection functions
    public float CalculateRange(PhysicalObject obj1, PhysicalObject obj2)
    {
        return Vector3.Distance(new Vector3(obj1.X, obj1.Y, obj1.Z), new Vector3(obj2.X, obj2.Y, obj2.Z));
    }
    public float CalculateRange(PhysicalObject s1, Vector3 point)
    {
        return Vector3.Distance(new Vector3(s1.X, s1.Y, s1.Z), point);
    }
    public string CalculateRangeBracket(float range)
    {
        if (range <= 3)
            return "Melee";
        else if (range <= 10)
            return "CQB";
        else if (range <= 30)
            return "Short";
        else if (range <= 70)
            return "Medium";
        else if (range <= 150)
            return "Long";
        else
            return "Coriolis";
    }
    public void DetectionAlertAllNonCoroutine()
    {
        //kill LOS between everyone
        foreach (Soldier s in AllSoldiers())
            s.RemoveAllLOSToAllSoldiers();

        SetLosCheckAll("losChange|losCheck"); //loscheckall
    }







    //insert game objects functions
    public void ConfirmInsertGameObjects()
    {
        //check input formatting
        if (insertObjectsUI.objectTypeDropdown.value != 0 && GetInsertLocation(out Tuple<Vector3, string> spawnLocation))
        {
            if (insertObjectsUI.objectTypeDropdown.value == 1)
            {
                GoodyBox gb = Instantiate(POIManager.Instance.gbPrefab).Init(spawnLocation);
                //fill gb with items
                foreach (Transform child in insertObjectsUI.gbItemsPanel)
                {
                    ItemIconGB itemIcon = child.GetComponent<ItemIconGB>();
                    if (itemIcon != null && itemIcon.pickupNumber > 0)
                        for (int i = 0; i < child.GetComponent<ItemIconGB>().pickupNumber; i++)
                            gb.Inventory.AddItem(ItemManager.Instance.SpawnItem(child.gameObject.name));
                }
            }
            else if (insertObjectsUI.objectTypeDropdown.value == 2)
                Instantiate(POIManager.Instance.terminalPrefab).Init(spawnLocation, insertObjectsUI.terminalTypeDropdown.captionText.text);
            else if (insertObjectsUI.objectTypeDropdown.value == 3)
                Instantiate(POIManager.Instance.barrelPrefab).Init(spawnLocation);
            else if (insertObjectsUI.objectTypeDropdown.value == 4)
            {
                DrugCabinet dc = Instantiate(POIManager.Instance.drugCabinetPrefab).Init(spawnLocation);
                //fill dc with items
                foreach (Transform child in insertObjectsUI.dcItemsPanel)
                {
                    ItemIconGB itemIcon = child.GetComponent<ItemIconGB>();
                    if (itemIcon != null && itemIcon.pickupNumber > 0)
                        for (int i = 0; i < child.GetComponent<ItemIconGB>().pickupNumber; i++)
                            dc.Inventory.AddItem(ItemManager.Instance.SpawnItem(child.gameObject.name));
                }
            }

            MenuManager.Instance.CloseOverrideInsertObjectsUI();
        }
        else
            print("Invalid Input");
    }
    public bool GetInsertLocation(out Tuple<Vector3, string> insertLocation)
    {
        insertLocation = default;
        if (HelperFunctions.ValidateIntInput(insertObjectsUI.xPos, out int x) && HelperFunctions.ValidateIntInput(insertObjectsUI.yPos, out int y) && HelperFunctions.ValidateIntInput(insertObjectsUI.zPos, out int z))
        {
            insertLocation = Tuple.Create(new Vector3(x, y, z), string.Empty);

            return true;
        }

        return false;
    }
    public void UpdateInsertGameObjectsUI()
    {
        insertObjectsUI.terminalTypeUI.SetActive(false);
        insertObjectsUI.allItemsPanelUI.SetActive(false);
        insertObjectsUI.allDrugsPanelUI.SetActive(false);

        if (insertObjectsUI.objectTypeDropdown.value == 1)
        {
            insertObjectsUI.allItemsPanelUI.SetActive(true);
            activeItemPanel = insertObjectsUI.allItemsPanelUI.transform;
        }
        else if (insertObjectsUI.objectTypeDropdown.value == 2)
        {
            insertObjectsUI.terminalTypeUI.SetActive(true);
        }
        else if (insertObjectsUI.objectTypeDropdown.value == 4)
        {
            insertObjectsUI.allDrugsPanelUI.SetActive(true);
            activeItemPanel = insertObjectsUI.allDrugsPanelUI.transform;
        }
    }


    public bool GameRunning 
    {
        get {  return gameRunning; } set { gameRunning = value; }
    }


    public void LoadData(GameData data)
    {
        currentRound = data.currentRound;
        currentTeam = data.currentTeam;
        currentTurn = data.currentTurn;

        battlefield.transform.position = data.mapPosition;
        battlefield.transform.localScale = data.mapDimensions;

        bottomPlane.transform.position = data.bottomPlanePosition;
        bottomPlane.transform.localScale = data.bottomPlaneDimensions;

        outlineArea.transform.position = data.outlineAreaPosition;
        outlineArea.transform.localScale = data.outlineAreaDimensions;

        cam.transform.position = data.camPosition;
        cam.orthographicSize = data.camOrthoSize;
        sun.transform.position = data.sunPosition;
        maxRounds = data.maxRounds;
        maxTurnTime = data.maxTurnTime;
        maxTeams = data.maxTeams;
        maxX = data.maxX;
        maxY = data.maxY;
        maxZ = data.maxZ;

        isDataLoaded = true;
    }
    public void SaveData(ref GameData data)
    {
        data.currentRound = currentRound;
        data.currentTeam = currentTeam;
        data.currentTurn = currentTurn;

        data.mapPosition = battlefield.transform.position;
        data.mapDimensions = battlefield.transform.localScale;
        data.camPosition = cam.transform.position;
        data.camOrthoSize = cam.orthographicSize;
        data.sunPosition = sun.transform.position;
        data.maxRounds = maxRounds;
        data.maxTurnTime = maxTurnTime;
        data.maxTeams = maxTeams;
        data.maxX = maxX;
        data.maxY = maxY;
        data.maxZ = maxZ;
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
