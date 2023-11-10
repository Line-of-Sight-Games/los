using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverwatchShotUI : MonoBehaviour
{
    public MainGame game;
    public MainMenu menu;

    private void Awake()
    {
        game = FindObjectOfType<MainGame>();
        menu = FindObjectOfType<MainMenu>();
    }

    public void ConfirmShotOverwatch()
    {
        Soldier shooter = game.soldierManager.FindSoldierById(transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text);
        IAmShootable target = game.soldierManager.FindSoldierByName(transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().options[transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().value].text);
        Item gun = shooter.EquippedGun;
        int actingHitChance;
        menu.SetShotResolvedFlagTo(false);

        //apply overwatch interrupt if resilience check failed
        Transform overwatchLocation = transform.Find("TargetPanel").Find("OverwatchLocation");
        if (int.TryParse(overwatchLocation.Find("XPos").GetComponent<TMP_InputField>().text, out int xOver) && int.TryParse(overwatchLocation.Find("YPos").GetComponent<TMP_InputField>().text, out int yOver) && int.TryParse(overwatchLocation.Find("ZPos").GetComponent<TMP_InputField>().text, out int zOver) && overwatchLocation.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value != 0)
        {
            game.tempShooterTarget = Tuple.Create(shooter, target);
            gun.SpendSingleAmmo();
            int randNum1 = game.RandomNumber(0, 100);
            int randNum2 = game.RandomNumber(0, 100);
            Tuple<int, int, int> chances = game.CalculateHitPercentage(shooter, target, gun);

            if (target is Soldier targetSoldier)
            {
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
                        targetSoldier.TakeDamage(shooter, gun.gunTraits.CritDamage, false, new List<string>() { "Critical", "Shot" });
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> CRITICAL SHOT </color>";

                        //paying xp for hit
                        if (chances.Item1 >= 10)
                            menu.AddXpAlert(shooter, 8, "Critical overwatch shot on " + targetSoldier.soldierName + "!", false);
                        else
                            menu.AddXpAlert(shooter, 10, "Critical overwatch shot with a " + chances.Item1 + "% chance on " + targetSoldier.soldierName + "!", false);
                    }
                    else
                    {
                        targetSoldier.TakeDamage(shooter, gun.gunTraits.Damage, false, new List<string>() { "Shot" });
                        menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Hit </color>";

                        //paying xp for hit
                        if (chances.Item1 >= 10)
                            menu.AddXpAlert(shooter, 2, "Overwatch shot hit on " + targetSoldier.soldierName + ".", false);
                        else
                            menu.AddXpAlert(shooter, 10, "Overwatch shot hit with a " + chances.Item1 + "% chance on " + targetSoldier.soldierName + "!", false);
                    }

                    //apply overwatch freeze on spot unless resisted
                    if (targetSoldier.ResilienceCheck() || targetSoldier.IsGuardsman())
                        menu.AddDamageAlert(targetSoldier, targetSoldier.soldierName + " resisted overwatch daze.", true, true);
                    else
                    {
                        targetSoldier.X = xOver;
                        targetSoldier.Y = yOver;
                        targetSoldier.Z = zOver;
                        targetSoldier.terrainOn = overwatchLocation.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().options[overwatchLocation.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value].text;
                        targetSoldier.ap = 0;
                        menu.AddDamageAlert(targetSoldier, $"{targetSoldier.soldierName} suffered overwatch daze at X: {targetSoldier.X}, Y: {targetSoldier.Y}, Z: {targetSoldier.Z}, Ter: {targetSoldier.terrainOn}.", false, true);
                    }


                    //don't show los check button if shot hits
                    menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(false);
                }
                else
                {
                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {game.RandomShotScatterDistance()}cm {game.RandomShotScatterHorizontal()}, {game.RandomShotScatterDistance()}cm {game.RandomShotScatterVertical()}.\n\nDamage event ({gun.gunTraits.Damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                    //show los check button if shot doesn't hit
                    menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                    /*//show guardsman retry if gun has ammo
                    if (shooter.IsGuardsman() && shooter.EquippedGun.CheckAnyAmmo())
                        menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(true);
                    else
                        menu.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);*/

                    //paying xp for dodge
                    if (chances.Item1 <= 90)
                        menu.AddXpAlert(targetSoldier, 1, "Dodged overwatch shot from " + shooter.soldierName + ".", false);
                    else
                        menu.AddXpAlert(targetSoldier, 10, "Dodged overwatch shot with a " + chances.Item1 + "% chance from " + shooter.soldierName + "!", false);

                    //push the no damage attack through for abilities trigger
                    targetSoldier.TakeDamage(shooter, 0, true, new List<string>() { "Shot" });
                }

                //unset overwatch after any shot
                shooter.DecrementOverwatch();

                //trigger loud action
                shooter.PerformLoudAction();

                menu.OpenShotResultUI();

                //refresh detections (potentially trigger more overwatch)
                game.StartCoroutine(game.DetectionAlertSingle(targetSoldier, "losChange", Vector3.zero, string.Empty, true));

                //destroy other overwatch instances
                foreach (Transform child in menu.overwatchShotUI.transform)
                    Destroy(child.gameObject);
            }
        }
    }

    public void OpenShotOverwatchConfirmUI()
    {
        Transform overwatchLocation = transform.Find("TargetPanel").Find("OverwatchLocation");
        if (int.TryParse(overwatchLocation.Find("XPos").GetComponent<TMP_InputField>().text, out int xOver) && int.TryParse(overwatchLocation.Find("YPos").GetComponent<TMP_InputField>().text, out int yOver) && int.TryParse(overwatchLocation.Find("ZPos").GetComponent<TMP_InputField>().text, out int zOver) && overwatchLocation.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value != 0)
        {
            //find shooter
            GameObject shotConfirmUI = transform.Find("ConfirmShotUI").gameObject;
            Soldier shooter = game.soldierManager.FindSoldierById(transform.Find("Shooter").GetComponent<TextMeshProUGUI>().text);
            Soldier target = game.soldierManager.FindSoldierByName(transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().options[transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().value].text);
            Item gun = shooter.EquippedGun;
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
            shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text =
                $"{game.shotParameters.Find(tuple => tuple.Item1 == "accuracy")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "sharpshooter")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "inspired")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "WS")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "juggernaut")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "stim")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "trauma")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "sustenance")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "tE")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "cover")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "vis")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "rain")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "wind")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "HP")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "tHP")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "Ter")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "tTer")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "elevation")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "kd")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "overwatch")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "flank")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "stealth")} " +
                $"| {game.shotParameters.Find(tuple => tuple.Item1 == "suppression")}";

            shotConfirmUI.SetActive(true);
        }
    }
}
