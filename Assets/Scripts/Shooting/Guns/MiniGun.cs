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
        transform.parent.gameObject.GetComponent<PlayerController>().Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);
        //find spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, effect);
        // apply recoil to player
        transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
    }
}
