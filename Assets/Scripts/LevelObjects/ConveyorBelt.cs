// Conveyor Belt - Halen
// Moves stuff when its standing or sitting on top of it.
// Last edit: 15/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Tooltip("The strength of the force the belt applies to objects standing on it.")]
    [Min(0)] public float speed;

    [Tooltip("If the direction of the conveyor belt is in the positive or negative X-axis.")]
    [SerializeField] private bool m_movesPositive;
    public bool movesPositive
    {
        get { return m_movesPositive; }
        set
        {
            m_movesPositive = value;

            // set direction value
            if (m_movesPositive) m_beltSpeed = -Mathf.Abs(m_beltSpeed);
            else m_beltSpeed = Mathf.Abs(m_beltSpeed);

            // set animation direction in animator
            m_animator.SetFloat("Direction", m_beltSpeed);
        }
    }

    private float m_beltSpeed;

    [Tooltip("Whether or not the conveyor belt is activated")]
    [SerializeField] private bool m_isActive = true;

    public bool isActive
    {
        get { return m_isActive; }
        set
        {
            m_isActive = value;
            if (m_animator)
            {
                if (m_isActive) m_animator.Play("ConveyorBelt");
                else m_animator.Play("Inactive");
            }
        }
    }

    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();

        if (m_movesPositive) m_beltSpeed = speed;
        else m_beltSpeed = -speed;

        // set initial belt speed and start belt
        if (m_animator)
        {
            m_animator.SetFloat("Direction", m_beltSpeed);
            if (m_isActive) m_animator.Play("ConveyorBelt");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_isActive) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            // apply the force
            rb.AddForce(new Vector3(m_beltSpeed, 0, 0) * speed, ForceMode.Force);
        }
    }
}
