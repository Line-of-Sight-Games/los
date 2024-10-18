using System.Collections.Generic;
using UnityEngine;

public class DipelecGen : MonoBehaviour, IDataPersistence
{
    private readonly string[] l1Dip =
    {
        "L1\nReceive a single piece of medium-level advice if it is asked in the form of a yes/no Question",
        "L1\nRetry your next missed shot once (excluding overwatch) (Must have required ammo)",
        "L1\nFor one turn only, all soldiers may enter cover for 0AP (Player may use this on the current turn or use anytime provided notice is given the previous turn)",
        "L1\nProvide 2 false detection movements to the Opponent. Detection movements must start and end out of the Opponent's view",
        "L1\nLearn approximate location of all enemy units. Receive one guaranteed Opponent overwatch miss for every two enemy soldiers who were already revealed",
        "L1\nThe next enemy soldier who successfully shoots a friendly soldier will be stunned for 1 turn",
        "L1\nCan swap one grenade currently equipped by any soldier for a different grenade. If no grenades are held, award any one grenade to the negotiator",
        "L1\nPlayer receives one free successful ULF effect to use anytime (no XP)",
        "L1\nNext successful shot will stun enemy soldier for one turn",
        "L1\nNegotiator receives 1Elec. If he has previously failed a hack, rerun it (most recent attempt at same level for 0AP) with the new Elec skill",
    };
    private readonly string[] l2Dip =
    {
        "L2\nNext shot will hit (XP given)",
        "L2\nFor one turn, the Player can ask if any revealed enemies are in cover. If any are, all enemy covers will be cancelled. If they are not, the Player retains the ability for another turn (Player may use this on the current turn or use anytime provided notice is given the previous turn) (Opponent informed once covers are cancelled)",
        "L2\nLook at enemy suadcard for 60s (Opponent informed)",
        "L2\nThe Player may remove one of their soldiers from the field. The soldier will return from where they were taken at start of their next turn. This ability may be used once anytime",
        "L2\nReorganise all equipment between fielded soldiers to the Player's liking",
        "L2\nCancel the ability of the furthest revealed enemy from the negotiator. If all enemies are currently hidden, this effect will apply to the next revealed enemy (Opponent informed)",
        "L2\nRelocate any friendly or revealed enemy soldier along a straight line between their current position and where they fielded from (Opponent informed if enemy soldier)",
        "L2\nNext two enemy shots will miss (XP given) (Opponent informed of reason after miss)",
        "L2\nAt the beginning of the Opponent's next turn, their overwatch will be disabled for 6 turns (Opponent informed)",
        "L2\nPoison the first soldier (friendly or enemy, no XP) encountered in a wind direction (8 options) chosen by the Player. Player may also poison closest soldier to centre if desired (Opposed informed if enemy poisoined)",
    };
    private readonly string[] l3Dip =
    {
        "L3\nField a randomly generated Recruit in your chosen specialty at a valid spawn point (Opponent informed of extra man). He may field with one gun, one knife or one grenade",
        "L3\nPlace 3 thermal cameras anywhere on the map (negotiator to receive XP for detections)",
        "L3\nMove and rotate a reasonable terrain feature to any location where there is room (will carry everything)",
        "L3\nAll equipped enemy grenades, radios and claymores will be confiscated by the GM (Opponent informed)",
        "L3\nPlace six claymores anywhere on the field (XP given) that is not within 6cm of a known enemy location (will use the Negotiator's F and C values)",
        "L3\nIn situations where the Player reasonably believes there is a mutual LOS between friendly and enemy soldiers, they may render their soldier unshootable until the end of their Opponent's turn. Can be used twice at anytime. (Opponent informed of reason after miss)",
        "L3\nAll enemy soldiers accrue an additional trauma level (Opponent informed)",
        "L3\nOne fielded soldier must call a free UHF strike (XP given)",
        "L3\nChoose a soldier. Completely rebuild (choose) their specialty, ability and skill point distribution (duplicates only permitted for deceased soldiers). Their XP, body and equipment will be conserved. They may equip a gun or swap if one is already equipped",
        "L3\nEnemy Corporals (and above) will be unable to take cover, equip new Specialty Armours or control engagements (Opponent informed)",
    };
    private readonly string[] l4Dip =
    {
        "L4\nAfter any attempted shot (either to or from the enemy), teleport the relevant enemy anywhere along the bullet's trajectory (Can be mid-air...enemy will fall). Can be used twice (Opponent informed after each use)",
        "L4\nAdminister any one drug to any one soldier without a side effect",
        "L4\nAll enemy Support skills set to 0. -1 to enemy Weapon skills (Opponent informed)",
        "L4\nTeleport entire fielded team to a single location (3cm distancing kept). Can be used anytime (only once) (Opponent informed if used but not the location)",
        "L4\nReset all starvation and trauma for fielded soldiers to 0. Remove any active poison",
        "L4\nRemove 10 items of indestructible cover. Any enemies currently in cover will be poisoned and receive 1 trauma point. (Opponent informed)",
        "L4\nChoose a terrain. All soldiers of that terrain will receive a UHF radio",
        "L4\nAll fielded enemies except the most experienced enemy (unless he is the sole survivor) will be poisoned (no XP) (Opponent informed)",
        "L4\n+2 increments to all soldiers' Support and Weapon skills",
        "L4\nAny enemies including and above the rank of Captain are executed. If none exist, one random unfielded soldier will be executed, if none exist, the most experienced enemy will have his HP halved (round down) (Opponent informed)",
    };
    private readonly string[] l5Dip =
    {
        "L5\nAll fielded enemies become Frozen. Unfielded enemies are executed (Opponent informed)",
        "L5\nEntire team's Dip and Elec skills are added together to deliver a UHF strike. If Negotiator is sole survivor, his skill points will be doubled. If no enemies are killed by this strike, it may be redone (once, XP given)",
        "L5\nPlayer never leaves the Command Zone and all enemy radio communications are permanently disabled. (Opponent informed)",
        "L5\nAll enemies must pass a deathroll to survive (no XP). If they all survive, all except the lowest ranked (Opponent chooses sole survivor if there are ties) will be executed (Opponent informed)",
        "L5\nDisable two of either shooting, melee attack or explosive use for the enemy (Oppponent informed)",
    };
    private readonly string[] l6Dip =
    {
        "L6\nVictory",
    };

