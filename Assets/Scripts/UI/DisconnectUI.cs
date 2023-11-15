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
    public TMP_Text disconnectText;

    public void SetText(int playerID)
    {
        disconnectText.text = "Player " + (playerID + 1).ToString() + "'s controller has disconnected. Reconnect it to resume or pause to quit.";
    }
}
