using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreDisplay : MonoBehaviour
{
    public Sprite[] backgroundImages;
    public Sprite[] playerIcons;
    public Sprite[] emptyPointIcons;
    public Sprite[] filledPointIcons;

    [Header("Image References")]
    [SerializeField] private Image m_background;
    [SerializeField] private Image m_playerIcon;
    [SerializeField] private Image m_crownIcon;

    [Space(10)]

    public Image pointPrefab;

    public void SetDisplayDetails(int playerID, int playerScore, bool hasCrown = false)
    {
        // give the player their crown (if they deserve it)
        m_crownIcon.gameObject.SetActive(hasCrown);
        
        // set the correct icon and background
        m_background.sprite = backgroundImages[playerID];
        m_playerIcon.sprite = playerIcons[playerID];

        // Instantiate the point icons based on the player's current score and total number of rounds
        Transform pointDisplayLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>().transform;

        for (int i = 0; i < playerScore; i++)
        {
            Image point = Instantiate(pointPrefab, pointDisplayLayoutGroup);
            point.sprite = filledPointIcons[playerID];
        }

        for (int i = 0; i < GameManager.Instance.numberOfRounds - playerScore; i ++)
        {
            Image point = Instantiate(pointPrefab, pointDisplayLayoutGroup);
            point.sprite = emptyPointIcons[playerID];
        }
    }

    // kills itself when the leaderboard is turned off
    private void OnDisable()
    {
        Destroy(this);
    }
}