    private readonly string[] l1Elec =
    {
        "L1\nHacker receives 1Dip. If he has previously failed a negotiation, rerun it (most recent attempt at same level for 0AP) with the new Dip skill",
        "L1\nReceive 9S and 15SR to distribute amongst fielded soldiers as desired",
        "L1\nMove any POI up to 20cm away from its origin in one of the 6 cardinal directions (x,-x,y,-y,z,-z) provided it ends up in an accessible spot (Opponent informed)",
        "L1\nNext turn, award 1AP to all soldiers who fail to receive Leadership AP",
        "L1\nLearn exact average enemy position- if all enemies are already revealed, award 3AP to any one soldier",
        "L1\nDeploy either a smoke or tabun grenade anywhere that can be see from directly above (no XP)",
        "L1\nLearn exact location and receive a glimpse of the farthest hidden enemy (no XP). If all enemies are already revealed, all fielded soldiers will gain 5SR",
        "L1\nLearn Commander's (or next most powerful unit's) exact location. If all enemies are already revealed, all soldiers will gain 5SR",
        "L1\nAll fielded soldiers (except the hacker) receive 2XP or the hacker receives 6XP",
        "L1\nAny soldiers between half and full health will be fully healed. If this does not apply to any soldiers. Receive 2H to distribute amongst team (cannot be applied to soldiers below half health)",
    };
    private readonly string[] l2Elec =
    {
        "L2\nChoose a soldier to link with. Any HP lost by either soldier will now be split 50/50 between the nominated soldier and the hacker. If damage is an odd number, reduce it by 1 first. Both soldiers receive 1H",
        "L2\nAll fielded soldiers receive +1 increment to their specialty",
        "L2\nEnemy shotcalcs will no longer provide hit percentages. They will also lose the ability to withhold shots under 25% (Opponent informed)",
        "L2\nAward BA to one soldier not wearing any armour. If the player prefers, or if all soldiers are already wearing BA, award 1H to all fielded soldiers (If hacker is sole survivor, he earns 4H)",
        "L2\nThe hacker and any soldiers with a direct LOS to him shall receive +1 increment in all Weapon and Support skills excluding their specialties. Soldiers who cannot see the hacker shall earn +1 increment to their specialty",
        "L2\nThe next explosive which deals any damage to a soldier will inflict only 50% (round down) to all soldiers",
        "L2\nSelect any fielded soldier. For as long as they live, any damage received by this soldier will be split 50/50 between them and the soldier which caused their damage. If the damage is an odd number, increase it by 1 first.  The attacking soldier may not drop below 3HP from this effect",
        "L2\nAll soldiers already on their native terrain shall earn 1H, 1E and 1M. Any soldiers not on their native terrain shall have the map feature (up to a rough 14cm radius) they are currently on converted to their native terrain with no bonuses. Soldiers on the baseboard will have a 14cm radius circle projected around them)",
        "L2\n10SR awarded to fielded soldiers and entire weather forecast is known.",
        "L2\nAt the beginning of the Player's next turn, reveal all enemies visible from directly above (no XP). If this reveals no new enemies, award 9S to all fielded soldiers",
    };
    private readonly string[] l3Elec =
    {
        "L3\nDistribute 8AP amongst fielded soldiers",
        "L3\nAll soldiers will be fully healed (no XP). Soldiers already on full health receive 1H",
        "L3\nNeither shots nor suppression consume ammunition. Remove any spare ammo equipped on enemy soldiers.",
        "L3\nAll fielded enemy soldiers accrue an additional 10 turns of starvation (Opponent informed)",
        "L3\nSelect a revealed enemy soldier who will spend 3AP each turn traversing the quickest route (with no regard for their safety) to a spawn of your choice as the first action of the Opponent's turn. (choose once). The enemy soldier may use additional AP as they wish and will lose 2HP for every turn they cannot move. When (if) they arrive, the Opponent will resume regular control (Opponent informed) (If no enemy soldiers are visible, this will apply to the next revealed soldier)",
        "L3\nEnemy soldiers will receive an unresistable suppression penalty of 80 on their next turn. Can be used anytime (Opponent informed)",
        "L3\nEngine will randomly generate 5 abilities, choose one of these to award to all fielded allies (same ability for all)",
        "L3\nSquadsight disabled for the enemy (Opponent informed)",
        "L3\n-12S to all fielded enemies (Opponent informed)",
        "L3\nSoldiers can permanently retry failed R rolls once per roll",
    };
    private readonly string[] l4Elec =
    {
        "L4\nAll fielded soldiers (except the hacker, unless he is the sole survivor) shall have their first ability upgraded",
        "L4\nPermanently remove all current and future appearances of a list of items specified by the Player (Opponent informed)",
        "L4\nThe player's last surviving soldier will have his H tripled and his HP restored to maximum. He may also choose an additional unupgraded ability",
        "L4\nStructurally collapse up to 6 structures (Opponent informed)",
        "L4\nChoose one primary, weapon and support skill (Not H, S, P or SR). All soldiers will have these skills doubled while all fielded enemies will have them zeroed (Opponent informed)",
        "L4\nEnemy soldiers will always be melee-controlled in engagements. Any damage-dealing melee attack will cause immediate death. (Opponent informed)",
        "L4\nAll soldiers will receive thermal goggles and a smoke grenade (if desired)",
        "L4\nImmediately resurrect a fallen soldier at his grave on full HP. He may commence his turn (Opponent informed)",
        "L4\nAll fielded soldiers shall earn a 'life'. Completely ignore the event that would have killed them (once each)",
        "L4\nAll actions now cost 1 less AP (min 0)",
    };
    private readonly string[] l5Elec =
    {
        "L5\nThe Player receives 2 minutes to alter anything they want on the map. This can't damage enemy units, should debris be placed on them, they will be placed on top. All enemy soldiers are revealed before the 2 minutes commences (Opponent informed)",
        "L5\nAny damage dealt to any enemy soldier shall also be applied to his allies (Opponent informed)",
        "L5\nAll soldiers awarded 6L",
        "L5\nAll soldiers become invisible to everything until any soldier issues or takes damage",
        "L5\nDeploy 2 Captains anywhere on the map. Player shall choose skill points, skin, upgraded ability and 1 weapon, 1 item and 1 armour each",
    };
    private readonly string[] l6Elec =
    {
        "L6\nVictory",
    };

