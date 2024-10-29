using UnityEngine;

public class SRHalfRadiusCollider : BaseTriggerCollider
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            //print($"{soldierBodyThatEntered.linkedSoldier.soldierName} entered the SRHalfRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
        }
    }
    private void OnTriggerExit(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            //print($"{soldierBodyThatEntered.linkedSoldier.soldierName} exited the SRHalfRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
        }
    }
}
