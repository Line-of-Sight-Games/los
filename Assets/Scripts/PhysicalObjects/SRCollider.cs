using UnityEngine;

public class SRCollider : MonoBehaviour
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
    public Vector3 CollisionPoint(Collider collider)
    {
        return HelperFunctions.ConvertPhysicalPosToMathPos(collider.ClosestPoint(transform.position));
    }
    public bool IsValidSRCollision(Collider other, out SoldierBodyCollider soldierBodyCollider)
    {
        soldierBodyCollider = null;
        if (other.gameObject.TryGetComponent(out SoldierBodyCollider collider))
        {
            soldierBodyCollider = collider;
            if (LinkedSoldier.OnBattlefield() && soldierBodyCollider.linkedSoldier.OnBattlefield())
                return true;
        }
        return false;
    }

    public Soldier LinkedSoldier
    {
        get { return linkedSoldier; }
    }
}
