using UnityEngine;

public class BaseBodyCollider : MonoBehaviour
{
    public PhysicalObject linkedBody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public PhysicalObject LinkedBody { get { return linkedBody; } }
}
