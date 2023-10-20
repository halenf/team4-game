using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    [Header("Pistol Details")]
    [Tooltip("Number of bullets fired in every burst shot.")]
    [Min(0)] public float burstNumber;
    [Tooltip("Bullets per second the burst shot fires.")]
    [Min(0)] public float shootingSpeed;
    
    public override void Shoot(int playerID, bool shouldBounce)
    {
        StartCoroutine(BurstShot(playerID, shouldBounce));
    }

    private IEnumerator BurstShot(int playerID, bool shouldBounce)
    {
        for (int i = 0; i < burstNumber; i++)
        {
            // instantiate the bullet
            Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.Init(playerID, bulletDamage, shouldBounce, transform.forward * bulletSpeed);
            // apply recoil to player
            transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
            // wait for next burst shot
            if (i != burstNumber - 1) yield return new WaitForSeconds(1f / shootingSpeed);
        }
    }
}
