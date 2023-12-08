// PauseUI - Halen
// Interface for updating the Pause menu canvas
// Last edit: 26/10/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Label that displays which player has control of the menu.")]
    [SerializeField] private Image m_labelDisplay;
    [SerializeField] private Sprite[] m_playerLabelSprites;

    [Header("Event System Settings")]
    public GameObject defaultSelectedObject;

    [Header("Transition Prefabs")]
    public GameObject fadeOut;
    public GameObject fadeOutReset;

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(int playerID)
    {
        m_labelDisplay.sprite = m_playerLabelSprites[playerID];
    }

    public void OnResume()
    {
        GameManager.Instance.TogglePause(0);
    }

    public void OnEndGame()
    {
        Time.timeScale = 1f;
        Instantiate(fadeOut);
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ResetGame();
    }

    private void OnEnable()
    {
        EventSystemManager.Instance.indicator.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        EventSystemManager.Instance.indicator.gameObject.SetActive(false);
    }
}
