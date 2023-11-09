// EventSystemManager - Halen, Cameron
// Updates the currently selected item for menus
// Last edit: 9/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EventSystemManager : MonoBehaviour
{
    // Static reference
    public static EventSystemManager Instance { get; private set; }

    // Singleton instantiation
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Component References")]
    public EventSystem eventSystem;

    /// <summary>
    /// Set's the event system's currently selected game object for UI
    /// </summary>
    /// <param name="newSelectedObject"></param>
    public void SetCurrentSelectedGameObject(GameObject newSelectedObject)
    {
        eventSystem.SetSelectedGameObject(newSelectedObject);
        Button newSelectable = newSelectedObject.GetComponent<Button>();
        newSelectable.Select();
        newSelectable.OnSelect(null);
    }

    /// <summary>
    /// sets the input to controll the UI
    /// </summary>
    /// <param name="playerController"></param>
    public void SetPlayerToControl(PlayerController playerController)
    {
        GetComponentInChildren<InputSystemUIInputModule>().actionsAsset = playerController.gameObject.GetComponent<PlayerInput>().actions;
    }
}
