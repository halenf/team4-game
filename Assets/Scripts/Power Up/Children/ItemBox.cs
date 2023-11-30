// Item box - Cameron
// activates power up in player
// Last edit: 26 /10/23
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : PowerUp
{
    [SerializeField] private PlayerController.Powerup m_currentPowerup;

    public override void OnStart()
    {
        //pick power up, random int chosen from length of enum Powerup cast back to the enum
        m_currentPowerup = (PlayerController.Powerup) UnityEngine.Random.Range(1, Enum.GetValues(typeof(PlayerController.Powerup)).Length - 1);
    }

    public override void OnTriggerEnter(Collider other)
    {
        //if colliding with player
        if (other.gameObject.tag == "Player")
        {
            //find player
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            player.currentPowerup = m_currentPowerup;

            //destroy self
            Destroy(gameObject);
        }
    }
}
