using System.Collections.Generic;
using UnityEngine;

public class DipelecGen : MonoBehaviour, IDataPersistence
{
    private string[] l1Dip =
    {
        "Receive a single piece of medium-level advice if it is asked in the form of a yes/no Question",
        "Retry your next missed shot once (excluding overwatch) (Must have required ammo)",
        "For one turn only, all soldiers may enter cover for 0AP (Player may use this on the current turn or use anytime provided notice is given the previous turn)",
        "Provide 2 false detection movements to the Opponent. Detection movements must start and end out of the Opponent's view",
        "Learn approximate location of all enemy units. Receive one guaranteed Opponent overwatch miss for every two enemy soldiers who were already revealed",
        "The next enemy soldier who successfully shoots a friendly soldier will be stunned for 1 turn",
        "Can swap one grenade currently equipped by any soldier for a different grenade. If no grenades are held, award any one grenade to the negotiator",
        "Player receives one free successful ULF effect to use anytime (no XP)",
        "Next successful shot will stun enemy soldier for one turn",
        "Negotiator receives 1Elec. If he has previously failed a hack, rerun it (most recent attempt at same level for 0AP) with the new Elec skill",
    };
    private string[] l2Dip =
    {
        "Next shot will hit (XP given)",
        "For one turn, the Player can ask if any revealed enemies are in cover. If any are, all enemy covers will be cancelled. If they are not, the Player retains the ability for another turn (Player may use this on the current turn or use anytime provided notice is given the previous turn) (Opponent informed once covers are cancelled)",
        "Look at enemy suadcard for 60s (Opponent informed)",
        "The Player may remove one of their soldiers from the field. The soldier will return from where they were taken at start of their next turn. This ability may be used once anytime",
        "Reorganise all equipment between fielded soldiers to the Player's liking",
        "Cancel the ability of the furthest revealed enemy from the negotiator. If all enemies are currently hidden, this effect will apply to the next revealed enemy (Opponent informed)",
        "Relocate any friendly or revealed enemy soldier along a straight line between their current position and where they fielded from (Opponent informed of enemy soldier)",
        "Next two enemy shots will miss (XP given) (Opponent informed of reason after miss)",
        "At the beginning of the Opponent's next turn, their overwatch will be disabled for 6 turns (Opponent informed)",
        "Poison the first soldier (friendly or enemy, no XP) encountered in a wind direction (8 options) chosen by the Player. Player may also poison closest soldier to centre if desired (Opposed informed if enemy poisoined)",
    };
    private string[] l3Dip =
    {
        "Field a randomly generated Recruit in your chosen specialty at a valid spawn point (Opponent informed of extra man). He may field with one gun, one knife or one grenade",
        "Place 3 thermal cameras anywhere on the map (negotiator to receive XP for detections)",
        "Move and rotate a reasonable terrain feature to any location where there is room (will carry everything)",
        "All equipped enemy grenades, radios and claymores will be confiscated by the GM (Opponent informed)",
        "Place six claymores anywhere on the field (XP given) that is not within 6cm of a known enemy location (will use the Negotiator's F and C values and the Negotiator's L cannot decrease for the rest of the game)",
        "In situations where the Player believes there is a mutual LOS between friendly and enemy soldiers, they may render their soldier unshootable until the end of their Opponent's turn. Can be used twice at anytime. (Opponent informed of reason after miss)",
        "All enemy soldiers accrue an additional trauma level (Opponent infromed)",
        "One fielded soldier must call a free UHF strike (XP given)",
        "Choose a soldier. Completely rebuild (choose) their specialty, ability and skill point distribution (duplicates only permitted for deceased soldiers). Their XP, body and equipment will be conserved. They may equip a gun or swap if one is already equipped.",
        "Enemy Corporals (and above) will be unable to take cover, equip new Specialty Armours or control engagements (Opponent informed)",
    };
    private string[] l4Dip =
    {
        "After any attempted shot (either to or from the enemy), teleport the relevant enemy anywhere along the bullet's trajectory (Can be mid-air...enemy will fall). Can be used twice (Opponent informed after each use)",
        "Administer any one drug to any one soldier without a side effect",
        "Set all enemy Support skills to 0. -1 to enemy Weapon skills (Opponent informed)",
        "Teleport entire fielded team to a single location (3cm distancing kept). Can be used anytime (only once) (Opponent informed if used but not the location)",
        "Reset all starvation and trauma for fielded soldiers to 0. Remove any active poison",
        "Remove 10 items of indestructible cover. Any enemies currently in cover will be poisoned and receive 1 trauma point. (Opponent informed)",
        "Choose a terrain. All soldiers of that terrain will receive a UHF radio",
        "All fielded enemies except the most experience enemy (unless he is the sole survivor) will be poisoned (no XP) (Opponent informed)",
        "+2 increments to all soldiers' Support and Weapon skills",
        "Any enemies including and above the rank of Captain are executed. If none exist, one random unfielded soldier will be executed, if none exist, the most experienced enemy will have his HP halved (round down) (Opponent informed)",
    };
    private string[] l5Dip =
    {
        "All fielded enemies become Frozen. Unfielded enemies are executed (Opponent informed)",
        "Entire team's Dip and Elec skills are added together to deliver a UHF strike. If Negotiator is sole survivor, his skill points will be doubled. If no enemies are killed by this strike, it may be redone (once, XP given)",
        "Player never leaves the Command Zone and all enemy radio communications are permanently disabled. (Opponent informed)",
        "All enemies must pass a deathroll to survive (no XP). If they all survive, all except the lowest ranked (Opponent chooses sole survivor if there are ties) will all be executed (Opponent informed)",
        "Disable two of either shooting, melee or explosives for the enemy (Oppponent informed)",
    };
    private string[] l6Dip =
    {
        "Victory",
    };

