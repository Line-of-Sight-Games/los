using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class MainGame : MonoBehaviour, IDataPersistence
{
    public MainMenu menu;
    public ItemManager itemManager;
    public SoldierManager soldierManager;
    public WeatherGen weather;
    public DipelecGen dipelec;
    public SoundManager soundManager;
    public SetBattlefieldParameters setBattlefieldParameters;

    public Terminal terminalPrefab;
    public bool gameOver;
    public int maxX, maxY, maxZ;
    public int currentRound, maxRounds, currentTeam, maxTeams, maxTurnTime;
    public Camera cam;
    public Light sun;
    public GameObject battlefield, bottomPlane, outlineArea, notEnoughAPUI, notEnoughMPUI, moveToSameSpotUI;
    public TMP_InputField xPos, yPos, zPos, fallInput;
    public TMP_Dropdown moveTypeDropdown, terrainDropdown, shotTypeDropdown, gunTypeDropdown, aimTypeDropdown, coverLevelDropdown, targetDropdown, meleeTypeDropdown, attackerWeaponDropdown, meleeTargetDropdown, defenderWeaponDropdown, damageEventTypeDropdown;
    public TextMeshProUGUI moveAP;
    public Toggle coverToggle, meleeToggle;
    public int x, y, z, fallDistance;
    Vector3 boundCrossOne = Vector3.zero, boundCrossTwo = Vector3.zero;

    public UnitDisplayPanel displayPanel;
    public Transform allItemsContentUI, inventoryItemsContentUI, groundItemsContentUI, activeItemPanel, allyButtonContentUI;
    public Soldier activeSoldier;

    //helper functions - game
    public List<Soldier> AllSoldiers()
    {
        return soldierManager.allSoldiers;
    }
    public int RandomNumber(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
    public int DiceRoll()
    {
        return RandomNumber(1, 6);
    }
    public bool CoinFlip()
    {
        if (RandomNumber(0, 1) == 1)
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
            return (Factorial(n) / (Factorial(n - x) * Factorial(x))) * Mathf.Pow(p, x) * Mathf.Pow(q, n - x);
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
        return decimal.Round((decimal)Random.Range(0.5f, 5.0f), 1).ToString();
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


    //turn resolutions
    public void GameOver(string result)
    {
        gameOver = true;
        Debug.Log("GameOver");

        if (menu.overrideView)
            menu.ToggleOverrideView();

        menu.FreezeTime();
        menu.DisplaySoldiersGameOver();
        menu.roundIndicator.text = "Game Over";
        menu.playerTurnIndicator.text = result;
        soundManager.PlayGameOverMusic();
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

                if (currentTeam < maxTeams)
                {
                    currentTeam += 1;
                }
                else
                {
                    if (currentRound == maxRounds)
                        GameOver("No Winner");
                    else
                    {
                        currentTeam = 1;
                        EndRound();
                        currentRound += 1;
                    }
                }

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
        foreach (Soldier s in AllSoldiers())
        {
            if (s.IsOnturnAndAlive()) //run things that trigger at the end of players turn
            {
                //reset usedap flag
                s.usedAP = false;
                s.usedMP = false;

                //reset bloodletter used on turn flag
                s.bloodLettedThisTurn = false;

                //reset inspired
                if (s.IsInspired())
                    s.UnsetInspired();

                //remove donated planner movement
                s.plannerDonatedMove = 0;

                //remove dissuaded
                if (s.IsDissuaded())
                    s.UnsetDissuaded();

                //increase rounds fielded
                s.roundsFielded++;
                if (s.IsConscious())
                {
                    //only give xp for every 4 rounds coscious on the field
                    s.roundsFieldedConscious++;
                    if (s.roundsFieldedConscious % 4 == 0)
                        menu.AddXpAlert(s, 1, "Survived " + s.roundsFieldedConscious + " rounds.", true);
                }

                //increase rounds without food
                s.IncreaseRoundsWithoutFood();

                //decrement stunnage
                if (s.stunnedRoundsVulnerable > 0)
                    s.stunnedRoundsVulnerable--;

                //decrement loud action counter
                if (s.loudActionRoundsVulnerable > 0)
                    s.loudActionRoundsVulnerable--;

                //end suppression
                if (s.suppressionValue != 0)
                    s.suppressionValue = 0;
            }
            else //run things that trigger at the end of another player's turn
            {
                
            }
        }

        menu.CheckXP();

        //post xp stuff
        foreach (Soldier s in AllSoldiers())
        {
            if (s.IsOnturnAndAlive()) //run things that trigger at the end of players turn
            {
                //dish out poison damage
                if (s.IsPoisoned())
                    StartCoroutine(s.TakePoisonDamage());

            }
            else //run things that trigger at the end of another player's turn
            {

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
            if (currentTeam == 1)
                StartRound();

            StartPlayerTurn();
        }
    }
    public void StartPlayerTurn()
    {
        if (menu.damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content").childCount > 0)
            StartCoroutine(menu.OpenDamageList());

        foreach (Soldier s in AllSoldiers())
        {
            if (s.IsOnturnAndAlive())
            {
                //activate witness abilities
                if (s.IsWitness())
                {
                    foreach (string ability in s.witnessActiveAbilities)
                        s.soldierAbilities.Remove(ability);
                    s.witnessActiveAbilities.Clear();

                    foreach (string ability in s.witnessStoredAbilities)
                    {
                        s.witnessActiveAbilities.Add(ability);
                        s.soldierAbilities.Add(ability);
                    }
                }

                //run things that trigger at the start of players turn
                s.GenerateAP();
                if (s.IsInspirer())
                    InspirerCheck(s);

                StartCoroutine(DetectionAlertAll("statChange"));
            }
            else
            {
                //run things that trigger at the start of another player's turn
            }
        }

        menu.CheckXP();
        DataPersistenceManager.Instance.SaveGame();
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
        DrainAP();
        DrainMP();
    }

    //cover functions
    public void ConfirmCover()
    {
        if (CheckAP(1))
        {
            DeductAP(1);
            activeSoldier.SetCover();
        }

        menu.CloseTakeCoverUI();
    }

    //playdead functions
    public void CheckPlaydead()
    {
        if (activeSoldier.state.Contains("Playdead"))
            activeSoldier.UnsetPlaydead();
        else
            menu.OpenPlaydeadAlertUI();
    }
    public void ConfirmPlaydead()
    {
        activeSoldier.SetPlaydead();
        menu.ClosePlaydeadAlertUI();
    }

    //move functions
    public void FleeSoldier()
    {
        activeSoldier.InstantKill(activeSoldier, new List<string>() { "Flee" });
    }
    public int CalculateFallDamage(Soldier soldier, int fallDistance)
    {
        return Mathf.CeilToInt(Mathf.Pow(fallDistance / 4.0f, 2) / 2.0f - soldier.stats.R.Val);
    }
    public void UpdateMoveAP()
    {
        int ap = 0;
        string move = moveTypeDropdown.options[moveTypeDropdown.value].text;

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

            if (coverToggle.isOn)
                ap++;
        }

        menu.moveUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = ap.ToString();
    }
    public void UpdateMoveDonated()
    {
        if (moveTypeDropdown.options[moveTypeDropdown.value].text.Contains("Planner"))
            menu.moveUI.transform.Find("MoveDonated").Find("MoveDonatedDisplay").GetComponent<TextMeshProUGUI>().text = activeSoldier.HalfMove.ToString();
    }
    public void UpdateMoveUI()
    {
        if (!menu.clearMoveFlag)
        {
            UpdateMoveAP();
            UpdateMoveDonated();
        }
    }
    public Soldier FindClosestAlly()
    {
        List<System.Tuple<float, Soldier>> soldierDistances = new();

        foreach (Soldier s in AllSoldiers())
        {
            if (s.IsAbleToWalk() && s.IsSameTeamAs(activeSoldier))
                soldierDistances.Add(System.Tuple.Create(CalculateRange(activeSoldier, s), s));
        }
        soldierDistances.OrderBy(t => t.Item1);

        if (soldierDistances.Count > 0)
            return soldierDistances[0].Item2;
        else
            return null;
    }
    public void ConfirmMove(bool force)
    {
        //modify AP spend if speed is below 6
        string launchMelee = string.Empty;
        int.TryParse(moveAP.text, out int ap);

        if (moveTypeDropdown.options[moveTypeDropdown.value].text.Contains("Planner"))
        {
            if (CheckMP(1) && CheckAP(ap))
            {
                //planner donation proceeds
                DeductAP(ap);
                DeductMP(activeSoldier.mp);
                foreach (Transform child in menu.moveUI.transform.Find("ClosestAlly").Find("ClosestAllyPanel"))
                    soldierManager.FindSoldierByName(child.Find("SoldierName").GetComponent<TextMeshProUGUI>().text).plannerDonatedMove += activeSoldier.HalfMove;
            }
            menu.CloseMoveUI();
        }
        else if (moveTypeDropdown.options[moveTypeDropdown.value].text.Contains("Exo"))
        {
            //check input formatting
            if (int.TryParse(xPos.text, out x) && int.TryParse(yPos.text, out y) && int.TryParse(zPos.text, out z) && terrainDropdown.value != 0)
            {
                if (x >= 1 && x <= maxX && y >= 1 && y <= maxY && z >= 0 && z <= maxZ)
                {
                    if (activeSoldier.X != x || activeSoldier.Y != y || activeSoldier.Z != z)
                    {
                        if (x <= activeSoldier.X + 3 && x >= activeSoldier.X - 3 && y <= activeSoldier.Y + 3 && y >= activeSoldier.Y - 3)
                        {
                            if (CheckAP(ap))
                            {
                                //jump actually proceeds
                                DeductAP(ap);

                                //trigger loud action
                                activeSoldier.PerformLoudAction(10);

                                Vector3 oldPos = new(activeSoldier.X, activeSoldier.Y, activeSoldier.Z);
                                activeSoldier.X = x;
                                activeSoldier.Y = y;
                                activeSoldier.Z = z;
                                activeSoldier.TerrainOn = terrainDropdown.options[terrainDropdown.value].text;

                                //activate in cover
                                if (coverToggle.isOn)
                                    activeSoldier.SetCover();
                                else
                                    activeSoldier.UnsetCover();

                                //break melee control
                                BreakAllMeleeEngagements(activeSoldier);

                                //break suppression
                                if (activeSoldier.suppressionValue != 0)
                                    activeSoldier.suppressionValue = 0;

                                //clear planner donated movement
                                activeSoldier.plannerDonatedMove = 0;

                                //launch melee if melee toggle is on
                                if (meleeToggle.isOn)
                                {
                                    if (moveTypeDropdown.value == 0)
                                        launchMelee = "Full Charge Attack";
                                    else if (moveTypeDropdown.value == 1)
                                        launchMelee = "Half Charge Attack";
                                    else if (moveTypeDropdown.value == 2)
                                        launchMelee = "3cm Charge Attack";
                                }

                                StartCoroutine(DetectionAlertSingle(activeSoldier, "losChange", oldPos, launchMelee));
                            }
                            menu.CloseMoveUI();
                        }
                        else
                            menu.OpenOvermoveUI("Warning: Landing is further than 3cm away from jump point.");
                    }
                    else
                        menu.OpenMoveToSameSpotUI();
                }
                else
                    Debug.Log("x, y, z values must not be out of bounds.");
            }
            else
                Debug.Log("Formatting was wrong, try again.");
        }
        else
        {
            //check input formatting
            if (int.TryParse(xPos.text, out x) && int.TryParse(yPos.text, out y) && int.TryParse(zPos.text, out z) && terrainDropdown.value != 0)
            {
                //get maxmove
                float maxMove;
                if (moveTypeDropdown.options[moveTypeDropdown.value].text.Contains("Full Move"))
                    maxMove = activeSoldier.FullMove;
                else if (moveTypeDropdown.options[moveTypeDropdown.value].text.Contains("Half Move"))
                    maxMove = activeSoldier.HalfMove;
                else
                    maxMove = activeSoldier.TileMove;

                //check values are within bounds
                if (x >= 1 && x <= maxX && y >= 1 && y <= maxY && z >= 0 && z <= maxZ)
                {
                    //check to make sure not moving to same space
                    if (activeSoldier.X != x || activeSoldier.Y != y || activeSoldier.Z != z)
                    {
                        //skip supression check if it's already happened before, otherwise run it
                        if (coverToggle.interactable || (activeSoldier.IsSuppressed() && activeSoldier.SuppressionCheck()))
                        {
                            float distance = CalculateMoveDistance(x, y, z);
                            if (force || distance <= maxMove)
                            {
                                if (CheckMP(1) && CheckAP(ap))
                                {
                                    //move actually proceeds
                                    DeductAP(ap);
                                    DeductMP(1);
                                    Vector3 oldPos = new(activeSoldier.X, activeSoldier.Y, activeSoldier.Z);
                                    activeSoldier.X = x;
                                    activeSoldier.Y = y;
                                    activeSoldier.Z = z;
                                    activeSoldier.TerrainOn = terrainDropdown.options[terrainDropdown.value].text;

                                    //activate in cover
                                    if (coverToggle.isOn)
                                        activeSoldier.SetCover();
                                    else
                                        activeSoldier.UnsetCover();

                                    //break melee control
                                    BreakAllMeleeEngagements(activeSoldier);

                                    //break suppression
                                    if (activeSoldier.suppressionValue != 0)
                                        activeSoldier.suppressionValue = 0;

                                    //clear planner donated movement
                                    activeSoldier.plannerDonatedMove = 0;

                                    //check broken soldier leaving field
                                    if (activeSoldier.tp == 4 && (activeSoldier.X == maxX || activeSoldier.X == 1 || activeSoldier.Y == maxY || activeSoldier.Y == 1))
                                    {
                                        //freeze time to block detection alerts running
                                        menu.FreezeTime();
                                        menu.OpenBrokenFledUI();
                                    }
                                    //check for fall damage
                                    if (int.TryParse(fallInput.text, out fallDistance))
                                        activeSoldier.TakeDamage(activeSoldier, CalculateFallDamage(activeSoldier, fallDistance), false, new List<string>() { "Fall" });

                                    //launch melee if melee toggle is on
                                    if (meleeToggle.isOn)
                                    {
                                        if (moveTypeDropdown.value == 0)
                                            launchMelee = "Full Charge Attack";
                                        else if (moveTypeDropdown.value == 1)
                                            launchMelee = "Half Charge Attack";
                                        else if (moveTypeDropdown.value == 2)
                                            launchMelee = "3cm Charge Attack";
                                    }

                                    StartCoroutine(DetectionAlertSingle(activeSoldier, "losChange", oldPos, launchMelee));
                                }
                                menu.CloseMoveUI();

                            }
                            else
                                menu.OpenOvermoveUI("Warning: Proposed move is " + (distance - maxMove).ToString("F1") + "cm over max (" + distance.ToString("F1") + "/" + maxMove + "cm)");
                        }
                        else
                            menu.OpenSuppressionMoveUI();
                    }
                    else
                        menu.OpenMoveToSameSpotUI();
                }
                else
                    Debug.Log("x, y, z values must not be out of bounds.");
            }
            else
                Debug.Log("Formatting was wrong, try again.");
        }
    }





    public void TakeTrauma()
    {
        activeSoldier.TakeTrauma(1);
    }




    //shot functions
    public void UpdateShotUI()
    {
        if (!menu.clearShotFlag)
        {
            if (shotTypeDropdown.value == 0)
            {
                UpdateShotAP();
                UpdateHitPercentage();
            }
            else if (shotTypeDropdown.value == 1)
            {
                UpdateShotAP();
                UpdateSuppressionValue();
            }
            else
            {
                UpdateShotAP();
                UpdateHitPercentage();
            }
        } 
    }
    public void UpdateShotAP()
    {
        int ap = 1;
        if (shotTypeDropdown.value == 1)
        {
            ap = activeSoldier.ap;
        }
        else
        {
            if (aimTypeDropdown.value == 0)
            {
                if (activeSoldier.IsGunner())
                    ap++;
                else
                {
                    ap += itemManager.FindItemById(gunTypeDropdown.options[gunTypeDropdown.value].text).gunType switch
                    {
                        "Sniper" or "LMG" => 2,
                        _ => 1,
                    };
                }
            }
        }
        
        menu.shotUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = ap.ToString();
    }
    public void UpdateSuppressionValue()
    {
        Item gun = itemManager.FindItemById(gunTypeDropdown.options[gunTypeDropdown.value].text);
        Soldier target = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);

        int suppressionValue = CalculateRangeBracket(CalculateRange(activeSoldier, target)) switch
        {
            "Melee" or "CQB" => gun.gunCQBSuppressionPenalty,
            "Short" => gun.gunShortSuppressionPenalty,
            "Medium" => gun.gunMedSuppressionPenalty,
            "Long" or "Coriolis" => gun.gunLongSuppressionPenalty,
            _ => 0,
        };

        menu.shotUI.transform.Find("SuppressionValue").Find("SuppressionValueDisplay").GetComponent<TextMeshProUGUI>().text = suppressionValue.ToString();
    }
    public int WeaponHitChance(Soldier target, Item gun)
    {
        int weaponHitChance;

        //get base hit chance
        switch (CalculateRangeBracket(CalculateRange(activeSoldier, target)))
        {
            case "Melee":
            case "CQB":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunCQBA;
                else
                    weaponHitChance = gun.gunCQBU;
                break;
            case "Short":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunShortA;
                else
                    weaponHitChance = gun.gunShortU;
                break;
            case "Medium":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunMedA;
                else
                    weaponHitChance = gun.gunMedU;
                break;
            case "Long":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunLongA;
                else
                    weaponHitChance = gun.gunLongU;
                break;
            case "Coriolis":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunCoriolisA;
                else
                    weaponHitChance = gun.gunCoriolisU;
                break;
            default:
                weaponHitChance = 0;
                break;
        }

        //apply sharpshooter buff
        if (weaponHitChance > 0 && activeSoldier.IsSharpshooter())
            weaponHitChance += 5;

        //apply inspirer buff
        weaponHitChance += activeSoldier.InspirerBonusWeapon(gun);

        //correct negatives
        if (weaponHitChance < 0)
            weaponHitChance = 0;

        Debug.Log("Weapon Hit Chance: " + weaponHitChance);
        return weaponHitChance;
    }
    public int WeaponHitChanceCover(Vector3 cover, Item gun)
    {
        int weaponHitChance;

        //get base hit chance
        switch (CalculateRangeBracket(CalculateRangeCover(activeSoldier, cover)))
        {
            case "Melee":
            case "CQB":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunCQBA;
                else
                    weaponHitChance = gun.gunCQBU;
                break;
            case "Short":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunShortA;
                else
                    weaponHitChance = gun.gunShortU;
                break;
            case "Medium":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunMedA;
                else
                    weaponHitChance = gun.gunMedU;
                break;
            case "Long":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunLongA;
                else
                    weaponHitChance = gun.gunLongU;
                break;
            case "Coriolis":
                if (aimTypeDropdown.value == 0)
                    weaponHitChance = gun.gunCoriolisA;
                else
                    weaponHitChance = gun.gunCoriolisU;
                break;
            default:
                weaponHitChance = 0;
                break;
        }

        //apply sharpshooter buff
        if (weaponHitChance > 0 && activeSoldier.IsSharpshooter())
            weaponHitChance += 5;

        //apply inspirer buff
        weaponHitChance += activeSoldier.InspirerBonusWeapon(gun);

        //correct negatives
        if (weaponHitChance < 0)
            weaponHitChance = 0;

        Debug.Log("Weapon Hit Chance: " + weaponHitChance);
        return weaponHitChance;
    }
    public float ShooterTraumaMod()
    {
        var traumaMod = activeSoldier.tp switch
        {
            1 => 0.05f,
            2 => 0.15f,
            3 => 0.25f,
            _ => 0.0f,
        };

        Debug.Log("Trauma Mod: " + traumaMod);
        return 1 - traumaMod;
    }
    public float ShooterSustenanceMod()
    {
        float sustenanceMod = 0;

        if (activeSoldier.roundsWithoutFood >= 20)
            sustenanceMod = 0.25f;

        Debug.Log("Sustenance Mod: " + sustenanceMod);
        return 1 - sustenanceMod;
    }
    public float RelevantWeaponSkill(Item gun)
    {
        float weaponSkill = gun.gunType switch
        {
            "Assault Rifle" => activeSoldier.stats.AR.Val,
            "LMG" => activeSoldier.stats.LMG.Val,
            "Rifle" => activeSoldier.stats.Ri.Val,
            "Shotgun" => activeSoldier.stats.Sh.Val,
            "SMG" => activeSoldier.stats.SMG.Val,
            "Sniper" => activeSoldier.stats.Sn.Val,
            _ => activeSoldier.stats.GetHighestWeaponSkill(),
        };

        //apply juggernaut armour debuff
        if (activeSoldier.inventory.IsWearingJuggernautArmour())
            weaponSkill--;

        //apply stim armour buff
        if (activeSoldier.inventory.IsWearingStimulantArmour())
            weaponSkill += 2;

        //apply trauma debuff
        weaponSkill *= ShooterTraumaMod();

        //apply sustenance debuff
        weaponSkill *= ShooterSustenanceMod();

        //correct negatives
        if (weaponSkill < 0)
            weaponSkill = 0;

        Debug.Log(gun.gunType + " Skill: " + weaponSkill);
        return weaponSkill;
    }
    public int TargetEvasion(Soldier target)
    {
        Debug.Log("Target Evasion: " + target.stats.E.Val);
        return target.stats.E.Val;
    }
    public float CoverMod()
    {
        var coverMod = coverLevelDropdown.value switch
        {
            1 => 0.1f,
            2 => 0.34f,
            3 => 0.62f,
            4 => 0.88f,
            _ => 0.0f,
        };

        Debug.Log("Cover Mod: " + coverMod);
        return 1 - coverMod;
    }
    public float VisMod()
    {
        float visMod;

        if (activeSoldier.IsCommander())
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

        Debug.Log("Vis Mod: " + visMod);
        return 1 - visMod;
    }
    public float RainMod(Soldier target)
    {
        string rainfall = weather.CurrentRain;

        if (activeSoldier.IsCalculator())
            rainfall = weather.DecreasedRain(rainfall);
        if (target.IsCalculator())
            rainfall = weather.IncreasedRain(rainfall);

        var rainMod = rainfall switch
        {
            "Zero" or "Light" => 0.0f,
            "Moderate" => 0.02f,
            "Heavy" => 0.08f,
            "Torrential" => 0.18f,
            _ => 0.0f,
        };

        Debug.Log("Rain Mod: " + rainMod);
        return 1 - rainMod;
    }
    public float RainModCover()
    {
        var rainModCover = weather.CurrentRain switch
        {
            "Zero" or "Light" => 0.0f,
            "Moderate" => 0.02f,
            "Heavy" => 0.08f,
            "Torrential" => 0.18f,
            _ => 0.0f,
        };

        Debug.Log("Rain Mod Cover: " + rainModCover);
        return 1 - rainModCover;
    }
    public float WindMod(Soldier target)
    {
        Vector2 shotLine = new(target.X - activeSoldier.X, target.Y - activeSoldier.Y);
        shotLine.Normalize();
        Vector2 windLine = weather.CurrentWindDirection;
        float shotAngleRelativeToWind = Vector2.Angle(shotLine, windLine);
        Debug.Log("WIND: " + windLine + " SHOT: " + shotLine + "ANGLE: " + shotAngleRelativeToWind);

        float windMod;

        string windSpeed = weather.CurrentWindSpeed;
        if (activeSoldier.IsCalculator())
            windSpeed = weather.DecreasedWindspeed(windSpeed);
        if (target.IsCalculator())
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

        Debug.Log("Wind Mod: " + windMod);
        return 1 - windMod;
    }
    public float WindModCover(Vector3 cover)
    {
        Vector2 shotLine = new(cover.x - activeSoldier.X, cover.y - activeSoldier.Y);
        shotLine.Normalize();
        Vector2 windLine = weather.CurrentWindDirection;
        float shotAngleRelativeToWind = Vector2.Angle(shotLine, windLine);
        Debug.Log("WIND: " + windLine + " SHOT: " + shotLine + "ANGLE: " + shotAngleRelativeToWind);

        float windModCover;
        if (shotAngleRelativeToWind <= 22.5 || shotAngleRelativeToWind >= 157.5)
            windModCover = 0f;
        else if (shotAngleRelativeToWind >= 67.5 && shotAngleRelativeToWind <= 112.5)
        {
            if (weather.CurrentWindSpeed == "Strong")
                windModCover = 0.29f;
            else if (weather.CurrentWindSpeed == "Moderate")
                windModCover = 0.12f;
            else if (weather.CurrentWindSpeed == "Light")
                windModCover = 0.06f;
            else
                windModCover = 0f;
        }
        else
        {
            if (weather.CurrentWindSpeed == "Strong")
                windModCover = 0.10f;
            else if (weather.CurrentWindSpeed == "Moderate")
                windModCover = 0.06f;
            else if (weather.CurrentWindSpeed == "Light")
                windModCover = 0.02f;
            else
                windModCover = 0f;
        }

        Debug.Log("Wind Mod Cover: " + windModCover);
        return 1 - windModCover;
    }
    public float ShooterHealthMod()
    {
        float shooterHealthMod;
        if (activeSoldier.state.Contains("Last Stand"))
            shooterHealthMod = 0.6f;
        else if (activeSoldier.hp <= activeSoldier.stats.H.Val / 2)
            shooterHealthMod = 0.16f;
        else if (activeSoldier.hp < activeSoldier.stats.H.Val)
            shooterHealthMod = 0.06f;
        else
            shooterHealthMod = 0f;

        Debug.Log("Shooter Health Mod: " + shooterHealthMod);
        return 1 - shooterHealthMod;
    }
    public float TargetHealthMod(Soldier target)
    {
        float targetHealthMod;
        if (target.state.Contains("Last Stand"))
            targetHealthMod = -0.4f;
        else if (target.hp <= target.stats.H.Val / 2)
            targetHealthMod = -0.14f;
        else if (target.hp < target.stats.H.Val)
            targetHealthMod = -0.04f;
        else
            targetHealthMod = 0f;

        Debug.Log("Target Health Mod: " + targetHealthMod);
        return 1 - targetHealthMod;
    }
    public float ShooterTerrainMod()
    {
        float shooterTerrainMod;
        if (activeSoldier.IsOnNativeTerrain())
            shooterTerrainMod = -0.04f;
        else if (activeSoldier.IsOnOppositeTerrain())
            shooterTerrainMod = 0.12f;
        else
            shooterTerrainMod = 0.06f;

        Debug.Log("Shooter Terrain Mod: " + shooterTerrainMod);
        return 1 - shooterTerrainMod;
    }
    public float TargetTerrainMod(Soldier target)
    {
        float targetTerrainMod;
        if (target.IsOnNativeTerrain())
            targetTerrainMod = 0.16f;
        else if (target.IsOnOppositeTerrain())
            targetTerrainMod = -0.08f;
        else
            targetTerrainMod = -0.02f;

        Debug.Log("Target Terrain Mod: " + targetTerrainMod);
        return 1 - targetTerrainMod;
    }
    public float ElevationMod(Soldier target)
    {
        float elevationMod = (target.Z - activeSoldier.Z) * 0.01f;

        Debug.Log("Elevation Mod: " + elevationMod);
        return 1 - elevationMod;
    }
    public float ElevationModCover(Vector3 cover)
    {
        float elevationModCover = (cover.z - activeSoldier.Z) * 0.01f;

        Debug.Log("Elevation Mod Cover: " + elevationModCover);
        return 1 - elevationModCover;
    }
    public float KdMod()
    {
        float kdMod;
        int kd = activeSoldier.GetKd();

        if (kd != 0)
            kdMod = -(2 * kd * 0.01f);
        else
            kdMod = 0;

        Debug.Log("KD Mod: " + kdMod);
        return 1 - kdMod;
    }
    public float OverwatchMod()
    {
        float overwatchMod;
        if (activeSoldier.IsOnOverwatch())
            overwatchMod = 0.4f;
        else
            overwatchMod = 0;

        Debug.Log("Overwatch Mod: " + overwatchMod);
        return 1 - overwatchMod;
    }
    public float FlankingMod(Soldier target)
    {
        //clear the flanker ui
        ClearFlankersUI(menu.flankersShotUI);
        float flankingMod;
        int flankersCount = 0;
        int flankingAngle = 80;

        if (activeSoldier.IsTactician())
            flankingAngle = 20;

        Debug.Log("Flanking Angle: " + flankingAngle);
        if (!target.IsTactician())
        {
            Vector2 shotLine = new(target.X - activeSoldier.X, target.Y - activeSoldier.Y);
            shotLine.Normalize();
            Debug.Log("shotLine: " + shotLine);

            Vector2 referenceLine = shotLine;
            foreach (Soldier s in AllSoldiers())
            {
                Debug.Log("name: " + s.soldierName + "can see: " + s.IsAbleToSee() + "same team: " + s.IsSameTeamAs(activeSoldier) + "can see in own right: " + s.CanSeeInOwnRight(target));
                if (s.IsAbleToSee() && s.IsSameTeamAs(activeSoldier) && s.CanSeeInOwnRight(target))
                {
                    Vector2 flankLine = new(target.X - s.X, target.Y - s.Y);
                    flankLine.Normalize();
                    Debug.Log("flankline: " + flankLine);
                    if (Vector2.Angle(referenceLine, flankLine) > flankingAngle && Vector2.Angle(shotLine, flankLine) > flankingAngle && flankersCount < 3)
                    {
                        flankersCount++;
                        referenceLine = flankLine;
                        Debug.Log(s.soldierName + " is flanking at angle: " + Vector2.Angle(shotLine, flankLine));

                        //add flanker to ui to visualise
                        GameObject flankerPortrait = Instantiate(menu.soldierPortraitPrefab, menu.flankersShotUI.transform.Find("FlankersPanel"));
                        flankerPortrait.GetComponent<SoldierPortrait>().Init(s);
                    }
                }
            }

            flankingMod = flankersCount switch
            {
                1 => -0.2f,
                2 => -0.5f,
                3 => -1.0f,
                _ => 0f,
            };

            //display flankers if there are any
            if (flankersCount > 0)
                menu.OpenFlankersUI(menu.flankersShotUI);
        }
        else
            flankingMod = 0;
        
        Debug.Log("Flanking Mod: " + flankingMod);
        return 1 - flankingMod;
    }
    public float StealthMod()
    {
        float stealthMod;
        if (!activeSoldier.IsRevealed())
            stealthMod = -0.4f;
        else
            stealthMod = 0;

        Debug.Log("Stealth Mod: " + stealthMod);
        return 1 - stealthMod;
    }
    public float ShooterSuppressionMod(bool suppressionCheck)
    {
        float suppressionMod = 0;
        
        if (!suppressionCheck)
             suppressionMod = activeSoldier.suppressionValue;

        Debug.Log("Suppression Mod: " + suppressionMod);
        return suppressionMod;
    }
    public int CalculateHitPercentage(bool suppressionCheck)
    {
        int hitChance;
        Item gun = itemManager.FindItemById(gunTypeDropdown.options[gunTypeDropdown.value].text);
        Soldier target = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);

        //if it's a normal shot
        if (shotTypeDropdown.value == 0)
        {
            hitChance = Mathf.RoundToInt(((WeaponHitChance(target, gun) + 10 * RelevantWeaponSkill(gun) - 12 * TargetEvasion(target)) * CoverMod() * VisMod() * RainMod(target) * WindMod(target) * ShooterHealthMod() * TargetHealthMod(target) * ShooterTerrainMod() * TargetTerrainMod(target) * ElevationMod(target) * KdMod() * OverwatchMod() * FlankingMod(target) * StealthMod()) - ShooterSuppressionMod(suppressionCheck));
            if (hitChance < 0)
                hitChance = 0;
        }
        else
        {
            if (int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("XPos").GetComponent<TMP_InputField>().text, out x) && int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("YPos").GetComponent<TMP_InputField>().text, out y) && int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("ZPos").GetComponent<TMP_InputField>().text, out z))
            {
                Vector3 coverPosition = new(x, y, z);
                hitChance = Mathf.RoundToInt(((WeaponHitChanceCover(coverPosition, gun) + 10 * RelevantWeaponSkill(gun)) * VisMod() * RainModCover() * WindModCover(coverPosition) * ShooterHealthMod() * ShooterTerrainMod() * ElevationModCover(coverPosition)) - ShooterSuppressionMod(suppressionCheck));
            }
            else
                hitChance = 0;
        }


        return hitChance;
    }
    public int CalculateCritPercentage(int hitChance)
    {
        int critChance;
        Item gun = itemManager.FindItemById(gunTypeDropdown.options[gunTypeDropdown.value].text);
        Soldier target = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);

        if (shotTypeDropdown.value == 0)
        {
            critChance = Mathf.RoundToInt((Mathf.Pow(RelevantWeaponSkill(gun), 2) * (hitChance / 100.0f)) - TargetEvasion(target));
            if (critChance < 0)
                critChance = 0;
        }
        else
        {
            if (int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("XPos").GetComponent<TMP_InputField>().text, out x) && int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("YPos").GetComponent<TMP_InputField>().text, out y) && int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("ZPos").GetComponent<TMP_InputField>().text, out z))
                critChance = Mathf.RoundToInt(Mathf.Pow(RelevantWeaponSkill(gun), 2) * (hitChance / 100.0f));
            else
                critChance = 0;
        }

        return critChance;
    }
    public void UpdateHitPercentage()
    {
        int suppressedHitChance = CalculateHitPercentage(false);
        int hitChance = CalculateHitPercentage(true);
        int critChance = CalculateCritPercentage(hitChance);

        menu.shotUI.transform.Find("TargetPanel").Find("SuppressedHitChance").Find("SuppressedHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = suppressedHitChance.ToString() + "%";
        menu.shotUI.transform.Find("TargetPanel").Find("HitChance").Find("HitChanceDisplay").GetComponent<TextMeshProUGUI>().text = hitChance.ToString() + "%";
        menu.shotUI.transform.Find("TargetPanel").Find("CritHitChance").Find("CritHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = critChance.ToString() + "%";
    }
    public void ConfirmShot()
    {
        if (int.TryParse(menu.shotUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap))
        {
            if (CheckAP(ap))
            {
                //deduct ap for aiming
                DeductAP(ap  - 1);

                Item gun = itemManager.FindItemById(gunTypeDropdown.options[gunTypeDropdown.value].text);
                Soldier target = soldierManager.FindSoldierByName(targetDropdown.options[targetDropdown.value].text);
                bool resistSuppression = activeSoldier.SuppressionCheck();

                //check for ammo
                if (gun.CheckAnyAmmo())
                {
                    //deduct ap for the shot
                    DeductAP(1);

                    //standard shot proceeds
                    if (shotTypeDropdown.value == 0)
                    {
                        gun.SpendSingleAmmo();
                        
                        int randNum1 = RandomNumber(0, 100);
                        int randNum2 = RandomNumber(0, 100);
                        int hitChance = CalculateHitPercentage(resistSuppression);
                        int critChance = CalculateCritPercentage(hitChance);

                        //display suppression indicator
                        if (activeSoldier.IsSuppressed())
                        {
                            menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(true);

                            if (resistSuppression)
                                menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Resisted Suppression</color>";
                            else
                                menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=orange>Suffered Suppression</color>";
                        }
                        else
                            menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(false);

                        //standard shot hits
                        if (randNum1 <= hitChance)
                        {
                            //standard shot crit hits
                            if (randNum2 <= critChance)
                            {
                                target.TakeDamage(activeSoldier, gun.gunCritDamage, false, new List<string>() { "Critical", "Shot" });
                                menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> CRITICAL HIT </color>";
                                menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                                //paying xp for hit
                                if (hitChance >= 10)
                                    menu.AddXpAlert(activeSoldier, 8, "Critical shot on " + target.soldierName + "!", false);
                                else
                                    menu.AddXpAlert(activeSoldier, 10, "Critical shot with a " + hitChance + "% chance on " + target.soldierName + "!", false);
                            }
                            else
                            {
                                target.TakeDamage(activeSoldier, gun.gunDamage, false, new List<string>() { "Shot" });
                                menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Hit </color>";
                                menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                                //paying xp for hit
                                if (hitChance >= 10)
                                    menu.AddXpAlert(activeSoldier, 2, "Shot hit on " + target.soldierName + ".", false);
                                else
                                    menu.AddXpAlert(activeSoldier, 10, "Shot hit with a " + hitChance + "% chance on " + target.soldierName + "!", false);
                            }

                            //don't show los check button if shot hits
                            menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(false);
                        }
                        else
                        {
                            menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                            menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {RandomShotScatterDistance()}cm {RandomShotScatterHorizontal()}, {RandomShotScatterDistance()}cm {RandomShotScatterVertical()}. Resolve hits as damage event ({gun.gunDamage}), or cover hit {gun.DisplayGunCoverDamage()}.";
                            //show los check button if shot misses
                            menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                            //paying xp for dodge
                            if (hitChance <= 90)
                                menu.AddXpAlert(target, 1, "Dodged shot from " + activeSoldier.soldierName + ".", false);
                            else
                                menu.AddXpAlert(target, 10, "Dodged shot with a " + hitChance + "% chance from " + activeSoldier.soldierName + "!", false);

                            //push the no damage attack through for witness trigger
                            target.TakeDamage(activeSoldier, 0, true, new List<string>() { "Shot" });
                        }

                        //trigger loud action
                        activeSoldier.PerformLoudAction();

                        menu.OpenShotResultUI();
                        menu.CloseShotUI();
                    }
                    else if (shotTypeDropdown.value == 1)
                    {
                        gun.SpendSpecificAmmo(gun.gunSuppressionDrain, true);

                        int suppressionValue = CalculateRangeBracket(CalculateRange(activeSoldier, target)) switch
                        {
                            "Melee" or "CQB" => gun.gunCQBSuppressionPenalty,
                            "Short" => gun.gunShortSuppressionPenalty,
                            "Medium" => gun.gunMedSuppressionPenalty,
                            "Long" or "Coriolis" => gun.gunLongSuppressionPenalty,
                            _ => 0,
                        };
                        target.SetSuppression(suppressionValue);
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Supressed (" + target.suppressionValue + ")</color>";

                        //trigger loud action
                        activeSoldier.PerformLoudAction();

                        menu.OpenShotResultUI();
                        menu.CloseShotUI();
                    }
                    else
                    {
                        if (int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("XPos").GetComponent<TMP_InputField>().text, out x) && int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("YPos").GetComponent<TMP_InputField>().text, out y) && int.TryParse(menu.shotUI.transform.Find("TargetPanel").Find("CoverLocation").Find("ZPos").GetComponent<TMP_InputField>().text, out z))
                        {
                            if (x >= 1 && x <= maxX && y >= 1 && y <= maxY && z >= 0 && z <= maxZ)
                            {
                                gun.SpendSingleAmmo();

                                int randNum1 = RandomNumber(0, 100);
                                int randNum2 = RandomNumber(0, 100);
                                int hitChance = CalculateHitPercentage(resistSuppression);
                                int critChance = CalculateCritPercentage(hitChance);
                                int coverDamage = CalculateRangeBracket(CalculateRangeCover(activeSoldier, new Vector3(x, y, z))) switch
                                {
                                    "Melee" or "CQB" => gun.gunCQBCoverDamage,
                                    "Short" => gun.gunShortCoverDamage,
                                    "Medium" => gun.gunMedCoverDamage,
                                    "Long" or "Coriolis" => gun.gunLongCoverDamage,
                                    _ => 0,
                                };

                                //display suppression indicator
                                if (activeSoldier.IsSuppressed())
                                {
                                    menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(true);

                                    if (resistSuppression)
                                        menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Resisted Suppression</color>";
                                    else
                                        menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=orange>Suffered Suppression</color>";
                                }
                                else
                                    menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(false);

                                //show los check button
                                menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                                //standard shot hits cover
                                if (randNum1 <= hitChance)
                                {
                                    //critical shot hits cover
                                    if (randNum2 <= critChance)
                                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> COVER DESTROYED </color>";
                                    else
                                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Cover hit (" + coverDamage + " damage)</color>";

                                }
                                else
                                {
                                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {RandomShotScatterDistance()}cm {RandomShotScatterHorizontal()}, {RandomShotScatterDistance()}cm {RandomShotScatterVertical()}. Resolve any hits as damage event ({gun.gunDamage}), or cover hit {gun.DisplayGunCoverDamage()}.";
                                }

                                //trigger loud action
                                activeSoldier.PerformLoudAction();

                                menu.OpenShotResultUI();
                                menu.CloseShotUI();
                            }
                            else
                                Debug.Log("x, y, z values must not be out of bounds.");
                        }
                        else
                            Debug.Log("Formatting was wrong, try again.");
                    }
                }
                else
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Gun is Empty";

                    menu.OpenShotResultUI();
                    menu.CloseShotUI();
                }
            }
        }  
    }










    //cross functional shot functions melee functions
    public void ClearFlankersUI(GameObject flankersUI)
    {
        flankersUI.SetActive(false);
        foreach (Transform child in flankersUI.transform.Find("FlankersPanel"))
            Destroy(child.gameObject);
    }











    //melee functions
    public void UpdateMeleeUI()
    {
        if (!menu.clearMeleeFlag)
        {
            UpdateMeleeAP();
            UpdateMeleeDamage();
        }
    }
    public void UpdateMeleeDamage()
    {
        int meleeDamage = CalculateMeleeResult();

        menu.meleeUI.transform.Find("TargetPanel").Find("MeleeDamage").Find("MeleeDamageDisplay").GetComponent<TextMeshProUGUI>().text = meleeDamage.ToString();
    }
    public void UpdateMeleeAP()
    {
        if (meleeTypeDropdown.options[meleeTypeDropdown.value].text == "Static Attack")
        {
            if (activeSoldier.IsFighter())
                menu.meleeUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "1";
            else
                menu.meleeUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "2";
        }
        else
            menu.meleeUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = "0";
    }
    public void UpdateMeleeTypeOptions()
    {
        Soldier defender = soldierManager.FindSoldierByName(meleeTargetDropdown.options[meleeTargetDropdown.value].text);
        List<TMP_Dropdown.OptionData> meleeTypeDetails = new()
        {
            new TMP_Dropdown.OptionData(menu.meleeChargeIndicator),
            new TMP_Dropdown.OptionData("Engagement Only"),
        };

        if (defender.controlledBySoldiersList.Contains(activeSoldier.id))
            meleeTypeDetails.Add(new TMP_Dropdown.OptionData("<color=green>Disengage</color>"));
        else if (defender.controllingSoldiersList.Contains(activeSoldier.id))
            meleeTypeDetails.Add(new TMP_Dropdown.OptionData("<color=red>Request Disengage</color>"));

        meleeTypeDropdown.ClearOptions();
        meleeTypeDropdown.AddOptions(meleeTypeDetails);
    }
    public float AttackerMeleeSkill()
    {
        float attackerMeleeSkill = activeSoldier.stats.M.Val;

        //apply JA debuff
        if (activeSoldier.inventory.IsWearingJuggernautArmour())
            attackerMeleeSkill--;

        //check for inspirer
        attackerMeleeSkill += activeSoldier.InspirerBonusWeaponMelee();

        //check for starve penalties
        if (activeSoldier.roundsWithoutFood >= 20)
            attackerMeleeSkill = Mathf.RoundToInt(attackerMeleeSkill * 0.75f);

        //correct negatives
        if (attackerMeleeSkill < 0)
            attackerMeleeSkill = 0;

        Debug.Log("Attacker M skill: " + attackerMeleeSkill);
        return attackerMeleeSkill;
    }
    public int AttackerWeaponDamage(Item weapon)
    {
        int attackerWeaponDamage;

        //if fist selected
        if (weapon == null)
            attackerWeaponDamage = 1;
        else
            attackerWeaponDamage = weapon.meleeDamage;

        Debug.Log("Attacker weapon damage: " + attackerWeaponDamage);
        return attackerWeaponDamage;
    }
    public float AttackerHealthMod()
    {
        float attackerHealthMod;
        if (activeSoldier.state.Contains("Last Stand"))
            attackerHealthMod = 0.6f;
        else if (activeSoldier.hp <= activeSoldier.stats.H.Val / 2)
            attackerHealthMod = 0.20f;
        else if (activeSoldier.hp < activeSoldier.stats.H.Val)
            attackerHealthMod = 0.06f;
        else
            attackerHealthMod = 0f;

        Debug.Log("Attacker Health Mod: " + attackerHealthMod);
        return 1 - attackerHealthMod;
    }
    public float AttackerTerrainMod()
    {
        float attackerTerrainMod;
        if (activeSoldier.IsOnNativeTerrain())
            attackerTerrainMod = -0.4f;
        else if (activeSoldier.IsOnOppositeTerrain())
            attackerTerrainMod = 0.2f;
        else
            attackerTerrainMod = 0f;

        Debug.Log("Attacker Terrain Mod: " + attackerTerrainMod);
        return 1 - attackerTerrainMod;
    }
    public float FlankingAgainstAttackerMod(Soldier defender)
    {
        //clear the flanker ui
        ClearFlankersUI(menu.flankersMeleeAttackerUI);

        float attackerFlankingMod;
        int flankersCount = 0;

        foreach (Soldier s in AllSoldiers())
        {
            if (s.IsAbleToSee() && s.IsOppositeTeamAs(activeSoldier) && s != defender && CalculateRange(s, activeSoldier) <= 4 && flankersCount < 3)
            {
                flankersCount++;

                //add flanker to ui to visualise
                GameObject flankerPortrait = Instantiate(menu.soldierPortraitPrefab, menu.flankersMeleeAttackerUI.transform.Find("FlankersPanel"));
                flankerPortrait.GetComponent<SoldierPortrait>().Init(s);
            }
        }

        attackerFlankingMod = flankersCount switch
        {
            1 => 0.16f,
            2 => 0.46f,
            3 => 0.8f,
            _ => 0f,
        };

        //display flankers if there are any
        if (flankersCount > 0)
            menu.OpenFlankersUI(menu.flankersMeleeAttackerUI);

        Debug.Log("Attacker flanking Mod: " + attackerFlankingMod);
        return 1 - attackerFlankingMod;
    }
    public float AttackerStrengthMod()
    {
        float strengthMod = activeSoldier.stats.Str.Val;
        strengthMod *= 0.2f;

        Debug.Log("Attacker Str bonus: " + strengthMod);
        return strengthMod;
    }
    public int DefenderMeleeSkill(Soldier defender)
    {
        int defenderMeleeSkill = defender.stats.M.Val;

        //check for starve penalties
        if (defender.roundsWithoutFood >= 20)
            defenderMeleeSkill = Mathf.RoundToInt(defenderMeleeSkill * 0.75f);

        Debug.Log("Defender M skill: " + defenderMeleeSkill);
        return defenderMeleeSkill;
    }
    public int DefenderWeaponDamage(Item weapon)
    {
        int defenderWeaponDamage;

        //if fist selected
        if (weapon == null)
            defenderWeaponDamage = 1;
        else
            defenderWeaponDamage = weapon.meleeDamage;

        Debug.Log("Defender weapon damage: " + defenderWeaponDamage);
        return defenderWeaponDamage;
    }
    public float ChargeModifier()
    {
        float chargeMod;

        chargeMod = meleeTypeDropdown.options[meleeTypeDropdown.value].text switch
        {
            "Full Charge Attack" => 1.9f,
            "Half Charge Attack" => 1.4f,
            "3cm Charge Attack" => 1.1f,
            "Static Attack" or _ => 0f,
        };

        Debug.Log("Charge mod: " + chargeMod);
        return chargeMod;
    }
    public float DefenderHealthMod(Soldier defender)
    {
        float defenderHealthMod;
        if (defender.state.Contains("Last Stand"))
            defenderHealthMod = 0.8f;
        else if (defender.hp <= activeSoldier.stats.H.Val / 2)
            defenderHealthMod = 0.4f;
        else if (defender.hp < activeSoldier.stats.H.Val)
            defenderHealthMod = 0.2f;
        else
            defenderHealthMod = 0f;

        Debug.Log("Defender Health Mod: " + defenderHealthMod);
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

        Debug.Log("Defender Terrain Mod: " + defenderTerrainMod);
        return 1 - defenderTerrainMod;
    }
    public float FlankingAgainstDefenderMod(Soldier defender)
    {
        //clear the flanker ui
        ClearFlankersUI(menu.flankersMeleeDefenderUI);
        float defenderFlankingMod;
        int flankersCount = 0;

        if (!defender.IsTactician())
        {
            foreach (Soldier s in AllSoldiers())
            {
                if (s.IsAbleToSee() && s.IsOppositeTeamAs(defender) && s != activeSoldier && CalculateRange(s, defender) <= 4 && flankersCount < 3)
                {
                    flankersCount++;

                    //add flanker to ui to visualise
                    GameObject flankerPortrait = Instantiate(menu.soldierPortraitPrefab, menu.flankersMeleeDefenderUI.transform.Find("FlankersPanel"));
                    flankerPortrait.GetComponent<SoldierPortrait>().Init(s);
                }
            }

            defenderFlankingMod = flankersCount switch
            {
                1 => 0.26f,
                2 => 0.56f,
                3 => 0.86f,
                _ => 0f,
            };

            //display flankers if there are any
            if (flankersCount > 0)
                menu.OpenFlankersUI(menu.flankersMeleeDefenderUI);
        }
        else
            defenderFlankingMod = 0;

        Debug.Log("Defender flanking Mod: " + defenderFlankingMod);
        return 1 - defenderFlankingMod;
    }
    public float SuppressionMod(Soldier defender)
    {
        int attackerSuppression = activeSoldier.suppressionValue, defenderSuppression = defender.suppressionValue;
        float suppressionMod = 1 - ((100 - attackerSuppression) / 100f) * ((100 - defenderSuppression) / 100f);

        Debug.Log("Suppression Mod: " + suppressionMod);
        return 1 - suppressionMod;
    }
    public float DefenderStrengthMod(Soldier defender)
    {
        float strengthMod = defender.stats.Str.Val;
        strengthMod *= 0.2f;

        Debug.Log("Defender Str bonus: " + strengthMod);
        return strengthMod;
    }
    public int CalculateMeleeResult()
    {
        float meleeDamage;
        int meleeDamageFinal;
        Item attackerWeapon = itemManager.FindItemById(attackerWeaponDropdown.options[attackerWeaponDropdown.value].text);
        Item defenderWeapon = itemManager.FindItemById(defenderWeaponDropdown.options[defenderWeaponDropdown.value].text);
        Soldier defender = soldierManager.FindSoldierByName(meleeTargetDropdown.options[meleeTargetDropdown.value].text);

        //if it's a normal attack
        if (meleeTypeDropdown.value == 0)
        {
            meleeDamage = ((AttackerMeleeSkill() + AttackerWeaponDamage(attackerWeapon)) * AttackerHealthMod() * AttackerTerrainMod() * KdMod() * FlankingAgainstAttackerMod(defender) * SuppressionMod(defender) + AttackerStrengthMod()) - ((DefenderMeleeSkill(defender) + DefenderWeaponDamage(defenderWeapon) + ChargeModifier()) * DefenderHealthMod(defender) * DefenderTerrainMod(defender) * FlankingAgainstDefenderMod(defender) * SuppressionMod(defender) + DefenderStrengthMod(defender));
            Debug.Log("Raw Melee Damage: " + meleeDamage);
            //rounding based on R
            if (activeSoldier.stats.R.Val > defender.stats.R.Val)
                meleeDamageFinal = Mathf.CeilToInt(meleeDamage);
            else if (activeSoldier.stats.R.Val < defender.stats.R.Val)
                meleeDamageFinal = Mathf.FloorToInt(meleeDamage);
            else
                meleeDamageFinal = Mathf.RoundToInt(meleeDamage);
        }
        else
            meleeDamageFinal = 0;

        float attackerBase = AttackerMeleeSkill() + AttackerWeaponDamage(attackerWeapon), attackerMods = AttackerHealthMod() * AttackerTerrainMod() * KdMod() * FlankingAgainstAttackerMod(defender), attackerStrengthMod = AttackerStrengthMod(), defenderBase = DefenderMeleeSkill(defender) + DefenderWeaponDamage(defenderWeapon) + ChargeModifier(), defenderMods = DefenderHealthMod(defender) * DefenderTerrainMod(defender) * FlankingAgainstDefenderMod(defender), defenderStrengthMod = DefenderStrengthMod(defender);
        Debug.Log("Attacker Base(" + attackerBase + ") * AttackerMods(" + attackerMods + ") + AttackerStrengthMod(" + attackerStrengthMod + ")\nDefenderBase(" + defenderBase + ") * DefenderMods(" + defenderMods + ") + DefenderStrengthMod(" + defenderStrengthMod + ")");


        return meleeDamageFinal;
    }
    public void EstablishController(Soldier controller, Soldier s2)
    {
        BreakMeleeEngagement(controller, s2);
        controller.controllingSoldiersList.Add(s2.id);
        controller.controlledBySoldiersList.Remove(s2.id);
        s2.controlledBySoldiersList.Add(controller.id);
        s2.controllingSoldiersList.Remove(controller.id);

        menu.OpenMeleeResultUI();
    }
    public void EstablishNoController(Soldier s1, Soldier s2)
    {
        BreakMeleeEngagement(s1, s2);

        menu.OpenMeleeResultUI();
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
        else if (!defender.IsConscious())
        {
            EstablishNoController(attacker, defender);
            controlResult = "<color=blue>No-one Controlling\n(" + defender.soldierName + " Unconscious)</color>";
        }
        else if (!attacker.IsConscious())
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
                        EstablishNoController(attacker, defender);
                        controlResult = "<color=orange>No-one Controlling\n(Evenly Matched)</color>";
                    }
                }
            }
        }
        return controlResult;
    }
    public void BreakAllMeleeEngagements(Soldier s1)
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
        if (int.TryParse(menu.meleeUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap))
        {
            if (CheckAP(ap))
            {
                menu.SetMeleeResolvedFlagTo(false);
                DeductAP(ap);

                Soldier defender = soldierManager.FindSoldierByName(meleeTargetDropdown.options[meleeTargetDropdown.value].text);
                int meleeDamage = CalculateMeleeResult();
                string damageMessage;
                bool counterattack = false, instantKill = false, loudAction = true, disengage = false;

                //engagement only options
                if (meleeTypeDropdown.value == 1)
                {
                    damageMessage = "<color=orange>No Damage\n(Enagament Only)</color>";
                    //loudAction = false;
                }
                else if (meleeTypeDropdown.value == 2)
                {
                    damageMessage = "<color=orange>No Damage\n(Disengagement)</color>";
                    disengage = true;
                    //loudAction = false;
                }
                else
                {
                    //instant kill scenarios
                    if (!activeSoldier.IsRevealed() && activeSoldier.stats.F.Val > defender.stats.M.Val) //stealth kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Stealth Attack)</color>";
                        instantKill = true;
                        loudAction = false;
                    }
                    else if (defender.IsOnOverwatch() && activeSoldier.stats.M.Val >= defender.stats.M.Val) //overwatch kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Overwatcher)</color>";
                        instantKill = true;
                    }
                    else if (!defender.IsConscious()) //unconscious kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Unconscious)</color>";
                        instantKill = true;
                    }
                    else if (defender.IsPlayingDead()) //playdead kill
                    {
                        damageMessage = "<color=green>INSTANT KILL\n(Playdead)</color>";
                        instantKill = true;
                    }
                    else
                    {
                        //melee attack proceeds
                        if (meleeDamage > 0)
                        {
                            if (activeSoldier.inventory.IsWearingExoArmour() && !defender.inventory.IsWearingJuggernautArmour()) //exo kill on standard man
                            {
                                damageMessage = "<color=green>INSTANT KILL\n(Exo Armour)</color>";
                                instantKill = true;
                            }
                            else
                            {
                                if (activeSoldier.IsBloodRaged())
                                    meleeDamage *= 2;

                                if (defender.inventory.IsWearingJuggernautArmour() && !activeSoldier.inventory.IsWearingExoArmour())
                                    damageMessage = "<color=orange>No Damage\n(Juggernaut Immune)</color>";
                                else
                                    damageMessage = "<color=green>Successful Attack\n(" + meleeDamage + " Damage)</color>";
                                defender.TakeDamage(activeSoldier, meleeDamage, false, new List<string>() { "Melee" });
                                activeSoldier.FighterMeleeHitReward();
                            }
                        }
                        else if (meleeDamage < 0)
                        {
                            //play counterattack sound
                            soundManager.PlayCounterattack();

                            if (activeSoldier.inventory.IsWearingExoArmour() && !defender.inventory.IsWearingJuggernautArmour()) //exo counter kill on standard man
                            {
                                damageMessage = "<color=green>INSTANT KILL\n(Exo Armour)</color>";
                                instantKill = true;
                            }
                            else
                            {
                                counterattack = true;
                                meleeDamage *= -1;
                                damageMessage = "<color=red>Counterattacked\n(" + meleeDamage + " Damage)</color>";
                                activeSoldier.TakeDamage(defender, meleeDamage, false, new List<string>() { "Melee" });
                            }
                        }
                        else
                        {
                            damageMessage = "<color=orange>No Damage\n(Evenly Matched)</color>";

                            //push the no damage attack through for witness trigger
                            defender.TakeDamage(activeSoldier, meleeDamage, true, new List<string>() { "Melee" });
                        }
                    }
                    
                    //reset blood rage even if non-successful attack
                    activeSoldier.UnsetBloodRage();
                }

                //add xp for successful melee attack
                if (meleeTypeDropdown.value == 0 && !damageMessage.Contains("No Damage"))
                {
                    if (counterattack)
                        menu.AddXpAlert(defender, 2 + meleeDamage, "Melee counterattack attack on " + activeSoldier.soldierName + " for " + meleeDamage + " damage.", false);
                    else
                        menu.AddXpAlert(activeSoldier, 1, "Successful melee attack on " + defender.soldierName + ".", false);
                }

                //kill if instantKill
                if (instantKill)
                    defender.InstantKill(activeSoldier, new List<string>() { "Melee" });

                //attacker and defender exit cover
                activeSoldier.UnsetCover();
                defender.UnsetCover();

                //attacker and defender exit overwatch
                activeSoldier.UnsetOverwatch();
                defender.UnsetOverwatch();

                //add melee alert
                menu.AddMeleeAlert(activeSoldier, defender, damageMessage, DetermineMeleeController(activeSoldier, defender, counterattack, disengage));

                //trigger loud action
                if (loudAction)
                    activeSoldier.PerformLoudAction();

                menu.OpenMeleeResultUI();
                menu.CloseMeleeUI();
            }
        }
    }
    public IEnumerator DetermineMeleeControllerMultiple(Soldier s1)
    {
        List<string> engagedSoldiersList = new();
        foreach (string soldierId in s1.controlledBySoldiersList)
            engagedSoldiersList.Add(soldierId);
        foreach (string soldierId in s1.controllingSoldiersList)
            engagedSoldiersList.Add(soldierId);

        if (engagedSoldiersList.Count > 0)
        {
            menu.SetMeleeResolvedFlagTo(false);
            yield return new WaitForSeconds(0.05f);
            foreach (string soldierId in engagedSoldiersList)
                menu.AddMeleeAlert(s1, soldierManager.FindSoldierById(soldierId), "No Damage\n(Engagement Change)", DetermineMeleeController(s1, soldierManager.FindSoldierById(soldierId), false, false));
        }
    }














    //configure functions
    public void UpdateConfigureAP()
    {
        int totalPickup = 0, totalDrop = 0, totalSwap = 0, ap = 0;

        foreach (Transform child in groundItemsContentUI)
            if (child.GetComponent<ItemIcon>().pickupNumber == 0)
                totalPickup++;

        foreach (Transform allyButton in allyButtonContentUI)
            foreach (Transform itemIcon in allyButton.GetComponent<AllyItemsButton>().linkedItemPanel.transform.Find("Viewport").Find("AllyInventoryContent"))
                if (itemIcon.GetComponent<ItemIcon>().pickupNumber == 0)
                    totalSwap++;

        foreach (Transform child in allItemsContentUI)
            if (child.GetComponent<ItemIcon>().pickupNumber > 0)
                totalPickup += child.GetComponent<ItemIcon>().pickupNumber;

        foreach (Transform child in inventoryItemsContentUI)
            if (child.GetComponent<ItemIcon>().pickupNumber == 0)
            {
                if (child.GetComponent<ItemIcon>().destination == null)
                    totalDrop++;
                else
                    totalSwap++;
            }

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

        menu.configureUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text = ap.ToString();
    }
    public List<Item> FindNearbyItems()
    {
        List<Item> nearbyGroundItems = new();
        var allItems = FindObjectsOfType<Item>();

        foreach (Item i in allItems)
            if (i.transform.parent == null && activeSoldier.PhysicalObjectWithinItemRadius(i))
                nearbyGroundItems.Add(i);

        return nearbyGroundItems;
    }
    public void ConfirmConfigure()
    {
        int.TryParse(menu.configureUI.transform.Find("APCost").Find("APCostDisplay").GetComponent<TextMeshProUGUI>().text, out int ap);

        if (ap > 0 && CheckAP(ap))
        {
            DeductAP(ap);
            foreach (Transform child in allItemsContentUI)
            {
                ItemIcon itemDetails = child.GetComponent<ItemIcon>();

                for (int i = 0; i < itemDetails.pickupNumber; i++)
                    activeSoldier.PickUpItem(itemManager.SpawnItem(child.gameObject.name));
            }

            foreach (Transform child in inventoryItemsContentUI)
            {
                ItemIcon itemDetails = child.GetComponent<ItemIcon>();

                if (itemDetails.pickupNumber == 0)
                {
                    if (itemDetails.destination == null)
                        activeSoldier.DropItem(itemDetails.linkedItem);
                    else
                        itemDetails.destination.GetComponent<Soldier>().PickUpItem(activeSoldier.DropItem(itemDetails.linkedItem));
                }
                    
            }

            foreach (Transform child in groundItemsContentUI)
            {
                ItemIcon itemDetails = child.GetComponent<ItemIcon>();

                if (itemDetails.pickupNumber == 0)
                    activeSoldier.PickUpItem(itemDetails.linkedItem);
            }

            foreach (Transform allyButton in allyButtonContentUI)
                foreach (Transform child in allyButton.GetComponent<AllyItemsButton>().linkedItemPanel.transform.Find("Viewport").Find("AllyInventoryContent"))
                {
                    ItemIcon itemDetails = child.GetComponent<ItemIcon>();

                    if (itemDetails.pickupNumber == 0)
                        activeSoldier.PickUpItem(itemDetails.linkedItem.owner.DropItem(itemDetails.linkedItem));
                }

            menu.CloseConfigureUI();
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
        TMP_Dropdown dipelecType = menu.dipelecUI.transform.Find("DipElecType").Find("DipElecTypeDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown level = menu.dipelecUI.transform.Find("Level").Find("LevelDropdown").GetComponent<TMP_Dropdown>();

        if (dipelecType.value == 0)
        {
            menu.dipelecUI.transform.Find("Level").gameObject.SetActive(true);
            switch (level.value)
            {
                case 0:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L1Dip;
                    break;
                case 1:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L2Dip;
                    break;
                case 2:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L3Dip;
                    break;
                case 3:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L4Dip;
                    break;
                case 4:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L5Dip;
                    break;
                case 5:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L6Dip;
                    break;
                default:
                    break;
            }
            menu.dipelecUI.transform.Find("SuccessChance").Find("SuccessChanceDisplay").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(CumulativeBinomialProbability(activeSoldier.stats.Dip.Val + activeSoldier.TacticianBonus(), level.value + 1, 0.5f, 0.5f) * 100f).ToString() + "%";
        }
        else if (dipelecType.value == 1)
        {
            menu.dipelecUI.transform.Find("Level").gameObject.SetActive(true);
            switch (level.value)
            {
                case 0:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L1Elec;
                    break;
                case 1:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L2Elec;
                    break;
                case 2:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L3Elec;
                    break;
                case 3:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L4Elec;
                    break;
                case 4:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L5Elec;
                    break;
                case 5:
                    menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = dipelec.L6Elec;
                    break;
                default:
                    break;
            }
            menu.dipelecUI.transform.Find("SuccessChance").Find("SuccessChanceDisplay").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(CumulativeBinomialProbability(activeSoldier.stats.Elec.Val + activeSoldier.CalculatorBonus(), level.value + 1, 0.5f, 0.5f) * 100f).ToString() + "%";
        }
        else
        {
            menu.dipelecUI.transform.Find("Level").gameObject.SetActive(false);
            menu.dipelecUI.transform.Find("Reward").Find("Reward").Find("RewardDisplay").GetComponent<TextMeshProUGUI>().text = "Permanently disable this terminal?\n<color=red>Warning: Penalties Apply</color>";
            menu.dipelecUI.transform.Find("SuccessChance").Find("SuccessChanceDisplay").GetComponent<TextMeshProUGUI>().text = "100%";
        }
    }
    public void ConfirmDipElec()
    {
        if (CheckAP(3))
        {
            DeductAP(3);
            bool terminalDisabled = false;
            int passCount = 0;
            string resultString = "";
            TMP_Dropdown levelDropdown = menu.dipelecUI.transform.Find("Level").Find("LevelDropdown").GetComponent<TMP_Dropdown>();

            if (menu.dipelecUI.transform.Find("DipElecType").Find("DipElecTypeDropdown").GetComponent<TMP_Dropdown>().value == 0)
            {
                for (int i = 0; i < activeSoldier.stats.Dip.Val; i++)
                {
                    if (CoinFlip())
                        passCount++;
                }
                resultString = "Negotiation";
            }
            else if (menu.dipelecUI.transform.Find("DipElecType").Find("DipElecTypeDropdown").GetComponent<TMP_Dropdown>().value == 1)
            {
                for (int i = 0; i < activeSoldier.stats.Elec.Val; i++)
                {
                    if (CoinFlip())
                        passCount++;
                }
                resultString = "Hack";
            }
            else
            {
                terminalDisabled = true;
            }

            if (!terminalDisabled)
            {
                if (passCount > levelDropdown.value)
                {
                    menu.dipelecResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Successful " + resultString + "</color>";
                    menu.AddXpAlert(activeSoldier, (int)Mathf.Pow(2, levelDropdown.value), "Successful level " + (levelDropdown.value + 1) + " " + resultString, true);
                }
                else
                    menu.dipelecResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Failed " + resultString;
            }
            else
            {
                menu.dipelecResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Terminal Diabled!";
                if (activeSoldier.hp > 4)
                    activeSoldier.TakeDamage(null, activeSoldier.hp - 3, true, new List<string>() { "Dipelec" });
            }

            menu.OpenDipelecResultUI();
            menu.CloseDipElecUI();
        }
    }
















    //AP/MP functions
    public bool CheckAP(int ap)
    {
        //check if it's on the current players turn
        if (menu.overrideView)
            return true;
        else
        {
            if (activeSoldier.ap >= ap)
                return true;
            else
            {
                notEnoughAPUI.SetActive(true);
                return false;
            }
        }
    }
    public bool CheckMP(int mp)
    {
        if (menu.overrideView)
            return true;
        else
        {
            if (activeSoldier.mp >= mp)
                return true;
            else
            {
                notEnoughMPUI.SetActive(true);
                return false;
            }
        }
    }
    public void DeductAP(int ap)
    {
        if (!menu.overrideView && ap > 0)
        {
            activeSoldier.ap -= ap;
            activeSoldier.usedAP = true;
        }
    }
    public void DeductMP(int mp)
    {
        if (!menu.overrideView)
        {
            activeSoldier.mp -= mp;
            activeSoldier.usedMP = true;
        }
    }
    public void DrainAP()
    {
        activeSoldier.ap = 0;
    }
    public void DrainMP()
    {
        activeSoldier.mp = 0;
    }

    //lastandicide functions
    public void Lastandicide()
    {
        if (CheckAP(1))
        {
            DeductAP(1);
            activeSoldier.InstantKill(activeSoldier, new List<string> { "Lastandicide" });
        }
    }

    public void CloseNotEnoughAP()
    {
        notEnoughAPUI.SetActive(false);
    }
    public void CloseNotEnoughMP()
    {
        notEnoughMPUI.SetActive(false);
    }

    //trauma functions
    public IEnumerator TraumaCheck(Soldier deadSoldier, int tp, bool commander, bool lastandicide)
    {
        Debug.Log("Trauma check start");
        //imperceptible delay to allow colliders to be recalculated at new destination
        yield return new WaitForSeconds(0.05f);
        Debug.Log("Trauma check passed wait");
        if (deadSoldier.IsDead())
        {
            int numberOfPasses;
            bool showTraumaUI = false, commanderPassFinished = false, lastandicidePassFinished = false;

            //check how many times to loop through checks
            if (commander && lastandicide)
                numberOfPasses = 3;
            else if (commander || lastandicide)
                numberOfPasses = 2;
            else
                numberOfPasses = 1;

            for (int i = 0; i < numberOfPasses; i++)
            {
                foreach (Soldier friendly in AllSoldiers())
                {
                    Debug.Log(friendly.soldierName + " trauma check attempting to run");

                    if (friendly.IsSameTeamAs(deadSoldier))
                    {
                        Debug.Log(friendly.soldierName + " trauma check actually running");
                        //desensitised
                        if (friendly.tp >= 5)
                            menu.AddTraumaAlert(friendly, tp, friendly.soldierName + " is " + friendly.GetTraumaState() + ". He is immune to trauma.", 0, 0, "");
                        else
                        {
                            //guaranteed trauma from commander death and/or lastandicide
                            if (commander)
                            {
                                menu.AddTraumaAlert(friendly, 1, "Commander died, an automatic trauma point has been accrued.", 0, 0, "");
                                commanderPassFinished = true;
                                showTraumaUI = true;
                            }
                            else if (lastandicide)
                            {
                                menu.AddTraumaAlert(friendly, 1, deadSoldier.soldierName + " committed Lastandicide, an automatic trauma point has been accrued.", 0, 0, "");
                                lastandicidePassFinished = true;
                                showTraumaUI = true;
                            }
                            else
                            {
                                //check friendlies seeing death
                                if (friendly.IsAbleToSee())
                                {
                                    int rolls = 0, xpOnResist = 0;
                                    if (friendly.PhysicalObjectWithinMaxRadius(deadSoldier))
                                    {
                                        if (friendly.IsResilient())
                                            menu.AddTraumaAlert(friendly, tp, friendly.soldierName + " has " + friendly.stats.R.Val + " Resilience. He is immune to trauma.", 0, 0, "");
                                        else
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

                                            menu.AddTraumaAlert(friendly, tp, friendly.soldierName + " is within " + range + " range of " + deadSoldier.soldierName + ". Check for LOS?", rolls, xpOnResist, range);
                                        }

                                        showTraumaUI = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (commanderPassFinished)
                    commander = false;
                if (lastandicidePassFinished)
                    lastandicide = false;
            }

            if (showTraumaUI)
            {
                yield return new WaitUntil(() => menu.meleeResolvedFlag == true);
                menu.OpenTraumaAlertUI();
            }
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
                //destroy all detection alerts for given player after done
                foreach (Transform child in traumaAlerts)
                    Destroy(child.gameObject);

                menu.CloseTraumaUI();
            }
            else
                Debug.Log("Haven't traumatised everyone.");
        }
        else
            Debug.Log("Haven't scrolled all the way to the bottom");
    }














    //damage event functions - game
    public void ConfirmDamageEvent()
    {
        if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Bloodletting"))
        {
            activeSoldier.TakeBloodlettingDamage();
            menu.CloseDamageEventUI();
        }
        else if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Other") && int.TryParse(menu.damageEventUI.transform.Find("Other").Find("OtherInput").GetComponent<TMP_InputField>().text, out int otherDamage))
        {
            activeSoldier.TakeDamage(null, otherDamage, false, new List<string>() { menu.damageEventUI.transform.Find("DamageSource").Find("DamageSourceInput").GetComponent<TMP_InputField>().text });
            menu.CloseDamageEventUI();
        }
        else
        {
            //check input formatting
            if (int.TryParse(menu.damageEventUI.transform.Find("Location").Find("XPos").GetComponent<TMP_InputField>().text, out x) && int.TryParse(menu.damageEventUI.transform.Find("Location").Find("YPos").GetComponent<TMP_InputField>().text, out y) && int.TryParse(menu.damageEventUI.transform.Find("Location").Find("ZPos").GetComponent<TMP_InputField>().text, out z) && menu.damageEventUI.transform.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value != 0)
            {
                if (x >= 1 && x <= maxX && y >= 1 && y <= maxY && z >= 0 && z <= maxZ)
                {
                    //fall actually proceeds
                    Vector3 oldPos = new(activeSoldier.X, activeSoldier.Y, activeSoldier.Z);
                    activeSoldier.X = x;
                    activeSoldier.Y = y;
                    activeSoldier.Z = z;
                    activeSoldier.TerrainOn = menu.damageEventUI.transform.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().options[menu.damageEventUI.transform.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value].text;

                    //break melee control
                    BreakAllMeleeEngagements(activeSoldier);

                    //break suppression
                    if (activeSoldier.suppressionValue != 0)
                        activeSoldier.suppressionValue = 0;

                    if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Fall"))
                    {
                        int.TryParse(menu.damageEventUI.transform.Find("FallDistance").Find("FallInput").GetComponent<TMP_InputField>().text, out int fallDistance);
                        activeSoldier.TakeDamage(null, CalculateFallDamage(activeSoldier, fallDistance), false, new List<string> { "Fall" });
                    }
                    else if (damageEventTypeDropdown.options[damageEventTypeDropdown.value].text.Contains("Collapse"))
                    {
                        int.TryParse(menu.damageEventUI.transform.Find("StructureHeight").Find("StructureHeightInput").GetComponent<TMP_InputField>().text, out int structureHeight);
                        //add xp if survives, otherwise kill
                        if (activeSoldier.StructuralCollapseCheck(structureHeight))
                        {
                            menu.AddXpAlert(activeSoldier, activeSoldier.stats.R.Val, "Survived a " + structureHeight + "cm structural collapse.", true);
                            menu.AddDamageAlert(activeSoldier, activeSoldier.soldierName + " survived a " + structureHeight + "cm structural collapse.", true);
                        }
                        else
                        {
                            if (activeSoldier.inventory.IsWearingJuggernautArmour())
                            {
                                activeSoldier.MakeUnconscious();
                                menu.AddDamageAlert(activeSoldier, activeSoldier.soldierName + " survived a " + structureHeight + "cm structural collapse with Juggernaut Armour.", true);
                            }
                            else
                            {
                                activeSoldier.InstantKill(null, new List<string>() { "Structural Collapse" });
                                activeSoldier.SetCrushed();
                            }
                        }
                    }
                        

                    //run detection alerts
                    StartCoroutine(DetectionAlertSingle(activeSoldier, "losChange", oldPos, string.Empty));

                    menu.CloseDamageEventUI();
                }
                else
                    Debug.Log("x, y, z values must not be out of bounds.");
            }
            else
                Debug.Log("Formatting was wrong, try again.");
        }
    }







    //inspirer functions
    public void InspirerCheck(Soldier inspirer)
    {
        menu.SetInspirerResolvedFlagTo(false);
        bool openInspirerUI = false;

        foreach (Soldier friendly in AllSoldiers())
        {
            float distance = CalculateRange(inspirer, friendly);

            if (friendly.IsAbleToSee() && inspirer.IsSameTeamAs(friendly) && friendly.PhysicalObjectWithinMaxRadius(inspirer))
            {
                menu.AddInspirerAlert(friendly, "An Inspirer (" + inspirer.soldierName + ") is within SR range of " + friendly.soldierName + ". Check for LOS?");
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
            Debug.Log("Haven't scrolled all the way to the bottom");
    }




    //xp functions - game
    public int CalculateShotKillXp(Soldier killer, Soldier deadMan)
    {
        int xp;
        int rankDifference = killer.RankDifferenceTo(deadMan);

        if (killer.IsHigherRankThan(deadMan))
            xp = 6 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
        else
            xp = 6 + rankDifference;
        
        //correct negatives
        if (xp < 0)
            xp = 0;

        return xp;
    }
    public int CalculateMeleeKillXp(Soldier killer, Soldier deadMan)
    {
        int xp;
        int rankDifference = killer.RankDifferenceTo(deadMan);

        if (killer.IsHigherRankThan(deadMan))
            xp = 10 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
        else
            xp = 10 + rankDifference;

        //correct negatives
        if (xp < 0)
            xp = 0;

        return xp;
    }
    public int CalculateMeleeCounterKillXp(Soldier killer, Soldier deadMan)
    {
        int xp;
        int rankDifference = killer.RankDifferenceTo(deadMan);

        if (killer.IsHigherRankThan(deadMan))
            xp = 20 + Mathf.CeilToInt(Mathf.Pow(rankDifference, 2) / 2);
        else
            xp = 20 + rankDifference;

        //correct negatives
        if (xp < 0)
            xp = 0;

        return xp;
    }





    //detection functions
    public float CalculateRange(PhysicalObject obj1, PhysicalObject obj2)
    {
        return Vector3.Distance(new Vector3(obj1.X, obj1.Y, obj1.Z), new Vector3(obj2.X, obj2.Y, obj2.Z));
    }
    public float CalculateRangeCover(Soldier s1, Vector3 cover)
    {
        return Vector3.Distance(new Vector3(s1.X, s1.Y, s1.Z), cover);
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

    public bool GlimpseDetectionDetecteeSeesMoving(Soldier movingSoldier, Soldier detectee, Vector3 movingSoldierOldPosition)
    {
        //reset bound crosses to zero
        boundCrossOne = Vector3.zero;
        boundCrossTwo = Vector3.zero;

        List<Vector3> previousPositionsMovingSoldier = new();
        List<bool> detecteeSeesMovingSoldier = new();
        int boundCrossCount = 0;

        float maxSteps = CalculateRangeCover(movingSoldier, movingSoldierOldPosition);

        //Debug.Log("Radius:" + detectee.SRColliderMax.radius);
        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 position = new(movingSoldierOldPosition.x + (movingSoldier.X - movingSoldierOldPosition.x) * (i / maxSteps), movingSoldierOldPosition.y + (movingSoldier.Y - movingSoldierOldPosition.y) * (i / maxSteps), movingSoldierOldPosition.z + (movingSoldier.Z - movingSoldierOldPosition.z) * (i / maxSteps));
            previousPositionsMovingSoldier.Add(position);

            //record when detectee sees moving soldier
            if (CalculateRangeCover(detectee, position) <= detectee.SRColliderMax.radius)
                detecteeSeesMovingSoldier.Add(true);
            else
                detecteeSeesMovingSoldier.Add(false);

            //Debug.Log("Point " + i + ": " + CalculateRangeCover(detectee, position));
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

        Debug.Log("DetecteeSeesMoving: Bound cross count: " + boundCrossCount);
        Debug.Log("DetecteeSeesMoving: Bound cross one = " + "X:" + boundCrossOne.x + " Y:" + boundCrossOne.y + " Z:" + boundCrossOne.z);
        Debug.Log("DetecteeSeesMoving: Bound cross two = " + "X:" + boundCrossTwo.x + " Y:" + boundCrossTwo.y + " Z:" + boundCrossTwo.z);

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

        float maxSteps = CalculateRangeCover(movingSoldier, movingSoldierOldPosition);

        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 position = new(movingSoldierOldPosition.x + (movingSoldier.X - movingSoldierOldPosition.x) * (i / maxSteps), movingSoldierOldPosition.y + (movingSoldier.Y - movingSoldierOldPosition.y) * (i / maxSteps), movingSoldierOldPosition.z + (movingSoldier.Z - movingSoldierOldPosition.z) * (i / maxSteps));
            previousPositionsMovingSoldier.Add(position);

            //record when moving solider sees detectee
            if (CalculateRangeCover(detectee, position) <= movingSoldier.SRColliderMax.radius)
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

        Debug.Log("MovingSeesDetectee: Bound cross count: " + boundCrossCount);
        Debug.Log("MovingSeesDetectee: Bound cross one = " + "X:" + boundCrossOne.x + " Y:" + boundCrossOne.y + " Z:" + boundCrossOne.z);
        Debug.Log("MovingSeesDetectee: Bound cross two = " + "X:" + boundCrossTwo.x + " Y:" + boundCrossTwo.y + " Z:" + boundCrossTwo.z);

        if (boundCrossCount > 0)
            return true;
        else
            return false;
    }

    public float CalculateMoveDistance(int x, int y, int z)
    {
        return Vector3.Distance(new Vector3(activeSoldier.X, activeSoldier.Y, activeSoldier.Z), new Vector3(x, y, z));
        //return Mathf.Sqrt(Mathf.Pow(activeSoldier.X - x, 2) + Mathf.Pow(activeSoldier.Y - y, 2) + Mathf.Pow(activeSoldier.Z - z, 2));
    }
    public IEnumerator DetectionAlertSingle(Soldier movingSoldier, string causeOfLosCheck, Vector3 movingSoldierOldPosition, string launchMelee)
    {
        //Debug.Log("Ran detection alert");
        yield return new WaitUntil(() => menu.meleeResolvedFlag == true && menu.inspirerResolvedFlag == true && menu.overrideView == false);

        string movingSoldierActiveStat = "F";
        string detecteeActiveStat = "C";
        int[] movingSoldierMultipliers = { 4, 2, 1 };
        int[] detecteeMultipliers = { 4, 3, 2 };

        //if moving soldier is offturn, swap the default active stats and Perceptiveness multipliers
        if (movingSoldier.IsOffturnAndAlive())
        {
            (detecteeActiveStat, movingSoldierActiveStat) = (movingSoldierActiveStat, detecteeActiveStat);
            (detecteeMultipliers, movingSoldierMultipliers) = (movingSoldierMultipliers, detecteeMultipliers);
        }

        //imperceptible delay to allow colliders to be recalculated at new destination
        yield return new WaitForSeconds(0.05f);

        if (movingSoldier.IsAlive() && !movingSoldier.IsPlayingDead())
        {
            bool showDetectionUI = false;
            foreach (Soldier detectee in AllSoldiers())
            {
                bool detectedLeft = false, detectedRight = false, avoidanceLeft = false, avoidanceRight = false, noDetectRight = false, noDetectLeft = false;
                string arrowType = "";
                string detectorLabel = "", counterLabel = "";

                if (detectee.IsAlive() && !detectee.IsPlayingDead() && movingSoldier.IsOppositeTeamAs(detectee))
                {
                    bool addDetection = false;

                    //check moving soldier seeing static soldiers
                    if (movingSoldier.IsAbleToSee())
                    {
                        if (movingSoldier.PhysicalObjectWithinMinRadius(detectee))
                        {
                            addDetection = true;

                            if (detectee.stats.GetStat(detecteeActiveStat).Val > movingSoldier.stats.P.Val * movingSoldierMultipliers[0])
                            {
                                detectorLabel += CreateDetectionMessage("avoidance", "3cm", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[0], movingSoldier.stats.P.Val);
                                avoidanceRight = true;
                            }
                            else
                            {
                                detectorLabel += CreateDetectionMessage("detection", "3cm", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[0], movingSoldier.stats.P.Val);
                                detectedRight = true;
                            }
                        }
                        else if (movingSoldier.PhysicalObjectWithinHalfRadius(detectee))
                        {
                            addDetection = true;

                            if (detectee.stats.GetStat(detecteeActiveStat).Val > movingSoldier.stats.P.Val * movingSoldierMultipliers[1])
                            {
                                detectorLabel += CreateDetectionMessage("avoidance", "half SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[1], movingSoldier.stats.P.Val);
                                avoidanceRight = true;
                            }
                            else
                            {
                                detectorLabel += CreateDetectionMessage("detection", "half SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[1], movingSoldier.stats.P.Val);
                                detectedRight = true;
                            }
                        }
                        else if (movingSoldier.PhysicalObjectWithinMaxRadius(detectee))
                        {
                            addDetection = true;

                            if (detectee.stats.GetStat(detecteeActiveStat).Val > movingSoldier.stats.P.Val * movingSoldierMultipliers[2])
                            {
                                detectorLabel += CreateDetectionMessage("avoidance", "full SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[2], movingSoldier.stats.P.Val);
                                avoidanceRight = true;
                            }
                            else
                            {
                                detectorLabel += CreateDetectionMessage("detection", "full SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[2], movingSoldier.stats.P.Val);
                                detectedRight = true;
                            }
                        }
                        else if (movingSoldierOldPosition != Vector3.zero && GlimpseDetectionMovingSeesDetectee(movingSoldier, detectee, movingSoldierOldPosition))
                        {
                            addDetection = true;

                            if (detectee.stats.GetStat(detecteeActiveStat).Val > movingSoldier.stats.P.Val * movingSoldierMultipliers[2])
                            {
                                if (boundCrossTwo != Vector3.zero)
                                    detectorLabel += CreateDetectionMessage("glimpseavoidance", "full SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[2], movingSoldier.stats.P.Val, boundCrossOne, boundCrossTwo);
                                else
                                    detectorLabel += CreateDetectionMessage("glimpseavoidance", "full SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[2], movingSoldier.stats.P.Val, boundCrossOne);

                                avoidanceRight = true;
                            }
                            else
                            {
                                if (boundCrossTwo != Vector3.zero)
                                    detectorLabel += CreateDetectionMessage("glimpsedetection", "full SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[2], movingSoldier.stats.P.Val, boundCrossOne, boundCrossTwo);
                                else
                                    detectorLabel += CreateDetectionMessage("glimpsedetection", "full SR", detecteeActiveStat, detectee.stats.GetStat(detecteeActiveStat).Val, movingSoldierMultipliers[2], movingSoldier.stats.P.Val, boundCrossOne);

                                detectedRight = true;
                            }
                        }
                        else
                        {
                            detectorLabel += "Not detected\n(out of SR)";
                            //Debug.Log("Not detected\n(out of SR)");
                            noDetectRight = true;
                        }
                    }
                    else
                    {
                        detectorLabel += "Not detected\n(blind)";
                        //Debug.Log("Not detected\n(blind)");
                        noDetectRight = true;
                    }

                    //check static soldiers seeing moving soldier
                    if (detectee.IsAbleToSee())
                    {
                        if (detectee.PhysicalObjectWithinMinRadius(movingSoldier))
                        {
                            addDetection = true;

                            if (movingSoldier.stats.GetStat(movingSoldierActiveStat).Val > detectee.stats.P.Val * detecteeMultipliers[0])
                            {
                                counterLabel += CreateDetectionMessage("avoidance", "3cm", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[0], detectee.stats.P.Val);
                                avoidanceLeft = true;
                            }
                            else
                            {
                                counterLabel += CreateDetectionMessage("detection", "3cm", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[0], detectee.stats.P.Val);
                                detectedLeft = true;
                            }
                        }
                        else if (detectee.PhysicalObjectWithinHalfRadius(movingSoldier))
                        {
                            addDetection = true;

                            if (movingSoldier.stats.GetStat(movingSoldierActiveStat).Val > detectee.stats.P.Val * detecteeMultipliers[1])
                            {
                                counterLabel += CreateDetectionMessage("avoidance", "half SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[1], detectee.stats.P.Val);
                                avoidanceLeft = true;
                            }
                            else
                            {
                                counterLabel += CreateDetectionMessage("detection", "half SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[1], detectee.stats.P.Val);
                                detectedLeft = true;
                            }
                        }
                        else if (detectee.PhysicalObjectWithinMaxRadius(movingSoldier))
                        {
                            addDetection = true;

                            if (movingSoldier.stats.GetStat(movingSoldierActiveStat).Val > detectee.stats.P.Val * detecteeMultipliers[2])
                            {
                                counterLabel += CreateDetectionMessage("avoidance", "full SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[2], detectee.stats.P.Val);
                                avoidanceLeft = true;
                            }
                            else
                            {
                                counterLabel += CreateDetectionMessage("detection", "full SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[2], detectee.stats.P.Val);
                                detectedLeft = true;
                            }
                        }
                        else if (movingSoldierOldPosition != Vector3.zero && GlimpseDetectionDetecteeSeesMoving(movingSoldier, detectee, movingSoldierOldPosition))
                        {
                            addDetection = true;

                            if (movingSoldier.stats.GetStat(movingSoldierActiveStat).Val > detectee.stats.P.Val * detecteeMultipliers[2])
                            {
                                if (boundCrossTwo != Vector3.zero)
                                    counterLabel += CreateDetectionMessage("glimpseavoidance", "full SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[2], detectee.stats.P.Val, boundCrossOne, boundCrossTwo);
                                else
                                    counterLabel += CreateDetectionMessage("glimpseavoidance", "full SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[2], detectee.stats.P.Val, boundCrossOne);

                                avoidanceLeft = true;
                            }
                            else
                            {
                                if (boundCrossTwo != Vector3.zero)
                                    counterLabel += CreateDetectionMessage("glimpsedetection", "full SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[2], detectee.stats.P.Val, boundCrossOne, boundCrossTwo);
                                else
                                    counterLabel += CreateDetectionMessage("glimpsedetection", "full SR", movingSoldierActiveStat, movingSoldier.stats.GetStat(movingSoldierActiveStat).Val, detecteeMultipliers[2], detectee.stats.P.Val, boundCrossOne);

                                detectedLeft = true;
                            }
                        }
                        else
                        {
                            counterLabel += "Not detected\n(out of SR)";
                            Debug.Log("Not detected\n(out of SR)");
                            noDetectLeft = true;
                        }
                    }
                    else
                    {
                        counterLabel += "Not detected\n(blind)";
                        Debug.Log("Not detected\n(blind)");
                        noDetectLeft = true;
                    }

                    //determine what kind of arrow to place
                    if (detectedRight)
                    {
                        if (avoidanceLeft)
                            arrowType = "avoidance2WayLeft";
                        else if (detectedLeft)
                            arrowType = "detection2Way";
                        else if (noDetectLeft)
                            arrowType = "detection1WayRight";
                    }
                    else if (avoidanceRight)
                    {
                        if (avoidanceLeft)
                            arrowType = "avoidance2Way";
                        else if (detectedLeft)
                            arrowType = "avoidance2WayRight";
                        else if (noDetectLeft)
                            arrowType = "avoidance1WayRight";
                    }
                    else if (noDetectRight)
                    {
                        if (avoidanceLeft)
                            arrowType = "avoidance1WayLeft";
                        else if (detectedLeft)
                            arrowType = "detection1WayLeft";
                    }

                    //suppress detection alerts that evaluate the same as prior state on stat changes which can not affect LOS
                    if (causeOfLosCheck.Equals("statChange"))
                    {
                        if (arrowType == "detection2Way" && movingSoldier.RevealedBySoldiers.Contains(detectee.id) && detectee.RevealedBySoldiers.Contains(movingSoldier.id))
                        {
                            addDetection = false;
                            Debug.Log(arrowType + ": soldiers can already see each other");
                        }
                        else if (arrowType == "detection1WayRight" && detectee.RevealedBySoldiers.Contains(movingSoldier.id))
                        {
                            addDetection = false;
                            Debug.Log(arrowType + ": onturn can see offturn (1 way)");
                        }
                        else if (arrowType == "detection1WayLeft" && movingSoldier.RevealedBySoldiers.Contains(detectee.id))
                        {
                            addDetection = false;
                            Debug.Log(arrowType + ": offturn can see onturn (1 way)");
                        }
                    }
                    

                    if (addDetection)
                    {
                        menu.AddDetectionAlert(movingSoldier, detectee, detectorLabel, counterLabel, arrowType);
                        showDetectionUI = true;
                    }
                }
            }

            if (showDetectionUI)
                menu.OpenGMAlertDetectionUI();
            else
                menu.ConfirmDetections(); //required to kill old LOS if soldier moves out of everyone's vis


            //run melee if required (Should wait till detections resolved)
            if (launchMelee != "")
                StartCoroutine(menu.OpenMeleeUI(launchMelee));
        }
    }

    public string CreateDetectionMessage(string type, string distance, string activeStatName, int activeStatValue, int perceptivenessMultiplier, int soldierPerceptiveness)
    {
        if (type.Contains("avoidance"))
        {
            //Debug.Log("<color=green>AVOIDANCE</color>\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " exceeded " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")");
            return "<color=green>AVOIDANCE</color>\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " exceeded " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")";
        }
        else
        {
            //Debug.Log("<color=red>DETECTED</color>\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " did not exceed " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")");
            return "<color=red>DETECTED</color>\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " did not exceed " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")";
        }
    }
    public string CreateDetectionMessage(string type, string distance, string activeStatName, int activeStatValue, int perceptivenessMultiplier, int soldierPerceptiveness, Vector3 boundCrossOne)
    {
        if (type.Contains("avoidance"))
        {
            //Debug.Log("<color=green>RETREAT AVOIDANCE</color>\n(Until: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " exceeded " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")");
            return "<color=green>RETREAT AVOIDANCE</color>\n(Until: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " exceeded " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")";
        }
        else
        {
            //Debug.Log("<color=red>RETREAT DETECTED</color>\n(Until: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " did not exceed " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")");
            return "<color=red>RETREAT DETECTED</color>\n(Until: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " did not exceed " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")";
        }
    }
    public string CreateDetectionMessage(string type, string distance, string activeStatName, int activeStatValue, int perceptivenessMultiplier, int soldierPerceptiveness, Vector3 boundCrossOne, Vector3 boundCrossTwo)
    {
        if (type.Contains("avoidance"))
        {
            //Debug.Log("<color=green>GLIMPSE AVOIDANCE</color>\n(Between: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + " and " + boundCrossTwo.x + ", " + boundCrossTwo.y + ", " + boundCrossTwo.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " exceeded " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")");
            return "<color=green>GLIMPSE AVOIDANCE</color>\n(Between: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + " and " + boundCrossTwo.x + ", " + boundCrossTwo.y + ", " + boundCrossTwo.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " exceeded " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")";
        }
        else
        {
            //Debug.Log("<color=red>GLIMPSE DETECTED</color>\n(Between: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + " and " + boundCrossTwo.x + ", " + boundCrossTwo.y + ", " + boundCrossTwo.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " did not exceed " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")");
            return "<color=red>GLIMPSE DETECTED</color>\n(Between: " + boundCrossOne.x + ", " + boundCrossOne.y + ", " + boundCrossOne.z + " and " + boundCrossTwo.x + ", " + boundCrossTwo.y + ", " + boundCrossTwo.z + ")\n(within " + distance + " " + activeStatName + "=" + activeStatValue + " did not exceed " + perceptivenessMultiplier + "P=" + soldierPerceptiveness * perceptivenessMultiplier + ")";
        }
    }

    public void DetectionAlertAllNonCoroutine(string causeOfLosCheck)
    {
        StartCoroutine(DetectionAlertAll(causeOfLosCheck));
    }
    public IEnumerator DetectionAlertAll(string causeOfLosCheck)
    {
        yield return new WaitUntil(() => menu.meleeResolvedFlag == true && menu.inspirerResolvedFlag == true);

        foreach (Soldier s in AllSoldiers())
        {
            if (s.IsOnturnAndAlive())
            {
                //Debug.Log("Running detection alert for " + s.soldierName);
                StartCoroutine(DetectionAlertSingle(s, causeOfLosCheck, Vector3.zero, string.Empty));
            }
        }
        menu.ConfirmDetections();
    }







    //insert game objects functions
    public void ConfirmInsertGameObjects()
    {
        TMP_Dropdown terrrainDropdownLocal = menu.overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>();
        string spawnedObject = menu.overrideInsertObjectsUI.transform.Find("OptionPanel").Find("ObjectType").Find("ObjectTypeDropdown").GetComponent<TMP_Dropdown>().value switch
        {
            2 => "Terminal",
            _ => string.Empty,
        };

        //check input formatting
        if (spawnedObject != string.Empty && terrrainDropdownLocal.value != 0 && int.TryParse(menu.overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Location").Find("XPos").GetComponent<TMP_InputField>().text, out x) && int.TryParse(menu.overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Location").Find("YPos").GetComponent<TMP_InputField>().text, out y) && int.TryParse(menu.overrideInsertObjectsUI.transform.Find("OptionPanel").Find("Location").Find("ZPos").GetComponent<TMP_InputField>().text, out z))
        {
            if (x >= 1 && x <= maxX && y >= 1 && y <= maxY && z >= 0 && z <= maxZ)
            {
                if (spawnedObject == "Terminal")
                    Instantiate(terminalPrefab).Init(x, y, z, terrrainDropdownLocal.options[terrrainDropdownLocal.value].text, "Combined");

                menu.CloseOverrideInsertObjectsUI();
            }
            else
                Debug.Log("x, y, z values must not be out of bounds.");
        }
        else
            Debug.Log("Formatting was wrong, try again.");
    }






    public void LoadData(GameData data)
    {
        currentRound = data.currentRound;
        currentTeam = data.currentTeam;

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
