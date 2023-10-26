// Gun - Halen, Cameron
// Stores gun data
// Last edit: 25/10/23

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

    [Header("Gun Properties")]
    [Min(0)] public float recoil;
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
    public abstract void Shoot(int playerID, bool shouldBounce, bool isBig);

    // Start is called before the first frame update
    void Start()
    {

    }
}
