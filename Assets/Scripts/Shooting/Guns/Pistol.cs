// Pistol - Halen, Cameron
// specific script for the pistol gun
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            //find spread rotation change
            Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

            // instantiate the bullet
            Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
            bullet.Init(playerID, bulletDamage, shouldBounce, bullet.transform.forward * bulletSpeed);
            // apply recoil to player
            transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
            // wait for next burst shot
            if (i != burstNumber - 1) yield return new WaitForSeconds(1f / shootingSpeed);
        }
    }
}
