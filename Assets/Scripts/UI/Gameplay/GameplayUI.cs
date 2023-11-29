// GameplayUI - Halen, Cameron
// Interface for updating the Gameplay UI canvas
// Last edit: 22/11/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AnnouncerSubtitleDisplay;

public class GameplayUI : MonoBehaviour
{
    public Sprite[] countdownSprites;
    public Sprite startRoundSprite;
    
    [Header("UI Elements")]
    [SerializeField] private Image m_countdownDisplay;
    [SerializeField] private TMP_Text m_roundWinnerDisplay;
    [SerializeField] private EndLaserWarning m_endLaserDisplay;

    //start stuff taken from leaderboard
    public GameObject fadeOut;

    [Space(10)]
    public float leaderboardDisplayTime;

    // Start is called before the first frame update
    void Start()
    {
        m_countdownDisplay.gameObject.SetActive(false);
        m_roundWinnerDisplay.text = "";
        m_endLaserDisplay.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int winningPlayer)
    {
        m_roundWinnerDisplay.text = "Player " + winningPlayer + " wins!";
    }

    /// <summary>
    /// Count down to the start of the new round, then start it.
    /// </summary>
    public void StartCountdown()
    {
        StartCoroutine(Countdown());
        m_endLaserDisplay.gameObject.SetActive(false);
    }

    /// <summary>
    /// Display the player who won the round, then load the next round.
    /// </summary>
    /// <param name="winningPlayerID"></param>
    public void StartRoundEnd(int winningPlayerID)
    {
        StartCoroutine(RoundEnd(winningPlayerID));
    }

    /// <summary>
    /// Enables the image with the laser warning.
    /// </summary>
    public void ShowLaserWarning()
    {
        m_endLaserDisplay.gameObject.SetActive(true);
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForEndOfFrame();

        Time.timeScale = 1f;
        m_countdownDisplay.gameObject.SetActive(true);

        // Count down from 3
        foreach (Sprite sprite in countdownSprites)
        {
            m_countdownDisplay.sprite = sprite;
            //m_countdownDisplay.SetNativeSize();
            yield return new WaitForSeconds(0.75f);
        }

        // Enable player inputs and start round
        
        m_countdownDisplay.sprite = startRoundSprite;
        //m_countdownDisplay.SetNativeSize();

        GameManager.Instance.EnablePlayers();

        // Keep Fight! up
        yield return new WaitForSeconds(0.75f);
        m_countdownDisplay.gameObject.SetActive(false);
    }

    private IEnumerator RoundEnd(int winningPlayerID)
    {
        yield return new WaitForEndOfFrame();

        // Show the player who won the round
        m_roundWinnerDisplay.text = "Player " + (winningPlayerID + 1).ToString() + " wins!";
        yield return new WaitForSeconds(leaderboardDisplayTime * Time.timeScale);
        
        m_roundWinnerDisplay.text = "";
        Instantiate(fadeOut);
    }

    
}
