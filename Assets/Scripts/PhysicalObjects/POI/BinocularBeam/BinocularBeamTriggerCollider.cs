using UnityEngine;

public class BinocularBeamTriggerCollider : SoldierTriggerCollider
{
    public BinocularBeam linkedBinocularBeam;
    private void Start()
    {
        LinkedSoldier = LinkedBinocularBeam.placedBy;
    }
    private void Update()
    {
        LinkedSoldier = LinkedBinocularBeam.placedBy;
    }
    public int ActivePBinocs()
    {
        if (LinkedBinocularBeam.flashMode)
            return 4;
        else
            return LinkedBinocularBeam.turnsActive / 2;
    }
    public string DetermineDetecteeLabel(Soldier detector, Soldier detectee)
    {
        if (detectee.ActiveC > detector.stats.P.Val + ActivePBinocs())
            return "AVOID";
        else
            return "DETECT";
    }
    public void OnTriggerEnter(Collider colliderThatEntered)
    {
        if (LinkedSoldier != null)
        {
            if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
            {
                if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
                {
                    Soldier detector = LinkedSoldier;
                    Soldier detectee = soldierThatEntered.LinkedSoldier;

                    if (detector.IsOppositeTeamAs(detectee))
                    {
                        //flag that the soldier is within the collider
                        if (!detector.soldiersWithinAnyCollider.Contains(detectee.Id))
                            detector.soldiersWithinAnyCollider.Add(detectee.Id);

                        if (detector.losCheck || detectee.losCheck) //only trigger if a change has happened
                        {
                            MenuManager.Instance.detectionUI.LOSAlertSoldierSoldierStart(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                            print($"{soldierThatEntered.LinkedSoldier.soldierName} entered the binocular beam of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
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
                                MenuManager.Instance.detectionUI.LOSAlertSoldierClaymore(detector, claymore);
                                print($"{claymoreThatEntered.LinkedClaymore} ({claymoreThatEntered.LinkedClaymore.X},{claymoreThatEntered.LinkedClaymore.Y},{claymoreThatEntered.LinkedClaymore.Z}) entered the binocular beam of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
                            }
                        }
                    }
                }
            }
        }
    }
    public void OnTriggerStay(Collider colliderThatStayed)
    {
        if (LinkedSoldier != null)
        {
            if (IsValidBodyCollision(colliderThatStayed, out BaseBodyCollider bodyThatStayed))
            {
                if (bodyThatStayed.TryGetComponent(out SoldierBodyCollider soldierThatStayed))
                {
                    Soldier detector = LinkedSoldier;
                    Soldier detectee = soldierThatStayed.LinkedSoldier;

                    if (detector.IsOppositeTeamAs(detectee))
                    {
                        //flag that the soldier is within the collider
                        if (!detector.soldiersWithinAnyCollider.Contains(detectee.Id))
                            detector.soldiersWithinAnyCollider.Add(detectee.Id);

                        if (detector.losCheck || detectee.losCheck) //only trigger if a change has happened
                        {
                            MenuManager.Instance.detectionUI.LOSAlertSoldierSoldierStay(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                            print($"{soldierThatStayed.LinkedSoldier.soldierName} stayed in the binocular beam of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatStayed)}");
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
                                MenuManager.Instance.detectionUI.LOSAlertSoldierClaymore(detector, claymore);
                                print($"{claymoreThatStayed.LinkedClaymore} ({claymoreThatStayed.LinkedClaymore.X},{claymoreThatStayed.LinkedClaymore.Y},{claymoreThatStayed.LinkedClaymore.Z}) stayed in the binocular beam of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatStayed)}");
                            }
                        }
                    }
                }
            }
        }
    }
    public void OnTriggerExit(Collider colliderThatExited)
    {
        if (LinkedSoldier != null)
        {
            if (IsValidBodyCollision(colliderThatExited, out BaseBodyCollider bodyThatExited))
            {
                if (bodyThatExited.TryGetComponent(out SoldierBodyCollider soldierThatExited))
                {
                    Soldier detector = LinkedSoldier;
                    Soldier detectee = soldierThatExited.LinkedSoldier;

                    if (detector.IsOppositeTeamAs(detectee))
                    {
                        if (detector.soldiersWithinAnyCollider.Contains(detectee.Id))
                        {
                            detector.soldiersWithinAnyCollider.Remove(detectee.Id); //flag that the soldier has left the collider

                            if (detector.losCheck || detectee.losCheck) //only trigger if a change has happened
                            {
                                MenuManager.Instance.detectionUI.LOSAlertSoldierSoldierEnd(detector, detectee, DetermineDetecteeLabel(detector, detectee));
                                print($"{soldierThatExited.LinkedSoldier.soldierName} exited the binocular beam of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatExited)}");
                            }
                        }
                    }
                }
            }
        }
    }

    public BinocularBeam LinkedBinocularBeam
    {
        get { return linkedBinocularBeam; }
        set { linkedBinocularBeam = value; }
    }
}
