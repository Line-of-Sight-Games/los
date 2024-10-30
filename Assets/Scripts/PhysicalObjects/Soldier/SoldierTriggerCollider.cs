using UnityEditor;
using UnityEngine;

public class SoldierTriggerCollider : BaseTriggerCollider
{
    public Soldier linkedSoldier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        linkedObject = linkedSoldier;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Soldier LinkedSoldier 
    { 
        get { return linkedSoldier; } 
        set { linkedSoldier = value; } 
    }
}
