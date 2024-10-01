using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public MainGame game;

    //audio stuff
    public AudioSource noisePlayer;
    public AudioClip banzai, overrideAlarm, detectionAlarm, buttonPress, levelUp, overmoveAlarm, gameOverMusic, failAbilityUpgrade, succeedAbilityUpgrade, newAbilityUpgrade;
    public AudioClip meleeCounter, meleeBreakeven, meleeSuccessStatic, meleeSuccessCharge;
    public bool banzaiPlayed, isPlaying;

    private float soundDuration = 0f;
    private float soundEndTime = 0f;

    public void PlaySound(AudioClip clip)
    {
        if (!isPlaying)
        {
            print($"Played sound {clip.name}, for {soundDuration}, ended at {soundEndTime}.");
            noisePlayer.PlayOneShot(clip);
            isPlaying = true;
            soundDuration = clip.length;
            soundEndTime = Time.time + soundDuration;
        }
    }

    void Update()
    {
        // Unlock if the sound duration has passed
        if (isPlaying && Time.time >= soundEndTime)
        {
            isPlaying = false;
        }
    }

    public void PlayBanzai()
    {
        if (!banzaiPlayed)
        {
            //print("played banzai");
            PlaySound(banzai);
            banzaiPlayed = true;
        }
    }
    public void PlayOverrideAlarm()
    {
        //print("played override alarm");
        PlaySound(overrideAlarm);
    }
    public void PlayDetectionAlarm()
    {
        //print("played detection alarm");
        PlaySound(detectionAlarm);
    }
    public void PlayButtonPress()
    {
        //print("played button press");
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
}
