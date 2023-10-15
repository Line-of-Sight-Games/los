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

public class Soldier : PhysicalObject, IDataPersistence, IHaveInventory, IAmShootable
{
    public Dictionary<string, object> details;
    public string soldierName, soldierTerrain, soldierSpeciality;
    public List<string> soldierAbilities;
    public int soldierTeam;
    public int soldierDisplayPriority;
    public Sprite soldierPortrait;
    public string soldierPortraitText;
    public bool fielded, selected, revealed, usedAP, usedMP, bloodLettedThisTurn, illusionedThisMove, hasKilled, overwatchFirstShotUsed, guardsmanRetryUsed;
    public int hp, ap, mp, tp, xp;
    public string rank;
    public int instantSpeed, roundsFielded, roundsFieldedConscious, roundsWithoutFood, loudActionRoundsVulnerable, stunnedRoundsVulnerable, overwatchShotCounter, suppressionValue, healthRemovedFromStarve, fighterHitCount, plannerDonatedMove, 
        timesBloodlet, overwatchXPoint, overwatchYPoint, overwatchConeRadius, overwatchConeArc, startX, startY, startZ;
    public string revealedByTeam, lastChosenStat, poisonedBy;
    public Statline stats;
    public Inventory inventory;
    public List<string> state, inventoryList, controlledBySoldiersList, controllingSoldiersList, revealedBySoldiersList, revealingSoldiersList, witnessStoredAbilities, witnessActiveAbilities;
    public Item itemPrefab;
    private JArray stateJArray, statsJArray, soldierAbilitiesJArray, itemsJArray, controllingSoldiersJArray, controlledBySoldiersJArray, revealedBySoldiersJArray, revealingSoldiersJArray, witnessStoredAbilitiesJArray, witnessActiveAbilitiesJArray;
    public SphereCollider SRColliderMax, SRColliderHalf, SRColliderMin, itemCollider;
    public BoxCollider bodyCollider;
    public Dictionary<string, string> inventorySlots = new()
    {
        { "Head", "" }, { "Chest", "" }, { "Back", "" }, { "Posterior", "" }, { "Lateral", "" }, { "Left_Leg", "" }, { "Right_Leg", "" }, { "Left_Hand", "" }, { "Right_Hand", "" }, { "Left_Brace", "" },
        { "Right_Brace", "" }, { "Backpack1", "" }, { "Backpack2", "" }, { "Backpack3", "" }, { "Armour1", "" }, { "Armour2", "" }, { "Armour3", "" }, { "Armour4", "" }, { "Misc", "" }
    };
    public Material selectedMaterial, deadMaterial;
    public List<Material> materials;

    public GameObject soldierUI, soldierUIPrefab;
    public MainGame game;
    public MainMenu menu;
    public SoldierManager soldierManager;
    public ItemManager itemManager;

    private void Awake()
    {
        game = FindObjectOfType<MainGame>();
        menu = FindObjectOfType<MainMenu>();
        soldierManager = FindObjectOfType<SoldierManager>();
        itemManager = FindObjectOfType<ItemManager>();
    }

    public Soldier Init(string name, int team, string terrain, Sprite portrait, string portraitText, string speciality, string ability)
    {
        id = GenerateGuid();
        soldierName = name;
        soldierTeam = team;
        soldierSpeciality = speciality;
        soldierDisplayPriority = CheckSpecialityIndex(speciality);
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
        details = new Dictionary<string, object>();

        //save basic information
        details.Add("soldierName", soldierName);
        details.Add("team", soldierTeam);
        details.Add("terrain", soldierTerrain);
        details.Add("portrait", soldierPortraitText);
        details.Add("speciality", soldierSpeciality);
        details.Add("abilities", soldierAbilities);
        details.Add("displayPriority", soldierDisplayPriority);
        details.Add("fielded", fielded);
        details.Add("hp", hp);
        details.Add("ap", ap);
        details.Add("mp", mp);
        details.Add("tp", tp);
        details.Add("xp", xp);
        details.Add("rank", rank);
        details.Add("state", state);

        //save position
        details.Add("x", x);
        details.Add("y", y);
        details.Add("z", z);
        details.Add("terrainOn", terrainOn);

        //save statline
        details.Add("stats", stats.AllStats);
        details.Add("instantSpeed", instantSpeed);

        //save inventory
        details.Add("inventory", Inventory.AllItemIds);
        details.Add("inventorySlots", inventorySlots);

        //save list of revealing soldiers
        details.Add("revealingSoldiers", revealingSoldiersList);

        //save list of revealed by soldiers
        details.Add("revealedBySoldiers", revealedBySoldiersList);

        //save list of controlling soldiers
        details.Add("controllingSoldiers", controllingSoldiersList);

        //save list of controlled by soldiers
        details.Add("controlledBySoldiers", controlledBySoldiersList);

        //save other details
        details.Add("roundsFielded", roundsFielded);
        details.Add("roundsFieldedConscious", roundsFieldedConscious);
        details.Add("roundsWithoutFood", roundsWithoutFood);
        details.Add("revealed", revealed);
        details.Add("usedAP", usedAP);
        details.Add("usedMP", usedMP);
        details.Add("loudActionRoundsVulnerable", loudActionRoundsVulnerable);
        details.Add("stunnedRoundsVulnerable", stunnedRoundsVulnerable);
        details.Add("overwatchShotCounter", overwatchShotCounter);
        details.Add("revealedByTeam", revealedByTeam);

        details.Add("lastChosenStat", lastChosenStat);
        details.Add("suppressionValue", suppressionValue);
        details.Add("healthRemovedFromStarve", healthRemovedFromStarve);
        details.Add("fighterHitCount", fighterHitCount);
        details.Add("plannerDonatedMove", plannerDonatedMove);
        details.Add("poisonedBy", poisonedBy);
        details.Add("timesBloodlet", timesBloodlet);
        details.Add("bloodLettedThisTurn", bloodLettedThisTurn);
        details.Add("illusionedThisMove", illusionedThisMove);
        details.Add("hasKilled", hasKilled);
        details.Add("guardsmanRetryUsed", guardsmanRetryUsed);

        details.Add("overwatchFirstShotUsed", overwatchFirstShotUsed);
        details.Add("witnessActiveAbilities", witnessActiveAbilities);
        details.Add("witnessStoredAbilities", witnessStoredAbilities);
        details.Add("overwatchXPoint", overwatchXPoint);
        details.Add("overwatchYPoint", overwatchYPoint);
        details.Add("overwatchConeRadius", overwatchConeRadius);
        details.Add("overwatchConeArc", overwatchConeArc);
        details.Add("startX", startX);
        details.Add("startY", startY);
        details.Add("startZ", startZ);

        //add the soldier in
        if (data.allSoldiersDetails.ContainsKey(id))
            data.allSoldiersDetails.Remove(id);

        data.allSoldiersDetails.Add(id, details);
    }

