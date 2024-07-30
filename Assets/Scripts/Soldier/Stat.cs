using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Stat
{

    [JsonIgnore] public Statline statlineBelongsTo;
    private string name;
    private string longname;
    private string description;
    private int baseValue;
    private int activeValue;
    private int increment;

    public Stat(Statline statline, string name, string longname, string desc, int baseVal, int inc)
    {
        statlineBelongsTo = statline;
        this.name = name;
        this.longname = longname;
        description = desc;
        baseValue = baseVal;
        activeValue = baseVal;
        increment = inc;
    }
    public string Name
    {
        get { return name; }
    }

    [JsonIgnore]
    public string Longname
    {
        get { return longname; }
    }

    [JsonIgnore]
    public int ReadIncrement
    {
        get { return increment; }
    }

    [JsonIgnore]
    public int Val
    {
        get { return activeValue; }
        set { activeValue = value; }
    }

    public int BaseVal
    {
        get { return baseValue; }
        set
        {
            baseValue = value;
            if (statlineBelongsTo.soldierBelongsTo != null && statlineBelongsTo.soldierBelongsTo.game != null && statlineBelongsTo.soldierBelongsTo.game.GameRunning)
            {
                statlineBelongsTo.soldierBelongsTo.CalculateActiveStats();
                if (statlineBelongsTo.soldierBelongsTo.IsMeleeEngaged() && (Name == "R" || Name == "M" || Name == "Str"))
                    statlineBelongsTo.soldierBelongsTo.game.StartCoroutine(statlineBelongsTo.soldierBelongsTo.game.DetermineMeleeControllerMultiple(statlineBelongsTo.soldierBelongsTo));
                else if (Name == "SR" || Name == "C" || Name == "F" || Name == "P")
                    statlineBelongsTo.soldierBelongsTo.game.StartCoroutine(statlineBelongsTo.soldierBelongsTo.game.DetectionAlertSingle(statlineBelongsTo.soldierBelongsTo, "statChange", Vector3.zero, string.Empty, false));
            }
        }
    }
    public int Increment()
    {
        BaseVal += ReadIncrement;
        if (statlineBelongsTo.soldierBelongsTo.game != null && Name == "H")
            statlineBelongsTo.soldierBelongsTo.TakeHeal(null, 1, 0, false, false);

        return ReadIncrement;
    }
    public int Decrement()
    {
        BaseVal -= ReadIncrement;
        if (BaseVal < 0)
            BaseVal = 0;

        return ReadIncrement;
    }
}
