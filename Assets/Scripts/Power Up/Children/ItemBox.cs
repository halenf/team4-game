// Item box - Cameron
// activates power up in player
// Last edit: 26 /10/23
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : PowerUp
{
    public void Start()
    {
        StartCoroutine(Die());
    }

    public override void OnTriggerEnter(Collider other)
    {
        //if colliding with player
        if (other.gameObject.tag == "Player")
        {
            //find player
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            //pick power up
            int choice = UnityEngine.Random.Range(1, Enum.GetNames(typeof(PlayerController.Powerup)).Length);

            player.ActivatePowerUp((PlayerController.Powerup)choice);

            //destroy self
            Destroy(gameObject);
        }
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);

        //destroy self
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        stage.StartPowerUpRoutine();
    }
}
