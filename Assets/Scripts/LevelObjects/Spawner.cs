using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("Whether to spawn multiple")]
    public bool repeating;
    [Tooltip("Whether to spawn with random rotation")]
    public bool randomRotation;
    [Tooltip("where to spawn")]
    public Transform spawnLocation;
    [Tooltip("what to spawn")]
    public GameObject spawnobject;
    [Tooltip("minimum time until object is spawned")]
    public float minSpawnTime;
    [Tooltip("maximum time until object is spawned")]
    public float maxSpawnTime;
    [Tooltip("velocity vector x is force towards the right y is up")]
    public Vector2 velocity;

    private Vector3 m_realVelocity;
    private float m_currentTime;
    GameObject m_reference;

    public void Start()
    {
        m_realVelocity = (Vector3)velocity;
        StartSpawnRoutine();
    }

    private void StartSpawnRoutine()
    {
        m_currentTime = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(m_currentTime);

        

        if(randomRotation)
        {
            Quaternion rotation = Quaternion.Euler(UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f));
            m_reference = Instantiate(spawnobject, spawnLocation.position, rotation);
        } else
        {
            m_reference = Instantiate(spawnobject, spawnLocation.position, Quaternion.identity);
        }

        Rigidbody rb = m_reference.GetComponent<Rigidbody>();

        if(rb != null)
        {
            rb.velocity = m_realVelocity;
        }



        if(repeating)
        {
            StartSpawnRoutine();
        }
    }
}
