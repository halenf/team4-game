// Item box - Cameron
// activates power up in player
// Last edit: 26/10/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : PowerUp
{
    public override void OnCollisionEnter(Collision other)
    {
        //if colliding with player
        if (other.gameObject.tag == "Player")
        {
            //find player
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            //pick power up
            int choice = Random.Range(1, 6);

            //call choice in player
            switch (choice)
            {
                case 1:
                    player.ActivateRicochet();
                    break;
                case 2:
                    player.IncreaseFireRate();
                    break;
                case 3:
                    player.ActivateSheild();
                    break;
                case 4:
                    player.ActivateBigBullets();
                    break;
                case 5:
                    player.ActivateExplodeBullets();
                    break;
            }
            //destroy self
            Destroy(gameObject);
        }
    }
}
