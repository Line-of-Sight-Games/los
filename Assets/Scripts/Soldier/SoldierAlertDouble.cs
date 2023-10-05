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

    private void Update()
    {
        // Check if the cursor is hovering over the GameObject with SoldierAlertDouble script
        if (IsCursorOverSoldierAlertDouble())
        {
            print($"hovering over LOS between: {s1.soldierName} and {s2.soldierName}");
            // Your additional logic here
        }
    }

    private bool IsCursorOverSoldierAlertDouble()
    {
        // Raycast from the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Adjust the layer mask according to your game setup
        int layerMask = LayerMask.GetMask("UI");

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Check if the hit object has the SoldierAlertDouble script
            SoldierAlertDouble soldierAlertDouble = hit.transform.GetComponent<SoldierAlertDouble>();
            return soldierAlertDouble != null;
        }

        return false;
    }
}
