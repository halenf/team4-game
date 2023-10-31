// shot gun - Cameron
// justa shoot function
// Last edit: 26/10/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
    public int minBulletAmount;
    public int maxBulletAmount;
    public override void Shoot(int playerID, Bullet.Effect effect)
    {
        int bulletAmount = Random.Range(minBulletAmount, maxBulletAmount);

        for (int i = 0; i < bulletAmount; i++)
        {
            //find spread rotation change
            Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

            // instantiate the bullet
            Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
            bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, effect);
            // apply recoil to player
            transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
        }
        transform.parent.gameObject.GetComponent<PlayerController>().Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

        transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
    }
}
