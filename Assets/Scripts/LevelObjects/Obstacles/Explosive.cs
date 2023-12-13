using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosive : Obstacle
{
    [SerializeField] private GameObject m_explosionPrefab;

    [Header("Object Properties")]
    public float maxHealth;
    private float m_currentHealth;

    public override bool isActive
    {
        get { return m_isActive; }
        set
        {
            if (!value) Explode();
            m_isActive = value;
        }
    }

    public override void Start()
    {
        base.Start();
        m_currentHealth = maxHealth;
        if (!m_isActive)
        {
            Debug.LogError("Explosive object instantiated with isActive set to false: " + gameObject.name + ", " + gameObject.GetInstanceID());
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;

        if (m_currentHealth < 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public override void DestroyObstacle(float time)
    {
        StartCoroutine(ExplodeAfterTime(time));
    }

    private IEnumerator ExplodeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Explode();
    }
}
