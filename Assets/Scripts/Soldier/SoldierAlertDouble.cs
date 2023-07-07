using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierAlertDouble : MonoBehaviour
{
    public Soldier s1, s2;
    public void SetSoldiers(Soldier initS1, Soldier initS2)
    {
        s1 = initS1;
        s2 = initS2;
    }
}
