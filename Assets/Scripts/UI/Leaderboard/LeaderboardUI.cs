// LeaderboardUI - Halen
// Interface for updating the Leaderboard UI canvas
// Last edit: 23/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MenuUI
{
    public PlayerScoreDisplay playerScoreDisplayPrefab;
    
    [Header("UI Object References")]
    [SerializeField] private GameObject m_endGameButtons;
    [SerializeField] private Transform m_scoreDisplayLayoutGroup;

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int winningPlayer, List<int> leaderboard, bool isGameOver)
    {
        // instantiate a score display for each player and set its display details
        for (int i = 0; i < leaderboard.Count; i++)
        {
            PlayerScoreDisplay scoreDisplay = Instantiate(playerScoreDisplayPrefab, m_scoreDisplayLayoutGroup.transform);
            scoreDisplay.SetDisplayDetails(i, leaderboard[i], i == winningPlayer);
        }

        // only enable the play again and menu buttons if the game is over
        m_endGameButtons.SetActive(isGameOver);
        m_selectedIndicator.gameObject.SetActive(isGameOver);
        EventSystemManager.Instance.inMenu = isGameOver;
        EventSystemManager.Instance.SetButtonIndicator(m_selectedIndicator, padding);
    }

    public void OnPlayAgain()
    {
        GameManager.Instance.LoadFirst();
    }

    public void OnMainMenu()
    {
        GameManager.Instance.ResetGame();
    }

    protected override void OnEnable()
    {
        m_selectedIndicator.gameObject.SetActive(false);
    }
}
