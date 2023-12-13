//stage - Cameron
//just stores spawn locations
// last edit 30/11/2023
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [Tooltip("Time Until the end lasers spawn.")]
    public float roundTime;
    private float m_currentRoundTime;

    private bool m_madeLasers = false; // tracks if the end lasers have been created

    [SerializeField] private bool m_shouldSpawnPowerups = true;
    public bool shouldSpawnPowerups
    {
        get { return m_shouldSpawnPowerups; }
        set
        {
            if (value != m_shouldSpawnPowerups)
            {
                if (value) StartPowerupSpawning();
                else StopAllCoroutines();
            }

            m_shouldSpawnPowerups = value;
        }
    }

    [Space(10)]

    [Header("Spawn Transforms")]
    [Tooltip("put spawn transforms here")]
    public Transform[] playerSpawns;
    [Tooltip("power up transforms")]
    public Transform[] powerUpSpawns;
    [Tooltip("gun box transforms")]
    public Transform[] gunBoxSpawns;
    [Tooltip("end game laser transforms")]
    public Transform[] endLaserSpawns;
    [Tooltip("places to make fire works when a round is over")]
    public Transform[] fireworkSpawns;
    [Tooltip("Position the camera starts in for your stage.")]
    public Transform cameraDefaultTransform;

    [Header("Prefabs")]
    public GameObject gunBoxPrefab;
    public GameObject powerupBoxPrefab;
    public TimedEndLaser endLaserPrefab;
    public GameObject spikeBallPrefab;
    public ParticleSystem fireworks;

    [Header("Powerup Properties")]
    [Tooltip("How long a gun box will be active for.")]
    [Min(0)] public float gunBoxLifetime;
    [Tooltip("minimum time a gun box will appear in")]
    [Min(0)] public float minGunTimer;
    [Tooltip("maximum time a gun box will appear in")]
    [Min(0)] public float maxGunTimer;    

    [Space(5)]
    [Tooltip("How long an item box will be active for.")]
    [Min(0)] public float powerupBoxLifetime;
    [Tooltip("minimum time a power up will appear in")]
    [Min(0)] public float minPowerupTimer;
    [Tooltip("maximum time a power up will appear in")]
    [Min(0)] public float maxPowerupTimer;
    
    [Header("end laser properties")]
    [Tooltip("Time until end lasers stop")]
    [Min(0)] public float endLaserTimer;
    [Tooltip("speed of end lasers")]
    [Min(0)] public float endLaserSpeed;

    /// <summary>
    /// call functions to start coroutines
    /// </summary>
    private void Start()
    {
        m_currentRoundTime = 0;

        if (m_shouldSpawnPowerups) StartPowerupSpawning();
    }

    private void Update()
    {
        m_currentRoundTime += Time.deltaTime;

        // spawn the end lasers if the round has ended
        if (!m_madeLasers && m_currentRoundTime >= roundTime)
        { 
            SpawnLasers();
            GameManager.Instance.ShowEndLaserWarning();
            m_madeLasers = true;
        }
    }

    private void StartPowerupSpawning()
    {
        // spawn gun boxes
        StartCoroutine(SpawnPowerup(gunBoxPrefab, gunBoxLifetime, minGunTimer, maxGunTimer, gunBoxSpawns));
        // spawn item boxes
        StartCoroutine(SpawnPowerup(powerupBoxPrefab, powerupBoxLifetime, minPowerupTimer, maxPowerupTimer, powerUpSpawns));
    }

    /// <summary>
    /// Spawns a powerup at a random slot, or the next empty slot, or not at all if all slots are full.
    /// </summary>
    private IEnumerator SpawnPowerup(GameObject powerupPrefab, float lifeTime, float minSpawnTimer, float maxSpawnTimer, Transform[] spawnLocations)
    {
        while (m_shouldSpawnPowerups)
        {
            // spawn timer
            yield return new WaitForSeconds(Random.Range(minSpawnTimer, maxSpawnTimer));

            // find an empty slot to spawn the powerup in
            // if there aren't any empty slots, just skip
            int counter = 0;
            int spawnIndex = Random.Range(0, spawnLocations.Length);
            while (true)
            {
                if (spawnLocations[spawnIndex].childCount == 0)
                {
                    GameObject powerup = Instantiate(powerupPrefab, spawnLocations[spawnIndex].transform);
                    powerup.GetComponent<PowerUp>().Init(lifeTime);
                    break; // if a powerup is spawned, exit
                }
                else
                {
                    spawnIndex++; // if the slot is full, move to the next
                    if (spawnIndex == spawnLocations.Length) spawnIndex = 0; // spawnIndex overflow
                }

                counter++; // increase the counter
                if (counter == spawnLocations.Length) break; // no places to spawn a powerup
            }
        }
    }

    /// <summary>
    /// Called when the Stage is unloaded or destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // stop the spawning coroutines
        StopAllCoroutines();
    }

    /// <summary>
    /// Spawn the round end lasers.
    /// </summary>
    private void SpawnLasers()
    {
        for (int i = 0; i < endLaserSpawns.Length; i++)
        {
            TimedEndLaser thisLaser = Instantiate(endLaserPrefab, endLaserSpawns[i].transform);
            if (thisLaser != null)
            {
                thisLaser.Init(endLaserSpeed, endLaserTimer);
            }
        }
    }

    public void SpawnFireworks()
    {
        foreach(Transform t in fireworkSpawns)
        {
            Instantiate(fireworks, t);
        }
    }
}