    private string[] l1Elec =
    {
        "Hacker receives 1Dip. If he has previously failed a negotiation, rerun it (most recent attempt at same level for 0AP) with the new Dip skill",
        "Receive 9S and 15SR to distribute amongst fielded soldiers as desired",
        "Move any POI up to 20cm away from its origin in one of the 6 cardinal directions (x,-x,y,-y,z,-z) provided it ends up in an accessible spot (Opponent informed)",
        "Next turn, award 1AP to all soldiers who fail to receive Leadership AP",
        "Learn exact average enemy position- if all enemies are already revealed, award 3AP to any one soldier",
        "Deploy either a smoke or tabun grenade anywhere that can be see from directly above (no XP)",
        "Learn exact location and receive a glimpse of the farthest hidden enemy (no XP). If all enemies are already revealed, all fielded soldiers will gain 5SR",
        "Learn Commander's (or next most powerful unit's) exact location. If all enemies are already revealed, all soldiers will gain 5SR",
        "All fielded soldiers (except the hacker) receive 2XP or the hacker receives 6XP",
        "Any soldiers between half and full health will be fully healed. If this does not apply to any soldiers. Receive 2H to distribute amongst team (cannot be applied to soldiers below half health)",
    };
    private string[] l2Elec =
    {
        "Choose a soldier to link with. Any HP lost by either soldier will now be split 50/50 between the nominated soldier and the hacker. If damage is an odd number, reduce it by 1 first. Both soldiers receive 1H",
        "All fielded soldiers receive +1 increment to their specialty",
        "Enemy shotcalcs will no longer provide hit percentages. They will also lose the ability to withhold shots under 25% (Opponent informed)",
        "Award BA to one soldier not wearing any armour. If the player prefers, or if all soldiers are already wearing BA, award 1H to all fielded soldiers (If hacker is sole survivor, he earns 4H)",
        "The hacker and any soldiers with a direct LOS to him shall receive +1 increment in all Weapon and Support skills excluding their specialties. Soldiers who cannot see the hacker shall earn +1 increment to their specialty",
        "The next explosive which deals any damage to a soldier will inflict only 50% (round down) to all soldiers",
        "Select any fielded soldier. For as long as they live, any damage received by this soldier will be split 50/50 between them and the soldier which caused their damage. If the damage is an odd number, increase it by 1 first.  The attacking soldier may not drop below 3HP from this effect",
        "All soldiers already on their native terrain shall earn 1H, 1E and 1M. Any soldiers not on their native terrain shall have the map feature (up to a rough 14cm radius) they are currently on converted to their native terrain with no bonuses. Soldiers on the baseboard will have a 14cm radius circle projected around them)",
        "10SR awarded to fielded soldiers and entire weather forecast is known.",
        "At the beginning of the Player's next turn, reveal all enemies visible from directly above (no XP). If this reveals no new enemies, award 9S to all fielded soldiers",
    };
    private string[] l3Elec =
    {
        "Distribute 8AP amongst fielded soldiers",
        "Soldiers will be fully healed (no XP). Soldiers already on full health receive 1H",
        "Neither shots nor suppression consume ammunition. Remove any spare ammo equipped on enemy soldiers.",
        "All fielded enemy soldiers accrue an additional 10 turns of starvation (Opponent informed)",
        "Select a revealed enemy soldier who will spend 3AP each turn traversing the quickest route to a spawn of your choice (choose once). The enemy soldier may use additional AP as they wish. When (if) they arrive, the Opponent will then resume regular control (Opponent informed).",
        "Enemy soldiers will receive an unresistable suppression penalty of 80 on their next turn. Can be used anytime (Opponent informed)",
        "Engine will randomly generate 5 abilities, choose one of these to award to all fielded allies (same ability for all)",
        "Squadsight disabled for the enemy (Opponent informed)",
        "-12S to all fielded enemies (Opponent informed)",
        "Soldiers can permanently retry failed rolls involving R",
    };
    private string[] l4Elec =
    {
        "All fielded soldiers (except the hacker…unless he is the sole survivor) shall have their first ability upgraded",
        "Permanently remove all current and future appearances of a list of items specified by the Player (Opponent informed)",
        "The player's last surviving soldier will have his H tripled and his HP restored to maximum. He may also choose an additional unupgraded ability",
        "Structurally collapse up to 6 structures (Opponent informed)",
        "Choose one primary, weapon and support skill (Not H, U, P or SR). All soldiers will have these skills doubled while all fielded enemies will have them zeroed (Opponent informed)",
        "Enemy soldiers can never control engagements. Any melee damage dealt to enemies will cause immediate death. (Opponent informed)",
        "All soldiers will receive thermal goggles and a smoke grenade (if desired)",
        "Immediately resurrect a fallen soldier at his grave on full HP. He may commence his turn (Opponent informed)",
        "All fielded soldiers shall earn a 'life'. Completely ignore the event that would have killed them(once each)",
        "All actions now cost 1 less AP",
    };
    private string[] l5Elec =
    {
        "The Player receives 2 minutes to alter anything they want on the map. This can't damage enemy units, should debris be placed on them, they will be placed on top. All enemy soldiers are revealed before the 2 minutes commences (Opponent informed)",
        "Any damage dealt to any enemy soldier shall also be applied to his allies (Opponent informed)",
        "All soldiers awarded 6L",
        "All soldiers become invisible to everything until any soldier issues or takes damage",
        "Deploy 2 Captains anywhere on the map. Player shall choose skill points, skin, upgraded ability and 1 weapon, 1 item and 1 armour each",
    };
    private string[] l6Elec =
    {
        "Victory",
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
        savedDip.Add(l1Dip[Random.Range(0, 10)]);
        savedDip.Add(l2Dip[Random.Range(0, 10)]);
        savedDip.Add(l3Dip[Random.Range(0, 10)]);
        savedDip.Add(l4Dip[Random.Range(0, 10)]);
        savedDip.Add(l5Dip[Random.Range(0, 5)]);
        savedDip.Add(l6Dip[0]);

        savedElec.Add(l1Elec[Random.Range(0, 10)]);
        savedElec.Add(l2Elec[Random.Range(0, 10)]);
        savedElec.Add(l3Elec[Random.Range(0, 10)]);
        savedElec.Add(l4Elec[Random.Range(0, 10)]);
        savedElec.Add(l5Elec[Random.Range(0, 5)]);
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
