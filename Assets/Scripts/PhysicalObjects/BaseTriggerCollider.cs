using UnityEngine;

public class BaseTriggerCollider : MonoBehaviour
{
    public PhysicalObject linkedObject;
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
    public bool IsValidBodyCollision(Collider other, out BaseBodyCollider bodyCollider)
    {
        bodyCollider = null;
        if (other.gameObject.TryGetComponent(out BaseBodyCollider collider))
        {
            bodyCollider = collider;
            if (LinkedObject.OnBattlefield() && bodyCollider.LinkedBody.OnBattlefield())
                return true;
        }
        return false;
    }

    public PhysicalObject LinkedObject
    {
        get { return linkedObject; }
    }
}
