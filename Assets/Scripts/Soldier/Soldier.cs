using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System.Collections;
using System;
using Newtonsoft.Json;
using UnityEditor;

public class Soldier : PhysicalObject, IDataPersistence, IHaveInventory, IAmShootable, IAmDetectable
{
    public Dictionary<string, object> details;
    public string soldierName, soldierTerrain, soldierSpeciality;
    public List<string> soldierAbilities;
    public int soldierTeam;
    public int soldierDisplayPriority;
    public Sprite soldierPortrait;
    public string soldierPortraitText;
    public bool fielded, selected, revealed, usedAP, usedMP, patriotic, bloodLettedThisTurn, illusionedThisMove, hasKilled, overwatchFirstShotUsed, guardsmanRetryUsed, amphStatReduction, modaProtect, trenXRayEffect, trenSRShrinkEffect;
    public int hp, ap, mp, tp, xp;
    public string rank;
    public int instantSpeed, roundsFielded, roundsFieldedConscious, roundsWithoutFood, loudActionTurnsVulnerable, stunnedTurnsVulnerable, overwatchShotCounter, suppressionValue, healthRemovedFromStarve, bleedoutTurns,
        plannerDonatedMove, turnsAvenging, overwatchXPoint, overwatchYPoint, overwatchConeRadius, overwatchConeArc, startX, startY, startZ, riotXPoint, riotYPoint;
    public string revealedByTeam, lastChosenStat, poisonedBy, isSpotting, glucoState;
    public Statline stats;
    public Inventory inventory;
    public List<string> state, inventoryList, controlledBySoldiersList, controllingSoldiersList, revealedBySoldiersList, revealingSoldiersList, witnessStoredAbilities, isSpottedBy, plannerGunsBlessed, gunnerGunsBlessed;
    public Item itemPrefab;
    private JArray statsJArray;
    public SphereCollider SRColliderMax, SRColliderHalf, SRColliderMin, itemCollider;
    public BoxCollider bodyCollider;
    public Dictionary<string, string> inventorySlots = new()
    {
        { "Head", "" }, { "Chest", "" }, { "Back", "" }, { "Posterior", "" }, { "Lateral", "" }, { "LeftLeg", "" }, { "RightLeg", "" }, { "LeftHand", "" }, { "RightHand", "" }
    };
    public string madeUnconBy;
    public List<string> madeUnconBydamageList;
    public Material selectedMaterial, deadMaterial;
    public List<Material> materials;

    public GameObject soldierUI, soldierUIPrefab, soldierSnapshotAlertPrefab;
    
    public SoldierManager soldierManager;
    public ItemManager itemManager;

    private void Awake()
    {
        game = FindFirstObjectByType<MainGame>();
        menu = FindFirstObjectByType<MainMenu>();
        soldierManager = FindFirstObjectByType<SoldierManager>();
        itemManager = FindFirstObjectByType<ItemManager>();
    }

    public Soldier Init(string name, int team, string terrain, Sprite portrait, string portraitText, string speciality, string ability)
    {
        id = GenerateGuid();
        soldierName = name;
        soldierTeam = team;
        soldierSpeciality = speciality;
        soldierDisplayPriority = CheckSpecialityIndex(speciality) + soldierTeam*100;
        soldierTerrain = terrain;
        soldierPortrait = portrait;
        soldierPortraitText = portraitText;
        soldierAbilities = new() { ability };
        stats = new Statline(this);
        inventory = new Inventory(this);
        IncrementSpeciality();
        IncrementDoubleRandom();
        hp = stats.H.BaseVal;
        GenerateAP();
        xp = 1;
        rank = "Recruit";
        SetState("Active");
        MapPhysicalPosition(0, 0, 0);

        return this;
    }
    public void SaveData(ref GameData data)
    {
        details = new Dictionary<string, object>
        {
            //save basic information
            { "soldierName", soldierName },
            { "team", soldierTeam },
            { "terrain", soldierTerrain },
            { "portrait", soldierPortraitText },
            { "speciality", soldierSpeciality },
            { "abilities", soldierAbilities },
            { "displayPriority", soldierDisplayPriority },
            { "fielded", fielded },
            { "hp", hp },
            { "ap", ap },
            { "mp", mp },
            { "tp", tp },
            { "xp", xp },
            { "rank", rank },
            { "state", state },

            //save position
            { "x", x },
            { "y", y },
            { "z", z },
            { "terrainOn", terrainOn },

            //save statline
            { "stats", stats.AllStats },
            { "instantSpeed", instantSpeed },

            //save inventory
            { "inventory", Inventory.AllItemIds },
            { "inventorySlots", inventorySlots },

            //save list of revealing soldiers
            { "revealingSoldiers", revealingSoldiersList },

            //save list of revealed by soldiers
            { "revealedBySoldiers", revealedBySoldiersList },

            //save list of controlling soldiers
            { "controllingSoldiers", controllingSoldiersList },

            //save list of controlled by soldiers
            { "controlledBySoldiers", controlledBySoldiersList },

            //save other details
            { "roundsFielded", roundsFielded },
            { "roundsFieldedConscious", roundsFieldedConscious },
            { "roundsWithoutFood", roundsWithoutFood },
            { "revealed", revealed },
            { "usedAP", usedAP },
            { "usedMP", usedMP },
            { "loudActionTurnsVulnerable", loudActionTurnsVulnerable },
            { "stunnedTurnsVulnerable", stunnedTurnsVulnerable },
            { "overwatchShotCounter", overwatchShotCounter },
            { "overwatchXPoint", overwatchXPoint },
            { "overwatchYPoint", overwatchYPoint },
            { "overwatchConeRadius", overwatchConeRadius },
            { "overwatchConeArc", overwatchConeArc },
            { "startX", startX },
            { "startY", startY },
            { "startZ", startZ },
            { "revealedByTeam", revealedByTeam },
            { "lastChosenStat", lastChosenStat },
            { "suppressionValue", suppressionValue },
            { "healthRemovedFromStarve", healthRemovedFromStarve },
            { "poisonedBy", poisonedBy },
            { "bleedoutTurns", bleedoutTurns },
            { "madeUnconBy", madeUnconBy },
            { "madeUnconBydamageList", madeUnconBydamageList },

            //save item details
            { "glucoState", glucoState },
            { "amphStatReduction", amphStatReduction },
            { "modaProtect", modaProtect },
            { "trenXRayEffect", trenXRayEffect },
            { "trenSRShrinkEffect", trenSRShrinkEffect },
            { "riotXPoint", riotXPoint },
            { "riotYPoint", riotYPoint },

            //save ability details
            { "plannerDonatedMove", plannerDonatedMove },
            { "patriotic", patriotic },
            { "bloodLettedThisTurn", bloodLettedThisTurn },
            { "illusionedThisMove", illusionedThisMove },
            { "hasKilled", hasKilled },
            { "turnsAvenging", turnsAvenging },
            { "guardsmanRetryUsed", guardsmanRetryUsed },
            { "isSpotting", isSpotting },
            { "isSpottedBy", isSpottedBy },
            { "overwatchFirstShotUsed", overwatchFirstShotUsed },
            { "witnessStoredAbilities", witnessStoredAbilities },
            { "plannerGunsBlessed", plannerGunsBlessed },
            { "gunnerGunsBlessed", gunnerGunsBlessed }
        };

        //add the soldier in
        if (data.allSoldiersDetails.ContainsKey(id))
            data.allSoldiersDetails.Remove(id);

        data.allSoldiersDetails.Add(id, details);
    }

