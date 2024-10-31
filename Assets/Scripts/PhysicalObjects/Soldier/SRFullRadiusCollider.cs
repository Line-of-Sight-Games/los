using System.Reflection.Emit;
using UnityEngine;

public class SRFullRadiusCollider : SoldierTriggerCollider
{
    private int pMultiplier = 1;
    public void OnTriggerEnter(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
            {
                Soldier detector = LinkedSoldier;
                Soldier detectee = soldierThatEntered.LinkedSoldier;
                string detecteeLabel = "";
                if (detector.IsOppositeTeamAs(detectee))
                {
                    if (detectee.ActiveC > detector.ActivePForDetection(pMultiplier))
                    {
                        detecteeLabel = "AVOIDED";
                        //avoidanceRight = true;
                    }
                    else
                    {
                        detecteeLabel = "DETECTED";
                        //avoidanceRight = true;
                    }
                    menu.detectionUI.LOSAlertSoldierSoldierStart(detector, detectee, detecteeLabel);
                    print($"{soldierThatEntered.LinkedSoldier.soldierName} entered the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
                }
            }
            else if (bodyThatEntered.TryGetComponent(out ClaymoreBodyCollider claymoreThatEntered))
            {
                print($"{claymoreThatEntered.LinkedClaymore} ({claymoreThatEntered.LinkedClaymore.X},{claymoreThatEntered.LinkedClaymore.Y},{claymoreThatEntered.LinkedClaymore.Z}) entered the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
            }
        }
    }
    public void OnTriggerExit(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
            {
                Soldier detector = LinkedSoldier;
                Soldier detectee = soldierThatEntered.LinkedSoldier;
                string detecteeLabel = "";
                if (detector.IsOppositeTeamAs(detectee))
                {
                    if (detectee.ActiveC > detector.ActivePForDetection(pMultiplier))
                    {
                        detecteeLabel = "AVOIDED";
                        //avoidanceRight = true;
                    }
                    else
                    {
                        detecteeLabel = "DETECTED";
                        //avoidanceRight = true;
                    }
                    menu.detectionUI.LOSAlertSoldierSoldierEnd(detector, detectee, detecteeLabel);
                    print($"{soldierThatEntered.LinkedSoldier.soldierName} exited the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
                }
            }
            else if (bodyThatEntered.TryGetComponent(out ClaymoreBodyCollider claymoreThatEntered))
            {
                print($"{claymoreThatEntered.LinkedClaymore} ({claymoreThatEntered.LinkedClaymore.X},{claymoreThatEntered.LinkedClaymore.Y},{claymoreThatEntered.LinkedClaymore.Z}) exited the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
            }
        }
    }
}
