// GameplayUI - Halen
// Interface for updating the Gameplay UI canvas
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        for (int i = 3; i > 0; i--)
        {
            countdownDisplay.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countdownDisplay.text = "Go!";
        yield return new WaitForSeconds(1.5f);
        countdownDisplay.text = "";
        // enable all players with game manager
    }
}
