//Minigun - Cameron
// shoot function for minigun
// Last edit: 9/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGun : Gun
{
    [Tooltip("the max differance in angle of force applied as recoil")]
    [Min(0)]public float randomRecoilAngle;
    [Tooltip("the max amount that the recoil can change by in percent")]
    [Min(0)] public float randomRecoilStrength;
    public override void Shoot(int playerID, Bullet.BulletEffect effect)
    {
        PlayerController player = transform.parent.gameObject.GetComponent<PlayerController>();
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);
        //find spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, effect);

        // Activate the muzzle flash
        muzzleFlash.Play();

        // apply recoil to player
        float tempRecoil = baseRecoil;
        if (player.IsGrounded()) tempRecoil *= groundedRecoilScalar;

        //find a random recoil change percentage
        float randomRecoilForce = Random.Range(tempRecoil - randomRecoilStrength, tempRecoil + randomRecoilStrength);
        
        //find a random direction from the direction the gun is facing in the bounds of -randomRecoilAngle and positive randomrecoil angle
        Vector3 ForceDirection = Quaternion.AngleAxis(Random.Range(-randomRecoilAngle, randomRecoilAngle), Vector3.forward) * -transform.forward;

        // apply recoil to the player
        transform.parent.GetComponent<Rigidbody>().AddForce(randomRecoilForce * ForceDirection, ForceMode.Impulse);

        //play sound
        SoundManager.Instance.PlayAudioAtPoint(bulletSpawnTransform.position, shootClip);
    }
}
