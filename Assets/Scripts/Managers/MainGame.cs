using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics;

public class MainGame : MonoBehaviour, IDataPersistence
{
    [SerializeField]
    private bool gameRunning;

    public MainMenu menu;
    public ItemManager itemManager;
    public SoldierManager soldierManager;
    public POIManager poiManager;
    public WeatherGen weather;
    public DipelecGen dipelec;
    public SoundManager soundManager;

    public MoveUI moveUI;
    public ShotUI shotUI;
    public MeleeUI meleeUI;
    public ConfigureUI configUI;
    public DipElecUI dipelecUI;
    public DamageEventUI damageEventUI;
    public OverwatchUI overwatchUI;
    public InsertObjectsUI insertObjectsUI;

    public bool gameOver, modaTurn, frozenTurn;
    public int maxX, maxY, maxZ;
    public int currentRound, currentTeam, currentTurn, maxRounds, maxTeams, maxTurnTime, tempTeam;
    public Camera cam;
    public Light sun;
    public GameObject battlefield, bottomPlane, outlineArea;
    Vector3 boundCrossOne = Vector3.zero, boundCrossTwo = Vector3.zero;
    public List<Tuple<string, string>> shotParameters = new(), meleeParameters = new();
    public Tuple<Vector3, string, int, int> tempMove;
    public Tuple<Soldier, IAmShootable> tempShooterTarget;
    public Soldier tempSoldier;
    public List<string> tempDamageSource;

    public Transform allItemsContentUI, inventoryItemsContentUI, groundItemsContentUI, activeItemPanel, allyButtonContentUI;
    public Soldier activeSoldier;

