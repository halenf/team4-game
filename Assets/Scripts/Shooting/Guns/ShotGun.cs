// shot gun - Cameron
// justa shoot function
// Last edit: 1/11/23
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
        }
        PlayerController player = transform.parent.gameObject.GetComponent<PlayerController>();
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

        float tempRecoil = recoil;
        if (player.IsGrounded()) tempRecoil *= groundMultiplyer;
        transform.parent.GetComponent<Rigidbody>().AddForce(tempRecoil * -transform.forward, ForceMode.Impulse);
    }
}
