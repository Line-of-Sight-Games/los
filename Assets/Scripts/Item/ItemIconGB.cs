using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;
using UnityEditor;

public class ItemIconGB : MonoBehaviour
{
    public AudioSource noisePlayerItemIcon;
    public AudioClip buttonPress;

    public ItemAssets itemAssets;
    public Item linkedItem;
    public int pickupNumber = 0;
    public List<Sprite> arrows = new();
    public MainGame game;
    public Transform destination;

    public void Start()
    {
        noisePlayerItemIcon = FindFirstObjectByType<AudioSource>();

        itemAssets = FindFirstObjectByType<ItemAssets>();
        game = FindFirstObjectByType<MainGame>();
        transform.Find("ItemImage").GetComponent<Image>().sprite = itemAssets.GetSprite(this.gameObject.name);
        transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
        destination = null;
    }

    public ItemIconGB Init(string name, int amount, Item item)
    {
        itemAssets = FindFirstObjectByType<ItemAssets>();
        game = FindFirstObjectByType<MainGame>();
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
        //print("played button press from soldier UI");
        noisePlayerItemIcon.PlayOneShot(buttonPress);
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
