using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;
using static UnityEngine.ParticleSystem;

public class FlameThower : Gun
{
    public GameObject hurtObject;
    public float timeToColliderOff;
    private float m_timeToColliderOff;
    private int m_playerID;
    private BulletEffect m_currentEffect;

    public override void Shoot(int playerID, Bullet.BulletEffect effect)
    {
        m_playerID = playerID;
        hurtObject.SetActive(true);
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
        if (m_timeToColliderOff > 0)
        {
            m_timeToColliderOff -= Time.deltaTime;
        }
        else
        {
            hurtObject.SetActive(false);
        }
    }

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

            //BulletDestroy();
            return;
        }

        // if the bullet hits a destructible platform
        if (collision.gameObject.tag == "Breakable")
        {
            collision.gameObject.GetComponent<BreakableObject>().TakeDamage(m_damage);
        }
    }
}
