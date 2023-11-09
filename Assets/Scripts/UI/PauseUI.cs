// PauseUI - Halen
// Interface for updating the Pause menu canvas
// Last edit: 26/10/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Label that displays which player has control of the menu.")]
    public TMP_Text labelDisplay;

    [Header("Default Selected Object")]
    public GameObject defaultSelectedObject;
    
    // Start is called before the first frame update
    void Start()
    {
        labelDisplay.text = "Player 1";
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int playerNumber)
    {
        string text = labelDisplay.text;
        text = text.Remove(text.Length - 1).Insert(text.Length - 1, playerNumber.ToString());
        labelDisplay.text = text;
    }

    public void OnResume()
    {
        GameManager.Instance.TogglePause(0);
    }

    public void OnEndGame()
    {
        GameManager.Instance.EndGame();
    }

    public void OnMainMenu()
    {
        GameManager.Instance.ResetGame();
    }
}
