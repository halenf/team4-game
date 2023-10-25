using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Array of all the buttons that appear on the pause menu in order of appearance.")]
    public Button[] options;
    [Tooltip("Label that displays which player has control of the menu.")]
    public TMP_Text labelDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        labelDisplay.text = "Player 1";
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
