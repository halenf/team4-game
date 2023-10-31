//Minigun - Cameron
// shoot function for minigun
// Last edit: 26/10/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGun : Gun
{
    public override void Shoot(int playerID, Bullet.Effect effect)
    {
        PlayerController player = transform.parent.gameObject.GetComponent<PlayerController>();
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);
        //find spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, effect);
        // apply recoil to player
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
