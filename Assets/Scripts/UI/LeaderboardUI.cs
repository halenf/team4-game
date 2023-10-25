// LeaderboardUI - Halen
// Interface for updating the Leaderboard UI canvas
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text winnerDisplay;
    public TMP_Text scoreListDisplay;
    [Tooltip("Array of buttons that are available to press.")]
    public Button[] buttons;
    
    // Start is called before the first frame update
    void Start()
    {
        scoreListDisplay.text = "";
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int winningPlayer, List<int> leaderboard)
    {
        winnerDisplay.text = "Player " + winningPlayer + " is the victor!";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            scoreListDisplay.text += "Player " + i + ": " + leaderboard[i];
        }
    }

    public void OnPlayAgain()
    {
        GameManager.Instance.LoadFirst();
    }

    public void OnMainMenu()
    {
        GameManager.Instance.ResetGame();
    }
}
