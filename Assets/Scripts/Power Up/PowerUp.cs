//PowerUp - Cameron
// just has on collision
// Last edit: 1/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    // properties
    private float m_lifeTime; // time box exists for
    private float m_lifeTimer; // tracks whether the box should despawn

    private bool m_hasQuit = false; // tracks if the game has ended

    private Animator m_animator;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem m_spawnEffect;
    [SerializeField] private ParticleSystem m_destroyEffect;

    public virtual void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Despawn the powerup if it has reached the end of its lifetime
        if (m_lifeTimer >= m_lifeTime) m_animator.SetTrigger("Despawn");

        // increase the powerup timer if the powerup is not permanent
        if (m_lifeTime > 0) m_lifeTimer += Time.deltaTime;
    }

    /// <summary>
    /// Initialise the powerup.
    /// </summary>
    /// <param name="_lifeTime">The powerup's lifetime. 0 won't despawn the powerup.</param>
    public virtual void Init(float _lifeTime)
    {
        m_lifeTime = _lifeTime;
    }

    public abstract void OnTriggerEnter(Collider other);

    private void OnApplicationQuit()
    {
        m_hasQuit = true;
    }

    protected void OnDestroy()
    {
        if (!m_hasQuit)
        {
            Instantiate(m_destroyEffect);
        }
    }
}
