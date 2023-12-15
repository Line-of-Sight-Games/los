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
    public ExplosionList Init(string explosionMessage)
    {
        transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text = explosionMessage;

        return this;
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

                if (hitByExplosion is Item item)
                {
                    if (int.TryParse(child.Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text, out int damage))
                    {
                        if (child.Find("DamageToggle").GetComponent<Toggle>().isOn)
                        {
                            if (item.IsGrenade())
                                item.traits.Add("Triggered");
                            else
                                item.DamageItem(explodedBy, damage);
                        }
                    }
                }
                else if (hitByExplosion is ExplosiveBarrel hitBarrel) //mark barrels for explosion
                {
                    if (child.Find("DamageToggle").GetComponent<Toggle>().isOn)
                        hitBarrel.triggered = true;
                }
                else if (hitByExplosion is Claymore hitClaymore) //mark claymore for explosion
                {
                    if (child.Find("DamageToggle").GetComponent<Toggle>().isOn)
                        hitClaymore.triggered = true;
                }
                else if (hitByExplosion is Terminal terminal)
                {
                    //set terminal destroyer to 3 hp
                    if (explodedBy.hp > 3)
                        explodedBy.TakeDamage(explodedBy, explodedBy.hp - 3, true, new() { "Dipelec" });
                    menu.poiManager.DestroyPOI(terminal);
                }
                else if (hitByExplosion is Soldier hitSoldier)
                {
                    if (int.TryParse(child.Find("Damage").Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text, out int damage))
                    {
                        if (child.Find("Damage").Find("DamageToggle").GetComponent<Toggle>().isOn)
                        {
                            if (transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text.Contains("Claymore")) //if it's a claymore
                            {
                                if (hitSoldier.IsUnconscious() || hitSoldier.IsLastStand() || hitSoldier.IsWearingExoArmour() || hitSoldier.IsWearingGhillieArmour() || hitSoldier.IsWearingStimulantArmour())
                                {
                                    damage = 0;
                                    hitSoldier.InstantKill(explodedBy, new() { "Explosive" });
                                }
                                else if (hitSoldier.IsWearingBodyArmour(false))
                                    damage = hitSoldier.GetFullHP() - 1;
                            }
                            hitSoldier.TakeDamage(explodedBy, damage, false, new() { "Explosive" });
                        }
                    }

                    if (int.TryParse(child.Find("Stun").Find("StunDamageIndicator").GetComponent<TextMeshProUGUI>().text, out int stun))
                    {
                        if (child.Find("Stun").Find("StunToggle").GetComponent<Toggle>().isOn)
                        {
                            if (transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text.Contains("Flashbang"))
                                hitSoldier.SetStunned(stun); //non-resistable stunnage on flashbang grenades only
                            else
                                hitSoldier.MakeStunned(stun);
                        }
                    }
                }
            }

            //actually explode the barrels
            foreach(Transform child in explosionAlerts)
            {
                PhysicalObject hitByExplosion = child.GetComponent<ExplosiveAlert>().hitByExplosion;
                Soldier explodedBy = child.GetComponent<ExplosiveAlert>().explodedBy;
                if (hitByExplosion is ExplosiveBarrel hitBarrel)
                {
                    if (hitBarrel.triggered)
                        hitBarrel.CheckExplosionBarrel(explodedBy);
                }
                else if (hitByExplosion is Claymore hitClaymore)
                {
                    if (hitClaymore.triggered)
                        hitClaymore.CheckExplosionClaymore(explodedBy, true);
                }
                else if (hitByExplosion is Item hitItem)
                {
                    if (hitItem.IsGrenade() && hitItem.IsTriggered())
                        hitItem.CheckExplosionGrenade(explodedBy, new(hitItem.X, hitItem.Y, hitItem.Z));
                } 
            }

            Destroy(explosionList);
        }
        else
            print("Haven't scrolled all the way to the bottom");
    }
}
