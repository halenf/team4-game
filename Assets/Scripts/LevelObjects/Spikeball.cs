using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikeball : MonoBehaviour
{
    public ParticleSystem sparks;

    public float sparkFrequency;

    private float m_currentDifficulty;

    private SphereCollider m_collider;

    private Rigidbody m_rb;
    // Start is called before the first frame update
    void Start()
    {
        m_collider = GetComponent<SphereCollider>();
        m_rb = GetComponent<Rigidbody>();
        m_currentDifficulty = sparkFrequency;
    }


    public void OnCollisionStay(Collision collision)
    {
        
        m_currentDifficulty -= m_rb.velocity.magnitude;
        if(m_currentDifficulty < 0)
        {
            Instantiate(sparks, collision.contacts[0].point, Quaternion.identity);
            m_currentDifficulty = sparkFrequency;
        }
        
    }
}
