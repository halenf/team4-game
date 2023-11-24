// platform - Cameron, Halen
// take damages and dies or explodes
// Last edit: 24/11/23

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject debrisObjectPrefab;
    public GameObject destroyEffect;

    [Space(10)]

    public float maxHealth;
    public float debrisDestroyTimer;

    public bool willRespawn;
    public float timeToRespawn;
    
    private float m_currentHealth;

    private MeshRenderer m_renderer;
    private Collider m_collider;

    private void Start()
    {
        m_currentHealth = maxHealth;
        m_renderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<Collider>();
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
            if (willRespawn)
            {
                m_renderer.enabled = false;
                m_collider.enabled = false;
                StartCoroutine(Respawn());
            }
            else
            {
                Destroy(gameObject);
            }   
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        m_collider.enabled = true;
        m_renderer.enabled = true;
    }
}
