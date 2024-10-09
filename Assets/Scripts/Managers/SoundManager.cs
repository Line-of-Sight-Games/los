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
}
