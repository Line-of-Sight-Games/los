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
        coverman = FindFirstObjectByType<Coverman>();
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
        if (menu.ValidateIntInput(XPos, out int x) && menu.ValidateIntInput(YPos, out int y) && menu.ValidateIntInput(ZPos, out int z))
        {
            coverLocation = new Vector3(x, y, z);
            return true;
        }
        return false;
    }
}
