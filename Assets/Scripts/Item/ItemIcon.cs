using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Tracing;
using UnityEditor;

public class ItemIcon : MonoBehaviour
{
    public AudioSource noisePlayerItemIcon;
    public AudioClip buttonPress;

    public ItemAssets itemAssets;
    public Item linkedItem;
    public int pickupNumber = 0;
    public List<Sprite> arrows = new();
    public MainGame game;
    public Transform destination;
    public string parentName;

    public void Start()
    {
        noisePlayerItemIcon = FindObjectOfType<AudioSource>();

        itemAssets = FindObjectOfType<ItemAssets>();
        game = FindObjectOfType<MainGame>();
        parentName = transform.parent.name;
        transform.Find("ItemImage").GetComponent<Image>().sprite = itemAssets.GetSprite(this.gameObject.name);
        transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
        destination = null;
    }

    public ItemIcon Init(string name, int amount, Item item)
    {
        itemAssets = FindObjectOfType<ItemAssets>();
        game = FindObjectOfType<MainGame>();
        gameObject.name = name;
        pickupNumber = amount;
        linkedItem = item;
        parentName = transform.parent.name;
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
        //check for valid swap destination
        if (destination != null && !parentName.Contains("AllItems") && !parentName.Contains("Ground"))
        {
            transform.Find("SwapDestination").gameObject.SetActive(true);
            transform.Find("SwapDestination").GetComponent<Image>().sprite = destination.GetComponent<Soldier>().soldierPortrait;
        }
        else
            transform.Find("SwapDestination").gameObject.SetActive(false);

        //check for display icon or not
        if (parentName.Contains("Display"))
        {
            //display live ammo
            if (linkedItem.gunType != null)
            {
                transform.Find("AmmoIndicator").gameObject.SetActive(true);
                transform.Find("AmmoIndicator").GetComponent<TextMeshProUGUI>().text = linkedItem.ammo.ToString();
            }
            else
                transform.Find("AmmoIndicator").gameObject.SetActive(false);

            transform.Find("SelectButton").gameObject.SetActive(false);
            //suppress item count for spawned items
            transform.Find("AmountIcon").gameObject.SetActive(false);
            transform.Find("Arrow").gameObject.SetActive(false);
        }
        else
        {
            if (parentName.Contains("Ally"))
            {
                //display live ammo
                if (linkedItem.gunType != null)
                {
                    transform.Find("AmmoIndicator").gameObject.SetActive(true);
                    transform.Find("AmmoIndicator").GetComponent<TextMeshProUGUI>().text = linkedItem.ammo.ToString();
                }
                else
                    transform.Find("AmmoIndicator").gameObject.SetActive(false);

                //suppress item count for spawned items
                transform.Find("AmountIcon").gameObject.SetActive(false);

                if (pickupNumber > 0)
                    transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
                else
                    transform.Find("Arrow").GetComponent<Image>().sprite = arrows[3];
            }
            else if (parentName.Contains("Inventory"))
            {
                //display live ammo
                if (linkedItem.gunType != null)
                {
                    transform.Find("AmmoIndicator").gameObject.SetActive(true);
                    transform.Find("AmmoIndicator").GetComponent<TextMeshProUGUI>().text = linkedItem.ammo.ToString();
                }
                else
                    transform.Find("AmmoIndicator").gameObject.SetActive(false);

                //suppress item count for spawned items
                transform.Find("AmountIcon").gameObject.SetActive(false);

                if (pickupNumber > 0)
                    transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
                else
                {
                    if (destination != null)
                        transform.Find("Arrow").GetComponent<Image>().sprite = arrows[3];
                    else
                        transform.Find("Arrow").GetComponent<Image>().sprite = arrows[2];
                }
                        
            }
            else if (parentName.Contains("Ground"))
            {
                //display live ammo
                if (linkedItem.gunType != null)
                {
                    transform.Find("AmmoIndicator").gameObject.SetActive(true);
                    transform.Find("AmmoIndicator").GetComponent<TextMeshProUGUI>().text = linkedItem.ammo.ToString();
                }
                else
                    transform.Find("AmmoIndicator").gameObject.SetActive(false);

                //suppress item count for spawned items
                transform.Find("AmountIcon").gameObject.SetActive(false);

                if (pickupNumber > 0)
                    transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
                else
                    transform.Find("Arrow").GetComponent<Image>().sprite = arrows[1];
            }
            else
            {
                //suppress ammo count for unspawned items
                transform.Find("AmmoIndicator").gameObject.SetActive(false);

                if (pickupNumber > 0)
                    transform.Find("Arrow").GetComponent<Image>().sprite = arrows[1];
                else
                    transform.Find("Arrow").GetComponent<Image>().sprite = arrows[0];
            }

            transform.Find("AmountIcon").GetComponent<TextMeshProUGUI>().text = pickupNumber.ToString();
        }
    }

    public void LeftClick()
    {
        if (parentName.Contains("Ally"))
            SwapClicked("Ally"); 
        else if (parentName.Contains("Inventory"))
            if (game.activeItemPanel != null)
                SwapClicked("Inventory");
            else
                DropClicked();
        else if (parentName.Contains("Ground"))
            SwapClicked("Ground");
        else
            PickupClicked();
    }

    public void RightClick()
    {
        if (parentName.Contains("Inventory") || parentName.Contains("Ally") || parentName.Contains("Ground"))
            SwapDropUnclicked();
        else
            PickupUnclicked();
    }

    public void SwapClicked(string itemSourceParent)
    {
        print("Swap clicked " + itemSourceParent);
        if (pickupNumber > 0)
            pickupNumber--;

        destination = itemSourceParent switch
        {
            "Inventory" => game.activeItemPanel.GetComponent<AllyItemsPanel>().linkedSoldier.transform,
            "Ally" => game.activeSoldier.transform,
            _ => null,
        };

        game.UpdateConfigureAP();
    }
    public void PickupClicked()
    {
        if (pickupNumber < 9)
            pickupNumber++;

        destination = game.activeSoldier.transform;

        game.UpdateConfigureAP();
    }
    public void DropClicked()
    {
        if (pickupNumber > 0)
            pickupNumber--;


        game.UpdateConfigureAP();
    }

    public void SwapDropUnclicked()
    {
        pickupNumber = 1;
        destination = null;

        game.UpdateConfigureAP();
    }
    public void PickupUnclicked()
    {
        if (pickupNumber > 0)
            pickupNumber--;

        if (pickupNumber == 0)
            destination = null;

        game.UpdateConfigureAP();
    }
   
}
