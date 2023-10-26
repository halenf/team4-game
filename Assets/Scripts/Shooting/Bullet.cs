// Bullet - Halen, Cameron
// Determines bullet behaviour
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody m_rb;
    private float m_playerID;

    public float minLifeTime;
    public float maxLifeTime;
    public float sizeScaler;

    [Header("Stats")]
    [SerializeField] private float m_damage;
    [SerializeField] private bool m_shouldBounce;

    /// <summary>
    /// For setting bullet details after being instantiated
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="damage"></param>
    /// <param name="shouldBounce"></param>
    public void Init(float playerID, float damage, bool shouldBounce, Vector3 velocity, bool isBig)
    {
        m_playerID = playerID;
        m_damage = damage;
        m_shouldBounce = shouldBounce;
        m_rb.velocity = velocity;
        float random = Random.Range(minLifeTime, maxLifeTime);
        if (isBig)
        {
            transform.localScale = transform.localScale * sizeScaler;
        }
        Destroy(gameObject, random);
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
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetInstanceID() != m_playerID)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(m_damage);
            Destroy(gameObject);
        }
        
        if (!m_shouldBounce && collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        } else
        {
            m_shouldBounce = false;
        }

        if(collision.gameObject.tag == "Platform")
        {
            collision.gameObject.GetComponent<Platform>().TakeDamage(m_damage);
        }
    }
}
