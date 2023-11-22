// GameplayUI - Halen, Cameron
// Interface for updating the Gameplay UI canvas
// Last edit: 22/11/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameplayUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text countdownDisplay;
    public TMP_Text roundWinnerDisplay;

    //start stuff taken from leaderboard
    public TMP_Text scoreListDisplay;

    public GameObject fadeOut;
    
    // Start is called before the first frame update
    void Start()
    {
        countdownDisplay.text = "";
        roundWinnerDisplay.text = "";
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int winningPlayer, List<int> leaderboard)
    {
        roundWinnerDisplay.text = "Player " + winningPlayer + " wins!";
        scoreListDisplay.gameObject.SetActive(true);
        string leaderboardText = "";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboardText += "Player " + (i + 1).ToString() + ": " + leaderboard[i] + " \n";
        }
        scoreListDisplay.text = leaderboardText;
    }

    /// <summary>
    /// Count down to the start of the new round, then start it.
    /// </summary>
    public void StartCountdown()
    {
        StartCoroutine(Countdown());
    }

    /// <summary>
    /// Display the player who won the round, then load the next round.
    /// </summary>
    /// <param name="winningPlayerID"></param>
    public void StartRoundEnd(int winningPlayerID)
    {
        StartCoroutine(RoundEnd(winningPlayerID));
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForEndOfFrame();
        // Count down from 3
        for (int i = 3; i > 0; i--)
        {
            countdownDisplay.text = i.ToString();
            Time.timeScale = 1f;
            yield return new WaitForSeconds(1);
        }

        // Enable player inputs and start round
        GameManager.Instance.EnablePlayers();
        countdownDisplay.text = "Go!";
        yield return new WaitForSeconds(1.5f); // Keep "Go!" up for 1.5 seconds
        countdownDisplay.text = "";
    }

    private IEnumerator RoundEnd(int winningPlayerID)
    {
        yield return new WaitForEndOfFrame();
        // Show the player who won the round
        roundWinnerDisplay.text = "Player " + (winningPlayerID + 1).ToString() + " wins!";
        yield return new WaitForSeconds(5);
        //roundWinnerDisplay.text = "";
        //GameManager.Instance.LoadStage();
        roundWinnerDisplay.text = "";
        scoreListDisplay.gameObject.SetActive(false);
        Instantiate(fadeOut);
    }
}
