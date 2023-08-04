using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class SoldierAlert : MonoBehaviour
{
    public AudioSource noisePlayerSoldierAlert;
    public AudioClip buttonPress;

    public Soldier soldier;
    public bool promotionComplete;
    public TMP_Dropdown statDropdown, abilityDropdown;
    public GameObject traumaButton;
    public Toggle captaincyToggle;

    private string[][] abilities =
    {
        new string[] { "Adept", "Aficionado" },
        new string[] { "Avenger", "Exactor" },
        new string[] { "Bloodletter", "Masochist" },
        new string[] { "Bull", "Colossus" },
        new string[] { "Calculator", "Supercomputer" },
        new string[] { "Daredevil", "Spider" },
        new string[] { "Dissuader", "Omen of Death" },
        new string[] { "Experimentalist", "Chemist" },
        new string[] { "Fighter", "Pugilist" },
        new string[] { "Guardsman", "Sentinel" },
        new string[] { "Gunner", "Cannoneer" },
        new string[] { "Illusionist", "Ghost" },
        new string[] { "Informer", "Double Agent" },
        new string[] { "Inspirer", "Galvaniser" },
        new string[] { "Insulator", "Absorber" },
        new string[] { "Jammer", "Corrupter" },
        new string[] { "Learner", "Mastermind" },
        new string[] { "Locater", "Logistician" },
        new string[] { "Patriot", "Zealot" },
        new string[] { "Planner", "Prophet" },
        new string[] { "Politician", "Master's Ally" },
        new string[] { "Revoker", "Pacifier" },
        new string[] { "Shadow", "Shapeshifter" },
        new string[] { "Sharpshooter", "Deadeye" },
        new string[] { "Spotter", "Tracker" },
        new string[] { "Sprinter", "Olympian" },
        new string[] { "Tactician", "Creator" },
        new string[] { "Tranquiliser", "Anaesthetist" },
        new string[] { "Vaulter", "Acrobat" },
        new string[] { "Witness", "Hypnotist" },
    };

    private void Start()
    {
        noisePlayerSoldierAlert = FindObjectOfType<AudioSource>();
    }

    public void PlayButtonPress()
    {
        //Debug.Log("played button press from soldier alert");
        noisePlayerSoldierAlert.PlayOneShot(buttonPress);
    }

    public void SetSoldier(Soldier initSoldier)
    {
        soldier = initSoldier;
    }

    public void PromoteSoldier()
    {
        string choiceStat = statDropdown.options[statDropdown.value].text;
        bool displayDropdown = false;

        if (choiceStat != "Select Option")
        {
            if (choiceStat != soldier.lastChosenStat)
            {
                if (choiceStat != soldier.soldierSpeciality)
                {
                    string[] stats = new string[3];

                    transform.Find("PromotionDescription").gameObject.SetActive(false);
                    transform.Find("StatDropTitle").gameObject.SetActive(false);
                    transform.Find("StatDropdown").gameObject.SetActive(false);
                    transform.Find("ConfirmButton").gameObject.SetActive(false);
                    transform.Find("LastStatAlert").gameObject.SetActive(false);
                    transform.Find("SpecialtyStatAlert").gameObject.SetActive(false);
                    transform.Find("LastStatAlert").gameObject.SetActive(false);
                    transform.Find("CaptaincyTitle").gameObject.SetActive(false);
                    transform.Find("CaptaincyToggle").gameObject.SetActive(false);

                    stats = soldier.Promote(choiceStat);

                    transform.Find("SpecialtyStat").gameObject.SetActive(true);
                    transform.Find("ChoiceStat").gameObject.SetActive(true);
                    transform.Find("RandomStat").gameObject.SetActive(true);
                    transform.Find("SpecialtyStat").GetComponent<TextMeshProUGUI>().text = stats[0];
                    transform.Find("ChoiceStat").GetComponent<TextMeshProUGUI>().text = stats[1];
                    transform.Find("RandomStat").GetComponent<TextMeshProUGUI>().text = stats[2];
                    soldier.game.soundManager.PlayPromotion();

                    if (soldier.NextRank() == "Major")
                    {
                        transform.Find("AbilityTitle").gameObject.SetActive(true);

                        if (!captaincyToggle.isOn)
                        {
                            PopulateAbilityOptions();
                            displayDropdown = true;
                        }
                        else
                        {
                            int chance = Random.Range(0, 4);

                            if (chance == 0)
                            {
                                transform.Find("AbilityTitle").GetComponent<TextMeshProUGUI>().text = "Failed to upgrade, no ability gained.";
                                promotionComplete = true;

                                soldier.game.soundManager.PlayFailedUpgrade();
                            }
                            else if (chance == 1)
                            {
                                string[][] localAbilities = abilities;

                                //remove soldier's current abilities from the list
                                for (int i = 0; i < soldier.soldierAbilities.Count; i++)
                                    localAbilities = localAbilities.Where(val => val[0] != localAbilities[i][0]).ToArray();

                                string ability = localAbilities[Random.Range(0, localAbilities.Length)][0];
                                transform.Find("AbilityTitle").GetComponent<TextMeshProUGUI>().text = "Granted random ability: " + ability;

                                //actually do the upgrade
                                soldier.soldierAbilities.Add(ability);
                                promotionComplete = true;

                                soldier.game.soundManager.PlayNewAbility();
                            }
                            else if (chance == 2)
                            {
                                PopulateAbilityOptions();
                                displayDropdown = true;
                            }
                            else if (chance == 3)
                            {
                                string[][] localAbilities = abilities;
                                localAbilities = localAbilities.Where(val => val[0] == soldier.soldierAbilities.First()).ToArray();
                                transform.Find("AbilityTitle").GetComponent<TextMeshProUGUI>().text = "Ability upgraded: " + localAbilities[0][1];

                                //actually do the upgrade
                                soldier.soldierAbilities.Remove(localAbilities[0][0]);
                                soldier.soldierAbilities.Add(localAbilities[0][1]);
                                promotionComplete = true;

                                soldier.game.soundManager.PlaySucceededUpgrade();
                            }
                        }

                        if (displayDropdown)
                        {
                            transform.Find("AbilityDropdown").gameObject.SetActive(true);
                            transform.Find("ConfirmButton2").gameObject.SetActive(true);
                        }    
                    }
                    else
                        promotionComplete = true;
                }
                else
                {
                    transform.Find("LastStatAlert").gameObject.SetActive(false);
                    transform.Find("SpecialtyStatAlert").gameObject.SetActive(true);
                }
            }
            else
            {
                transform.Find("SpecialtyStatAlert").gameObject.SetActive(false);
                transform.Find("LastStatAlert").gameObject.SetActive(true);
            }
        }
    }

    public void PromoteSoldierCaptain()
    {
        if (abilityDropdown.options[abilityDropdown.value].text != "Select Option")
        {
            soldier.soldierAbilities.Add(abilityDropdown.options[abilityDropdown.value].text);

            transform.Find("AbilityDropdown").gameObject.SetActive(false);
            transform.Find("ConfirmButton2").gameObject.SetActive(false);
            transform.Find("AbilityTitle").GetComponent<TextMeshProUGUI>().text = "Ability gained: " + abilityDropdown.options[abilityDropdown.value].text;
            promotionComplete = true;

            soldier.game.soundManager.PlayPromotion();
        }

    }

    public void PopulateAbilityOptions()
    {
        List<string> abilityOptions = new();
        string[][] localAbilities = abilities;

        //remove soldier's current abilities from the list
        for (int i = 0; i < soldier.soldierAbilities.Count; i++)
            localAbilities = localAbilities.Where(val => val[0] != localAbilities[i][0]).ToArray();

        //take a random list of 4 from the remaining list, removing each one in sequence
        for (int i = 0; i < 4; i++)
        {
            int index = Random.Range(0, localAbilities.Length - i);

            abilityOptions.Add(localAbilities[index][0]);
            localAbilities = localAbilities.Where(val => val[0] != localAbilities[index][0]).ToArray();
        }

        transform.Find("AbilityDropdown").GetComponent<TMP_Dropdown>().AddOptions(abilityOptions);
    }

    public void TraumatiseSoldier()
    {
        string distance;
        bool resisted = false;

        int.TryParse(transform.Find("TraumaIndicator").GetComponent<TextMeshProUGUI>().text, out int trauma);
        int.TryParse(transform.Find("Rolls").GetComponent<TextMeshProUGUI>().text, out int rolls);
        int.TryParse(transform.Find("XpOnResist").GetComponent<TextMeshProUGUI>().text, out int xpOnResist);
        distance = transform.Find("Distance").GetComponent<TextMeshProUGUI>().text;

        if (transform.Find("TraumaToggle").GetComponent<Toggle>().isOn)
        {
            if (transform.Find("TraumaToggle").GetComponent<Toggle>().IsInteractable())
            {
                //do a resilience check
                for (int i = 0; i < rolls; i++)
                {
                    if (soldier.ResilienceCheck())
                        resisted = true;
                }

                if (resisted)
                {
                    transform.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text = "<color=green>TRAUMA RESISTED</color>";
                    transform.Find("TraumaIndicator").GetComponent<TextMeshProUGUI>().text = "";
                    transform.Find("TraumaDescription").GetComponent<TextMeshProUGUI>().text = soldier.soldierName + " resisted the trauma." /*(" + rolls + " resilience check)."*/;
                    soldier.menu.AddXpAlert(soldier, xpOnResist, "Resisted traumatic event at " + distance + " range.", true);
                }
                else
                {
                    soldier.TakeTrauma(trauma);
                    transform.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text = "TRAUMA GAINED";
                    transform.Find("TraumaDescription").GetComponent<TextMeshProUGUI>().text = soldier.soldierName + " failed to resist the trauma and took " + trauma + " trauma. " + /*(" + rolls + " Resilience check).*/ "He is now " + soldier.GetTraumaState() + ".";
                }
            }
            else
            {
                soldier.TakeTrauma(trauma);
                transform.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text = "TRAUMA GAINED";
                transform.Find("TraumaDescription").GetComponent<TextMeshProUGUI>().text = soldier.soldierName + " took " + trauma + " trauma. " + /*(no Resilience check alllowed).*/ "He is now " + soldier.GetTraumaState() + ".";
            }
            
        }
        else
        {
            transform.Find("TraumaGainTitle").GetComponent<TextMeshProUGUI>().text = "<color=white>NO LOS</color>";
            transform.Find("TraumaIndicator").GetComponent<TextMeshProUGUI>().text = "";
            transform.Find("TraumaDescription").GetComponent<TextMeshProUGUI>().text = "No trauma points accrued.";
        }

        traumaButton.SetActive(false);
        transform.Find("TraumaToggle").gameObject.SetActive(false);
    }
}


