using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

public class SRFullRadiusCollider : SoldierTriggerCollider
{
    public SRHalfRadiusCollider halfCollider;
    public SRMinRadiusCollider minCollider;
    public int ActivePMultiplier(Soldier detectee)
    {
        int pMultiplier = 1;
        if (halfCollider.soldiersWithinHalfSR.Contains(detectee.Id))
            pMultiplier = 2;
        if (minCollider.soldiersWithinMinSR.Contains(detectee.Id))
            pMultiplier = 3;

        print($"pmultiplier = {pMultiplier}");
        return pMultiplier;
    }
    public string DetermineDetecteeLabel(Soldier detector, Soldier detectee)
    {
        if (detectee.ActiveC > detector.ActivePForDetection(ActivePMultiplier(detectee)))
            return "AVOID";
        else if (detector.IsOnOverwatch())
            return "OVERWATCH";
        else
            return "DETECT";
    }
    public void OnTriggerEnter(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
            {
                Soldier detector = LinkedSoldier;
                Soldier detectee = soldierThatEntered.LinkedSoldier;
                if (detector.IsOppositeTeamAs(detectee))
                {
                    if (detector.losCheck || detectee.losCheck) //only trigger if a change has happened
                    {
                        menu.detectionUI.LOSAlertSoldierSoldierStart(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                        print($"{soldierThatEntered.LinkedSoldier.soldierName} entered the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
                    }
                }
            }
            else if (bodyThatEntered.TryGetComponent(out ClaymoreBodyCollider claymoreThatEntered))
            {
                Soldier detector = LinkedSoldier;
                Claymore claymore = claymoreThatEntered.LinkedClaymore;
                if (detector.IsOppositeTeamAs(claymore.placedBy))
                {
                    if (!claymore.revealed)
                    {
                        if (detector.stats.P.Val > claymore.ActiveC)
                        {
                            menu.detectionUI.LOSAlertSoldierClaymore(detector, claymore);
                            print($"{claymoreThatEntered.LinkedClaymore} ({claymoreThatEntered.LinkedClaymore.X},{claymoreThatEntered.LinkedClaymore.Y},{claymoreThatEntered.LinkedClaymore.Z}) entered the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
                        }
                    }
                }
            }
        }
    }
    public void OnTriggerStay(Collider colliderThatStayed)
    {
        if (IsValidBodyCollision(colliderThatStayed, out BaseBodyCollider bodyThatStayed))
        {
            if (bodyThatStayed.TryGetComponent(out SoldierBodyCollider soldierThatStayed))
            {
                Soldier detector = LinkedSoldier;
                Soldier detectee = soldierThatStayed.LinkedSoldier;
                if (detector.losCheck || detectee.losCheck) //only trigger if a change has happened
                {
                    if (detector.IsOppositeTeamAs(detectee))
                    {
                        menu.detectionUI.LOSAlertSoldierSoldierStay(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                        print($"{soldierThatStayed.LinkedSoldier.soldierName} stayed in the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatStayed)}");
                    }
                }
            }
            else if (bodyThatStayed.TryGetComponent(out ClaymoreBodyCollider claymoreThatStayed))
            {
                Soldier detector = LinkedSoldier;
                Claymore claymore = claymoreThatStayed.LinkedClaymore;
                if (detector.IsOppositeTeamAs(claymore.placedBy))
                {
                    if (!claymore.revealed)
                    {
                        if (detector.stats.P.Val > claymore.ActiveC)
                        {
                            menu.detectionUI.LOSAlertSoldierClaymore(detector, claymore);
                            print($"{claymoreThatStayed.LinkedClaymore} ({claymoreThatStayed.LinkedClaymore.X},{claymoreThatStayed.LinkedClaymore.Y},{claymoreThatStayed.LinkedClaymore.Z}) stayed in the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatStayed)}");
                        }
                    }
                }
            }
        }
    }
    public void OnTriggerExit(Collider colliderThatExited)
    {
        if (IsValidBodyCollision(colliderThatExited, out BaseBodyCollider bodyThatExited))
        {
            if (bodyThatExited.TryGetComponent(out SoldierBodyCollider soldierThatExited))
            {
                Soldier detector = LinkedSoldier;
                Soldier detectee = soldierThatExited.LinkedSoldier;
                if (detector.IsOppositeTeamAs(detectee))
                {
                    if (detector.IsOppositeTeamAs(detectee))
                    {
                        menu.detectionUI.LOSAlertSoldierSoldierEnd(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                        print($"{soldierThatExited.LinkedSoldier.soldierName} exited the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatExited)}");
                    }
                }
            }
        }
    }
}
