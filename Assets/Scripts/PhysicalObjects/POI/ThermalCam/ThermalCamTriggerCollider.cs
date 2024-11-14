using UnityEngine;

public class ThermalCamTriggerCollider : BaseTriggerCollider
{
    public ThermalCamera linkedThermalCam;
    public void OnTriggerEnter(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
            {
                if (LinkedThermalCamera.placedBy != null)
                {
                    Soldier detector = LinkedThermalCamera.placedBy;
                    Soldier detectee = soldierThatEntered.LinkedSoldier;

                    //flag that the soldier is within the collider
                    if (!detector.soldiersWithinAnyCollider.Contains(detectee.Id))
                        detector.soldiersWithinAnyCollider.Add(detectee.Id);

                    if (detectee.IsOppositeTeamAs(LinkedThermalCamera.placedBy))
                    {
                        if (detectee.losCheck && (detectee.causeOfLosCheck.Contains("move") || detectee.causeOfLosCheck.Contains("thermalCam"))) //only trigger if a change has happened and it's a move or the placement of the cam
                        {
                            menu.detectionUI.LOSAlertThermalCamSoldierStart(LinkedThermalCamera, detectee);
                            print($"{soldierThatEntered.LinkedSoldier.soldierName} entered the beam of {LinkedThermalCamera} ({LinkedThermalCamera.X},{LinkedThermalCamera.Y},{LinkedThermalCamera.Z}) at {CollisionPoint(colliderThatEntered)}");
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
                if (LinkedThermalCamera.placedBy != null)
                {
                    Soldier detector = LinkedThermalCamera.placedBy;
                    Soldier detectee = soldierThatStayed.LinkedSoldier;

                    //flag that the soldier is within the collider
                    if (!detector.soldiersWithinAnyCollider.Contains(detectee.Id))
                        detector.soldiersWithinAnyCollider.Add(detectee.Id);

                    if (detectee.IsOppositeTeamAs(LinkedThermalCamera.placedBy))
                    {
                        if (detectee.losCheck && (detectee.causeOfLosCheck.Contains("move") || detectee.causeOfLosCheck.Contains("thermalCam"))) //only trigger if a change has happened and it's a move or the placement of the cam
                        {
                            menu.detectionUI.LOSAlertThermalCamSoldierStay(LinkedThermalCamera, detectee);
                            print($"{soldierThatStayed.LinkedSoldier.soldierName} stayed in the beam of {LinkedThermalCamera} ({LinkedThermalCamera.X},{LinkedThermalCamera.Y},{LinkedThermalCamera.Z}) at {CollisionPoint(colliderThatStayed)}");
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
                if (LinkedThermalCamera.placedBy != null)
                {
                    Soldier detector = LinkedThermalCamera.placedBy;
                    Soldier detectee = soldierThatExited.LinkedSoldier;

                    //flag that the soldier has left the collider
                    detector.soldiersWithinAnyCollider.Remove(detectee.Id);

                    if (detectee.IsOppositeTeamAs(LinkedThermalCamera.placedBy))
                    {
                        if (detectee.losCheck && (detectee.causeOfLosCheck.Contains("move") || detectee.causeOfLosCheck.Contains("thermalCam"))) //only trigger if a change has happened and it's a move or the placement of the cam
                        {
                            menu.detectionUI.LOSAlertThermalCamSoldierEnd(LinkedThermalCamera, detectee);
                            print($"{soldierThatExited.LinkedSoldier.soldierName} exited the beam of {LinkedThermalCamera} ({LinkedThermalCamera.X},{LinkedThermalCamera.Y},{LinkedThermalCamera.Z}) at {CollisionPoint(colliderThatExited)}");
                        }
                    }
                }
            }
        }
    }

    public ThermalCamera LinkedThermalCamera
    {
        get { return linkedThermalCam; }
        set { linkedThermalCam = value; }
    }
}