    public void LoadData(GameData data)
    {
        data.allSoldiersDetails.TryGetValue(id, out details);
        //print(id);
        soldierName = (string)details["soldierName"];
        soldierTeam = Convert.ToInt32(details["team"]);
        soldierTerrain = (string)details["terrain"];
        //load portrait
        soldierPortrait = LoadPortrait((string)details["portrait"]);
        soldierPortraitText = (string)details["portrait"];
        soldierSpeciality = (string)details["speciality"];
        //load abilities
        soldierAbilities = new();
        soldierAbilitiesJArray = (JArray)details["abilities"];
        foreach (string ability in soldierAbilitiesJArray)
            soldierAbilities.Add(ability);

        soldierDisplayPriority = Convert.ToInt32(details["displayPriority"]);
        fielded = (bool)details["fielded"];
        hp = Convert.ToInt32(details["hp"]);
        ap = Convert.ToInt32(details["ap"]);
        mp = Convert.ToInt32(details["mp"]);
        tp = Convert.ToInt32(details["tp"]);
        xp = Convert.ToInt32(details["xp"]);
        rank = (string)details["rank"];

        //load state
        state = new();
        stateJArray = (JArray)details["state"];
        foreach (string stateString in stateJArray)
            SetState(stateString);

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
        {
            stats.SetStat(stat.GetValue("Name").ToString(), (int)stat.GetValue("BaseVal"));
        }
        instantSpeed = Convert.ToInt32(details["instantSpeed"]);

        //load items
        inventory = new Inventory(this);
        itemsJArray = (JArray)details["inventory"];
        foreach (string itemId in itemsJArray)
            inventoryList.Add(itemId);

        inventorySlots = JsonConvert.DeserializeObject<Dictionary<string, string>>(details["inventorySlots"].ToString());

        //load list of revealing soldiers
        revealingSoldiersList = new();
        revealingSoldiersJArray = (JArray)details["revealingSoldiers"];
        foreach (string soldierId in revealingSoldiersJArray)
            revealingSoldiersList.Add(soldierId);

        //load list of revealed by soldiers
        revealedBySoldiersList = new();
        revealedBySoldiersJArray = (JArray)details["revealedBySoldiers"];
        foreach (string soldierId in revealedBySoldiersJArray)
            revealedBySoldiersList.Add(soldierId);

        //load list of controlling soldiers
        controllingSoldiersList = new();
        controllingSoldiersJArray = (JArray)details["controllingSoldiers"];
        foreach (string soldierId in controllingSoldiersJArray)
            controllingSoldiersList.Add(soldierId);

        //load list of controlled by soldiers
        controlledBySoldiersList = new();
        controlledBySoldiersJArray = (JArray)details["controlledBySoldiers"];
        foreach (string soldierId in controlledBySoldiersJArray)
        {
            controlledBySoldiersList.Add(soldierId);
        }

        //load other details
        roundsFielded = Convert.ToInt32(details["roundsFielded"]);
        roundsFieldedConscious = Convert.ToInt32(details["roundsFieldedConscious"]);
        roundsWithoutFood = Convert.ToInt32(details["roundsWithoutFood"]);
        revealed = (bool)details["revealed"];
        usedAP = (bool)details["usedAP"];
        usedMP = (bool)details["usedMP"];
        loudActionRoundsVulnerable = Convert.ToInt32(details["loudActionRoundsVulnerable"]);
        stunnedRoundsVulnerable = Convert.ToInt32(details["stunnedRoundsVulnerable"]);
        overwatchShotCounter = Convert.ToInt32(details["overwatchShotCounter"]);
        revealedByTeam = (string)details["revealedByTeam"];
        lastChosenStat = (string)details["lastChosenStat"];
        suppressionValue = Convert.ToInt32(details["suppressionValue"]);
        healthRemovedFromStarve = Convert.ToInt32(details["healthRemovedFromStarve"]);
        fighterHitCount = Convert.ToInt32(details["fighterHitCount"]);
        plannerDonatedMove = Convert.ToInt32(details["plannerDonatedMove"]);
        poisonedBy = (string)details["poisonedBy"];
        bloodLettedThisTurn = (bool)details["bloodLettedThisTurn"];
        illusionedThisMove = (bool)details["illusionedThisMove"];
        hasKilled = (bool)details["hasKilled"];
        overwatchFirstShotUsed = (bool)details["overwatchFirstShotUsed"];
        guardsmanRetryUsed = (bool)details["guardsmanRetryUsed"];

        //load witness stored active abilities
        witnessActiveAbilities = new();
        witnessActiveAbilitiesJArray = (JArray)details["witnessActiveAbilities"];
        foreach (string ability in witnessActiveAbilitiesJArray)
            witnessActiveAbilities.Add(ability);
        //load witness stored loading abilities
        witnessStoredAbilities = new();
        witnessStoredAbilitiesJArray = (JArray)details["witnessStoredAbilities"];
        foreach (string ability in witnessStoredAbilitiesJArray)
            witnessStoredAbilities.Add(ability);
        overwatchXPoint = Convert.ToInt32(details["overwatchXPoint"]);
        overwatchYPoint = Convert.ToInt32(details["overwatchYPoint"]);
        overwatchConeRadius = Convert.ToInt32(details["overwatchConeRadius"]);
        overwatchConeArc = Convert.ToInt32(details["overwatchConeArc"]);
        startX = Convert.ToInt32(details["startX"]);
        startY = Convert.ToInt32(details["startY"]);
        startZ = Convert.ToInt32(details["startZ"]);

        //link to maingame object
        game = FindObjectOfType<MainGame>();
    }
    public Soldier LinkWithUI(GameObject displayPanel)
    {
        soldierUI = Instantiate(soldierUIPrefab, displayPanel.transform);
        soldierUI.GetComponent<SoldierUI>().linkedSoldier = this;
        soldierUI.transform.SetAsFirstSibling();
        CheckSpecialityColor(soldierSpeciality);

        return this;
    }

