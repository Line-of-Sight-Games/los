using UnityEngine;

public class BaseTriggerCollider : MonoBehaviour
{
    public MainMenu menu;
    public PhysicalObject linkedObject;
    private void Awake()
    {
        menu = FindFirstObjectByType<MainMenu>();
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
