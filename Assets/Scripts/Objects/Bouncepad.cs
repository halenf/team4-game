// Bouncepad - Cameron
// Applies an upwards force to any rigidbody that enters its trigger.
// Last edit: 1/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    [Tooltip("Bounce strength.")]
    [Min(0)] public float force;

    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.GetComponent<Rigidbody>())
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * force, ForceMode.VelocityChange);
            m_animator.Play("Bounce");
        }
    }
}
