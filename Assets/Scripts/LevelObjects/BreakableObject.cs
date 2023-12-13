// platform - Cameron, Halen
// take damages and dies or explodes
// Last edit: 7/12/23

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Effect Prefabs")]
    public GameObject debrisObjectPrefab;

    [Header("Object Properties")]
    public float maxHealth;
    private float m_currentHealth;

    public float debrisDestroyTimer;

    [Space(5)]

    public float lifeTime;
    private float m_lifeTimer;

    public bool willRespawn;
    public float timeToRespawn;

    private bool m_hasQuit = false;

    // components
    private MeshRenderer m_renderer;
    private Collider m_collider;

    private void Start()
    {
        m_currentHealth = maxHealth;
        m_renderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<Collider>();
    }

    private void Update()
    {
        // if object has a lifetime
        if (lifeTime > 0)
        {
            // if timer is up, break the object
            if (m_lifeTimer >= lifeTime)
            {
                BreakObject();
                m_lifeTimer = 0;
            }

            // increase the timer
            m_lifeTimer += Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;
        if (m_currentHealth <= 0)
        {
            BreakObject();
        }
    }

    private void BreakObject()
    {
        // Only try to create the debris if the object has debris set
        if (debrisObjectPrefab)
        {
            GameObject debris = Instantiate(debrisObjectPrefab, transform.position, Random.rotation);
            Destroy(debris, debrisDestroyTimer);
        }

        // respawn on timer or disable object
        if (willRespawn)
        {
            m_renderer.enabled = false;
            m_collider.enabled = false;
            StartCoroutine(Respawn());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        m_collider.enabled = true;
        m_renderer.enabled = true;
    }
}
