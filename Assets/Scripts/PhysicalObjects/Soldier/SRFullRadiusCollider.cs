using UnityEngine;

public class SRFullRadiusCollider : BaseTriggerCollider
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
            //print($"{soldierBodyThatEntered.LinkedSoldier.soldierName} entered the SRFullRadiusCollider of {LinkedBody.soldierName} at {CollisionPoint(colliderThatEntered)}");
        }
    }
    public  void OnTriggerExit(Collider colliderThatEntered)
    {
        if (IsValidBodyCollision(colliderThatEntered, out BaseBodyCollider bodyThatEntered))
        {
            //print($"{soldierBodyThatEntered.LinkedSoldier.soldierName} exited the SRFullRadiusCollider of {LinkedSoldier.soldierName} at {CollisionPoint(colliderThatEntered)}");
        }
    }
}
