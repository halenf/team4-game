using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
    public int minBulletAmount;
    public int maxBulletAmount;
    public override void Shoot(int playerID, bool shouldBounce, bool isBig)
    {
        int bulletAmount = Random.Range(minBulletAmount, maxBulletAmount);

        for (int i = 0; i < bulletAmount; i++)
        {
            //find spread rotation change
            Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

            // instantiate the bullet
            Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation * shootDirection);
            bullet.Init(playerID, bulletDamage, shouldBounce, bullet.transform.forward * bulletSpeed, isBig);
            // apply recoil to player
            transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
        }

        transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
    }
}