    public bool IsFielded()
    {
        if (fielded)
            return true;
        else
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
        if (IsAlive() && soldierTeam == s.soldierTeam && IsNotSelf(s))
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
        if (IsAlive() && soldierTeam != s.soldierTeam && IsNotSelf(s))
            return true;
        else
            return false;
    }
    public bool IsOnturn()
    {
        if (soldierTeam == game.currentTeam)
            return true;
        else
            return false;
    }
    public bool IsOnturnAndAlive()
    {
        if (IsAlive() && IsOnturn())
            return true;
        else
            return false;
    }
    public bool IsOffturn()
    {
        if (soldierTeam != game.currentTeam)
            return true;
        else
            return false;
    }
    public bool IsOffturnAndAlive()
    {
        if (IsAlive() && IsOffturn())
            return true;
        else
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
    public bool IsRevealing(string id)
    {
        if (IsAbleToSee() && RevealingSoldiers.Contains(id))
            return true;
        else
            return false;
    }
    public bool IsBeingRevealedBy(string id)
    {
        if (IsAlive() && RevealedBySoldiers.Contains(id))
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
    public void PickUpItemToSlot(Item item, string slotName)
    {
        Inventory.AddItemToSlot(item, slotName);
        item.RunPickupEffect();
    }
    public void BrokenDropAllItemsExceptArmour()
    {
        List<Item> itemList = new();
        foreach (Item item in Inventory.AllItems)
            if (!item.itemName.Contains("Armour"))
                itemList.Add(item);

        foreach (Item item in itemList)
            DropItem(item);
    }
    public void FrozenMultiShot()
    {
        if (HasGunEquipped())
        {
            if (EquippedGun.CheckAnyAmmo())
            {
                StartCoroutine(menu.CreateOverwatchShotUI(this, ClosestEnemyVisible()));
                StartCoroutine(menu.OpenOverwatchShotUI());
            }
        }
    }
    public void DestroyAllItems()
    {
        List<Item> itemList = new();
        foreach (Item item in Inventory.AllItems)
            itemList.Add(item);

        foreach (Item item in itemList)
            item.manager.DestroyItem(item);
    }
    public Item DropItem(Item item)
    {
        Inventory.RemoveItem(item);

        return item;
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
        return soldierSpeciality switch
        {
            "Leadership" => "Commander (L)",
            "Health",
            "Resilience",
            "Speed",
            "Evasion",
            "Stealth",
            "Perceptiveness",
            "Camouflage",
            "Sight Radius",
            "Rifle",
            "Assault Rifle",
            "Light Machine Gun",
            "Sniper Rifle",
            "Sub-Machine Gun",
            "Shotgun",
            "Melee",
            "Strength",
            "Diplomacy",
            "Electronics",
            "Healing"
        };
    }
    public void IncrementXP(int xp, bool learnerEnabled)
    {
        if (learnerEnabled && IsLearner()) 
            this.xp += (int)((1.5f * xp) + 1);
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
        print((int)(Mathf.Log(Convert.ToSingle(MinXPForRank()), 2.0f) - Mathf.Log(Convert.ToSingle(s.MinXPForRank()), 2.0f)));
        return (int)(Mathf.Log(Convert.ToSingle(MinXPForRank()), 2.0f) - Mathf.Log(Convert.ToSingle(s.MinXPForRank()), 2.0f));
    }
    public string GetTraumaState()
    {
        return tp switch
        {
            0 => "Committed",
            1 => "<color=yellow>Wavering</color>",
            2 => "<color=yellow>Shaken</color>",
            3 => "<color=orange>Frozen</color>",
            4 => "<color=red>Broken</color>",
            _ => "<color=blue>Desensitised</color>",
        };
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
    public void AssignItemsToSlots()
    {
        foreach (KeyValuePair<string, string> kvp in inventorySlots)
            PickUpItemToSlot(itemManager.FindItemById(kvp.Value), kvp.Key);
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
                ApplyTerrainMods();
                ApplyAbilityMods(); 
                ApplyTraumaMods();
                ApplyHealthStateMods();
                ApplyPlaydeadMods();
                ApplyStunnedMods();
                ApplyItemMods();
                ApplyLoudActionMods();
                CorrectNegatives();

                //get actual speed including enviro effects
                CalculateInstantSpeed();
            }
        }
    }
    public void CheckSightRadius()
    {
        if (stats.SR.Val == 0)
        {
            game.BreakAllControllingMeleeEngagments(this);
        }
    }
    public void ApplyVisMods()
    {
        if (game.weather.CurrentWeather.Contains("Zero visibility"))
            stats.SR.Val -= 100;
        else if (game.weather.CurrentWeather.Contains("Poor visibility"))
            stats.SR.Val -= 90;
        else if (game.weather.CurrentWeather.Contains("Moderate visibility"))
            stats.SR.Val -= 70;
        else if (game.weather.CurrentWeather.Contains("Good visibility"))
            stats.SR.Val -= 40;

        //commander always has full visibility due to thermal goggles
        if (IsCommander())
            stats.SR.Val = stats.SR.BaseVal;

        if (SRColliderMax != null)
        {
            SRColliderMax.radius = stats.SR.Val;
            SRColliderHalf.radius = stats.SR.Val / 2;
            SRColliderMin.radius = 3;
        }
    }
    public void ApplyTerrainMods()
    {
        if (IsOnNativeTerrain())
        {
            stats.C.Val++;
            stats.F.Val++;
        }
        else if (IsOnOppositeTerrain())
        {
            stats.C.Val--;
            stats.F.Val--;
        }
    }
    public void ApplyAbilityMods()
    {
        //mods applied to self
        ApplyWitnessMods();
        ApplyPatriotMods();
        ApplyShadowMods();
        ApplyGuardsmanMods();
        ApplyFighterMods();

        //mods applied by allies
        ApplyInspirerBuffMod();
        ApplyPlannerBuffMod();
        ApplyBloodletterBuffMod();
    }
    public void ApplyWitnessMods()
    {
        if (IsWitness() && witnessStoredAbilities.Count == 0 && witnessActiveAbilities.Count == 0)
            stats.P.Val += 2;
    }
    public void ApplyPatriotMods()
    {
        if (IsOnNativeTerrain() && IsPatriot())
            stats.S.Val += 12;
    }
    public void ApplyShadowMods()
    {
        if (IsShadow())
        {
            System.Tuple<int, string>[] stealthPerceptionCamo = { System.Tuple.Create(stats.F.BaseVal, stats.F.Name), System.Tuple.Create(stats.P.BaseVal, stats.P.Name), System.Tuple.Create(stats.C.BaseVal, stats.C.Name) };
            System.Array.Sort(stealthPerceptionCamo);

            stats.GetStat(stealthPerceptionCamo[0].Item2).Val = stealthPerceptionCamo[2].Item1;
        }
    }
    public void ApplyGuardsmanMods()
    {
        if (IsGuardsman() && IsOnOverwatch())
            stats.P.Val += 1;
    }
    public void ApplyFighterMods()
    {
        if (IsFighter())
        {
            stats.SR.Val += fighterHitCount * 5;
            stats.S.Val += fighterHitCount * 3;
        }
    }
    public void ApplyInspirerBuffMod()
    {
        int inspirerIncrease = soldierSpeciality switch
        {
            "Leadership" or "Health" or "Resilience" or "Evasion" or "Stealth" or "Perceptiveness" or "Camouflage" => 1,
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
    public void ApplyBloodletterBuffMod()
    {
        stats.S.Val += 3 * timesBloodlet;
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
            TakeDamage(null, healthRemovedFromStarve, true, new List<string> { "Sustenance" });
        }

        if (roundsWithoutFood >= 30 && IsConscious())
            MakeUnconscious();

        if (roundsWithoutFood >= 40)
            InstantKill(null, new List<string>() { "Sustenance" });
    }

    public void ApplyHealthStateMods()
    {
        if (CheckState("Unconscious") || CheckState("Playdead"))
        {
            stats.SR.Val = 0;
            stats.E.Val = 0;
            stats.F.Val = 0;
            stats.C.Val = 0;
            stats.M.Val = 0;
        }
    }

    public void ApplyTraumaMods()
    {
        if (tp >= 3 && tp < 5)
            stats.L.Val = 0;
    }

    public void ApplyPlaydeadMods()
    {
        if (CheckState("Playdead"))
            stats.SR.Val = 0;
    }

    public void ApplyStunnedMods()
    {
        if (IsStunned())
        {
            stats.SR.Val = 0;
            stats.E.Val = 0;
            stats.F.Val = 0;
            stats.C.Val = 0;
            stats.M.Val = 0;
        }
    }

    public void ApplyItemMods()
    {
        if (IsWearingBodyArmour())
        {
            stats.C.Val--;
            stats.F.Val--;
        }

        if (IsWearingGhillieArmour())
        {
            stats.C.Val += 4;
            stats.F.Val += 4;
        }

        if (IsWearingExoArmour())
        {
            stats.F.Val = 0;
            stats.Str.Val *= 3;
        }

        if (IsWearingJuggernautArmour())
        {
            stats.C.Val = 0;
            stats.F.Val = 0;
            stats.P.Val -= 2;
        }

        if (IsCarryingRiotShield())
        {
            stats.C.Val = 0;
            stats.F.Val = 0;
        }
    }
    public void ApplyLoudActionMods()
    {
        if (loudActionRoundsVulnerable > 0)
        {
            stats.C.Val = 0;
            stats.F.Val = 0;
        }
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
        stats.Elec.BaseVal = 0;
        stats.Dip.BaseVal = 0;

        //remove all engagements
        if (IsMeleeEngaged())
            StartCoroutine(game.DetermineMeleeControllerMultiple(this));

        StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
    }

    public void UnsetPlaydead()
    {
        UnsetState("Playdead");
        StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
    }

    public IEnumerator TakePoisonDamage()
    {
        //print("take poison damage coroutine");
        yield return new WaitUntil(() => menu.xpResolvedFlag == true);
        //print("take poison damage coroutine melee flag passed");
        TakeDamage(soldierManager.FindSoldierById(poisonedBy), 2, false, new List<string>() { "Poison" });
    }

    public int ApplyDamageMods(Soldier damagedBy, int damage, List<string> damageSource)
    {
        //apply mods that apply to shot damage
        if (damageSource.Contains("Shot"))
        {
            if (IsWearingExoArmour() && game.CoinFlip())
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted {damage} {menu.PrintList(damageSource)} damage with Exo Armour.", true, false);
                damage = 0;
            }
        }

        //apply mods that apply to melee damage
        if (damageSource.Contains("Melee"))
        {
            if (IsWearingJuggernautArmour() && !damagedBy.IsWearingExoArmour())
            {
                menu.AddDamageAlert(this, $"{soldierName} resisted {damage} {menu.PrintList(damageSource)} damage with Juggernaut Armour.", true, false);
                damage = 0;
            }
        }

        //apply mods that apply to explosive damage
        if (damageSource.Contains("Explosive"))
        {
            if (IsWearingJuggernautArmour())
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
            if (IsWearingJuggernautArmour())
            {
                remainingDamage = Inventory.GetItem("Armour_Juggernaut").TakeAblativeDamage(damage);

                if (remainingDamage < damage)
                    menu.AddDamageAlert(this, $"{soldierName} absorbed {damage - remainingDamage} {menu.PrintList(damageSource)} damage with Juggernaut Armour.", true, false);
            }
            else if (IsWearingBodyArmour())
            {
                remainingDamage = Inventory.GetItem("Armour_Body").TakeAblativeDamage(damage);

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
        //apply damage mods
        if (!skipDamageMods)
            damage = ApplyDamageMods(damagedBy, damage, damageSource);

        if (damage > 0)
        {
            //remove overwatch if damage taken
            UnsetOverwatch();

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
                    if (CheckState("Unconscious"))
                        Kill(damagedBy, damageSource);
                    else if (CheckState("Last Stand"))
                    {
                        if (!ResilienceCheck())
                            MakeUnconscious();
                        else
                        {
                            menu.AddXpAlert(this, 2, "Resisted Unconsciousness.", false);
                            menu.AddDamageAlert(this, $"{soldierName} resisted falling <color=blue>Unconscious</color>.", true, true);
                        }
                    }
                    else
                    {
                        if (hp == 1)
                        {
                            if (!ResilienceCheck())
                            {
                                MakeLastStand();
                                menu.AddXpAlert(this, 2, "Resisted Unconsciousness.", false);
                                menu.AddDamageAlert(this, $"{soldierName} resisted falling <color=blue>Unconscious</color>.", true, true);
                            }
                            else
                                MakeUnconscious();
                        }
                        else if (hp == 2)
                        {
                            if (!ResilienceCheck())
                                MakeLastStand();
                            else
                            {
                                menu.AddXpAlert(this, 1, "Resisted Last Stand.", false);
                                menu.AddDamageAlert(this, $"{soldierName} resisted falling into <color=red>Last Stand</color>.", true, true);
                            }
                        }
                        else if (hp == 3)
                        {
                            bool pass = false;

                            for (int i = 0; i < stats.R.Val; i++)
                            {
                                if (ResilienceCheck())
                                    pass = true;
                            }

                            if (!pass)
                                MakeLastStand();
                            else
                            {
                                menu.AddXpAlert(this, 1, "Resisted Last Stand.", false);
                                menu.AddDamageAlert(this, $"{soldierName} resisted falling into <color=red>Last Stand</color>.", true, true);
                            }
                        }
                    }
                }

                //if not broken by health state change break remaining melee engagements
                game.BreakAllControllingMeleeEngagments(this);
            }

            //add damage alert
            menu.AddDamageAlert(this, $"{soldierName} took {damage} ({menu.PrintList(damageSource)}) damage. He is now {CheckHealthState()}.", false, false);
            //make sure damage came from another soldier
            if (damagedBy != null)
            {
                //apply stun affect from tranquiliser
                if (damagedBy.IsTranquiliser() && (damageSource.Contains("Shot") || damageSource.Contains("Melee")))
                    TranquiliserMakeStunned(1);
            }
        }
        else
            print("Damage was reduced to 0.");

        //make sure damage came from another soldier
        if (damagedBy != null)
        {
            //apply witness ability slurp
            if (IsWitness())
            {
                witnessStoredAbilities.Clear();
                foreach (string ability in damagedBy.soldierAbilities)
                    witnessStoredAbilities.Add(ability);
            }
        }
    }



    public void TakeTrauma(int trauma)
    {
        if (tp < 5)
        {
            tp += trauma;

            //perform frozen shenanigans
            if (tp == 3)
            {
                FrozenMultiShot();
            }

            //drop all items for broken
            if (tp == 4)
            {
                BrokenDropAllItemsExceptArmour();
                foreach (Soldier s in game.AllSoldiers())
                    game.BreakMeleeEngagement(this, s);
            }
        }
    }

    public void TakeHeal(Soldier healedBy, int heal, int traumaHeal, bool overhealthEnabled, bool resurrectEnabled)
    {
        if (CheckState("Dead"))
        {
            if (resurrectEnabled)
                Resurrect(heal);
            else
                print("Can't heal a dead soldier");
        }
        else
        {
            if (CheckState("Unconscious"))
                MakeLastStand();

            else
            {
                if (CheckState("Last Stand"))
                    MakeActive();

                hp += heal;

                //no overfilling health via heal unless overhealth is enabled
                if (!overhealthEnabled && hp > stats.H.Val)
                    hp = stats.H.Val;

                HealTrauma(traumaHeal);

                if (healedBy != null)
                {
                    //add xp for successful heal
                    if (healedBy == this)
                        menu.AddXpAlert(healedBy, Mathf.CeilToInt((heal + traumaHeal) / 2.0f), $"Healed self by {heal} hp and removed {traumaHeal} trauma points.", true);
                    else
                        menu.AddXpAlert(healedBy, heal + traumaHeal, $"Healed {soldierName} by {heal} hp and removed {traumaHeal} trauma points.", true);
                }
            }
        } 
    }

    public void HealTrauma(int traumaHeal)
    {
        //don't heal if already desensitised or commited
        if (tp < 5 && tp > 0)
            tp -= traumaHeal;

        //correct negatives
        if (tp < 0)
            tp = 0;
    }

    public void CalculateInstantSpeed()
    {
        if (IsAbleToWalk())
        {
            instantSpeed = (int)((stats.S.Val - CalculateCarryWeight() + ApplyTerrainModsMove()) * ApplyVisModsMove() * ApplyRainModsMove() * ApplySustenanceModsMove() * ApplyTraumaModsMove() * ApplyKdModsMove()) + stats.Str.Val;

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
        if (IsAbleToSee())
            return (instantSpeed - stats.Str.Val)*ApplySuppressionModsMove() + stats.Str.Val;

        return 0;
    }
    public int CalculateCarryWeight()
    {
        int carryWeight = 0;

        foreach (Item i in Inventory.AllItems)
        {
            if (IsBull() && (i.gunType != null || i.itemName.Contains("Ammo")))
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
        if (IsCommander())
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

    public int GetKd() 
    {
        int kd = 0;
        foreach (Soldier s in game.AllSoldiers())
        {
            if (IsOppositeTeamAs(s) && s.IsDead())
                kd++;
            else if (IsSameTeamAs(s) && s.IsDead())
                kd--;
        }

        return kd;
    }

    public float ApplyKdModsMove()
    {
        float kdModMove = (2 * GetKd() / 100f);

        //print("Kd Mod Move: " + kdModMove);
        return 1 - kdModMove;
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
        soldierUI.transform.Find("HP").gameObject.GetComponent<TextMeshProUGUI>().text = "HP:" + GetFullHP();
        soldierUI.transform.Find("AP").gameObject.GetComponent<TextMeshProUGUI>().text = "AP:" + ap;
        soldierUI.transform.Find("MP").gameObject.GetComponent<TextMeshProUGUI>().text = "MA:" + mp;
        soldierUI.transform.Find("Location").gameObject.GetComponent<TextMeshProUGUI>().text = "X:" + x + "   Y:" + y + "   Z:" + z;
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

        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsAlive() && IsOppositeTeamAs(s) && PhysicalObjectWithinRadius(s,loudActionRadius))
            {
                if (PhysicalObjectWithinMinRadius(s) && vulnerableTurns < 3)
                    vulnerableTurns = 3;
                else if (PhysicalObjectWithinHalfRadius(s) && vulnerableTurns < 2)
                    vulnerableTurns = 2;
                else if (PhysicalObjectWithinMaxRadius(s) && vulnerableTurns < 1)
                    vulnerableTurns = 1;
            }
        }

        //rerun detection alerts if loud action performed for first time
        if (vulnerableTurns > 0)
        {
            if (loudActionRoundsVulnerable == 0)
                StartCoroutine(game.DetectionAlertSingle(this, "statChange", Vector3.zero, string.Empty, true));

            if (vulnerableTurns > loudActionRoundsVulnerable)
                SetLoudRevealed(vulnerableTurns);
        }
    }
    public void PerformLoudAction()
    {
        int vulnerableTurns = 0;

        foreach (Soldier s in game.AllSoldiers())
        {
            if (s.IsAlive() && IsOppositeTeamAs(s))
            {
                if (PhysicalObjectWithinMinRadius(s) && vulnerableTurns < 3)
                    vulnerableTurns = 3;
                else if (PhysicalObjectWithinHalfRadius(s) && vulnerableTurns < 2)
                    vulnerableTurns = 2;
                else if (PhysicalObjectWithinMaxRadius(s) && vulnerableTurns < 1)
                    vulnerableTurns = 1;
            }
        }

        //rerun detection alerts if loud action performed for first time
        if (vulnerableTurns > 0)
        {
            if (loudActionRoundsVulnerable == 0)
                StartCoroutine(game.DetectionAlertSingle(this, "statChange", Vector3.zero, string.Empty, true));

            if (vulnerableTurns > loudActionRoundsVulnerable)
                SetLoudRevealed(vulnerableTurns);
        }
    }

    public void GenerateAP()
    {
        var leadership = stats.L.Val;
        var runLoop = true;
        var localAp = 3;
        var localMp = 1;

        while (runLoop)
        {
            if (UnityEngine.Random.Range(1, 7) <= leadership)
            {
                localMp++;
                if (UnityEngine.Random.Range(1, 7) <= 3)
                    localAp++;
                else
                    localAp += 2;

                if (leadership > 6)
                    localAp += 3;
                else
                    localAp += (leadership / 2);

                leadership -= 6;
            }
            else 
                runLoop = false;
        }

        //check for wearing logistics belt
        if (IsWearingLogisticsBelt())
            localAp++;

        //add inspirer ap bonus to support specialities
        localAp += InspirerBonusSupport();

        //minus dissauder ap
        localAp += DissuaderPenalty();

        ap = localAp;
        mp = localMp;
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
        TMP_Dropdown allPortraits = FindObjectOfType<AllPortraits>().allPortraitsDropdown;
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
        TMP_Dropdown allPortraits = FindObjectOfType<AllPortraits>().allPortraitsTeamsightDropdown;
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
        TMP_Dropdown allInsignia = FindObjectOfType<AllInsignia>().allInsigniaDropdown;
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
    public string IncrementRandom(string choiceStat)
    {
        string[] stats =
        {
            "Leadership", "Health", "Resilience", "Speed", "Evasion", "Stealth", "Perceptiveness", "Camouflage", "Sight Radius", 
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
            "Leadership", "Health", "Resilience", "Speed", "Evasion", "Stealth", "Perceptiveness", "Camouflage", "Sight Radius",
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
            "Stealth" => "F: +" + stats.F.Increment(),
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
        if (fielded)
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
                    case "Stealth":
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
            "Stealth" => 6,
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
    public void SetLoudRevealed(int rounds)
    {
        loudActionRoundsVulnerable = rounds;
        print(soldierName + " is vulnerable to detection for " + rounds + " rounds.");
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
        suppressionValue = Mathf.RoundToInt((1 - ((100 - suppressionValue)/100f)*((100 - suppression)/100f))*100);
    }
    public void UnsetSuppression()
    {
        suppressionValue = 0;
    }

    public bool IsStunned()
    {
        if (stunnedRoundsVulnerable > 0)
            return true;
        else
            return false;
    }
    public void SetStunned(int rounds)
    {
        stunnedRoundsVulnerable = rounds;
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
    public bool IsOnDrug(string drugName)
    {
        return CheckState(drugName);
    }
    public bool IsOnOverwatch()
    {
        if (CheckState("Overwatch"))
            return true;
        else
            return false;
    }
    public void SetOverwatch(int x, int y, int r, int a)
    {
        overwatchXPoint = x;
        overwatchYPoint = y;
        overwatchConeRadius = r;
        overwatchConeArc = a;
        guardsmanRetryUsed = false;
        if (IsGuardsman())
            overwatchShotCounter = 2;
        else
            overwatchShotCounter = 1;
        SetState("Overwatch");
        StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
    }
    public void DecrementOverwatch()
    {
        if (IsOnOverwatch())
        {
            if (!IsGuardsman()) 
            {
                overwatchShotCounter--;
                guardsmanRetryUsed = false;
                menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);
            }
            else
            {
                if (guardsmanRetryUsed)
                {
                    overwatchShotCounter--;
                    guardsmanRetryUsed = false;
                    menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);
                }
                else
                {
                    guardsmanRetryUsed = true;
                    menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(true);
                }
            }

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
            StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
        }
    }
    public bool IsBloodRaged()
    {
        if (CheckState("Blood Rage"))
            return true;

        return false;
    }
    public void SetBloodRage()
    {
        SetState("Blood Rage");
    }
    public void UnsetBloodRage()
    {
        UnsetState("Blood Rage");
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
        if (CheckState("Crushed"))
            return false;

        return true;
    }
    public void SetCrushed()
    {
        if (IsInteractable())
        {
            SetState("Crushed");
            DestroyAllItems();
        }
    }
    public void UnsetCrushed()
    {
        UnsetState("Crushed");
    }
    public void MakeStunned(int stunRounds)
    {
        if (stunRounds > stunnedRoundsVulnerable)
        {
            SetStunned(stunRounds);

            //remove all engagements
            if (IsMeleeEngaged())
                StartCoroutine(game.DetermineMeleeControllerMultiple(this));

            if (ResilienceCheck())
                menu.AddDamageAlert(this, $"{this.soldierName} resisted being stanned for {stunnedRoundsVulnerable} rounds.", true, true);
            else
                menu.AddDamageAlert(this, $"{this.soldierName} suffered stun for {stunnedRoundsVulnerable}.", true, true);

            StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
        }
    }
    public void TranquiliserMakeStunned(int stunRounds)
    {
        if (ResilienceCheck())
            menu.AddDamageAlert(this, $"Resisted a {stunRounds} round stun.", true, true);
        else
        {
            menu.AddDamageAlert(this, $"Suffered a {stunRounds} round stun.", false, true);
            MakeStunned(stunRounds);
        }
    }
    public string CheckHealthState()
    {
        if (IsDead())
            return "<color=red>Dead</color>";
        else if (IsUnconscious())
            return "<color=blue>Unconscious</color>";
        else if (IsLastStand())
            return "<color=red>Last Stand</color>";
        else
            return "Active";
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
        print(soldierName + " returns to service.");
        StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
    }
    public void MakeLastStand()
    {
        if (IsWearingJuggernautArmour())
            menu.AddDamageAlert(this, $"{soldierName} resisted <color=red>Last Stand</color> with JA.", true, true);
        else
        {
            ClearHealthState();
            SetState("Last Stand");
            menu.AddDamageAlert(this, $"{soldierName} fell into <color=red>Last Stand</color>.", false, true);
            StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
        }
    }
    public void MakeUnconscious()
    {
        ClearHealthState();
        SetState("Unconscious");
        menu.AddDamageAlert(this, $"{soldierName} fell into <color=blue>Unconscious</color>.", false, true);

        //remove all engagements
        if (IsMeleeEngaged())
            StartCoroutine(game.DetermineMeleeControllerMultiple(this));

        StartCoroutine(game.DetectionAlertSingle(this, "losChange", Vector3.zero, string.Empty, false));
        print(soldierName + " is Unconscious.");
    }
    public void Resurrect(int hp)
    {
        ClearHealthState();
        SetState("Active");
        CheckSpecialityColor(soldierSpeciality);
        TakeHeal(null, hp, 0, true, false);
    }

    public void InstantKill(Soldier killedBy, List<string> damageSource)
    {
        if (this.IsAlive())
        {
            menu.AddDamageAlert(this, $"{soldierName} was killed instantly by {menu.PrintList(damageSource)}.", false, false);
            Kill(killedBy, damageSource);
        }
    }
    public void Kill(Soldier killedBy, List<string> damageSource)
    {
        if (IsAlive())
        {
            int tp = 1;
            bool lastandicide = false;

            //make him dead
            ClearHealthState();
            SetState("Dead");
            menu.AddDamageAlert(this, $"{soldierName} died!", false, true);
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

                    killedBy.FighterMeleeKillReward();
                }
                else if (damageSource.Contains("Poison"))
                    menu.AddXpAlert(killedBy, 10 + stats.R.Val, $"Killed {soldierName} by poisoning.", false);

                //set haskilled flag for avenger
                killedBy.hasKilled = true;
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
    public bool PhysicalObjectWithinRadius(PhysicalObject obj, float radius)
    {
        if (game.CalculateRange(this, obj) <= radius)
            return true;

        return false;
    }
    public bool PhysicalObjectWithinRadius(Vector3 point, float radius)
    {
        if (game.CalculateRange(this, point) <= radius)
            return true;

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
        foreach (Terminal t in FindObjectsOfType<Terminal>())
            if (PhysicalObjectWithinMeleeRadius(t))
                return true;
        return false;
    }
    public Terminal ClosestTerminal()
    {
        if (TerminalInRange())
        {
            List<Tuple<float, Terminal>> soldierDistanceToTerminals = new();

            foreach (Terminal t in FindObjectsOfType<Terminal>())
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
            if (s.IsSameTeamAs(this))
                soldierDistances.Add(Tuple.Create(game.CalculateRange(this, s), s));

        soldierDistances = soldierDistances.OrderBy(t => t.Item1).ToList();

        if (soldierDistances.Count > 0)
            return soldierDistances[0].Item2;

        return null;
    }
    public Soldier ClosestAllyMobile()
    {
        List<Tuple<float, Soldier>> soldierDistances = new();

        foreach (Soldier s in game.AllSoldiers())
            if (s.IsSameTeamAs(this) && s.IsAbleToWalk())
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
            if (s.IsOppositeTeamAs(this))
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
            if (s.IsOppositeTeamAs(this) && s.IsRevealed())
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
            if (s.IsAbleToSee() && IsSameTeamAsIncludingSelf(s))
                if (game.CalculateRange(s, obj) <= s.SRColliderMax.radius)
                    objectIsRevealed = true;
        return objectIsRevealed;
    }












    //status checks
    public bool ResilienceCheck()
    {
        if (game.DiceRoll() <= stats.R.Val)
            return true;
        else
            return false;
    }
    public bool SuppressionCheck()
    {
        if (suppressionValue > 0)
        {
            if (ResilienceCheck())
            {
                menu.AddXpAlert(this, 2, "Resisted Suppression.", true);
                return true;
            }
            else
                return false;
        }
            
        return true;
    }
    public bool StructuralCollapseCheck(int structureHeight)
    {
        int survivalPassesNeeded = Mathf.FloorToInt(structureHeight / 10f);
        int survivalPassesAchieved = 0;
        int survivalAttempts = 1;

        if (IsWearingJuggernautArmour() && HasArmourIntegrity())
            survivalAttempts += 2;
        else if (IsWearingBodyArmour() && HasArmourIntegrity())
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












    //item checks
    public bool HasAnyItemInHand()
    {
        if (LeftHandItem != null || RightHandItem != null)
            return true;
        return false;
    }
    public bool HasGunEquipped()
    {
        if (EquippedGun != null)
            return true;
        return false;
    }
    public bool HasSMGOrPistolEquipped()
    {
        if (HasGunEquipped())
            if (EquippedGun.gunType == "Pistol" || EquippedGun.gunType == "SMG")
                return true;
        return false;
    }
    public bool IsDualWielding()
    {
        if (LeftHandItem != null && RightHandItem != null)
            return true;
        return false;
    }
    public bool HasArmourIntegrity()
    {
        if ((Inventory.FindItem("Armour_Juggernaut") && Inventory.GetItem("Armour_Juggernaut").ablativeHealth > 0) || (Inventory.FindItem("Armour_Body") && Inventory.GetItem("Armour_Body").ablativeHealth > 0))
            return true;
        return false;
    }
    public bool IsWearingBodyArmour()
    {
        if (Inventory.FindItem("Armour_Body"))
            return true;
        return false;
    }
    public bool IsWearingJuggernautArmour()
    {
        if (Inventory.FindItem("Armour_Juggernaut"))
            return true;
        return false;
    }
    public bool IsWearingExoArmour()
    {
        if (Inventory.FindItem("Armour_Exo"))
            return true;
        return false;
    }
    public bool IsWearingGhillieArmour()
    {
        if (Inventory.FindItem("Armour_Ghillie"))
            return true;
        return false;
    }
    public bool IsWearingStimulantArmour()
    {
        if (Inventory.FindItem("Armour_Stimulant"))
            return true;
        return false;
    }
    public bool IsCarryingRiotShield()
    {
        if (Inventory.FindItem("Riot_Shield"))
            return true;
        return false;
    }
    public bool IsWearingLogisticsBelt()
    {
        if (Inventory.FindItem("Logistics_Belt"))
            return true;
        return false;
    }
















    //ability checks
    public bool IsAdept()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Adept"))
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
    public bool IsLearner()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Learner"))
                return true;

        return false;
    }
    public bool IsGuardsman()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Guardsman"))
                return true;

        return false;
    }
    public bool IsFighter()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Fighter"))
                return true;

        return false;
    }
    public void FighterMeleeHitReward()
    {
        if (IsFighter())
            fighterHitCount++;
    }
    public void FighterMeleeKillReward()
    {
        if (IsFighter() && usedMP)
        {
            ap += 3;
            mp += 1;
        }
    }
    public bool IsPatriot()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Patriot"))
                return true;

        return false;
    }
    public bool IsTranquiliser()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Tranquiliser"))
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
        UnsetState("Inspired");
        if (soldierSpeciality == "Health")
            TakeDamage(null, 1, true, new List<string>() { "Inspirer Debuff" });
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
        if (IsInspired() && gun.gunType == soldierSpeciality)
            return 5;

        return 0;
    }
    public float InspirerBonusWeaponMelee()
    {
        if (IsInspired() && soldierSpeciality == "Melee")
            return 0.5f;

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
    public bool IsBull()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Bull"))
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
    public bool IsShadow()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Shadow"))
                return true;

        return false;
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
    public bool IsWitness()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Witness"))
                return true;

        return false;
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
    public bool IsPlanner()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Planner"))
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
        timesBloodlet++;
        bloodLettedThisTurn = true;
        SetBloodRage();
        stats.H.BaseVal--;
        hp--;
    }
    public bool IsInsulator()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Insulator"))
                return true;

        return false;
    }
    public bool IsExperimentalist()
    {
        if (IsConscious())
            if (soldierAbilities.Contains("Experimentalist"))
                return true;

        return false;
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
                foreach (string id in revealingSoldiersList)
                    soldierManager.FindSoldierById(id).SetDissuaded();
        }
    }
    public Item LeftHandItem
    {
        get 
        {
            inventorySlots.TryGetValue("Left_Hand", out string leftHand);
            return itemManager.FindItemById(leftHand);
        }
    }
    public Item RightHandItem
    {
        get
        {
            inventorySlots.TryGetValue("Right_Hand", out string rightHand);
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
    public Item EquippedGun
    {
        get
        {
            Item gunEquipped = null;
            if (LeftHandItem != null)
            {
                if (LeftHandItem.gunType != null)
                    gunEquipped = LeftHandItem;
            }
            else
            {
                if (RightHandItem != null)
                    if (RightHandItem.gunType != null)
                        gunEquipped = RightHandItem;
            }

            return gunEquipped;
        }
    }
    public Inventory Inventory
    {
        get { return inventory; }
    }
    public string Id
    {
        get { return id; }
    }
}
