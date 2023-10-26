// LeaderboardUI - Halen
// Interface for updating the Leaderboard UI canvas
// Last edit: 25/10/23

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
        scoreListDisplay.text = "";
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int winningPlayer, List<int> leaderboard)
    {
        scoreListDisplay.text = "";
        winnerDisplay.text = "Player " + winningPlayer + " is the victor!";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            scoreListDisplay.text += "Player " + i + 1 + ": " + leaderboard[i] + " \n";
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
