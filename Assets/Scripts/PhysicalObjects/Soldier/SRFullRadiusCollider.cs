using System.Reflection.Emit;
using UnityEngine;

public class SRFullRadiusCollider : SoldierTriggerCollider
{
    private int pMultiplier = 1;
    public string DetermineDetecteeLabel(Soldier detector, Soldier detectee)
    {
        if (detectee.ActiveC > detector.ActivePForDetection(pMultiplier))
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
                    menu.detectionUI.LOSAlertSoldierSoldierStart(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                    print($"{soldierThatEntered.LinkedSoldier.soldierName} entered the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
                }
            }
            else if (bodyThatEntered.TryGetComponent(out ClaymoreBodyCollider claymoreThatEntered))
            {
                print($"{claymoreThatEntered.LinkedClaymore} ({claymoreThatEntered.LinkedClaymore.X},{claymoreThatEntered.LinkedClaymore.Y},{claymoreThatEntered.LinkedClaymore.Z}) entered the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
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
                print($"{claymoreThatStayed.LinkedClaymore} ({claymoreThatStayed.LinkedClaymore.X},{claymoreThatStayed.LinkedClaymore.Y},{claymoreThatStayed.LinkedClaymore.Z}) stayed in the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatStayed)}");
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
                    menu.detectionUI.LOSAlertSoldierSoldierEnd(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                    print($"{soldierThatExited.LinkedSoldier.soldierName} exited the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatExited)}");
                }
            }
            else if (bodyThatExited.TryGetComponent(out ClaymoreBodyCollider claymoreThatExited))
            {
                print($"{claymoreThatExited.LinkedClaymore} ({claymoreThatExited.LinkedClaymore.X},{claymoreThatExited.LinkedClaymore.Y},{claymoreThatExited.LinkedClaymore.Z}) exited the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatExited)}");
            }
        }
    }
}