    public List<string> savedDip = new();
    public List<string> savedElec = new();

    public void LoadData(GameData data)
    {
        savedDip = data.savedDip;
        savedElec = data.savedElec;
    }

    public void SaveData(ref GameData data)
    {
        data.savedDip = savedDip;
        data.savedElec = savedElec;
    }

    public void GenerateDipelec()
    {
        savedDip.Add(l1Dip[Random.Range(0, l1Dip.Length)]);
        savedDip.Add(l2Dip[Random.Range(0, l2Dip.Length)]);
        savedDip.Add(l3Dip[Random.Range(0, l3Dip.Length)]);
        savedDip.Add(l4Dip[Random.Range(0, l4Dip.Length)]);
        savedDip.Add(l5Dip[Random.Range(0, l5Dip.Length)]);
        savedDip.Add(l6Dip[0]);

        savedElec.Add(l1Elec[Random.Range(0, l1Elec.Length)]);
        savedElec.Add(l2Elec[Random.Range(0, l2Elec.Length)]);
        savedElec.Add(l3Elec[Random.Range(0, l3Elec.Length)]);
        savedElec.Add(l4Elec[Random.Range(0, l4Elec.Length)]);
        savedElec.Add(l5Elec[Random.Range(0, l5Elec.Length)]);
        savedElec.Add(l6Elec[0]);
    }

