using UnityEngine;

public class ActiveSoldier : MonoBehaviour
{
    public static ActiveSoldier Instance { get; private set; }

    public Soldier S;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetActiveSoldier(Soldier soldier)
    {
        S = soldier;
        // Optional: fire event for observers
    }

    public void UnsetActiveSoldier()
    {
        S = null;
        // Optional: fire event for observers
    }
}
