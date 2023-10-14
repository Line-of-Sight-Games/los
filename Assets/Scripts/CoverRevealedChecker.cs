using TMPro;
using UnityEngine;

public class CoverRevealedChecker : MonoBehaviour
{
    public MainMenu menu;
    public TMP_InputField XPos, YPos, ZPos;
    public Coverman coverman;
    public GameObject coverNotRevealed;
    public TextMeshProUGUI shooterId;

    private void Start()
    {
        coverman = FindObjectOfType<Coverman>();
    }
    private void Update()
    {
        if (GetCoverLocation(out Vector3 coverLocation))
        {
            coverman.SetCovermanLocation(coverLocation);
            if (menu.soldierManager.FindSoldierById(shooterId.text).PhysicalObjectIsRevealed(coverman))
                coverNotRevealed.SetActive(false);
            else
                coverNotRevealed.SetActive(true);
        }
        else
            coverNotRevealed.SetActive(true);
    }
    public bool GetCoverLocation(out Vector3 coverLocation)
    {
        coverLocation = default;
        if (XPos.textComponent.GetComponent<TextMeshProUGUI>().color == menu.normalTextColour && YPos.textComponent.GetComponent<TextMeshProUGUI>().color == menu.normalTextColour && ZPos.textComponent.GetComponent<TextMeshProUGUI>().color == menu.normalTextColour)
        {
            coverLocation = new Vector3(int.Parse(XPos.text), int.Parse(YPos.text), int.Parse(ZPos.text));
            return true;
        }
        return false;
    }
}
