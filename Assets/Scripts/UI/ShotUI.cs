using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShotUI : MonoBehaviour
{
    public OverwatchShotUI overwatchShotUI;

    public TextMeshProUGUI shooterID;
    public TMP_Dropdown shotTypeDropdown;
    public TMP_Dropdown gunDropdown;
    public TMP_Dropdown gunsEmptyDropdown;
    public TMP_Dropdown comboGunsDropdown;
    public TMP_Dropdown comboGunsEmptyDropdown;
    public TMP_Dropdown aimTypeDropdown;
    public TMP_Dropdown targetDropdown;
    public TMP_Dropdown coverLevelDropdown;
    public TMP_InputField coverXPos;
    public TMP_InputField coverYPos;
    public TMP_InputField coverZPos;
    public TextMeshProUGUI barrelLocation;
    public TextMeshProUGUI suppressionValue;
    public TextMeshProUGUI apCost;
    public Button backButton;

    public GameObject aimTypeUI;
    public GameObject suppressionValueUI;
    public GameObject coverLocationUI;
    public GameObject invalidCoverLocationUI;
    public GameObject barrelLocationUI;
    public GameObject coverLevelUI;
    public GameObject flankersShotUI;
    public GameObject shotConfirmUI;
    public GameObject shotResultUI;

    public bool clearShotFlag;

    public Tuple<Soldier, IAmShootable> tempShooterTarget;
    public List<Tuple<string, string>> shotParameters = new();

    //shot functions - menu
    public void OpenShotUI()
    {
        //clear old data
        ClearShotUI();
        ClearShotConfirmUI();

        //set shooter details
        Soldier shooter = ActiveSoldier.Instance.S;
        shooterID.text = shooter.Id;

        //generate gun dropdown
        List<TMP_Dropdown.OptionData> gunOptionDataList = new();
        TMP_Dropdown.OptionData gunOptionData;
        bool leftGrey = false, rightGrey = false;
        if (shooter.LeftHandItem != null)
        {
            if (shooter.LeftHandItem.IsGun())
            {
                if (shooter.LeftHandItem.CheckAnyAmmo())
                    gunOptionData = new(shooter.LeftHandItem.itemName, shooter.LeftHandItem.itemImage, Color.white);
                else
                {
                    gunOptionData = gunsEmptyDropdown.options[gunsEmptyDropdown.options.FindIndex(option => option.text.Contains($"{shooter.LeftHandItem.itemName}"))];
                    leftGrey = true;
                }
                gunOptionDataList.Add(gunOptionData);
            }
        }
        if (shooter.RightHandItem != null)
        {
            if (shooter.RightHandItem.IsGun())
            {
                if (shooter.RightHandItem.CheckAnyAmmo())
                    gunOptionData = new(shooter.RightHandItem.itemName, shooter.RightHandItem.itemImage, Color.white);
                else
                {
                    gunOptionData = gunsEmptyDropdown.options[gunsEmptyDropdown.options.FindIndex(option => option.text.Contains($"{shooter.RightHandItem.itemName}"))];
                    rightGrey = true;
                }
                gunOptionDataList.Add(gunOptionData);
            }
        }
        if (gunOptionDataList.Count > 1)
        {
            foreach (TMP_Dropdown.OptionData option in comboGunsDropdown.options)
            {
                if (option.text.Contains(shooter.LeftHandItem.itemName) && option.text.Contains(shooter.RightHandItem.itemName))
                {
                    if (leftGrey || rightGrey)
                    {
                        gunOptionData = comboGunsEmptyDropdown.options[comboGunsEmptyDropdown.options.FindIndex(option => option.text.Contains($"{shooter.RightHandItem.itemName}") && option.text.Contains($"{shooter.RightHandItem.itemName}"))];
                        gunOptionDataList.Add(gunOptionData);
                    }
                    else
                    {
                        gunOptionData = option;
                        gunOptionDataList.Add(gunOptionData);
                    }
                }
            }

            aimTypeDropdown.value = 1;
            aimTypeDropdown.interactable = false;
        }
        gunDropdown.AddOptions(gunOptionDataList);

        if (leftGrey)
        {
            gunDropdown.GetComponent<DropdownController>().optionsToGrey.Add(shooter.LeftHandItem.itemName);
            gunDropdown.value = 1;
        }
        if (rightGrey)
            gunDropdown.GetComponent<DropdownController>().optionsToGrey.Add(shooter.RightHandItem.itemName);
        if (leftGrey || rightGrey)
            gunDropdown.GetComponent<DropdownController>().optionsToGrey.Add("2");

        //block suppression option if gun does not have enough ammo
        int gunsWithoutEnoughAmmoToSuppress = 0;
        foreach (Item gun in ActiveSoldier.Instance.S.EquippedGuns)
        {
            print($"{gun.itemName}|{gun.ammo}|{gun.suppressDrain}");
            if (!gun.CheckGreaterThanSpecificAmmo(gun.suppressDrain, true))
                gunsWithoutEnoughAmmoToSuppress++;
        }
        print(gunsWithoutEnoughAmmoToSuppress);
        if (gunsWithoutEnoughAmmoToSuppress == ActiveSoldier.Instance.S.EquippedGuns.Count)
            shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression");

        //if soldier engaged in melee block force unaimed shot
        if (shooter.IsMeleeEngaged())
        {
            aimTypeDropdown.value = 1;
            shotTypeDropdown.interactable = false;
            aimTypeDropdown.interactable = false;
            coverLevelDropdown.interactable = false;
        }

        UpdateShotType(shooter);
        UpdateShotUI(shooter);

        gameObject.SetActive(true);
    }

    public IEnumerator OpenOverwatchShotUI(Soldier shooter, Soldier target)
    {
        yield return new WaitUntil(() => MenuManager.Instance.MovementResolvedFlag() && MenuManager.Instance.detectionResolvedFlag);

        overwatchShotUI.Init(shooter, target);
        overwatchShotUI.gameObject.SetActive(true);
    }
    public void GuardsmanOverwatchRetry()
    {
        overwatchShotUI.ConfirmShotOverwatch(true);
    }
    public IEnumerator OpenShotResultUI(bool runSecondShot)
    {
        yield return new WaitUntil(() => MenuManager.Instance.explosionResolvedFlag);

        if (runSecondShot)
            shotResultUI.transform.Find("RunSecondShot").gameObject.SetActive(true);
        else
            shotResultUI.transform.Find("RunSecondShot").gameObject.SetActive(false);

        shotResultUI.SetActive(true);
    }
    public void CloseShotResultUI()
    {
        if (MenuManager.Instance.OverrideKey())
        {
            if (shotResultUI.transform.Find("RunSecondShot").gameObject.activeInHierarchy)
                ConfirmShot(false);
            else
            {
                SetShotResolvedFlagTo(true);
                shotResultUI.SetActive(false);
            }
        }
    }
    public void ClearShotUI()
    {
        clearShotFlag = true;
        shotTypeDropdown.interactable = true;
        shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        shotTypeDropdown.value = 0;

        gunDropdown.interactable = true;
        gunDropdown.value = 0;
        gunDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
        gunDropdown.ClearOptions();

        aimTypeDropdown.interactable = true;
        aimTypeDropdown.value = 0;

        targetDropdown.interactable = true;
        targetDropdown.ClearOptions();
        targetDropdown.value = 0;

        coverLevelDropdown.interactable = true;
        coverLevelDropdown.value = 0;

        coverXPos.text = "";
        coverYPos.text = "";
        coverZPos.text = "";
        invalidCoverLocationUI.SetActive(false);
        MenuManager.Instance.ClearFlankersUI(flankersShotUI);
        clearShotFlag = false;
    }

    public void CloseShotUI()
    {
        gameObject.SetActive(false);
        shotConfirmUI.SetActive(false);
    }
    public void OpenShotConfirmUI()
    {
        if (int.TryParse(apCost.text, out int ap))
        {
            if (ActiveSoldier.Instance.S.CheckAP(ap))
            {
                Soldier shooter = SoldierManager.Instance.FindSoldierById(shooterID.text);
                IAmShootable target = GameManager.Instance.FindShootableById(targetDropdown.captionText.text);
                Item gun1 = null, gun2 = null;
                Tuple<int, int, int> chances1 = null, chances2 = null;

                //if shooting with two guns
                if (gunDropdown.value == 2)
                {
                    gun1 = shooter.EquippedGuns[0];
                    gun2 = shooter.EquippedGuns[1];
                }
                else
                    gun1 = shooter.EquippedGuns[gunDropdown.value];
                shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text = gunDropdown.captionText.text;

                //if gun is valid, get chance for first shot
                if (gun1 != null)
                {
                    if (shotTypeDropdown.value == 1)
                        chances1 = Tuple.Create(100, 0, 100);
                    else
                        chances1 = CalculateHitPercentage(shooter, target, gun1);
                }

                //if first shot is valid, display details
                if (chances1 != null)
                {
                    //show gun image
                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("GunImage").GetComponent<Image>().sprite = gun1.itemImage;

                    //only shot suppression hit chance if suppressed
                    if (shooter.IsSuppressed() && shotTypeDropdown.value != 1)
                    {
                        shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").gameObject.SetActive(true);
                        shotConfirmUI.transform.Find("OptionPanel").Find("HitChanceLabels").Find("SuppressedHitChance").gameObject.SetActive(true);
                    }
                    else
                    {
                        shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").gameObject.SetActive(false);
                        shotConfirmUI.transform.Find("OptionPanel").Find("HitChanceLabels").Find("SuppressedHitChance").gameObject.SetActive(false);
                    }

                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = chances1.Item3.ToString() + "%";
                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = chances1.Item1.ToString() + "%";
                    shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = chances1.Item2.ToString() + "%";

                    //enable back button only if shot is aimed and under 25%
                    if (aimTypeDropdown.captionText.text.Contains("Aimed") && chances1.Item1 <= 25)
                        shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = true;
                    else
                        shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = false;

                    //add parameter to equation view
                    shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = DisplayShotParameters();


                    shotConfirmUI.SetActive(true);
                }

                //if shooting with two guns
                if (gunDropdown.value == 2)
                {
                    shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").gameObject.SetActive(true);

                    //show gun image
                    shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("GunImage").GetComponent<Image>().sprite = gun2.itemImage;

                    //if gun is valid
                    if (gun2 != null)
                    {
                        if (shotTypeDropdown.value == 1)
                            chances2 = Tuple.Create(100, 0, 100);
                        else
                            chances2 = CalculateHitPercentage(shooter, target, gun2);
                    }

                    //only continue if shot is valid
                    if (chances2 != null)
                    {
                        //only shot suppression hit chance if suppressed
                        if (shooter.IsSuppressed() && shotTypeDropdown.value != 1)
                            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").gameObject.SetActive(true);
                        else
                            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").gameObject.SetActive(false);

                        shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = chances2.Item3.ToString() + "%";
                        shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = chances2.Item1.ToString() + "%";
                        shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = chances2.Item2.ToString() + "%";

                        //back button always disabled for unaimed shot
                        shotConfirmUI.transform.Find("OptionPanel").Find("Back").GetComponent<Button>().interactable = false;

                        //add parameter to equation view
                        //shotConfirmUI.transform.Find("EquationPanel").Find("Parameters").GetComponent<TextMeshProUGUI>().text = DisplayShotParameters();

                        shotConfirmUI.SetActive(true);
                    }
                }
                else
                    shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").gameObject.SetActive(false);
            }
        }
    }
    public void ExitShotConfirmUI()
    {
        FileUtility.WriteToReport($"{ActiveSoldier.Instance.S.soldierName} bails out of aimed shot ({shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text}|{shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text})"); //write to report

        int.TryParse(apCost.text, out int ap);
        //deduct ap for aiming if leaving shot
        ActiveSoldier.Instance.S.DeductAP(ap - 1);

        CloseShotUI();
    }
    public void ClearShotConfirmUI()
    {
        //if (shotConfirmUI.activeInHierarchy)
        {
            clearShotFlag = true;
            shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("PrimaryGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("SuppressedHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("HitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("OptionPanel").Find("AltGun").Find("CritHitChance").GetComponent<TextMeshProUGUI>().text = "";
            shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text = "";
            clearShotFlag = false;
        }
    }












    // actual shot functions
    public void UpdateShotUI(Soldier shooter)
    {
        //if function is called not from a script, shooter has to be determined from interface
        if (shooter.id == "0")
            shooter = SoldierManager.Instance.FindSoldierById(shooterID.text);

        if (!MenuManager.Instance.clearShotFlag)
        {
            UpdateShotAP(shooter);
            if (shotTypeDropdown.value == 0)
                UpdateTarget(shooter);
            else
                UpdateSuppressionValue(shooter);
        }
    }
    public void UpdateShotType(Soldier shooter)
    {
        //if function is called not from a script, shooter has to be determined from interface
        if (shooter.id == "0")
            shooter = SoldierManager.Instance.FindSoldierById(shooterID.text);

        List<TMP_Dropdown.OptionData> targetOptionDataList = new();

        //initialise
        targetDropdown.ClearOptions();
        aimTypeUI.SetActive(false);
        suppressionValueUI.SetActive(false);
        coverLocationUI.SetActive(false);

        //generate target list
        foreach (Soldier s in GameManager.Instance.AllFieldedSoldiers())
        {
            TMP_Dropdown.OptionData targetOptionData = null;
            if (s.IsAlive() && shooter.IsOppositeTeamAs(s) && s.IsRevealed())
            {
                if (shooter.CanSeeInOwnRight(s))
                    targetOptionData = new(s.Id, s.soldierPortrait, Color.white);
                else
                {
                    if (s.IsJammer() && !shooter.IsRevoker())
                        targetOptionData = new(s.Id, s.LoadPortraitJammed(s.soldierPortraitText), Color.white);
                    else
                        targetOptionData = new(s.Id, s.LoadPortraitTeamsight(s.soldierPortraitText), Color.white);
                }

            }

            if (targetOptionData != null)
            {
                targetOptionDataList.Add(targetOptionData);

                //remove option if soldier is engaged and this soldier is not on the engagement list
                if (ActiveSoldier.Instance.S.IsMeleeEngaged() && !ActiveSoldier.Instance.S.IsMeleeEngagedWith(s))
                    targetOptionDataList.Remove(targetOptionData);
            }
        }

        if (shotTypeDropdown.value == 0)
        {
            aimTypeUI.SetActive(true);

            //add explosive barrels to target list
            foreach (ExplosiveBarrel b in FindObjectsByType<ExplosiveBarrel>(default))
            {
                TMP_Dropdown.OptionData targetOptionData = null;
                if (shooter.PhysicalObjectIsRevealed(b))
                    targetOptionData = new(b.Id, MenuManager.Instance.explosiveBarrelSprite, Color.white);

                if (targetOptionData != null)
                    targetOptionDataList.Add(targetOptionData);
            }

            //add coverman
            targetOptionDataList.Add(new("coverman", MenuManager.Instance.covermanSprite, Color.white));
        }
        else if (shotTypeDropdown.value == 1)
            suppressionValueUI.SetActive(true);

        targetDropdown.AddOptions(targetOptionDataList);
        UpdateShotUI(shooter);
    }
    public void UpdateShotAP(Soldier shooter)
    {
        int ap = 1;
        if (shotTypeDropdown.value == 1)
        {
            ap = shooter.ap;
        }
        else
        {
            if (aimTypeDropdown.value == 0)
            {
                if (shooter.IsGunner())
                    ap++;
                else
                {
                    bool hasLMGOrSniper = false;
                    foreach (Item gun in shooter.EquippedGuns)
                        if (gun.SpecialityTag().Equals("Sniper Rifle") || gun.SpecialityTag().Equals("Light Machine Gun"))
                            hasLMGOrSniper = true;

                    if (hasLMGOrSniper)
                        ap += 2;
                    else
                        ap++;
                }
            }
        }

        //set ap to 0 for overwatch
        if (shooter.IsOffturn())
            ap = 0;

        apCost.text = ap.ToString();
    }
    public void UpdateTarget(Soldier shooter)
    {
        print($"caption text -> {targetDropdown.captionText.text}");
        IAmShootable target = GameManager.Instance.FindShootableById(targetDropdown.captionText.text);

        //initialise
        coverLocationUI.SetActive(false);
        barrelLocationUI.SetActive(false);
        coverLevelUI.SetActive(false);

        print($"updating target -> {target}");
        if (target is Coverman)
        {
            print("targeting coverman");
            coverLocationUI.SetActive(true);
            shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression");
            MenuManager.Instance.ClearFlankersUI(MenuManager.Instance.flankersShotUI);
        }
        else if (target is ExplosiveBarrel targetBarrel)
        {
            barrelLocation.text = $"X:{targetBarrel.X} Y:{targetBarrel.Y} Z:{targetBarrel.Z}";
            barrelLocationUI.SetActive(true);
            shotTypeDropdown.GetComponent<DropdownController>().optionsToGrey.Add("Suppression");
            MenuManager.Instance.ClearFlankersUI(MenuManager.Instance.flankersShotUI);
        }
        else if (target is Soldier targetSoldier)
        {
            if (targetSoldier.IsInCover())
                coverLevelUI.SetActive(true);

            UpdateTargetFlanking(shooter, targetSoldier);
        }
    }
    public void UpdateSuppressionValue(Soldier shooter)
    {
        int suppressionValue = 0;
        IAmShootable target = GameManager.Instance.FindShootableById(targetDropdown.captionText.text);
        string suppressionBracket = GameManager.Instance.CalculateRangeBracket(GameManager.Instance.CalculateRange(shooter, target as PhysicalObject)) switch
        {
            "Melee" or "CQB" => "cQBSupPen",
            "Short" => "shortSupPen",
            "Medium" => "medSupPen",
            "Long" or "Coriolis" => "longSupPen",
            _ => "",
        };

        print($"{suppressionBracket}");
        if (gunDropdown.value == 0)
            suppressionValue = shooter.EquippedGuns[0].GetSuppressionValue(suppressionBracket);
        else if (gunDropdown.value == 1)
            suppressionValue = (int)shooter.EquippedGuns[1].GetSuppressionValue(suppressionBracket);
        else if (gunDropdown.value == 2)
        {
            foreach (Item gun in shooter.EquippedGuns)
            {
                print($"{gun.itemName}");
                print($"pre {suppressionValue}");
                suppressionValue = HelperFunctions.CalculateSuppression(suppressionValue, gun.GetSuppressionValue(suppressionBracket));
                print($"post {suppressionValue}");
            }
        }

        this.suppressionValue.text = suppressionValue.ToString();
    }
    public void UpdateTargetFlanking(Soldier shooter, Soldier target)
    {
        //clear the flanker ui
        MenuManager.Instance.ClearFlankersUI(MenuManager.Instance.flankersShotUI);
        int flankersCount = 0;
        int flankingAngle = 80;
        List<Tuple<float, Soldier>> allFlankingAngles = new();
        List<Tuple<float, Soldier>> confirmedFlankingAngles = new();

        if (shooter.IsTactician() && !target.IsRevoker())
            flankingAngle = 20;

        if (target != null)
        {
            if (!target.IsTactician() || shooter.IsRevoker())
            {
                //find all soldiers who could be considered for flanking and their flanking angles
                foreach (Soldier s in GameManager.Instance.AllFieldedSoldiers())
                    if (s.IsAbleToSee() && s.IsSameTeamAs(shooter) && s.CanSeeInOwnRight(target) && !HelperFunctions.IsWithinAngle(new(shooter.X, shooter.Y), new(s.X, s.Y), new(target.X, target.Y), 80f))
                        allFlankingAngles.Add(Tuple.Create(HelperFunctions.CalculateAngle360(new(shooter.X, shooter.Y), new(s.X, s.Y), new(target.X, target.Y)), s));

                //order smallest angle to largest angle
                allFlankingAngles = allFlankingAngles.OrderBy(t => t.Item1).ToList();

                /*string msg = "";
                foreach (var item in allFlankingAngles)
                    msg += $"({item.Item1}, {item.Item2.soldierName})";
                print(msg);*/

                // Iterate through both lists to find out which gives more flanking options
                for (int i = 0; i < allFlankingAngles.Count; i++)
                {
                    Tuple<float, Soldier> currentItem = allFlankingAngles[i];
                    print($"current item {currentItem}");
                    // Check if Item1 is greater than flankingAngle
                    if (currentItem.Item1 > flankingAngle)
                    {
                        confirmedFlankingAngles.Add(currentItem);
                        allFlankingAngles = allFlankingAngles.Select(t => Tuple.Create(Math.Abs(t.Item1 - currentItem.Item1), t.Item2)).ToList();
                    }
                }

                foreach (Tuple<float, Soldier> confirmedFlankAngle in confirmedFlankingAngles)
                {
                    if (flankersCount < 3)
                    {
                        flankersCount++;

                        //add flanker to ui to visualise
                        GameObject flankerPortrait = Instantiate(MenuManager.Instance.soldierPortraitPrefab, MenuManager.Instance.flankersShotUI.transform.Find("FlankersPanel"));
                        flankerPortrait.GetComponentInChildren<SoldierPortrait>().Init(confirmedFlankAngle.Item2);
                    }
                }

                //display flankers if there are any
                if (flankersCount > 0)
                    MenuManager.Instance.OpenFlankersUI(MenuManager.Instance.flankersShotUI);
            }
        }
    }
    public int WeaponHitChance(Soldier shooter, IAmShootable target, Item gun)
    {
        int weaponHitChance, baseWeaponHitChance, sharpshooterBonus = 0, inspiredBonus = 0;

        //get base hit chance
        switch (GameManager.Instance.CalculateRangeBracket(GameManager.Instance.CalculateRange(shooter, target as PhysicalObject)))
        {
            case "Melee":
            case "CQB":
                if (aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.cQBA;
                else
                    baseWeaponHitChance = gun.cQBU;
                break;
            case "Short":
                if (aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.shortA;
                else
                    baseWeaponHitChance = gun.shortU;
                break;
            case "Medium":
                if (aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.medA;
                else
                    baseWeaponHitChance = gun.medU;
                break;
            case "Long":
                if (aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.longA;
                else
                    baseWeaponHitChance = gun.longU;
                break;
            case "Coriolis":
                if (aimTypeDropdown.value == 0)
                    baseWeaponHitChance = gun.coriolisA;
                else
                    baseWeaponHitChance = gun.coriolisU;
                break;
            default:
                baseWeaponHitChance = 0;
                break;
        }
        weaponHitChance = baseWeaponHitChance;

        //apply sharpshooter buff
        if (baseWeaponHitChance > 0 && shooter.IsSharpshooter() && target is Soldier targetSoldier && !targetSoldier.IsRevoker())
            sharpshooterBonus = 5;
        weaponHitChance += sharpshooterBonus;

        //apply inspirer buff
        inspiredBonus += shooter.InspirerBonusWeapon(gun);
        weaponHitChance += inspiredBonus;

        //correct negatives
        if (weaponHitChance < 0)
            weaponHitChance = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("accuracy", $"{baseWeaponHitChance}"));
        shotParameters.Add(Tuple.Create("sharpshooter", $"{sharpshooterBonus}"));
        shotParameters.Add(Tuple.Create("inspired", $"{inspiredBonus}"));

        return weaponHitChance;
    }
    public float ShooterTraumaMod(Soldier shooter)
    {
        float traumaMod = shooter.tp switch
        {
            1 => 0.1f,
            2 => 0.2f,
            3 => 0.4f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("trauma", $"{1 - traumaMod}"));

        return 1 - traumaMod;
    }
    public float ShooterSustenanceMod(Soldier shooter)
    {
        float sustenanceMod = 0;

        if (shooter.roundsWithoutFood >= 20)
            sustenanceMod = 0.5f;

        //report parameters
        shotParameters.Add(Tuple.Create("sustenance", $"{1 - sustenanceMod}"));

        return 1 - sustenanceMod;
    }
    public float ShooterSmokeMod(Soldier shooter)
    {
        float smokeMod = 0;

        if (shooter.IsInSmokeBlindZone())
            smokeMod = 0.9f;
        else if (shooter.IsInSmokeDefenceZone())
            smokeMod = 0.45f;

        //report parameters
        shotParameters.Add(Tuple.Create("smoke", $"{1 - smokeMod}"));

        return 1 - smokeMod;
    }
    public float ShooterTabunMod(Soldier shooter)
    {
        float tabunMod = 0;

        if (shooter.IsInTabun())
        {
            if (shooter.CheckTabunEffectLevel(100))
                tabunMod = 0.8f;
            else if (shooter.CheckTabunEffectLevel(50))
                tabunMod = 0.4f;
            else if (shooter.CheckTabunEffectLevel(25))
                tabunMod = 0.2f;
        }

        //report parameters
        shotParameters.Add(Tuple.Create("tabun", $"{1 - tabunMod}"));

        return 1 - tabunMod;
    }
    public int ShooterFightMod(Soldier shooter)
    {
        int fightMod = 0;

        if (shooter.FightActive())
            fightMod = 5 * shooter.stats.F.Val;
        else if (shooter.AvengingActive()) //avenger ability
            fightMod = 5 * (shooter.stats.F.Val - 1);

        //report parameters
        shotParameters.Add(Tuple.Create("fight", $"{fightMod}"));

        return fightMod;
    }
    public float RelevantWeaponSkill(Soldier shooter, Item gun)
    {
        int juggernautBonus = 0, stimBonus = 0;

        float weaponSkill = gun.SpecialityTag() switch
        {
            "Assault Rifle" => shooter.stats.AR.Val,
            "Light Machine Gun" => shooter.stats.LMG.Val,
            "Rifle" => shooter.stats.Ri.Val,
            "Shotgun" => shooter.stats.Sh.Val,
            "Sub-Machine Gun" => shooter.stats.SMG.Val,
            "Sniper Rifle" => shooter.stats.Sn.Val,
            "Pistol" or _ => shooter.stats.GetHighestWeaponSkill(),
        };
        //report parameters
        shotParameters.Add(Tuple.Create("WS", $"{weaponSkill}"));

        //apply juggernaut armour debuff
        if (shooter.IsWearingJuggernautArmour(false))
            juggernautBonus = -1;
        weaponSkill += juggernautBonus;
        //report parameters
        shotParameters.Add(Tuple.Create("juggernaut", $"{juggernautBonus}"));

        //apply stim armour buff
        if (shooter.IsWearingStimulantArmour())
            stimBonus = 2;
        weaponSkill += stimBonus;
        //report parameters
        shotParameters.Add(Tuple.Create("stim", $"{stimBonus}"));

        //apply trauma debuff
        weaponSkill *= ShooterTraumaMod(shooter);

        //apply sustenance debuff
        weaponSkill *= ShooterSustenanceMod(shooter);

        //correct negatives
        if (weaponSkill < 0)
            weaponSkill = 0;

        return weaponSkill;
    }
    public int TargetEvasion(IAmShootable target)
    {
        int targetE = 0;
        if (target is Soldier targetSoldier)
            targetE = targetSoldier.stats.E.Val;
        shotParameters.Add(Tuple.Create("tE", $"{targetE}"));
        return targetE;
    }
    public float CoverMod()
    {
        var coverMod = coverLevelDropdown.value switch
        {
            1 => 0.1f,
            2 => 0.34f,
            3 => 0.62f,
            4 => 0.88f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("cover", $"{1 - coverMod}"));
        return 1 - coverMod;
    }
    public float VisMod(Soldier shooter)
    {
        float visMod;

        if (shooter.IsWearingThermalGoggles())
            visMod = 0.0f;
        else
        {
            visMod = WeatherManager.Instance.CurrentVis switch
            {
                "Full" => 0.0f,
                "Good" => 0.02f,
                "Moderate" => 0.18f,
                "Poor" => 0.26f,
                "Zero" => 0.64f,
                _ => 0.0f,
            };
        }

        //report parameters
        shotParameters.Add(Tuple.Create("vis", $"{1 - visMod}"));
        return 1 - visMod;
    }
    public float RainMod(Soldier shooter, IAmShootable target)
    {
        string rainfall = WeatherManager.Instance.CurrentRain;

        if (shooter.IsCalculator() && target is Soldier targetSoldier1 && !targetSoldier1.IsRevoker())
            rainfall = WeatherManager.Instance.DecreasedRain(rainfall);
        if (target is Soldier targetSoldier2 && targetSoldier2.IsCalculator() && !shooter.IsRevoker())
            rainfall = WeatherManager.Instance.IncreasedRain(rainfall);

        float rainMod = rainfall switch
        {
            "Zero" or "Light" => 0.0f,
            "Moderate" => 0.02f,
            "Heavy" => 0.08f,
            "Torrential" => 0.18f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("rain", $"{1 - rainMod}"));
        return 1 - rainMod;
    }
    public float RainMod(Soldier shooter)
    {
        string rainfall = WeatherManager.Instance.CurrentRain;

        if (shooter.IsCalculator())
            rainfall = WeatherManager.Instance.DecreasedRain(rainfall);

        var rainMod = rainfall switch
        {
            "Zero" or "Light" => 0.0f,
            "Moderate" => 0.02f,
            "Heavy" => 0.08f,
            "Torrential" => 0.18f,
            _ => 0.0f,
        };

        //report parameters
        shotParameters.Add(Tuple.Create("rain", $"{1 - rainMod}"));
        return 1 - rainMod;
    }
    public float WindMod(Soldier shooter, IAmShootable target)
    {
        Vector2 shotLine = new(target.X - shooter.X, target.Y - shooter.Y);
        shotLine.Normalize();
        Vector2 windLine = WeatherManager.Instance.CurrentWindDirectionVector;
        float shotAngleRelativeToWind = Vector2.Angle(shotLine, windLine);
        //print("WIND: " + windLine + " SHOT: " + shotLine + "ANGLE: " + shotAngleRelativeToWind);

        float windMod;

        string windSpeed = WeatherManager.Instance.CurrentWindSpeed;
        if (shooter.IsCalculator())
            windSpeed = WeatherManager.Instance.DecreasedWindspeed(windSpeed);
        if (target is Soldier targetSoldier)
            if (targetSoldier.IsCalculator())
                windSpeed = WeatherManager.Instance.IncreasedWindspeed(windSpeed);

        if (shotAngleRelativeToWind <= 22.5 || shotAngleRelativeToWind >= 157.5)
            windMod = 0f;
        else if (shotAngleRelativeToWind >= 67.5 && shotAngleRelativeToWind <= 112.5)
        {

            if (windSpeed == "Strong")
                windMod = 0.29f;
            else if (windSpeed == "Moderate")
                windMod = 0.12f;
            else if (windSpeed == "Light")
                windMod = 0.06f;
            else
                windMod = 0f;
        }
        else
        {
            if (windSpeed == "Strong")
                windMod = 0.10f;
            else if (windSpeed == "Moderate")
                windMod = 0.06f;
            else if (windSpeed == "Light")
                windMod = 0.02f;
            else
                windMod = 0f;
        }

        //report parameters
        shotParameters.Add(Tuple.Create("wind", $"{1 - windMod}"));
        return 1 - windMod;
    }
    public float ShooterHealthMod(Soldier shooter)
    {
        float shooterHealthMod;
        if (shooter.IsLastStand())
            shooterHealthMod = 0.6f;
        else if (shooter.hp <= shooter.stats.H.Val / 2)
            shooterHealthMod = 0.16f;
        else if (shooter.hp < shooter.stats.H.Val)
            shooterHealthMod = 0.06f;
        else
            shooterHealthMod = 0f;

        //report parameters
        shotParameters.Add(Tuple.Create("HP", $"{1 - shooterHealthMod}"));

        return 1 - shooterHealthMod;
    }
    public float TargetHealthMod(IAmShootable target)
    {
        float targetHealthMod = 0;
        if (target is Soldier targetSoldier)
        {

            if (targetSoldier.IsLastStand())
                targetHealthMod = -0.4f;
            else if (targetSoldier.hp <= targetSoldier.stats.H.Val / 2)
                targetHealthMod = -0.14f;
            else if (targetSoldier.hp < targetSoldier.stats.H.Val)
                targetHealthMod = -0.04f;
            else
                targetHealthMod = 0f;
        }
        //report parameters
        shotParameters.Add(Tuple.Create("tHP", $"{1 - targetHealthMod}"));
        return 1 - targetHealthMod;
    }
    public float ShooterTerrainMod(Soldier shooter)
    {
        float shooterTerrainMod;
        if (shooter.IsOnNativeTerrain())
            shooterTerrainMod = -0.04f;
        else if (shooter.IsOnOppositeTerrain())
            shooterTerrainMod = 0.12f;
        else
            shooterTerrainMod = 0.06f;

        //report parameters
        shotParameters.Add(Tuple.Create("Ter", $"{1 - shooterTerrainMod}"));

        return 1 - shooterTerrainMod;
    }
    public float TargetTerrainMod(IAmShootable target)
    {
        float targetTerrainMod = 0;
        if (target is Soldier targetSoldier)
        {
            if (targetSoldier.IsOnNativeTerrain())
                targetTerrainMod = 0.16f;
            else if (targetSoldier.IsOnOppositeTerrain())
                targetTerrainMod = -0.08f;
            else
                targetTerrainMod = -0.02f;
        }

        //report parameters
        shotParameters.Add(Tuple.Create("tTer", $"{1 - targetTerrainMod}"));
        return 1 - targetTerrainMod;
    }
    public float ElevationMod(Soldier shooter, IAmShootable target)
    {
        float elevationMod = (target.Z - shooter.Z) * 0.01f;

        //report parameters
        shotParameters.Add(Tuple.Create("elevation", $"{1 - elevationMod}"));
        return 1 - elevationMod;
    }
    public float KdMod(Soldier shooter)
    {
        float kdMod;
        int kd = shooter.GetKd();

        if (kd != 0)
            kdMod = -(2 * kd * 0.01f);
        else
            kdMod = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("kd", $"{1 - kdMod}"));

        return 1 - kdMod;
    }
    public float OverwatchMod(Soldier shooter)
    {
        float overwatchMod;
        if (shooter.IsOnOverwatch())
        {
            if (shooter.IsGuardsman())
                overwatchMod = 0.2f;
            else
                overwatchMod = 0.4f;
        }
        else
            overwatchMod = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("overwatch", $"{1 - overwatchMod}"));

        return 1 - overwatchMod;
    }
    public float FlankingMod(IAmShootable target)
    {
        float flankingMod = 0;
        if (target is Soldier targetSoldier)
        {
            if (!targetSoldier.IsTactician())
            {
                flankingMod = MenuManager.Instance.flankersShotUI.transform.Find("FlankersPanel").childCount switch
                {
                    1 => -0.2f,
                    2 => -0.5f,
                    3 => -1.0f,
                    _ => 0f,
                };
            }
        }

        //report parameters
        shotParameters.Add(Tuple.Create("flank", $"{1 - flankingMod}"));
        return 1 - flankingMod;
    }
    public float StealthMod(Soldier shooter)
    {
        float stealthMod;
        if (shooter.IsHidden())
            stealthMod = -0.4f;
        else
            stealthMod = 0;

        //report parameters
        shotParameters.Add(Tuple.Create("stealth", $"{1 - stealthMod}"));

        return 1 - stealthMod;
    }
    public int ShooterSuppressionMod(Soldier shooter)
    {
        int suppressionMod = shooter.GetSuppression();

        //report parameters
        shotParameters.Add(Tuple.Create("suppression", $"{suppressionMod}"));

        return suppressionMod;
    }
    public Tuple<int, int, int> CalculateHitPercentage(Soldier shooter, IAmShootable target, Item gun)
    {
        //destroy old shot parameters
        shotParameters.Clear();
        Tuple<int, int, int> chances;
        int suppressedHitChance, hitChance, critChance;

        if (target is Coverman || target is ExplosiveBarrel) //shooting at objects
        {
            //calculate base shot chance
            hitChance = Mathf.RoundToInt((WeaponHitChance(shooter, target, gun) + 10 * RelevantWeaponSkill(shooter, gun)) * VisMod(shooter) * RainMod(shooter, target) * WindMod(shooter, target) * ShooterHealthMod(shooter) * ShooterTerrainMod(shooter) * ElevationMod(shooter, target) * ShooterSmokeMod(shooter) * ShooterTabunMod(shooter)) + ShooterFightMod(shooter);

            //calculate critical shot chance
            critChance = Mathf.RoundToInt((Mathf.Pow(RelevantWeaponSkill(shooter, gun), 2) * (hitChance / 100.0f)));
        }
        else //shooting at soldier
        {
            //calculate base shot chance
            hitChance = Mathf.RoundToInt((WeaponHitChance(shooter, target, gun) + 10 * RelevantWeaponSkill(shooter, gun) - 12 * TargetEvasion(target)) * CoverMod() * VisMod(shooter) * RainMod(shooter, target) * WindMod(shooter, target) * ShooterHealthMod(shooter) * TargetHealthMod(target) * ShooterTerrainMod(shooter) * TargetTerrainMod(target) * ElevationMod(shooter, target) * KdMod(shooter) * OverwatchMod(shooter) * FlankingMod(target) * StealthMod(shooter) * ShooterSmokeMod(shooter) * ShooterTabunMod(shooter)) + ShooterFightMod(shooter);

            //calculate critical shot chance
            critChance = Mathf.RoundToInt((Mathf.Pow(RelevantWeaponSkill(shooter, gun), 2) * (hitChance / 100.0f)) - TargetEvasion(target));
        }

        //declare suppression shot chance
        suppressedHitChance = hitChance - ShooterSuppressionMod(shooter);

        //cap extremes
        if (suppressedHitChance < 0)
            suppressedHitChance = 0;
        else if (suppressedHitChance > 100)
            suppressedHitChance = 100;

        if (hitChance < 0)
            hitChance = 0;
        else if (hitChance > 100)
            hitChance = 100;

        if (critChance < 0)
            critChance = 0;
        else if (critChance > 100)
            critChance = 100;

        chances = Tuple.Create(hitChance, critChance, suppressedHitChance);

        return chances;
    }
    public void ConfirmShot(bool retry)
    {
        Soldier shooter = SoldierManager.Instance.FindSoldierById(shooterID.text);
        IAmShootable target = GameManager.Instance.FindShootableById(targetDropdown.captionText.text);
        Item gun = null;
        bool runSecondShot = false;
        string gunNames = MenuManager.Instance.shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text;
        print(gunNames);
        if (gunNames.Contains("|"))
        {
            string[] guns = MenuManager.Instance.shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text.Split('|');
            gun = shooter.GetEquippedGun(guns[0]);
            MenuManager.Instance.shotConfirmUI.transform.Find("GunName").GetComponent<TextMeshProUGUI>().text = guns[1];
            runSecondShot = true;
        }
        else
            gun = shooter.GetEquippedGun(gunNames);

        int.TryParse(apCost.text, out int ap);
        int actingHitChance;
        bool resistSuppression;
        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(false);
        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(false);
        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("AvengerRetry").gameObject.SetActive(false);
        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("GuardsmanRetry").gameObject.SetActive(false);

        tempShooterTarget = Tuple.Create(shooter, target);
        if (runSecondShot || retry)
        {
            if (retry)
                FileUtility.WriteToReport($"{shooter.soldierName} attempts avenger retry."); //write to report
        }
        else
            ActiveSoldier.Instance.S.DeductAP(ap);

        SetShotResolvedFlagTo(false);

        if (shotTypeDropdown.value == 0) //standard shot
        {
            if (target is Soldier)
                FileUtility.WriteToReport($"{shooter.soldierName} shoots at {(target as Soldier).soldierName}. Weather: {WeatherManager.Instance.CurrentWeather} Target Cover: {coverLevelDropdown.captionText.text}"); //write to report
            else
                FileUtility.WriteToReport($"{shooter.soldierName} shoots at {target.GetType()}. Weather: {WeatherManager.Instance.CurrentWeather}"); //write to report

            //play shot sfx
            SoundManager.Instance.PlayShotResolution(gun);

            resistSuppression = shooter.SuppressionCheck();
            gun.SpendSingleAmmo();

            int randNum1 = HelperFunctions.RandomShotNumber();
            int randNum2 = HelperFunctions.RandomCritNumber();
            Tuple<int, int, int> chances;
            chances = CalculateHitPercentage(shooter, target, gun);

            //display shooter suppression indicator
            if (shooter.IsSuppressed())
            {
                MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").gameObject.SetActive(true);

                if (resistSuppression)
                {
                    FileUtility.WriteToReport($"{shooter.soldierName} resists suppression."); //write to report

                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Resisted Suppression</color>";
                    actingHitChance = chances.Item1;
                }
                else
                {
                    FileUtility.WriteToReport($"{shooter.soldierName} suffers suppression."); //write to report

                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("SuppressionResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=orange>Suffered Suppression</color>";
                    actingHitChance = chances.Item3;
                }
            }
            else
                actingHitChance = chances.Item1;

            if (target is Coverman)
            {
                int coverDamage = GameManager.Instance.CalculateRangeBracket(GameManager.Instance.CalculateRange(shooter, target as PhysicalObject)) switch
                {
                    "Melee" or "CQB" => gun.cQBCovDamage,
                    "Short" => gun.shortCovDamage,
                    "Medium" => gun.medCovDamage,
                    "Long" or "Coriolis" => gun.longCovDamage,
                    _ => 0,
                };

                //show los check button
                MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                //standard shot hits cover
                if (randNum1 <= actingHitChance)
                {
                    FileUtility.WriteToReport($"{shooter.soldierName} hits cover at ({target.X}, {target.Y}, {target.Z}) ({actingHitChance}%|{chances.Item2}%)"); //write to report

                    //play cover destruction
                    SoundManager.Instance.PlayCoverDestruction();

                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                    //critical shot hits cover
                    if (randNum2 <= chances.Item2)
                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> COVER DESTROYED </color>";
                    else
                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Cover hit (" + coverDamage + " damage)</color>";
                }
                else
                {
                    string missString = GameManager.Instance.RandomShotMissString();

                    FileUtility.WriteToReport($"{shooter.soldierName} misses cover at ({target.X}, {target.Y}, {target.Z}) ({actingHitChance}%|{chances.Item2}%), shot goes {missString}"); //write to report

                    //play shot miss dialogue
                    SoundManager.Instance.PlaySoldierShotMiss(shooter);

                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {missString}.\n\nDamage event ({gun.damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                }
            }
            else if (target is ExplosiveBarrel targetBarrel)
            {
                //show los check button
                MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                //standard shot hits barrel
                if (randNum1 <= actingHitChance)
                {
                    FileUtility.WriteToReport($"{shooter.soldierName} hits explosive barrel at ({target.X}, {target.Y}, {target.Z}) ({actingHitChance}%|{chances.Item2}%)"); //write to report

                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";

                    //critical shot hits barrel
                    if (randNum2 <= chances.Item2)
                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Barrel Explodes (Crit)!</color>";
                    else
                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green>Barrel Explodes!</color>";
                    targetBarrel.CheckExplosionBarrel(shooter);
                }
                else
                {
                    string missString = GameManager.Instance.RandomShotMissString();

                    FileUtility.WriteToReport($"{shooter.soldierName} misses explosive barrel at ({target.X}, {target.Y}, {target.Z}) ({actingHitChance}%|{chances.Item2}%), shot goes {missString}"); //write to report

                    //play shot miss dialogue
                    SoundManager.Instance.PlaySoldierShotMiss(shooter);

                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {missString}.\n\nDamage event ({gun.damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                }
            }
            else if (target is Soldier targetSoldier) //check if target is soldier
            {
                //standard shot hits
                if (randNum1 <= actingHitChance)
                {
                    Soldier originalTarget = targetSoldier;

                    //if target is engaged, and not engaged with the shooter
                    if (targetSoldier.IsMeleeEngaged() && !targetSoldier.IsMeleeEngagedWith(shooter))
                    {
                        //pick random target to hit in engagement
                        int randNum3 = HelperFunctions.RandomNumber(0, originalTarget.EngagedSoldiers.Count);
                        if (randNum3 > 0)
                        {
                            targetSoldier = originalTarget.EngagedSoldiers[randNum3 - 1];
                            MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Shot into melee aiming for {originalTarget.soldierName}, hit {targetSoldier.soldierName}.";
                        }
                        else
                            MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Shot into melee and hit intended target.";

                        FileUtility.WriteToReport($"{shooter.soldierName} shoots into melee aiming for {originalTarget.soldierName}, hits {targetSoldier.soldierName} ({actingHitChance}%|{chances.Item2}%)."); //write to report
                    }
                    else
                    {
                        FileUtility.WriteToReport($"{shooter.soldierName} hits {targetSoldier.soldierName} ({actingHitChance}%|{chances.Item2}%)."); //write to report

                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Shot directly on target.";
                    }


                    //standard shot crit hits
                    if (randNum2 <= chances.Item2)
                    {
                        FileUtility.WriteToReport($"The shot is critical!"); //write to report

                        targetSoldier.TakeDamage(shooter, gun.critDamage, false, new() { "Critical", "Shot" }, Vector3.zero);
                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> CRITICAL SHOT </color>";

                        if (targetSoldier.IsSelf(originalTarget)) //only pay xp if you hit correct target 
                        {
                            //paying xp for hit
                            if (actingHitChance < 10)
                                MenuManager.Instance.AddXpAlert(shooter, 10, $"Critical shot <10% hit on {targetSoldier.soldierName}!", false);
                            else
                                MenuManager.Instance.AddXpAlert(shooter, 8, $"Critical shot hit on {targetSoldier.soldierName}!", false);
                        }
                    }
                    else
                    {
                        targetSoldier.TakeDamage(shooter, gun.damage, false, new() { "Shot" }, Vector3.zero);
                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "<color=green> Hit </color>";

                        if (targetSoldier.IsSelf(originalTarget)) //only pay xp if you hit correct target 
                        {
                            //paying xp for hit
                            if (actingHitChance < 10)
                                MenuManager.Instance.AddXpAlert(shooter, 10, $"Shot <10% hit on {targetSoldier.soldierName}!", false);
                            else
                                MenuManager.Instance.AddXpAlert(shooter, 2, $"Shot hit on {targetSoldier.soldierName}.", false);
                        }
                    }
                }
                else
                {
                    string missString = GameManager.Instance.RandomShotMissString();

                    FileUtility.WriteToReport($"{shooter.soldierName} misses {targetSoldier.soldierName} ({actingHitChance}%|{chances.Item2}%), shot goes {missString}"); //write to report

                    //play shot miss dialogue
                    SoundManager.Instance.PlaySoldierShotMiss(shooter);

                    //set sound flags after ally misses shot
                    foreach (Soldier s in GameManager.Instance.AllSoldiers())
                    {
                        if (s.IsSameTeamAs(shooter))
                            SoundManager.Instance.SetSoldierSelectionSoundFlagAfterAllyMissesShot(s);
                    }

                    //set sound flags after enemy misses shot
                    SoundManager.Instance.SetSoldierSelectionSoundFlagAfterEnemyMissesShot(targetSoldier);

                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = "Miss";
                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Missed by {missString}.\n\nDamage event ({gun.damage}) on alternate target, or cover damage {gun.DisplayGunCoverDamage()}.";
                    //show los check button if shot misses
                    MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("LosCheck").gameObject.SetActive(true);

                    //show avenger retry if opponent has killed
                    if (shooter.IsAvenger() && targetSoldier.hasKilled && gun.CheckAnyAmmo() && !retry && !targetSoldier.IsRevoker())
                        MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("AvengerRetry").gameObject.SetActive(true);

                    //paying xp for dodge
                    if (actingHitChance > 90)
                        MenuManager.Instance.AddXpAlert(targetSoldier, 10, $"Shot >90% dodged from {shooter.soldierName}!", false);
                    else
                        MenuManager.Instance.AddXpAlert(targetSoldier, 1, $"Shot dodged from {shooter.soldierName}.", false);

                    //push a zero damage attack through for abilities trigger
                    targetSoldier.TakeDamage(shooter, 0, true, new() { "Shot" }, Vector3.zero);
                }
            }
        }
        else if (shotTypeDropdown.value == 1) //supression shot
        {
            FileUtility.WriteToReport($"{shooter.soldierName} suppresses {(target as Soldier).soldierName}"); //write to report

            //play suppression sfx
            SoundManager.Instance.PlaySuppressionResolution(gun);

            //play suppression dialogue
            SoundManager.Instance.PlaySoldierSuppressEnemy(shooter);

            gun.SpendSpecificAmmo(gun.suppressDrain, true);
            int suppressionValue = GameManager.Instance.CalculateRangeBracket(GameManager.Instance.CalculateRange(shooter, target as PhysicalObject)) switch
            {
                "Melee" or "CQB" => gun.cQBSupPen,
                "Short" => gun.shortSupPen,
                "Medium" => gun.medSupPen,
                "Long" => gun.longSupPen,
                "Coriolis" => gun.corSupPen,
                _ => 0,
            };
            (target as Soldier).SetSuppression(suppressionValue);
            MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("Result").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"<color=green> Supressing ({suppressionValue})</color>";
            MenuManager.Instance.shotResultUI.transform.Find("OptionPanel").Find("ScatterResult").Find("ResultDisplay").GetComponent<TextMeshProUGUI>().text = $"Suppressing {(target as Soldier).soldierName} until next round.";
        }

        //trigger loud action
        if (gun.HasSuppressorAttached()) //suppressor 
            shooter.PerformLoudAction(20);
        else
            shooter.PerformLoudAction();

        MenuManager.Instance.StartCoroutine(OpenShotResultUI(runSecondShot));
        CloseShotUI();
    }


    public string DisplayShotParameters()
    {
        List<string> colouredParameters = new();
        foreach (Tuple<string, string> param in shotParameters)
        {

            if (param.Item1 == "accuracy" || param.Item1 == "sharpshooter" || param.Item1 == "inspired" || param.Item1 == "WS" || param.Item1 == "stim" || param.Item1 == "juggernaut" || param.Item1 == "fight")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else if (param.Item1 == "tE" || param.Item1 == "suppression")
            {
                if (float.Parse(param.Item2) > 0)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else if (float.Parse(param.Item2) < 0)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
            else
            {
                if (float.Parse(param.Item2) > 1)
                    colouredParameters.Add($"<color=green>{param}</color>");
                else if (float.Parse(param.Item2) < 1)
                    colouredParameters.Add($"<color=red>{param}</color>");
                else
                    colouredParameters.Add($"{param}");
            }
        }

        return $"{colouredParameters.Find(str => str.Contains("accuracy"))} " +
                $"| {colouredParameters.Find(str => str.Contains("sharpshooter"))} " +
                $"| {colouredParameters.Find(str => str.Contains("inspired"))} " +
                $"| {colouredParameters.Find(str => str.Contains("WS"))} " +
                $"| {colouredParameters.Find(str => str.Contains("juggernaut"))} " +
                $"| {colouredParameters.Find(str => str.Contains("stim"))} " +
                $"| {colouredParameters.Find(str => str.Contains("trauma"))} " +
                $"| {colouredParameters.Find(str => str.Contains("sustenance"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tE"))} " +
                $"| {colouredParameters.Find(str => str.Contains("cover"))} " +
                $"| {colouredParameters.Find(str => str.Contains("vis"))} " +
                $"| {colouredParameters.Find(str => str.Contains("rain"))} " +
                $"| {colouredParameters.Find(str => str.Contains("wind"))} " +
                $"| {colouredParameters.Find(str => str.Contains("HP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tHP"))} " +
                $"| {colouredParameters.Find(str => str.Contains("Ter"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tTer"))} " +
                $"| {colouredParameters.Find(str => str.Contains("elevation"))} " +
                $"| {colouredParameters.Find(str => str.Contains("kd"))} " +
                $"| {colouredParameters.Find(str => str.Contains("overwatch"))} " +
                $"| {colouredParameters.Find(str => str.Contains("flank"))} " +
                $"| {colouredParameters.Find(str => str.Contains("stealth"))} " +
                $"| {colouredParameters.Find(str => str.Contains("smoke"))} " +
                $"| {colouredParameters.Find(str => str.Contains("tabun"))} " +
                $"| {colouredParameters.Find(str => str.Contains("suppression"))} " +
                $"| {colouredParameters.Find(str => str.Contains("fight"))}";
    }
    public void SetShotResolvedFlagTo(bool value)
    {
        if (value)
            MenuManager.Instance.UnfreezeTimer();
        else
            MenuManager.Instance.FreezeTimer();

        MenuManager.Instance.shotResolvedFlag = value;
    }
}
