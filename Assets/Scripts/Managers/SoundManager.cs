using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public MainGame game;

    //audio stuff
    public AudioSource noisePlayer;
    public AudioClip banzai, overrideAlarm, detectionAlarm, buttonPress, levelUp, overmoveAlarm, gameOverMusic, failAbilityUpgrade, succeedAbilityUpgrade, newAbilityUpgrade;
    public AudioClip meleeCounter, meleeBreakeven, meleeSuccessStatic, meleeSuccessCharge;
    public bool banzaiPlayed;

    public void PlayBanzai()
    {
        if (!banzaiPlayed)
        {
            //print("played banzai");
            noisePlayer.PlayOneShot(banzai);
            banzaiPlayed = true;
        }
    }
    public void PlayOverrideAlarm()
    {
        //print("played override alarm");
        noisePlayer.PlayOneShot(overrideAlarm);
    }
    public void PlayDetectionAlarm()
    {
        //print("played detection alarm");
        noisePlayer.PlayOneShot(detectionAlarm);
    }
    public void PlayButtonPress()
    {
        //print("played button press");
        noisePlayer.PlayOneShot(buttonPress);
    }

    public void PlayPromotion()
    {
        noisePlayer.PlayOneShot(levelUp);
    }

    public void PlayOvermoveAlarm()
    {
        noisePlayer.PlayOneShot(overmoveAlarm);
    }

    public void PlayGameOverMusic()
    {
        noisePlayer.PlayOneShot(gameOverMusic);
    }

    public void PlayFailedUpgrade()
    {
        noisePlayer.PlayOneShot(failAbilityUpgrade);
    }
    public void PlaySucceededUpgrade()
    {
        noisePlayer.PlayOneShot(succeedAbilityUpgrade);
    }
    public void PlayNewAbility()
    {
        noisePlayer.PlayOneShot(newAbilityUpgrade);
    }
    public void PlayMeleeResolution(string result)
    {
        if (result.Equals("counter"))
            noisePlayer.PlayOneShot(meleeCounter);
        else if (result.Equals("breakeven"))
            noisePlayer.PlayOneShot(meleeBreakeven);
        else if (result.Equals("successStatic"))
            noisePlayer.PlayOneShot(meleeSuccessStatic);
        else if (result.Equals("successCharge"))
            noisePlayer.PlayOneShot(meleeSuccessCharge);
    }
}
