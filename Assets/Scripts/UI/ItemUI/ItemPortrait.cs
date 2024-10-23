using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPortrait : MonoBehaviour
{
    public Sprite groundSprite;
    public void Init(Item item)
    {
        GetComponent<Image>().sprite = item.itemImage;
        if (item.IsNestedOnSoldier()) 
        {
            transform.Find("SoldierPortrait").gameObject.SetActive(true);
            transform.Find("SoldierPortrait").GetComponent<SoldierPortrait>().Init(item.SoldierNestedOn());
        }
        else if (item.owner is null)
        {
            transform.Find("GroundPortrait").gameObject.SetActive(true);
            transform.Find("GroundPortrait").Find("ItemLocation").GetComponent<TextMeshProUGUI>().text = $"X:{item.X} Y:{item.Y} Z:{item.Z}";
        }
    }
}
