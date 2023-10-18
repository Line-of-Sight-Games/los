using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySourcePanel : MonoBehaviour
{
    public MainMenu menu;
    public IHaveInventory linkedInventorySource;
    public SoundManager soundManager;

    public InventorySourcePanel Init(IHaveInventory inventorySource)
    {
        soundManager = FindObjectOfType<SoundManager>();
        menu = FindObjectOfType<MainMenu>();
        linkedInventorySource = inventorySource;

        return this;
    }

    public void CloseInventorySourcePanel()
    {
        menu.CloseItemPanel(gameObject);
    }

    public void ButtonSoundTrigger()
    {
        soundManager.PlayButtonPress();
    }
}