    public void LoadData(GameData data)
    {
        data.allSoldiersDetails.TryGetValue(id, out details);
        soldierName = (string)details["soldierName"];
        soldierTeam = Convert.ToInt32(details["team"]);
        soldierTerrain = (string)details["terrain"];
        soldierPortrait = LoadPortrait((string)details["portrait"]);
        soldierPortraitText = (string)details["portrait"];
        soldierSpeciality = (string)details["speciality"];
        soldierAbilities = (details["abilities"] as JArray).Select(token => token.ToString()).ToList();
        soldierDisplayPriority = Convert.ToInt32(details["displayPriority"]);
        fielded = (bool)details["fielded"];
        hp = Convert.ToInt32(details["hp"]);
        ap = Convert.ToInt32(details["ap"]);
        mp = Convert.ToInt32(details["mp"]);
        tp = Convert.ToInt32(details["tp"]);
        xp = Convert.ToInt32(details["xp"]);
        rank = (string)details["rank"];
        state = (details["state"] as JArray).Select(token => token.ToString()).ToList();

        //load position
        x = Convert.ToInt32(details["x"]);
        y = Convert.ToInt32(details["y"]);
        z = Convert.ToInt32(details["z"]);
        terrainOn = (string)details["terrainOn"];
        MapPhysicalPosition(x, y, z);

        //load stats
        stats = new Statline(this);
        statsJArray = (JArray)details["stats"];
        foreach (JObject stat in statsJArray)
            stats.SetStat(stat.GetValue("Name").ToString(), (int)stat.GetValue("BaseVal"));
        instantSpeed = Convert.ToInt32(details["instantSpeed"]);

        //load inventory info
        inventory = new Inventory(this);
        inventoryList = (details["inventory"] as JArray).Select(token => token.ToString()).ToList();
        inventorySlots = JsonConvert.DeserializeObject<Dictionary<string, string>>(details["inventorySlots"].ToString());

        //load associated soldier lists
        revealingSoldiersList = (details["revealingSoldiers"] as JArray).Select(token => token.ToString()).ToList();
        revealedBySoldiersList = (details["revealedBySoldiers"] as JArray).Select(token => token.ToString()).ToList();
        controllingSoldiersList = (details["controllingSoldiers"] as JArray).Select(token => token.ToString()).ToList();
        controlledBySoldiersList = (details["controlledBySoldiers"] as JArray).Select(token => token.ToString()).ToList();

        //load other details
        roundsFielded = Convert.ToInt32(details["roundsFielded"]);
        roundsFieldedConscious = Convert.ToInt32(details["roundsFieldedConscious"]);
        roundsWithoutFood = Convert.ToInt32(details["roundsWithoutFood"]);
        revealed = (bool)details["revealed"];
        usedAP = (bool)details["usedAP"];
        usedMP = (bool)details["usedMP"];
        loudActionTurnsVulnerable = Convert.ToInt32(details["loudActionTurnsVulnerable"]);
        stunnedTurnsVulnerable = Convert.ToInt32(details["stunnedTurnsVulnerable"]);
        overwatchShotCounter = Convert.ToInt32(details["overwatchShotCounter"]);
        overwatchXPoint = Convert.ToInt32(details["overwatchXPoint"]);
        overwatchYPoint = Convert.ToInt32(details["overwatchYPoint"]);
        overwatchConeRadius = Convert.ToInt32(details["overwatchConeRadius"]);
        overwatchConeArc = Convert.ToInt32(details["overwatchConeArc"]);
        startX = Convert.ToInt32(details["startX"]);
        startY = Convert.ToInt32(details["startY"]);
        startZ = Convert.ToInt32(details["startZ"]);
        revealedByTeam = (string)details["revealedByTeam"];
        lastChosenStat = (string)details["lastChosenStat"];
        suppressionValue = Convert.ToInt32(details["suppressionValue"]);
        healthRemovedFromStarve = Convert.ToInt32(details["healthRemovedFromStarve"]);
        poisonedBy = (string)details["poisonedBy"];
        bleedoutTurns = Convert.ToInt32(details["bleedoutTurns"]);
        madeUnconBy = (string)details["madeUnconBy"];
        madeUnconBydamageList = (details["madeUnconBydamageList"] as JArray).Select(token => token.ToString()).ToList();

        //load item details
        glucoState = (string)details["glucoState"];
        amphStatReduction = (bool)details["amphStatReduction"];
        modaProtect = (bool)details["modaProtect"];
        trenXRayEffect = (bool)details["trenXRayEffect"];
        trenSRShrinkEffect = (bool)details["trenSRShrinkEffect"];
        riotXPoint = Convert.ToInt32(details["riotXPoint"]);
        riotYPoint = Convert.ToInt32(details["riotYPoint"]);

        //load ability details
        plannerDonatedMove = Convert.ToInt32(details["plannerDonatedMove"]);
        bloodLettedThisTurn = (bool)details["bloodLettedThisTurn"];
        patriotic = (bool)details["patriotic"];
        illusionedThisMove = (bool)details["illusionedThisMove"];
        hasKilled = (bool)details["hasKilled"];
        turnsAvenging = Convert.ToInt32(details["turnsAvenging"]);
        overwatchFirstShotUsed = (bool)details["overwatchFirstShotUsed"];
        guardsmanRetryUsed = (bool)details["guardsmanRetryUsed"];
        isSpotting = (string)details["isSpotting"];
        isSpottedBy = (details["isSpottedBy"] as JArray).Select(token => token.ToString()).ToList();
        witnessStoredAbilities = (details["witnessStoredAbilities"] as JArray).Select(token => token.ToString()).ToList();
        plannerGunsBlessed = (details["plannerGunsBlessed"] as JArray).Select(token => token.ToString()).ToList();
        gunnerGunsBlessed = (details["gunnerGunsBlessed"] as JArray).Select(token => token.ToString()).ToList();

        //link to maingame object
        game = FindFirstObjectByType<MainGame>();
    }
    public Soldier LinkWithUI(Transform displayPanel)
    {
        soldierUI = Instantiate(soldierUIPrefab, displayPanel);
        soldierUI.GetComponent<SoldierUI>().linkedSoldier = this;
        CheckSpecialityColor(soldierSpeciality);

        return this;
    }
    public void SetActiveSoldier()
    {
        menu.activeSoldier = this;
        game.activeSoldier = this;
        selected = true;
    }
    public void UnsetActiveSoldier()
    {
        menu.activeSoldier = null;
        game.activeSoldier = null;
        selected = false;
    }
    public bool IsFielded()
    {
        if (fielded)
            return true;
        return false;
    }
    public bool IsAlive()
    {
        if (IsFielded())
        {
            if (CheckState("Dead"))
                return false;
            else
                return true;
        }
        else
            return false;
    }
    public bool IsDead()
    {
        if (IsFielded())
        {
            if (CheckState("Dead"))
                return true;
            else
                return false;
        }
        else
            return false;
    }
    public bool IsConscious()
    {
        if (IsAlive())
        {
            if (CheckState("Unconscious"))
                return false;
            else
                return true;
        }
        else
            return false;
    }
    public bool IsInjured()
    {
        if (IsAlive())
            if (hp < stats.H.Val)
                return true;
        return false;
    }
    public bool IsTraumatised()
    {
        if (IsAlive())
            if (tp > 0)
                return true;
        return false;
    }
    public bool IsUnconscious()
    {
        if (CheckState("Unconscious"))
            return true;
        else
            return false;
    }
    public bool IsLastStand()
    {
        if (CheckState("Last Stand"))
            return true;
        else
            return false;
    }
    public bool IsAbleToWalk()
    {
        if (IsConscious())
        {
            if (IsLastStand())
                return false;
            else
                return true;
        }
        else
            return false;
    }
    public bool IsAbleToSee()
    {
        if (IsConscious())
        {
            if (stats.SR.Val > 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    public bool IsPlayingDead()
    {
        if (IsAlive() && CheckState("Playdead"))
            return true;
        else
            return false;
    }
    public bool IsSelf(Soldier s)
    {
        if (this == s)
            return true;

        return false;
    }
    public bool IsNotSelf(Soldier s)
    {
        if (this != s)
            return true;
        
        return false;
    }
    public bool IsSameTeamAs(Soldier s)
    {
        if (soldierTeam == s.soldierTeam && IsNotSelf(s))
            return true;
        
        return false;
    }
    public bool IsSameTeamAsIncludingSelf(Soldier s)
    {
        if (IsSameTeamAs(s) || IsSelf(s))
            return true;

        return false;
    }
    public bool IsOppositeTeamAs(Soldier s)
    {
        if (soldierTeam != s.soldierTeam && IsNotSelf(s))
            return true;

        return false;
    }
    public bool IsOnturn()
    {
        if (soldierTeam == game.currentTeam)
            return true;
        return false;
    }
    public bool IsOnturnAndAlive()
    {
        if (IsAlive() && IsOnturn())
            return true;
        return false;
    }
    public bool IsOffturn()
    {
        if (soldierTeam != game.currentTeam)
            return true;
        return false;
    }
    public bool IsOffturnAndAlive()
    {
        if (IsAlive() && IsOffturn())
            return true;
        return false;
    }
    public bool IsRevealed()
    {
        if ((IsAlive() && RevealedBySoldiers.Any()) || IsDead())
            return true;
        else
            return false;
    }
    public bool IsHidden()
    {
        return !IsRevealed();
    }
    public bool IsRevealing(Soldier soldier)
    {
        if (IsAbleToSee() && RevealingSoldiers.Contains(soldier.Id))
            return true;
        else
            return false;
    }
    public bool IsBeingRevealedBy(Soldier soldier)
    {
        if (IsAlive() && RevealedBySoldiers.Contains(soldier.Id))
            return true;
        else
            return false;
    }
    public bool CanSeeInOwnRight(Soldier s)
    {
        if (IsAbleToSee() && RevealingSoldiers.Contains(s.id))
            return true;
        else
            return false;
    }
    public bool IsResilient()
    {
        if (IsAlive() && stats.R.Val >= 6)
            return true;
        else
            return false;
    }
    public int RoundByResilience(float numberToRound)
    {
        if (ResilienceCheck())
            return Mathf.FloorToInt(numberToRound);
        return Mathf.CeilToInt(numberToRound);
    }
    public void FrozenMultiShot()
    {
        if (HasAnyAmmo() && !IsMeleeControlled())
            game.StartFrozenTurn(this);
    }

    public void DestroyAllBreakableItems(Soldier destroyedBy)
    {
        List<Item> itemList = new();
        foreach (Item item in Inventory.AllItems)
            itemList.Add(item);

        foreach (Item item in itemList)
            item.DestroyItem(destroyedBy);
    }
    public void BrokenDropAllItemsExceptArmour()
    {
        List<Item> itemList = new();
        Dictionary<string, string> itemSlots = new();
        foreach (Item item in Inventory.AllItems)
            if (!item.itemName.Contains("Armour"))
                itemList.Add(item);
        foreach (KeyValuePair<string, string> kvp in InventorySlots)
            itemSlots.Add(kvp.Key, kvp.Value);

        foreach (Item item in itemList)
            foreach (KeyValuePair<string, string> kvp in itemSlots)
                if (kvp.Value == item.id)
                    Inventory.RemoveItemFromSlot(item, kvp.Key);
    }
    public int GetFullHP()
    {
        return hp + GetArmourHP();
    }
    public int GetArmourHP()
    {
        int ahp = 0;

        foreach (Item item in Inventory.AllItems)
            ahp += item.ablativeHealth;

        return ahp;
    }
    public void PaintColor()
    {
        if (selected)
            gameObject.GetComponent<Renderer>().material = selectedMaterial;
        else
        {
            if (IsDead() || IsPlayingDead())
                gameObject.GetComponent<Renderer>().material = deadMaterial;
            else
                gameObject.GetComponent<Renderer>().material = materials[soldierTeam];
        }
    }
    public string PrintSoldierSpeciality()
    {
        return menu.FindStringInColXReturnStringInColYInMatrix(menu.specialtiesStats, soldierSpeciality, 1, 0);
    }
    public void IncrementXP(int xp, bool learnerEnabled)
    {
        if (learnerEnabled && IsLearner()) 
            this.xp += Mathf.CeilToInt(1.5f * xp);
        else
            this.xp += xp;
    }

    public string NextRank()
    {
        return this.rank switch
        {
            "Recruit" => "Private",
            "Private" => "Lieutenant",
            "Lieutenant" => "Sergeant",
            "Sergeant" => "Corporal",
            "Corporal" => "Captain",
            "Captain" => "Major",
            "Major" => "Lieutenant-Colonel",
            "Lieutenant-Colonel" => "Colonel",
            "Colonel" => "Brigadier",
            "Brigadier" => "Major-General",
            "Major-General" => "Lieutenant-General",
            "Lieutenant-General" => "General",
            "General" or _ => "",
        };
    }
    public int MinXPForRank()
    {
        return this.rank switch
        {
            "Recruit" => 1,
            "Private" => 2,
            "Lieutenant" => 4,
            "Sergeant" => 8,
            "Corporal" => 16,
            "Captain" => 32,
            "Major" => 64,
            "Lieutenant-Colonel" => 128,
            "Colonel" => 256,
            "Brigadier" => 512,
            "Major-General" => 1024,
            "Lieutenant-General" => 2048,
            "General" => 4096,
            _ => 0,
        };
    }
    public bool IsHigherRankThan(Soldier s)
    {
        if (MinXPForRank() > s.MinXPForRank())
            return true;

        return false;
    }
    public int RankDifferenceTo(Soldier s)
    {
        //print((int)(Mathf.Log(Convert.ToSingle(MinXPForRank()), 2.0f) - Mathf.Log(Convert.ToSingle(s.MinXPForRank()), 2.0f)));
        return (int)(Mathf.Log(Convert.ToSingle(MinXPForRank()), 2.0f) - Mathf.Log(Convert.ToSingle(s.MinXPForRank()), 2.0f));
    }

    public string[] Promote(string choiceStat)
    {
        string[] stats = new string[3];

        rank = NextRank();
        stats[0] = IncrementStat(choiceStat);
        stats[1] = IncrementSpeciality();
        stats[2] = IncrementRandom(choiceStat);
        lastChosenStat = choiceStat;

        return stats;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Battlefield"))
        {
            if (game.currentRound > 0 && game.weather.savedWeather.Count > 0)
            {
                CalculateActiveStats();
                DisplayStats();
                CheckRevealed();
                CheckSightRadius();
            }
        }
    }
    public void CalculateActiveStats()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Battlefield"))
        {
            if (game.currentRound > 0 && game.weather.savedWeather.Count > 0)
            {
                stats.L.Val = stats.L.BaseVal;
                stats.H.Val = stats.H.BaseVal;
                stats.R.Val = stats.R.BaseVal;
                stats.S.Val = stats.S.BaseVal;
                stats.E.Val = stats.E.BaseVal;
                stats.F.Val = stats.F.BaseVal;
                stats.P.Val = stats.P.BaseVal;
                stats.C.Val = stats.C.BaseVal;
                stats.SR.Val = stats.SR.BaseVal;
                stats.Ri.Val = stats.Ri.BaseVal;
                stats.AR.Val = stats.AR.BaseVal;
                stats.LMG.Val = stats.LMG.BaseVal;
                stats.Sn.Val = stats.Sn.BaseVal;
                stats.SMG.Val = stats.SMG.BaseVal;
                stats.Sh.Val = stats.Sh.BaseVal;
                stats.M.Val = stats.M.BaseVal;
                stats.Str.Val = stats.Str.BaseVal;
                stats.Dip.Val = stats.Dip.BaseVal;
                stats.Elec.Val = stats.Elec.BaseVal;
                stats.Heal.Val = stats.Heal.BaseVal;

                //map from BaseVal stats to Val stats using enviro effects etc.
                ApplyVisMods();
                ApplyTrenboloneMods();
                ApplyAmphetamineMods();
                ApplyTerrainMods();
                ApplyAbilityMods(); 
                ApplyTraumaMods();
                ApplyHealthStateMods();
                ApplyItemMods();
                ApplySmokeMods();
                ApplyTabunMods();
                ApplyLoudActionMods();

                CorrectNegatives();

                //get actual speed including enviro effects
                CalculateInstantSpeed();
            }
        }
    }
    public void CheckSightRadius()
    {
        //reflect changes to colliders
        SRColliderMin.radius = Mathf.Min(3, stats.SR.Val);
        SRColliderHalf.radius = Mathf.Max(SRColliderMin.radius, (stats.SR.Val / 2));
        SRColliderMax.radius = Mathf.Max(SRColliderMin.radius, stats.SR.Val);

        //check if soldier becomes blind
        if (stats.SR.Val == 0)
        {
            game.BreakAllControllingMeleeEngagments(this);
            UnsetOverwatch();
        }
    }
    public void ApplyVisMods()
    {
        if (!IsWearingThermalGoggles())
        {
            if (game.weather.CurrentWeather.Contains("Zero visibility"))
                stats.SR.Val -= 100;
            else if (game.weather.CurrentWeather.Contains("Poor visibility"))
                stats.SR.Val -= 90;
            else if (game.weather.CurrentWeather.Contains("Moderate visibility"))
                stats.SR.Val -= 70;
            else if (game.weather.CurrentWeather.Contains("Good visibility"))
                stats.SR.Val -= 40;
        }
    }
    public void ApplyTrenboloneMods()
    {
        //trenbolone radius shrink effect
        if (trenSRShrinkEffect)
            stats.SR.Val = Mathf.RoundToInt(0.4f * stats.SR.Val);
    }
    public void ApplyAmphetamineMods()
    {
        if (amphStatReduction)
            foreach (Stat stat in stats.AllStats)
                if (!stat.Name.Equals("H") && !stat.Longname.Equals(soldierSpeciality))
                    stat.Val -= stat.ReadIncrement*2;
    }
    public void ApplyTerrainMods()
    {
        if (IsOnNativeTerrain())
            stats.C.Val++;
        else if (IsOnOppositeTerrain())
            stats.C.Val--;
    }
    public void ApplyAbilityMods()
    {
        //mods applied to self
        ApplyWitnessMods();
        ApplyPatriotMods();
        ApplyShadowMods();
        ApplyGuardsmanMods();

        //mods applied by allies
        ApplyInspirerBuffMod();
        ApplyPlannerBuffMod();
    }
    public void ApplyWitnessMods()
    {
        if (IsWitness() && witnessStoredAbilities.Count == 0)
            stats.P.Val += 2;
    }
    public void ApplyPatriotMods()
    {
        if (IsPatriotic())
            stats.S.Val += 12;
    }
    public void ApplyShadowMods()
    {
        if (IsShadow())
        {
            Tuple<int, string>[] perceptionCamo = { Tuple.Create(stats.P.BaseVal, stats.P.Name), Tuple.Create(stats.C.BaseVal, stats.C.Name) };
            Array.Sort(perceptionCamo);

            // if less than half of the greater, set to half of the greater
            if (stats.GetStat(perceptionCamo[0].Item2).Val < Mathf.CeilToInt(perceptionCamo[1].Item1 / 2.0f))
                stats.GetStat(perceptionCamo[0].Item2).Val = Mathf.CeilToInt(perceptionCamo[1].Item1 / 2.0f);
        }
    }
    public void ApplyGuardsmanMods()
    {
        if (IsGuardsman() && IsOnOverwatch())
            stats.P.Val += 1;
    }
    public void ApplyInspirerBuffMod()
    {
        int inspirerIncrease = soldierSpeciality switch
        {
            "Leadership" or "Health" or "Resilience" or "Evasion" or "Fight" or "Perceptiveness" or "Camouflage" => 1,
            "Speed" => 6,
            "Sight Radius" => 10,
            _ => 0,
        };

        if (IsInspired() && inspirerIncrease > 0)
            stats.GetStat(soldierSpeciality).Val += inspirerIncrease;
    }
    public void ApplyPlannerBuffMod()
    {
        stats.S.Val += plannerDonatedMove;
    }
    public void ApplySustenanceMods()
    {
        if (roundsWithoutFood < 20 && healthRemovedFromStarve > 0)
        {
            TakeHeal(null, healthRemovedFromStarve, 0, false, false);
            healthRemovedFromStarve = 0;
        }

        if (roundsWithoutFood >= 20 && healthRemovedFromStarve == 0)
        {
            healthRemovedFromStarve = Mathf.RoundToInt(hp / 2.0f);
            TakeDamage(null, healthRemovedFromStarve, true, new() { "Sustenance" });
        }

        if (roundsWithoutFood >= 30 && IsConscious())
            MakeUnconscious(null, new() { "Sustenance" });

        if (roundsWithoutFood >= 40)
            InstantKill(null, new() { "Sustenance" });
    }

    public void ApplyHealthStateMods()
    {
        if (IsUnconscious() || IsPlayingDead() || IsStunned())
        {
            stats.SR.Val = 0;
            stats.E.Val = 0;
            stats.C.Val = 0;
            stats.M.Val = 0;
        }
    }
    public void ApplyTraumaMods()
    {
        if (tp >= 3 && tp < 5)
            stats.L.Val = 0;
    }
    public void ApplySmokeMods()
    {
        if (IsInSmoke())
        {
            if (IsSmokeBlinded())
            {
                stats.E.Val += 6;
                if (!IsWearingThermalGoggles())
                    stats.SR.Val = 0;
            }
            else if (IsSmokeCovered())
            {
                stats.E.Val += 3;
                if (!IsWearingThermalGoggles())
                    stats.SR.Val -= 70;
                stats.P.Val -= 2;
            }
        }
    }
    public void ApplyTabunMods()
    {
        if (IsInTabun())
        {
            stats.C.Val = 0;
            stats.P.Val = 0;

            if (CheckTabunEffectLevel(100))
            {
                stats.E.Val -= 4;
                stats.SR.Val -= 80;
            }
            else if (CheckTabunEffectLevel(50))
            {
                stats.E.Val -= 2;
                stats.SR.Val -= 40;
            }
            else if (CheckTabunEffectLevel(25))
            {
                stats.E.Val -= 1;
                stats.SR.Val -= 20;
            }
        }
    }
    public void ApplyItemMods()
    {
        if (IsWearingBodyArmour(false))
            stats.C.Val--;

        if (IsWearingGhillieArmour())
            stats.C.Val += 4;

        if (IsWearingExoArmour())
        {
            stats.C.Val = 0;
            stats.Str.Val *= 3;
        }

        if (IsWearingJuggernautArmour(false))
        {
            stats.C.Val = 0;
            stats.P.Val -= 2;
        }

        if (HasActiveRiotShield())
            stats.C.Val = 0;
    }
    public void ApplyLoudActionMods()
    {
        if (loudActionTurnsVulnerable > 0)
            stats.C.Val = 0;
    }
    public void CorrectNegatives()
    {
        if (stats.L.Val < 0)
            stats.L.Val = 0;
        if (stats.H.Val < 0)
            stats.H.Val = 0;
        if (stats.R.Val < 0)
            stats.R.Val = 0;
        if (stats.S.Val < 0)
            stats.S.Val = 0;
        if (stats.E.Val < 0)
            stats.E.Val = 0;
        if (stats.F.Val < 0)
            stats.F.Val = 0;
        if (stats.P.Val < 0)
            stats.P.Val = 0;
        if (stats.C.Val < 0)
            stats.C.Val = 0;
        if (stats.SR.Val < 0)
            stats.SR.Val = 0;
        if (stats.Ri.Val < 0)
            stats.Ri.Val = 0;
        if (stats.AR.Val < 0)
            stats.AR.Val = 0;
        if (stats.LMG.Val < 0)
            stats.LMG.Val = 0;
        if (stats.Sn.Val < 0)
            stats.Sn.Val = 0;
        if (stats.SMG.Val < 0)
            stats.SMG.Val = 0;
        if (stats.Sh.Val < 0)
            stats.Sh.Val = 0;
        if (stats.M.Val < 0)
            stats.M.Val = 0;
        if (stats.Str.Val < 0)
            stats.Str.Val = 0;
        if (stats.Dip.Val < 0)
            stats.Dip.Val = 0;
        if (stats.Elec.Val < 0)
            stats.Elec.Val = 0;
        if (stats.Heal.Val < 0)
            stats.Heal.Val = 0;
    }
    public void SetState(string stateName)
    {
        if (!state.Contains(stateName))
            state.Add(stateName);
    }
    public bool CheckState(string stateName)
    {
        return state.Contains(stateName);
    }
    public void UnsetState(string stateName)
    {
        state.RemoveAll(e => e == stateName);
    }
    public void SetPlaydead()
    {
        SetState("Playdead");

        stats.L.BaseVal = 0;
        stats.F.BaseVal = 0;
        stats.Dip.BaseVal = 0;
        stats.Elec.BaseVal = 0;

        //remove all engagements
        if (IsMeleeEngaged())
            StartCoroutine(game.DetermineMeleeControllerMultiple(this));

        StartCoroutine(game.DetectionAlertSingle(this, "losChange|statChange(SR)(C)|playdeadActive", Vector3.zero, string.Empty)); //losCheck
    }

    public void UnsetPlaydead()
    {
        UnsetState("Playdead");

        StartCoroutine(game.DetectionAlertSingle(this, "losChange|statChange(SR)(C)|playdeadDeactive", Vector3.zero, string.Empty)); //losCheck
    }
    public void TakeDrug(string drugName, Soldier administeredBy)
    {
        if (IsWearingStimulantArmour())
            menu.AddDamageAlert(this, $"{soldierName} is wearing Stim Armour, {drugName} had no effect.", true, true);
        else
        {
            if (this.IsSameTeamAsIncludingSelf(administeredBy) && administeredBy.IsMedic())
            {
                if (!IsOnDrug(drugName))
                {
                    TakeSpecificDrug(administeredBy, drugName, true, false);
                    menu.AddDamageAlert(this, $"{soldierName} took {drugName}. It worked with no side-effect.", true, true);
                }
            }
            else
            {
                if (IsOnDrug(drugName))
                    InstantKill(administeredBy, new List<string>() { "Overdose" });
                else if (IsOnAnyDrug())
                    TakePoisoning(administeredBy.Id, false);
                else
                {
                    int num = game.RandomNumber(1, 10);
                    if (num == 10)
                    {
                        TakeSpecificDrug(administeredBy, drugName, false, !IsExperimentalist());
                        menu.AddDamageAlert(this, $"{soldierName} took {drugName}. It didn't work but conferred a side-effect.", false, true);
                        if (IsExperimentalist())
                            menu.AddDamageAlert(this, $"{soldierName} is an <color=green>Experimentalist</color> and immune to the side-effect.", true, true);
                        else
                            menu.AddDamageAlert(this, $"{soldierName} suffered a side-effect.", false, true);
                    }
                    else if (num == 1)
                        menu.AddDamageAlert(this, $"{soldierName} took {drugName}. It didn't work at all.", false, true);
                    else
                    {
                        TakeSpecificDrug(administeredBy, drugName, true, !IsExperimentalist());
                        menu.AddDamageAlert(this, $"{soldierName} took {drugName}. It worked but conferred a side-effect.", true, true);
                        if (IsExperimentalist())
                            menu.AddDamageAlert(this, $"{soldierName} is an <color=green>Experimentalist</color> and immune to the side-effect.", true, true);
                        else
                            menu.AddDamageAlert(this, $"{soldierName} suffered a side-effect.", false, true);
                    }
                }
            }
        }
    }
    public void TakeSpecificDrug(Soldier administeredBy, string drugName, bool effect, bool sideEffect)
    {
        switch (drugName)
        {
            case "Amphetamine":
                if (effect)
                    stats.GetStat(soldierSpeciality).BaseVal *= 3;
                if (sideEffect)
                    amphStatReduction = true;
                break;
            case "Androstenedione":
                if (effect) { }
                if (sideEffect)
                {
                    stats.S.Decrement();
                    stats.S.Decrement();
                }
                break;
            case "Cannabinoid":
                if (effect)
                    stats.P.BaseVal *= 2;
                if (sideEffect)
                    stats.C.BaseVal = 0;
                break;
            case "Danazol":
                if (effect)
                    hp *= 2;
                if (sideEffect)
                {
                    stats.L.BaseVal = 0;
                    stats.R.BaseVal = 0;
                }
                break;
            case "Glucocorticoid":
                if (effect)
                    glucoState += "effect";
                if (sideEffect)
                    glucoState += "side";
                break;
            case "Modafinil":
                if (effect)
                    modaProtect = true;
                if (sideEffect)
                    TakeDamage(administeredBy, 1, false, new() { "Modafinil" });
                break;
            case "Shard":
                if (effect)
                {
                    stats.Ri.Increment();
                    stats.AR.Increment();
                    stats.LMG.Increment();
                    stats.Sn.Increment();
                    stats.SMG.Increment();
                    stats.Sh.Increment();
                    stats.M.Increment();
                }
                if (sideEffect)
                    soldierAbilities.Clear();
                break;
            case "Trenbolone":
                if (effect)
                    trenXRayEffect = true;
                if (sideEffect)
                    trenSRShrinkEffect = true;
                break;
            default:
                break;
        }
        SetOnDrug(drugName);
    }
    public IEnumerator TakePoisonDamage()
    {
        yield return new WaitUntil(() => menu.xpResolvedFlag == true);
        TakeDamage(soldierManager.FindSoldierById(poisonedBy), 2, false, new() { "Poison" });
    }
    public IEnumerator BleedoutKill()
    {
        yield return new WaitUntil(() => menu.xpResolvedFlag == true);
        madeUnconBydamageList.Add("Bleedout");
        InstantKill(soldierManager.FindSoldierById(madeUnconBy), madeUnconBydamageList);
    }

    public int ApplyDamageMods(Soldier damagedBy, int damage, List<string> damageSource)
    {
        //apply mods that apply to shot damage
        if (damageSource.Contains("Shot"))
        {
            if (damagedBy != null && HasActiveAndCorrectlyAngledRiotShield(new(damagedBy.X, damagedBy.Y)))
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted {damage} {menu.PrintList(damageSource)} damage with Riot Shield.", true, false);
                damage = 0;
            }
            if (IsWearingExoArmour() && game.CoinFlip())
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted {damage} {menu.PrintList(damageSource)} damage with Exo Armour.", true, false);
                damage = 0;
            }
        }

        //apply mods that apply to melee damage
        if (damageSource.Contains("Melee"))
        {
            if (IsWearingJuggernautArmour(false) && damagedBy != null && !damagedBy.IsWearingExoArmour())
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted {damage} {menu.PrintList(damageSource)} damage with Juggernaut Armour.", true, false);
                damage = 0;
            }
        }

