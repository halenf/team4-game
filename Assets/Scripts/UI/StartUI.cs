// StartUI - Halen
// Interface for editing the Start menu canvas
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class StartUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image titleIcon;
    [Tooltip("Array for each of the text objects for the connected player displays.")]
    public TMP_Text[] playerConnectedDisplay;
    public TMP_Text startPromptDisplay;

    [Header("Default Strings")]
    [Tooltip("The text that is displayed in a controller slot when there isn't a player bound to it.")]
    public string emptyControllerSlotText;
    [Tooltip("The text that is displayed in a controller slot once a player binds to it.")]
    public string fullControllerSlotText;
    [Tooltip("Prompt that is displayed when at least two players are connected and the game can start.")]
    public string readyToStartText;

    private void Start()
    {
        // Set each player box text display to the default text
        foreach(TMP_Text textObject in playerConnectedDisplay)
        {
            textObject.text = fullControllerSlotText;
        }

        startPromptDisplay.text = "";
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(List<Gamepad> controllers)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            playerConnectedDisplay[i].text = emptyControllerSlotText;
        }

        if (controllers.Count > 1) startPromptDisplay.text = readyToStartText;
    }
}
