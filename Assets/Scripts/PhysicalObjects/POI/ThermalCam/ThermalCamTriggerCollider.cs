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
                Soldier detectee = soldierThatEntered.LinkedSoldier;
                if (detectee.IsOppositeTeamAs(LinkedThermalCamera.placedBy))
                {
                    menu.detectionUI.LOSAlertThermalCamSoldierStart(LinkedThermalCamera, detectee);
                    print($"{soldierThatEntered.LinkedSoldier.soldierName} entered the beam of {LinkedThermalCamera} ({LinkedThermalCamera.X},{LinkedThermalCamera.Y},{LinkedThermalCamera.Z}) at {CollisionPoint(colliderThatEntered)}");
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
                Soldier detectee = soldierThatStayed.LinkedSoldier;
                if (detectee.IsOppositeTeamAs(LinkedThermalCamera.placedBy))
                {
                    menu.detectionUI.LOSAlertThermalCamSoldierStay(LinkedThermalCamera, detectee);
                    print($"{soldierThatStayed.LinkedSoldier.soldierName} stayed in the beam of {LinkedThermalCamera} ({LinkedThermalCamera.X},{LinkedThermalCamera.Y},{LinkedThermalCamera.Z}) at {CollisionPoint(colliderThatStayed)}");
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
                Soldier detectee = soldierThatExited.LinkedSoldier;
                if (detectee.IsOppositeTeamAs(LinkedThermalCamera.placedBy))
                {
                    menu.detectionUI.LOSAlertThermalCamSoldierEnd(LinkedThermalCamera, detectee);
                    print($"{soldierThatExited.LinkedSoldier.soldierName} exited the beam of {LinkedThermalCamera} ({LinkedThermalCamera.X},{LinkedThermalCamera.Y},{LinkedThermalCamera.Z}) at {CollisionPoint(colliderThatExited)}");
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
