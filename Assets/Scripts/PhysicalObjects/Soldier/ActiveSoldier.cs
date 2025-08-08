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

        S.selected = true;
    }

    public void UnsetActiveSoldier()
    {
        S.selected = false;

        S = null;
    }
}
