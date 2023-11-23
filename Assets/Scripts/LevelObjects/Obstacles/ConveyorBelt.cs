// Conveyor Belt - Halen
// Moves stuff when its standing or sitting on top of it.
// Last edit: 17/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : Obstacle
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
            ToggleBeltAnimation();
        }
    }

    [Tooltip("Whether or not the conveyor belt is activated")]
    [SerializeField] private bool m_isActive = true;
    public bool isActive
    {
        get { return m_isActive; }
        set
        {
            if (m_isActive != value) ToggleBeltAnimation();
            m_isActive = value;
        }
    }

    [Space(10)]
    [Tooltip("For scaling the speed of the animation.")]
    [Range(0, 1)] public float animationSpeedScalar;
    [SerializeField] private Animator m_animator;

    private void Start()
    {
        // set initial belt speed and start belt
        //m_animator.Play("ConveyorBelt");
        ToggleBeltAnimation();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_isActive) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            // apply the force
            rb.AddForce(Vector3.right * (m_movesPositive ? speed : -speed), ForceMode.Force);
        }
    }

    private void ToggleBeltAnimation()
    {
        // if the belt is active, animate in the appropriate direction, otherwise disable/set speed to zero
        float _speed = m_isActive ? (m_movesPositive ? speed : -speed) : 0;
        m_animator.SetFloat("Speed", _speed * animationSpeedScalar);
    }

    public override void ToggleState()
    {
        isActive = !isActive;
    }

    public override void ToggleState(bool state)
    {
        isActive = state;
    }
}
