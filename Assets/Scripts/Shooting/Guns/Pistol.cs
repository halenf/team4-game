// Pistol - Halen, Cameron
// specific script for the pistol gun
// Last edit: 9/11/23

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
    
   
    public override void Shoot(int playerID, Bullet.BulletEffect effect, int bounces)
    {
        StartCoroutine(BurstShot(playerID, effect, bounces));
    }

    private IEnumerator BurstShot(int playerID, Bullet.BulletEffect effect, int bounces)
    {
        for (int i = 0; i < burstNumber; i++)
        {
            //rumble controller
            PlayerController player = transform.parent.parent.GetComponent<PlayerController>();
            player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

            //find spread rotation change
            Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

            // instantiate the bullet
            Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
            bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, bounces, effect);

            // Activate the muzzle flash
            muzzleFlash.Play();

            // apply recoil to player
            float tempRecoil = baseRecoil;
            if (player.isGrounded) tempRecoil *= groundedRecoilScalar;
            player.GetComponent<Rigidbody>().AddForce(tempRecoil * -transform.forward, ForceMode.Impulse);

            //play shoot sound
            float pitch = Random.Range(1 - pitchMagnitude, 1 + pitchMagnitude);
            SoundManager.Instance.PlayAudioAtPoint(bulletSpawnTransform.position, shootClip, pitch, volume);

            // wait for next burst shot
            if (i != burstNumber - 1) yield return new WaitForSeconds(1f / shootingSpeed);
        }
    }
}
