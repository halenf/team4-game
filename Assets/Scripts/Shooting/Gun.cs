// Gun - Halen, Cameron
// Stores gun data
// Last edit: 3/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public abstract class Gun : MonoBehaviour
{
    [Header("Gun Properties")]
    [Min(0)] public float baseRecoil;
    [Range(0, 1)] public float groundedRecoilScalar;
    [Tooltip("Number of bullets the player can fire each second.")]
    [Min(0)] public float baseFireRate;
    [Min(0)] public int ammoCapacity;
    public Transform bulletSpawnTransform;
    public ParticleSystem muzzleFlash;
    [Space(5)]
    public Sprite indicator;
    public Color colour;

    [Header("Bullet Properties")]
    [InspectorName("Prefab")] public Bullet bulletPrefab;
    [Tooltip("Angle on either side of the player's aim direction in degrees that a bullet can be randomly added to bullet velocity.")]
    [Min(0)] public float spread;
    [Min(0)] public float bulletSpeed;
    [Min(0)] public float bulletDamage;
    [Min(0)] public float bulletLifeTime;

    [Header("Rumble")]
    [Range(0, 1)]public float highRumbleFrequency;
    [Range(0, 1)]public float lowRumbleFrequency;
    [Min(0)]public float rumbleTime;

    [Header("Audio")]
    public AudioClip shootClip;
    public AudioClip equipClip;
    public float pitchMagnitude;
    public float volume;

    private Material m_material;

    public virtual void Awake()
    {
        SoundManager.Instance.PlayAudioAtPoint(transform, equipClip);
    }

    public void GetMaterial()
    {
        m_material = gameObject.GetComponentInChildren<MeshRenderer>().material;
        m_material.EnableKeyword("_EMISSION");
    }

    /// <summary>
    /// Makes the gun shoot.
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="shouldBounce"></param>
    public virtual void Shoot(int playerID, Bullet.BulletEffect effect, int bounces)
    {
        // calc spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnTransform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, bullet.transform.forward * bulletSpeed, bulletLifeTime, bounces, effect);

        // Activate the muzzle flash
        if (muzzleFlash) muzzleFlash.Play();
        
        // Make the player's controller rumble
        PlayerController player = transform.parent.GetComponent<PlayerController>();
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

        // apply recoil to player
        float tempRecoil = baseRecoil;
        if (player.IsGrounded()) tempRecoil *= groundedRecoilScalar;
        transform.parent.gameObject.GetComponent<Rigidbody>().AddForce(tempRecoil * -transform.forward, ForceMode.Impulse);

        //play shoot sound
        float pitch = Random.Range(1 - pitchMagnitude, 1 + pitchMagnitude);
        SoundManager.Instance.PlayAudioAtPoint(bulletSpawnTransform.position, shootClip, pitch, volume);
    }

    public void ChangeMaterial(int playerID)
    {
        Material playerMaterial = (Material)Resources.Load("Materials/Player/Player" + (playerID + 1).ToString());
        GetMaterial();
        m_material.SetColor("_EmissionColor", playerMaterial.color * 2);
        m_material.color = playerMaterial.color;
    }
}
