using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        Soldier target = game.soldierManager.FindSoldierByName(transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().options[transform.Find("TargetPanel").Find("Target").Find("TargetDropdown").GetComponent<TMP_Dropdown>().value].text);
        Item gun = game.itemManager.FindItemById(transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>().options[transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>().value].text);
        bool resistSuppression = shooter.ResilienceCheck(), resistOverwatchFreeze = target.ResilienceCheck();
        int actingHitChance;

        //apply overwatch interrupt if resilience check failed
        Transform overwatchLocation = transform.Find("TargetPanel").Find("OverwatchLocation");
        if (int.TryParse(overwatchLocation.Find("XPos").GetComponent<TMP_InputField>().text, out int xOver) && int.TryParse(overwatchLocation.Find("YPos").GetComponent<TMP_InputField>().text, out int yOver) && int.TryParse(overwatchLocation.Find("ZPos").GetComponent<TMP_InputField>().text, out int zOver) && overwatchLocation.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value != 0)
        {
            gun.SpendSingleAmmo();
            int randNum1 = game.RandomNumber(0, 100);
            int randNum2 = game.RandomNumber(0, 100);
            Tuple<int, int, int> chances = game.CalculateHitPercentage(shooter, target, gun);

            //display suppression indicator
            if (shooter.IsSuppressed())
            {
                menu.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(true);

                if (resistSuppression)
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
                    target.TakeDamage(shooter, gun.gunCritDamage, false, new List<string>() { "Critical", "Shot" });
                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> CRITICAL HIT </color>";

                    //paying xp for hit
                    if (chances.Item1 >= 10)
                        menu.AddXpAlert(shooter, 8, "Critical overwatch shot on " + target.soldierName + "!", false);
                    else
                        menu.AddXpAlert(shooter, 10, "Critical overwatch shot with a " + chances.Item1 + "% chance on " + target.soldierName + "!", false);
                }
                else
                {
                    target.TakeDamage(shooter, gun.gunDamage, false, new List<string>() { "Shot" });
                    menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Hit </color>";

                    //paying xp for hit
                    if (chances.Item1 >= 10)
                        menu.AddXpAlert(shooter, 2, "Overwatch shot hit on " + target.soldierName + ".", false);
                    else
                        menu.AddXpAlert(shooter, 10, "Overwatch shot hit with a " + chances.Item1 + "% chance on " + target.soldierName + "!", false);
                }

                //don't show los check button if shot hits
                menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(false);
            }
            else
            {
                menu.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                menu.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {game.RandomShotScatterDistance()}cm {game.RandomShotScatterHorizontal()}, {game.RandomShotScatterDistance()}cm {game.RandomShotScatterVertical()}.\n\nDamage event ({gun.gunDamage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                //show los check button if shot misses
                menu.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                //paying xp for dodge
                if (chances.Item1 <= 90)
                    menu.AddXpAlert(target, 1, "Dodged overwatch shot from " + shooter.soldierName + ".", false);
                else
                    menu.AddXpAlert(target, 10, "Dodged overwatch shot with a " + chances.Item1 + "% chance from " + shooter.soldierName + "!", false);

                //push the no damage attack through for witness trigger
                target.TakeDamage(shooter, 0, true, new List<string>() { "Shot" });
            }

            //apply overwatch freeze on spot unless resisted
            if (!resistOverwatchFreeze)
            {
                target.X = xOver;
                target.Y = yOver;
                target.Z = zOver;
                target.terrainOn = overwatchLocation.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().options[overwatchLocation.Find("Terrain").Find("TerrainDropdown").GetComponent<TMP_Dropdown>().value].text;
                target.ap = 0;
                menu.AddDamageAlert(target, $"{target.soldierName} suffered overwatch daze at X: {target.X}, Y: {target.Y}, Z: {target.Z}, Ter: {target.terrainOn}.", false, true);
            }
            else
                menu.AddDamageAlert(target, target.soldierName + " resisted overwatch daze.", true, true);

            //unset overwatch after any shot
            shooter.UnsetOverwatch();

            //trigger loud action
            shooter.PerformLoudAction();

            menu.OpenShotResultUI();
            //destroy this instance of the overwatch shot
            Destroy(this.gameObject);
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
            Item gun = game.itemManager.FindItemById(transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>().options[transform.Find("Gun").Find("GunDropdown").GetComponent<TMP_Dropdown>().value].text);
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
