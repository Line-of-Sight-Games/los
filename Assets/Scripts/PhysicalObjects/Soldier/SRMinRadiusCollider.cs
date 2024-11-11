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
                SafeAddToList(soldierThatEntered.LinkedSoldier);
                print($"{soldierThatEntered.LinkedSoldier.soldierName} entered the SRMinRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
            }
        }
    }
    public void OnTriggerStay(Collider colliderThatStayed)
    {
        if (IsValidBodyCollision(colliderThatStayed, out BaseBodyCollider bodyThatStayed))
        {
            if (bodyThatStayed.TryGetComponent(out SoldierBodyCollider soldierThatStayed))
            {
                SafeAddToList(soldierThatStayed.LinkedSoldier);
                print($"{soldierThatStayed.LinkedSoldier.soldierName} stayed in the SRMinRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatStayed)}");
            }
        }
    }
    public void OnTriggerExit(Collider colliderThatExited)
    {
        if (IsValidBodyCollision(colliderThatExited, out BaseBodyCollider bodyThatExited))
        {
            if (bodyThatExited.TryGetComponent(out SoldierBodyCollider soldierThatExited))
            {
                RemoveFromList(soldierThatExited.LinkedSoldier);
                print($"{soldierThatExited.LinkedSoldier.soldierName} exited the SRMinRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatExited)}");
            }
        }
    }
}
