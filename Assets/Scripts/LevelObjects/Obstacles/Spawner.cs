// Spawner - Cameron
// Spawns a set game object with various parameters.
// Last edit: 17/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Obstacle
{
    private Vector3 m_velocityVector3;
    private float m_spawnTime;

    public override bool isActive
    {
        get { return m_isActive; }
        set
        {
            if (value != m_isActive)
            {
                // reactivates or deactivates spawner coroutines - halen
                if (m_isActive) StartSpawnRoutine();
                else StopAllCoroutines();
            }

            m_isActive = value;           
        }
    }

    [Header("Spawner Properties")]
    [Tooltip("Whether the spawner will only spawn the object once or many times.")]
    public bool isRepeating;

    [Header("Object Spawn Properties")]
    [Tooltip("Prefab that will be cloned.")]
    public GameObject spawnObjectPrefab;

    [Tooltip("Where the object spawns.")]
    public Transform[] objectSpawnLocations;

    [Tooltip("Whether to spawn with random rotation.")]
    public bool objectHasRandomRotation;

    [Tooltip("Minimum time for the object to spawn.")]
    [Min(0)] public float minSpawnTime;

    [Tooltip("Maximum time for the object to spawn.")]
    [Min(0)] public float maxSpawnTime;

    [Tooltip("minimum initial velocity of the spawned object. X = Horizontal velocity. Y = Vertical velocity.")]
    public Vector2 minInitialVelocity;

    [Tooltip("maximum initial velocity of the spawned object. X = Horizontal velocity. Y = Vertical velocity.")]
    public Vector2 maxInitialVelocity;

    [Tooltip("how long before the spawned object is destroyed")]
    [Min(0)]public float lifeTime;

    public override void Start()
    {
        base.Start();
        if (isActive) StartSpawnRoutine();
    }

    private void StartSpawnRoutine()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        m_velocityVector3 = new Vector3(Random.Range(minInitialVelocity.x, maxInitialVelocity.x), Random.Range(minInitialVelocity.y, maxInitialVelocity.y), 0);
        m_spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        yield return new WaitForSeconds(m_spawnTime);

        // Spawn object with preset properties
        GameObject spawnedObject = Instantiate(spawnObjectPrefab, objectSpawnLocations[Random.Range(0, objectSpawnLocations.Length)]);
        spawnedObject.GetComponent<Rigidbody>().velocity = m_velocityVector3;

        if(lifeTime != 0)
        {
            Destroy(spawnedObject, lifeTime);
        }

        // repeat
        while (isRepeating)
        {
            m_velocityVector3 = new Vector3(Random.Range(minInitialVelocity.x, maxInitialVelocity.x), Random.Range(minInitialVelocity.y, maxInitialVelocity.y), 0);
            m_spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(m_spawnTime);

            // Spawn object with preset properties
            spawnedObject = Instantiate(spawnObjectPrefab, objectSpawnLocations[Random.Range(0, objectSpawnLocations.Length)]);
            spawnedObject.GetComponent<Rigidbody>().velocity = m_velocityVector3;

            if (lifeTime != 0)
            {
                Destroy(spawnedObject, lifeTime);
            }
        }
    }
}
