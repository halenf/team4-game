// Disconnect UI - Cameron
// has settable text
// Last edit: 1/11/23
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisconnectUI : MonoBehaviour
{
    [Tooltip("The text saying eho dissconected")]
    public TMP_Text disconnectText;

    public void SetText(int playerID)
    {
        disconnectText.text = "Player " + playerID + " has dissconected their controller. Plug it back in to continue playing.";
    }
}
