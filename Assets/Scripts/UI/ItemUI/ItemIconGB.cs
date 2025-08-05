using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;
using UnityEditor;

public class ItemIconGB : MonoBehaviour
{
    public ItemAssets itemAssets;
    public Item linkedItem;
    public int pickupNumber = 0;
    public List<Sprite> arrows = new();
    public Transform destination;

    public void Start()
    {
        itemAssets = FindFirstObjectByType<ItemAssets>();
        transform.Find("ItemImage").GetComponent<Image>().sprite = itemAssets.GetSprite(this.gameObject.name);
        transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
        destination = null;
    }

    public ItemIconGB Init(string name, int amount, Item item)
    {
        itemAssets = FindFirstObjectByType<ItemAssets>();
        gameObject.name = name;
        pickupNumber = amount;
        linkedItem = item;
        transform.Find("ItemImage").GetComponent<Image>().sprite = itemAssets.GetSprite(this.gameObject.name);
        transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
        destination = null;

        return this;
    }

    public void PlayButtonPress()
    {
        SoundManager.Instance.PlayButtonPress();
    }
    public void Update()
    {
        //suppress ammo count for unspawned items
        transform.Find("AmountIcon").GetComponent<TextMeshProUGUI>().text = pickupNumber.ToString();

        if (pickupNumber > 0)
            transform.Find("Arrow").GetComponent<Image>().sprite = arrows[1];
        else
            transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
    }

    public void LeftClick()
    {
        PickupClicked();
    }

    public void RightClick()
    {
        PickupUnclicked();
    }
    public void PickupClicked()
    {
        if (pickupNumber < 99)
            pickupNumber++;
    }
    public void PickupUnclicked()
    {
        if (pickupNumber > 0)
            pickupNumber--;
    }
   
}
