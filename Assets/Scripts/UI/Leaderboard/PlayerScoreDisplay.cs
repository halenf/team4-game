// Player Score Display - Halen
// Sets the values on the player score display prefabs for the leaderboard canvas.
// Last edit: 23/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreDisplay : MonoBehaviour
{
    public Image pointPrefab;
    [Space(10)]
    public Sprite[] backgroundImages;
    public Sprite[] playerIcons;
    public Sprite[] emptyPointIcons;
    public Sprite[] filledPointIcons;

    [Header("Image References")]
    [SerializeField] private Image m_background;
    [SerializeField] private Image m_playerIcon;
    [SerializeField] private Image m_crownIcon;

    [Space(10)]

    [SerializeField] private Transform m_pointDisplayLayoutGroup;

    public void SetDisplayDetails(int playerID, int playerScore, bool hasCrown = false)
    {
        // give the player their crown (if they deserve it)
        m_crownIcon.gameObject.SetActive(hasCrown);
        
        // set the correct icon and background
        m_background.sprite = backgroundImages[playerID];
        m_playerIcon.sprite = playerIcons[playerID];

        // Instantiate the point icons based on the player's current score and total number of rounds
        for (int i = 0; i < playerScore; i++)
        {
            Image point = Instantiate(pointPrefab, m_pointDisplayLayoutGroup);
            point.sprite = filledPointIcons[playerID];
        }

        for (int i = 0; i < GameManager.Instance.numberOfRounds - playerScore; i ++)
        {
            Image point = Instantiate(pointPrefab, m_pointDisplayLayoutGroup);
            point.sprite = emptyPointIcons[playerID];
        }
    }

    // kills itself when the leaderboard is turned off
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
