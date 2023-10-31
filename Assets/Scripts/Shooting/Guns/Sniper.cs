// Sniper - Cameron
// just uses a basic shoot function
// Last edit: 26/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Gun
{
    public override void Shoot(int playerID, Bullet.Effect effect)
    {
        //find spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, effect);
        PlayerController player = transform.parent.gameObject.GetComponent<PlayerController>();
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

        if (player.IsGrounded())
        {
            transform.parent.GetComponent<Rigidbody>().AddForce(groundRecoil * -transform.forward, ForceMode.Impulse);
        }
        else
        {
            transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
        }
    }

    

}
