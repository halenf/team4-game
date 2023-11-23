// GameplayUI - Halen, Cameron
// Interface for updating the Gameplay UI canvas
// Last edit: 22/11/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public Sprite[] countdownSprites;
    public Sprite startSprite;
    
    [Header("UI Elements")]
    [SerializeField] private Image m_countdownDisplay;
    [SerializeField] private TMP_Text m_roundWinnerDisplay;
    [SerializeField] private GameObject m_subtitlesObject;
    private TMP_Text m_subtitlesText;

    //start stuff taken from leaderboard
    public GameObject fadeOut;

    [Space(10)]
    public float leaderboardDisplayTime;

    private void Awake()
    {
        m_subtitlesText = m_subtitlesObject.GetComponentInChildren<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_countdownDisplay.gameObject.SetActive(false);
        m_roundWinnerDisplay.text = "";
        m_subtitlesObject.SetActive(false);
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

        Time.timeScale = 1f;
        m_countdownDisplay.gameObject.SetActive(true);

        // Count down from 3
        foreach (Sprite sprite in countdownSprites)
        {
            m_countdownDisplay.sprite = sprite;
            m_countdownDisplay.SetNativeSize();
            yield return new WaitForSeconds(1);
        }

        // Enable player inputs and start round
        GameManager.Instance.EnablePlayers();
        m_countdownDisplay.sprite = startSprite;
        m_countdownDisplay.SetNativeSize();

        // Keep "Go!" up for 1.5 seconds
        yield return new WaitForSeconds(1.5f);
        m_countdownDisplay.gameObject.SetActive(false);
    }

    private IEnumerator RoundEnd(int winningPlayerID)
    {
        yield return new WaitForEndOfFrame();

        // Show the player who won the round
        m_roundWinnerDisplay.text = "Player " + (winningPlayerID + 1).ToString() + " wins!";
        yield return new WaitForSeconds(leaderboardDisplayTime);
        
        m_roundWinnerDisplay.text = "";
        Instantiate(fadeOut);
    }

    public void SetSubtitles(string subtitle)
    {
        m_subtitlesObject.SetActive(true);
        m_subtitlesText.text = subtitle;
    }

    public void TurnOffSubtitles()
    {
        m_subtitlesObject.SetActive(false);
    }
}
