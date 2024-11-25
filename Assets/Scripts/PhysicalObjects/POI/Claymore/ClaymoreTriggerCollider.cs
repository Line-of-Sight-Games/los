using UnityEngine;

public class ClaymoreTriggerCollider : BaseTriggerCollider
{
    public Claymore linkedClaymore;
    public void OnTriggerEnter(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
            {
                if (!LinkedClaymore.Triggered)
                {
                    if (LinkedClaymore.CheckClaymoreTriggered(soldierThatEntered.LinkedSoldier))
                        LinkedClaymore.CheckExplosionClaymore(soldierThatEntered.LinkedSoldier, false);
                }
            }
        }
    }
    public void OnTriggerStay(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            if (bodyThatEntered.TryGetComponent(out SoldierBodyCollider soldierThatEntered))
            {
                if (!LinkedClaymore.Triggered)
                {
                    if (LinkedClaymore.CheckClaymoreTriggered(soldierThatEntered.LinkedSoldier))
                        LinkedClaymore.CheckExplosionClaymore(soldierThatEntered.LinkedSoldier, false);
                }
            }
        }
    }

    public Claymore LinkedClaymore
    {
        get { return linkedClaymore; }
        set { linkedClaymore = value; }
    }
}