    public MainGame Init()
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
        return soldierManager.allSoldiers;
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
    public int DiceRoll()
    {
        return HelperFunctions.RandomNumber(1, 6);
    }
    public bool CoinFlip()
    {
        if (HelperFunctions.RandomNumber(0, 1) == 1)
            return true;

        return false;
    }
    public int Factorial(int number)
    {
        if (number == 0)
            return 1;
        else
        {
            int result = 1;

            while (number != 1)
            {
                result *= number;
                number--;
            }
            return result;
        }
    }
    public float BinomialProbability(int n, int x, float p, float q)
    {
        if (n >= x)
            return Factorial(n) / (Factorial(n - x) * Factorial(x)) * Mathf.Pow(p, x) * Mathf.Pow(q, n - x);
        else
            return 0;
    }
    public float CumulativeBinomialProbability(int n, int x, float p, float q)
    {
        if (n >= x)
        {
            float result = 0;

            while (x <= n)
            {
                result += BinomialProbability(n, x, p, q);
                x++;
            }
            return result;
        }
        else
            return 0;
    }
    public string RandomShotScatterDistance()
    {
        return decimal.Round((decimal)UnityEngine.Random.Range(0.5f, 5.0f), 1).ToString();
    }
    public string RandomShotScatterHorizontal()
    {
        if (CoinFlip())
            return "left";
        else
            return "right";
    }
    public string RandomShotScatterVertical()
    {
        if (CoinFlip())
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

        if (menu.OverrideView)
            menu.UnsetOverrideView();

        menu.FreezeTimer();
        menu.DisplaySoldiersGameOver();
        menu.roundIndicator.text = "Game Over";
        menu.teamTurnIndicator.text = result;

        //play game over music
        soundManager.PlayGameOverMusic();
    }
    public void SwitchTeam(int team)
    {
        tempTeam = currentTeam;
        currentTeam = team;
    }
    public void StartModaTurn(Soldier modaSoldier, Soldier killedBy, List<string> damageSource)
    {
        menu.FreezeTimer();

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
        menu.UnfreezeTimer();

        //update temp parameters
        tempDamageSource.Add("Modafinil");

        //deactivate modafinil turn
        activeSoldier.Kill(tempSoldier, tempDamageSource);
        modaTurn = false;
        SwitchTeam(tempTeam);
    }
    public void StartFrozenTurn(Soldier frozenSoldier)
    {
        menu.SetOverrideView();

        //change game parameters
        frozenTurn = true;
        SwitchTeam(frozenSoldier.soldierTeam);

        //activate the surviving soldier
        frozenSoldier.soldierUI.GetComponent<SoldierUI>().OpenSoldierMenu("frozen");
        menu.OpenShotUI();
    }
    public void EndFrozenTurn()
    {
        menu.UnsetOverrideView();

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
                menu.SetTeamTurnOverFlagTo(false);

                EndTeamTurn();

                yield return new WaitUntil(() => menu.teamTurnOverFlag == true);

                currentTurn++;
                if (currentTeam < maxTeams)
                {
                    currentTeam++;
                }
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
                if (menu.lostLosUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content").childCount > 0)
                    StartCoroutine(menu.OpenLostLOSList());

                menu.SetTeamTurnStartFlagTo(false);

                yield return new WaitUntil(() => menu.teamTurnStartFlag == true);

                menu.turnTime = 0;
                soundManager.banzaiPlayed = false;
                StartTurn();
            }
        }
    }
    public void EndTeamTurn()
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
                        menu.AddXpAlert(s, 1, $"Survived {s.roundsFieldedConscious} rounds.", true);
                }

                //increase rounds without food
                s.IncreaseRoundsWithoutFood();

                //unset suppression
                s.UnsetSuppression();
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

        menu.CheckXP();

        //post xp stuff
        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.IsOnturnAndAlive()) //run things that trigger at the end of friendly team turn
            {
                //dish out poison damage
                if (s.IsPoisoned())
                    StartCoroutine(s.TakePoisonDamage());
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
                    StartCoroutine(s.BleedoutKill());    
            }

            //increment recon binoculars
            if (s.IsUsingBinocularsInReconMode())
            {
                if (poiManager.FindPOIById(s.binocularBeamId.Split("|")[0]) is BinocularBeam binocBeam)
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
            FileUtility.WriteToReport($"\nRound {currentRound} | Team: {currentTeam}");
            if (currentTeam == 1)
                StartRound();

            IncreaseTurnsActiveAllClouds();
            DecreaseTurnsSpyingJammingAllULFs();
            StartCoroutine(StartTeamTurn());
        }
    }
    public IEnumerator StartTeamTurn()
    {
        if (menu.damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content").childCount > 0)
            StartCoroutine(menu.OpenDamageList());

        foreach (Soldier s in AllFieldedSoldiers())
        {
            if (s.IsOnturnAndAlive())
            {
                //patriot ability
                s.SetPatriotic();

                //run things that trigger at the start of players turn
                if (s.IsInspirer())
                    CheckInspirer(s);

                yield return new WaitUntil(() => menu.inspirerResolvedFlag);

                s.GenerateAP();
            }
            else
            {
                //run things that trigger at the start of another team's turn
            }
        }
        menu.CheckXP();

        //run los checks only if weather changes
        if (CheckWeatherChange(weather.LastTurnVis, weather.CurrentVis) != "false")
            SetLosCheckAll("statChange(SR)|weatherChange"); //loscheckall
        //losCheck will automatically run due to collider change
    }
    public void StartRound()
    {

    }

    public string CheckWeatherChange(string lastTurnVis, string currentVis)
    {
        int lastTurnVisVal, currentVisVal;

        lastTurnVisVal = lastTurnVis switch
        {
            "Full" => 0,
            "Good" => 1,
            "Moderate" => 2,
            "Poor" => 3,
            "Zero" => 4,
            _ => 0,
        };
        currentVisVal = currentVis switch
        {
            "Full" => 0,
            "Good" => 1,
            "Moderate" => 2,
            "Poor" => 3,
            "Zero" => 4,
            _ => 0,
        };
        if (currentVisVal < lastTurnVisVal)
            return "increase";
        else if (currentVisVal > lastTurnVisVal)
            return "decrease";
        else
            return "false";
    }

    //draining soldier AP and MP after use during turn
    public void EndSoldierTurn()
    {
        activeSoldier.DrainAP();
        activeSoldier.DrainMP();
    }

    //cover functions
    public void ConfirmCover()
    {
        if (activeSoldier.CheckAP(1))
        {
            activeSoldier.DeductAP(1);
            activeSoldier.SetCover();
        }

        menu.CloseTakeCoverUI();
    }

    //playdead functions
    public void CheckPlaydead()
    {
        if (activeSoldier.IsPlayingDead())
            activeSoldier.UnsetPlaydead();
        else
            menu.OpenPlaydeadAlertUI();
    }
    public void ConfirmPlaydead()
    {
        activeSoldier.SetPlaydead();
        menu.ClosePlaydeadAlertUI();
    }

    //overwatch functions
    public void ConfirmOverwatch()
    {
        if (menu.ValidateIntInput(overwatchUI.xPos, out int x) && menu.ValidateIntInput(overwatchUI.yPos, out int y) && menu.ValidateIntInput(overwatchUI.radius, out int r) && menu.ValidateIntInput(overwatchUI.arc, out int a))
        {
            if (activeSoldier.CheckAP(2))
            {
                //play overwatch confirm dialogue
                soundManager.PlaySoldierEnterOverwatch(activeSoldier.soldierSpeciality);

                activeSoldier.DrainAP();
                activeSoldier.SetOverwatch(x, y, r, a);
            }
            menu.CloseOverwatchUI();
        }

    }


    //move functions
    public void FleeSoldier()
    {
        activeSoldier.InstantKill(activeSoldier, new List<string>() { "Flee" });
    }
    public int CalculateFallDamage(Soldier soldier, int fallDistance)
    {
        int damage = Mathf.CeilToInt(Mathf.Pow(fallDistance / 4.0f, 2) / 2.0f - soldier.stats.R.Val);

        //play fall damage sfx
        if (damage > 0)
            soundManager.PlayFallFromHeight();

        return Mathf.CeilToInt(Mathf.Pow(fallDistance / 4.0f, 2) / 2.0f - soldier.stats.R.Val);
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
            if (ap > 1 && activeSoldier.InstantSpeed <= 6)
                ap--;

            if (ap > 1 && activeSoldier.IsSprinter())
                ap--;

            if (moveUI.coverToggle.isOn)
                ap++;
        }

        moveUI.apCost.text = ap.ToString();
    }
    public void UpdateMoveDonated()
    {
        if (moveUI.moveTypeDropdown.captionText.text.Contains("Planner"))
            moveUI.moveDonated.text = activeSoldier.HalfMove.ToString();
    }
    public void UpdateMoveUI()
    {
        if (!menu.clearMoveFlag)
        {
            UpdateMoveAP();
            UpdateMoveDonated();
        }
    }
    public bool GetMoveLocation(out Tuple<Vector3, string> moveLocation)
    {
        moveLocation = default;
        if (menu.ValidateIntInput(moveUI.xPos, out int x) && menu.ValidateIntInput(moveUI.yPos, out int y) && menu.ValidateIntInput(moveUI.zPos, out int z) && moveUI.terrainDropdown.value != 0)
        {
            moveLocation = Tuple.Create(new Vector3(x, y, z), moveUI.terrainDropdown.captionText.text);
            return true;
        }

        return false;
    }
    public void ConfirmMove(bool force)
    {
        int.TryParse(moveUI.apCost.text, out int ap);

        if (moveUI.moveTypeDropdown.captionText.text.Contains("Planner"))
        {
            if (activeSoldier.CheckMP(1) && activeSoldier.CheckAP(ap))
            {
                //planner donation proceeds
                activeSoldier.DeductAP(ap);
                activeSoldier.DrainMP();
                foreach (Transform child in moveUI.closestAllyUI.transform.Find("ClosestAllyPanel"))
                    soldierManager.FindSoldierByName(child.Find("SoldierName").GetComponent<TextMeshProUGUI>().text).plannerDonatedMove += activeSoldier.HalfMove;
            }
            menu.CloseMoveUI();
        }
        else if (moveUI.moveTypeDropdown.captionText.text.Contains("Exo"))
        {
            if (GetMoveLocation(out Tuple<Vector3, string> moveToLocation))
            {
                if (activeSoldier.X != moveToLocation.Item1.x || activeSoldier.Y != moveToLocation.Item1.y || activeSoldier.Z != moveToLocation.Item1.z)
                {
                    if (force || (moveToLocation.Item1.x <= activeSoldier.X + 3 && moveToLocation.Item1.x >= activeSoldier.X - 3 && moveToLocation.Item1.y <= activeSoldier.Y + 3 && moveToLocation.Item1.y >= activeSoldier.Y - 3))
                    {
                        if (activeSoldier.CheckAP(ap))
                        {
                            //play move dialogue
                            if (moveUI.meleeToggle.isOn)
                                soundManager.PlaySoldierMeleeMove(activeSoldier.soldierSpeciality); //play melee move dialogue
                            else
                                soundManager.PlaySoldierConfirmMove(activeSoldier.soldierSpeciality); //play standard move dialogue

                            PerformMove(activeSoldier, ap, moveToLocation, moveUI.meleeToggle.isOn, moveUI.coverToggle.isOn, moveUI.fallInput.text, true);

                            //trigger loud action
                            activeSoldier.PerformLoudAction(10);
                        }
                        menu.CloseMoveUI();
                    }
                    else
                        menu.OpenOvermoveUI("Warning: Landing is further than 3cm away from jump point.");
                }
                else
                    menu.generalAlertUI.Activate("Cannot move to same location, use OVERRIDE if terrain change is required");
            }
        }
        else
        {
            //get maxmove
            float maxMove;
            if (moveUI.moveTypeDropdown.captionText.text.Contains("Full"))
                maxMove = activeSoldier.FullMove;
            else if (moveUI.moveTypeDropdown.captionText.text.Contains("Half"))
                maxMove = activeSoldier.HalfMove;
            else
                maxMove = activeSoldier.TileMove;

            if (GetMoveLocation(out Tuple<Vector3, string> moveToLocation))
            {
                if (activeSoldier.X != moveToLocation.Item1.x || activeSoldier.Y != moveToLocation.Item1.y || activeSoldier.Z != moveToLocation.Item1.z)
                {
                    //skip supression check if it's already happened before, otherwise run it
                    if (!activeSoldier.IsSuppressed() || (activeSoldier.IsSuppressed() && (moveUI.moveTypeDropdown.interactable == false || activeSoldier.SuppressionCheck())))
                    {
                        int distance = CalculateMoveDistance(moveToLocation.Item1);
                        if (force || distance <= maxMove)
                        {
                            if (activeSoldier.CheckMP(1) && activeSoldier.CheckAP(ap)) 
                            {
                                //play move dialogue
                                if (moveUI.meleeToggle.isOn)
                                    soundManager.PlaySoldierMeleeMove(activeSoldier.soldierSpeciality); //play melee move dialogue
                                else
                                {
                                    if (!moveUI.moveTypeDropdown.captionText.text.Contains("Tile"))
                                        soundManager.PlaySoldierConfirmMove(activeSoldier.soldierSpeciality); //play standard move dialogue
                                }

                                PerformMove(activeSoldier, ap, moveToLocation, moveUI.meleeToggle.isOn, moveUI.coverToggle.isOn, moveUI.fallInput.text, false);
                            }
                            menu.CloseMoveUI();
                        }
                        else
                            menu.OpenOvermoveUI($"Warning: Proposed move is {distance - maxMove} cm over max ({distance}/{maxMove})");
                    }
                    else
                        menu.OpenSuppressionMoveUI();
                }
                else
                    menu.generalAlertUI.Activate("Cannot move to same location, use OVERRIDE if terrain change is required");
            }
            else
                print("Invalid Input");
        }
    }
    public void PerformSpawn(Soldier movingSoldier, Tuple<Vector3, string> moveToLocation)
    {
        movingSoldier.moveResolvedFlag = false;
        FileUtility.WriteToReport($"{movingSoldier.soldierName} spawned at {moveToLocation}");

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
        FileUtility.WriteToReport($"{movingSoldier.soldierName} moved to {moveToLocation}");

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
        BreakAllControllingMeleeEngagments(movingSoldier);

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
            movingSoldier.TakeDamage(movingSoldier, CalculateFallDamage(movingSoldier, fallDistanceInt), false, new() { "Fall" });

        //launch melee if melee toggle is on
        if (meleeToggle)
        {
            if (moveUI.moveTypeDropdown.value == 0)
                launchMelee = "Full Charge Attack";
            else if (moveUI.moveTypeDropdown.value == 1)
                launchMelee = "Half Charge Attack";
            else if (moveUI.moveTypeDropdown.value == 2)
                launchMelee = "3cm Charge Attack";
            StartCoroutine(menu.OpenMeleeUI(launchMelee));
        }

        //check broken soldier leaving field
        CheckBrokenFlee(movingSoldier);

        movingSoldier.SetLosCheck("losChange|move"); //losCheck
    }
    public void CheckBrokenFlee(Soldier movingSoldier)
    {
        if (movingSoldier.IsBroken())
            if (movingSoldier.X == movingSoldier.startX && movingSoldier.Y == movingSoldier.startY && movingSoldier.Z == movingSoldier.startZ)
                menu.OpenBrokenFledUI();
    }
    public void CheckDeploymentBeacons(Soldier fieldedSoldier)
    {
        foreach (DeploymentBeacon beacon in FindObjectsByType<DeploymentBeacon>(default))
            if (beacon.placedBy.IsSameTeamAs(fieldedSoldier) && fieldedSoldier.X == beacon.X && fieldedSoldier.Y == beacon.Y && fieldedSoldier.Z == beacon.Z)
                menu.AddXpAlert(beacon.placedBy, 1, $"Ally ({fieldedSoldier.soldierName}) deployed through beacon at ({beacon.X}, {beacon.Y}, {beacon.Z})", true);
    }











    //shot functions
    public void UpdateShotUI(Soldier shooter)
    {
        //if function is called not from a script, shooter has to be determined from interface
        if (shooter.id == "0")
            shooter = soldierManager.FindSoldierById(shotUI.shooterID.text);

        if (!menu.clearShotFlag)
        {
            UpdateShotAP(shooter);
            if (shotUI.shotTypeDropdown.value == 0)
                UpdateTarget(shooter);
            else
                UpdateSuppressionValue(shooter);
        }
    }
    public void UpdateShotType(Soldier shooter)
    {
        //if function is called not from a script, shooter has to be determined from interface
        if (shooter.id == "0")
            shooter = soldierManager.FindSoldierById(shotUI.shooterID.text);

        List<TMP_Dropdown.OptionData> targetOptionDataList = new();

        //initialise
        shotUI.targetDropdown.ClearOptions();
        shotUI.aimTypeUI.SetActive(false);
        shotUI.suppressionValueUI.SetActive(false);
        shotUI.coverLocationUI.SetActive(false);

        //generate target list
        foreach (Soldier s in AllFieldedSoldiers())
        {
            TMP_Dropdown.OptionData targetOptionData = null;
            if (s.IsAlive() && shooter.IsOppositeTeamAs(s) && s.IsRevealed())
            {
                if (shooter.CanSeeInOwnRight(s))
                    targetOptionData = new(s.Id, s.soldierPortrait, Color.white);
                else
                {
                    if (s.IsJammer() && !shooter.IsRevoker())
                        targetOptionData = new(s.Id, s.LoadPortraitJammed(s.soldierPortraitText), Color.white);
                    else
                        targetOptionData = new(s.Id, s.LoadPortraitTeamsight(s.soldierPortraitText), Color.white);
                }

            }

            if (targetOptionData != null)
            {
                targetOptionDataList.Add(targetOptionData);

                //remove option if soldier is engaged and this soldier is not on the engagement list
                if (activeSoldier.IsMeleeEngaged() && !activeSoldier.IsMeleeEngagedWith(s))
                    targetOptionDataList.Remove(targetOptionData);
            }
        }

        if (shotUI.shotTypeDropdown.value == 0)
        {
            shotUI.aimTypeUI.SetActive(true);

            //add explosive barrels to target list
            foreach (ExplosiveBarrel b in FindObjectsByType<ExplosiveBarrel>(default))
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (shooter.PhysicalObjectIsRevealed(b))
                    targetOptionData = new(b.Id, menu.explosiveBarrelSprite, Color.white);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }

            //add coverman
            targetOptionDataList.Add(new("coverman", menu.covermanSprite, Color.white));
        }
        else if (shotUI.shotTypeDropdown.value == 1)
            shotUI.suppressionValueUI.SetActive(true);

        shotUI.targetDropdown.AddOptions(targetOptionDataList);
        UpdateShotUI(shooter);
    }
    public void UpdateShotAP(Soldier shooter)
    {
        int ap = 1;
        if (shotUI.shotTypeDropdown.value == 1)
        {
            ap = shooter.ap;
        }
        else
        {
            if (shotUI.aimTypeDropdown.value == 0)
            {
                if (shooter.IsGunner())
                    ap++;
                else
                {
                    bool hasLMGOrSniper = false;
                    foreach (Item gun in shooter.EquippedGuns)
                        if (gun.SpecialityTag().Equals("Sniper Rifle") || gun.SpecialityTag().Equals("Light Machine Gun"))
                            hasLMGOrSniper = true;

                    if (hasLMGOrSniper)
                        ap += 2;
                    else
                        ap++;
                }
            }
        }

        //set ap to 0 for overwatch
        if (shooter.soldierTeam != currentTeam)
            ap = 0;

        shotUI.apCost.text = ap.ToString();
    }
    public void UpdateTarget(Soldier shooter)
    {
        print($"caption text -> {shotUI.targetDropdown.captionText.text}");
        IAmShootable target = FindShootableById(shotUI.targetDropdown.captionText.text);

        //initialise
        shotUI.coverLocationUI.SetActive(false);
        shotUI.barrelLocationUI.SetActive(false);
        shotUI.coverLevelUI.SetActive(false);

        print($"updating target -> {target}");
        if (target is Coverman)
        {
            print("targeting coverman");
            shotUI.coverLocationUI.SetActive(true);
            shotUI.shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression");
            menu.ClearFlankersUI(menu.flankersShotUI);
        }
        else if (target is ExplosiveBarrel targetBarrel)
        {
            shotUI.barrelLocation.text = $"X:{targetBarrel.X} Y:{targetBarrel.Y} Z:{targetBarrel.Z}";
            shotUI.barrelLocationUI.SetActive(true);
            shotUI.shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression");
            menu.ClearFlankersUI(menu.flankersShotUI);
        }
        else if (target is Soldier targetSoldier)
        {
            if (targetSoldier.IsInCover())
                shotUI.coverLevelUI.SetActive(true);

            UpdateTargetFlanking(shooter, targetSoldier);
        }
    }
    public void UpdateSuppressionValue(Soldier shooter)
    {
        int suppressionValue = 0;
        IAmShootable target = FindShootableById(shotUI.targetDropdown.captionText.text);
        string suppressionBracket = CalculateRangeBracket(CalculateRange(shooter, target as PhysicalObject)) switch
        {
            "Melee" or "CQB" => "cQBSupPen",
            "Short" => "shortSupPen",
            "Medium" => "medSupPen",
            "Long" or "Coriolis" => "longSupPen",
            _ => "",
        };

        print($"{suppressionBracket}");
        if (shotUI.gunDropdown.value == 0)
            suppressionValue = shooter.EquippedGuns[0].GetSuppressionValue(suppressionBracket);
        else if (shotUI.gunDropdown.value == 1)
            suppressionValue = (int)shooter.EquippedGuns[1].GetSuppressionValue(suppressionBracket);
        else if (shotUI.gunDropdown.value == 2)
        {
            foreach (Item gun in shooter.EquippedGuns)
            {
                print($"{gun.itemName}");
                print($"pre {suppressionValue}");
                suppressionValue = HelperFunctions.CalculateSuppression(suppressionValue, gun.GetSuppressionValue(suppressionBracket));
                print($"post {suppressionValue}");
            }
        }

        shotUI.suppressionValue.text = suppressionValue.ToString();
    }
    public void UpdateTargetFlanking(Soldier shooter, Soldier target)
    {
        //clear the flanker ui
        menu.ClearFlankersUI(menu.flankersShotUI);
        int flankersCount = 0;
        int flankingAngle = 80;
        List<Tuple<float, Soldier>> allFlankingAngles = new();
        List<Tuple<float, Soldier>> confirmedFlankingAngles = new();

        if (shooter.IsTactician() && !target.IsRevoker())
            flankingAngle = 20;

        if (target != null)
        {
            if (!target.IsTactician() || shooter.IsRevoker())
            {
                //find all soldiers who could be considered for flanking and their flanking angles
                foreach (Soldier s in AllFieldedSoldiers())
                    if (s.IsAbleToSee() && s.IsSameTeamAs(shooter) && s.CanSeeInOwnRight(target) && !HelperFunctions.IsWithinAngle(new(shooter.X, shooter.Y), new(s.X, s.Y), new(target.X, target.Y), 80f))
                        allFlankingAngles.Add(Tuple.Create(HelperFunctions.CalculateAngle360(new(shooter.X, shooter.Y), new(s.X, s.Y), new(target.X, target.Y)), s));

                //order smallest angle to largest angle
                allFlankingAngles = allFlankingAngles.OrderBy(t => t.Item1).ToList();

                /*string msg = "";
                foreach (var item in allFlankingAngles)
                    msg += $"({item.Item1}, {item.Item2.soldierName})";
                print(msg);*/

                // Iterate through both lists to find out which gives more flanking options
                for (int i = 0; i < allFlankingAngles.Count; i++)
                {
                    Tuple<float, Soldier> currentItem = allFlankingAngles[i];
                    print($"current item {currentItem}");
                    // Check if Item1 is greater than flankingAngle
                    if (currentItem.Item1 > flankingAngle)
                    {
                        confirmedFlankingAngles.Add(currentItem);
                        allFlankingAngles = allFlankingAngles.Select(t => Tuple.Create(Math.Abs(t.Item1 - currentItem.Item1), t.Item2)).ToList();
                    }
                }

                foreach (Tuple<float, Soldier> confirmedFlankAngle in confirmedFlankingAngles)
                {
                    if (flankersCount < 3)
                    {
                        flankersCount++;

                        //add flanker to ui to visualise
                        GameObject flankerPortrait = Instantiate(menu.soldierPortraitPrefab, menu.flankersShotUI.transform.Find("FlankersPanel"));
                        flankerPortrait.GetComponentInChildren<SoldierPortrait>().Init(confirmedFlankAngle.Item2);
                    }
                }

                //display flankers if there are any
                if (flankersCount > 0)
                    menu.OpenFlankersUI(menu.flankersShotUI);
            }
        }
    }
    public int WeaponHitChance(Soldier shooter, IAmShootable target, Item gun)
    {
        int weaponHitChance, baseWeaponHitChance, sharpshooterBonus = 0, inspiredBonus = 0;

        //get base hit chance
        switch (CalculateRangeBracket(CalculateRange(shooter, target as PhysicalObject)))
        {
            case "Melee":
            case "CQB":
                if (shotUI.aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.cQBA;
                else
                    baseWeaponHitChance = gun.cQBU;
                break;
            case "Short":
                if (shotUI.aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.shortA;
                else
                    baseWeaponHitChance = gun.shortU;
                break;
            case "Medium":
                if (shotUI.aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.medA;
                else
                    baseWeaponHitChance = gun.medU;
                break;
            case "Long":
                if (shotUI.aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.longA;
                else
                    baseWeaponHitChance = gun.longU;
                break;
            case "Coriolis":
                if (shotUI.aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.coriolisA;
                else
                    baseWeaponHitChance = gun.coriolisU;
                break;
            default:
                baseWeaponHitChance = 0;
                break;
        }
        weaponHitChance = baseWeaponHitChance;

        //apply sharpshooter buff
        if (baseWeaponHitChance > 0 && shooter.IsSharpshooter() && target is Soldier targetSoldier && !targetSoldier.IsRevoker())
            sharpshooterBonus = 5;
        weaponHitChance += sharpshooterBonus;

        //apply inspirer buff
        inspiredBonus += shooter.InspirerBonusWeapon(gun);
        weaponHitChance += inspiredBonus;

        //correct negatives
        if (weaponHitChance < 0)
            weaponHitChance = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("accuracy", $"{baseWeaponHitChance}"));
        shotParameters.Add(Tuple.Create("sharpshooter", $"{sharpshooterBonus}"));
        shotParameters.Add(Tuple.Create("inspired", $"{inspiredBonus}"));

        return weaponHitChance;
    }
    public float ShooterTraumaMod(Soldier shooter)
    {
        float traumaMod = shooter.tp switch
        {
            1 => 0.1f,
            2 => 0.2f,
            3 => 0.4f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("trauma", $"{1 - traumaMod}"));

        return 1 - traumaMod;
    }
    public float ShooterSustenanceMod(Soldier shooter)
    {
        float sustenanceMod = 0;

        if (shooter.roundsWithoutFood >= 20)
            sustenanceMod = 0.5f;

        //report parameters
        shotParameters.Add(Tuple.Create("sustenance", $"{1 - sustenanceMod}"));

        return 1 - sustenanceMod;
    }
    public float ShooterSmokeMod(Soldier shooter)
    {
        float smokeMod = 0;

        if (shooter.IsSmokeBlinded())
            smokeMod = 0.9f;
        else if (shooter.IsSmokeCovered())
            smokeMod = 0.45f;

        //report parameters
        shotParameters.Add(Tuple.Create("smoke", $"{1 - smokeMod}"));

        return 1 - smokeMod;
    }
    public float ShooterTabunMod(Soldier shooter)
    {
        float tabunMod = 0;

        if (shooter.IsInTabun())
        {
            if (shooter.CheckTabunEffectLevel(100))
                tabunMod = 0.8f;
            else if (shooter.CheckTabunEffectLevel(50))
                tabunMod = 0.4f;
            else if (shooter.CheckTabunEffectLevel(25))
                tabunMod = 0.2f;
        }

        //report parameters
        shotParameters.Add(Tuple.Create("tabun", $"{1 - tabunMod}"));

        return 1 - tabunMod;
    }
    public int ShooterFightMod(Soldier shooter)
    {
        int fightMod = 0;

        if (shooter.FightActive())
            fightMod = 5 * shooter.stats.F.Val;
        else if (shooter.AvengingActive()) //avenger ability
            fightMod = 5 * (shooter.stats.F.Val - 1);

        //report parameters
        shotParameters.Add(Tuple.Create("fight", $"{fightMod}"));

        return fightMod;
    }
    public float RelevantWeaponSkill(Soldier shooter, Item gun)
    {
        int juggernautBonus = 0, stimBonus = 0;

        float weaponSkill = gun.SpecialityTag() switch
        {
            "Assault Rifle" => shooter.stats.AR.Val,
            "Light Machine Gun" => shooter.stats.LMG.Val,
            "Rifle" => shooter.stats.Ri.Val,
            "Shotgun" => shooter.stats.Sh.Val,
            "Sub-Machine Gun" => shooter.stats.SMG.Val,
            "Sniper Rifle" => shooter.stats.Sn.Val,
            "Pistol" or _ => shooter.stats.GetHighestWeaponSkill(),
        };
        //report parameters
        shotParameters.Add(Tuple.Create("WS", $"{weaponSkill}"));

        //apply juggernaut armour debuff
        if (shooter.IsWearingJuggernautArmour(false))
            juggernautBonus = -1;
        weaponSkill += juggernautBonus;
        //report parameters
        shotParameters.Add(Tuple.Create("juggernaut", $"{juggernautBonus}"));

        //apply stim armour buff
        if (shooter.IsWearingStimulantArmour())
            stimBonus = 2;
        weaponSkill += stimBonus;
        //report parameters
        shotParameters.Add(Tuple.Create("stim", $"{stimBonus}"));

        //apply trauma debuff
        weaponSkill *= ShooterTraumaMod(shooter);

        //apply sustenance debuff
        weaponSkill *= ShooterSustenanceMod(shooter);

        //correct negatives
        if (weaponSkill < 0)
            weaponSkill = 0;

        return weaponSkill;
    }
    public int TargetEvasion(IAmShootable target)
    {
        int targetE = 0;
        if (target is Soldier targetSoldier)
            targetE = targetSoldier.stats.E.Val;
        shotParameters.Add(Tuple.Create("tE", $"{targetE}"));
        return targetE;
    }
    public float CoverMod()
    {
        var coverMod = shotUI.coverLevelDropdown.value switch
        {
            1 => 0.1f,
            2 => 0.34f,
            3 => 0.62f,
            4 => 0.88f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("cover", $"{1 - coverMod}"));
        return 1 - coverMod;
    }
    public float VisMod(Soldier shooter)
    {
        float visMod;

        if (shooter.IsWearingThermalGoggles())
            visMod = 0.0f;
        else
        {
            visMod = weather.CurrentVis switch
            {
                "Full" => 0.0f,
                "Good" => 0.02f,
                "Moderate" => 0.18f,
                "Poor" => 0.26f,
                "Zero" => 0.64f,
                _ => 0.0f,
            };
        }

        //report parameters
        shotParameters.Add(Tuple.Create("vis", $"{1 - visMod}"));
        return 1 - visMod;
    }
    public float RainMod(Soldier shooter, IAmShootable target)
    {
        string rainfall = weather.CurrentRain;

        if (shooter.IsCalculator() && target is Soldier targetSoldier1 && !targetSoldier1.IsRevoker())
            rainfall = weather.DecreasedRain(rainfall);
        if (target is Soldier targetSoldier2 && targetSoldier2.IsCalculator() && !shooter.IsRevoker())
            rainfall = weather.IncreasedRain(rainfall);

        float rainMod = rainfall switch
        {
            "Zero" or "Light" => 0.0f,
            "Moderate" => 0.02f,
            "Heavy" => 0.08f,
            "Torrential" => 0.18f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("rain", $"{1 - rainMod}"));
        return 1 - rainMod;
    }
    public float RainMod(Soldier shooter)
    {
        string rainfall = weather.CurrentRain;

        if (shooter.IsCalculator())
            rainfall = weather.DecreasedRain(rainfall);

        var rainMod = rainfall switch
        {
            "Zero" or "Light" => 0.0f,
            "Moderate" => 0.02f,
            "Heavy" => 0.08f,
            "Torrential" => 0.18f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("rain", $"{1 - rainMod}"));
        return 1 - rainMod;
    }
    public float WindMod(Soldier shooter, IAmShootable target)
    {
        Vector2 shotLine = new(target.X - shooter.X, target.Y - shooter.Y);
        shotLine.Normalize();
        Vector2 windLine = weather.CurrentWindDirectionVector;
        float shotAngleRelativeToWind = Vector2.Angle(shotLine, windLine);
        //print("WIND: " + windLine + " SHOT: " + shotLine + "ANGLE: " + shotAngleRelativeToWind);

        float windMod;

        string windSpeed = weather.CurrentWindSpeed;
        if (shooter.IsCalculator())
            windSpeed = weather.DecreasedWindspeed(windSpeed);
        if (target is Soldier targetSoldier)
            if (targetSoldier.IsCalculator())
                windSpeed = weather.IncreasedWindspeed(windSpeed);

        if (shotAngleRelativeToWind <= 22.5 || shotAngleRelativeToWind >= 157.5)
            windMod = 0f;
        else if (shotAngleRelativeToWind >= 67.5 && shotAngleRelativeToWind <= 112.5)
        {

            if (windSpeed == "Strong")
                windMod = 0.29f;
            else if (windSpeed == "Moderate")
                windMod = 0.12f;
            else if (windSpeed == "Light")
                windMod = 0.06f;
            else
                windMod = 0f;
        }
        else
        {
            if (windSpeed == "Strong")
                windMod = 0.10f;
            else if (windSpeed == "Moderate")
                windMod = 0.06f;
            else if (windSpeed == "Light")
                windMod = 0.02f;
            else
                windMod = 0f;
        }

        //report parameters
        shotParameters.Add(Tuple.Create("wind", $"{1 - windMod}"));
        return 1 - windMod;
    }
    public float ShooterHealthMod(Soldier shooter)
    {
        float shooterHealthMod;
        if (shooter.IsLastStand())
            shooterHealthMod = 0.6f;
        else if (shooter.hp <= shooter.stats.H.Val / 2)
            shooterHealthMod = 0.16f;
        else if (shooter.hp < shooter.stats.H.Val)
            shooterHealthMod = 0.06f;
        else
            shooterHealthMod = 0f;

        //report parameters
        shotParameters.Add(Tuple.Create("HP", $"{1 - shooterHealthMod}"));

        return 1 - shooterHealthMod;
    }
    public float TargetHealthMod(IAmShootable target)
    {
        float targetHealthMod = 0;
        if (target is Soldier targetSoldier)
        {

            if (targetSoldier.IsLastStand())
                targetHealthMod = -0.4f;
            else if (targetSoldier.hp <= targetSoldier.stats.H.Val / 2)
                targetHealthMod = -0.14f;
            else if (targetSoldier.hp < targetSoldier.stats.H.Val)
                targetHealthMod = -0.04f;
            else
                targetHealthMod = 0f;
        }
        //report parameters
        shotParameters.Add(Tuple.Create("tHP", $"{1 - targetHealthMod}"));
        return 1 - targetHealthMod;
    }
    public float ShooterTerrainMod(Soldier shooter)
    {
        float shooterTerrainMod;
        if (shooter.IsOnNativeTerrain())
            shooterTerrainMod = -0.04f;
        else if (shooter.IsOnOppositeTerrain())
            shooterTerrainMod = 0.12f;
        else
            shooterTerrainMod = 0.06f;

        //report parameters
        shotParameters.Add(Tuple.Create("Ter", $"{1 - shooterTerrainMod}"));

        return 1 - shooterTerrainMod;
    }
    public float TargetTerrainMod(IAmShootable target)
    {
        float targetTerrainMod = 0;
        if (target is Soldier targetSoldier)
        {
            if (targetSoldier.IsOnNativeTerrain())
                targetTerrainMod = 0.16f;
            else if (targetSoldier.IsOnOppositeTerrain())
                targetTerrainMod = -0.08f;
            else
                targetTerrainMod = -0.02f;
        }

        //report parameters
        shotParameters.Add(Tuple.Create("tTer", $"{1 - targetTerrainMod}"));
        return 1 - targetTerrainMod;
    }
    public float ElevationMod(Soldier shooter, IAmShootable target)
    {
        float elevationMod = (target.Z - shooter.Z) * 0.01f;

        //report parameters
        shotParameters.Add(Tuple.Create("elevation", $"{1 - elevationMod}"));
        return 1 - elevationMod;
    }
    public float KdMod(Soldier shooter)
    {
        float kdMod;
        int kd = shooter.GetKd();

        if (kd != 0)
            kdMod = -(2 * kd * 0.01f);
        else
            kdMod = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("kd", $"{1 - kdMod}"));

        return 1 - kdMod;
    }
    public float OverwatchMod(Soldier shooter)
    {
        float overwatchMod;
        if (shooter.IsOnOverwatch())
        {
            if (shooter.IsGuardsman())
                overwatchMod = 0.2f;
            else
                overwatchMod = 0.4f;
        }
        else
            overwatchMod = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("overwatch", $"{1 - overwatchMod}"));

        return 1 - overwatchMod;
    }
    public float FlankingMod(IAmShootable target)
    {
        float flankingMod = 0;
        if (target is Soldier targetSoldier)
        {
            if (!targetSoldier.IsTactician())
            {
                flankingMod = menu.flankersShotUI.transform.Find("FlankersPanel").childCount switch
                {
                    1 => -0.2f,
                    2 => -0.5f,
                    3 => -1.0f,
                    _ => 0f,
                };
            }
        }

        //report parameters
        shotParameters.Add(Tuple.Create("flank", $"{1 - flankingMod}"));
        return 1 - flankingMod;
    }
    public float StealthMod(Soldier shooter)
    {
        float stealthMod;
        if (shooter.IsHidden())
            stealthMod = -0.4f;
        else
            stealthMod = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("stealth", $"{1 - stealthMod}"));

        return 1 - stealthMod;
    }
    public int ShooterSuppressionMod(Soldier shooter)
    {
        int suppressionMod = shooter.GetSuppression();

        //report parameters
        shotParameters.Add(Tuple.Create("suppression", $"{suppressionMod}"));

        return suppressionMod;
    }
    public Tuple<int, int, int> CalculateHitPercentage(Soldier shooter, IAmShootable target, Item gun)
    {
        //destroy old shot parameters
        shotParameters.Clear();
        Tuple<int, int, int> chances;
        int suppressedHitChance, hitChance, critChance;

        if (target is Coverman || target is ExplosiveBarrel) //shooting at objects
        {
            //calculate base shot chance
            hitChance = Mathf.RoundToInt((WeaponHitChance(shooter, target, gun) + 10 * RelevantWeaponSkill(shooter, gun)) * VisMod(shooter) * RainMod(shooter, target) * WindMod(shooter, target) * ShooterHealthMod(shooter) * ShooterTerrainMod(shooter) * ElevationMod(shooter, target) * ShooterSmokeMod(shooter) * ShooterTabunMod(shooter)) + ShooterFightMod(shooter);

            //calculate critical shot chance
            critChance = Mathf.RoundToInt((Mathf.Pow(RelevantWeaponSkill(shooter, gun), 2) * (hitChance / 100.0f)));
        }
        else //shooting at soldier
        {
            //calculate base shot chance
            hitChance = Mathf.RoundToInt((WeaponHitChance(shooter, target, gun) + 10 * RelevantWeaponSkill(shooter, gun) - 12 * TargetEvasion(target)) * CoverMod() * VisMod(shooter) * RainMod(shooter, target) * WindMod(shooter, target) * ShooterHealthMod(shooter) * TargetHealthMod(target) * ShooterTerrainMod(shooter) * TargetTerrainMod(target) * ElevationMod(shooter, target) * KdMod(shooter) * OverwatchMod(shooter) * FlankingMod(target) * StealthMod(shooter) * ShooterSmokeMod(shooter) * ShooterTabunMod(shooter)) + ShooterFightMod(shooter);

            //calculate critical shot chance
            critChance = Mathf.RoundToInt((Mathf.Pow(RelevantWeaponSkill(shooter, gun), 2) * (hitChance / 100.0f)) - TargetEvasion(target));
        }

        //declare suppression shot chance
        suppressedHitChance = hitChance - ShooterSuppressionMod(shooter);

        //cap extremes
        if (suppressedHitChance < 0)
            suppressedHitChance = 0;
        else if (suppressedHitChance > 100)
            suppressedHitChance = 100;

        if (hitChance < 0)
            hitChance = 0;
        else if (hitChance > 100)
            hitChance = 100;

        if (critChance < 0)
            critChance = 0;
        else if (critChance > 100)
            critChance = 100;

        chances = Tuple.Create(hitChance, critChance, suppressedHitChance);

        return chances;
    }
    public void ConfirmShot(bool retry)
    {
        Soldier shooter = soldierManager.FindSoldierById(shotUI.shooterID.text);
        IAmShootable target = FindShootableById(shotUI.targetDropdown.captionText.text);
        Item gun = null;
        bool runSecondShot = false;
        string gunNames = menu.shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text;
        print(gunNames);
        if (gunNames.Contains("|"))
        {
            string[] guns = menu.shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text.Split('|');
            gun = shooter.GetEquippedGun(guns[0]);
            menu.shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text = guns[1];
            runSecondShot = true;
        }
        else
            gun = shooter.GetEquippedGun(gunNames);

        int.TryParse(shotUI.apCost.text, out int ap);
        int actingHitChance;
        bool resistSuppression;
        menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(false);
        menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(false);
        menu.shotResultUI.transform.Find("OptionPanel").Find("AvengerRetry").gameObject.SetActive(false);
        menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);

        tempShooterTarget = Tuple.Create(shooter, target);
        if (runSecondShot || retry) { }
        else
            activeSoldier.DeductAP(ap);

        menu.SetShotResolvedFlagTo(false);

        if (shotUI.shotTypeDropdown.value == 0) //standard shot
        {
            //play shot sfx
            soundManager.PlayShotResolution(gun);

            if (target is Soldier)
                FileUtility.WriteToReport($"{shooter.soldierName} shooting at {(target as Soldier).soldierName}");
            else
                FileUtility.WriteToReport($"{shooter.soldierName} shooting at {target.GetType()}");

            resistSuppression = shooter.SuppressionCheck();
            gun.SpendSingleAmmo();

            int randNum1 = HelperFunctions.RandomNumber(1, 100);
            int randNum2 = HelperFunctions.RandomNumber(1, 100);
            Tuple<int, int, int> chances;
            chances = CalculateHitPercentage(shooter, target, gun);

            //display shooter suppression indicator
            if (shooter.IsSuppressed())
            {
                menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(true);

                if (resistSuppression)
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Resisted Suppression</color>";
                    actingHitChance = chances.Item1;
                }
                else
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=orange>Suffered Suppression</color>";
                    actingHitChance = chances.Item3;
                }
            }
            else
                actingHitChance = chances.Item1;

            if (target is Coverman)
            {
                int coverDamage = CalculateRangeBracket(CalculateRange(shooter, target as PhysicalObject)) switch
                {
                    "Melee" or "CQB" => gun.cQBCovDamage,
                    "Short" => gun.shortCovDamage,
                    "Medium" => gun.medCovDamage,
                    "Long" or "Coriolis" => gun.longCovDamage,
                    _ => 0,
                };

                //show los check button
                menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                //standard shot hits cover
                if (randNum1 <= actingHitChance)
                {
                    //play cover destruction
                    soundManager.PlayCoverDestruction();

                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                    //critical shot hits cover
                    if (randNum2 <= chances.Item2)
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> COVER DESTROYED </color>";
                    else
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Cover hit (" + coverDamage + " damage)</color>";
                }
                else
                {
                    //play shot miss dialogue
                    soundManager.PlaySoldierShotMiss(shooter.soldierSpeciality);

                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {RandomShotScatterDistance()}cm {RandomShotScatterHorizontal()}, {RandomShotScatterDistance()}cm {RandomShotScatterVertical()}.\n\nDamage event ({gun.damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                }
            }
            else if (target is ExplosiveBarrel targetBarrel)
            {
                //show los check button
                menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                //standard shot hits barrel
                if (randNum1 <= actingHitChance)
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                    //critical shot hits barrel
                    if (randNum2 <= chances.Item2)
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Barrel Explodes (Crit)!</color>";
                    else
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Barrel Explodes!</color>";
                    targetBarrel.CheckExplosionBarrel(shooter);
                }
                else
                {
                    //play shot miss dialogue
                    soundManager.PlaySoldierShotMiss(shooter.soldierSpeciality);

                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {RandomShotScatterDistance()}cm {RandomShotScatterHorizontal()}, {RandomShotScatterDistance()}cm {RandomShotScatterVertical()}.\n\nDamage event ({gun.damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                }
            }
            else if (target is Soldier targetSoldier) //check if target is soldier
            {
                //standard shot hits
                if (randNum1 <= actingHitChance)
                {
                    Soldier originalTarget = targetSoldier;

                    //if target is engaged, and not engaged with the shooter
                    if (targetSoldier.IsMeleeEngaged() && !targetSoldier.IsMeleeEngagedWith(shooter)) 
                    {
                        //pick random target to hit in engagement
                        int randNum3 = HelperFunctions.RandomNumber(0, originalTarget.EngagedSoldiers.Count);
                        if (randNum3 > 0)
                        {
                            targetSoldier = originalTarget.EngagedSoldiers[randNum3 - 1];
                            menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Shot into melee aiming for {originalTarget.soldierName}, hit {targetSoldier.soldierName}.";
                        }
                        else
                            menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Shot into melee and hit intended target.";
                    }
                    else
                        menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                    //standard shot crit hits
                    if (randNum2 <= chances.Item2)
                    {
                        targetSoldier.TakeDamage(shooter, gun.critDamage, false, new() { "Critical", "Shot" });
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> CRITICAL SHOT </color>";

                        if (targetSoldier.IsSelf(originalTarget)) //only pay xp if you hit correct target 
                        {
                            //paying xp for hit
                            if (chances.Item1 >= 10)
                                menu.AddXpAlert(shooter, 8, $"Critical shot on {targetSoldier.soldierName}!", false);
                            else
                                menu.AddXpAlert(shooter, 10, $"Critical shot with a {chances.Item1}% chance on {targetSoldier.soldierName}!", false);
                        }
                    }
                    else
                    {
                        targetSoldier.TakeDamage(shooter, gun.damage, false, new() { "Shot" });
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Hit </color>";

                        if (targetSoldier.IsSelf(originalTarget)) //only pay xp if you hit correct target 
                        {
                            //paying xp for hit
                            if (chances.Item1 >= 10)
                                menu.AddXpAlert(shooter, 2, $"Shot hit on {targetSoldier.soldierName}.", false);
                            else
                                menu.AddXpAlert(shooter, 10, $"Shot hit with a {chances.Item1}% chance on {targetSoldier.soldierName}!", false);
                        }
                    }
                }
                else
                {
                    //play shot miss dialogue
                    soundManager.PlaySoldierShotMiss(shooter.soldierSpeciality);

                    //set sound flags after ally misses shot
                    foreach (Soldier s in AllSoldiers())
                    {
                        if (s.IsSameTeamAs(shooter))
                            soundManager.SetSoldierSelectionSoundFlagAfterAllyMissesShot(s);
                    }

                    //set sound flags after enemy misses shot
                    soundManager.SetSoldierSelectionSoundFlagAfterEnemyMissesShot(targetSoldier);

                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {RandomShotScatterDistance()}cm {RandomShotScatterHorizontal()}, {RandomShotScatterDistance()}cm {RandomShotScatterVertical()}.\n\nDamage event ({gun.damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                    //show los check button if shot misses
                    menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                    //show avenger retry if opponent has killed
                    if (shooter.IsAvenger() && targetSoldier.hasKilled && gun.CheckAnyAmmo() && !retry && !targetSoldier.IsRevoker())
                        menu.shotResultUI.transform.Find("OptionPanel").Find("AvengerRetry").gameObject.SetActive(true);

                    //paying xp for dodge
                    if (chances.Item1 <= 90)
                        menu.AddXpAlert(targetSoldier, 1, $"Dodged shot from {shooter.soldierName}.", false);
                    else
                        menu.AddXpAlert(targetSoldier, 10, $"Dodged shot with a {chances.Item1}% chance from {shooter.soldierName}!", false);

                    //push a zero damage attack through for abilities trigger
                    targetSoldier.TakeDamage(shooter, 0, true, new() { "Shot" });
                }
            }
        }
        else if (shotUI.shotTypeDropdown.value == 1) //supression shot
        {
            //play suppression sfx
            soundManager.PlaySuppressionResolution(gun);

            //play suppression dialogue
            soundManager.PlaySoldierSuppressEnemy(shooter.soldierSpeciality);

            FileUtility.WriteToReport($"{shooter.soldierName} suppressing {target}");

            gun.SpendSpecificAmmo(gun.suppressDrain, true);

            int suppressionValue = CalculateRangeBracket(CalculateRange(shooter, target as PhysicalObject)) switch
            {
                "Melee" or "CQB" => gun.cQBSupPen,
                "Short" => gun.shortSupPen,
                "Medium" => gun.medSupPen,
                "Long" => gun.longSupPen,
                "Coriolis" => gun.corSupPen,
                _ => 0,
            };
            (target as Soldier).SetSuppression(suppressionValue);
            menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"<color=green> Supressing ({suppressionValue})</color>";
            menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Suppressing {(target as Soldier).soldierName} until next round.";
        }

        //trigger loud action
        if (gun.HasSuppressorAttached()) //suppressor 
            shooter.PerformLoudAction(20);
        else
            shooter.PerformLoudAction();

        StartCoroutine(menu.OpenShotResultUI(runSecondShot));
        menu.CloseShotUI();
    }


















    //melee functions
    public void UpdateMeleeUI()
    {
        Soldier attacker = soldierManager.FindSoldierById(meleeUI.attackerID.text);
        Soldier defender = soldierManager.FindSoldierByName(meleeUI.targetDropdown.captionText.text);

        if (!menu.clearMeleeFlag)
        {
            UpdateMeleeAP(attacker);
            if (meleeUI.meleeTypeDropdown.value == 0) //If it's an actual attack
            {
                UpdateMeleeDefenderWeapon(defender);
                UpdateMeleeFlankingAgainstAttacker(attacker, defender);
                UpdateMeleeFlankingAgainstDefender(attacker, defender);
            }
        }
    }
    public void UpdateMeleeAP(Soldier attacker)
    {
        if (meleeUI.meleeTypeDropdown.options[0].text.Contains("Charge"))
            meleeUI.apCost.text = "0";
        else
        {
            if (attacker.IsBrawler())
                meleeUI.apCost.text = "1";
            else
                meleeUI.apCost.text = "2";
        }
    }
    public void UpdateMeleeDefenderWeapon(Soldier defender)
    {
        //show defender weapon
        if (defender.BestMeleeWeapon != null)
            meleeUI.defenderWeaponImage.sprite = defender.BestMeleeWeapon.itemImage;
        else
            meleeUI.defenderWeaponImage.sprite = menu.fist;
    }
    public void UpdateMeleeFlankingAgainstAttacker(Soldier attacker, Soldier defender)
    {
        //clear the flanker ui
        menu.ClearFlankersUI(meleeUI.flankersMeleeAttackerUI);
        int flankersCount = 0;
        foreach (Soldier s in AllSoldiers())
        {
            if (s.IsAbleToSee() && s.IsOppositeTeamAs(attacker) && !s.IsSelf(defender) && s.PhysicalObjectWithinMeleeRadius(attacker) && s.IsRevealing(attacker))
            {
                //add flanker to ui to visualise
                flankersCount++;
                GameObject flankerPortrait = Instantiate(menu.possibleFlankerPrefab, meleeUI.flankersMeleeAttackerUI.transform.Find("FlankersPanel"));
                flankerPortrait.GetComponentInChildren<SoldierPortrait>().Init(s);
            }
        }

        //display flankers if there are any
        if (flankersCount > 0)
            menu.OpenFlankersUI(meleeUI.flankersMeleeAttackerUI);
    }
    public void UpdateMeleeFlankingAgainstDefender(Soldier attacker, Soldier defender)
    {
        //clear the flanker ui
        menu.ClearFlankersUI(meleeUI.flankersMeleeDefenderUI);
        int flankersCount = 0;
        if (!defender.IsTactician() || attacker.IsRevoker())
        {
            foreach (Soldier s in AllSoldiers())
            {
                if (s.IsAbleToSee() && s.IsOppositeTeamAs(defender) && !s.IsSelf(attacker) && s.PhysicalObjectWithinMeleeRadius(defender) && s.IsRevealing(defender) && flankersCount < 3)
                {
                    flankersCount++;

                    //add flanker to ui to visualise
                    GameObject flankerPortrait = Instantiate(menu.possibleFlankerPrefab, meleeUI.flankersMeleeDefenderUI.transform.Find("FlankersPanel"));
                    flankerPortrait.GetComponentInChildren<SoldierPortrait>().Init(s);
                }
            }

            //display flankers if there are any
            if (flankersCount > 0)
                menu.OpenFlankersUI(meleeUI.flankersMeleeDefenderUI);
        }
    }
    public void UpdateMeleeTypeOptions()
    {
        Soldier attacker = soldierManager.FindSoldierById(meleeUI.attackerID.text);
        Soldier defender = soldierManager.FindSoldierByName(meleeUI.targetDropdown.captionText.text);

        List<TMP_Dropdown.OptionData> meleeTypeDetails = new()
        {
            new TMP_Dropdown.OptionData(menu.meleeChargeIndicator),
            new TMP_Dropdown.OptionData("Engagement Only"),
        };

        if (defender.controlledBySoldiersList.Contains(attacker.id))
            meleeTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Disengage</color>"));
        else if (defender.controllingSoldiersList.Contains(attacker.id))
            meleeTypeDetails.Add(new TMP_Dropdown.OptionData("<color=red>Request Disengage</color>"));

        meleeUI.meleeTypeDropdown.ClearOptions();
        meleeUI.meleeTypeDropdown.AddOptions(meleeTypeDetails);
    }
    public float AttackerMeleeSkill(Soldier attacker)
    {
        int juggernautBonus = 0;
        float inspirerBonus, attackerMeleeSkill = attacker.stats.M.Val;

        //apply JA debuff
        if (attacker.IsWearingJuggernautArmour(false))
            juggernautBonus = -1;
        attackerMeleeSkill += juggernautBonus;

        //check for inspirer
        inspirerBonus = attacker.InspirerBonusWeaponMelee();
        attackerMeleeSkill += inspirerBonus;

        //apply sustenance debuff
        attackerMeleeSkill *= AttackerSustenanceMod(attacker);

        //correct negatives
        if (attackerMeleeSkill < 0)
            attackerMeleeSkill = 0;

        meleeParameters.Add(Tuple.Create("aM", $"{attacker.stats.M.Val}"));
        meleeParameters.Add(Tuple.Create("aJuggernaut", $"{juggernautBonus}"));
        meleeParameters.Add(Tuple.Create("aInspirer", $"{inspirerBonus}"));
        return attackerMeleeSkill;
    }
    public float AttackerSustenanceMod(Soldier attacker)
    {
        float sustenanceMod = 0;

        if (attacker.roundsWithoutFood >= 20)
            sustenanceMod = 0.5f;

        meleeParameters.Add(Tuple.Create("aSustenance", $"{1 - sustenanceMod}"));
        return 1 - sustenanceMod;
    }
    public int AttackerWeaponDamage(Item weapon)
    {
        int attackerWeaponDamage;

        //if fist selected
        if (weapon == null)
            attackerWeaponDamage = 1;
        else
            attackerWeaponDamage = weapon.meleeDamage;

        meleeParameters.Add(Tuple.Create("aWep", $"{attackerWeaponDamage}"));
        return attackerWeaponDamage;
    }
    public float AttackerHealthMod(Soldier attacker)
    {
        float attackerHealthMod;
        if (attacker.IsLastStand())
            attackerHealthMod = 0.6f;
        else if (attacker.hp <= attacker.stats.H.Val / 2)
            attackerHealthMod = 0.20f;
        else if (attacker.hp < attacker.stats.H.Val)
            attackerHealthMod = 0.06f;
        else
            attackerHealthMod = 0f;

        meleeParameters.Add(Tuple.Create("aHP", $"{1 - attackerHealthMod}"));
        return 1 - attackerHealthMod;
    }
    public float AttackerTerrainMod(Soldier attacker)
    {
        float attackerTerrainMod;
        if (attacker.IsOnNativeTerrain())
            attackerTerrainMod = -0.4f;
        else if (attacker.IsOnOppositeTerrain())
            attackerTerrainMod = 0.2f;
        else
            attackerTerrainMod = 0f;

        meleeParameters.Add(Tuple.Create("aTer", $"{1 - attackerTerrainMod}"));
        return 1 - attackerTerrainMod;
    }
    public float FlankingAgainstAttackerMod()
    {
        int flankersCount = 0;
        foreach (Transform child in meleeUI.flankersMeleeAttackerUI.transform.Find("FlankersPanel"))
            if (child.GetComponentInChildren<Toggle>().isOn)
                flankersCount++;

        float attackerFlankingMod = flankersCount switch
        {
            0 => 0f,
            1 => 0.16f,
            2 => 0.46f,
            3 or _ => 0.8f
        };

        meleeParameters.Add(Tuple.Create("aFlank", $"{1 - attackerFlankingMod}"));
        return 1 - attackerFlankingMod;
    }
    public float AttackerStrengthMod(Soldier attacker)
    {
        float strengthMod = attacker.stats.Str.Val;
        strengthMod *= 0.2f;

        meleeParameters.Add(Tuple.Create("aStr", $"{attacker.stats.Str.Val}"));
        return strengthMod;
    }
    public float DefenderMeleeSkill(Soldier defender)
    {
        int juggernautBonus = 0;
        float defenderMeleeSkill = defender.stats.M.Val;

        //apply JA debuff
        if (defender.IsWearingJuggernautArmour(false))
            juggernautBonus = -1;
        defenderMeleeSkill += juggernautBonus;

        //apply sustenance debuff
        defenderMeleeSkill *= DefenderSustenanceMod(defender);

        //correct negatives
        if (defenderMeleeSkill < 0)
            defenderMeleeSkill = 0;

        meleeParameters.Add(Tuple.Create("dM", $"{defender.stats.M.Val}"));
        meleeParameters.Add(Tuple.Create("dJuggernaut", $"{juggernautBonus}"));
        return defenderMeleeSkill;
    }
    public float DefenderSustenanceMod(Soldier defender)
    {
        float sustenanceMod = 0;

        if (defender.roundsWithoutFood >= 20)
            sustenanceMod = 0.5f;

        meleeParameters.Add(Tuple.Create("dSustenance", $"{1 - sustenanceMod}"));
        return 1 - sustenanceMod;
    }
    public int DefenderWeaponDamage(Item weapon)
    {
        int defenderWeaponDamage;

        //if fist selected
        if (weapon == null)
            defenderWeaponDamage = 1;
        else
            defenderWeaponDamage = weapon.meleeDamage;

        meleeParameters.Add(Tuple.Create("dWep", $"{defenderWeaponDamage}"));
        return defenderWeaponDamage;
    }
    public float ChargeModifier()
    {
        float chargeMod;

        chargeMod = meleeUI.meleeTypeDropdown.captionText.text switch
        {
            "Full Charge Attack" => 1.9f,
            "Half Charge Attack" => 1.4f,
            "3cm Charge Attack" => 1.1f,
            "Static Attack" or _ => 0f,
        };

        meleeParameters.Add(Tuple.Create("charge", $"{chargeMod}"));
        return chargeMod;
    }
    public float DefenderHealthMod(Soldier defender)
    {
        float defenderHealthMod;
        if (defender.IsLastStand())
            defenderHealthMod = 0.8f;
        else if (defender.hp <= activeSoldier.stats.H.Val / 2)
            defenderHealthMod = 0.4f;
        else if (defender.hp < activeSoldier.stats.H.Val)
            defenderHealthMod = 0.2f;
        else
            defenderHealthMod = 0f;

        meleeParameters.Add(Tuple.Create("dHP", $"{1 - defenderHealthMod}"));
        return 1 - defenderHealthMod;
    }
    public float DefenderTerrainMod(Soldier defender)
    {
        float defenderTerrainMod;
        if (defender.IsOnNativeTerrain())
            defenderTerrainMod = -0.4f;
        else if (defender.IsOnOppositeTerrain())
            defenderTerrainMod = 0.4f;
        else
            defenderTerrainMod = 0f;

        meleeParameters.Add(Tuple.Create("dTer", $"{1 - defenderTerrainMod}"));
        return 1 - defenderTerrainMod;
    }
    public float FlankingAgainstDefenderMod(Soldier defender)
    {
        float defenderFlankingMod = 0;
        if (!defender.IsTactician())
        {
            int flankersCount = 0;
            foreach (Transform child in meleeUI.flankersMeleeDefenderUI.transform.Find("FlankersPanel"))
                if (child.GetComponentInChildren<Toggle>().isOn)
                    flankersCount++;

            defenderFlankingMod = flankersCount switch
            {
                0 => 0f,
                1 => 0.26f,
                2 => 0.56f,
                3 or _ => 0.86f
            };
        }

        meleeParameters.Add(Tuple.Create("dFlank", $"{1 - defenderFlankingMod}"));
        return 1 - defenderFlankingMod;
    }
    public float DefenderStrengthMod(Soldier defender)
    {
        float strengthMod = defender.stats.Str.Val;
        strengthMod *= 0.2f;

        meleeParameters.Add(Tuple.Create("dStr", $"{defender.stats.Str.Val}"));
        return strengthMod;
    }
    public float AttackerSuppressionMod(Soldier soldier)
    {
        float suppressionMod = soldier.GetSuppression() / 100f;

        meleeParameters.Add(Tuple.Create("aSuppression", $"{1 - suppressionMod}"));
        return 1 - suppressionMod;
    }
    public float DefenderSuppressionMod(Soldier soldier)
    {
        float suppressionMod = soldier.GetSuppression() / 100f;

        meleeParameters.Add(Tuple.Create("dSuppression", $"{1 - suppressionMod}"));
        return 1 - suppressionMod;
    }
    public float AttackerFightMod(Soldier soldier)
    {
        float fightMod = 0;

        if (soldier.FightActive())
            fightMod += 0.5f * soldier.stats.F.Val;
        else if (soldier.AvengingActive()) //avenger ability
            fightMod += 0.5f * (soldier.stats.F.Val - 1);

        meleeParameters.Add(Tuple.Create("aFight", $"{fightMod}"));
        return fightMod;
    }
    public float DefenderFightMod(Soldier soldier)
    {
        float fightMod = 0;

        if (soldier.FightActive())
            fightMod += 0.5f * soldier.stats.F.Val;
        else if (soldier.AvengingActive()) //avenger ability
            fightMod += 0.5f * (soldier.stats.F.Val - 1);

        meleeParameters.Add(Tuple.Create("dFight", $"{fightMod}"));
        return fightMod;
    }
    public int CalculateMeleeResult(Soldier attacker, Soldier defender)
    {
        //destroy old melee parameters
        meleeParameters.Clear();

        float meleeDamage;
        int meleeDamageFinal;
        Item attackerWeapon = attacker.BestMeleeWeapon;
        Item defenderWeapon = defender.BestMeleeWeapon;
        int bloodrageMultiplier = 1;

        //if it's a normal attack
        if (meleeUI.meleeTypeDropdown.value == 0)
        {
            meleeDamage = (AttackerMeleeSkill(attacker) + AttackerWeaponDamage(attackerWeapon)) * AttackerHealthMod(attacker) * AttackerTerrainMod(attacker) * KdMod(attacker) * FlankingAgainstAttackerMod() * AttackerSuppressionMod(attacker) + AttackerStrengthMod(attacker) - ((DefenderMeleeSkill(defender) + DefenderWeaponDamage(defenderWeapon) + ChargeModifier()) * DefenderHealthMod(defender) * DefenderTerrainMod(defender) * FlankingAgainstDefenderMod(defender) * DefenderSuppressionMod(defender) + DefenderStrengthMod(defender)) - DefenderFightMod(defender) + AttackerFightMod(attacker);

            print($"attacker: {(AttackerMeleeSkill(attacker) + AttackerWeaponDamage(attackerWeapon)) * AttackerHealthMod(attacker) * AttackerTerrainMod(attacker) * KdMod(attacker) * FlankingAgainstAttackerMod() * AttackerSuppressionMod(attacker) + AttackerStrengthMod(attacker)}");

            print($"defender: {((DefenderMeleeSkill(defender) + DefenderWeaponDamage(defenderWeapon) + ChargeModifier()) * DefenderHealthMod(defender) * DefenderTerrainMod(defender) * FlankingAgainstDefenderMod(defender) * DefenderSuppressionMod(defender) + DefenderStrengthMod(defender))}");

            //check bloodletter damage bonus
            if (meleeDamage > 0 && attacker.IsBloodRaged() && !defender.IsRevoker())
                bloodrageMultiplier = 2;

            meleeParameters.Add(Tuple.Create("bloodrage", $"{bloodrageMultiplier}"));
            meleeDamage *= bloodrageMultiplier;


            //rounding based on R
            if (attacker.stats.R.Val > defender.stats.R.Val)
            {
                meleeParameters.Add(Tuple.Create("rounding", "Attacker favoured."));
                meleeDamageFinal = Mathf.CeilToInt(meleeDamage);
            }
            else if (attacker.stats.R.Val < defender.stats.R.Val)
            {
                meleeParameters.Add(Tuple.Create("rounding", "Defender favoured."));
                meleeDamageFinal = Mathf.FloorToInt(meleeDamage);
            }
            else
            {
                meleeParameters.Add(Tuple.Create("rounding", "Neither favoured."));
                meleeDamageFinal = Mathf.RoundToInt(meleeDamage);
            }
        }
        else
        {
            meleeParameters.Add(Tuple.Create("rounding", "N/A"));
            meleeDamageFinal = 0;
        }

        return meleeDamageFinal;
    }
    public void EstablishController(Soldier controller, Soldier s2)
    {
        BreakMeleeEngagement(controller, s2);
        controller.controllingSoldiersList.Add(s2.id);
        controller.controlledBySoldiersList.Remove(s2.id);
        s2.controlledBySoldiersList.Add(controller.id);
        s2.controllingSoldiersList.Remove(controller.id);

        StartCoroutine(menu.OpenMeleeResultUI());
    }
    public void EstablishNoController(Soldier s1, Soldier s2)
    {
        BreakMeleeEngagement(s1, s2);

        StartCoroutine(menu.OpenMeleeResultUI());
    }
    public string DetermineMeleeController(Soldier attacker, Soldier defender, bool counterattack, bool disengage)
    {
        string controlResult;

        if (defender.IsDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=red>No-one Controlling\n(" + defender.soldierName + " Dead)</color>";
        }
        else if (attacker.IsDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=red>No-one Controlling\n(" + attacker.soldierName + " Dead)</color>";
        }
        else if (defender.IsUnconscious())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=blue>No-one Controlling\n(" + defender.soldierName + " Unconscious)</color>";
        }
        else if (attacker.IsUnconscious())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=blue>No-one Controlling\n(" + attacker.soldierName + " Unconscious)</color>";
        }
        else if (defender.IsPlayingDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + defender.soldierName + " Playdead)</color>";
        }
        else if (attacker.IsPlayingDead())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + attacker.soldierName + " Playdead)</color>";
        }
        else if (defender.IsBroken())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + defender.soldierName + " Broken)</color>";
        }
        else if (counterattack)
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + defender.soldierName + " Counterattacked)</color>";
        }
        else if (disengage)
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=orange>No-one Controlling\n(" + attacker.soldierName + " Disengaged)</color>";
        }
        else
        {
            if (attacker.stats.R.Val > defender.stats.R.Val)
            {
                EstablishController(attacker, defender);
                controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher R)</color>";
            }
            else if (attacker.stats.R.Val < defender.stats.R.Val)
            {
                EstablishController(defender, attacker);
                controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher R)</color>";
            }
            else
            {
                if (attacker.stats.Str.Val > defender.stats.Str.Val)
                {
                    EstablishController(attacker, defender);
                    controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher Str)</color>";
                }
                else if (attacker.stats.Str.Val < defender.stats.Str.Val)
                {
                    EstablishController(defender, attacker);
                    controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher Str)</color>";
                }
                else
                {
                    if (attacker.stats.M.Val > defender.stats.M.Val)
                    {
                        EstablishController(attacker, defender);
                        controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher M)</color>";
                    }
                    else if (attacker.stats.M.Val < defender.stats.M.Val)
                    {
                        EstablishController(defender, attacker);
                        controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher M)</color>";
                    }
                    else
                    {
                        if (attacker.stats.F.Val > defender.stats.F.Val)
                        {
                            EstablishController(attacker, defender);
                            controlResult = "<color=green>" + attacker.soldierName + " Controlling\n(Higher F)</color>";
                        }
                        else if (attacker.stats.F.Val < defender.stats.F.Val)
                        {
                            EstablishController(defender, attacker);
                            controlResult = "<color=red>" + defender.soldierName + " Controlling\n(Higher )</color>";
                        }
                        else
                        {
                            EstablishNoController(attacker, defender);
                            controlResult = "<color=orange>No-one Controlling\n(Evenly Matched)</color>";
                        }
                    }
                }
            }
        }
        return controlResult;
    }
    public void BreakAllControllingMeleeEngagments(Soldier s1)
    {
        List<string> engagedSoldiersList = new();

        foreach (string soldierId in s1.controllingSoldiersList)
            engagedSoldiersList.Add(soldierId);

        foreach (string soldierId in engagedSoldiersList)
            BreakMeleeEngagement(s1, soldierManager.FindSoldierById(soldierId));
    }
    public void BreakMeleeEngagement(Soldier s1, Soldier s2)
    {
        s1.controllingSoldiersList.Remove(s2.id);
        s1.controlledBySoldiersList.Remove(s2.id);
        s2.controllingSoldiersList.Remove(s1.id);
        s2.controlledBySoldiersList.Remove(s1.id);
    }
    public void ConfirmMelee()
    {
        Soldier attacker = soldierManager.FindSoldierById(meleeUI.attackerID.text);
        Soldier defender = soldierManager.FindSoldierByName(meleeUI.targetDropdown.captionText.text);

        if (int.TryParse(meleeUI.apCost.text, out int ap))
        {
            if (activeSoldier.CheckAP(ap))
            {
                FileUtility.WriteToReport($"{attacker.soldierName} starting melee attack on {defender.soldierName}");

                //determine if damage is from melee or melee charge
                List<string> damageType = new() { "Melee" };
                if (ap == 0)
                    damageType.Add("Charge");

                menu.SetMeleeResolvedFlagTo(false);
                activeSoldier.DeductAP(ap);

                int meleeDamage = CalculateMeleeResult(attacker, defender);
                string damageMessage;
                bool counterattack = false, instantKill = false, loudAction = true, disengage = false;

                //engagement only options
                if (meleeUI.meleeTypeDropdown.value == 1)
                {
                    damageMessage = "<color=orange>No Damage\n(Enagament Only)</color>";
                    //loudAction = false;
                }
                else if (meleeUI.meleeTypeDropdown.value == 2)
                {
                    damageMessage = "<color=orange>No Damage\n(Disengagement)</color>";
                    disengage = true;
                    //loudAction = false;
                }
                else
                {
                    //instant kill scenarios
                    if (attacker.IsHidden() && attacker.stats.M.Val > defender.stats.M.Val) //stealth kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Stealth Attack)</color>";
                        instantKill = true;
                        loudAction = false;
                    }
                    else if (defender.IsOnOverwatch() && attacker.stats.M.Val > defender.stats.M.Val) //overwatcher kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Overwatcher)</color>";
                        instantKill = true;
                        loudAction = false;
                    }
                    else if (defender.IsUnconscious()) //unconscious kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Unconscious)</color>";
                        instantKill = true;
                        loudAction = false;
                    }
                    else if (defender.IsPlayingDead()) //playdead kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Playdead)</color>";
                        instantKill = true;
                        loudAction = false;
                    }
                    else
                    {
                        //drop impractical handheld items
                        attacker.DropWeakerHandheldItem();
                        defender.DropWeakerHandheldItem();

                        if (meleeDamage > 0)
                        {
                            //play melee success sfx
                            if (damageType.Contains("Charge"))
                                soundManager.PlayMeleeResolution("successCharge"); //play melee success charge sfx
                            else
                                soundManager.PlayMeleeResolution("successStatic"); //play melee success static sfx

                            if (attacker.IsWearingExoArmour() && !defender.IsWearingJuggernautArmour(false)) //exo kill on standard man
                            {
                                damageMessage = "<color=green>INSTANT KILL\n(Exo Armour)</color>";
                                instantKill = true;
                            }
                            else
                            {
                                if (defender.IsWearingJuggernautArmour(false) && !attacker.IsWearingExoArmour())
                                    damageMessage = "<color=orange>No Damage\n(Juggernaut Immune)</color>";
                                else
                                    damageMessage = "<color=green>Successful Attack\n(" + meleeDamage + " Damage)</color>";
                                defender.TakeDamage(attacker, meleeDamage, false, damageType);
                                attacker.BrawlerMeleeHitReward();
                            }
                        }
                        else if (meleeDamage < 0)
                        {
                            //play melee counterattack sfx
                            soundManager.PlayMeleeResolution("counter");

                            if (!defender.IsWearingExoArmour() && attacker.IsWearingJuggernautArmour(false)) //no damage counter against jugs
                                damageMessage = "<color=orange>No Damage\n(Juggernaut Immune)</color>";
                            else
                            {
                                counterattack = true;
                                meleeDamage *= -1;
                                damageMessage = "<color=red>Counterattacked\n(" + meleeDamage + " Damage)</color>";
                                attacker.TakeDamage(defender, meleeDamage, false, damageType);
                            }
                        }
                        else
                        {
                            //play melee breakeven sfx
                            soundManager.PlayMeleeResolution("breakeven");
                            //play melee breakeven dialogue
                            soundManager.PlaySoldierMeleeBreakeven(activeSoldier.soldierSpeciality);

                            damageMessage = "<color=orange>No Damage\n(Evenly Matched)</color>";
                        }
                    }

                    //reset bloodrage even if non-successful attack
                    attacker.UnsetBloodRage();

                    //push a zero damage attack to the defender for abilities trigger
                    defender.TakeDamage(attacker, 0, true, damageType);
                }

                //add xp for successful melee attack
                if (meleeUI.meleeTypeDropdown.value == 0)
                {
                    if (!damageMessage.Contains("No Damage"))
                    {
                        if (counterattack)
                            menu.AddXpAlert(defender, 2 + meleeDamage, $"Melee counterattack attack on {attacker.soldierName} for {meleeDamage} damage.", false);
                        else
                            menu.AddXpAlert(attacker, 1, $"Successful melee attack on {defender.soldierName}.", false);
                    }
                    else
                    {
                        if (meleeDamage == 0)
                            menu.AddXpAlert(defender, 2, $"Melee block against {attacker.soldierName}.", false);
                    }
                }

                //kill if instantKill
                if (instantKill)
                    defender.InstantKill(attacker, new List<string>() { "Melee" });

                //attacker and defender exit cover
                attacker.UnsetCover();
                defender.UnsetCover();

                //attacker and defender exit overwatch
                attacker.UnsetOverwatch();
                defender.UnsetOverwatch();

                //add melee alert
                menu.AddMeleeAlert(attacker, defender, damageMessage, DetermineMeleeController(attacker, defender, counterattack, disengage));

                //trigger loud action
                if (loudAction)
                    attacker.PerformLoudAction();

                StartCoroutine(menu.OpenMeleeResultUI());
                menu.CloseMeleeUI();
            }
        }
    }
    public IEnumerator DetermineMeleeControllerMultiple(Soldier s1)
    {
        if (s1.EngagedSoldiers.Count > 0)
        {
            menu.SetMeleeResolvedFlagTo(false);
            yield return new WaitUntil(() => menu.MovementResolvedFlag() && menu.detectionResolvedFlag);
            foreach (Soldier s in s1.EngagedSoldiers)
                menu.AddMeleeAlert(s1, s, "No Damage\n(Engagement Change)", DetermineMeleeController(s1, s, false, false));
        }
    }














    //configure functions
    public int UpdateConfigureAP()
    {
        int totalDrop = 0, totalPickup = 0, totalSwap = 0, ap = 0;

        foreach (ItemIcon itemIcon in menu.configUI.GetComponentsInChildren<ItemIcon>(true))
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
        if (activeSoldier.IsAdept() && ap > 1)
            ap -= 1;

        //configure free on first soldier turn is they haven't used ap
        if (activeSoldier.roundsFielded == 0 && !activeSoldier.usedAP)
            ap = 0;

        configUI.apCost.text = ap.ToString();
        return ap;
    }
    public List<Item> FindNearbyItems()
    {
        List<Item> nearbyItems = new();
        foreach (Item i in FindObjectsByType<Item>(default))
            if (activeSoldier.PhysicalObjectWithinItemRadius(i))
                nearbyItems.Add(i);

        return nearbyItems;
    }
    public void ConfirmConfigure()
    {
        int ap = UpdateConfigureAP();
        if (activeSoldier.CheckAP(ap))
        {
            activeSoldier.DeductAP(ap);
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
            activeSoldier.PerformLoudAction(5);
            menu.CloseConfigureUI();
        }
    }
    //disarm function
    public void ConfirmDisarm()
    {
        if (activeSoldier.CheckAP(1))
        {
            activeSoldier.DeductAP(1);

            POI poiToDisarm = poiManager.FindPOIById(menu.disarmUI.transform.Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().captionText.text);
            Item disarmedItem = null;
            Soldier placedBy = null;

            if (poiToDisarm is Claymore claymoreToDisarm)
            {
                disarmedItem = itemManager.SpawnItem("Claymore");
                placedBy = claymoreToDisarm.placedBy;
            }
            else if (poiToDisarm is DeploymentBeacon depbeaconToDisarm)
            {
                disarmedItem = itemManager.SpawnItem("Deployment_Beacon");
                placedBy = depbeaconToDisarm.placedBy;
            }
            else if (poiToDisarm is ThermalCamera thermalcamToDisarm)
            {
                SetLosCheckAll("losChange|thermalCamDeactive"); //loscheckall
                disarmedItem = itemManager.SpawnItem("Thermal_Camera");
                placedBy = thermalcamToDisarm.placedBy;
            }

            //xp for disarming enemy objects
            if (placedBy != null && activeSoldier.IsOppositeTeamAs(placedBy))
                menu.AddXpAlert(activeSoldier, 2, "Disarmed enemy device.", true);

            //set item to same position as poi and destroy poi
            disarmedItem.X = poiToDisarm.X;
            disarmedItem.Y = poiToDisarm.Y;
            disarmedItem.Z = poiToDisarm.Z;
            menu.poiManager.DestroyPOI(poiToDisarm);
            
            activeSoldier.PerformLoudAction(6);
            menu.CloseDisarmUI();
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

        FileUtility.WriteToReport($"{activeSoldier.soldierName} used {itemUsed.itemName}.");

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
                menu.OpenGrenadeUI(useItemUI);
                break;
            case "Binoculars":
                menu.OpenBinocularsUI(useItemUI);
                break;
            case "Claymore":
                menu.OpenClaymoreUI(useItemUI);
                break;
            case "Deployment_Beacon":
                menu.OpenDeploymentBeaconUI(useItemUI);
                break;
            case "Riot_Shield":
                menu.OpenRiotShieldUI(useItemUI);
                break;
            case "Thermal_Camera":
                menu.OpenThermalCamUI(useItemUI);
                break;
            case "UHF_Radio":
                menu.OpenUHFUI(useItemUI);
                break;
            default:
                break;

        }
        activeSoldier.DeductAP(ap);
        menu.CloseUseItemUI();
    }
    public void ConfirmDropThrowItem(UseItemUI useItemUI)
    {
        int.TryParse(useItemUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap);
        if (useItemUI.transform.Find("OptionPanel").Find("Message").Find("Text").GetComponent<TextMeshProUGUI>().text.Contains("Throw"))
            menu.OpenThrowUI(useItemUI);
        else
            menu.OpenDropUI(useItemUI);

        activeSoldier.DeductAP(ap);
        menu.CloseDropThrowItemUI();
    }
    public void UpdateSoldierUsedOn(UseItemUI useItemUI)
    {
        useItemUI.soldierUsedOn = soldierManager.FindSoldierById(menu.useItemUI.transform.Find("OptionPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().captionText.text);
    }
    public void UpdateItemUsedOn(UseItemUI useItemUI)
    {
        useItemUI.itemUsedOn = itemManager.FindItemById(menu.useItemUI.transform.Find("OptionPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().captionText.text);
    }
    public void ConfirmUHF(UseItemUI useUHFUI)
    {
        TMP_InputField targetX = useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField targetY = useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("YPos").GetComponent<TMP_InputField>();
        TMP_InputField targetZ = useUHFUI.transform.Find("OptionPanel").Find("UHFTarget").Find("ZPos").GetComponent<TMP_InputField>();
        GameObject totalMiss = useUHFUI.transform.Find("OptionPanel").Find("TotalMiss").gameObject;
        TMP_Dropdown strikeOption = useUHFUI.transform.Find("OptionPanel").Find("StrikeOptions").Find("StrikeOptionsDropdown").GetComponent<TMP_Dropdown>();

        int dipelecScore = itemManager.scoreTable[activeSoldier.stats.Dip.Val, activeSoldier.stats.Elec.Val];
        Tuple<int, string, int, int, int> strike = itemManager.GetStrike(strikeOption.captionText.text);
        int rolls = strike.Item4;
        int radius = strike.Item3;
        int damage = strike.Item5;

        if (!useUHFUI.transform.Find("PressedOnce").gameObject.activeInHierarchy) //first press
        {

            if (menu.ValidateIntInput(targetX, out int x) && menu.ValidateIntInput(targetY, out int y))
            {
                int highestRoll = 0, newX, newY;
                float scatterDistance;
                int scatterDegree = HelperFunctions.RandomNumber(0, 360);
                for (int i = 0; i < rolls; i++)
                {
                    int roll = DiceRoll();
                    if (roll > highestRoll)
                        highestRoll = roll;
                }

                //play uhf result dialogue
                soundManager.PlayUHFResult(highestRoll);

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
                    (newX, newY) = CalculateScatteredCoordinates(x, y, scatterDegree, scatterDistance);

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
                menu.CloseUHFUI();
                useUHFUI.itemUsed.UseItem(useUHFUI.itemUsedIcon, useUHFUI.itemUsedOn, useUHFUI.soldierUsedOn);
            }
            else
            {
                if (menu.ValidateIntInput(targetX, out int x) && menu.ValidateIntInput(targetY, out int y) && menu.ValidateIntInput(targetZ, out int z))
                {
                    menu.CloseUHFUI();
                    useUHFUI.itemUsed.UseItem(useUHFUI.itemUsedIcon, useUHFUI.itemUsedOn, useUHFUI.soldierUsedOn);
                    CheckExplosionUHF(activeSoldier, new Vector3(x, y, z), radius, damage);
                }
            }

            //perform loud action
            activeSoldier.PerformLoudAction(14);

            //set sound flags after enemy use UHF
            foreach (Soldier s in AllSoldiers())
            {
                if (s.IsSameTeamAs(activeSoldier))
                    soundManager.SetSoldierSelectionSoundFlagAfterAllyUseUHF(s);
                else
                    soundManager.SetSoldierSelectionSoundFlagAfterEnemyUseUHF(s);
            }
        }
    }
    public void ConfirmULF(UseItemUI useULFUI)
    {
        if (activeSoldier.CheckAP(3))
        {
            activeSoldier.DeductAP(3);
            useULFUI.itemUsed.UseULF(useULFUI.itemUsedFromSlotName);
            menu.CloseUseULFUI();
        }
    }
    public void ConfirmRiotShield(UseItemUI useRiotShield)
    {
        TMP_InputField targetX = useRiotShield.transform.Find("OptionPanel").Find("RiotShieldTarget").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField targetY = useRiotShield.transform.Find("OptionPanel").Find("RiotShieldTarget").Find("YPos").GetComponent<TMP_InputField>();

        if (menu.ValidateIntInput(targetX, out int x) && menu.ValidateIntInput(targetY, out int y))
        {
            //set riot shield facing
            activeSoldier.riotXPoint = x;
            activeSoldier.riotYPoint = y;

            menu.CloseRiotShieldUI();
            useRiotShield.itemUsed.UseItem(useRiotShield.itemUsedIcon, useRiotShield.itemUsedOn, useRiotShield.soldierUsedOn);
        }
    }
    public void ConfirmGrenade(UseItemUI useGrenade)
    {
        string grenadeName = useGrenade.transform.Find("OptionPanel").Find("GrenadeType").Find("Text").GetComponent<TextMeshProUGUI>().text;
        ValidThrowChecker throwTarget = useGrenade.transform.Find("OptionPanel").Find("Target").GetComponent<ValidThrowChecker>();
        GameObject throwBeyondRadius = useGrenade.transform.Find("OptionPanel").Find("Target").Find("ThrowBeyondRadius").gameObject;
        GameObject throwBeyondBlindRadius = useGrenade.transform.Find("OptionPanel").Find("Target").Find("ThrowBeyondBlindRadius").gameObject;
        GameObject scatteredOffMap = useGrenade.transform.Find("OptionPanel").Find("ScatteredOffMap").gameObject;

        if (!throwTarget.pressedOnce.activeInHierarchy) //first press
        {
            if (menu.ValidateIntInput(throwTarget.XPos, out int x) && menu.ValidateIntInput(throwTarget.YPos, out int y) && menu.ValidateIntInput(throwTarget.ZPos, out int z) && !throwBeyondRadius.activeInHierarchy && !throwBeyondBlindRadius.activeInHierarchy)
            {
                int newX, newY;
                throwTarget.GetThrowLocation(out Vector3 throwLocation);
                int throwDistance = Mathf.RoundToInt(Vector3.Distance(new(activeSoldier.X, activeSoldier.Y, activeSoldier.Z), throwLocation));
                int scatterDegree = HelperFunctions.RandomNumber(0, 360);
                int scatterDistance = activeSoldier.StrengthCheck() switch
                {
                    false => Mathf.CeilToInt(DiceRoll() * activeSoldier.stats.Str.Val / 2.0f),
                    _ => -1,
                };

                if (scatterDistance == -1 || throwDistance <= 3)
                    useGrenade.transform.Find("OptionPanel").Find("Target").Find("PreciseThrow").gameObject.SetActive(true);
                else
                {
                    (newX, newY) = CalculateScatteredCoordinates(x, y, scatterDegree, scatterDistance);

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
                activeSoldier.Inventory.ConsumeItemInSlot(useGrenade.itemUsed, useGrenade.itemUsedFromSlotName); //destroy grenade
                menu.CloseGrenadeUI();
            }
            else
            {
                if (menu.ValidateIntInput(throwTarget.XPos, out int x) && menu.ValidateIntInput(throwTarget.YPos, out int y) && menu.ValidateIntInput(throwTarget.ZPos, out int z))
                {
                    useGrenade.itemUsed.UseItem(useGrenade.itemUsedIcon, useGrenade.itemUsedOn, useGrenade.soldierUsedOn);
                    useGrenade.itemUsed.CheckExplosionGrenade(activeSoldier, new Vector3(x, y, z));
                    menu.CloseGrenadeUI();
                }
            }
        }
    }
    public void ConfirmThrow(UseItemUI throwItemUI)
    {
        ValidThrowChecker throwTarget = throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").GetComponent<ValidThrowChecker>();
        GameObject throwBeyondRadius = throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("ThrowBeyondRadius").gameObject;
        GameObject throwBeyondBlindRadius = throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("ThrowBeyondBlindRadius").gameObject;
        GameObject scatteredOffMap = throwItemUI.transform.Find("OptionPanel").Find("ScatteredOffMap").gameObject;
        GameObject itemWillBreak = throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("ItemWillBreak").gameObject;
        GameObject catcher = throwItemUI.transform.Find("OptionPanel").Find("Catcher").gameObject;

        if (!throwItemUI.transform.Find("PressedOnce").gameObject.activeInHierarchy) //first press
        {
            if (menu.ValidateIntInput(throwTarget.XPos, out int x) && menu.ValidateIntInput(throwTarget.YPos, out int y) && menu.ValidateIntInput(throwTarget.ZPos, out int z) && !throwBeyondRadius.activeInHierarchy && !throwBeyondBlindRadius.activeInHierarchy)
            {
                int newX, newY;
                throwTarget.GetThrowLocation(out Vector3 throwLocation);
                int throwDistance = Mathf.RoundToInt(Vector3.Distance(new(activeSoldier.X, activeSoldier.Y, activeSoldier.Z), throwLocation));
                int scatterDegree = HelperFunctions.RandomNumber(0, 360);
                int scatterDistance = activeSoldier.StrengthCheck() switch
                {
                    false => Mathf.CeilToInt(DiceRoll() * activeSoldier.stats.Str.Val / 2.0f),
                    _ => -1,
                };

                if (scatterDistance == -1 || throwDistance <= 3)
                    throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("PreciseThrow").gameObject.SetActive(true);
                else
                {
                    (newX, newY) = CalculateScatteredCoordinates(x, y, scatterDegree, scatterDistance);

                    throwTarget.XPos.text = $"{newX}";
                    throwTarget.YPos.text = $"{newY}";

                    if (newX <= 0 || newX > maxX || newY <= 0 || newY > maxY) //if scattering off map
                        scatteredOffMap.SetActive(true);

                    throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("FinalPosition").gameObject.SetActive(true);
                }

                throwItemUI.transform.Find("PressedOnce").gameObject.SetActive(true);
                throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("XPos").GetComponent<LocationInputController>().SetMin(1);
                throwItemUI.transform.Find("OptionPanel").Find("ThrowTarget").Find("YPos").GetComponent<LocationInputController>().SetMin(1);
            }
        }
        else //second press
        {
            if (scatteredOffMap.activeInHierarchy)
            {
                activeSoldier.Inventory.ConsumeItemInSlot(throwItemUI.itemUsed, throwItemUI.itemUsedFromSlotName); //destroy item
                menu.CloseThrowUI();
            }
            else
            {
                if (menu.ValidateIntInput(throwTarget.YPos, out int x) && menu.ValidateIntInput(throwTarget.YPos, out int y) && menu.ValidateIntInput(throwTarget.ZPos, out int z))
                {
                    if (itemWillBreak.activeInHierarchy)
                    {
                        throwItemUI.itemUsed.TakeDamage(activeSoldier, 1, new() { "Fall" }); //destroy item
                    }
                    else if (catcher.activeInHierarchy)
                    {
                        if (throwItemUI.itemUsed.IsCatchable())
                        {
                            Soldier catchingSoldier = soldierManager.FindSoldierByName(catcher.GetComponentInChildren<TMP_Dropdown>().captionText.text);

                            //if soldier has left hand free catch it there, otherwise catch in right hand
                            if (catchingSoldier.LeftHandItem == null)
                                throwItemUI.itemUsed.MoveItem(activeSoldier, throwItemUI.itemUsedFromSlotName, catchingSoldier, "LeftHand");
                            else
                                throwItemUI.itemUsed.MoveItem(activeSoldier, throwItemUI.itemUsedFromSlotName, catchingSoldier, "RightHand");
                        }
                        else
                        {
                            throwItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(throwItemUI.itemUsed, throwItemUI.itemUsedFromSlotName); //move item to ground
                            throwItemUI.itemUsed.X = x;
                            throwItemUI.itemUsed.Y = y;
                            throwItemUI.itemUsed.Z = z;
                        }
                    }
                    else
                    {
                        throwItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(throwItemUI.itemUsed, throwItemUI.itemUsedFromSlotName); //move item to ground
                        throwItemUI.itemUsed.X = x;
                        throwItemUI.itemUsed.Y = y;
                        throwItemUI.itemUsed.Z = z;
                    }
                    activeSoldier.PerformLoudAction(5);
                    menu.CloseThrowUI();
                }
            }
        }
    }
    public void ConfirmDrop(UseItemUI dropItemUI)
    {
        TMP_InputField targetX = dropItemUI.transform.Find("OptionPanel").Find("DropTarget").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField targetY = dropItemUI.transform.Find("OptionPanel").Find("DropTarget").Find("YPos").GetComponent<TMP_InputField>();
        TMP_InputField targetZ = dropItemUI.transform.Find("OptionPanel").Find("DropTarget").Find("ZPos").GetComponent<TMP_InputField>();
        GameObject invalidThrow = dropItemUI.transform.Find("OptionPanel").Find("DropTarget").Find("InvalidThrow").gameObject;
        GameObject itemWillBreak = dropItemUI.transform.Find("OptionPanel").Find("DropTarget").Find("ItemWillBreak").gameObject;
        GameObject catcher = dropItemUI.transform.Find("OptionPanel").Find("Catcher").gameObject;

        if (menu.ValidateIntInput(targetX, out int x) && menu.ValidateIntInput(targetY, out int y) && menu.ValidateIntInput(targetZ, out int z) && !invalidThrow.activeInHierarchy)
        {
            if (itemWillBreak.activeInHierarchy)
                dropItemUI.itemUsed.TakeDamage(activeSoldier, 1, new() { "Fall" }); //destroy item
            else if (catcher.activeInHierarchy)
            {
                if (dropItemUI.itemUsed.IsCatchable())
                {
                    Soldier catchingSoldier = soldierManager.FindSoldierByName(catcher.GetComponentInChildren<TMP_Dropdown>().captionText.text);

                    //if soldier has left hand free catch it there, otherwise catch in right hand
                    if (catchingSoldier.LeftHandItem == null)
                        dropItemUI.itemUsed.MoveItem(activeSoldier, dropItemUI.itemUsedFromSlotName, catchingSoldier, "LeftHand");
                    else
                        dropItemUI.itemUsed.MoveItem(activeSoldier, dropItemUI.itemUsedFromSlotName, catchingSoldier, "RightHand");
                }
                else
                {
                    dropItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(dropItemUI.itemUsed, dropItemUI.itemUsedFromSlotName); //move item to ground
                    dropItemUI.itemUsed.X = x;
                    dropItemUI.itemUsed.Y = y;
                    dropItemUI.itemUsed.Z = z;
                }
            }
            else
            {
                dropItemUI.itemUsed.owner?.Inventory.RemoveItemFromSlot(dropItemUI.itemUsed, dropItemUI.itemUsedFromSlotName); //move item to ground
                dropItemUI.itemUsed.X = x;
                dropItemUI.itemUsed.Y = y;
                dropItemUI.itemUsed.Z = z;
            }
            activeSoldier.PerformLoudAction(5);
            menu.CloseDropUI();
        }
    }
    public void ConfirmBinoculars(UseItemUI useBinoculars)
    {
        
        TMP_InputField focusX = useBinoculars.transform.Find("OptionPanel").Find("FocusPosition").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField focusY = useBinoculars.transform.Find("OptionPanel").Find("FocusPosition").Find("YPos").GetComponent<TMP_InputField>();

        if (menu.ValidateIntInput(focusX, out int x) && menu.ValidateIntInput(focusY, out int y))
        {
            useBinoculars.itemUsed.UseItem(useBinoculars.itemUsedIcon, useBinoculars.itemUsedOn, useBinoculars.soldierUsedOn);
            string useMode = useBinoculars.itemUsed.GetBinocularMode(useBinoculars.itemUsedFromSlotName);
            StartCoroutine(activeSoldier.SetUsingBinoculars(new(x, y), useMode));

            menu.CloseBinocularsUI();
        }
    }
    public void ConfirmClaymore(UseItemUI useClaymore)
    {
        TMP_InputField placedX = useClaymore.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField placedY = useClaymore.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("YPos").GetComponent<TMP_InputField>();
        TMP_InputField placedZ = useClaymore.transform.Find("OptionPanel").Find("ClaymorePlacing").Find("ZPos").GetComponent<TMP_InputField>();
        TMP_InputField facingX = useClaymore.transform.Find("OptionPanel").Find("ClaymoreFacing").Find("XPos").GetComponent<TMP_InputField>();
        TMP_InputField facingY = useClaymore.transform.Find("OptionPanel").Find("ClaymoreFacing").Find("YPos").GetComponent<TMP_InputField>();


        if (menu.ValidateIntInput(placedX, out int x) && menu.ValidateIntInput(placedY, out int y) && menu.ValidateIntInput(placedZ, out int z) && menu.ValidateIntInput(facingX, out int fx) && menu.ValidateIntInput(facingY, out int fy))
        {
            if (CalculateRange(activeSoldier, new Vector3(x, y, z)) <= activeSoldier.SRColliderMin.radius)
            {
                useClaymore.itemUsed.UseItem(useClaymore.itemUsedIcon, useClaymore.itemUsedOn, useClaymore.soldierUsedOn);
                Instantiate(poiManager.claymorePrefab).Init(new(x, y, z), Tuple.Create(activeSoldier.ActiveC, fx, fy, activeSoldier.Id));

                activeSoldier.PerformLoudAction(10);
                menu.CloseClaymoreUI();
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

        if (menu.ValidateIntInput(placedX, out int x) && menu.ValidateIntInput(placedY, out int y) && menu.ValidateIntInput(placedZ, out int z))
        {
            if (CalculateRange(activeSoldier, new Vector3(x, y, z)) <= activeSoldier.SRColliderMin.radius)
            {
                //play use deployment beacon
                soundManager.PlayUseDepBeacon();

                useDeploymentBeacon.itemUsed.UseItem(useDeploymentBeacon.itemUsedIcon, useDeploymentBeacon.itemUsedOn, useDeploymentBeacon.soldierUsedOn);
                Instantiate(poiManager.deploymentBeaconPrefab).Init(new(x, y, z), activeSoldier.Id);

                activeSoldier.PerformLoudAction(10);
                menu.CloseDeploymentBeaconUI();
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

        if (menu.ValidateIntInput(placedX, out int x) && menu.ValidateIntInput(placedY, out int y) && menu.ValidateIntInput(placedZ, out int z) && menu.ValidateIntInput(facingX, out int fx) && menu.ValidateIntInput(facingY, out int fy))
        {
            if (CalculateRange(activeSoldier, new Vector3(x, y, z)) <= activeSoldier.SRColliderMin.radius)
            {
                useThermalCam.itemUsed.UseItem(useThermalCam.itemUsedIcon, useThermalCam.itemUsedOn, useThermalCam.soldierUsedOn);
                
                SetLosCheckAll("losChange|thermalCamActive"); //loscheckall
                Instantiate(poiManager.thermalCamPrefab).Init(new(x, y, z), Tuple.Create(fx, fy, activeSoldier.Id));

                activeSoldier.PerformLoudAction(10);
                menu.CloseThermalCamUI();
            }
            else
                useThermalCam.transform.Find("OptionPanel").Find("OutOfRange").gameObject.SetActive(true);
        }
    }
    static Tuple<int, int> CalculateScatteredCoordinates(int targetX, int targetY, float scatterDegree, float scatterDistance)
    {
        // Convert degree to radians
        double radians = Math.PI * scatterDegree / 180.0;

        // Calculate new coordinates
        int newX = Mathf.RoundToInt((float)(targetX + scatterDistance * Math.Cos(radians)));
        int newY = Mathf.RoundToInt((float)(targetY + scatterDistance * Math.Sin(radians)));

        return Tuple.Create(newX, newY);
    }
    public void CheckExplosionUHF(Soldier explodedBy, Vector3 position, int radius, int damage)
    {
        //play explosion sfx
        soundManager.PlayExplosion();

        GameObject explosionList = Instantiate(menu.explosionListPrefab, menu.explosionUI.transform).GetComponent<ExplosionList>().Init($"UHF | Detonated: {position.x},{position.y},{position.z} | Radius: {radius}cm | Damage: {damage}").gameObject;
        explosionList.transform.Find("ExplodedBy").GetComponent<TextMeshProUGUI>().text = explodedBy.id;

        //create explosion objects
        Explosion explosion1 = Instantiate(menu.poiManager.explosionPrefab, position, default).Init(radius / 2, position);
        Explosion explosion2 = Instantiate(menu.poiManager.explosionPrefab, position, default).Init(radius, position);

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
                    menu.AddExplosionAlertItem(explosionList, hitItem, position, explodedBy, Mathf.RoundToInt(damagef));
                else if (obj is POI hitPoi)
                    menu.AddExplosionAlertPOI(explosionList, hitPoi, explodedBy, Mathf.RoundToInt(damagef));
                else if (obj is Soldier hitSoldier)
                    menu.AddExplosionAlert(explosionList, hitSoldier, position, explodedBy, hitSoldier.RoundByResilience(damagef) - hitSoldier.stats.R.Val, 1);
            }
        }

        //show explosion ui
        menu.OpenExplosionUI();
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
        foreach (Item item in itemManager.allItems)
        {
            if (item.IsJamming())
                item.jammingForTurns--;
            if (item.IsSpying())
                item.spyingForTurns--;
        }
    }





    //dipelec functions
    public void UpdateDipElecUI()
    {
        if (!menu.clearDipelecFlag)
            UpdateDipElecRewardAndChance();

    }
    public void UpdateDipElecRewardAndChance()
    {
        dipelecUI.dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();

        if (dipelecUI.dipElecTypeDropdown.value == 0)
        {
            dipelecUI.levelUI.SetActive(true);
            for (int i = 1; i <= 6; i++)
                if (activeSoldier.stats.Dip.Val + activeSoldier.TacticianBonus() < i)
                    dipelecUI.dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Add($"{i}");
            if (dipelecUI.dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Contains($"{dipelecUI.dipElecLevelDropdown.value + 1}"))
                dipelecUI.dipElecLevelDropdown.value = 0;

            dipelecUI.successChanceDisplay.text = Mathf.FloorToInt(CumulativeBinomialProbability(activeSoldier.stats.Dip.Val + activeSoldier.TacticianBonus(), dipelecUI.dipElecLevelDropdown.value + 1, 0.5f, 0.5f) * 100f).ToString() + "%";
        }
        else if (dipelecUI.dipElecTypeDropdown.value == 1)
        {
            dipelecUI.levelUI.SetActive(true);
            for (int i = 1; i <= 6; i++)
                if (activeSoldier.stats.Elec.Val + activeSoldier.CalculatorBonus() < i)
                    dipelecUI.dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Add($"{i}");
            if (dipelecUI.dipElecLevelDropdown.GetComponent<DropdownController>().optionsToGrey.Contains($"{dipelecUI.dipElecLevelDropdown.value + 1}"))
                dipelecUI.dipElecLevelDropdown.value = 0;
            dipelecUI.successChanceDisplay.text = Mathf.FloorToInt(CumulativeBinomialProbability(activeSoldier.stats.Elec.Val + activeSoldier.CalculatorBonus(), dipelecUI.dipElecLevelDropdown.value + 1, 0.5f, 0.5f) * 100f).ToString() + "%";
        }
        else
        {
            dipelecUI.levelUI.SetActive(false);
            dipelecUI.successChanceDisplay.text = "100%";
        }
    }
    public void ConfirmDipElec()
    {
        if (activeSoldier.CheckAP(3))
        {
            menu.FreezeTimer();
            activeSoldier.DeductAP(3);
            bool terminalDisabled = false;
            int passCount = 0;
            string resultString = "";

            Terminal terminal = poiManager.FindPOIById(dipelecUI.terminalID.text) as Terminal;

            if (dipelecUI.dipElecTypeDropdown.value == 0)
            {
                resultString += "Negotiation";
                terminal.SoldiersAlreadyNegotiated.Add(activeSoldier.Id);
                for (int i = 0; i < activeSoldier.stats.Dip.Val; i++)
                {
                    if (CoinFlip())
                        passCount++;
                }
            }
            else if (dipelecUI.dipElecTypeDropdown.value == 1)
            {
                resultString += "Hack";
                terminal.SoldiersAlreadyHacked.Add(activeSoldier.Id);
                for (int i = 0; i < activeSoldier.stats.Elec.Val; i++)
                {
                    if (CoinFlip())
                        passCount++;
                }
            }
            else
                terminalDisabled = true;

            if (!terminalDisabled)
            {
                int targetLevel = dipelecUI.dipElecLevelDropdown.value + 1;
                if (passCount >= targetLevel)
                {
                    for (int i = targetLevel; i >= 1; i--)
                    {
                        menu.dipelecResultUI.transform.Find("OptionPanel").Find("Title").GetComponentInChildren<TextMeshProUGUI>().text = $"<color=green>{resultString} successful</color>";
                        GameObject dipelecReward = Instantiate(menu.dipelecRewardPrefab, menu.dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"));
                        dipelecReward.GetComponentInChildren<TextMeshProUGUI>().text = (resultString == "Hack") ? dipelec.GetLevelElec(i) : dipelec.GetLevelDip(i);
                    }

                    //add xp for successful dipelec
                    menu.AddXpAlert(activeSoldier, (int)Mathf.Pow(2, targetLevel-1), $"Successful level {targetLevel} {resultString}", true);

                    resultString += $"SuccessL{targetLevel}";
                }
                else
                {
                    menu.dipelecResultUI.transform.Find("OptionPanel").Find("Title").GetComponentInChildren<TextMeshProUGUI>().text = $"<color=red>{resultString} failed</color>";
                    GameObject dipelecReward = Instantiate(menu.dipelecRewardPrefab, menu.dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"));
                    dipelecReward.GetComponentInChildren<TextMeshProUGUI>().text = $"No reward";

                    resultString += $"Fail";
                }
            }
            else
            {
                GameObject dipelecReward = Instantiate(menu.dipelecRewardPrefab, menu.dipelecResultUI.transform.Find("OptionPanel").Find("Rewards"));
                dipelecReward.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=red>Terminal disabled</color>";

                terminal.terminalEnabled = false;
                if (activeSoldier.hp > 3)
                    activeSoldier.TakeDamage(activeSoldier, activeSoldier.hp - 3, true, new() { "Dipelec" });
            }
            //play dipelec result sfx
            soundManager.PlayDipelecResolution(resultString);

            menu.OpenDipelecResultUI();
            menu.CloseDipElecUI();
        }
    }

















    //lastandicide functions
    public void Lastandicide()
    {
        if (activeSoldier.CheckAP(1))
        {
            activeSoldier.DeductAP(1);
            activeSoldier.InstantKill(activeSoldier, new List<string> { "Lastandicide" });
        }
    }

    //trauma functions
    public void TraumaCheck(Soldier deadSoldier, int tp, bool commander, bool lastandicide)
    {
        if (deadSoldier.IsDead())
        {
            bool showTraumaUI = false;

            foreach (Soldier friendly in AllSoldiers())
            {
                //print(friendly.soldierName + " trauma check attempting to run");

                if (friendly.IsSameTeamAs(deadSoldier) && friendly.IsAlive())
                {
                    //print(friendly.soldierName + " trauma check actually running");
                    //desensitised
                    if (friendly.IsDesensitised())
                        menu.AddTraumaAlert(friendly, tp, friendly.soldierName + " is " + friendly.GetTraumaState() + ". He is immune to trauma.", 0, 0, "");
                    else
                    {
                        //guaranteed trauma from commander death and/or lastandicide
                        if (commander)
                        {
                            menu.AddTraumaAlert(friendly, 1, "Commander died, an automatic trauma point has been accrued.", 0, 0, "");
                            showTraumaUI = true;
                        }
                        if (lastandicide)
                        {
                            menu.AddTraumaAlert(friendly, 1, deadSoldier.soldierName + " committed Lastandicide, an automatic trauma point has been accrued.", 0, 0, "");
                            showTraumaUI = true;
                        }

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
                                    menu.AddTraumaAlert(friendly, tp, $"{friendly.soldierName} is Resilient. Within {range} range of {deadSoldier.soldierName}. Check for LOS?", rolls, xpOnResist, range);
                                else
                                    menu.AddTraumaAlert(friendly, tp, $"{friendly.soldierName} is within {range} range of {deadSoldier.soldierName}. Check for LOS?", rolls, xpOnResist, range);

                                showTraumaUI = true;
                            }
                        }
                    }
                }
            }

            if (showTraumaUI)
                StartCoroutine(menu.OpenTraumaAlertUI());
        }
    }
    public void ConfirmTrauma()
    {
        ScrollRect traumaScroller = menu.traumaUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();
        bool unresolved = false;

        if (traumaScroller.verticalNormalizedPosition <= 0.05f)
        {
            Transform traumaAlerts = menu.traumaUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");

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

                menu.CloseTraumaUI();
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
            activeSoldier.TakeBloodlettingDamage();
            menu.CloseDamageEventUI();
        }
        else if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Other") && int.TryParse(damageEventUI.otherInput.text, out int otherDamage))
        {
            activeSoldier.TakeDamage(null, otherDamage, false, new() { damageEventUI.damageSource.text });
            menu.CloseDamageEventUI();
        }
        else
        {
            //check input
            if (GetFallOrCollapseLocation(out Tuple<Vector3, string> fallCollapseLocation))
            {
                if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Fall"))
                    activeSoldier.TakeDamage(null, CalculateFallDamage(activeSoldier, int.Parse(damageEventUI.fallInput.text)), false, new() { "Fall" });
                else if (damageEventUI.damageEventTypeDropdown.captionText.text.Contains("Collapse"))
                {
                    int structureHeight = int.Parse(damageEventUI.structureHeight.text);
                    //add xp if survives, otherwise kill
                    if (activeSoldier.StructuralCollapseCheck(structureHeight))
                    {
                        menu.AddXpAlert(activeSoldier, activeSoldier.stats.R.Val, $"Survived a {structureHeight}cm structural collapse.", true);
                        menu.AddDamageAlert(activeSoldier, $"{activeSoldier.soldierName} survived a {structureHeight}cm structural collapse.", true, false);
                    }
                    else
                    {
                        if (activeSoldier.IsWearingJuggernautArmour(false))
                        {
                            activeSoldier.MakeUnconscious(null, new() { "Structural Collapse" });
                            menu.AddDamageAlert(activeSoldier, $"{activeSoldier.soldierName} survived a {structureHeight}cm structural collapse with Juggernaut Armour.", true, false);
                        }
                        else
                        {
                            activeSoldier.InstantKill(null, new() { "Structural Collapse" });
                            activeSoldier.SetCrushed();
                        }
                    }
                }

                //move actually proceeds
                PerformMove(activeSoldier, 0, fallCollapseLocation, false, false, string.Empty, true);

                menu.CloseDamageEventUI();

            }
            else
                print("Invalid Input");
        }
    }
    public bool GetFallOrCollapseLocation(out Tuple<Vector3, string> fallCollapseLocation)
    {
        fallCollapseLocation = default;
        if (menu.ValidateIntInput(damageEventUI.xPos, out int x) && menu.ValidateIntInput(damageEventUI.yPos, out int y) && menu.ValidateIntInput(damageEventUI.zPos, out int z) && damageEventUI.terrainDropdown.value != 0)
        {
            fallCollapseLocation = Tuple.Create(new Vector3(x, y, z), damageEventUI.terrainDropdown.captionText.text);

            return true;
        }

        return false;
    }






    //inspirer functions
    public void CheckInspirer(Soldier inspirer)
    {
        menu.SetInspirerResolvedFlagTo(false);
        bool openInspirerUI = false;

        foreach (Soldier friendly in AllSoldiers())
        {
            if (friendly.IsAbleToSee() && inspirer.IsSameTeamAs(friendly) && friendly.PhysicalObjectWithinMaxRadius(inspirer) && !friendly.IsRevoker())
            {
                menu.AddInspirerAlert(friendly, "An Inspirer (" + inspirer.soldierName + ") is within SR range of " + friendly.soldierName + ". Is LOS present?");
                openInspirerUI = true;
            }
        }

        if (openInspirerUI)
            menu.OpenInspirerUI();
        else
            menu.SetInspirerResolvedFlagTo(true);
    }
    public void ConfirmInspirer()
    {
        ScrollRect inspireScroller = menu.inspirerUI.transform.Find("OptionPanel").Find("Scroll").GetComponent<ScrollRect>();

        if (inspireScroller.verticalNormalizedPosition <= 0.05f)
        {
            Transform inspirerAlerts = menu.inspirerUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content");

            foreach (Transform child in inspirerAlerts)
            {
                if (child.Find("InspirerToggle").GetComponent<Toggle>().isOn)
                    child.GetComponent<SoldierAlert>().soldier.SetInspired();
            }

            //destroy inspirer alerts
            foreach (Transform child in inspirerAlerts)
                Destroy(child.gameObject);

            menu.CloseInspirerUI();
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

            if (killer.IsHigherRankThan(deadMan))
                xp = 6 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
            else
                xp = 6 + rankDifference;

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

            if (killer.IsHigherRankThan(deadMan))
                xp = 10 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
            else
                xp = 10 + rankDifference;

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

            if (killer.IsHigherRankThan(deadMan))
                xp = 20 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
            else
                xp = 20 + rankDifference;

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
    public bool GlimpseDetectionMovingSeesDetecteeThroughOverwatchCone(Soldier movingSoldier, Soldier detectee, Vector3 movingSoldierOldPosition)
    {
        //reset bound crosses to zero
        boundCrossOne = Vector3.zero;
        boundCrossTwo = Vector3.zero;

        List<Vector3> previousPositionsMovingSoldier = new();
        List<bool> movingSoldierSeesDetectee = new();
        int boundCrossCount = 0;

        float maxSteps = CalculateRange(movingSoldier, movingSoldierOldPosition);

        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 position = new(movingSoldierOldPosition.x + (movingSoldier.X - movingSoldierOldPosition.x) * (i / maxSteps), movingSoldierOldPosition.y + (movingSoldier.Y - movingSoldierOldPosition.y) * (i / maxSteps), movingSoldierOldPosition.z + (movingSoldier.Z - movingSoldierOldPosition.z) * (i / maxSteps));
            previousPositionsMovingSoldier.Add(position);

            //record when moving solider sees detectee
            if (CalculateRange(detectee, position) <= movingSoldier.SRColliderFull.radius)
                movingSoldierSeesDetectee.Add(true);
            else
                movingSoldierSeesDetectee.Add(false);

            //print("Point " + i + ": " + CalculateRange(detectee, position));
        }

        //find borders where moving soldier crossed into detectee radius
        for (int i = 0; i < movingSoldierSeesDetectee.Count - 1; i++)
        {
            if (movingSoldierSeesDetectee[i] != movingSoldierSeesDetectee[i + 1])
            {
                boundCrossCount++;
                if (boundCrossOne == Vector3.zero)
                    boundCrossOne = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
                else
                    boundCrossTwo = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
            }
        }

        print("GlimpseMovingSeesDetectee: Bound cross count: " + boundCrossCount);
        print("GlimpseMovingSeesDetectee: Bound cross one = " + "X:" + boundCrossOne.x + " Y:" + boundCrossOne.y + " Z:" + boundCrossOne.z);
        print("GlimpseMovingSeesDetectee: Bound cross two = " + "X:" + boundCrossTwo.x + " Y:" + boundCrossTwo.y + " Z:" + boundCrossTwo.z);

        if (boundCrossCount > 0)
            return true;

        return false;
    }
    public bool GlimpseDetectionDetecteeSeesMovingThroughOverwatchCone(Soldier movingSoldier, Soldier detectee, Vector3 movingSoldierOldPosition)
    {
        //reset bound crosses to zero
        boundCrossOne = Vector3.zero;
        boundCrossTwo = Vector3.zero;

        List<Vector3> previousPositionsMovingSoldier = new();
        List<bool> detecteeSeesMovingSoldier = new();
        int boundCrossCount = 0;

        float maxSteps = CalculateRange(movingSoldier, movingSoldierOldPosition);

        //print("Radius:" + detectee.SRColliderMax.radius);
        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 position = new(movingSoldierOldPosition.x + (movingSoldier.X - movingSoldierOldPosition.x) * (i / maxSteps), movingSoldierOldPosition.y + (movingSoldier.Y - movingSoldierOldPosition.y) * (i / maxSteps), movingSoldierOldPosition.z + (movingSoldier.Z - movingSoldierOldPosition.z) * (i / maxSteps));
            previousPositionsMovingSoldier.Add(position);

            //record when detectee sees moving soldier
            if (detectee.PhysicalObjectWithinOverwatchCone(position))
                detecteeSeesMovingSoldier.Add(true);
            else
                detecteeSeesMovingSoldier.Add(false);

            //print("Point " + i + ": " + CalculateRange(detectee, position));
        }

        //find borders where moving soldier crossed into detectee radius
        for (int i = 0; i < detecteeSeesMovingSoldier.Count - 1; i++)
        {
            if (detecteeSeesMovingSoldier[i] != detecteeSeesMovingSoldier[i + 1])
            {
                boundCrossCount++;
                if (boundCrossOne == Vector3.zero)
                    boundCrossOne = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
                else
                    boundCrossTwo = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
            }
        }

        print("GlimpseDetecteeSeesMoving: Bound cross count: " + boundCrossCount);
        print("GlimpseDetecteeSeesMoving: Bound cross one = " + "X:" + boundCrossOne.x + " Y:" + boundCrossOne.y + " Z:" + boundCrossOne.z);
        print("GlimpseDetecteeSeesMoving: Bound cross two = " + "X:" + boundCrossTwo.x + " Y:" + boundCrossTwo.y + " Z:" + boundCrossTwo.z);

        if (boundCrossCount > 0)
            return true;

        return false;
    }
    public bool GlimpseDetectionDetecteeSeesMoving(Soldier movingSoldier, Soldier detectee, Vector3 movingSoldierOldPosition)
    {
        //reset bound crosses to zero
        boundCrossOne = Vector3.zero;
        boundCrossTwo = Vector3.zero;

        List<Vector3> previousPositionsMovingSoldier = new();
        List<bool> detecteeSeesMovingSoldier = new();
        int boundCrossCount = 0;

        float maxSteps = CalculateRange(movingSoldier, movingSoldierOldPosition);

        //print("Radius:" + detectee.SRColliderMax.radius);
        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 position = new(movingSoldierOldPosition.x + (movingSoldier.X - movingSoldierOldPosition.x) * (i / maxSteps), movingSoldierOldPosition.y + (movingSoldier.Y - movingSoldierOldPosition.y) * (i / maxSteps), movingSoldierOldPosition.z + (movingSoldier.Z - movingSoldierOldPosition.z) * (i / maxSteps));
            previousPositionsMovingSoldier.Add(position);

            //record when detectee sees moving soldier
            if (CalculateRange(detectee, position) <= detectee.SRColliderFull.radius)
                detecteeSeesMovingSoldier.Add(true);
            else
                detecteeSeesMovingSoldier.Add(false);

            //print("Point " + i + ": " + CalculateRangeCover(detectee, position));
        }

        //find borders where moving soldier crossed into detectee radius
        for (int i = 0; i < detecteeSeesMovingSoldier.Count - 1; i++)
        {
            if (detecteeSeesMovingSoldier[i] != detecteeSeesMovingSoldier[i + 1])
            {
                boundCrossCount++;
                if (boundCrossOne == Vector3.zero)
                    boundCrossOne = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
                else
                    boundCrossTwo = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
            }
        }

        print("DetecteeSeesMoving: Bound cross count: " + boundCrossCount);
        print("DetecteeSeesMoving: Bound cross one = " + "X:" + boundCrossOne.x + " Y:" + boundCrossOne.y + " Z:" + boundCrossOne.z);
        print("DetecteeSeesMoving: Bound cross two = " + "X:" + boundCrossTwo.x + " Y:" + boundCrossTwo.y + " Z:" + boundCrossTwo.z);

        if (boundCrossCount > 0)
            return true;
        else
            return false;
    }

    public bool GlimpseDetectionMovingSeesDetectee(Soldier movingSoldier, Soldier detectee, Vector3 movingSoldierOldPosition)
    {
        //reset bound crosses to zero
        boundCrossOne = Vector3.zero;
        boundCrossTwo = Vector3.zero;

        List<Vector3> previousPositionsMovingSoldier = new();
        List<bool> movingSoldierSeesDetectee = new();
        int boundCrossCount = 0;

        float maxSteps = CalculateRange(movingSoldier, movingSoldierOldPosition);

        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 position = new(movingSoldierOldPosition.x + (movingSoldier.X - movingSoldierOldPosition.x) * (i / maxSteps), movingSoldierOldPosition.y + (movingSoldier.Y - movingSoldierOldPosition.y) * (i / maxSteps), movingSoldierOldPosition.z + (movingSoldier.Z - movingSoldierOldPosition.z) * (i / maxSteps));
            previousPositionsMovingSoldier.Add(position);

            //record when moving solider sees detectee
            if (CalculateRange(detectee, position) <= movingSoldier.SRColliderFull.radius)
                movingSoldierSeesDetectee.Add(true);
            else
                movingSoldierSeesDetectee.Add(false);
        }

        //find borders where moving soldier radius crossed over detectee
        for (int i = 0; i < movingSoldierSeesDetectee.Count - 1; i++)
        {
            if (movingSoldierSeesDetectee[i] != movingSoldierSeesDetectee[i + 1])
            {
                boundCrossCount++;
                if (boundCrossOne == Vector3.zero)
                    boundCrossOne = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
                else
                    boundCrossTwo = new(Mathf.Round(previousPositionsMovingSoldier[i].x), Mathf.Round(previousPositionsMovingSoldier[i].y), Mathf.Round(previousPositionsMovingSoldier[i].z));
            }
        }

        print("MovingSeesDetectee: Bound cross count: " + boundCrossCount);
        print("MovingSeesDetectee: Bound cross one = " + "X:" + boundCrossOne.x + " Y:" + boundCrossOne.y + " Z:" + boundCrossOne.z);
        print("MovingSeesDetectee: Bound cross two = " + "X:" + boundCrossTwo.x + " Y:" + boundCrossTwo.y + " Z:" + boundCrossTwo.z);

        if (boundCrossCount > 0)
            return true;
        else
            return false;
    }

    public int CalculateMoveDistance(Vector3 moveToLocation)
    {
        return Mathf.RoundToInt(Vector3.Distance(new Vector3(activeSoldier.X, activeSoldier.Y, activeSoldier.Z), moveToLocation));
    }

    public string CreateDetectionMessageClaymore(int claymoreC, int soldierP)
    {
        return $"<color=red>DETECTED</color>\nClaymore C={claymoreC} did not exceed P={soldierP}";
    }
    public string CreateDetectionMessage(string type, string distance, int activeStatValue, int perceptivenessMultiplier, int soldierPerceptiveness)
    {
        string title, part1, join, part2;

        part1 = $"(within {distance} C={activeStatValue}";
        part2 = $"{perceptivenessMultiplier}P={soldierPerceptiveness * perceptivenessMultiplier})";

        if (type.Contains("avoidance"))
        {
            title = "<color=green>AVOIDANCE</color>\n";
            join = " exceeded ";
        }
        else if (type.Contains("overwatch"))
        {
            title = "<color=yellow>OVERWATCH</color>\n";
            join = " did not exceed ";
        }
        else
        {
            title = "<color=red>DETECTED</color>\n";
            join = " did not exceed ";
        }

        return $"{title}{part1}{join}{part2}";
    }
    public string CreateDetectionMessage(string type, string distance, int activeStatValue, int perceptivenessMultiplier, int soldierPerceptiveness, Vector3 boundCrossOne)
    {
        string title, part1, join, part2;

        part1 = $"(Until: {boundCrossOne.x}, {boundCrossOne.y}, {boundCrossOne.z})\n(within {distance} C={activeStatValue}";
        part2 = $"{perceptivenessMultiplier}P={soldierPerceptiveness * perceptivenessMultiplier})";

        if (type.Contains("avoidance"))
        {
            title = "<color=green>RETREAT AVOIDANCE</color>\n";
            join = " exceeded ";
        }
        else if (type.Contains("overwatch"))
        {
            title = "<color=yellow>RETREAT OVERWATCH</color>\n";
            join = " did not exceed ";
        }
        else
        {
            title = "<color=red>RETREAT DETECTED</color>\n";
            join = " did not exceed ";
        }

        return $"{title}{part1}{join}{part2}";
    }
    public string CreateDetectionMessage(string type, string distance, int activeStatValue, int perceptivenessMultiplier, int soldierPerceptiveness, Vector3 boundCrossOne, Vector3 boundCrossTwo)
    {
        string title, part1, join, part2;

        part1 = $"(Between: {boundCrossOne.x}, {boundCrossOne.y}, {boundCrossOne.z} and {boundCrossTwo.x}, {boundCrossTwo.y}, {boundCrossTwo.z})\n(within {distance} C={activeStatValue}";
        part2 = $"{perceptivenessMultiplier}P={soldierPerceptiveness * perceptivenessMultiplier})";

        if (type.Contains("avoidance"))
        {
            title = "<color=green>GLIMPSE AVOIDANCE</color>\n";
            join = " exceeded ";
        }
        else if (type.Contains("overwatch"))
        {
            title = "<color=yellow>GLIMPSE OVERWATCH</color>\n";
            join = " did not exceed ";
        }
        else
        {
            title = "<color=red>GLIMPSE DETECTED</color>\n";
            join = " did not exceed ";
        }

        return $"{title}{part1}{join}{part2}";
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
                GoodyBox gb = Instantiate(poiManager.gbPrefab).Init(spawnLocation);
                //fill gb with items
                foreach (Transform child in insertObjectsUI.gbItemsPanel)
                {
                    ItemIconGB itemIcon = child.GetComponent<ItemIconGB>();
                    if (itemIcon != null && itemIcon.pickupNumber > 0)
                        for (int i = 0; i < child.GetComponent<ItemIconGB>().pickupNumber; i++)
                            gb.Inventory.AddItem(itemManager.SpawnItem(child.gameObject.name));
                }
            }
            else if (insertObjectsUI.objectTypeDropdown.value == 2)
                Instantiate(poiManager.terminalPrefab).Init(spawnLocation, insertObjectsUI.terminalTypeDropdown.captionText.text);
            else if (insertObjectsUI.objectTypeDropdown.value == 3)
                Instantiate(poiManager.barrelPrefab).Init(spawnLocation);
            else if (insertObjectsUI.objectTypeDropdown.value == 4)
            {
                DrugCabinet dc = Instantiate(poiManager.drugCabinetPrefab).Init(spawnLocation);
                //fill dc with items
                foreach (Transform child in insertObjectsUI.dcItemsPanel)
                {
                    ItemIconGB itemIcon = child.GetComponent<ItemIconGB>();
                    if (itemIcon != null && itemIcon.pickupNumber > 0)
                        for (int i = 0; i < child.GetComponent<ItemIconGB>().pickupNumber; i++)
                            dc.Inventory.AddItem(itemManager.SpawnItem(child.gameObject.name));
                }
            }

            menu.CloseOverrideInsertObjectsUI();
        }
        else
            print("Invalid Input");
    }
    public bool GetInsertLocation(out Tuple<Vector3, string> insertLocation)
    {
        insertLocation = default;
        if (menu.ValidateIntInput(insertObjectsUI.xPos, out int x) && menu.ValidateIntInput(insertObjectsUI.yPos, out int y) && menu.ValidateIntInput(insertObjectsUI.zPos, out int z))
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
}
