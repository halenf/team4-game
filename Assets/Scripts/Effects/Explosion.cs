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
    
    [Header("Properties")]
    [Tooltip("Damage dealt by the explosion to players.")]
    [Min(0)] public float damage;

    [Header("Particle Systems")]
    public ParticleSystem fragments;
    public ParticleSystem blast;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int playerID, float damage, float radius, float lifetime)
    {
        m_playerID = playerID;
        this.damage = damage;
        
        // Create the explosion collider
        SphereCollider collider = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        collider.isTrigger = true;
        collider.radius = radius;
        collider.center = Vector3.zero;

        // set details of the particle systems
        var fragmentsModule = fragments.main;
        fragmentsModule.startSpeed = radius * 3.75f;

        var blastModule = blast.main;
        blastModule.startSize = radius;

        // i'm tired, figure it out
        StartCoroutine(Explode(lifetime));
        Instantiate(fragments, transform.position, Quaternion.identity);
        Instantiate(blast, transform.position, Quaternion.identity);
        Destroy(gameObject, blastModule.startLifetime.constant);
    }

    private IEnumerator Explode(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        GetComponent<SphereCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the explosion hits a player
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            player.TakeDamage(damage);
        }
    }
}
