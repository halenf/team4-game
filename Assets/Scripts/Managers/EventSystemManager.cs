// EventSystemManager - Halen
// Updates the currently selected item for menus
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
}
