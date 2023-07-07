using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public MainGame game;

    //audio stuff
    public AudioSource noisePlayer;
    public AudioClip banzai, overrideAlarm, detectionAlarm, buttonPress, levelUp, overmoveAlarm, gameOverMusic, failAbilityUpgrade, succeedAbilityUpgrade, newAbilityUpgrade;
    public bool banzaiPlayed;

    public void PlayBanzai()
    {
        if (!banzaiPlayed)
        {
            //Debug.Log("played banzai");
            noisePlayer.PlayOneShot(banzai);
            banzaiPlayed = true;
        }
    }
    public void PlayOverrideAlarm()
    {
        //Debug.Log("played override alarm");
        noisePlayer.PlayOneShot(overrideAlarm);
    }
    public void PlayDetectionAlarm()
    {
        //Debug.Log("played detection alarm");
        noisePlayer.PlayOneShot(detectionAlarm);
    }
    public void PlayButtonPress()
    {
        //Debug.Log("played button press");
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
}
