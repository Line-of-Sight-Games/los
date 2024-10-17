using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public MainGame game;
    public bool banzaiPlayed, isMute;

    //audio stuff
    public AudioSource audioSource;
    private Dictionary<AudioClip, Coroutine> playingSounds = new();

    //meta sfx
    public AudioClip banzai, overrideAlarm, detectionAlarm, buttonPress, levelUp, overmoveAlarm, gameOverMusic, failAbilityUpgrade, succeedAbilityUpgrade, newAbilityUpgrade;

    //game sfx
    public AudioClip explosion;
    public AudioClip meleeCounter, meleeBreakeven, meleeSuccessStatic, meleeSuccessCharge;
    public AudioClip placeClaymore;
    public AudioClip dipelecFail, dipelecHacking, dipelecNegotiating, dipelecSuccessL1, dipelecSuccessL2, dipelecSuccessL3, dipelecSuccessL4;
    public AudioClip shotAR, shotLMG, shotPi, shotRi, shotSh, shotSMG, shotSn, shotSuppressLMG_SMG_AR, shotSuppressPi_Ri_Sn_Sh, shotSilencedLMG_SMG_AR, shotSilencedPi_Ri_Sn_Sh, coverDestruction, reloadAR, reloadLMG, reloadPi, reloadRi, reloadSh, reloadSMG, reloadSn;
    public AudioClip fallFromHeight, structuralCollapse;
    public AudioClip equipArmour, equipWearableGear, configureGeneral;
    public AudioClip itemUseFrag, itemUseFlash, itemUseSmoke, itemUseTabun;
    public AudioClip itemUseDepBeacon, itemUseETool, itemUseFood, itemUseMedikit, itemUsePoisonSatchel, itemUseSyringe, itemUseWater, itemUsePoisonedItem;

    //dialogue
    public AudioClip[] commanderConfirmMove, spartanConfirmMove, survivorConfirmMove, runnerConfirmMove, evaderConfirmMove, reservistConfirmMove, seekerConfirmMove, chameleonConfirmMove, scoutConfirmMove, infantrymanConfirmMove, operatorConfirmMove, earthquakeConfirmMove, hunterConfirmMove, cycloneConfirmMove, hammerConfirmMove, wolfConfirmMove, herculesConfirmMove, diplomatConfirmMove, technicianConfirmMove, medicConfirmMove;
    public AudioClip[] commanderConfigNearGB, spartanConfigNearGB, survivorConfigNearGB, runnerConfigNearGB, evaderConfigNearGB, reservistConfigNearGB, seekerConfigNearGB, chameleonConfigNearGB, scoutConfigNearGB, infantrymanConfigNearGB, operatorConfigNearGB, earthquakeConfigNearGB, hunterConfigNearGB, cycloneConfigNearGB, hammerConfigNearGB, wolfConfigNearGB, herculesConfigNearGB, diplomatConfigNearGB, technicianConfigNearGB, medicConfigNearGB;
    public AudioClip[] commanderDetectClaymore, spartanDetectClaymore, survivorDetectClaymore, runnerDetectClaymore, evaderDetectClaymore, reservistDetectClaymore, seekerDetectClaymore, chameleonDetectClaymore, scoutDetectClaymore, infantrymanDetectClaymore, operatorDetectClaymore, earthquakeDetectClaymore, hunterDetectClaymore, cycloneDetectClaymore, hammerDetectClaymore, wolfDetectClaymore, herculesDetectClaymore, diplomatDetectClaymore, technicianDetectClaymore, medicDetectClaymore;
    public AudioClip[] commanderEnterOverwatch, spartanEnterOverwatch, survivorEnterOverwatch, runnerEnterOverwatch, evaderEnterOverwatch, reservistEnterOverwatch, seekerEnterOverwatch, chameleonEnterOverwatch, scoutEnterOverwatch, infantrymanEnterOverwatch, operatorEnterOverwatch, earthquakeEnterOverwatch, hunterEnterOverwatch, cycloneEnterOverwatch, hammerEnterOverwatch, wolfEnterOverwatch, herculesEnterOverwatch, diplomatEnterOverwatch, technicianEnterOverwatch, medicEnterOverwatch;
    public AudioClip[] commanderHealAlly, spartanHealAlly, survivorHealAlly, runnerHealAlly, evaderHealAlly, reservistHealAlly, seekerHealAlly, chameleonHealAlly, scoutHealAlly, infantrymanHealAlly, operatorHealAlly, earthquakeHealAlly, hunterHealAlly, cycloneHealAlly, hammerHealAlly, wolfHealAlly, herculesHealAlly, diplomatHealAlly, technicianHealAlly, medicHealAlly;
    public AudioClip[] commanderEquipArmour, spartanEquipArmour, survivorEquipArmour, runnerEquipArmour, evaderEquipArmour, reservistEquipArmour, seekerEquipArmour, chameleonEquipArmour, scoutEquipArmour, infantrymanEquipArmour, operatorEquipArmour, earthquakeEquipArmour, hunterEquipArmour, cycloneEquipArmour, hammerEquipArmour, wolfEquipArmour, herculesEquipArmour, diplomatEquipArmour, technicianEquipArmour, medicEquipArmour;
    public AudioClip[] commanderPlaceClaymore, spartanPlaceClaymore, survivorPlaceClaymore, runnerPlaceClaymore, evaderPlaceClaymore, reservistPlaceClaymore, seekerPlaceClaymore, chameleonPlaceClaymore, scoutPlaceClaymore, infantrymanPlaceClaymore, operatorPlaceClaymore, earthquakePlaceClaymore, hunterPlaceClaymore, cyclonePlaceClaymore, hammerPlaceClaymore, wolfPlaceClaymore, herculesPlaceClaymore, diplomatPlaceClaymore, technicianPlaceClaymore, medicPlaceClaymore;
    public AudioClip[] commanderUseGrenade, spartanUseGrenade, survivorUseGrenade, runnerUseGrenade, evaderUseGrenade, reservistUseGrenade, seekerUseGrenade, chameleonUseGrenade, scoutUseGrenade, infantrymanUseGrenade, operatorUseGrenade, earthquakeUseGrenade, hunterUseGrenade, cycloneUseGrenade, hammerUseGrenade, wolfUseGrenade, herculesUseGrenade, diplomatUseGrenade, technicianUseGrenade, medicUseGrenade;
    public AudioClip[] commanderUseTabun, spartanUseTabun, survivorUseTabun, runnerUseTabun, evaderUseTabun, reservistUseTabun, seekerUseTabun, chameleonUseTabun, scoutUseTabun, infantrymanUseTabun, operatorUseTabun, earthquakeUseTabun, hunterUseTabun, cycloneUseTabun, hammerUseTabun, wolfUseTabun, herculesUseTabun, diplomatUseTabun, technicianUseTabun, medicUseTabun;
    public AudioClip[] commanderUseSmoke, spartanUseSmoke, survivorUseSmoke, runnerUseSmoke, evaderUseSmoke, reservistUseSmoke, seekerUseSmoke, chameleonUseSmoke, scoutUseSmoke, infantrymanUseSmoke, operatorUseSmoke, earthquakeUseSmoke, hunterUseSmoke, cycloneUseSmoke, hammerUseSmoke, wolfUseSmoke, herculesUseSmoke, diplomatUseSmoke, technicianUseSmoke, medicUseSmoke;
    public AudioClip[] commanderKillEnemy, spartanKillEnemy, survivorKillEnemy, runnerKillEnemy, evaderKillEnemy, reservistKillEnemy, seekerKillEnemy, chameleonKillEnemy, scoutKillEnemy, infantrymanKillEnemy, operatorKillEnemy, earthquakeKillEnemy, hunterKillEnemy, cycloneKillEnemy, hammerKillEnemy, wolfKillEnemy, herculesKillEnemy, diplomatKillEnemy, technicianKillEnemy, medicKillEnemy;
    public AudioClip[] commanderMeleeBreakeven, spartanMeleeBreakeven, survivorMeleeBreakeven, runnerMeleeBreakeven, evaderMeleeBreakeven, reservistMeleeBreakeven, seekerMeleeBreakeven, chameleonMeleeBreakeven, scoutMeleeBreakeven, infantrymanMeleeBreakeven, operatorMeleeBreakeven, earthquakeMeleeBreakeven, hunterMeleeBreakeven, cycloneMeleeBreakeven, hammerMeleeBreakeven, wolfMeleeBreakeven, herculesMeleeBreakeven, diplomatMeleeBreakeven, technicianMeleeBreakeven, medicMeleeBreakeven;
    public AudioClip[] commanderMeleeMove, spartanMeleeMove, survivorMeleeMove, runnerMeleeMove, evaderMeleeMove, reservistMeleeMove, seekerMeleeMove, chameleonMeleeMove, scoutMeleeMove, infantrymanMeleeMove, operatorMeleeMove, earthquakeMeleeMove, hunterMeleeMove, cycloneMeleeMove, hammerMeleeMove, wolfMeleeMove, herculesMeleeMove, diplomatMeleeMove, technicianMeleeMove, medicMeleeMove;
    public AudioClip[] commanderShotMiss, spartanShotMiss, survivorShotMiss, runnerShotMiss, evaderShotMiss, reservistShotMiss, seekerShotMiss, chameleonShotMiss, scoutShotMiss, infantrymanShotMiss, operatorShotMiss, earthquakeShotMiss, hunterShotMiss, cycloneShotMiss, hammerShotMiss, wolfShotMiss, herculesShotMiss, diplomatShotMiss, technicianShotMiss, medicShotMiss;
    public AudioClip[] commanderPickupUHF, spartanPickupUHF, survivorPickupUHF, runnerPickupUHF, evaderPickupUHF, reservistPickupUHF, seekerPickupUHF, chameleonPickupUHF, scoutPickupUHF, infantrymanPickupUHF, operatorPickupUHF, earthquakePickupUHF, hunterPickupUHF, cyclonePickupUHF, hammerPickupUHF, wolfPickupUHF, herculesPickupUHF, diplomatPickupUHF, technicianPickupUHF, medicPickupUHF;
    public AudioClip[] commanderPickupULF, spartanPickupULF, survivorPickupULF, runnerPickupULF, evaderPickupULF, reservistPickupULF, seekerPickupULF, chameleonPickupULF, scoutPickupULF, infantrymanPickupULF, operatorPickupULF, earthquakePickupULF, hunterPickupULF, cyclonePickupULF, hammerPickupULF, wolfPickupULF, herculesPickupULF, diplomatPickupULF, technicianPickupULF, medicPickupULF;
    public AudioClip[] commanderSeeEnemy, spartanSeeEnemy, survivorSeeEnemy, runnerSeeEnemy, evaderSeeEnemy, reservistSeeEnemy, seekerSeeEnemy, chameleonSeeEnemy, scoutSeeEnemy, infantrymanSeeEnemy, operatorSeeEnemy, earthquakeSeeEnemy, hunterSeeEnemy, cycloneSeeEnemy, hammerSeeEnemy, wolfSeeEnemy, herculesSeeEnemy, diplomatSeeEnemy, technicianSeeEnemy, medicSeeEnemy;
    public AudioClip[] commanderSuppressEnemy, spartanSuppressEnemy, survivorSuppressEnemy, runnerSuppressEnemy, evaderSuppressEnemy, reservistSuppressEnemy, seekerSuppressEnemy, chameleonSuppressEnemy, scoutSuppressEnemy, infantrymanSuppressEnemy, operatorSuppressEnemy, earthquakeSuppressEnemy, hunterSuppressEnemy, cycloneSuppressEnemy, hammerSuppressEnemy, wolfSuppressEnemy, herculesSuppressEnemy, diplomatSuppressEnemy, technicianSuppressEnemy, medicSuppressEnemy;
    public AudioClip[] ulfFail, ulfSuccess, uhfDialUp, uhfMiss, uhfHit, uhfDirectHit;

    public List<string> soldierTakenDamage, soldierAllyKilledJA, soldierAllyKilledOrUncon;
    public AudioClip[] commanderSelectionAfterDamage, spartanSelectionAfterDamage, survivorSelectionAfterDamage, runnerSelectionAfterDamage, evaderSelectionAfterDamage, reservistSelectionAfterDamage, seekerSelectionAfterDamage, chameleonSelectionAfterDamage, scoutSelectionAfterDamage, infantrymanSelectionAfterDamage, operatorSelectionAfterDamage, earthquakeSelectionAfterDamage, hunterSelectionAfterDamage, cycloneSelectionAfterDamage, hammerSelectionAfterDamage, wolfSelectionAfterDamage, herculesSelectionAfterDamage, diplomatSelectionAfterDamage, technicianSelectionAfterDamage, medicSelectionAfterDamage;
    public AudioClip[] commanderSelectionAfterAllyKilledJA, spartanSelectionAfterAllyKilledJA, survivorSelectionAfterAllyKilledJA, runnerSelectionAfterAllyKilledJA, evaderSelectionAfterAllyKilledJA, reservistSelectionAfterAllyKilledJA, seekerSelectionAfterAllyKilledJA, chameleonSelectionAfterAllyKilledJA, scoutSelectionAfterAllyKilledJA, infantrymanSelectionAfterAllyKilledJA, operatorSelectionAfterAllyKilledJA, earthquakeSelectionAfterAllyKilledJA, hunterSelectionAfterAllyKilledJA, cycloneSelectionAfterAllyKilledJA, hammerSelectionAfterAllyKilledJA, wolfSelectionAfterAllyKilledJA, herculesSelectionAfterAllyKilledJA, diplomatSelectionAfterAllyKilledJA, technicianSelectionAfterAllyKilledJA, medicSelectionAfterAllyKilledJA;
    public AudioClip[] commanderSelectionAfterAllyKilledOrUncon, spartanSelectionAfterAllyKilledOrUncon, survivorSelectionAfterAllyKilledOrUncon, runnerSelectionAfterAllyKilledOrUncon, evaderSelectionAfterAllyKilledOrUncon, reservistSelectionAfterAllyKilledOrUncon, seekerSelectionAfterAllyKilledOrUncon, chameleonSelectionAfterAllyKilledOrUncon, scoutSelectionAfterAllyKilledOrUncon, infantrymanSelectionAfterAllyKilledOrUncon, operatorSelectionAfterAllyKilledOrUncon, earthquakeSelectionAfterAllyKilledOrUncon, hunterSelectionAfterAllyKilledOrUncon, cycloneSelectionAfterAllyKilledOrUncon, hammerSelectionAfterAllyKilledOrUncon, wolfSelectionAfterAllyKilledOrUncon, herculesSelectionAfterAllyKilledOrUncon, diplomatSelectionAfterAllyKilledOrUncon, technicianSelectionAfterAllyKilledOrUncon, medicSelectionAfterAllyKilledOrUncon;
    public AudioClip[] commanderSelectionGeneric, spartanSelectionGeneric, survivorSelectionGeneric, runnerSelectionGeneric, evaderSelectionGeneric, reservistSelectionGeneric, seekerSelectionGeneric, chameleonSelectionGeneric, scoutSelectionGeneric, infantrymanSelectionGeneric, operatorSelectionGeneric, earthquakeSelectionGeneric, hunterSelectionGeneric, cycloneSelectionGeneric, hammerSelectionGeneric, wolfSelectionGeneric, herculesSelectionGeneric, diplomatSelectionGeneric, technicianSelectionGeneric, medicSelectionGeneric;

    //basic sound functions
    public void PlaySound(AudioClip clip)
    {
        if (!isMute)
        {
            // Check if the sound is already playing
            if (!playingSounds.ContainsKey(clip))
            {
                // Start the coroutine to play the sound and track it
                Coroutine soundCoroutine = StartCoroutine(PlayAndTrackSound(clip));
                playingSounds.Add(clip, soundCoroutine);
            }
        }
    }
    private IEnumerator PlayAndTrackSound(AudioClip clip)
    {
        // Play the sound using PlayOneShot
        print($"Playing sound \"{clip.name}\"");
        audioSource.PlayOneShot(clip);

        // Wait for the clip to finish
        yield return new WaitForSeconds(clip.length);

        // Remove the sound from the tracking dictionary after it finishes
        print($"Closing sound \"{clip.name}\"");
        playingSounds.Remove(clip);
    }







    //meta sfx functions
    public void PlayBanzai()
    {
        if (!banzaiPlayed)
        {
            PlaySound(banzai);
            banzaiPlayed = true;
        }
    }
    public void PlayOverrideAlarm()
    {
        PlaySound(overrideAlarm);
    }
    public void PlayDetectionAlarm()
    {
        PlaySound(detectionAlarm);
    }
    public void PlayButtonPress()
    {
        PlaySound(buttonPress);
    }
    public void PlayPromotion()
    {
        PlaySound(levelUp);
    }
    public void PlayOvermoveAlarm()
    {
        PlaySound(overmoveAlarm);
    }
    public void PlayGameOverMusic()
    {
        PlaySound(gameOverMusic);
    }
    public void PlayFailedUpgrade()
    {
        PlaySound(failAbilityUpgrade);
    }
    public void PlaySucceededUpgrade()
    {
        PlaySound(succeedAbilityUpgrade);
    }
    public void PlayNewAbility()
    {
        PlaySound(newAbilityUpgrade);
    }





    //game sfx functions
    public void PlayExplosion()
    {
        PlaySound(explosion);
    }
    public void PlayMeleeResolution(string result)
    {
        if (result.Equals("counter"))
            PlaySound(meleeCounter);
        else if (result.Equals("breakeven"))
            PlaySound(meleeBreakeven);
        else if (result.Equals("successStatic"))
            PlaySound(meleeSuccessStatic);
        else if (result.Equals("successCharge"))
            PlaySound(meleeSuccessCharge);
    }
    public void PlayPlaceClaymore()
    {
        PlaySound(placeClaymore);
    }
    public void PlayDipelecResolution(string result)
    {
        if (result.Contains("Hack"))
            PlaySound(dipelecHacking);
        else if (result.Contains("Negotiation"))
            PlaySound(dipelecNegotiating);

        if (result.Contains("Fail"))
            PlaySound(dipelecFail);
        else if (result.Contains("Success"))
        {
            if (result.Contains("L1"))
                PlaySound(dipelecSuccessL1);
            else if (result.Contains("L2"))
                PlaySound(dipelecSuccessL2);
            else if (result.Contains("L3"))
                PlaySound(dipelecSuccessL3);
            else if (result.Contains("L4"))
                PlaySound(dipelecSuccessL4);
        }
    }
    public void PlayShotResolution(Item gun)
    {
        if (gun.IsAssaultRifle())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotAR);
        }
        else if (gun.IsLMG())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotLMG);
        }
        else if (gun.IsPistol())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotPi);
        }
        else if (gun.IsRifle())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotRi);
        }
        else if (gun.IsShotgun())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotSh);
        }
        else if (gun.IsSMG())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotSMG);
        }
        else if (gun.IsSniper())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotSn);
        }
    }
    public void PlaySuppressionResolution(Item gun)
    {
        if (gun.IsLMG() || gun.IsSMG() || gun.IsAssaultRifle())
            PlaySound(shotSuppressLMG_SMG_AR);
        else if (gun.IsPistol() || gun.IsRifle() || gun.IsSniper() || gun.IsShotgun())
            PlaySound(shotSuppressPi_Ri_Sn_Sh);
    }
    public void PlayReload(Item gun)
    {
        if (gun.IsAssaultRifle())
            PlaySound(reloadAR);
        else if (gun.IsLMG())
            PlaySound(reloadLMG);
        else if (gun.IsPistol())
            PlaySound(reloadPi);
        else if (gun.IsRifle())
            PlaySound(reloadRi);
        else if (gun.IsShotgun())
            PlaySound(reloadSh);
        else if (gun.IsSMG())
            PlaySound(reloadSMG);
        else if (gun.IsSniper())
            PlaySound(reloadSn);
    }
    public void PlayCoverDestruction()
    {
        PlaySound(coverDestruction);
    }
    public void PlayFallFromHeight()
    {
        PlaySound(fallFromHeight);
    }
    public void PlayStructuralCollapse()
    {
        PlaySound(structuralCollapse);
    }
    public void PlayEquipArmour()
    {
        PlaySound(equipArmour);
    }
    public void PlayEquipWearableGear()
    {
        PlaySound(equipWearableGear);
    }
    public void PlayConfigGeneral()
    {
        PlaySound(configureGeneral);
    }
    public void PlayUseGrenade(Item grenade)
    {
        if (grenade.IsFrag())
            PlaySound(itemUseFrag);
        else if (grenade.IsFlashbang())
            PlaySound(itemUseFlash);
        else if (grenade.IsSmoke())
            PlaySound(itemUseSmoke);
        else if (grenade.IsTabun())
            PlaySound(itemUseTabun);
    }
    public void PlayUseDepBeacon()
    {
        PlaySound(itemUseDepBeacon);
    }
    public void PlayUseETool()
    {
        PlaySound(itemUseETool);
    }
    public void PlayUseFood()
    {
        PlaySound(itemUseFood);
    }
    public void PlayUseMedikit()
    {
        PlaySound(itemUseMedikit);
    }
    public void PlayUsePoisonSatchel()
    {
        PlaySound(itemUsePoisonSatchel);
    }
    public void PlayUseSyringe()
    {
        PlaySound(itemUseSyringe);
    }
    public void PlayUseWater()
    {
        PlaySound(itemUseWater);
    }
    public void PlayUsePoisonedItem()
    {
        PlaySound(itemUsePoisonedItem);
    }









    //soldier voices
    public void PlayRandomVoice(AudioClip[] audioClipArray)
    {
        if (audioClipArray.Any())
        {
            if (game.CoinFlip()) //50% chance of silence
            {
                PlaySound(audioClipArray[HelperFunctions.RandomNumber(0, audioClipArray.Length - 1)]);
            }
        }
    }
    public void PlaySoldierConfigNearGB(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderConfigNearGB);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanConfigNearGB);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorConfigNearGB);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerConfigNearGB);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderConfigNearGB);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistConfigNearGB);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerConfigNearGB);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerConfigNearGB);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutConfigNearGB);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanConfigNearGB);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorConfigNearGB);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeConfigNearGB);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterConfigNearGB);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneConfigNearGB);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerConfigNearGB);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfConfigNearGB);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesConfigNearGB);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatConfigNearGB);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianConfigNearGB);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicConfigNearGB);
    }
    public void PlaySoldierConfirmMove(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderConfirmMove);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanConfirmMove);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorConfirmMove);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerConfirmMove);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderConfirmMove);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistConfirmMove);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerConfirmMove);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerConfirmMove);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutConfirmMove);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanConfirmMove);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorConfirmMove);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeConfirmMove);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterConfirmMove);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneConfirmMove);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerConfirmMove);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfConfirmMove);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesConfirmMove);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatConfirmMove);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianConfirmMove);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicConfirmMove);
    }
    public void PlaySoldierDetectClaymore(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderDetectClaymore);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanDetectClaymore);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorDetectClaymore);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerDetectClaymore);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderDetectClaymore);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistDetectClaymore);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerDetectClaymore);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerDetectClaymore);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutDetectClaymore);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanDetectClaymore);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorDetectClaymore);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeDetectClaymore);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterDetectClaymore);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneDetectClaymore);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerDetectClaymore);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfDetectClaymore);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesDetectClaymore);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatDetectClaymore);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianDetectClaymore);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicDetectClaymore);
    }
    public void PlaySoldierEnterOverwatch(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderEnterOverwatch);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanEnterOverwatch);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorEnterOverwatch);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerEnterOverwatch);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderEnterOverwatch);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistEnterOverwatch);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerEnterOverwatch);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerEnterOverwatch);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutEnterOverwatch);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanEnterOverwatch);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorEnterOverwatch);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeEnterOverwatch);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterEnterOverwatch);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneEnterOverwatch);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerEnterOverwatch);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfEnterOverwatch);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesEnterOverwatch);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatEnterOverwatch);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianEnterOverwatch);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicEnterOverwatch);
    }
    public void PlaySoldierHealAlly(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderHealAlly);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanHealAlly);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorHealAlly);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerHealAlly);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderHealAlly);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistHealAlly);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerHealAlly);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerHealAlly);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutHealAlly);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanHealAlly);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorHealAlly);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeHealAlly);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterHealAlly);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneHealAlly);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerHealAlly);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfHealAlly);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesHealAlly);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatHealAlly);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianHealAlly);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicHealAlly);
    }
    public void PlaySoldierEquipArmour(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderEquipArmour);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanEquipArmour);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorEquipArmour);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerEquipArmour);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderEquipArmour);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistEquipArmour);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerEquipArmour);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerEquipArmour);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutEquipArmour);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanEquipArmour);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorEquipArmour);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeEquipArmour);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterEquipArmour);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneEquipArmour);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerEquipArmour);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfEquipArmour);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesEquipArmour);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatEquipArmour);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianEquipArmour);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicEquipArmour);
    }
    public void PlaySoldierPlaceClaymore(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderPlaceClaymore);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanPlaceClaymore);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorPlaceClaymore);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerPlaceClaymore);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderPlaceClaymore);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistPlaceClaymore);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerPlaceClaymore);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerPlaceClaymore);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutPlaceClaymore);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanPlaceClaymore);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorPlaceClaymore);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakePlaceClaymore);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterPlaceClaymore);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cyclonePlaceClaymore);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerPlaceClaymore);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfPlaceClaymore);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesPlaceClaymore);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatPlaceClaymore);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianPlaceClaymore);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicPlaceClaymore);
    }
    public void PlaySoldierUseGrenade(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderUseGrenade);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanUseGrenade);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorUseGrenade);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerUseGrenade);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderUseGrenade);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistUseGrenade);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerUseGrenade);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerUseGrenade);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutUseGrenade);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanUseGrenade);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorUseGrenade);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeUseGrenade);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterUseGrenade);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneUseGrenade);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerUseGrenade);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfUseGrenade);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesUseGrenade);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatUseGrenade);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianUseGrenade);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicUseGrenade);
    }
    public void PlaySoldierUseTabun(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderUseTabun);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanUseTabun);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorUseTabun);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerUseTabun);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderUseTabun);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistUseTabun);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerUseTabun);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerUseTabun);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutUseTabun);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanUseTabun);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorUseTabun);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeUseTabun);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterUseTabun);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneUseTabun);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerUseTabun);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfUseTabun);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesUseTabun);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatUseTabun);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianUseTabun);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicUseTabun);
    }
    public void PlaySoldierUseSmoke(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderUseSmoke);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanUseSmoke);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorUseSmoke);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerUseSmoke);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderUseSmoke);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistUseSmoke);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerUseSmoke);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerUseSmoke);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutUseSmoke);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanUseSmoke);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorUseSmoke);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeUseSmoke);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterUseSmoke);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneUseSmoke);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerUseSmoke);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfUseSmoke);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesUseSmoke);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatUseSmoke);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianUseSmoke);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicUseSmoke);
    }
    public void PlaySoldierKillEnemy(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderKillEnemy);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanKillEnemy);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorKillEnemy);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerKillEnemy);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderKillEnemy);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistKillEnemy);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerKillEnemy);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerKillEnemy);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutKillEnemy);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanKillEnemy);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorKillEnemy);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeKillEnemy);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterKillEnemy);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneKillEnemy);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerKillEnemy);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfKillEnemy);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesKillEnemy);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatKillEnemy);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianKillEnemy);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicKillEnemy);
    }
    public void PlaySoldierMeleeBreakeven(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderMeleeBreakeven);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanMeleeBreakeven);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorMeleeBreakeven);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerMeleeBreakeven);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderMeleeBreakeven);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistMeleeBreakeven);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerMeleeBreakeven);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerMeleeBreakeven);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutMeleeBreakeven);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanMeleeBreakeven);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorMeleeBreakeven);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeMeleeBreakeven);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterMeleeBreakeven);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneMeleeBreakeven);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerMeleeBreakeven);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfMeleeBreakeven);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesMeleeBreakeven);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatMeleeBreakeven);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianMeleeBreakeven);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicMeleeBreakeven);
    }
    public void PlaySoldierMeleeMove(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderMeleeMove);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanMeleeMove);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorMeleeMove);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerMeleeMove);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderMeleeMove);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistMeleeMove);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerMeleeMove);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerMeleeMove);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutMeleeMove);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanMeleeMove);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorMeleeMove);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeMeleeMove);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterMeleeMove);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneMeleeMove);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerMeleeMove);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfMeleeMove);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesMeleeMove);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatMeleeMove);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianMeleeMove);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicMeleeMove);
    }
    public void PlaySoldierShotMiss(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderShotMiss);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanShotMiss);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorShotMiss);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerShotMiss);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderShotMiss);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistShotMiss);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerShotMiss);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerShotMiss);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutShotMiss);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanShotMiss);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorShotMiss);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeShotMiss);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterShotMiss);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneShotMiss);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerShotMiss);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfShotMiss);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesShotMiss);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatShotMiss);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianShotMiss);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicShotMiss);
    }
    public void PlaySoldierPickupUHF(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderPickupUHF);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanPickupUHF);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorPickupUHF);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerPickupUHF);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderPickupUHF);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistPickupUHF);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerPickupUHF);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerPickupUHF);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutPickupUHF);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanPickupUHF);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorPickupUHF);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakePickupUHF);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterPickupUHF);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cyclonePickupUHF);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerPickupUHF);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfPickupUHF);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesPickupUHF);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatPickupUHF);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianPickupUHF);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicPickupUHF);
    }
    public void PlaySoldierPickupULF(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderPickupULF);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanPickupULF);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorPickupULF);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerPickupULF);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderPickupULF);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistPickupULF);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerPickupULF);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerPickupULF);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutPickupULF);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanPickupULF);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorPickupULF);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakePickupULF);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterPickupULF);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cyclonePickupULF);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerPickupULF);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfPickupULF);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesPickupULF);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatPickupULF);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianPickupULF);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicPickupULF);
    }
    public void PlaySoldierSeeEnemy(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderSeeEnemy);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanSeeEnemy);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorSeeEnemy);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerSeeEnemy);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderSeeEnemy);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistSeeEnemy);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSeeEnemy);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerSeeEnemy);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutSeeEnemy);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanSeeEnemy);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSeeEnemy);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSeeEnemy);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSeeEnemy);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSeeEnemy);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerSeeEnemy);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfSeeEnemy);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesSeeEnemy);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSeeEnemy);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianSeeEnemy);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicSeeEnemy);
    }
    public void PlaySoldierSuppressEnemy(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderSuppressEnemy);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanSuppressEnemy);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorSuppressEnemy);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerSuppressEnemy);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderSuppressEnemy);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistSuppressEnemy);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSuppressEnemy);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerSuppressEnemy);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutSuppressEnemy);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanSuppressEnemy);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSuppressEnemy);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSuppressEnemy);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSuppressEnemy);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSuppressEnemy);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerSuppressEnemy);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfSuppressEnemy);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesSuppressEnemy);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSuppressEnemy);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianSuppressEnemy);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicSuppressEnemy);
    }
    public void PlayUHFDialUp()
    {
        PlaySound(uhfDialUp[HelperFunctions.RandomNumber(0, uhfDialUp.Length - 1)]);
    }
    public void PlayUHFResult(int roll)
    {
        if (roll == 6)
            PlaySound(uhfDirectHit[HelperFunctions.RandomNumber(0, uhfDirectHit.Length - 1)]);
        else if (roll == 1)
            PlaySound(uhfMiss[HelperFunctions.RandomNumber(0, uhfMiss.Length - 1)]);
        else
            PlaySound(uhfHit[HelperFunctions.RandomNumber(0, uhfHit.Length - 1)]);
    }
    public void PlayULFResult(string result)
    {
        if (result.Contains("Jamming") || result.Contains("Spying"))
            PlaySound(ulfSuccess[HelperFunctions.RandomNumber(0, ulfSuccess.Length - 1)]);
        else
            PlaySound(ulfFail[HelperFunctions.RandomNumber(0, ulfFail.Length - 1)]);
    }







    //soldier selection dialogue
    public void UnsetAllSoldierSelectionSoundFlags(Soldier soldier)
    {
        soldierTakenDamage.Remove(soldier.Id);
        soldierAllyKilledJA.Remove(soldier.Id);
        soldierAllyKilledOrUncon.Remove(soldier.Id);
    }
    public void SetSoundFlag(Soldier soldier, List<string> soldierList)
    {
        if (soldier.IsConscious())
        {
            if (!soldierList.Contains(soldier.Id))
                soldierList.Add(soldier.Id);
        }
    }
    public void SetSoldierSelectionSoundFlagAfterDamage(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierTakenDamage);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyKilledJA(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierAllyKilledJA);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyKilledOrUncon(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierAllyKilledOrUncon);
    }
    public void PlaySoldierSelection(Soldier soldier)
    {
        if (soldier.IsConscious())
        {
            if (soldierTakenDamage.Contains(soldier.Id))
                PlaySoldierSelectionAfterDamage(soldier);
            else if (soldierAllyKilledJA.Contains(soldier.Id))
                PlaySoldierSelectionAfterAllyKilledJA(soldier);
            else if (soldierAllyKilledOrUncon.Contains(soldier.Id))
                PlaySoldierSelectionAfterAllyKilledOrUncon(soldier);
            else
                PlaySoldierSelectionGeneric(soldier);

            
            
        }

        //unset other flags
        UnsetAllSoldierSelectionSoundFlags(soldier);
    }
    public void PlaySoldierSelectionAfterDamage(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterDamage);
    }
    public void PlaySoldierSelectionAfterAllyKilledJA(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterAllyKilledJA);
    }
    public void PlaySoldierSelectionAfterAllyKilledOrUncon(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterAllyKilledOrUncon);
    }
    public void PlaySoldierSelectionGeneric(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionGeneric);
    }
}
