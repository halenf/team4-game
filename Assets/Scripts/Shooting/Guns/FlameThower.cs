// Flamethrower - Cameron
// custum behaviour for the flamethrower
// Last edit: 9/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;
using static UnityEngine.ParticleSystem;

public class FlameThower : Gun
{
    private Collider m_collider;
    public float timeToColliderOff;
    private float m_timeToColliderOff;
    private int m_playerID;
    public void Start()
    {
        m_collider = GetComponent<Collider>();
    }
    public override void Shoot(int playerID, Bullet.BulletEffect effect)
    {
        //get player ID so it is impossible to damage shooter
        m_playerID = playerID;
        //enable damage
        m_collider.enabled = true;
        m_timeToColliderOff = timeToColliderOff;
        // Activate the muzzle flash
        if (muzzleFlash) muzzleFlash.Play();

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
            m_collider.enabled = false;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        //i took this from the bullets on collision

        // If collides with a player that isn't the one who shot it
        if (collision.gameObject.tag == "Player"
            && collision.gameObject.GetComponent<PlayerController>().id != m_playerID)
        {
            
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(bulletDamage);
            

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
