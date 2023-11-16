using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikeball : MonoBehaviour
{
    public ParticleSystem sparks;

    public float reqiuerdSpeed;

    private SphereCollider m_collider;

    private Rigidbody m_rb;
    // Start is called before the first frame update
    void Start()
    {
        m_collider = GetComponent<SphereCollider>();
        m_rb = GetComponent<Rigidbody>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (m_rb.velocity.magnitude > reqiuerdSpeed)
        {
            MakeSparks();
        }
    }

    private void MakeSparks()
    {
        Instantiate(sparks, -transform.up * m_collider.radius, Quaternion.identity);
    }
}
