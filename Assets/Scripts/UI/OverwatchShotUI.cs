using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class OverwatchShotUI : MonoBehaviour
{
    public MainGame game;
    public MainMenu menu;

    public TextMeshProUGUI shooterID;
    public SoldierPortrait shooterPortrait;
    public TMP_Dropdown shotTypeDropdown;
    public Image gunImage;
    public TMP_Dropdown aimDropdown;
    public TMP_Dropdown targetDropdown;
    public TMP_InputField xPos;
    public TMP_InputField yPos;
    public TMP_InputField zPos;
    public TMP_Dropdown terrainDropdown;

    private void Awake()
    {
        game = FindFirstObjectByType<MainGame>();
        menu = FindFirstObjectByType<MainMenu>();
    }
    public OverwatchShotUI Init(Soldier shooter, Soldier target)
    {
        //set shooter
        shooterID.text = shooter.Id;

        shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        List<TMP_Dropdown.OptionData> targetDetails = new();

        //display shooter
        if (shooter.IsBeingRevealedBy(target))
        {
            shooterPortrait.Init(shooter);
            shooterPortrait.gameObject.SetActive(true);
        }
        else
            shooterPortrait.gameObject.SetActive(false);

        //set as regular shot
        shotTypeDropdown.value = 0;
        shotTypeDropdown.interactable = false;

        //set as aimed shot
        aimDropdown.value = 0;
        aimDropdown.interactable = false;

        //generate gun image
        gunImage.sprite = shooter.EquippedGuns.First().itemImage;

        //set target
        TMP_Dropdown.OptionData option = new(target.soldierName, target.soldierPortrait, Color.white);
        targetDetails.Add(option);
        targetDropdown.AddOptions(targetDetails);
        targetDropdown.interactable = false;

        //prefill with target's current position
        xPos.placeholder.GetComponent<TextMeshProUGUI>().text = target.X.ToString();
        yPos.placeholder.GetComponent<TextMeshProUGUI>().text = target.Y.ToString();
        zPos.placeholder.GetComponent<TextMeshProUGUI>().text = target.Z.ToString();

        return this;
    }
    public void ConfirmShotOverwatch(bool retry)
    {
        Soldier shooter = game.soldierManager.FindSoldierById(transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text);
        IAmShootable target = game.soldierManager.FindSoldierByName(transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().captionText.text);
        Item gun = shooter.EquippedGuns.First();
        int actingHitChance;
        menu.SetShotResolvedFlagTo(false);

        //apply overwatch interrupt if resilience check failed
        if (menu.ValidateIntInput(xPos, out int xOver) && menu.ValidateIntInput(yPos, out int yOver) && menu.ValidateIntInput(zPos, out int zOver) && terrainDropdown.value != 0)
        {
            if (target is Soldier targetSoldier)
            {
                //set target position temporarily to point of shot
                Tuple<int, int, int, string> finalLocation = Tuple.Create(targetSoldier.X, targetSoldier.Y, targetSoldier.Z, targetSoldier.TerrainOn);
                targetSoldier.X = xOver;
                targetSoldier.Y = yOver;
                targetSoldier.Z = zOver;
                targetSoldier.terrainOn = terrainDropdown.captionText.text;

                game.tempShooterTarget = Tuple.Create(shooter, target);
                int randNum1 = HelperFunctions.RandomNumber(1, 100);
                int randNum2 = HelperFunctions.RandomNumber(1, 100);
                Tuple<int, int, int> chances = game.CalculateHitPercentage(shooter, target, gun);
                gun.SpendSingleAmmo();

                //display suppression indicator
                if (shooter.IsSuppressed())
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(true);

                    if (shooter.ResilienceCheck())
                    {
                        menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Resisted Suppression</color>";
                        actingHitChance = chances.Item1;
                    }
                    else
                    {
                        menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=orange>Suffered Suppression</color>";
                        actingHitChance = chances.Item3;
                    }
                }
                else
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(false);
                    actingHitChance = chances.Item1;
                }

                //standard shot hits
                if (randNum1 <= actingHitChance)
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                    //standard shot crit hits
                    if (randNum2 <= chances.Item2)
                    {
                        targetSoldier.TakeDamage(shooter, gun.gunTraits["CritDamage"], false, new() { "Critical", "Shot" });
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> CRITICAL SHOT </color>";

                        //paying xp for hit
                        if (chances.Item1 >= 10)
                            menu.AddXpAlert(shooter, 8, "Critical overwatch shot on " + targetSoldier.soldierName + "!", false);
                        else
                            menu.AddXpAlert(shooter, 10, "Critical overwatch shot with a " + chances.Item1 + "% chance on " + targetSoldier.soldierName + "!", false);
                    }
                    else
                    {
                        targetSoldier.TakeDamage(shooter, gun.gunTraits["Damage"], false, new() { "Shot" });
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Hit </color>";

                        //paying xp for hit
                        if (chances.Item1 >= 10)
                            menu.AddXpAlert(shooter, 2, "Overwatch shot hit on " + targetSoldier.soldierName + ".", false);
                        else
                            menu.AddXpAlert(shooter, 10, "Overwatch shot hit with a " + chances.Item1 + "% chance on " + targetSoldier.soldierName + "!", false);
                    }

                    //apply overwatch freeze on spot unless resisted
                    if (targetSoldier.ResilienceCheck())
                    {
                        menu.AddXpAlert(targetSoldier, 2, $"Resisted overwatch daze.", false);
                        menu.AddDamageAlert(targetSoldier, $"{targetSoldier.soldierName} resisted overwatch daze.", true, true);
                        
                        //restore actual position of move
                        targetSoldier.X = finalLocation.Item1;
                        targetSoldier.Y = finalLocation.Item2;
                        targetSoldier.Z = finalLocation.Item3;
                        targetSoldier.terrainOn = finalLocation.Item4;
                    }
                    else if (targetSoldier.IsGuardsman())
                    {
                        menu.AddDamageAlert(targetSoldier, $"{targetSoldier.soldierName} resisted overwatch daze (<color=green>Guardsman</color>).", true, true);

                        //restore actual position of move
                        targetSoldier.X = finalLocation.Item1;
                        targetSoldier.Y = finalLocation.Item2;
                        targetSoldier.Z = finalLocation.Item3;
                        targetSoldier.terrainOn = finalLocation.Item4;
                    }
                    else
                    {
                        //overwatch daze ap
                        targetSoldier.ap = 0;
                        menu.AddDamageAlert(targetSoldier, $"{targetSoldier.soldierName} suffered overwatch daze at X: {targetSoldier.X}, Y: {targetSoldier.Y}, Z: {targetSoldier.Z}, Ter: {targetSoldier.terrainOn}.", false, true);
                    }

                    //don't show los check button if shot hits
                    menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(false);
                }
                else
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {game.RandomShotScatterDistance()}cm {game.RandomShotScatterHorizontal()}, {game.RandomShotScatterDistance()}cm {game.RandomShotScatterVertical()}.\n\nDamage event ({gun.gunTraits["Damage"]}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";

                    //show los check button if shot doesn't hit
                    menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                    //show guardsman retry if gun has ammo
                    if (shooter.IsGuardsman() && shooter.HasAnyAmmo() && !retry && !targetSoldier.IsRevoker())
                        menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(true);
                    else
                        menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);

                    //paying xp for dodge
                    if (chances.Item1 <= 90)
                        menu.AddXpAlert(targetSoldier, 1, "Dodged overwatch shot from " + shooter.soldierName + ".", false);
                    else
                        menu.AddXpAlert(targetSoldier, 10, "Dodged overwatch shot with a " + chances.Item1 + "% chance from " + shooter.soldierName + "!", false);

                    //restore actual position of move
                    targetSoldier.X = finalLocation.Item1;
                    targetSoldier.Y = finalLocation.Item2;
                    targetSoldier.Z = finalLocation.Item3;
                    targetSoldier.terrainOn = finalLocation.Item4;

                    //push the no damage attack through for abilities trigger
                    targetSoldier.TakeDamage(shooter, 0, true, new() { "Shot" });
                }

                //unset overwatch after any shot
                shooter.DecrementOverwatch();

                //trigger loud action
                shooter.PerformLoudAction();

                menu.OpenShotResultUI(false);

                //refresh detections (potentially trigger more overwatch)
                targetSoldier.SetLosCheck("losChange|postOverwatch"); //losCheck

                CloseOverwatchShotUI();
            }
        }
    }
    public void CloseOverwatchShotUI()
    {
        gameObject.SetActive(false);
    }
    public void OpenShotOverwatchConfirmUI()
    {
        if (menu.ValidateIntInput(xPos, out int xOver) && menu.ValidateIntInput(yPos, out int yOver) && menu.ValidateIntInput(zPos, out int zOver) && terrainDropdown.value != 0)
        {
            //find shooter
            GameObject shotConfirmUI = transform.Find("ConfirmShotUI").gameObject;
            Soldier shooter = game.soldierManager.FindSoldierById(transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text);
            Soldier target = game.soldierManager.FindSoldierByName(transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().captionText.text);
            Item gun = shooter.EquippedGuns.First();
            Tuple<int, int, int> chances = game.CalculateHitPercentage(shooter, target, gun);

            //only shot suppression hit chance if suppressed
            if (shooter.IsSuppressed())
                shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(true);
            else
                shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(false);

            shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").Find("SuppressedHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item3.ToString() + "%";
            shotConfirmUI.transform.Find("OptionPanel").Find("HitChance").Find("HitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item1.ToString() + "%";
            shotConfirmUI.transform.Find("OptionPanel").Find("CritHitChance").Find("CritHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item2.ToString() + "%";

            //add parameter to equation view
            shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = menu.DisplayShotParameters();

            shotConfirmUI.SetActive(true);
        }
    }
}
