// platform - Cameron, Halen
// take damages and dies or explodes
// Last edit: 22/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject debrisObjectPrefab;
    public GameObject destroyEffect;

    [Space(10)]

    public float maxHealth;
    public float debrisDestroyTimer;
    
    private float m_currentHealth;

    private void Start()
    {
        m_currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;
        if (m_currentHealth <= 0)
        {
            // Only try to create the debris if the object has debris set
            if (debrisObjectPrefab)
            {
                GameObject debris = Instantiate(debrisObjectPrefab, transform.position, Random.rotation);
                Destroy(debris, debrisDestroyTimer);
            }
            
            // only create the destroy effect if there is one
            if (destroyEffect)
            {
                GameObject gameObject = Instantiate(destroyEffect, transform.position, Quaternion.identity);
                Destroy(gameObject, debrisDestroyTimer);
            }

            Destroy(gameObject);
        }
    }
}
