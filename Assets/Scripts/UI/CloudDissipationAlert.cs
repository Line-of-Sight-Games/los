using UnityEngine;

public class CloudDissipationAlert : MonoBehaviour
{
    public void PlayButtonPress()
    {
        FindFirstObjectByType<SoundManager>().PlayButtonPress();
    }

    // Update is called once per frame
    public void CloseCloudDissipationAlert()
    {
        Destroy(this.gameObject);
    }
}
