// Bullet - Halen
// Determines bullet behaviour
// Last edit: 20/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody m_rb;
    private float m_playerID;

    public float minLifeTime;
    public float maxLifeTime;

    [Header("Stats")]
    [SerializeField] private float m_damage;
    [SerializeField] private bool m_shouldBounce;

    /// <summary>
    /// For setting bullet details after being instantiated
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="damage"></param>
    /// <param name="shouldBounce"></param>
    public void Init(float playerID, float damage, bool shouldBounce, Vector3 velocity)
    {
        m_playerID = playerID;
        m_damage = damage;
        m_shouldBounce = shouldBounce;
        m_rb.velocity = velocity;
        transform.rotation = Quaternion.LookRotation(velocity);
        float random = Random.Range(minLifeTime, maxLifeTime);
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
        else if (m_shouldBounce)
        {
            // Get the normal of the object the bullet just collided with
            // and bounce off it using that direction to flip the bullet's velocity
        }

        if(collision.gameObject.tag == "Platform")
        {
            collision.gameObject.GetComponent<Platform>().TakeDamage(m_damage);
        }
    }
}
