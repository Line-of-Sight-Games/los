using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverwatchShotUI : MonoBehaviour
{
    public ShotUI shotUI;

    public TextMeshProUGUI shooterID, targetID;
    public SoldierPortrait shooterPortrait;
    public TMP_Dropdown shotTypeDropdown;
    public Image gunImage;
    public TMP_Dropdown aimDropdown;
    public TMP_Dropdown targetDropdown;
    public TMP_InputField xPos;
    public TMP_InputField yPos;
    public TMP_InputField zPos;
    public TMP_Dropdown terrainDropdown;
    public GameObject shotConfirmUI;
    public GameObject outOfRange;

    Tuple<int, int, int, string> intendedLocation;
    Tuple<int, int, int, string> locationAtShot;

    public OverwatchShotUI Init(Soldier shooter, Soldier target)
    {
        shotUI.SetShotResolvedFlagTo(false);

        //set shooter and target
        shooterID.text = shooter.Id;
        targetID.text = target.Id;

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
        TMP_Dropdown.OptionData option = new(target.Id, target.soldierPortrait, Color.white);
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
        Soldier shooter = SoldierManager.Instance.FindSoldierById(shooterID.text);
        IAmShootable target = SoldierManager.Instance.FindSoldierById(targetID.text);
        Item gun = shooter.EquippedGuns.First();
        int actingHitChance;

        //apply overwatch interrupt if resilience check failed
        if (target is Soldier targetSoldier)
        {
            //reset parameters for guardsman overwatch retry
            if (retry)
            {
                FileUtility.WriteToReport($"{shooter.soldierName} attempts Guardsman overwatch retry."); //write to report
                shooter.SetState("Overwatch"); //refresh overwatch state for guardsman

                //set target position back to shot position
                targetSoldier.X = locationAtShot.Item1;
                targetSoldier.Y = locationAtShot.Item2;
                targetSoldier.Z = locationAtShot.Item3;
                targetSoldier.TerrainOn = locationAtShot.Item4;
                shotUI.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);
            }

            FileUtility.WriteToReport($"{shooter.soldierName} overwatch shoots at {targetSoldier.soldierName}"); //write to report

            //play shot sfx
            SoundManager.Instance.PlayShotResolution(gun);

            shotUI.tempShooterTarget = Tuple.Create(shooter, target);
            int randNum1 = HelperFunctions.RandomShotNumber();
            int randNum2 = HelperFunctions.RandomCritNumber();
            Tuple<int, int, int> chances = shotUI.CalculateHitPercentage(shooter, target, gun);
            gun.SpendSingleAmmo();
            //print($"shotparams: {MenuManager.Instance.DisplayShotParameters()}\n\n{chances.Item1}|{chances.Item2}>>>{randNum1}|{randNum2}");

            //display suppression indicator
            if (shooter.IsSuppressed())
            {
                shotUI.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(true);

                if (shooter.ResilienceCheck())
                {
                    FileUtility.WriteToReport($"{shooter.soldierName} resists suppression."); //write to report

                    shotUI.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Resisted Suppression</color>";
                    actingHitChance = chances.Item1;
                }
                else
                {
                    FileUtility.WriteToReport($"{shooter.soldierName} suffers suppression."); //write to report

                    shotUI.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=orange>Suffered Suppression</color>";
                    actingHitChance = chances.Item3;
                }
            }
            else
            {
                shotUI.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(false);
                actingHitChance = chances.Item1;
            }

            //standard shot hits
            if (randNum1 <= actingHitChance)
            {
                FileUtility.WriteToReport($"{shooter.soldierName} hits {targetSoldier.soldierName} ({actingHitChance}%|{chances.Item2}%)."); //write to report

                shotUI.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                //standard shot crit hits
                if (randNum2 <= chances.Item2)
                {
                    FileUtility.WriteToReport($"The shot is critical!"); //write to report

                    targetSoldier.TakeDamage(shooter, gun.critDamage, false, new() { "Critical", "Overwatch", "Shot" }, Vector3.zero);
                    shotUI.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> CRITICAL SHOT </color>";

                    //paying xp for hit
                    if (actingHitChance < 10)
                        MenuManager.Instance.AddXpAlert(shooter, 10, $"Critical overwatch shot <10% hit on {targetSoldier.soldierName}!", false);
                    else
                        MenuManager.Instance.AddXpAlert(shooter, 8, $"Critical overwatch shot hit on {targetSoldier.soldierName}!", false);
                }
                else
                {
                    targetSoldier.TakeDamage(shooter, gun.damage, false, new() { "Overwatch", "Shot" }, Vector3.zero);
                    shotUI.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Hit </color>";

                    //paying xp for hit
                    if (actingHitChance < 10)
                        MenuManager.Instance.AddXpAlert(shooter, 10, $"Overwatch shot <10% hit on {targetSoldier.soldierName}!", false);
                    else
                        MenuManager.Instance.AddXpAlert(shooter, 2, $"Overwatch shot hit on {targetSoldier.soldierName}.", false);
                }

                //check for overwatch daze
                if (targetSoldier.IsAbleToWalk())
                {
                    if (!targetSoldier.ResilienceCheck())
                    {
                        MenuManager.Instance.AddXpAlert(targetSoldier, 2, $"Resisted overwatch daze.", false);
                        MenuManager.Instance.AddSoldierAlert(targetSoldier, "DAZE RESISTED", Color.green, $"Resists overwatch daze.", -1, -1);

                        //restore actual position of move
                        targetSoldier.X = intendedLocation.Item1;
                        targetSoldier.Y = intendedLocation.Item2;
                        targetSoldier.Z = intendedLocation.Item3;
                        targetSoldier.TerrainOn = intendedLocation.Item4;
                    }
                    else if (targetSoldier.IsGuardsman())
                    {
                        MenuManager.Instance.AddSoldierAlert(targetSoldier, "DAZE RESISTED", Color.green, $"Resists overwatch daze with <color=green>Guardsman</color> ability.", -1, -1);

                        //restore actual position of move
                        targetSoldier.X = intendedLocation.Item1;
                        targetSoldier.Y = intendedLocation.Item2;
                        targetSoldier.Z = intendedLocation.Item3;
                        targetSoldier.TerrainOn = intendedLocation.Item4;
                    }
                    else //apply overwatch daze
                    {
                        targetSoldier.ap = 0;
                        MenuManager.Instance.AddSoldierAlert(targetSoldier, "DAZED", Color.red, $"Suffers overwatch daze at X: {targetSoldier.X}, Y: {targetSoldier.Y}, Z: {targetSoldier.Z}, Ter: {targetSoldier.TerrainOn}.", -1, -1);
                    }
                }

                //don't show los check button if shot hits
                shotUI.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(false);
            }
            else
            {
                string missString = GameManager.Instance.RandomShotMissString();

                FileUtility.WriteToReport($"{shooter.soldierName} misses {targetSoldier.soldierName} ({actingHitChance}%|{chances.Item2}%), shot goes {missString}"); //write to report

                shotUI.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                shotUI.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {missString}.\n\nDamage event ({gun.damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";

                //show los check button if shot doesn't hit
                shotUI.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                //show guardsman retry if gun has ammo
                if (shooter.IsGuardsman() && shooter.guardsmanRetryUsed == false && shooter.HasAnyAmmo() && !targetSoldier.IsRevoker())
                {
                    shooter.guardsmanRetryUsed = true;
                    shotUI.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(true);
                }
                else
                    shotUI.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);

                //paying xp for dodge
                if (actingHitChance > 90)
                    MenuManager.Instance.AddXpAlert(targetSoldier, 10, $"Overwatch shot >90% dodged from {shooter.soldierName}!", false);
                else
                    MenuManager.Instance.AddXpAlert(targetSoldier, 1, $"Overwatch shot dodged from {shooter.soldierName}.", false);

                //restore actual position of move
                targetSoldier.X = intendedLocation.Item1;
                targetSoldier.Y = intendedLocation.Item2;
                targetSoldier.Z = intendedLocation.Item3;
                targetSoldier.TerrainOn = intendedLocation.Item4;

                //push a zero damage attack through for abilities trigger
                targetSoldier.TakeDamage(shooter, 0, true, new() { "Shot" }, Vector3.zero);
            }

            //unset overwatch after any shot
            shooter.UnsetOverwatch();

            //trigger loud action
            if (gun.HasSuppressorAttached()) //suppressor
                shooter.PerformLoudAction(20);
            else
                shooter.PerformLoudAction();

            MenuManager.Instance.StartCoroutine(shotUI.OpenShotResultUI(false));

            //refresh detections (potentially trigger more overwatch)
            targetSoldier.SetLosCheck("losChange|postOverwatch"); //losCheck

            CloseOverwatchShotUI();
        }
    }
    public void CloseOverwatchShotUI()
    {
        ClearOverwatchShotUI();
        gameObject.SetActive(false);
    }
    public void ClearOverwatchShotUI()
    {
        targetDropdown.ClearOptions(); 
        xPos.text = "";
        yPos.text = "";
        zPos.text = "";
        terrainDropdown.value = 0;
        outOfRange.SetActive(false);
        shotConfirmUI.SetActive(false);
    }
    public void OpenOverwatchShotConfirmUI()
    {
        if (HelperFunctions.ValidateIntInput(xPos, out int xOver) && HelperFunctions.ValidateIntInput(yPos, out int yOver) && HelperFunctions.ValidateIntInput(zPos, out int zOver) && terrainDropdown.value != 0)
        {
            //find shooter
            Soldier shooter = SoldierManager.Instance.FindSoldierById(shooterID.text);
            Soldier target = SoldierManager.Instance.FindSoldierById(targetID.text);
            Item gun = shooter.EquippedGuns.First();

            //save target intended location
            intendedLocation = Tuple.Create(target.X, target.Y, target.Z, target.TerrainOn);
            //set target position temporarily to point of shot
            locationAtShot = Tuple.Create(xOver, yOver, zOver, terrainDropdown.captionText.text);

            if (shooter.PhysicalObjectWithinOverwatchCone(new Vector3(locationAtShot.Item1, locationAtShot.Item2, locationAtShot.Item3)))
            {
                target.X = xOver;
                target.Y = yOver;
                target.Z = zOver;
                target.TerrainOn = terrainDropdown.captionText.text;

                //calculate hit percentage
                Tuple<int, int, int> chances = shotUI.CalculateHitPercentage(shooter, target, gun);

                //only shot suppression hit chance if suppressed
                if (shooter.IsSuppressed())
                    shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(true);
                else
                    shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").gameObject.SetActive(false);

                shotConfirmUI.transform.Find("OptionPanel").Find("SuppressedHitChance").Find("SuppressedHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item3.ToString() + "%";
                shotConfirmUI.transform.Find("OptionPanel").Find("HitChance").Find("HitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item1.ToString() + "%";
                shotConfirmUI.transform.Find("OptionPanel").Find("CritHitChance").Find("CritHitChanceDisplay").GetComponent<TextMeshProUGUI>().text = chances.Item2.ToString() + "%";

                //add parameter to equation view
                shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = shotUI.DisplayShotParameters();

                shotConfirmUI.SetActive(true);
            }
            else
                outOfRange.SetActive(true);
        }
    }
}
