// Flamethrower - Cameron
// custum behaviour for the flamethrower
// Last edit: 9/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThower : Gun
{
    public Collider smallCollider;
    public Collider bigCollider;
    public float timeToColliderOff;
    private float m_timeToColliderOff;
    private int m_playerID;

    private ParticleSystem m_fireParticleEffect;
    private bool m_isShooting;
    private AudioSource m_audioSource;

    public override void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_fireParticleEffect = GetComponentInChildren<ParticleSystem>();
        SoundManager.Instance.PlayAudioAtPoint(transform, equipClip);
    }

    public override void Shoot(int playerID, Bullet.BulletEffect effect, int bounces)
    {
        m_audioSource.enabled = true;
        //get player ID so it is impossible to damage shooter
        m_playerID = playerID;

       
        //enable damage
        if (effect != Bullet.BulletEffect.None)
        {
            bigCollider.enabled = true;
            smallCollider.enabled = false;
            m_fireParticleEffect.startSpeed = 52f;
        }
        else
        {
            bigCollider.enabled = false;
            smallCollider.enabled = true;
            m_fireParticleEffect.startSpeed = 16.29f;
        }
        
        m_timeToColliderOff = timeToColliderOff;

        // Activate the fire particle system
        m_isShooting = true;

        // Make the player's controller rumble
        PlayerController player = transform.parent.GetComponent<PlayerController>();
        player.Rumble(lowRumbleFrequency, highRumbleFrequency, rumbleTime);

        // apply recoil to player
        float tempRecoil = baseRecoil;
        if (player.IsGrounded()) tempRecoil *= groundedRecoilScalar;
        transform.parent.gameObject.GetComponent<Rigidbody>().AddForce(tempRecoil * -transform.forward, ForceMode.Impulse);
    }

    public void Update()
    {
        //turn off the damage if the buttun hasnt been held in the timer time
        if (m_timeToColliderOff > 0)
        {
            m_timeToColliderOff -= Time.deltaTime;
        }
        else
        {
            m_audioSource.enabled = false;
            smallCollider.enabled = false;
            bigCollider.enabled = false;
            m_isShooting = false;
        }

        // enabling and disabling the fire particle effect
        if (m_isShooting)
        {
            if (!m_fireParticleEffect.isPlaying) m_fireParticleEffect.Play();
        }
        else m_fireParticleEffect.Stop();
    }

    private void OnTriggerStay(Collider collision)
    {
        //i took this from the bullets on collision

        // If collides with a player that isn't the one who shot it
        if (collision.gameObject.tag == "Player"
            && collision.gameObject.GetComponent<PlayerController>().id != m_playerID)
        {
            
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(bulletDamage, AnnouncerSubtitleDisplay.AnnouncementType.DeathFire);
            

            //BulletDestroy();
            return;
        }

        // if the bullet hits a destructible platform
        if (collision.gameObject.tag == "Breakable")
        {
            collision.gameObject.GetComponent<BreakableObject>().TakeDamage(bulletDamage);
        }
    }
}