    public string GetLevelDip(int level)
    {
        return _ = level switch
        {
            1 => L1Dip,
            2 => L2Dip,
            3 => L3Dip,
            4 => L4Dip,
            5 => L5Dip,
            6 => L6Dip,
            _ => "",
        };
    }
    public string GetLevelElec(int level)
    {
        return _ = level switch
        {
            1 => L1Elec,
            2 => L2Elec,
            3 => L3Elec,
            4 => L4Elec,
            5 => L5Elec,
            6 => L6Elec,
            _ => "",
        };
    }
    public string L1Dip
    {
        get { return savedDip[0]; }
    }
    public string L2Dip
    {
        get { return savedDip[1]; }
    }
    public string L3Dip
    {
        get { return savedDip[2]; }
    }
    public string L4Dip
    {
        get { return savedDip[3]; }
    }
    public string L5Dip
    {
        get { return savedDip[4]; }
    }
    public string L6Dip
    {
        get { return savedDip[5]; }
    }

    public string L1Elec
    {
        get { return savedElec[0]; }
    }
    public string L2Elec
    {
        get { return savedElec[1]; }
    }
    public string L3Elec
    {
        get { return savedElec[2]; }
    }
    public string L4Elec
    {
        get { return savedElec[3]; }
    }
    public string L5Elec
    {
        get { return savedElec[4]; }
    }
    public string L6Elec
    {
        get { return savedElec[5]; }
    }
}
