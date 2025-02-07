using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionAlert : SoldierAlert
{
    public bool promotionComplete;
    public Image arrow, postPromotionIcon;
    public TextMeshProUGUI statDropdownTitle;
    public TMP_Dropdown statDropdown, abilityDropdown;
    public Toggle captaincyToggle;

    
    public TextMeshProUGUI captaincyTitle;
    public GameObject promoteButton;
    public GameObject promoteButton2;
    public TextMeshProUGUI specialtyStatText;
    public TextMeshProUGUI choiceStatText;
    public TextMeshProUGUI randomStatText;
    public TextMeshProUGUI lastStatAlert;
    public TextMeshProUGUI specialtyStatAlert;
    public TextMeshProUGUI abilityTitle;

    private readonly string[][] abilities =
    {
        new string[] { "Adept", "Aficionado" },
        new string[] { "Avenger", "Exactor" },
        new string[] { "Bloodletter", "Masochist" },
        new string[] { "Bull", "Colossus" },
        new string[] { "Calculator", "Supercomputer" },
        new string[] { "Daredevil", "Spider" },
        new string[] { "Dissuader", "Omen of Death" },
        new string[] { "Experimentalist", "Chemist" },
        new string[] { "Brawler", "Pugilist" },
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
        soundManager = FindFirstObjectByType<SoundManager>();
    }

    public void PromoteSoldier()
    {
        string choiceStat = statDropdown.captionText.text;
        bool displayDropdown = false;

        if (choiceStat != "Select Option")
        {
            if (choiceStat != soldier.lastChosenStat)
            {
                if (choiceStat != soldier.soldierSpeciality)
                {
                    string[] stats = new string[3];

                    arrow.gameObject.SetActive(false);
                    postPromotionIcon.gameObject.SetActive(false);
                    title.gameObject.SetActive(false);
                    description.gameObject.SetActive(false);
                    statDropdownTitle.gameObject.SetActive(false);
                    statDropdown.gameObject.SetActive(false);
                    promoteButton.SetActive(false);
                    lastStatAlert.gameObject.SetActive(false);
                    specialtyStatAlert.gameObject.SetActive(false);
                    captaincyTitle.gameObject.SetActive(false);
                    captaincyToggle.gameObject.SetActive(false);

                    stats = soldier.Promote(choiceStat);
                    FileUtility.WriteToReport($"{soldier.soldierName} promotion: specialty ({stats[0]}), choice ({stats[1]}), random ({stats[2]})"); //write to report

                    specialtyStatText.text = stats[0];
                    specialtyStatText.gameObject.SetActive(true);
                    choiceStatText.text = stats[1];
                    choiceStatText.gameObject.SetActive(true);
                    randomStatText.text = stats[2];
                    randomStatText.gameObject.SetActive(true);

                    soldier.game.soundManager.PlayPromotion();

                    if (soldier.NextRank() == "Major")
                    {
                        abilityTitle.gameObject.SetActive(true);

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
                                abilityTitle.text = "Failed to upgrade, no ability gained.";
                                FileUtility.WriteToReport($"{soldier.soldierName} failed ability upgrade, no ability granted"); //write to report

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
                                abilityTitle.text = "Granted random ability: " + ability;
                                FileUtility.WriteToReport($"{soldier.soldierName} granted random ability: {ability}"); //write to report

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
                                abilityTitle.text = "Ability upgraded: " + localAbilities[0][1];
                                FileUtility.WriteToReport($"{soldier.soldierName} upgraded ability: {localAbilities[0][1]}"); //write to report

                                //actually do the upgrade
                                soldier.soldierAbilities.Remove(localAbilities[0][0]);
                                soldier.soldierAbilities.Add(localAbilities[0][1]);
                                promotionComplete = true;

                                soldier.game.soundManager.PlaySucceededUpgrade();
                            }
                        }

                        if (displayDropdown)
                        {
                            abilityDropdown.gameObject.SetActive(true);
                            promoteButton2.SetActive(true);
                        }
                    }
                    else
                        promotionComplete = true;
                }
                else
                {
                    lastStatAlert.gameObject.SetActive(false);
                    specialtyStatAlert.gameObject.SetActive(true);
                }
            }
            else
            {
                lastStatAlert.gameObject.SetActive(true);
                specialtyStatAlert.gameObject.SetActive(false);
            }
        }
    }

    public void PromoteSoldierCaptain()
    {
        if (abilityDropdown.captionText.text != "Select Option")
        {
            soldier.soldierAbilities.Add(abilityDropdown.captionText.text);

            abilityDropdown.gameObject.SetActive(false);
            promoteButton2.SetActive(false);
            abilityTitle.text = "Ability gained: " + abilityDropdown.captionText.text;

            FileUtility.WriteToReport($"{soldier.soldierName} granted chosen ability: {abilityDropdown.captionText.text}"); //write to report
            promotionComplete = true;

            //soldier.game.soundManager.PlayPromotion();
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

        abilityDropdown.GetComponent<TMP_Dropdown>().AddOptions(abilityOptions);
    }
}
