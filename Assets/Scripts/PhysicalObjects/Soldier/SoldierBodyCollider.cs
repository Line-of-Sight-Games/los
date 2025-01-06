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
            physicalSoldier.transform.localPosition = new(physicalSoldier.transform.localPosition.x, 0.5f, physicalSoldier.transform.localPosition.z);
            physicalSoldier.transform.localScale = new(physicalSoldier.transform.localScale.x, 0.5f, physicalSoldier.transform.localScale.z);
        }
        else if (linkedSoldier.IsLastStand())
        {
            physicalSoldier.transform.localPosition = new(physicalSoldier.transform.localPosition.x, 1, physicalSoldier.transform.localPosition.z);
            physicalSoldier.transform.localScale = new(physicalSoldier.transform.localScale.x, 1, physicalSoldier.transform.localScale.z);
        }
        else
        {
            physicalSoldier.transform.localPosition = new(physicalSoldier.transform.localPosition.x, 2, physicalSoldier.transform.localPosition.z);
            physicalSoldier.transform.localScale = new(physicalSoldier.transform.localScale.x, 2, physicalSoldier.transform.localScale.z);
        }
    }

    public Soldier LinkedSoldier
    {
        get { return linkedSoldier; }
        set { linkedSoldier = value; }
    }
}
