using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public MainGame game;

    //audio stuff
    public AudioSource audioSource;
    public AudioClip banzai, overrideAlarm, detectionAlarm, buttonPress, levelUp, overmoveAlarm, gameOverMusic, failAbilityUpgrade, succeedAbilityUpgrade, newAbilityUpgrade;
    public AudioClip meleeCounter, meleeBreakeven, meleeSuccessStatic, meleeSuccessCharge;
    public AudioClip[] commanderSelectionGeneric;
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
        print($"Playing sound {clip.name}");
        audioSource.PlayOneShot(clip);

        // Wait for the clip to finish
        yield return new WaitForSeconds(clip.length);

        // Remove the sound from the tracking dictionary after it finishes
        print($"Closing sound {clip.name}");
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







    //soldier voices
    public void PlayRandomVoice(AudioClip[] audioClipArray)
    {
        if (game.CoinFlip()) //50% chance of silence
        {
            PlaySound(commanderSelectionGeneric[Random.Range(0, commanderSelectionGeneric.Length)]);
        }
    }
    public void PlaySoldierSelectedGeneric(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(commanderSelectionGeneric);
    }
}
