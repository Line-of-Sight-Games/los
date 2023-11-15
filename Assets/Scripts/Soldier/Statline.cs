using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statline 
{
    [JsonIgnore] public Soldier soldierBelongsTo;
    private readonly List<Stat> stats = new();

    public Statline(Soldier soldier) 
    {
        soldierBelongsTo = soldier;

        stats.Add(new Stat(this, "L", "Leadership", "ability to increase AP", 1, 1));
        stats.Add(new Stat(this, "H", "Health", "maximum health", 6, 1));
        stats.Add(new Stat(this, "R", "Resilience", "ability to resist adverse effects", 1, 1));
        stats.Add(new Stat(this, "S", "Speed", "maximum movement", 30, 6));
        stats.Add(new Stat(this, "E", "Evasion", "ability to dodge projectiles", 1, 1));
        stats.Add(new Stat(this, "F", "Stealth", "ability to avoid detection on your turn", 1, 1));
        stats.Add(new Stat(this, "P", "Perceptiveness", "ability to detect hidden enemies", 1, 1));
        stats.Add(new Stat(this, "C", "Camouflage", "ability to avoid detection on enemy turn", 1, 1));
        stats.Add(new Stat(this, "SR", "Sight Radius", "maximum sight radius", 100, 10));
        stats.Add(new Stat(this, "Ri", "Rifle", "skill with rifles", 1, 1));
        stats.Add(new Stat(this, "AR", "Assault Rifle", "skill with assault rifles", 1, 1));
        stats.Add(new Stat(this, "LMG", "Light Machine Gun", "skill with lmgs", 1, 1));
        stats.Add(new Stat(this, "Sn", "Sniper Rifle", "skill with sniper rifles", 1, 1));
        stats.Add(new Stat(this, "SMG", "Sub-Machine Gun", "skill with smgs", 1, 1));
        stats.Add(new Stat(this, "Sh", "Shotgun", "skill with shotguns", 1, 1));
        stats.Add(new Stat(this, "M", "Melee", "skill with melee weapons", 1, 1));
        stats.Add(new Stat(this, "Str", "Strength", "ability to carry more and throw further", 1, 1));
        stats.Add(new Stat(this, "Dip", "Diplomacy", "ability to positively interact with NPC's and radios", 1, 1));
        stats.Add(new Stat(this, "Elec", "Electronics", "ability to positively interact with terminals and radios", 1, 1));
        stats.Add(new Stat(this, "Heal", "Healing", "ability to use MedKits to heal allies", 0, 1));
    }

    public Stat GetStat(string code)
    {
        foreach (Stat s in stats)
            if (s.Name == code || s.Longname == code)
                return s;
        return null;
    }

    public void SetStat(string code, int val)
    {
        foreach (Stat s in stats)
        {
            if (s.Name == code)
                s.BaseVal = val;
        }
    }

    public int GetHighestWeaponSkill()
    {
        int highest = 0;
        Stat[] weaponSkills = { Ri, AR, LMG, Sn, SMG, Sh };

        foreach (Stat s in weaponSkills)
            if (s.Val > highest)
                highest = s.Val;

        return highest;
    }

    public List<Stat> AllStats
    {
        get { return stats; }
    }
    public Stat L
    {
        get { return GetStat("L"); }
    }
    public Stat H
    {
        get { return GetStat("H"); }
    }
    public Stat R
    {
        get { return GetStat("R"); }
    }
    public Stat S
    {
        get { return GetStat("S"); }
    }
    public Stat E
    {
        get { return GetStat("E"); }
    }
    public Stat F
    {
        get { return GetStat("F"); }
    }
    public Stat P
    {
        get { return GetStat("P"); }
    }
    public Stat C
    {
        get { return GetStat("C"); }
    }
    public Stat SR
    {
        get { return GetStat("SR"); }
    }
    public Stat Ri
    {
        get { return GetStat("Ri"); }
    }
    public Stat AR
    {
        get { return GetStat("AR"); }
    }
    public Stat LMG
    {
        get { return GetStat("LMG"); }
    }
    public Stat Sn
    {
        get { return GetStat("Sn"); }
    }
    public Stat SMG
    {
        get { return GetStat("SMG"); }
    }
    public Stat Sh
    {
        get { return GetStat("Sh"); }
    }
    public Stat M
    {
        get { return GetStat("M"); }
    }
    public Stat Str
    {
        get { return GetStat("Str"); }
    }
    public Stat Dip
    {
        get { return GetStat("Dip"); }
    }
    public Stat Elec
    {
        get { return GetStat("Elec"); }
    }
    public Stat Heal
    {
        get { return GetStat("Heal"); }
    }
}
