using UnityEngine;

public class CloudDissipationAlert : MonoBehaviour
{
    public void PlayButtonPress()
    {
        SoundManager.Instance.PlayButtonPress();
    }

    // Update is called once per frame
    public void CloseCloudDissipationAlert()
    {
        Destroy(this.gameObject);
    }
}
