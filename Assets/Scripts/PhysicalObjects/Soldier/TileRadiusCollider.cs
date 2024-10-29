using UnityEngine;

public class TileRadiusCollider : BaseTriggerCollider
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
            //print($"{soldierBodyThatEntered.linkedSoldier.soldierName} entered the TileRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
        }
    }
    private void OnTriggerExit(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            //print($"{soldierBodyThatEntered.linkedSoldier.soldierName} exited the TileRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
        }
    }
}
