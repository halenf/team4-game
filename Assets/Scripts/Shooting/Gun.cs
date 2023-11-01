// Gun - Halen, Cameron
// Stores gun data
// Last edit: 26/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public Bullet bulletPrefab;

    [Header("Bullet Properties")]
    [Tooltip("Angle on either side of the player's aim direction in degrees that a bullet can be randomly added to bullet velocity.")]
    [Min(0)] public float spread;
    [Min(0)] public float bulletSpeed;
    [Min(0)] public float bulletDamage;
    [Tooltip("Will be true when the player has the Ricochet powerup.")]
    public bool shouldBounce;

    [Range(0, 1)]public float highRumbleFrequency;
    [Range(0, 1)]public float lowRumbleFrequency;
    [Min(0)]public float rumbleTime;

    public float bulletLifeTime;

    [Header("Gun Properties")]
    [Min(0)] public float recoil;
    [Range(0, 1)] public float groundMultiplyer;
    [Tooltip("Number of bullets the player can fire each second.")]
    [Min(0)] public float baseFireRate;
    [Min(0)] public float ammoCapacity;
    public Transform bulletSpawnTransform;
    public GameObject gunPrefab;

    /// <summary>
    /// Makes the gun shoot.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="shouldBounce"></param>
    public virtual void Shoot(int playerID, Bullet.Effect effect)
    {
        //find spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, effect);
        // apply recoil to player
        PlayerController player = transform.parent.gameObject.GetComponent<PlayerController>();
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

        float tempRecoil = recoil;
        if (player.IsGrounded()) tempRecoil *= groundMultiplyer;
        transform.parent.GetComponent<Rigidbody>().AddForce(tempRecoil * -transform.forward, ForceMode.Impulse);
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}
