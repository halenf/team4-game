// GameplayUI - Halen
// Interface for updating the Gameplay UI canvas
// Last edit: 26/10/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text countdownDisplay;
    public TMP_Text roundWinnerDisplay;
    
    // Start is called before the first frame update
    void Start()
    {
        countdownDisplay.text = "";
        roundWinnerDisplay.text = "";
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int winningPlayer)
    {
        roundWinnerDisplay.text = "Player " + winningPlayer + " wins!";
    }

    public void StartCountdown()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        // Count down from 3
        for (int i = 3; i > 0; i--)
        {
            countdownDisplay.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        // Enable player inputs and start round
        GameManager.Instance.EnablePlayers();
        countdownDisplay.text = "Go!";
        yield return new WaitForSeconds(1.5f); // Keep "Go!" up for 1.5 seconds
        countdownDisplay.text = "";
    }
}
