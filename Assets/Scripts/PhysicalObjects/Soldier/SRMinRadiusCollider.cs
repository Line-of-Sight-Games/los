using System.Collections.Generic;
using UnityEngine;

public class SRMinRadiusCollider : SoldierTriggerCollider
{
    public List<string> soldiersWithinMinSR;
    public void SafeAddToList(Soldier soldier)
    {
        if (!soldiersWithinMinSR.Contains(soldier.Id))
            soldiersWithinMinSR.Add(soldier.Id);
    }
    public void RemoveFromList(Soldier soldier)
    {
        soldiersWithinMinSR.Remove(soldier.Id);
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
