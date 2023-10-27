// LeaderboardUI - Halen
// Interface for updating the Leaderboard UI canvas
// Last edit: 27/10/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text winnerDisplay;
    public TMP_Text scoreListDisplay;

    [Header("Default Selected Object")]
    public GameObject defaultSelectedObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int winningPlayer, List<int> leaderboard)
    {
        winnerDisplay.text = "Player " + winningPlayer + " is the victor!";
        string leaderboardText = "";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboardText += "Player " + (i + 1).ToString() + ": " + leaderboard[i] + " \n";
        }
        scoreListDisplay.text = leaderboardText;
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
