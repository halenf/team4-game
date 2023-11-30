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
            m_isActive = value;

            // reactivates or deactivates spawner coroutines - halen
            if (m_isActive) StartSpawnRoutine();
            else StopAllCoroutines();
        }
    }

    [Header("Spawner Properties")]
    [Tooltip("Whether the spawner will only spawn the object once or many times.")]
    public bool isRepeating;

    [Header("Object Spawn Properties")]
    [Tooltip("Prefab that will be cloned.")]
    public GameObject spawnObjectPrefab;

    [Tooltip("Where the object spawns.")]
    public Transform objectSpawnLocation;

    [Tooltip("Whether to spawn with random rotation.")]
    public bool objectHasRandomRotation;

    [Tooltip("Minimum time for the object to spawn.")]
    [Min(0)] public float minSpawnTime;

    [Tooltip("Maximum time for the object to spawn.")]
    [Min(0)] public float maxSpawnTime;

    [Tooltip("Initial velocity of the spawned object. X = Horizontal velocity. Y = Vertical velocity.")]
    public Vector2 initialVelocity;

    [Tooltip("how long before the spawned object is destroyed")]
    [Min(0)]public float lifeTime;

    public override void Start()
    {
        base.Start();
        m_velocityVector3 = (Vector3)initialVelocity;
        if (isActive) StartSpawnRoutine();
    }

    private void StartSpawnRoutine()
    {
        m_spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(m_spawnTime);
        
        // Spawn object with preset properties
        GameObject spawnedObject = Instantiate(spawnObjectPrefab, objectSpawnLocation.position, objectHasRandomRotation ? Random.rotation : Quaternion.identity);
        spawnedObject.GetComponent<Rigidbody>().velocity = m_velocityVector3;

        if(lifeTime != 0)
        {
            Destroy(spawnedObject, lifeTime);
        }
        // Continue to spawn objects
        if(isRepeating) StartSpawnRoutine();
    }
}
