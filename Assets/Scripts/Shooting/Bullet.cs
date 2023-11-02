// Bullet - Halen, Cameron
// Determines bullet behaviour
// Last edit: 2/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // component references
    private Rigidbody m_rb;

    // properties
    private float m_damage;
    private float m_playerID;

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

    [Header("Particle Effects")]
    public ParticleSystem sparksPrefab;

    /// <summary>
    /// For setting bullet details after being instantiated
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="damage"></param>
    /// <param name="velocity"></param>
    /// <param name="lifeTime"></param>
    /// <param name="effect"></param>
    public void Init(float playerID, float damage, Vector3 velocity, float lifeTime, BulletEffect effect)
    {
        m_playerID = playerID;
        m_damage = damage;
        m_rb.velocity = velocity;

        // Set the colour of the particle system to default - Halen
        var particleSettings = sparksPrefab.main;
        particleSettings.startColor = Color.yellow;

        switch(effect)
        {
            case BulletEffect.Ricochet:
            { 
                Destroy(gameObject, lifeTime);
                break;
            }
            case BulletEffect.Big:
            {
                transform.localScale = transform.localScale * 2;
                Destroy(gameObject, lifeTime);
                break;
            }
            case BulletEffect.Explode:
            {
                StartCoroutine(Explode(lifeTime));
                break;
            }
            case BulletEffect.None:
            {
                Destroy(gameObject, lifeTime);
                break;
            }
        }
    }

    private IEnumerator Explode(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
        // editing particle system properties
        var particleSettings = sparksPrefab.main;
        
        // specific collisions

        // If the bullet collides with a player that isn't the one who shot it
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetInstanceID() != m_playerID)
        {
            // deal damage to player
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(m_damage);

            // make particle effect red because blood
            particleSettings.startColor = Color.red;
        }

        // if the bullet hits a destructible platform
        if (collision.gameObject.tag == "Platform")
        {
            collision.gameObject.GetComponent<Platform>().TakeDamage(m_damage);
        }

        // general collisions

        // if the bullet should bounce, then don't destroy it on collision
        if (m_currentEffect == BulletEffect.Ricochet)
        {
            particleSettings.startColor = Color.blue;
            m_currentEffect = BulletEffect.None;
        }
        else
        {
            particleSettings.startColor = Color.yellow;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // instantiate explosion if player has exploding bullets
        // otherwise instantiate sparks
        if (m_currentEffect == BulletEffect.Explode) Instantiate(explosionPrefab);
        else Instantiate(sparksPrefab, transform.position, transform.rotation);
    }
}
