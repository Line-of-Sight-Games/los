using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class TabManager : MonoBehaviour
{
    EventSystem system;

    void Start()
    {
        system = EventSystem.current;// EventSystemManager.currentSystem;
    }

    // Update is called once per frame
    void Update()
    {
        if (system.currentSelectedGameObject != null)
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                Selectable next;
                if (Keyboard.current.leftShiftKey.isPressed)
                    next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                else
                    next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();

                if (next != null)
                {

                    if (next.TryGetComponent<TMP_InputField>(out var inputfield))
                        inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                    system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
                }
                else
                    print("next nagivation element not found");
            }
        }
        //else
            //print("no game object currently selected");
    }
}

