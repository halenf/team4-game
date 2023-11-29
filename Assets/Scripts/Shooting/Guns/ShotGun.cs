// shot gun - Cameron
// justa shoot function
// Last edit: 9/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
    public int minBulletAmount;
    public int maxBulletAmount;
    public override void Shoot(int playerID, Bullet.BulletEffect effect, int bounces)
    {
        int bulletAmount = Random.Range(minBulletAmount, maxBulletAmount);

        for (int i = 0; i < bulletAmount; i++)
        {
            //find spread rotation change
            Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

            // instantiate the bullet
            Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
            bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, bounces, effect);
        }
        // Activate the muzzle flash
        muzzleFlash.Play();

        PlayerController player = transform.parent.gameObject.GetComponent<PlayerController>();

        // make their controller rumble
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

        // apply recoil to player
        float tempRecoil = baseRecoil;
        if (player.IsGrounded()) tempRecoil *= groundedRecoilScalar;
        transform.parent.GetComponent<Rigidbody>().AddForce(tempRecoil * -transform.forward, ForceMode.Impulse);
        float pitch = Random.Range(1 - pitchMagnitude, 1 + pitchMagnitude);
        SoundManager.Instance.PlayAudioAtPoint(bulletSpawnTransform.position, shootClip, pitch, volume);
    }
}
