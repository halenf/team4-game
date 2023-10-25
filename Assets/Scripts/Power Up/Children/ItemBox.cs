using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : PowerUp
{
    public override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            int choice = Random.Range(1, 4);

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
            }
            Destroy(gameObject);
        }
    }
}
