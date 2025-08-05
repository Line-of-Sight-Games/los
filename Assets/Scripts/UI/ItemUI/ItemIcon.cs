using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class ItemIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item item; // The item associated with this icon
    public RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public ItemSlot originalSlot, currentSlot;

    public ItemSlot suppressor;
    public ItemSlot medikit1;
    public ItemSlot medikit2;
    public TextMeshProUGUI weightIndicator;
    public TextMeshProUGUI ammoIndicator;
    public TMP_InputField overrideAmmoIndicator;

    public DropthrowPopup dropThrowPopup;
    public SpyJamPopup spyJamPopup;
    public BinocReconPopup binocReconPopup;
    public BinocInHandPopup binocInHandPopup;

    void Start()
    {
        dropThrowPopup = FindFirstObjectByType<DropthrowPopup>(FindObjectsInactive.Include);
        spyJamPopup = FindFirstObjectByType<SpyJamPopup>(FindObjectsInactive.Include);
        binocReconPopup = FindFirstObjectByType<BinocReconPopup>(FindObjectsInactive.Include);
        binocInHandPopup = FindFirstObjectByType<BinocInHandPopup>(FindObjectsInactive.Include);
    }

    public ItemIcon Init(Item item, ItemSlot originalSlot)
    {
        print($"trying to initialise prefab of {item.itemName}({item.Id})");
        this.item = item;
        gameObject.name = item.itemName;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        this.originalSlot = originalSlot;
        currentSlot = originalSlot;
        item.markedForAction = string.Empty;
        transform.Find("ItemImage").GetComponent<Image>().sprite = FindFirstObjectByType<ItemAssets>().GetSprite(item.itemName);

        //show suppressor slot
        if (item.IsSuppressibleGun())
            ShowSuppressor();
        else
            suppressor.gameObject.SetActive(false);

        //show medikit slots
        if (item.IsBackpack() || item.IsBrace() || item.IsBag())
        {
            print($"{item.Id} is a brace/bag/backpack");
            ShowMedikit1();
            if (item.IsBackpack())
                ShowMedikit2();
            else
                medikit2.gameObject.SetActive(false);
        }
        else
        {
            medikit1.gameObject.SetActive(false);
            medikit2.gameObject.SetActive(false);
        }

        return this;
    }

    private void Update()
    {
        DisplayItemDetails();
    }

    public void DisplayItemDetails()
    {
        //show item weight
        if (item.weight > 0)
        {
            weightIndicator.gameObject.SetActive(true);
            if (item.owner is Soldier linkedSoldier && linkedSoldier.IsBull() && (item.IsGun() || item.IsAmmo()))
                weightIndicator.text = $"{1}";
            else
                weightIndicator.text = $"{item.weight}";
        }
        else
            weightIndicator.gameObject.SetActive(false);

        //show ammo indicator
        if (item.IsGun() || item.IsAmmo())
        {
            ammoIndicator.gameObject.SetActive(true);
            ammoIndicator.text = $"{item.ammo}";
        }
        else
            ammoIndicator.gameObject.SetActive(false);

        //show override ammo field
        if (MenuManager.Instance.OverrideView)
        {
            if (item.IsGun() || item.IsAmmo())
            {
                overrideAmmoIndicator.gameObject.SetActive(true);
                overrideAmmoIndicator.placeholder.GetComponent<TextMeshProUGUI>().text = $"{item.ammo}";
            }
            else
                overrideAmmoIndicator.gameObject.SetActive(false);
        }
        else
            overrideAmmoIndicator.gameObject.SetActive(false);
    }
    public void AddItemIconInSlot(Item item, ItemSlot slot)
    {
        print($"trying to add item icon to slot for item: {item.Id}");
        if (item != null && slot != null)
        {
            print($"item and slot are not null");
            if (slot.item == null)
            {
                print("targetslot is empty");
                ItemIcon newItemIcon = Instantiate(MenuManager.Instance.itemIconPrefab, slot.transform).GetComponent<ItemIcon>().Init(item, slot);
                print($"{newItemIcon.name} @ {newItemIcon.transform}");
                slot.AssignItemIcon(newItemIcon);
            }
        }
    }
    public void ShowSuppressor()
    {
        if (item.Inventory.GetItemInSlot("Suppressor") != null)
            AddItemIconInSlot(item.Inventory.GetItem("Suppressor"), suppressor);

        suppressor.gameObject.SetActive(true);
    }
    public void ShowMedikit1()
    {
        print($"trying to show medikits in item {item.Id}");
        if (item.Inventory.GetItemInSlot("Medikit1") != null)
            AddItemIconInSlot(item.Inventory.GetItemInSlot("Medikit1"), medikit1);

        medikit1.gameObject.SetActive(true);
    }
    public void ShowMedikit2()
    {
        print($"searching inventory of item: {item.Id} trying to show medikit2");
        if (item.Inventory.GetItemInSlot("Medikit2") != null)
        {
            print($"found medikit2 item in inventory of item: {item.Id}");
            AddItemIconInSlot(item.Inventory.GetItemInSlot("Medikit2"), medikit2);
        }
        
        medikit2.gameObject.SetActive(true);
    }
    public void ChangeAmmo()
    {
        if (int.TryParse(overrideAmmoIndicator.text, out int newAmmo) && newAmmo >= 0)
        {
            FileUtility.WriteToReport($"(Override) {item.itemName} ammo changed from {item.ammo} to {newAmmo}"); //write to report

            item.ammo = newAmmo;
        }
                
        overrideAmmoIndicator.text = "";
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.localScale = new(0.5f, 0.5f);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        GameObject targetDrop = eventData.pointerEnter;
        string originalInventoryId = "none", targetInventoryId = "none", originalSlotName = item.whereEquipped, targetSlotName = "none";
        if (!MenuManager.Instance.onItemUseScreen && !MenuManager.Instance.inventorySourceViewOnly)
        {
            if (targetDrop != null)
            {
                ItemSlot targetSlot = targetDrop.GetComponent<ItemSlot>();
                InventorySourcePanel targetPanel = targetDrop.GetComponentInParent<InventorySourcePanel>();
                originalInventoryId = item.ownerId;

                if (item.IsOnlyRemovableFromCorpse() && item.IsNestedOnSoldier() && item.SoldierNestedOn().IsAlive())
                    ReturnToOldSlot();
                else
                {
                    if (targetSlot != null)
                    {
                        //check that the target slot is free and that any slots it blocks are also free
                        if (CheckValidSlot(targetSlot) && CheckBlockedSlotsAreFree(targetSlot))
                        {
                            SetCurrentSlot(targetSlot.AssignItemIcon(this));

                            targetSlotName = targetSlot.name;

                            InventoryDisplayPanelSoldier soldierInventory = targetSlot.GetComponentInParent<InventoryDisplayPanelSoldier>();
                            if (targetSlotName.Contains("Suppressor"))
                                targetInventoryId = targetSlot.parentIcon.item.Id;
                            else if (targetSlotName.Contains("Medikit"))
                                targetInventoryId = targetSlot.parentIcon.item.Id;
                            else if (targetSlotName.Contains("Backpack"))
                                targetInventoryId = soldierInventory.GetItemInSlot("Back").Id;
                            else if (targetSlotName.Contains("LeftBrace"))
                            {
                                targetInventoryId = soldierInventory.GetItemInSlot("LeftLeg").Id;
                                targetSlotName = "Brace1";
                            }
                            else if (targetSlotName.Contains("RightBrace"))
                            {
                                targetInventoryId = soldierInventory.GetItemInSlot("RightLeg").Id;
                                targetSlotName = "Brace1";
                            }
                            else if (targetSlotName.Contains("BArmour"))
                                targetInventoryId = soldierInventory.GetItemInSlot("Chest").Id;
                            else if (targetSlotName.Contains("JArmour"))
                                targetInventoryId = soldierInventory.GetItemInSlot("Head").Id;
                            else if (targetSlotName.Contains("Bag"))
                                targetInventoryId = soldierInventory.GetItemInSlot("Posterior").Id;
                            else
                                targetInventoryId = targetSlot.linkedInventoryObject.Id;

                            if (targetSlot == originalSlot)
                                item.markedForAction = string.Empty;
                            else
                                item.markedForAction = $"{originalInventoryId}|{originalSlotName}|{targetInventoryId}|{targetSlotName}";
                        }
                        else
                            ReturnToOldSlot();
                    }
                    else if (targetPanel != null)
                    {

                        if (targetPanel.linkedInventorySource == item.owner)
                        {
                            ReturnToOriginalSlot();

                            item.markedForAction = string.Empty;
                        }
                        else if (targetPanel.name.Contains("Ground"))
                        {
                            SetCurrentSlot(Instantiate(targetPanel.itemSlotPrefab, targetPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>().Init(targetPanel.linkedInventorySource).AssignItemIcon(this));

                            targetSlotName = transform.parent.name;
                            item.markedForAction = $"{originalInventoryId}|{originalSlotName}|{targetInventoryId}|{targetSlotName}";
                        }
                        else if (targetPanel.name.Contains("GB"))
                        {
                            SetCurrentSlot(Instantiate(targetPanel.itemSlotPrefab, targetPanel.transform.Find("Viewport").Find("Contents")).GetComponent<ItemSlot>().Init(targetPanel.linkedInventorySource).AssignItemIcon(this));

                            targetInventoryId = targetPanel.linkedInventorySource.Id;
                            targetSlotName = transform.parent.name;
                            item.markedForAction = $"{originalInventoryId}|{originalSlotName}|{targetInventoryId}|{targetSlotName}";
                        }
                        else
                            ReturnToOldSlot();
                    }
                    else
                        ReturnToOldSlot();
                }
            }
            else
                ReturnToOldSlot();

            GameManager.Instance.UpdateConfigureAP();
        }
        else
            ReturnToOldSlot();
    }
    public void UseItem()
    {
        if (MenuManager.Instance.onItemUseScreen && !MenuManager.Instance.OverrideView)
        {
            if (item.IsUsable())
            {
                if (item.IsBinoculars() && item.SoldierNestedOn().IsUsingBinocularsInReconMode()) //give option to relocate or stop recon
                {
                    Vector2 mousePosition = Input.mousePosition;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        binocReconPopup.transform.parent.GetComponent<RectTransform>(),
                        mousePosition,
                        null,
                        out Vector2 localPoint
                    );

                    binocReconPopup.binocsUsed = item;
                    binocReconPopup.binocsItemIcon = this;
                    binocReconPopup.GetComponent<RectTransform>().anchoredPosition = localPoint;
                    binocReconPopup.ShowBinocReconPopup();
                }
                else if (item.IsBinoculars() && item.whereEquipped.Contains("Hand")) //give option to use either recon or flash mode
                {
                    Vector2 mousePosition = Input.mousePosition;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        binocInHandPopup.transform.parent.GetComponent<RectTransform>(),
                        mousePosition,
                        null,
                        out Vector2 localPoint
                    );

                    binocInHandPopup.binocsUsed = item;
                    binocInHandPopup.binocsItemIcon = this;
                    binocInHandPopup.GetComponent<RectTransform>().anchoredPosition = localPoint;
                    binocInHandPopup.ShowBinocInHandPopup();
                }
                else if (item.SoldierNestedOn().IsAbleToUseItems())
                {
                    if (item.IsULF())
                    {
                        Vector2 mousePosition = Input.mousePosition;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            spyJamPopup.transform.parent.GetComponent<RectTransform>(),
                            mousePosition,
                            null,
                            out Vector2 localPoint
                        );

                        spyJamPopup.ulfUsed = item;
                        spyJamPopup.GetComponent<RectTransform>().anchoredPosition = localPoint;
                        spyJamPopup.ShowSpyJamPopup();
                    }
                    else
                    {
                        int ap = item.usageAP;
                        //adept ability
                        if (MenuManager.Instance.activeSoldier.IsAdept() && item.usageAP > 1)
                            ap--;
                        //gunner ability
                        if (MenuManager.Instance.activeSoldier.IsGunner() && item.IsAmmo())
                            ap = 1;

                        if (MenuManager.Instance.activeSoldier.CheckAP(ap))
                        {
                            if (MenuManager.Instance.activeSoldier.HandsFreeToUseItem(item))
                                MenuManager.Instance.OpenUseItemUI(item, transform.parent.name, this, ap);
                        }
                    }
                }
                
            }
            else
                MenuManager.Instance.generalAlertUI.Activate("This item is not usable in this way.");
        }
    }
    public void DropThrowItem()
    {
        if (MenuManager.Instance.onItemUseScreen && !MenuManager.Instance.OverrideView)
        {
            if (item.IsThrowable())
            {
                Vector2 mousePosition = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    dropThrowPopup.transform.parent.GetComponent<RectTransform>(),
                    mousePosition,
                    null,
                    out Vector2 localPoint
                );

                dropThrowPopup.itemToDropThrow = item;
                dropThrowPopup.itemIconToDropThrow = this;
                dropThrowPopup.GetComponent<RectTransform>().anchoredPosition = localPoint;
                dropThrowPopup.ShowDropThrowPopup();
            }
            else
            {
                MenuManager.Instance.OpenDropThrowItemUI("drop", item, transform.parent.name, this);
            }
        }
    }
    public bool CheckValidSlot(ItemSlot targetSlot)
    {
        foreach (string slot in item.equippableSlots)
            if (targetSlot.item == null && !targetSlot.unavailable && (targetSlot.name.Contains(slot) || targetSlot.name.Contains("ItemSlot")))
                return true;
        return false;
    }
    public bool CheckEmptySlot(ItemSlot targetSlot)
    {
        if (targetSlot.item == null)
            return true;
        return false;
    }
    public bool CheckBlockedSlotsAreFree(ItemSlot targetSlot)
    {
        InventoryDisplayPanelSoldier inventoryDisplay = targetSlot.GetComponentInParent<InventoryDisplayPanelSoldier>();
        if (inventoryDisplay != null)
        {
            for (int i = 0; i < inventoryDisplay.blockedSlotMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < inventoryDisplay.blockedSlotMatrix.GetLength(1); j++)
                {
                    if (inventoryDisplay.blockedSlotMatrix[i, 0] == item.itemName && inventoryDisplay.blockedSlotMatrix[0, j] == targetSlot.name)
                    {
                        List<string> slotNames = inventoryDisplay.blockedSlotMatrix[i, j].ToString().Split('|').ToList();
                        foreach (string slotName in slotNames)
                        {
                            if (slotName != "")
                            {
                                if (!CheckEmptySlot(inventoryDisplay.transform.FindRecursively(slotName).GetComponent<ItemSlot>()))
                                    return false;
                            }
                        }
                    }
                }
            }
        }
                    
        return true;                
    }
    public void ReturnToOriginalSlot()
    {
        SetCurrentSlot(originalSlot);
        originalSlot.AssignItemIcon(this);
    }
    public void ReturnToOldSlot()
    {
        SetCurrentSlot(currentSlot);
        currentSlot.AssignItemIcon(this);
    }
    public void SetCurrentSlot(ItemSlot slot)
    {
        currentSlot.ClearItemIcon();
        currentSlot = slot;
    }
}
