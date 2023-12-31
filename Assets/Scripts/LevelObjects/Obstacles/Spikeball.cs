// Spikeball - Cameron
// Spawns sparks as the spikeball roll against objects.
// Last edit: 17/11/23

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spikeball : Obstacle
{
    // component references
    private Rigidbody m_rb;
    
    public ParticleSystem sparksPrefab;

    [Tooltip("Larger numbers will reduce the spawn frequecny of sparks.")]
    public float sparkFrequency;
    private float m_sparkSpawnFactor;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        m_rb = GetComponent<Rigidbody>();
        m_sparkSpawnFactor = sparkFrequency;
    }

    public void OnCollisionStay(Collision collision)
    {
        m_sparkSpawnFactor -= m_rb.velocity.magnitude;
        if(m_sparkSpawnFactor <= 0)
        {
            Instantiate(sparksPrefab, collision.contacts[0].point, Quaternion.identity);
            m_sparkSpawnFactor = sparkFrequency;
        }

        // Check if the explosion hits a player that isn't the player who created it
        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(player.maxHealth, AnnouncerSubtitleDisplay.AnnouncementType.DeathExplosion);
        }

        // check if the explosion hits a breakable object
        if (collision.gameObject.tag == "Breakable")
        {
            collision.gameObject.GetComponent<BreakableObject>().TakeDamage(collision.gameObject.GetComponent<BreakableObject>().maxHealth);
        }
    }
}
