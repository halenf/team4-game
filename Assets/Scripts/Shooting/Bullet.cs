// Bullet - Halen, Cameron
// Determines bullet behaviour
// Last edit: 24/11/23

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // component references
    private Rigidbody m_rb;

    // properties
    private float m_damage;
    private int m_playerID;

    public enum BulletEffect
    {
        None,
        Ricochet, 
        Big, 
        Explode
    }

    [Header("Bullet Effects")]
    [SerializeField] [InspectorName("Bullet Effect")] private BulletEffect m_currentEffect;
    public Explosion explosionPrefab;
    [Space(10)]
    [Min(0)] public float explosionDamage;
    [Min(0)] public float explosionRadius;
    [Tooltip("Time the explosion hitbox is active for in seconds.")]
    [Min(0)] public float explosionLifetime;

    [Header("Particle Effects")]
    public ParticleSystem sparksPrefab;
    public ParticleSystem ricochetPrefab;
    public ParticleSystem bloodPrefab;

    private int m_bounces;

    [Space(10)]

    [Tooltip("How intense the glow of the bullet and bullet trail are.")]
    [Range(-5, 5)] public float emissionIntensity;

    // tracks which particle to instantiate when the bullet is destroyed
    private ParticleSystem m_particle;

    /// <summary>
    /// For setting bullet details after being instantiated
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="damage"></param>
    /// <param name="velocity"></param>
    /// <param name="lifeTime"></param>
    /// <param name="effect"></param>
    public void Init(int playerID, float damage, Vector3 velocity, float lifeTime, int bounces, BulletEffect effect)
    {
        m_playerID = playerID;
        m_damage = damage;
        m_rb.velocity = velocity;
        m_currentEffect = effect;
        m_bounces = bounces;

        // Set the particle system to default - Halen
        m_particle = sparksPrefab;

        switch(m_currentEffect)
        {
            case BulletEffect.Ricochet:
            { 
                break;
            }
            case BulletEffect.Big:
            {
                transform.localScale = transform.localScale * 2;
                break;
            }
            case BulletEffect.Explode:
            {
                break;
            }
            case BulletEffect.None:
            {
                break;
            }
        }

        // set the bullet to destroy itself after an amount of time
        StartCoroutine(BulletDestroy(lifeTime));

        // Set the bullet material colour and emission intensity
        MeshRenderer bullet = GetComponentInChildren<MeshRenderer>();
        if (bullet)
        {
            bullet.material = (Material)Resources.Load("Materials/Player/Player" + (m_playerID + 1).ToString());
            bullet.material.SetColor("_EmissionColor", bullet.material.color * emissionIntensity);
        }

        // Set the bullet trail material colour and emission intensity
        TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
        if (trail)
        {
            trail.material = (Material)Resources.Load("Materials/Player/Player" + (m_playerID + 1).ToString() + "_alt");
            trail.material.SetColor("_EmissionColor", trail.material.color * emissionIntensity);
        }
    }
    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.drag = 0;
        m_rb.angularDrag = 0;
        m_rb.useGravity = false;
    }

    // When bullet collides with another object
    private void OnCollisionEnter(Collision collision)
    {
        // default particle effect
        m_particle = sparksPrefab;

        // If the bullet collides with a player that isn't the one who shot it
        if (collision.gameObject.tag == "Player"
            && collision.gameObject.GetComponent<PlayerController>().id != m_playerID)
        {
            // deal damage to player if the bullet is not an exploding bullet
            if (m_currentEffect != BulletEffect.Explode)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                player.TakeDamage(m_damage, AnnouncerSubtitleDisplay.AnnouncementType.DeathBullet);
            }

            // set the particle effect to blood
            m_particle = bloodPrefab;

            StartCoroutine(BulletDestroy(0));
            return;
        }

        // if the bullet hits a destructible platform
        if (collision.gameObject.tag == "Breakable")
        {
            collision.gameObject.GetComponent<BreakableObject>().TakeDamage(m_damage);
        }

        // destroy or bounce bullet
        if (m_currentEffect == BulletEffect.Ricochet && m_bounces > 0)
        {
            Instantiate(ricochetPrefab, transform.position, transform.rotation);

            m_bounces--;
        }
        else
        {
            StartCoroutine(BulletDestroy(0));
        }
    }

    private IEnumerator BulletDestroy(float time)
    {
        // wait for specified amount of time
        yield return new WaitForSeconds(time);

        // instantiate explosion if player has exploding bullets
        // otherwise instantiate sparks
        if (m_currentEffect == BulletEffect.Explode)
        {
            Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.Init(explosionDamage, explosionRadius, explosionLifetime);
        }
        else Instantiate(m_particle, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
