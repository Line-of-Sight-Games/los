using UnityEngine;

public class SoldierBodyCollider : BaseBodyCollider
{
    public Soldier linkedSoldier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Soldier LinkedSoldier { get { return linkedSoldier; } }
}
