using System;
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
        menu = FindFirstObjectByType<MainMenu>();
    }
    public ExplosionList Init(string explosionMessage)
    {
        transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text = explosionMessage;

        return this;
    }
    public void ConfirmExplosion(GameObject explosionList)
    {
        if (menu.OverrideKey())
        {
            int posDamage = 0, negDamage = 0, posStun = 0, negStun = 0;
            ScrollRect explosionScroller = explosionList.transform.Find("Scroll").GetComponent<ScrollRect>();
            Soldier explosionCausedBy = menu.soldierManager.FindSoldierById(transform.Find("ExplodedBy").GetComponent<TextMeshProUGUI>().text);
            print($"Explosion ({explosionList.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text}) ({explosionCausedBy.soldierName} {explosionCausedBy.soldierTeam}) Start - {posDamage + posStun}, {negDamage + negStun}");
            if (explosionScroller.verticalNormalizedPosition <= 0.05f)
            {
                Transform explosionAlerts = explosionList.transform.Find("Scroll").Find("View").Find("Content");

                foreach (Transform child in explosionAlerts)
                {
                    PhysicalObject hitByExplosion = child.GetComponent<ExplosiveAlert>().hitByExplosion;
                    Soldier explodedBy = child.GetComponent<ExplosiveAlert>().explodedBy;

                    if (hitByExplosion is Item item)
                    {
                        if (int.TryParse(child.Find("Damage").Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text, out int damage))
                        {
                            if (child.Find("IsAffected").GetComponent<Toggle>().isOn)
                            {
                                print($"{item.itemName} | {item.id} flagged for taking damage.");
                                if (item.IsGrenade() || item.IsClaymore())
                                {
                                    print($"{item.itemName} | {item.id} flagged for being triggered.");
                                    item.SetTriggered();
                                }
                                else
                                    item.DamageItem(explodedBy, damage);
                            }
                        }
                    }
                    else if (hitByExplosion is ExplosiveBarrel hitBarrel) //mark barrels for explosion
                    {
                        if (child.Find("IsAffected").GetComponent<Toggle>().isOn)
                            hitBarrel.triggered = true;
                    }
                    else if (hitByExplosion is Claymore hitClaymore) //mark claymore for explosion
                    {
                        if (child.Find("IsAffected").GetComponent<Toggle>().isOn)
                            hitClaymore.triggered = true;
                    }
                    else if (hitByExplosion is Terminal terminal)
                    {
                        if (child.Find("IsAffected").GetComponent<Toggle>().isOn)
                        {
                            //set terminal destroyer to 3 hp
                            if (explodedBy.hp > 3)
                                explodedBy.TakeDamage(explodedBy, explodedBy.hp - 3, true, new() { "Dipelec" });
                            menu.poiManager.DestroyPOI(terminal);
                        }
                    }
                    else if (hitByExplosion is DeploymentBeacon deploymentBeacon)
                    {
                        if (child.Find("IsAffected").GetComponent<Toggle>().isOn)
                            menu.poiManager.DestroyPOI(deploymentBeacon);
                    }
                    else if (hitByExplosion is Soldier hitSoldier)
                    {
                        if (int.TryParse(child.Find("Damage").Find("ExplosiveDamageIndicator").GetComponent<TextMeshProUGUI>().text, out int damage) && int.TryParse(child.Find("Stun").Find("StunDamageIndicator").GetComponent<TextMeshProUGUI>().text, out int stun))
                        {
                            if (child.Find("IsAffected").GetComponent<Toggle>().isOn)
                            {
                                if (transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text.Contains("Flashbang"))
                                {
                                    if (stun == 0)
                                        menu.AddXpAlert(hitSoldier, 2, $"Resisted stunning from flashbang.", true);
                                    else
                                        hitSoldier.SetStunned(stun); //non-resistable stunnage on flashbang grenades only   
                                }
                                else
                                    hitSoldier.TakeStun(stun);

                                hitSoldier.TakeDamage(explodedBy, damage, false, new() { "Explosive" });

                                //do xp calculations, enemy - friendly damage
                                if (hitSoldier.IsOppositeTeamAs(explodedBy))
                                    posDamage += damage;
                                else
                                    negDamage += damage;

                                //add xp for stunning Flashbang only
                                if (explosionList.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text.Contains("Flashbang"))
                                {
                                    //do xp calculations, enemy - friendly stunnage
                                    if (hitSoldier.IsOppositeTeamAs(explodedBy))
                                        posStun += stun;
                                    else
                                        negStun += stun;
                                }
                            }
                            print($"Explosion ({explosionList.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text}) ({explosionCausedBy.soldierName} {explosionCausedBy.soldierTeam}) ({explodedBy.soldierName} {explodedBy.soldierTeam}) Tested {hitSoldier.soldierName} {hitSoldier.soldierTeam} - {posDamage + posStun}, {negDamage + negStun}");
                        }
                    }
                }

                //actually explode triggered things
                foreach (Transform child in explosionAlerts)
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
                    else if (hitByExplosion is Item hitItem && hitItem.IsTriggered())
                    {
                        print($"{hitItem.id} is triggered.");
                        if (hitItem.IsGrenade())
                            hitItem.CheckExplosionGrenade(explodedBy, new(hitItem.X, hitItem.Y, hitItem.Z));
                        else if (hitItem.IsClaymore())
                        {
                            Instantiate(menu.poiManager.claymorePrefab).Init(Tuple.Create(new Vector3(hitItem.X, hitItem.Y, hitItem.Z), "Urban"), Tuple.Create(0, 0, hitItem.X, hitItem.Y, explodedBy.Id)).CheckExplosionClaymore(explodedBy, true);
                            hitItem.DestroyItem(explodedBy);
                        }
                    }
                }
                print($"Explosion ({explosionList.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text}) ({explosionCausedBy.soldierName} {explosionCausedBy.soldierTeam}) Resolved - {posDamage + posStun}, {negDamage + negStun}");

                //actually apply the xp
                if (posDamage + posStun > negDamage + negStun)
                {
                    if (explosionList.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text.Contains("Flashbang"))
                        menu.AddXpAlert(explosionCausedBy, posDamage - negDamage + posStun - negStun, $"Explosion ({explosionList.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text}) did {posDamage}/{posStun} damage/stun to enemies and {negDamage}/{negStun} damage/stun to allies.", false);
                    else
                        menu.AddXpAlert(explosionCausedBy, posDamage - negDamage, $"Explosion ({explosionList.transform.Find("Title").Find("Text").GetComponent<TextMeshProUGUI>().text}) did {posDamage} damage to enemies and {negDamage} damage to allies.", false);
                }

                menu.CloseExplosionUI();
                Destroy(explosionList);
            }
            else
                print("Haven't scrolled all the way to the bottom");
        }
    }
        
}
