// LeaderboardUI - Halen
// Interface for updating the Leaderboard UI canvas
// Last edit: 23/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public PlayerScoreDisplay playerScoreDisplayPrefab;
    
    [Header("UI Object References")]
    [SerializeField] private GameObject m_endGameButtons;
    [SerializeField] private Transform m_scoreDisplayLayoutGroup;

    [Header("Default Selected Object")]
    public GameObject defaultSelectedObject;

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

        EventSystemManager.Instance.indicator.gameObject.SetActive(isGameOver);
    }

    public void OnPlayAgain()
    {
        GameManager.Instance.LoadFirst();
    }

    public void OnMainMenu()
    {
        GameManager.Instance.ResetGame();
    }

    private void OnDisable()
    {
        try
        {
            EventSystemManager.Instance.indicator.gameObject.SetActive(false);
        }
        catch (MissingReferenceException e)
        {
            // do nothing
        }
    }
}
