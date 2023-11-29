// StartUI - Halen
// Interface for updating the Start menu canvas
// Last edit: 26/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class StartUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Array for each of the text objects for the connected player displays.")]
    [SerializeField] private Image[] m_playerConnectedDisplays;
    [SerializeField] private Image[] m_inputDisplays;
    [SerializeField] private GameObject m_startPromptDisplay;

    [Header("Sprites")]
    [SerializeField] private Sprite m_connectPromptSprite;
    [SerializeField] private Sprite[] m_connectedDisplaySprites;

    private void Start()
    {
        // Set each player display to the join prompt
        foreach(Image image in m_playerConnectedDisplays)
        {
            image.sprite = m_connectPromptSprite;
        }

        // Disable start prompt display
        m_startPromptDisplay.SetActive(false);
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(List<Gamepad> controllers, int playerID)
    {
        m_playerConnectedDisplays[playerID].sprite = m_connectedDisplaySprites[playerID];
        m_playerConnectedDisplays[playerID].SetNativeSize();
        m_playerConnectedDisplays[playerID].rectTransform.position += new Vector3(0, -40, 0);

        // activate the start prompt
        if (controllers.Count > 1) m_startPromptDisplay.SetActive(true);
    }

    // turn on the knobs when a player has input
    public void ShowPlayerInput(bool show, int id)
    {
        m_inputDisplays[id].gameObject.SetActive(show);
    }
}
