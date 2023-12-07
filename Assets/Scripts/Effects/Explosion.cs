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
    [Tooltip("Damage dealt by the explosion to players.")]
    [Min(0)] public float damage;
    [Tooltip("changes the size of hit sphere")]
    public float radius;
    public float lifetime;
    [Tooltip("changes the size of the visual portion of the explosion")]
    public float blastSizeMultiplyer;
    [Header("Particle Systems")]
    //public ParticleSystem fragments;
    public ParticleSystem blast;

    [Header("Audio Properties")]
    public float volume;

    void Update()
    {
        if (m_shouldDisableCollider) GetComponent<SphereCollider>().enabled = false;
    }

    public void Awake()
    {
        // Create the explosion collider
        SphereCollider collider = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        collider.isTrigger = true;
        collider.radius = radius;
        //collider.center = transform.position;

        // set details of the particle systems
        //var fragmentsModule = fragments.main;
        //fragmentsModule.startSpeed = radius * 3.75f;

        var blastModule = blast.main;
        blastModule.startSize = radius * blastSizeMultiplyer;

        // start explosion and make particles and set destruction and play explosion sound
        //SoundManager.Instance.PlaySound("SFX/SFX-EXPLOSION", volume);
        //CameraManager.Instance.ScreenShake(.05f, 30f, .5f);
        StartCoroutine(Explode(lifetime));
        //Instantiate(fragments, transform.position, Quaternion.identity);
        Instantiate(blast, transform);
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
    }
}
