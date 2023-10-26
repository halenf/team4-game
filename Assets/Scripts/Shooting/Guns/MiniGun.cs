using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGun : Gun
{
    public override void Shoot(int playerID, bool shouldBounce, bool isBig, bool explode)
    {
        //find spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, shouldBounce, bullet.transform.forward * bulletSpeed, isBig, explode);
        // apply recoil to player
        transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
    }
}
