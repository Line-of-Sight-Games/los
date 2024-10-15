using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public MainGame game;

    //audio stuff
    public AudioSource audioSource;
    public AudioClip banzai, overrideAlarm, detectionAlarm, buttonPress, levelUp, overmoveAlarm, gameOverMusic, failAbilityUpgrade, succeedAbilityUpgrade, newAbilityUpgrade;
    public AudioClip meleeCounter, meleeBreakeven, meleeSuccessStatic, meleeSuccessCharge;
    public AudioClip placeClaymore;
    public AudioClip[] commanderSelectionGeneric, spartanSelectionGeneric, survivorSelectionGeneric, runnerSelectionGeneric, evaderSelectionGeneric, reservistSelectionGeneric, seekerSelectionGeneric, chameleonSelectionGeneric, scoutSelectionGeneric, infantrymanSelectionGeneric, operatorSelectionGeneric, earthquakeSelectionGeneric, hunterSelectionGeneric, cycloneSelectionGeneric, hammerSelectionGeneric, wolfSelectionGeneric, herculesSelectionGeneric, diplomatSelectionGeneric, technicianSelectionGeneric, medicSelectionGeneric;
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




    public bool banzaiPlayed, isMute;

    private Dictionary<AudioClip, Coroutine> playingSounds = new();

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





    //fx functions
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
    public void PlaySoldierSelectedGeneric(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanSelectionGeneric);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionGeneric);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerSelectionGeneric);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionGeneric);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistSelectionGeneric);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionGeneric);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionGeneric);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionGeneric);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionGeneric);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionGeneric);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionGeneric);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionGeneric);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionGeneric);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionGeneric);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfSelectionGeneric);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesSelectionGeneric);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionGeneric);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionGeneric);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicSelectionGeneric);
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
}
