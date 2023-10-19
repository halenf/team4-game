using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody m_rb;
    private float m_playerID;

    [Header("Stats")]
    [SerializeField] private float m_damage;
    [SerializeField] private bool m_shouldBounce;

    public Bullet(float playerID, float damage, bool shouldBounce)
    {
        m_playerID = playerID;
        m_damage = damage;
        m_shouldBounce = shouldBounce;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
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
    }
}
