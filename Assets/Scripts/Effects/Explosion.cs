// explosion - Halen
// Creates damaging hitbox and spawns explosion particle system
// Last edit: 3/11/23
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private int m_playerID;
    private bool m_shouldDisableCollider;
    
    [Header("Properties")]
    [Tooltip("Damage dealt by the explosion to players and objects.")]
    [Min(0)] public float damage;

    [Tooltip("The size of the explosion.")]
    public float radius;

    [Tooltip("How long the explosion is active for.")]
    public float lifetime;

    [Tooltip("Size of the explosion particle effect.")]
    public float blastSizeMultiplier;

    [Header("Particle Systems")]
    public ParticleSystem explosionEffectPrefab;

    [Header("Audio Properties")]
    public float volume;

    void Update()
    {
        if (m_shouldDisableCollider) GetComponent<SphereCollider>().enabled = false;
    }

    public void Start()
    {
        // Create the explosion collider
        SphereCollider collider = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        collider.isTrigger = true;
        collider.radius = radius;

        // Play sound effect and shake screen
        SoundManager.Instance.PlaySound("SFX/SFX-EXPLOSION", volume);
        CameraManager.Instance.ScreenShake(.05f, 30f, .5f);

        // create particle system
        ParticleSystem explosion = Instantiate(explosionEffectPrefab, transform);
        var blastModule = explosion.main;
        blastModule.startSize = radius * blastSizeMultiplier;

        // Enable explosion, destroy object after lifetime
        StartCoroutine(Explode(lifetime));
        Destroy(gameObject, blastModule.startLifetime.constant);
    }

    public void Init(float damage, float radius, float lifetime)
    {
        this.damage = damage;
        this.radius = radius;
        this.lifetime = lifetime;
    }

    private IEnumerator Explode(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        GetComponent<SphereCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the explosion hits a player that isn't the player who created it
        if (other.gameObject.tag == "Player")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(damage, AnnouncerSubtitleDisplay.AnnouncementType.DeathExplosion);
            m_shouldDisableCollider = true;
        }

        // check if the explosion hits a breakable object
        if (other.gameObject.tag == "Breakable")
        {
            other.gameObject.GetComponent<BreakableObject>().TakeDamage(damage);
            m_shouldDisableCollider = true;
        }

        // check if the explosion hits another explosive object
        if (other.gameObject.GetComponent<Explosive>())
        {
            Explosive explosive = other.gameObject.GetComponent<Explosive>();
            explosive.TakeDamage(explosive.maxHealth);
        }
    }
}
