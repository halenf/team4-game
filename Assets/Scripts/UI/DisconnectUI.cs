// Disconnect UI - Cameron
// has settable text
// Last edit: 16/11/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisconnectUI : MonoBehaviour
{
    [Tooltip("The text saying who disconnected")]
    [SerializeField] private TMP_Text disconnectText;

    public void SetDisplayDetails(int playerID)
    {
        switch(playerID)
        {
            case 0:
            disconnectText.text = "PLAYER ONE'S CONTROLLER HAD DISCONNECTED. RECONNECT IT TO RESUME OR PAUSE TO QUIT.";
                break;
            case 1:
                disconnectText.text = "PLAYER TWO'S CONTROLLER HAD DISCONNECTED. RECONNECT IT TO RESUME OR PAUSE TO QUIT.";
                break;
            case 2:
                disconnectText.text = "PLAYER THREE'S CONTROLLER HAD DISCONNECTED. RECONNECT IT TO RESUME OR PAUSE TO QUIT.";
                break;
            case 3:
                disconnectText.text = "PLAYER FOUR'S CONTROLLER HAD DISCONNECTED. RECONNECT IT TO RESUME OR PAUSE TO QUIT.";
                break;
        }
    }

    private void Update()
    {
        Time.timeScale = 0f;
    }
}
