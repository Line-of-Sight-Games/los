using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionList : MonoBehaviour
{
    public MainMenu menu;
    private void Awake()
    {
        menu = FindObjectOfType<MainMenu>();
    }
    public void ConfirmExplosion(GameObject explosionList)
    {
        ScrollRect explosionScroller = explosionList.transform.Find("Scroll").GetComponent<ScrollRect>();

        if (explosionScroller.verticalNormalizedPosition <= 0.05f)
        {
            Transform explosionAlerts = explosionList.transform.Find("Scroll").Find("View").Find("Content");

            foreach (Transform child in explosionAlerts)
            {
                PhysicalObject hitByExplosion = child.GetComponent<ExplosiveAlert>().hitByExplosion;
                Soldier explodedBy = child.GetComponent<ExplosiveAlert>().explodedBy;

                if (hitByExplosion is Soldier hitSoldier)
                {
                    if (int.TryParse(child.Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text, out int damage))
                    {
                        if (child.Find("DamageToggle").GetComponent<Toggle>().isOn)
                            hitSoldier.TakeDamage(explodedBy, damage, false, new() { "Explosive" });

                        if (child.Find("StunToggle").GetComponent<Toggle>().isOn)
                            hitSoldier.MakeStunned(1);
                    }

                    //destroy all items
                    if (damage > 5)
                        hitSoldier.DestroyAllBreakableItems();
                    else if (damage > 0)
                        hitSoldier.DestroyAllFragileItems();
                }
                else if (hitByExplosion is ExplosiveBarrel hitBarrel) //mark barrels for explosion }
                {
                    if (child.Find("DamageToggle").GetComponent<Toggle>().isOn)
                        hitBarrel.triggered = true;
                }
                else if (hitByExplosion is Terminal terminal)
                {
                    //set terminal destroyer to 3 hp
                    if (explodedBy.hp > 3)
                        explodedBy.TakeDamage(explodedBy, explodedBy.hp - 3, true, new List<string>() { "Dipelec" });
                    menu.poiManager.DestroyPOI(terminal);
                }
            }

            //actually explode the barrels
            foreach(Transform child in explosionAlerts)
            {
                PhysicalObject hitByExplosion = child.GetComponent<ExplosiveAlert>().hitByExplosion;
                Soldier explodedBy = child.GetComponent<ExplosiveAlert>().explodedBy;
                if (hitByExplosion is ExplosiveBarrel hitBarrel)
                    if (hitBarrel.triggered)
                        hitBarrel.CheckExplosion(explodedBy, Instantiate(menu.explosionListPrefab, menu.explosionUI.transform));
            }

            Destroy(explosionList);
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }
}
