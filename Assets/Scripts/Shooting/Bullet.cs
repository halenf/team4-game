// Bullet - Halen, Cameron
// Determines bullet behaviour
// Last edit: 2/11/23

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
    public void Init(int playerID, float damage, Vector3 velocity, float lifeTime, BulletEffect effect)
    {
        m_playerID = playerID;
        m_damage = damage;
        m_rb.velocity = velocity;
        m_currentEffect = effect;

        // Set the particle system to default - Halen
        m_particle = sparksPrefab;

        switch(m_currentEffect)
        {
            case BulletEffect.Ricochet:
            { 
                BulletDestroy(lifeTime);
                break;
            }
            case BulletEffect.Big:
            {
                transform.localScale = transform.localScale * 2;
                BulletDestroy(lifeTime);
                break;
            }
            case BulletEffect.Explode:
            {
                StartCoroutine(Explode(lifeTime));
                break;
            }
            case BulletEffect.None:
            {
                BulletDestroy(lifeTime);
                break;
            }
        }

        // Set the bullet trail materials
        TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
        if (trail) trail.material = (Material) Resources.Load("Materials/Player/Player" + (m_playerID + 1).ToString() + "_alt");
    }

    private IEnumerator Explode(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.Init(explosionDamage, explosionRadius, explosionLifetime);
        BulletDestroy();
    }

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.drag = 0;
        m_rb.angularDrag = 0;
        m_rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // When bullet collides with another object
    private void OnCollisionEnter(Collision collision)
    {
        // default particle effect
        m_particle = sparksPrefab;

        // If the bullet collides with a player that isn't the one who shot it
        if (collision.gameObject.tag == "Player"
            && GameManager.Instance.GetPlayerID(collision.gameObject.GetComponent<PlayerController>()) != m_playerID)
        {
            // deal damage to player if the bullet is not an exploding bullet
            if (m_currentEffect != BulletEffect.Explode)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                player.TakeDamage(m_damage);
            }

            // set the particle effect to blood
            m_particle = bloodPrefab;

            BulletDestroy();
            return;
        }

        // if the bullet hits a destructible platform
        if (collision.gameObject.tag == "Breakable")
        {
            collision.gameObject.GetComponent<BreakableObject>().TakeDamage(m_damage);
        }

        // destroy or bounce bullet
        if (m_currentEffect == BulletEffect.Ricochet)
        {
            Instantiate(ricochetPrefab, transform.position, transform.rotation);
            m_currentEffect = BulletEffect.None;
        }
        else
        {
            BulletDestroy();
        }
    }

    private void OnDestroy()
    {
        // instantiate explosion if player has exploding bullets
        // otherwise instantiate sparks
        if (m_currentEffect == BulletEffect.Explode)
        {
            Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.Init(explosionDamage, explosionRadius, explosionLifetime);
        }
        else Instantiate(m_particle, transform.position, transform.rotation);
    }

    public virtual void BulletDestroy()
    {
        Destroy(gameObject);
    }

    public virtual void BulletDestroy(float lifeTime)
    {
        Destroy(gameObject, lifeTime);
    }
}
