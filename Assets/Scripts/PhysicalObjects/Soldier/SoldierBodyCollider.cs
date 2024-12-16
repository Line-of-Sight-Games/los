using UnityEngine;

public class SoldierBodyCollider : BaseBodyCollider
{
    public Soldier linkedSoldier;
    public GameObject physicalSoldier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        linkedBody = linkedSoldier;
    }

    // Update is called once per frame
    void Update()
    {
        if (linkedSoldier.IsDead() || linkedSoldier.IsUnconscious() || linkedSoldier.IsPlayingDead())
        {
            //doesn't work cause blocks soldier physical from moving
            physicalSoldier.transform.position = new(physicalSoldier.transform.position.x, 0.5f, physicalSoldier.transform.position.z);
            physicalSoldier.transform.localScale = new(physicalSoldier.transform.localScale.x, 0.5f, physicalSoldier.transform.localScale.z);
        }
        else if (linkedSoldier.IsLastStand())
        {
            physicalSoldier.transform.position = new(physicalSoldier.transform.position.x, 1, physicalSoldier.transform.position.z);
            physicalSoldier.transform.localScale = new(physicalSoldier.transform.localScale.x, 1, physicalSoldier.transform.localScale.z);
        }
        else
        {
            physicalSoldier.transform.position = new(physicalSoldier.transform.position.x, 2, physicalSoldier.transform.position.z);
            physicalSoldier.transform.localScale = new(physicalSoldier.transform.localScale.x, 2, physicalSoldier.transform.localScale.z);
        }
    }

    public Soldier LinkedSoldier
    {
        get { return linkedSoldier; }
        set { linkedSoldier = value; }
    }
}
