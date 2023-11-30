// Bouncepad - Cameron
// Applies an upwards force to any rigidbody that enters its trigger.
// Last edit: 17/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncepad : Obstacle
{
    [Tooltip("Bounce strength.")]
    [Min(0)] public float force;

    public void OnTriggerEnter(Collider collision)
    {
        if(isActive && collision.gameObject.GetComponent<Rigidbody>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * force, ForceMode.VelocityChange);
            m_animator.Play("Bounce");
            SoundManager.Instance.PlaySound("Obstacles/SFX-BOUNCEPAD");
        }
    }
}
