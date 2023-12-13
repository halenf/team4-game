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
    [SerializeField] private GameObject[] m_playerPleaseConnectObjects;
    [SerializeField] private GameObject[] m_playerConnectedObjects;
    [SerializeField] private Image[] m_inputDisplays;
    [SerializeField] private GameObject m_startPromptDisplay;
    [SerializeField] private Sprite[] m_standing;
    [SerializeField] private Sprite[] m_thumbsUp;
    [SerializeField] private GameObject controllsImage;

    [Header("Sprites")]
    [SerializeField] private Sprite m_connectPromptSprite;
    [SerializeField] private Sprite[] m_connectedDisplaySprites;

    private void Start()
    {

        // Disable start prompt display
        m_startPromptDisplay.SetActive(false);
    }

    /// <summary>
    /// Update all the display details of the canvas.
    /// </summary>
    public void SetDisplayDetails(List<Gamepad> controllers)
    {
        int i;
        for (i = 0; i < controllers.Count; i++)
        {
            m_playerPleaseConnectObjects[i].SetActive(false);
            m_playerConnectedObjects[i].SetActive(true);
            ShowPlayerInput(false, i);
        }
        for (;i < 4; i++)
        {
            m_playerPleaseConnectObjects[i].SetActive(true);
            m_playerConnectedObjects[i].SetActive(false);
            ShowPlayerInput(false, i);
            //m_playerConnectedDisplays[i].SetNativeSize();
            //m_playerConnectedDisplays[i].rectTransform.localPosition = new Vector3(0, -40, 0);
        }

        // activate the start prompt
        if (controllers.Count > 1) m_startPromptDisplay.SetActive(true);
    }

    // turn on the knobs when a player has input
    public void ShowPlayerInput(bool show, int id)
    {
        if(show)
        {
            m_inputDisplays[id].sprite = m_thumbsUp[id];
        }
        else
        {
            m_inputDisplays[id].sprite = m_standing[id];
        }
    }

    public void ShowControls(List<Gamepad> controllers)
    {
        controllsImage.SetActive(false);
        foreach (Gamepad controller in controllers)
        {
            if (controller.selectButton.IsPressed())
            {
                controllsImage.SetActive(true);
                return;
            }
        }
    }
}