        //apply mods that apply to explosive damage
        if (damageSource.Contains("Explosive"))
        {
            if (IsWearingJuggernautArmour(false))
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted {damage} {menu.PrintList(damageSource)} damage with Juggernaut Armour.", true, false);
                damage = 0;
            }
        }

        //apply mods that apply to all physical damage
        if (damageSource.Contains("Shot") || damageSource.Contains("Melee") || damageSource.Contains("Explosive"))
        {
            int remainingDamage = damage;

            //tank the damage on the armour if wearing BA or JA
            if (IsWearingJuggernautArmour(true))
            {
                remainingDamage = Inventory.GetItem("Armour_Juggernaut").TakeAblativeDamage(damagedBy, damage, damageSource);

                if (remainingDamage < damage)
                    menu.AddDamageAlert(this, $"{soldierName} absorbed {damage - remainingDamage} {menu.PrintList(damageSource)} damage with Juggernaut Armour.", true, false);
            }
            else if (IsWearingBodyArmour(true))
            {
                remainingDamage = Inventory.GetItem("Armour_Body").TakeAblativeDamage(damagedBy, damage, damageSource);

                if (remainingDamage < damage)
                    menu.AddDamageAlert(this, $"{soldierName} absorbed {damage - remainingDamage} {menu.PrintList(damageSource)} damage with Body Armour.", true, false);
            }

            damage = remainingDamage;
        }

        //apply insulator damage halving
        if (IsInsulator() && ResilienceCheck())
        {
            menu.AddDamageAlert(this, $"{soldierName} <color=green>Insulated</color> {damage - damage/2} {menu.PrintList(damageSource)} damage.", true, false);
            damage /= 2;
        }

        //apply andro damage reduction
        if (IsOnDrug("Androstenedione"))
        {
            menu.AddDamageAlert(this, $"{soldierName} resisted 1 {menu.PrintList(damageSource)} damage with Androstenedione.", true, false);
            damage -= 1;
        }

        //apply stim armour damage reduction
        if (IsWearingStimulantArmour())
        {
            menu.AddDamageAlert(this, $"{soldierName} resisted 2 {menu.PrintList(damageSource)} damage with Stim Armour.", true, false);
            damage -= 2;
        }

        //block damage if it's first turn and soldier has not used ap
        if (roundsFielded == 0 && !usedAP)
        {
            menu.AddDamageAlert(this, $"{soldierName} can't be damaged before using AP. {damage} {menu.PrintList(damageSource)} damage resisted.", true, false);
            damage = 0;
        }


        //correct negatives
        if (damage < 0)
            damage = 0;

        return damage;
    }

    public void TakeDamage(Soldier damagedBy, int damage, bool skipDamageMods, List<string> damageSource)
    {
        if (IsAlive())
        {
            //apply damage mods
            if (!skipDamageMods)
                damage = ApplyDamageMods(damagedBy, damage, damageSource);

            //make sure damage came from another soldier
            if (damagedBy != null && this.IsOppositeTeamAs(damagedBy))
            {
                //apply witness ability slurp
                if (IsWitness() && !damagedBy.IsRevoker())
                {
                    //apply the fresh abilities
                    soldierAbilities.Clear();
                    soldierAbilities.Add("Witness");
                    soldierAbilities.AddRange(damagedBy.soldierAbilities);

                    //store fresh abilities
                    witnessStoredAbilities.AddRange(damagedBy.soldierAbilities);
                    witnessStoredAbilities = witnessStoredAbilities.Distinct().ToList(); //make sure only unique abilities are represented
                }

                //informer ability display info
                if (IsInformer() && !damagedBy.IsRevoker())
                    AddSoldierSnapshot(damagedBy);
            }

            if (damage > 0)
            {
                //remove overwatch if damage taken
                UnsetOverwatch();
                //remove all spotting if damage taken
                RemoveAllSpotting();

                if (hp > 0)
                {
                    hp -= damage;

                    if (hp <= 0)
                    {
                        hp = 0;
                        Kill(damagedBy, damageSource);
                    }
                    else
                    {
                        if (IsUnconscious())
                            Kill(damagedBy, damageSource);
                        else if (IsLastStand())
                        {
                            if (ResilienceCheck())
                            {
                                menu.AddXpAlert(this, 2, "Resisted Unconsciousness.", false);
                                menu.AddDamageAlert(this, $"{soldierName} resisted falling <color=blue>Unconscious</color>.", true, true);
                            }
                            else
                                MakeUnconscious(damagedBy, damageSource);
                        }
                        else
                        {
                            if (hp == 1)
                            {
                                if (ResilienceCheck())
                                {
                                    MakeLastStand();
                                    menu.AddXpAlert(this, 2, "Resisted Unconsciousness.", false);
                                    menu.AddDamageAlert(this, $"{soldierName} resisted falling <color=blue>Unconscious</color>.", true, true);
                                }
                                else
                                    MakeUnconscious(damagedBy, damageSource);
                            }
                            else if (hp == 2)
                            {
                                if (ResilienceCheck())
                                {
                                    menu.AddXpAlert(this, 1, "Resisted Last Stand.", false);
                                    menu.AddDamageAlert(this, $"{soldierName} resisted falling into <color=red>Last Stand</color>.", true, true);
                                }
                                else
                                    MakeLastStand();
                            }
                            else if (hp == 3)
                            {
                                bool pass = false;

                                for (int i = 0; i < stats.R.Val; i++)
                                {
                                    if (ResilienceCheck())
                                        pass = true;
                                }

                                if (pass)
                                {
                                    menu.AddXpAlert(this, 1, "Resisted Last Stand.", false);
                                    menu.AddDamageAlert(this, $"{soldierName} resisted falling into <color=red>Last Stand</color>.", true, true);
                                }
                                else
                                    MakeLastStand();
                            }
                        }
                    }

                    //if not broken by health state change break remaining melee engagements
                    game.BreakAllControllingMeleeEngagments(this);
                }

                //add damage alert
                menu.AddDamageAlert(this, $"{soldierName} took {damage} ({menu.PrintList(damageSource)}) damage.", false, false);
                //make sure damage came from another soldier
                if (damagedBy != null)
                {
                    //apply stun affect from tranquiliser
                    if (damagedBy.IsTranquiliser() && (damageSource.Contains("Shot") || damageSource.Contains("Melee")) && !IsRevoker())
                        TakeStun(1);
                }
            }
        }
    }
    public void AddSoldierSnapshot(Soldier attackedBy)
    {
        GameObject snapshot = Instantiate(soldierSnapshotAlertPrefab, menu.damageUI.transform.Find("OptionPanel").Find("Scroll").Find("View").Find("Content"));

        snapshot.GetComponent<SoldierAlert>().SetSoldier(this);
        snapshot.transform.Find("SoldierID").GetComponent<TextMeshProUGUI>().text = attackedBy.Id;
        snapshot.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(attackedBy);
        snapshot.transform.Find("SnapshotDetails").GetComponent<TextMeshProUGUI>().text = $"{soldierName} has informed on an attacker ({attackedBy.soldierName}). Click to see details.";

        //try and open damagealert
        StartCoroutine(menu.OpenDamageList());
    }

    public void TabunTraumaCheck()
    {
        menu.AddTraumaAlert(this, 1, "Tabun exposure.", stats.R.Val + stats.Heal.Val, 1, "");
        menu.OpenTraumaAlertUI();
    }
    public void TakeTrauma(int trauma)
    {
        if (tp < 5)
        {
            tp += trauma;
            FileUtility.WriteToReport($"{soldierName} takes {trauma} trauma points. He is {GetTraumaState()}.");

            //perform frozen shenanigans
            if (IsFrozen())
            {
                FrozenMultiShot();
            }

            //drop all items for broken
            if (IsBroken())
            {
                BrokenDropAllItemsExceptArmour();
                foreach (Soldier s in game.AllSoldiers())
                    game.BreakMeleeEngagement(this, s);
            }
        }
    }

    public void TakeHeal(Soldier healedBy, int heal, int traumaHeal, bool overhealthEnabled, bool resurrectEnabled)
    {
        if (IsDead())
        {
            if (resurrectEnabled)
                Resurrect(heal);
            else
                print("Can't heal a dead soldier");
        }
        else
        {
            if (IsUnconscious())
            {
                if (healedBy != null)
                {
                    menu.AddDamageAlert(this, $"{soldierName} was revived by {healedBy.soldierName} (Uncon -> LS).", true, true);
                    menu.AddXpAlert(healedBy, 2, $"Revived {soldierName}.", true);
                }
                MakeLastStand();
            }
            else
            {
                int actualHeal = Heal(heal, overhealthEnabled);
                int actualTraumaHeal = HealTrauma(traumaHeal);

                if (IsLastStand())
                {
                    if (healedBy != null)
                        menu.AddDamageAlert(this, $"{soldierName} was stabilised by {healedBy.soldierName} (LS -> Active).", true, true);
                    MakeActive();
                }

                //xp for healing
                if (healedBy != null)
                {
                    //add xp for successful heal
                    if (healedBy == this)
                    {
                        menu.AddDamageAlert(this, $"{soldierName} healed self for {actualHeal} hp and removed {actualTraumaHeal} trauma points.", true, false);
                        menu.AddXpAlert(healedBy, Mathf.CeilToInt((actualHeal + actualTraumaHeal) / 2.0f), $"Healed self by {actualHeal} hp and removed {actualTraumaHeal} trauma points.", true);
                    }
                    else
                    {
                        menu.AddDamageAlert(this, $"{soldierName} was healed by {healedBy.soldierName} for {actualHeal} hp and removed {actualTraumaHeal} trauma points.", true, false);
                        menu.AddXpAlert(healedBy, actualHeal + actualTraumaHeal, $"Healed {soldierName} by {actualHeal} hp and removed {actualTraumaHeal} trauma points.", true);
                    }
                }
            }
        } 
    }
    public int Heal(int heal, bool overhealthEnabled)
    {
        int actualHeal = heal;
        hp += heal;

        //no overfilling health via heal unless overhealth is enabled
        if (!overhealthEnabled && hp > stats.H.Val)
        {
            actualHeal = heal - (hp - stats.H.Val);
            hp = stats.H.Val;
        }
        return actualHeal;
    }
    public int HealTrauma(int traumaHeal)
    {
        int actualTraumaHeal = 0;
        //don't heal if already desensitised or commited
        if (tp < 5 && tp > 0)
        {
            actualTraumaHeal = Mathf.Min(traumaHeal, tp);
            tp -= actualTraumaHeal;
        }
        
        return actualTraumaHeal;
    }

    public void CalculateInstantSpeed()
    {
        if (IsAbleToWalk())
        {
            instantSpeed = (int)((stats.S.Val - CalculateCarryWeight() + ApplyTerrainModsMove()) * ApplyVisModsMove() * ApplyRainModsMove() * ApplySustenanceModsMove() * ApplyTraumaModsMove() * ApplyKdModsMove() * ApplySmokeModsMove() * ApplyTabunModsMove()) + stats.Str.Val + ApplyFightModsMove();

            //halve movement for team 1 on first turn
            if (soldierTeam == 1 && game.currentRound == 1)
                instantSpeed /= 2;

            //cap lowest speed at 3cm
            if (instantSpeed < 3)
                instantSpeed = 3;
        }
        else
            instantSpeed = 0;
    }
    public float CalculateInstantSpeedSuppressed()
    {
        if (IsAbleToWalk())
            return (instantSpeed - stats.Str.Val)*ApplySuppressionModsMove() + stats.Str.Val;

        return 0;
    }
    public int CalculateCarryWeight()
    {
        int carryWeight = 0;

        foreach (Item i in Inventory.AllItems)
        {
            if (IsBull() && (i.IsGun() || i.IsAmmo()))
                carryWeight += 1;
            else
                carryWeight += i.weight;
        }

        //print("Carry Weight: " + carryWeight);
        return carryWeight;
    }
    public int ApplyTerrainModsMove()
    {
        int terrainModMove = 0;

        if (IsOnNativeTerrain())
            terrainModMove = 1;
        else if (IsOnOppositeTerrain())
            terrainModMove = -1;

        //print("Terrain Mod Move: " + terrainModMove);
        return terrainModMove;
    }
    public float ApplyVisModsMove()
    {
        float visModMove;

        //commander immune
        if (IsWearingThermalGoggles())
            visModMove = 0.0f;
        else
        {
            visModMove = game.weather.CurrentVis switch
            {
                "Zero" => 0.5f,
                "Poor" => 0.1f,
                _ => 0.0f,
            };
        }

        //print("Vis Mod Move: " + visModMove);
        return 1 - visModMove;
    }

    public float ApplyRainModsMove()
    {
        var rainModMove = game.weather.CurrentRain switch
        {
            "Torrential" => 0.2f,
            "Heavy" => 0.1f,
            _ => 0.0f,
        };

        //print("Rain Mod Move: " + rainModMove);
        return 1 - rainModMove;
    }

    public float ApplySustenanceModsMove()
    {
        float sustenanceModMove = 0f;

        if (RoundsWithoutFood >= 20)
            sustenanceModMove = 0.5f;
        else if (RoundsWithoutFood >= 10)
            sustenanceModMove = 0.2f;

        //print("Sustenance Mod Move: " + sustenanceModMove);
        return 1 - sustenanceModMove;
    }

    public float ApplyTraumaModsMove()
    {
        var traumaModMove = tp switch
        {
            3 => 0.4f,
            2 => 0.2f,
            1 => 0.1f,
            _ => 0.0f,
        };

        //print("Trauma Mod Move: " + traumaModMove);
        return 1 - traumaModMove;
    }
    public float ApplyKdModsMove()
    {
        float kdModMove = (2 * GetKd() / 100f);

        //print("Kd Mod Move: " + kdModMove);
        return 1 - kdModMove;
    }
    public float ApplySmokeModsMove()
    {
        float smokeModMove = 0;

        if (IsSmokeCovered())
            smokeModMove = 0.4f;

        return 1 - smokeModMove;
    }
    public float ApplyTabunModsMove()
    {
        float tabunModMove = 0;

        if (IsInTabun())
        {
            if (CheckTabunEffectLevel(100))
                tabunModMove = 0.8f;
            else if (CheckTabunEffectLevel(50))
                tabunModMove = 0.4f;
            else if (CheckTabunEffectLevel(25))
                tabunModMove = 0.2f;
        }

        return 1 - tabunModMove;
    }
    public int ApplyFightModsMove()
    {
        int fightModMove = 0;

        if (FightActive())
            fightModMove = 5 * stats.F.Val;
        else if (AvengingActive()) //avenger ability
            fightModMove = 5 * (stats.F.Val - 1);

        //print("Fight Mod Move: " + fightModMove);
        return fightModMove;
    }
    public float ApplySuppressionModsMove()
    {
        float suppressionModMove = (suppressionValue / 100f);

        //print("Suppression Mod Move: " + suppressionModMove);
        return 1 - suppressionModMove;
    }
    public void DisplayStats()
    {
        soldierUI.transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(this);
        soldierUI.transform.Find("AP").gameObject.GetComponent<TextMeshProUGUI>().text = "AP:" + ap;
        soldierUI.transform.Find("MP").gameObject.GetComponent<TextMeshProUGUI>().text = "MA:" + mp;
        soldierUI.transform.Find("Location").gameObject.GetComponent<TextMeshProUGUI>().text = "X:" + x + "   Y:" + y + "   Z:" + z;
    }
    public int DetectionActiveStat(int multiplier)
    {
        return stats.P.Val * multiplier;
    }
    public void CheckRevealed()
    {
        //check if revealed at all
        if (RevealedBySoldiers.Any())
            revealed = true;
        else
            revealed = false; 
    }
    public bool FindMeleeTargets()
    {
        foreach (Soldier s in game.AllSoldiers())
            if (s.IsAlive() && IsOppositeTeamAs(s) && s.IsRevealed() && PhysicalObjectWithinMeleeRadius(s))
                return true;

        return false;
    }
    public void PerformLoudAction(int loudActionRadius)
    {
        int vulnerableTurns = 0;

        //shadow ability
        if (IsShadow())
            loudActionRadius = Mathf.CeilToInt(loudActionRadius / 2.0f);

        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsConscious() && this.IsOppositeTeamAs(s) && PhysicalObjectWithinRadius(s,loudActionRadius))
            {
                if (s.PhysicalObjectWithinMinRadius(this) && vulnerableTurns < 6)
                    vulnerableTurns = 6;
                else if (s.PhysicalObjectWithinHalfRadius(this) && vulnerableTurns < 4)
                    vulnerableTurns = 4;
                else if (s.PhysicalObjectWithinMaxRadius(this) && vulnerableTurns < 2)
                    vulnerableTurns = 2;

                if (s.IsSpotter() && IsHidden())
                    s.SetSpotting(this);
            }
        }

        if (vulnerableTurns > 0)
        {
            //run detection alerts if loud action performed for first time
            if (loudActionTurnsVulnerable == 0)
                StartCoroutine(game.DetectionAlertSingle(this, "statChange(C)|loudAction", Vector3.zero, string.Empty)); //loscheck

            if (vulnerableTurns > loudActionTurnsVulnerable)
                SetLoudRevealed(vulnerableTurns);
        }
    }
    public void PerformLoudAction()
    {
        int vulnerableTurns = 0;

        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsConscious() && this.IsOppositeTeamAs(s))
            {
                if (s.PhysicalObjectWithinMinRadius(this) && vulnerableTurns < 6)
                    vulnerableTurns = 6;
                else if (s.PhysicalObjectWithinHalfRadius(this) && vulnerableTurns < 4)
                    vulnerableTurns = 4;
                else if (s.PhysicalObjectWithinMaxRadius(this) && vulnerableTurns < 2)
                    vulnerableTurns = 2;

                if (s.IsSpotter() && IsHidden())
                    s.SetSpotting(this);
            }
        }
        
        if (vulnerableTurns > 0)
        {
            //run detection alerts if loud action performed for first time
            if (loudActionTurnsVulnerable == 0)
                StartCoroutine(game.DetectionAlertSingle(this, "statChange(C)|loudAction", Vector3.zero, string.Empty)); //losCheck

            if (vulnerableTurns > loudActionTurnsVulnerable)
                SetLoudRevealed(vulnerableTurns);
        }
    }
    public List<int> APLoop()
    {
        List<int> apMp = new();
        int ap = 3, mp = 1;
        var leadership = stats.L.Val;
        var runLoop = true;

        while (runLoop)
        {
            if (UnityEngine.Random.Range(1, 7) <= leadership)
            {
                mp++;
                if (UnityEngine.Random.Range(1, 7) <= 3)
                    ap++;
                else
                    ap += 2;

                if (leadership > 6)
                    ap += 3;
                else
                    ap += (leadership / 2);

                leadership -= 6;
            }
            else
                runLoop = false;
        }

        apMp.Add(ap);
        apMp.Add(mp);
        return apMp;
    }
    public void GenerateAP()
    {
        List<int> apMp = APLoop();

        //check for wearing logistics belt
        if (IsWearingLogisticsBelt())
            apMp[0]++;

        //add inspirer ap bonus to support specialities
        apMp[0] += InspirerBonusSupport();

        //minus dissauder ap
        apMp[0] += DissuaderPenalty();

        //check gluco immobilisation
        if (glucoState == "side")
            apMp[1] = 0;

        //run gluco rush
        if (glucoState.Contains("effect"))
        {
            apMp = apMp.Zip(APLoop(), (x, y) => x + y).ToList();
            apMp = apMp.Zip(APLoop(), (x, y) => x + y).ToList();
            apMp = apMp.Zip(APLoop(), (x, y) => x + y).ToList();
            apMp = apMp.Zip(APLoop(), (x, y) => x + y).ToList();
        }

        ap = apMp[0];
        mp = apMp[1];

        //post check to set gluco to finished state either side or neutral
        switch (glucoState)
        {
            case "effectside":
                glucoState = "side";
                break;
            case "effect":
                glucoState = "";
                break;
            default:
                break;
        }
    }

    public void Unreveal()
    {
        //revealedByTeam = "";
        RevealedBySoldiers = new List<string>();
        RevealingSoldiers = new List<string>();
        revealed = false;
    }

    public void RemoveSoldierLOS(string name)
    {
        RemoveRevealedBySoldier(name);
        RemoveRevealingSoldier(name);
    }

    public void RemoveRevealedBySoldier(string name)
    {
        List<string> temp = revealedBySoldiersList;
        temp.Remove(name);

        if (RevealedBySoldiers.Contains(name))
            RevealedBySoldiers = temp;
    }

    public void RemoveRevealingSoldier(string name)
    {
        if (RevealingSoldiers.Contains(name))
            RevealingSoldiers.Remove(name);
    }

    public Sprite LoadPortrait(string portraitName)
    {
        TMP_Dropdown allPortraits = FindFirstObjectByType<AllPortraits>().allPortraitsDropdown;
        return portraitName switch
        {
            "Alpine_Commander" => allPortraits.options[0].image,
            "Alpine_Balaclava" => allPortraits.options[1].image,
            "Alpine_BroadBrim" => allPortraits.options[2].image,
            "Alpine_Cap" => allPortraits.options[3].image,
            "Alpine_GasMask" => allPortraits.options[4].image,
            "Alpine_Helmet" => allPortraits.options[5].image,
            "Alpine_Visor" => allPortraits.options[6].image,
            "Alpine_WWII" => allPortraits.options[7].image,
            "Desert_Commander" => allPortraits.options[8].image,
            "Desert_Balaclava" => allPortraits.options[9].image,
            "Desert_BroadBrim" => allPortraits.options[10].image,
            "Desert_DarkWWII" => allPortraits.options[11].image,
            "Desert_GasMask" => allPortraits.options[12].image,
            "Desert_Helmet" => allPortraits.options[13].image,
            "Desert_LightWWII" => allPortraits.options[14].image,
            "Desert_Shades" => allPortraits.options[15].image,
            "Jungle_Commander" => allPortraits.options[16].image,
            "Jungle_Balaclava" => allPortraits.options[17].image,
            "Jungle_BeardWWII" => allPortraits.options[18].image,
            "Jungle_Mewham" => allPortraits.options[19].image,
            "Jungle_DarkWWII" => allPortraits.options[20].image,
            "Jungle_LightWWII" => allPortraits.options[21].image,
            "Jungle_Rang" => allPortraits.options[22].image,
            "Jungle_Shades" => allPortraits.options[23].image,
            "Urban_Commander" => allPortraits.options[24].image,
            "Urban_Anubis" => allPortraits.options[25].image,
            "Urban_Beret" => allPortraits.options[26].image,
            "Urban_BlackBalaclava" => allPortraits.options[27].image,
            "Urban_BrownBalaclava" => allPortraits.options[28].image,
            "Urban_Facepaint" => allPortraits.options[29].image,
            "Urban_Shades" => allPortraits.options[30].image,
            "Urban_WWII" => allPortraits.options[31].image,
            _ => allPortraits.options[0].image,
        };
    }
    public Sprite LoadPortraitTeamsight(string portraitName)
    {
        TMP_Dropdown allPortraits = FindFirstObjectByType<AllPortraits>().allPortraitsTeamsightDropdown;
        return portraitName switch
        {
            "Alpine_Commander" => allPortraits.options[0].image,
            "Alpine_Balaclava" => allPortraits.options[1].image,
            "Alpine_BroadBrim" => allPortraits.options[2].image,
            "Alpine_Cap" => allPortraits.options[3].image,
            "Alpine_GasMask" => allPortraits.options[4].image,
            "Alpine_Helmet" => allPortraits.options[5].image,
            "Alpine_Visor" => allPortraits.options[6].image,
            "Alpine_WWII" => allPortraits.options[7].image,
            "Desert_Commander" => allPortraits.options[8].image,
            "Desert_Balaclava" => allPortraits.options[9].image,
            "Desert_BroadBrim" => allPortraits.options[10].image,
            "Desert_DarkWWII" => allPortraits.options[11].image,
            "Desert_GasMask" => allPortraits.options[12].image,
            "Desert_Helmet" => allPortraits.options[13].image,
            "Desert_LightWWII" => allPortraits.options[14].image,
            "Desert_Shades" => allPortraits.options[15].image,
            "Jungle_Commander" => allPortraits.options[16].image,
            "Jungle_Balaclava" => allPortraits.options[17].image,
            "Jungle_BeardWWII" => allPortraits.options[18].image,
            "Jungle_Mewham" => allPortraits.options[19].image,
            "Jungle_DarkWWII" => allPortraits.options[20].image,
            "Jungle_LightWWII" => allPortraits.options[21].image,
            "Jungle_Rang" => allPortraits.options[22].image,
            "Jungle_Shades" => allPortraits.options[23].image,
            "Urban_Commander" => allPortraits.options[24].image,
            "Urban_Anubis" => allPortraits.options[25].image,
            "Urban_Beret" => allPortraits.options[26].image,
            "Urban_BlackBalaclava" => allPortraits.options[27].image,
            "Urban_BrownBalaclava" => allPortraits.options[28].image,
            "Urban_Facepaint" => allPortraits.options[29].image,
            "Urban_Shades" => allPortraits.options[30].image,
            "Urban_WWII" => allPortraits.options[31].image,
            _ => allPortraits.options[0].image,
        };
    }
    public Sprite LoadPortraitJammed(string portraitName)
    {
        TMP_Dropdown allPortraits = FindFirstObjectByType<AllPortraits>().allPortraitsJammedDropdown;
        return portraitName switch
        {
            "Alpine_Commander" => allPortraits.options[0].image,
            "Alpine_Balaclava" => allPortraits.options[1].image,
            "Alpine_BroadBrim" => allPortraits.options[2].image,
            "Alpine_Cap" => allPortraits.options[3].image,
            "Alpine_GasMask" => allPortraits.options[4].image,
            "Alpine_Helmet" => allPortraits.options[5].image,
            "Alpine_Visor" => allPortraits.options[6].image,
            "Alpine_WWII" => allPortraits.options[7].image,
            "Desert_Commander" => allPortraits.options[8].image,
            "Desert_Balaclava" => allPortraits.options[9].image,
            "Desert_BroadBrim" => allPortraits.options[10].image,
            "Desert_DarkWWII" => allPortraits.options[11].image,
            "Desert_GasMask" => allPortraits.options[12].image,
            "Desert_Helmet" => allPortraits.options[13].image,
            "Desert_LightWWII" => allPortraits.options[14].image,
            "Desert_Shades" => allPortraits.options[15].image,
            "Jungle_Commander" => allPortraits.options[16].image,
            "Jungle_Balaclava" => allPortraits.options[17].image,
            "Jungle_BeardWWII" => allPortraits.options[18].image,
            "Jungle_Mewham" => allPortraits.options[19].image,
            "Jungle_DarkWWII" => allPortraits.options[20].image,
            "Jungle_LightWWII" => allPortraits.options[21].image,
            "Jungle_Rang" => allPortraits.options[22].image,
            "Jungle_Shades" => allPortraits.options[23].image,
            "Urban_Commander" => allPortraits.options[24].image,
            "Urban_Anubis" => allPortraits.options[25].image,
            "Urban_Beret" => allPortraits.options[26].image,
            "Urban_BlackBalaclava" => allPortraits.options[27].image,
            "Urban_BrownBalaclava" => allPortraits.options[28].image,
            "Urban_Facepaint" => allPortraits.options[29].image,
            "Urban_Shades" => allPortraits.options[30].image,
            "Urban_WWII" => allPortraits.options[31].image,
            _ => allPortraits.options[0].image,
        };
    }
    public Sprite LoadInsignia(string rank)
    {
        TMP_Dropdown allInsignia = FindFirstObjectByType<AllInsignia>().allInsigniaDropdown;
        return rank switch
        {
            "Private" => allInsignia.options[1].image,
            "Lieutenant" => allInsignia.options[2].image,
            "Sergeant" => allInsignia.options[3].image,
            "Corporal" => allInsignia.options[4].image,
            "Captain" => allInsignia.options[5].image,
            "Major" => allInsignia.options[6].image,
            "Lieutenant-Colonel" => allInsignia.options[7].image,
            "Colonel" => allInsignia.options[8].image,
            "Brigadier" => allInsignia.options[9].image,
            "Major-General" => allInsignia.options[10].image,
            "Lieutenant-General" => allInsignia.options[11].image,
            "General" => allInsignia.options[12].image,
            "Recruit" or _ => allInsignia.options[0].image,
        };
    }
    public Sprite LoadPosition(string position)
    {
        TMP_Dropdown allPositions = FindFirstObjectByType<AllPositions>().allPositionsDropdown;
        return position switch
        {
            "Last Stand" => allPositions.options[1].image,
            "Unconscious" => allPositions.options[2].image,
            "Active" or _ => allPositions.options[0].image,
        };
    }
    public string IncrementRandom(string choiceStat)
    {
        string[] stats =
        {
            "Leadership", "Health", "Resilience", "Speed", "Evasion", "Fight", "Perceptiveness", "Camouflage", "Sight Radius", 
            "Rifle", "Assault Rifle", "Light Machine Gun", "Sniper Rifle", "Sub-Machine Gun", "Shotgun", "Melee",
            "Strength", "Diplomacy", "Electronics", "Healing"
        };

        stats = stats.Where(e => e != soldierSpeciality && e != choiceStat).ToArray();
        return IncrementStat(stats[UnityEngine.Random.Range(0, stats.Length)]);
    }
    public void IncrementDoubleRandom()
    {
        int num1 = -1, num2 = -1, num3 = -1;
        string[] stats =
        {
            "Leadership", "Health", "Resilience", "Speed", "Evasion", "Fight", "Perceptiveness", "Camouflage", "Sight Radius",
            "Rifle", "Assault Rifle", "Light Machine Gun", "Sniper Rifle", "Sub-Machine Gun", "Shotgun", "Melee",
            "Strength", "Diplomacy", "Electronics", "Healing"
        };

        stats = stats.Where(e => e != soldierSpeciality).ToArray();

        num1 = UnityEngine.Random.Range(0, stats.Length);
        num2 = UnityEngine.Random.Range(0, stats.Length);

        if (num1 == num2)
        {
            num3 = UnityEngine.Random.Range(0, stats.Length);
            while (num1 == num2 || num1 == num3 || num2 == num3)
            {
                num1 = UnityEngine.Random.Range(0, stats.Length);
                num2 = UnityEngine.Random.Range(0, stats.Length);
                num3 = UnityEngine.Random.Range(0, stats.Length);
            }
        }

        IncrementStat(stats[num1]);
        IncrementStat(stats[num2]);
        if (num3 > -1)
            IncrementStat(stats[num3]);
    }
    public string IncrementSpeciality()
    {
        return IncrementStat(soldierSpeciality);
    }
    public string IncrementStat(string statName)
    {
        string incrementDisplay = statName switch
        {
            "Leadership" => "L: +" + stats.L.Increment(),
            "Health" => "H: +" + stats.H.Increment(),
            "Resilience" => "R: +" + stats.R.Increment(),
            "Speed" => "S: +" + stats.S.Increment(),
            "Evasion" => "E: +" + stats.E.Increment(),
            "Fight" => "F: +" + stats.F.Increment(),
            "Perceptiveness" => "P: +" + stats.P.Increment(),
            "Camouflage" => "C: +" + stats.C.Increment(),
            "Sight Radius" => "SR: +" + stats.SR.Increment(),
            "Rifle" => "Ri: +" + stats.Ri.Increment(),
            "Assault Rifle" => "AR: +" + stats.AR.Increment(),
            "Light Machine Gun" => "LMG: +" + stats.LMG.Increment(),
            "Sniper Rifle" => "Sn: +" + stats.Sn.Increment(),
            "Sub-Machine Gun" => "SMG: +" + stats.SMG.Increment(),
            "Shotgun" => "Sh: +" + stats.Sh.Increment(),
            "Melee" => "M: +" + stats.M.Increment(),
            "Strength" => "Str: +" + stats.Str.Increment(),
            "Diplomacy" => "Dip: +" + stats.Dip.Increment(),
            "Electronics" => "Elec: +" + stats.Elec.Increment(),
            "Healing" => "Heal: +" + stats.Heal.Increment(),
            _ => "Error",
        };

        return incrementDisplay;
    }
    public void CheckSpecialityColor(string speciality)
    {
        if (IsFielded())
        {
            soldierUI.transform.Find("ActionButton").gameObject.GetComponent<Button>().enabled = true;
            soldierUI.transform.Find("FieldButton").gameObject.SetActive(false);

            if (IsDead())
            {
                soldierUI.transform.Find("KIA").gameObject.SetActive(true);
                soldierUI.transform.Find("ActionButton").gameObject.GetComponent<Image>().color = new Color(1, 0, 0, 0.2f);
            }
            else
            {
                soldierUI.transform.Find("KIA").gameObject.SetActive(false);
                switch (speciality)
                {
                    case "Leadership":
                    case "Health":
                    case "Resilience":
                    case "Speed":
                    case "Evasion":
                    case "Fight":
                    case "Perceptiveness":
                    case "Camouflage":
                    case "Sight Radius":
                        soldierUI.transform.Find("ActionButton").gameObject.GetComponent<Image>().color = new Color(0, 1, 0, 0.2f);
                        break;
                    case "Rifle":
                    case "Assault Rifle":
                    case "Light Machine Gun":
                    case "Sniper Rifle":
                    case "Sub-Machine Gun":
                    case "Shotgun":
                    case "Melee":
                        soldierUI.transform.Find("ActionButton").gameObject.GetComponent<Image>().color = new Color(1, 0.92f, 0.016f, 0.2f);
                        break;
                    case "Strength":
                    case "Diplomacy":
                    case "Electronics":
                    case "Healing":
                        soldierUI.transform.Find("ActionButton").gameObject.GetComponent<Image>().color = new Color(0, 0, 1, 0.2f);
                        break;
                }
            }
        }
        else
        {
            soldierUI.transform.Find("ActionButton").gameObject.GetComponent<Image>().color = new Color(0.16f, 0.16f, 0.16f, 0.7f);
            soldierUI.transform.Find("ActionButton").gameObject.GetComponent<Button>().enabled = false;
            soldierUI.transform.Find("FieldButton").gameObject.SetActive(true);
        } 
    }
    public int CheckSpecialityIndex(string speciality)
    {
        return speciality switch
        {
            "Leadership" => 1,
            "Health" => 2,
            "Resilience" => 3,
            "Speed" => 4,
            "Evasion" => 5,
            "Fight" => 6,
            "Perceptiveness" => 7,
            "Camouflage" => 8,
            "Sight Radius" => 9,
            "Rifle" => 10,
            "Assault Rifle" => 11,
            "Light Machine Gun" => 12,
            "Sniper Rifle" => 13,
            "Sub-Machine Gun" => 14,
            "Shotgun" => 15,
            "Melee" => 16,
            "Strength" => 17,
            "Diplomacy" => 18,
            "Electronics" => 19,
            "Healing" => 20,
            _ => 0,
        };
    }
    public void IncreaseRoundsWithoutFood()
    {
        if (!IsWearingStimulantArmour())
            RoundsWithoutFood++;
    }
    public void ResetRoundsWithoutFood()
    {
        RoundsWithoutFood = 0;
    }
    public void SetLoudRevealed(int turns)
    {
        loudActionTurnsVulnerable = turns;
        print($"{soldierName} is vulnerable to detection for {turns} turns.");
    }

    public bool IsSuppressed()
    {
        if (GetSuppression() > 0)
            return true;
        else
            return false;
    }
    public int GetSuppression()
    {
        float suppressionValueFinal = 1 - (suppressionValue/100f);
        List<float> suppressionValues = new();

        //get all engaged soldiers' suppression values
        foreach (string id in controlledBySoldiersList)
            suppressionValues.Add(1 - (soldierManager.FindSoldierById(id).suppressionValue/100f));
        foreach (string id in controllingSoldiersList)
            suppressionValues.Add(1 - (soldierManager.FindSoldierById(id).suppressionValue/100f));

        //multiply all inverses
        foreach (float val in suppressionValues)
            suppressionValueFinal *= val;

        return Mathf.RoundToInt((1 - suppressionValueFinal)*100);
    }
    public void SetSuppression(int suppression)
    {
        suppressionValue = HelperFunctions.CalculateSuppression(suppressionValue, suppression);
    }
    public void UnsetSuppression()
    {
        suppressionValue = 0;
    }

    public bool IsStunned()
    {
        if (stunnedTurnsVulnerable > 0)
            return true;
        else
            return false;
    }
    public void TakePoisoning(string poisonedBy, bool resistable)
    {
        if (resistable && ResilienceCheck())
        {
            menu.AddXpAlert(this, stats.R.Val, "Resisted poisoning.", true);
            menu.AddDamageAlert(this, $"{soldierName} resisted poisoning.", true, true);
        }
        else
        {
            this.poisonedBy = poisonedBy;
            menu.AddDamageAlert(this, $"{soldierName} was poisoned!", false, true);
            SetPoisoned();
        }
    }
    public bool IsPoisoned()
    {
        if (CheckState("Poisoned"))
            return true;
        else
            return false;
    }
    public void SetPoisoned()
    {
        SetState("Poisoned");
    }

    public void UnsetPoisoned()
    {
        UnsetState("Poisoned");
    }
    public bool IsOnAnyDrug()
    {
        if (IsOnDrug("Modafinil") || IsOnDrug("Amphetamine") || IsOnDrug("Androstenedione") || IsOnDrug("Cannabinoid") ||
            IsOnDrug("Shard") || IsOnDrug("Glucocorticoid") || IsOnDrug("Danazol") || IsOnDrug("Trenbolone"))
            return true;
        return false;
    }
    public bool IsOnDrug(string drugName)
    {
        return CheckState(drugName);
    }
    public void SetOnDrug(string drugName)
    {
        SetState(drugName);
    }
    public void UnsetOnDrug(string drugName)
    {
        switch (drugName)
        {
            case "Amphetamine":
                amphStatReduction = false;
                break;
            case "Androstenedione":
                break;
            case "Cannabinoid":
                break;
            case "Danazol":
                break;
            case "Glucocorticoid":
                glucoState = "";
                break;
            case "Modafinil":
                modaProtect = false;
                break;
            case "Shard":
                break;
            case "Trenbolone":
                trenXRayEffect = false;
                trenSRShrinkEffect = false;
                break;
            default:
                break;
        }
        UnsetState(drugName);
    }
    public bool IsOnOverwatch()
    {
        if (CheckState("Overwatch"))
            return true;
        return false;
    }
    public bool IsInSmoke()
    {
        if (IsSmokeBlinded() || IsSmokeCovered())
            return true;
        return false;
    }
    public bool IsSmokeBlinded()
    {
        if (CheckState("SmokeBlinded"))
            return true;
        return false;
    }
    public bool IsSmokeCovered()
    {
        if (CheckState("SmokeCovered"))
            return true;
        return false;
    }
    public void SetSmokeCovered()
    {
        UnsetState("SmokeBlinded");
        SetState("SmokeCovered");
    }
    public void SetSmokeBlinded()
    {
        UnsetState("SmokeCovered");
        SetState("SmokeBlinded");
    }
    public void UnsetSmoked()
    {
        UnsetState("SmokeCovered");
        UnsetState("SmokeBlinded");
    }
    public bool IsInTabun()
    {
        if (CheckTabunEffectLevel(100) || CheckTabunEffectLevel(50) || CheckTabunEffectLevel(25))
            return true;
        return false;
    }
    public bool CheckTabunEffectLevel(int level)
    {
        if (CheckState($"Tabun{level}"))
            return true;
        return false;
    }
    public void SetTabunEffectLevel(int level)
    {
        state.RemoveAll(e => e.Contains("Tabun"));
        SetState($"Tabun{level}");
    }
    public void SetTabunOuterAffected()
    {
        bool rCheck = ResilienceCheck(), healCheck = HealCheck();

        //experimentalist ability
        if (IsExperimentalist())
        {
            if (!rCheck && !healCheck)
                rCheck = true;
            else if (rCheck ^ healCheck)
            {
                rCheck = true;
                healCheck = true;
            }
        }

        if (rCheck && healCheck)
        {
            menu.AddXpAlert(this, 2, $"{soldierName} resisted tabun gas.", false);
            menu.AddDamageAlert(this, $"Resisted tabun gas.", true, true);
        }
        else if (rCheck ^ healCheck)
        {
            SetTabunEffectLevel(25);
            menu.AddDamageAlert(this, $"Suffered <color=yellow>Light</color> effects from tabun gas.", false, true);
        }
        else
        {
            SetTabunEffectLevel(50);
            menu.AddDamageAlert(this, $"Suffered <color=orange>Moderate</color> effects from tabun gas.", false, true);
        }
    }
    public void SetTabunInnerAffected()
    {
        bool rCheck = ResilienceCheck(), healCheck = HealCheck();

        //experimentalist ability
        if (IsExperimentalist())
            rCheck = true;

        if (rCheck && healCheck)
        {
            menu.AddXpAlert(this, 2, $"Fully resisted tabun gas.", false);
            menu.AddDamageAlert(this, $"Resisted tabun gas.", true, true);
        }
        else if (rCheck ^ healCheck)
        {
            menu.AddXpAlert(this, 1, $"Partially resisted tabun gas.", false);
            SetTabunEffectLevel(50);
            menu.AddDamageAlert(this, $"Suffered <color=orange>Moderate</color> effects from tabun gas.", false, true);

            StartCoroutine(game.DetectionAlertSingle(this, "statChange(P)(C)(SR)|tabunActive(half)", Vector3.zero, string.Empty)); //losCheck
        }
        else
        {
            SetTabunEffectLevel(100);
            menu.AddDamageAlert(this, $"Suffered <color=red>Severe</color> effects from tabun gas.", false, true);

            StartCoroutine(game.DetectionAlertSingle(this, "statChange(P)(C)(SR)|tabunActive(full)", Vector3.zero, string.Empty)); //losCheck
        }
    }
    public void UnsetTabun()
    {
        state.RemoveAll(e => e.Contains("Tabun"));
        TabunTraumaCheck();

        StartCoroutine(game.DetectionAlertSingle(this, "statChange(P)(C)(SR)|tabunDeactive", Vector3.zero, string.Empty)); //losCheck
    }
    public void SetOverwatch(int x, int y, int r, int a)
    {
        overwatchXPoint = x;
        overwatchYPoint = y;
        overwatchConeRadius = r;
        overwatchConeArc = a;
        guardsmanRetryUsed = false;
        overwatchShotCounter = 1;
        SetState("Overwatch");

        StartCoroutine(game.DetectionAlertSingle(this, "losChange|overwatchActive", Vector3.zero, string.Empty)); //losCheck
    }
    public void DecrementOverwatch()
    {
        if (IsOnOverwatch())
        {
            overwatchShotCounter--;
            if (overwatchShotCounter == 0)
                UnsetOverwatch();
        }
    }
    public void UnsetOverwatch()
    {
        if (IsOnOverwatch())
        {
            overwatchShotCounter = 0;
            overwatchXPoint = 0;
            overwatchYPoint = 0;
            overwatchConeRadius = 0;
            overwatchConeArc = 0;
            UnsetState("Overwatch");

            StartCoroutine(game.DetectionAlertSingle(this, "losChange|overwatchDeactive", Vector3.zero, string.Empty)); //losCheck
        }
    }
    public bool IsAvenging()
    {
        if (CheckState("Avenging"))
            return true;
        return false;
    }
    public void SetAvenging()
    {
        turnsAvenging = 2;
        SetState("Avenging");
    }
    public void UnsetAvenging()
    {
        UnsetState("Avenging");
    }
    public bool IsBloodRaged()
    {
        if (CheckState("Bloodrage"))
            return true;

        return false;
    }
    public void SetBloodRage()
    {
        SetState("Bloodrage");
    }
    public void UnsetBloodRage()
    {
        UnsetState("Bloodrage");
    }
    public bool IsInCover()
    {
        if (CheckState("Cover"))
            return true;

        return false;
    }
    public void SetCover()
    {
        SetState("Cover");
    }
    public void UnsetCover()
    {
        UnsetState("Cover");
    }
    public bool IsInteractable()
    {
        if (CheckState("Crushed") || IsMeleeControlled())
            return false;

        return true;
    }
    public void SetCrushed()
    {
        if (IsInteractable())
        {
            SetState("Crushed");
            DestroyAllBreakableItems(null);
        }
    }
    public void UnsetCrushed()
    {
        UnsetState("Crushed");
    }
    public void DropHandheldItems()
    {
        Item leftHand = Inventory.GetItemInSlot("LeftHand"), rightHand = Inventory.GetItemInSlot("RightHand");

        if (leftHand != null)
            Inventory.RemoveItemFromSlot(leftHand, "LeftHand");
        if (rightHand != null)
            Inventory.RemoveItemFromSlot(rightHand, "RightHand");
    }
    public void SetStunned(int stunTurns)
    {
        if (stunTurns > stunnedTurnsVulnerable)
        {
            stunnedTurnsVulnerable = stunTurns;

            DropHandheldItems();

            //remove all engagements
            if (IsMeleeEngaged())
                StartCoroutine(game.DetermineMeleeControllerMultiple(this));

            StartCoroutine(game.DetectionAlertSingle(this, "statChange(C)(SR)|stunActive", Vector3.zero, string.Empty)); //losCheck
        }
    }
    public int TakeStun(int stunRounds)
    {
        if (stunRounds > 0)
        {
            int resistedRounds = 0;
            for (int i = 0; i < stunRounds; i++)
            {
                if (ResilienceCheck())
                    resistedRounds++;
            }

            if (resistedRounds > 0)
            {                
                menu.AddDamageAlert(this, $"Resisted stun ({resistedRounds} rounds).", true, true);
                menu.AddXpAlert(this, resistedRounds, $"Resisted stun ({resistedRounds} rounds).", true);
            }
            if (resistedRounds < stunRounds)
            {
                menu.AddDamageAlert(this, $"Suffered stun ({stunRounds - resistedRounds} rounds).", false, true);
                SetStunned(stunRounds * 2);
            }

            return stunRounds - resistedRounds;
        }

        return 0;
    }
    public void ClearHealthState()
    {
        UnsetState("Active");
        UnsetState("Last Stand");
        UnsetState("Dead");
        UnsetState("Unconscious");
    }
    public void MakeActive()
    {
        ClearHealthState();
        SetState("Active");

        StartCoroutine(game.DetectionAlertSingle(this, "losChange|healthState(active)", Vector3.zero, string.Empty)); //losCheck
    }
    public void MakeLastStand()
    {
        if (IsWearingJuggernautArmour(false))
            menu.AddDamageAlert(this, $"{soldierName} resisted <color=red>Last Stand</color> with JA.", true, true);
        else
        {
            ClearHealthState();
            SetState("Last Stand");
            menu.AddDamageAlert(this, $"{soldierName} fell into <color=red>Last Stand</color>.", false, true);

            StartCoroutine(game.DetectionAlertSingle(this, "losChange|healthState(lastStand)", Vector3.zero, string.Empty)); //losCheck
        }
    }
    public void MakeUnconscious(Soldier damagedBy, List<string> damageSource)
    {
        bleedoutTurns = 2 * ((stats.H.Val + stats.R.Val) / 3);

        ClearHealthState();
        SetState("Unconscious");
        menu.AddDamageAlert(this, $"{soldierName} fell into <color=blue>Unconscious ({bleedoutTurns})</color>.", false, true);
        
        //set up payout for who made soldier uncon
        if (damagedBy != null)
            madeUnconBy = damagedBy.Id;
        else
            madeUnconBy = string.Empty;
        madeUnconBydamageList = damageSource;

        DropHandheldItems();

        //remove all engagements
        if (IsMeleeEngaged())
            StartCoroutine(game.DetermineMeleeControllerMultiple(this));

        StartCoroutine(game.DetectionAlertSingle(this, "losChange|statChange(C)(SR)|healthState(unconscious)", Vector3.zero, string.Empty)); //losCheck
    }
    public void Resurrect(int hp)
    {
        ClearHealthState();
        SetState("Active");
        CheckSpecialityColor(soldierSpeciality);
        TakeHeal(null, hp, 0, true, false);

        StartCoroutine(game.DetectionAlertSingle(this, "losChange|healthState(active)", Vector3.zero, string.Empty));
    }

    public void InstantKill(Soldier killedBy, List<string> damageSource)
    {
        if (IsAlive())
            Kill(killedBy, damageSource);
    }
    public void Kill(Soldier killedBy, List<string> damageSource)
    {
        if (IsAlive())
        {
            if (modaProtect)
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted death with Modafinil. He gets an immediate turn.", true, true);
                game.StartModaTurn(this);
            }
            else 
            {
                menu.AddDamageAlert(this, $"{soldierName} was killed by {menu.PrintList(damageSource)}. He is now <color=red>Dead</color>", false, false);
                int tp = 1;
                bool lastandicide = false;

                //make him dead
                ClearHealthState();
                SetState("Dead");
                hp = 0;

                //remove all reveals and revealedby
                RevealingSoldiers.Clear();
                RevealedBySoldiers.Clear();

                //remove all instances of anyone else revealing them
                foreach (Soldier s in game.AllSoldiers())
                    s.RevealingSoldiers.Remove(id);

                //remove all engagements
                if (IsMeleeEngaged())
                    StartCoroutine(game.DetermineMeleeControllerMultiple(this));
                //check if critical trauma
                if (damageSource.Contains("Critical") || damageSource.Contains("Melee") || damageSource.Contains("Explosive") || damageSource.Contains("Deathroll"))
                    tp++;
                //check if lastandicide
                if (damageSource.Contains("Lastandicide"))
                    lastandicide = true;
                //run trauma check
                StartCoroutine(game.TraumaCheck(this, tp, IsCommander(), lastandicide));
                //remove all LOS
                menu.ConfirmDetections();
                //re-render as dead
                CheckSpecialityColor(soldierSpeciality);

                if (killedBy != null)
                {
                    //pay xp for relevant damage type kill
                    if (damageSource.Contains("Shot"))
                        menu.AddXpAlert(killedBy, game.CalculateShotKillXp(killedBy, this), $"Killed {soldierName} with a shot.", false);
                    else if (damageSource.Contains("Melee"))
                    {
                        if (damageSource.Contains("Counter"))
                            menu.AddXpAlert(killedBy, game.CalculateMeleeCounterKillXp(killedBy, this), $"Killed {soldierName} in melee (counterattack).", false);
                        else
                            menu.AddXpAlert(killedBy, game.CalculateMeleeKillXp(killedBy, this), $"Killed {soldierName} in melee.", false);

                        killedBy.BrawlerMeleeKillReward(damageSource);
                    }
                    else if (damageSource.Contains("Poison"))
                        menu.AddXpAlert(killedBy, 10 + this.stats.R.Val, $"Killed {soldierName} by poisoning.", false);

                    //set haskilled flag for avenger
                    if (this.IsOppositeTeamAs(killedBy))
                        killedBy.hasKilled = true;
                }

                //set fight flags for allied avenger(s)
                foreach (Soldier s in game.AllSoldiers())
                    if (this.IsSameTeamAs(s) && s.IsAvenger())
                        s.SetAvenging();
            }
        }
    }


    //detection checks
    public bool PhysicalObjectWithinOverwatchCone(PhysicalObject obj)
    {
        if (IsOnOverwatch())
        {
            if (overwatchXPoint != 0 && overwatchYPoint != 0 && PhysicalObjectWithinRadius(obj, overwatchConeRadius))
            {
                Vector2 centreLine = new(overwatchXPoint - X, overwatchYPoint - Y);
                Vector2 targetLine = new(obj.X - X, obj.Y - Y);
                centreLine.Normalize();
                targetLine.Normalize();

                if (Vector2.Angle(centreLine, targetLine) <= overwatchConeArc / 2.0f)
                    return true;
            }
        }

        return false;
    }
    public bool PhysicalObjectWithinOverwatchCone(Vector3 point)
    {
        if (IsOnOverwatch())
        {
            if (overwatchXPoint != 0 && overwatchYPoint != 0 && PhysicalObjectWithinRadius(point, overwatchConeRadius))
            {
                Vector2 centreLine = new(overwatchXPoint - X, overwatchYPoint - Y);
                Vector2 targetLine = new(point.x - X, point.y - Y);
                centreLine.Normalize();
                targetLine.Normalize();

                if (Vector2.Angle(centreLine, targetLine) <= overwatchConeArc / 2.0f)
                    return true;
            }
        }

        return false;
    }
    public bool PhysicalObjectWithinMaxRadius(PhysicalObject obj)
    {
        if (PhysicalObjectWithinRadius(obj, this.SRColliderMax.radius))
            return true;

        return false;
    }
    public bool PhysicalObjectWithinHalfRadius(PhysicalObject obj)
    {
        if (PhysicalObjectWithinRadius(obj, this.SRColliderHalf.radius))
            return true;

        return false;
    }
    public bool PhysicalObjectWithinMinRadius(PhysicalObject obj)
    {
        if (PhysicalObjectWithinRadius(obj, this.SRColliderMin.radius))
            return true;

        return false;
    }
    public bool PhysicalObjectWithinItemRadius(PhysicalObject obj)
    {
        if (PhysicalObjectWithinRadius(obj, this.itemCollider.radius))
            return true;

        return false;
    }
    public bool PhysicalObjectWithinMeleeRadius(PhysicalObject obj)
    {
        if (PhysicalObjectWithinRadius(obj, this.SRColliderMin.radius))
            return true;

        return false;
    }
    public bool TerminalInRange()
    {
        foreach (Terminal t in FindObjectsByType<Terminal>(default))
            if (PhysicalObjectWithinMeleeRadius(t))
                return true;
        return false;
    }
    public Terminal ClosestTerminal()
    {
        if (TerminalInRange())
        {
            List<Tuple<float, Terminal>> soldierDistanceToTerminals = new();

            foreach (Terminal t in FindObjectsByType<Terminal>(default))
                soldierDistanceToTerminals.Add(Tuple.Create(game.CalculateRange(this, t), t));

            soldierDistanceToTerminals = soldierDistanceToTerminals.OrderBy(t => t.Item1).ToList();

            if (soldierDistanceToTerminals.Count > 0)
                return soldierDistanceToTerminals[0].Item2;
        }

        return null;
    }
    public Soldier ClosestAlly()
    {
        List<Tuple<float, Soldier>> soldierDistances = new();

        foreach (Soldier s in game.AllSoldiers())
            if (s.IsAlive() && this.IsSameTeamAs(s))
                soldierDistances.Add(Tuple.Create(game.CalculateRange(this, s), s));

        soldierDistances = soldierDistances.OrderBy(t => t.Item1).ToList();

        if (soldierDistances.Count > 0)
            return soldierDistances[0].Item2;

        return null;
    }
    public Soldier ClosestAllyForPlannerBuff()
    {
        List<Tuple<float, Soldier>> soldierDistances = new();

        foreach (Soldier s in game.AllSoldiers())
            if (this.IsSameTeamAs(s) && s.IsAbleToWalk() && !s.IsRevoker())
                soldierDistances.Add(Tuple.Create(game.CalculateRange(this, s), s));

        soldierDistances = soldierDistances.OrderBy(t => t.Item1).ToList();

        if (soldierDistances.Count > 0)
            return soldierDistances[0].Item2;

        return null;
    }
    public Soldier ClosestEnemy()
    {
        List<Tuple<float, Soldier>> soldierDistances = new();

        foreach (Soldier s in game.AllSoldiers())
            if (s.IsAlive() && this.IsOppositeTeamAs(s))
                soldierDistances.Add(Tuple.Create(game.CalculateRange(this, s), s));

        soldierDistances = soldierDistances.OrderBy(t => t.Item1).ToList();

        if (soldierDistances.Count > 0)
            return soldierDistances[0].Item2;
        else
            return null;
    }
    public Soldier ClosestEnemyVisible()
    {
        List<Tuple<float, Soldier>> soldierDistances = new();

        foreach (Soldier s in game.AllSoldiers())
            if (s.IsAlive() && this.IsOppositeTeamAs(s) && s.IsRevealed())
                soldierDistances.Add(Tuple.Create(game.CalculateRange(this, s), s));

        soldierDistances = soldierDistances.OrderBy(t => t.Item1).ToList();

        if (soldierDistances.Count > 0)
            return soldierDistances[0].Item2;
        return null;
    }
    public bool PhysicalObjectIsRevealed(PhysicalObject obj)
    {
        bool objectIsRevealed = false;

        foreach (Soldier s in game.AllSoldiers())
            if (s.IsAbleToSee() && this.IsSameTeamAsIncludingSelf(s))
                if (game.CalculateRange(s, obj) <= s.SRColliderMax.radius)
                    objectIsRevealed = true;
        return objectIsRevealed;
    }
    public bool CheckSmokeClouds()
    {
        SmokeCloud[] allSmokeClouds = FindObjectsByType<SmokeCloud>(default);
        if (allSmokeClouds.Length > 0)
        {
            if (IsAlive())
            {
                bool currentlyInSmoke = false;
                foreach (SmokeCloud cloud in allSmokeClouds)
                {
                    if (cloud.TurnsUntilDissipation > 0)
                    {
                        if (PhysicalObjectWithinRadius(cloud, 5))
                        {
                            currentlyInSmoke = true;
                            SetSmokeBlinded();
                        }
                        else if (PhysicalObjectWithinRadius(cloud, 20))
                        {
                            currentlyInSmoke = true;
                            SetSmokeCovered();
                        }


                        //if soldier wasn't in smoke before check and becomes smoke covered, increment soldiers covered for xp purposes
                        if (IsInSmoke())
                        {
                            if (this.IsSameTeamAs(cloud.placedBy) && !cloud.alliesAffected.Contains(Id))
                                cloud.alliesAffected.Add(Id);
                            else if (this.IsOppositeTeamAs(cloud.placedBy) && !cloud.enemiesAffected.Contains(Id))
                                cloud.enemiesAffected.Add(Id);
                        }
                    }
                    else
                        game.poiManager.DestroyPOI(cloud);
                }

                if (currentlyInSmoke)
                    return true;
            }
        }
            
        return false;
    }
    public bool CheckTabunClouds()
    {
        TabunCloud[] allTabunClouds = FindObjectsByType<TabunCloud>(default);
        print($"running function checktabunclouds for {this.soldierName}");
        if (allTabunClouds.Length > 0)
        {
            print($"there are clouds to test {this.soldierName}");
            if (IsAlive() && !IsWearingStimulantArmour())
            {
                print($"testing soldier {this.soldierName}");
                bool currentlyInTabun = false;
                foreach (TabunCloud cloud in allTabunClouds)
                {
                    if (cloud.TurnsUntilDissipation > 0)
                    {
                        print($"testing tabun cloud {cloud.Id}");
                        if (PhysicalObjectWithinRadius(cloud, 5))
                        {
                            print($"{this.soldierName} in inner tabun cloud {cloud.Id}");
                            currentlyInTabun = true;
                            SetTabunInnerAffected();
                        }
                        else if (PhysicalObjectWithinRadius(cloud, 20))
                        {
                            print($"{this.soldierName} in outer tabun cloud {cloud.Id}");
                            currentlyInTabun = true;
                            SetTabunOuterAffected();
                        }

                        //if soldier wasn't in smoke before check and becomes smoke covered, increment soldiers covered for xp purposes
                        if (IsInTabun())
                        {
                            if (this.IsSameTeamAs(cloud.placedBy) && !cloud.alliesAffected.Contains(Id))
                                cloud.alliesAffected.Add(Id);
                            else if (this.IsOppositeTeamAs(cloud.placedBy) && !cloud.enemiesAffected.Contains(Id))
                                cloud.enemiesAffected.Add(Id);
                        }
                    }
                    else
                        game.poiManager.DestroyPOI(cloud);
                }

                if (currentlyInTabun)
                    return true;
            }
        }
        return false;
    }
    public bool HandsFreeToUseItem(Item item)
    {
        string message = "";
        if (IsWearingJuggernautArmour(false) && !(item.IsGun() || item.IsGrenade() || item.IsRiotShield()))
            message = "Juggernaut Armour Blocking";
        else if (item.IsRiotShield() && !HasAHandFree(true))
            message = "Hands Full";
        else if (item.IsLargeMedikit() && !HasAHandFree(true))
            message = "Hands Full";
        else if (!IsValidLoadout())
        {
            if (!(HasAHandFree(false) && item.whereEquipped.Contains("Hand")))
                message = "Hands Full";
        }

        print(message);

        if (message == "")
            return true;
        else
            menu.OpenCannotUseItemUI(message);

        return false;
    }










    //status checks
    public bool ResilienceCheck()
    {
        if (game.DiceRoll() <= stats.R.Val)
            return true;
        return false;
    }
    public bool StrengthCheck()
    {
        if (game.DiceRoll() <= stats.Str.Val)
            return true;
        return false;
    }
    public bool HealCheck()
    {
        if (game.DiceRoll() <= stats.Heal.Val)
            return true;
        return false;
    }
    public bool SuppressionCheck()
    {
        if (suppressionValue > 0)
        {
            if (ResilienceCheck())
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted {suppressionValue} suppression.", true, true);
                menu.AddXpAlert(this, 1, "Resisted Suppression.", true);
                return true;
            }
            else
            {
                menu.AddDamageAlert(this, $"{soldierName} failed to resist {suppressionValue} suppression.", false, true);
                return false;
            }
        }
            
        return true;
    }
    public bool StructuralCollapseCheck(int structureHeight)
    {
        int survivalPassesNeeded = Mathf.FloorToInt(structureHeight / 10f);
        int survivalPassesAchieved = 0;
        int survivalAttempts = 1;

        if (IsWearingJuggernautArmour(true))
            survivalAttempts += 2;
        else if (IsWearingBodyArmour(true))
            survivalAttempts++;

        for (int i = 0; i < survivalPassesNeeded; i++)
        {
            for (int j = 0; j < survivalAttempts; j++)
            {
                if (ResilienceCheck())
                {
                    survivalPassesAchieved++;
                    break;
                }
            }
        }

        if (survivalPassesAchieved >= survivalPassesNeeded)
            return true;
        return false;
    }
    public bool IsCommander()
    {
        if (soldierSpeciality == "Leadership")
            return true;
        return false;
    }
    public bool IsMedic()
    {
        if (soldierSpeciality == "Healing")
            return true;
        return false;
    }
    public bool IsMeleeEngaged()
    {
        if (controlledBySoldiersList.Count > 0 || controllingSoldiersList.Count > 0)
            return true;
        return false;
    }
    public bool IsMeleeControlled()
    {
        if (controlledBySoldiersList.Count > 0)
            return true;
        return false;
    }
    public bool IsMeleeControlling()
    {
        if (controllingSoldiersList.Count > 0 && controlledBySoldiersList.Count == 0)
            return true;
        return false;
    }
    public bool IsMeleeEngagedWith(Soldier s)
    {
        if (controlledBySoldiersList.Contains(s.id) || controllingSoldiersList.Contains(s.id))
            return true;
        return false;
    }
    public bool IsOnOppositeTerrain()
    {
        if ((TerrainOn == "Alpine" && soldierTerrain == "Desert")
            || (TerrainOn == "Jungle" && soldierTerrain == "Urban")
            || (TerrainOn == "Desert" && soldierTerrain == "Alpine")
            || (TerrainOn == "Urban" && soldierTerrain == "Jungle"))
            return true;
        return false;
    }
    public bool IsOnNativeTerrain()
    {
        if (TerrainOn == soldierTerrain)
            return true;
        return false;
    }
    public bool IsBroken()
    {
        if (tp == 4)
            return true;
        return false;
    }
    public bool IsFrozen()
    {
        if (tp == 3)
            return true;
        return false;
    }
    public bool IsDesensitised()
    {
        if (tp >= 5)
            return true;
        return false;
    }
    public bool IsNotDesensitised()
    {
        if (!IsDesensitised())
            return true;
        return false;
    }
    public int GetKd()
    {
        int kd = 0;
        foreach (Soldier s in game.AllSoldiers())
        {
            if (this.IsOppositeTeamAs(s) && s.IsDead())
                kd++;
            else if (this.IsSameTeamAs(s) && s.IsDead())
                kd--;
        }

        return kd;
    }
    public bool FightActive()
    {
        if (GetKd() < 0)
            return true;

        return false;
    }
    public bool AvengingActive()
    {
        if (!FightActive() && IsAvenging() && stats.F.Val > 1)
            return true;

        return false;
    }










    //item checks
    public bool HasAHandFree(bool fullyFree)
    {
        if (fullyFree)
        {
            print("checking hands fully free");
            if (LeftHandItem == null && RightHandItem == null)
                return true;
            else if (LeftHandItem != null && RightHandItem == null)
                return true;
            else if (RightHandItem != null && LeftHandItem == null)
                return true;
        }
        else
        {
            print("checking hands free");
            if (LeftHandItem == null && RightHandItem == null)
                return true;
            else if (LeftHandItem != null && (RightHandItem == null || RightHandItem.IsWeapon() || RightHandItem.IsSMG() || RightHandItem.IsPistol()))
                return true;
            else if (RightHandItem != null && (LeftHandItem == null || LeftHandItem.IsWeapon() || LeftHandItem.IsSMG() || LeftHandItem.IsPistol()))
                return true;
        }
        
        return false;
    }
    public bool HasGunsEquipped()
    {
        if (EquippedGuns.Any())
            return true;
        return false;
    }
    public bool HasSingleGunEquipped()
    {
        if (HasGunsEquipped() && EquippedGuns.Count == 1)
            return true;
        return false;
    }
    public bool HasTwoGunsEquipped()
    {
        if (HasGunsEquipped() && EquippedGuns.Count == 2)
            return true;
        return false;
    }
    public Item GetEquippedGun(string gunName)
    {
        foreach (Item gun in EquippedGuns)
            if (gun.itemName.Equals(gunName))
                return gun;
        return null;
    }
    public bool HasAnyAmmo()
    {
        if (EquippedGuns.Any())
        {
            foreach (Item gun in EquippedGuns)
                if (gun.CheckAnyAmmo())
                    return true;
        }
        return false;
    }
    public bool HasSMGsOrPistolsEquipped()
    {
        if (HasGunsEquipped())
        {
            foreach (Item gun in EquippedGuns)
                if (!gun.IsPistol() && !gun.IsSMG())
                    return false;
        }
        return true;
    }
    public bool HasArmourIntegrity()
    {
        if (IsWearingJuggernautArmour(true) || IsWearingBodyArmour(true))
            return true;
        return false;
    }
    public bool IsWearingThermalGoggles()
    {
        if (Inventory.HasItemOfType("Thermal_Goggles"))
            return true;
        return false;
    }
    public bool IsWearingBodyArmour(bool requiresIntegrity)
    {
        if (requiresIntegrity)
        {
            if (Inventory.HasItemOfType("Armour_Body") && Inventory.GetItem("Armour_Body").ablativeHealth > 0)
                return true;
        }
        else
        {
            if (Inventory.HasItemOfType("Armour_Body"))
                return true;
        }
        return false;
    }
    public bool IsWearingJuggernautArmour(bool requiresIntegrity)
    {
        if (requiresIntegrity)
        {
            if (Inventory.HasItemOfType("Armour_Juggernaut") && Inventory.GetItem("Armour_Juggernaut").ablativeHealth > 0)
                return true;
        }
        else
        {
            if (Inventory.HasItemOfType("Armour_Juggernaut"))
                return true;
        }
        return false;
    }
    public bool IsWearingExoArmour()
    {
        if (Inventory.HasItemOfType("Armour_Exo"))
            return true;
        return false;
    }
    public bool IsWearingGhillieArmour()
    {
        if (Inventory.HasItemOfType("Armour_Ghillie"))
            return true;
        return false;
    }
    public bool IsWearingStimulantArmour()
    {
        if (Inventory.HasItemOfType("Armour_Stimulant"))
            return true;
        return false;
    }
    public bool IsCarryingRiotShield()
    {
        if (Inventory.HasItemOfType("Riot_Shield"))
            return true;
        return false;
    }
    public bool HasActiveRiotShield()
    {
        if (IsCarryingRiotShield() && HasAHandFree(true))
            return true;
        return false;
    }
    public bool HasActiveAndCorrectlyAngledRiotShield(Vector3 damageOriginPoint)
    {
        if(HasActiveRiotShield() && HelperFunctions.IsWithinAngle(new(riotXPoint, riotYPoint), damageOriginPoint, new(X, Y), 67.5f))
            return true;
        return false;
    }
    public bool IsWearingLogisticsBelt()
    {
        if (Inventory.HasItemOfType("Logistics_Belt"))
            return true;
        return false;
    }
    public bool IsValidLoadout()
    {
        // Check if both hands are empty
        if (LeftHandItem == null && RightHandItem == null)
            return true;

        // Check if one hand is a gun and the other is empty
        if ((LeftHandItem != null && LeftHandItem.IsGun() && RightHandItem == null) || (RightHandItem != null && RightHandItem.IsGun() && LeftHandItem == null))
            return true;

        // Check if a weapon is equipped in either hand and the other hand is empty or contains a pistol, SMG or weapon
        if ((LeftHandItem != null && LeftHandItem.IsWeapon() && (RightHandItem == null || RightHandItem.IsPistol() || RightHandItem.IsSMG() || RightHandItem.IsWeapon())) || (RightHandItem != null && RightHandItem.IsWeapon() && (LeftHandItem == null || LeftHandItem.IsPistol() || LeftHandItem.IsSMG() || LeftHandItem.IsWeapon())))
            return true;

        // Check if a pistol is equipped in either hand and the other hand is empty or contains a pistol, SMG or weapon
        if ((LeftHandItem != null && LeftHandItem.IsPistol() && (RightHandItem == null || RightHandItem.IsPistol() || RightHandItem.IsSMG() || RightHandItem.IsWeapon())) || (RightHandItem != null && RightHandItem.IsPistol() && (LeftHandItem == null || LeftHandItem.IsPistol() || LeftHandItem.IsSMG() || LeftHandItem.IsWeapon())))
            return true;

        // Check if an SMG is equipped in either hand and the other hand is empty or contains a pistol, SMG or weapon
        if ((LeftHandItem != null && LeftHandItem.IsSMG() && (RightHandItem == null || RightHandItem.IsPistol() || RightHandItem.IsSMG() || RightHandItem.IsWeapon())) || (RightHandItem != null && RightHandItem.IsSMG() && (LeftHandItem == null || LeftHandItem.IsPistol() || LeftHandItem.IsSMG() || LeftHandItem.IsWeapon())))
            return true;

        return false;
    }
    public void DropOtherMeleeWeapon()
    {
        int leftMelee, rightMelee;
        if (LeftHandItem == null)
            leftMelee = 1;
        else
            leftMelee = LeftHandItem.meleeDamage;

        if (RightHandItem == null)
            rightMelee = 1;
        else
            rightMelee = RightHandItem.meleeDamage;

        if (rightMelee > leftMelee)
        {
            if (LeftHandItem != null && !IsValidLoadout())
                Inventory.RemoveItemFromSlot(LeftHandItem, "LeftHand");
        }
        else
        {
            if (RightHandItem != null && !IsValidLoadout())
                Inventory.RemoveItemFromSlot(RightHandItem, "RightHand");
        }
    }














    //ability checks
    public bool IsAdept()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Adept"))
                return true;

        return false;
    }
    public bool IsAvenger()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Avenger"))
                return true;

        return false;
    }
    public bool IsBloodletter()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Bloodletter"))
                return true;

        return false;
    }
    public void TakeBloodlettingDamage()
    {
        stats.S.BaseVal += 3;
        bloodLettedThisTurn = true;
        SetBloodRage();
        stats.H.BaseVal--;
        TakeDamage(this, 1, true, new() { "Bloodletting" });
    }
    public bool IsExperimentalist()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Experimentalist"))
                return true;

        return false;
    }
    public bool IsBull()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Bull"))
                return true;

        return false;
    }
    public bool IsCalculator()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Calculator"))
                return true;

        return false;
    }
    public int CalculatorBonus()
    {
        if (IsCalculator())
            return 1;

        return 0;
    }
    public bool IsDissuader()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Dissuader"))
                return true;

        return false;
    }
    public void SetDissuaded()
    {
        SetState("Dissuaded");
    }
    public void UnsetDissuaded()
    {
        UnsetState("Dissuaded");
    }
    public bool IsDissuaded()
    {
        if (CheckState("Dissuaded"))
            return true;

        return false;
    }
    public int DissuaderPenalty()
    {
        if (IsDissuaded())
            return -1;

        return 0;
    }
    public bool IsBrawler()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Brawler"))
                return true;

        return false;
    }
    public void BrawlerMeleeHitReward()
    {
        if (IsBrawler())
        {
            stats.SR.BaseVal += 5;
            stats.S.BaseVal += 3;
        }
    }
    public void BrawlerMeleeKillReward(List<string> damageSource)
    {
        if (IsBrawler())
        {
            ap += 3;
            if (damageSource.Contains("Charge"))
                mp += 1;
        }
    }
    public bool IsGuardsman()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Guardsman"))
                return true;

        return false;
    }
    public bool IsGunner()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Gunner"))
                return true;

        return false;
    }
    public bool IsInsulator()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Insulator"))
                return true;

        return false;
    }
    public bool IsIllusionist()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Illusionist"))
                return true;

        return false;
    }
    public bool IsInspirer()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Inspirer"))
                return true;

        return false;
    }
    public void SetInspired()
    {
        SetState("Inspired");
        if (soldierSpeciality == "Health")
            TakeHeal(null, 1, 0, true, false);
    }
    public void UnsetInspired()
    {
        if (IsInspired())
        {
            UnsetState("Inspired");
            if (soldierSpeciality == "Health")
                TakeDamage(null, 1, true, new() { "Inspirer Debuff" });
        }
    }
    public bool IsInspired()
    {
        if (CheckState("Inspired"))
            return true;

        return false;
    }
    public int InspirerBonusSupport()
    {
        if (IsInspired())
        {
            return soldierSpeciality switch
            {
                "Strength" or "Diplomacy" or "Electronics" or "Healing" => 1,
                _ => 0,
            };
        }

        return 0;
    }
    public int InspirerBonusWeapon(Item gun)
    {
        if (IsInspired() && gun.SpecialityTag() == soldierSpeciality)
            return 5;

        return 0;
    }
    public float InspirerBonusWeaponMelee()
    {
        if (IsInspired() && soldierSpeciality == "Melee")
            return 0.5f;

        return 0;
    }
    public bool IsInformer()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Informer"))
                return true;

        return false;
    }
    public bool IsJammer()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Jammer"))
                return true;

        return false;
    }
    public bool IsLocator()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Locator"))
                return true;

        return false;
    }
    public int LocatorBonus()
    {
        if (IsLocator())
            return 1;

        return 0;
    }
    public bool IsLearner()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Learner"))
                return true;

        return false;
    }
    public bool IsPolitician()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Politician"))
                return true;

        return false;
    }
    public int PoliticianBonus()
    {
        if (IsPolitician())
            return 1;

        return 0;
    }
    public bool IsPlanner()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Planner"))
                return true;

        return false;
    }
    public bool IsPatriot()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Patriot"))
                return true;

        return false;
    }
    public void SetPatriotic()
    {
        if (IsPatriot() && IsOnNativeTerrain())
            patriotic = true;
    }
    public void UnsetPatriotic()
    {
        patriotic = false;
    }
    public bool IsPatriotic()
    {
        if (patriotic)
            return true;
        return false;
    }
    public bool IsRevoker()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Revoker"))
                return true;
        return false;
    }
    public bool IsShadow()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Shadow"))
                return true;
        return false;
    }
    public int ShadowXpBonus(bool revoked)
    {
        if (!revoked)
        {
            if (IsShadow())
                return 1;
        }

        return 0;
    }
    public bool IsSharpshooter()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Sharpshooter"))
                return true;
        return false;
    }
    public bool IsSprinter()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Sprinter"))
                return true;
        return false;
    }
    public bool IsSpotter()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Spotter"))
                return true;
        return false;
    }
    public bool IsSpotted()
    {
        if (isSpottedBy.Count > 0)
            return true;
        return false;
    }
    public bool IsSpotting()
    {
        if (IsSpotter())
            if (isSpotting != string.Empty)
                return true;
        return false;
    }
    public void SetSpotting(Soldier spottingSoldier)
    {
        if (!spottingSoldier.isSpottedBy.Contains(Id))
        {
            spottingSoldier.isSpottedBy.Add(Id);
            isSpotting = spottingSoldier.Id;
        }
    }
    public void RemoveSpottedBy(string spotterId)
    {
        isSpottedBy.Remove(spotterId);
    }
    public void RemoveAllSpotting()
    {
        if (IsSpotting())
        {
            game.soldierManager.FindSoldierById(isSpotting).RemoveSpottedBy(Id);
            isSpotting = string.Empty;
        }
    }
    public bool IsTactician()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Tactician"))
                return true;

        return false;
    }
    public int TacticianBonus()
    {
        if (IsTactician())
            return 1;

        return 0;
    }
    public bool IsTranquiliser()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Tranquiliser"))
                return true;

        return false;
    }
    public bool IsWitness()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Witness"))
                return true;

        return false;
    }
















    //status
    public void PaintSpeciality(Transform soldierStatsUI)
    {
        TextMeshProUGUI[] statLabels = soldierStatsUI.Find("Stats").Find("Labels").GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI t in statLabels)
        {
            foreach (string[] s in menu.AllStats)
            {
                Color displayColor = Color.white;
                if (t.text == s[0] && s[1] == soldierSpeciality)
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
        
        if (hp == stats.H.Val)
            return "Full Health";
        else if (hp == 0)
            return "<color=red>Dead</color>";
        else if (hp == 3)
            return "<color=red>Critically Injured</color>";
        else if (hp > stats.H.Val)
            return "<color=green>Overhealth</color>";
        else if (hp <= stats.H.Val / 2)
            return "<color=orange>Severely Injured</color>";
        else
            return "<color=yellow>Injured</color>";
    }

    public string GetConsciousState()
    {
        if (IsUnconscious())
            return $", <color=blue>Unconscious ({bleedoutTurns})</color>";
        else if (IsLastStand())
            return ", <color=red>Last Stand</color>";
        else
            return "";
    }
    public string GetArmourState()
    {
        if (GetArmourHP() > 0)
            return $", <color=green>Armoured({GetArmourHP()})</color>";
        return "";
    }
    public string GetFightState()
    {
        if (FightActive())
            return $", <color=green>Fighting({stats.F.Val})</color>";
        else if (AvengingActive())
            return $", <color=green>Avenging({stats.F.Val - 1})</color>";

        return "";
    }
    public string GetTraumaState()
    {
        return tp switch
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
        if (IsStunned())
            return $", <color=red>Stunned({stunnedTurnsVulnerable})</color>";
        return "";
    }

    public string GetHungerState()
    {
        if (RoundsWithoutFood >= 30)
            return ", <color=red>Starving</color>";
        else if (RoundsWithoutFood >= 20)
            return ", <color=orange>Very Hungry</color>";
        else if (RoundsWithoutFood >= 10)
            return ", <color=yellow>Hungry</color>";
        else
            return "";
    }
    public string GetMeleeControlState()
    {
        string controlString = "";

        if (controllingSoldiersList.Count > 0)
        {
            controlString += ", <color=green>Controlling (";

            for (int i = 0; i < controllingSoldiersList.Count; i++)
            {
                if (i > 0)
                    controlString += ", " + soldierManager.FindSoldierById(controllingSoldiersList[i]).soldierName;
                else
                    controlString += soldierManager.FindSoldierById(controllingSoldiersList[i]).soldierName;
            }
            controlString += ")</color>";
        }

        if (controlledBySoldiersList.Count > 0)
        {
            controlString += ", <color=red>Controlled By (";

            for (int i = 0; i < controlledBySoldiersList.Count; i++)
            {
                if (i > 0)
                    controlString += ", " + soldierManager.FindSoldierById(controlledBySoldiersList[i]).soldierName;
                else
                    controlString += soldierManager.FindSoldierById(controlledBySoldiersList[i]).soldierName;
            }
            controlString += ")</color>";
        }

        return controlString;
    }
    public string GetCoverState()
    {
        if (IsInCover())
            return ", <color=green>Taking Cover</color>";
        return "";
    }
    public string GetOverwatchState()
    {
        if (IsOnOverwatch())
            return $", <color=green>Overwatch ({overwatchXPoint},{overwatchYPoint})</color>";
        return "";
    }
    public string GetLoudDetectedState()
    {
        if (loudActionTurnsVulnerable > 0)
            return $", <color=red>Vulnerable({loudActionTurnsVulnerable})</color>";
        return "";
    }

    public string GetPoisonedState()
    {
        if (IsPoisoned())
            return ", <color=red>Poisoned</color>";
        return "";
    }

    public string GetSuppressionState()
    {
        if (GetSuppression() > 0)
            return $", <color=orange>Suppressed ({GetSuppression()})</color>";
        return "";
    }

    public string GetPlaydeadState()
    {
        if (IsPlayingDead())
            return ", <color=yellow>Playdead</color>";
        return "";
    }
    public string GetSmokedState()
    {
        if (IsSmokeBlinded())
            return $", <color=red>Smoke - Blind Zone</color>";
        else if (IsSmokeCovered())
            return $", <color=red>Smoke - Defence Zone</color>";
        return "";
    }
    public string GetTabunedState()
    {
        if (IsInTabun())
        {
            if (CheckTabunEffectLevel(100))
                return $", <color=red>Tabun - Severe</color>";
            else if (CheckTabunEffectLevel(50))
                return $", <color=red>Tabun - Moderate</color>";
            else if (CheckTabunEffectLevel(25))
                return $", <color=red>Tabun - Light</color>";
        }
        
        return "";
    }
    public string GetDrugState()
    {
        string drugState = "";

        if (IsOnDrug("Modafinil"))
            drugState += ", <color=purple>Moda</color>";
        if (IsOnDrug("Amphetamine"))
            drugState += ", <color=purple>Amph</color>";
        if (IsOnDrug("Androstenedione"))
            drugState += ", <color=purple>Andro</color>";
        if (IsOnDrug("Cannabinoid"))
            drugState += ", <color=purple>Canna</color>";
        if (IsOnDrug("Shard"))
            drugState += ", <color=purple>Shard</color>";
        if (IsOnDrug("Glucocorticoid"))
        {
            drugState += ", <color=purple>Gluco</color>";
            if (glucoState == "side")
            {
                if (mp == 0)
                    drugState += "<color=purple>(Immobilised)</color>";
                else
                    drugState += "<color=purple>(Rush)</color>";
            }
        }
        if (IsOnDrug("Danazol"))
            drugState += ", <color=purple>Dana</color>";
        if (IsOnDrug("Trenbolone"))
            drugState += ", <color=purple>Tren</color>";

        return drugState;
    }
    public string GetPlannerBuffState()
    {
        if (plannerDonatedMove > 0)
            return ", <color=green>Planner Buff</color>";
        return "";
    }
    public string GetPatriotState()
    {
        if (IsPatriotic())
            return ", <color=green>Patriotic</color>";
        return "";
    }
    public string GetInspiredState()
    {
        if (IsInspired())
            return ", <color=green>Inspired</color>";
        return "";
    }
    public string GetDissuadedState()
    {
        if (IsDissuaded())
            return ", <color=red>Dissuaded</color>";
        return "";
    }
    public string GetBloodRageState()
    {
        if (IsBloodRaged())
            return ", <color=green>Bloodrage</color>";
        return "";
    }
    public string GetSpottingState()
    {
        if (IsSpotting())
            return $", <color=green>Spotting ({soldierManager.FindSoldierById(isSpotting).soldierName})</color>";
        return "";
    }
    public string GetWitnessState()
    {
        if (IsWitness() && witnessStoredAbilities.Count != 0)
            return $", <color=green>Witnessing ({menu.PrintList(soldierAbilities.Where(ability => witnessStoredAbilities.Contains(ability)).ToList())})</color>";
        return "";
    }
    public string GetStatus()
    {
        string status = "";
        status += GetHealthState();

        if (IsAlive())
        {
            status += GetConsciousState();
            status += GetArmourState();
            status += GetTraumaState();
            status += GetFightState();
            status += GetStunnedState();
            status += GetHungerState();
            status += GetLoudDetectedState();
            status += GetMeleeControlState();
            status += GetOverwatchState();
            status += GetCoverState();
            status += GetPoisonedState();
            status += GetSuppressionState();
            status += GetPlaydeadState();
            status += GetSmokedState();
            status += GetTabunedState();
            status += GetDrugState();

            status += GetPlannerBuffState();
            status += GetPatriotState();
            status += GetInspiredState();
            status += GetDissuadedState();
            status += GetBloodRageState();
            status += GetSpottingState();
            status += GetWitnessState();
        }
        return status;
    }














    //properties
    public int FullMove
    {
        get { return InstantSpeed; }
    }
    public int HalfMove
    {
        get { return Mathf.RoundToInt(InstantSpeed / 2f); }
    }
    public int TileMove
    {
        get { return 3; }
    }
    public int FullMoveSuppressed
    {
        get { return Mathf.RoundToInt(CalculateInstantSpeedSuppressed()); }
    }
    public int HalfMoveSuppressed
    {
        get { return Mathf.RoundToInt(CalculateInstantSpeedSuppressed() / 2f); }
    }
    public int DragMove
    {
        get { return stats.Str.Val; }
    }
    public int InstantSpeed
    {
        get 
        { 
            CalculateInstantSpeed(); 
            return instantSpeed; 
        }
    }
    public int ThrowRadius
    {
        get { return 10 * stats.Str.Val; }
    }
    public int RoundsWithoutFood
    {
        get { return roundsWithoutFood; }
        set { roundsWithoutFood = value; ApplySustenanceMods(); }
    }
    public List<Soldier> EngagedSoldiers
    {
        get
        {
            List<Soldier> engagedSoldiers = new();
            foreach(string id in controlledBySoldiersList)
                engagedSoldiers.Add(soldierManager.FindSoldierById(id));
            foreach (string id in controllingSoldiersList)
                engagedSoldiers.Add(soldierManager.FindSoldierById(id));
            return engagedSoldiers;
        }
    }
    public List<string> RevealedBySoldiers
    {
        get { return revealedBySoldiersList; }
        set
        {
            //print("Setting RevealedBySoldiers");
            if (revealedBySoldiersList.Any() && !value.Any() && IsAlive() && !IsPlayingDead())
            {
                revealedBySoldiersList = value;
                menu.AddLostLosAlert(this);
                StartCoroutine(menu.OpenLostLOSList());
            }
            else
                revealedBySoldiersList = value;
        }
    }

    public List<string> RevealingSoldiers
    {
        get { return revealingSoldiersList; }
        set 
        { 
            revealingSoldiersList = value;

            if (IsDissuader())
            {
                foreach (string id in revealingSoldiersList)
                {
                    Soldier soldier = soldierManager.FindSoldierById(id);
                    if (soldier != null && !soldier.IsRevoker())
                        soldier.SetDissuaded();
                }
            }
        }
    }
    public Item LeftHandItem
    {
        get 
        {
            InventorySlots.TryGetValue("LeftHand", out string leftHand);
            return itemManager.FindItemById(leftHand);
        }
    }
    public Item RightHandItem
    {
        get
        {
            InventorySlots.TryGetValue("RightHand", out string rightHand);
            return itemManager.FindItemById(rightHand);
        }
    }
    public Item BestMeleeWeapon
    {
        get
        {
            int leftMelee, rightMelee;
            if (LeftHandItem == null)
                leftMelee = 1;
            else
                leftMelee = LeftHandItem.meleeDamage;

            if (RightHandItem == null)
                rightMelee = 1;
            else
                rightMelee = RightHandItem.meleeDamage;

            if (rightMelee > leftMelee)
                return RightHandItem;
            else
                return LeftHandItem;
        }
    }                  
    public List<Item> EquippedGuns
    {
        get
        {
            List<Item> gunsEquipped = new();
            if (LeftHandItem != null && LeftHandItem.IsGun())
                gunsEquipped.Add(LeftHandItem);
            if (RightHandItem != null && RightHandItem.IsGun())
                gunsEquipped.Add(RightHandItem);

            return gunsEquipped;
        }
    }
    public Inventory Inventory { get { return inventory; } }
    public GameObject GameObject { get { return gameObject; } }
    public List<string> InventoryList { get { return inventoryList; } }
    public Dictionary<string, string> InventorySlots { get { return inventorySlots; } }
    public int ActiveC { get { return stats.C.Val; } }
}
