using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SRHalfRadiusCollider : SoldierTriggerCollider
{
    public List<string> soldiersWithinHalfSR;
    public void SafeAddToList(Soldier soldier)
    {
        if (!soldiersWithinHalfSR.Contains(soldier.Id))
            soldiersWithinHalfSR.Add(soldier.Id);
    }
    public void RemoveFromList(Soldier soldier)
    {
        soldiersWithinHalfSR.Remove(soldier.Id);
    }
    public void OnTriggerEnter(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
            {
                if (LinkedSoldier.IsOppositeTeamAs(soldierThatEntered.LinkedSoldier))
                    SafeAddToList(soldierThatEntered.LinkedSoldier);
            }
        }
    }
    public void OnTriggerStay(Collider colliderThatStayed)
    {
        if (IsValidBodyCollision(colliderThatStayed, out BaseBodyCollider bodyThatStayed))
        {
            if (bodyThatStayed.TryGetComponent(out SoldierBodyCollider soldierThatStayed))
            {
                if (LinkedSoldier.IsOppositeTeamAs(soldierThatStayed.LinkedSoldier))
                    SafeAddToList(soldierThatStayed.LinkedSoldier);
            }
        }
    }
    public void OnTriggerExit(Collider colliderThatExited)
    {
        if (IsValidBodyCollision(colliderThatExited, out BaseBodyCollider bodyThatExited))
        {
            if (bodyThatExited.TryGetComponent(out SoldierBodyCollider soldierThatExited))
            {
                if (LinkedSoldier.IsOppositeTeamAs(soldierThatExited.LinkedSoldier))
                    RemoveFromList(soldierThatExited.LinkedSoldier);
            }
        }
    }
}
