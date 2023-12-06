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
    [Tooltip("time till the end lasers show up")]
    public float roundTime;
    private float m_currentRoundTime;

    // tracks if the end lasers have been created
    private bool m_madeLasers = false;

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

    [Header("Pickup Properties")]
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

    private List<GameObject> m_currentPowerupBoxes;
    private List<GameObject> m_currentGunBoxes;

    /// <summary>
    /// call functions to start coroutines
    /// </summary>
    private void Start()
    {
        m_currentRoundTime = 0;

        m_currentGunBoxes = new List<GameObject>(gunBoxSpawns.Length);
        for (int i = 0; i < gunBoxSpawns.Length; i++)
        {
            m_currentGunBoxes.Add(gunBoxPrefab);
        }
        m_currentPowerupBoxes = new List<GameObject>(powerUpSpawns.Length);
        for (int i = 0; i < powerUpSpawns.Length; i++)
        {
            m_currentPowerupBoxes.Add(powerupBoxPrefab);
        }

        StartGunRoutine();
        StartPowerUpRoutine();
    }

    private void Update()
    {
        m_currentRoundTime += Time.deltaTime;

        // spawn the end lasers if the round has ended
        if (!m_madeLasers && m_currentRoundTime >= roundTime)
        { 
            MakeLasers();
            GameManager.Instance.ShowEndLaserWarning();
            m_madeLasers = true;
        }
    }

    /// <summary>
    /// makes the random time to spawn a gun for and then starts the gunbox spawn coroutine with that time
    /// </summary>
    private void StartGunRoutine()
    {
        float time = Random.Range(minGunTimer, maxGunTimer);
        StartCoroutine(SpawnGunBox(time));
    }

    /// <summary>
    /// waits for its time to be over then finds a gunbox transform and spawns a gunbox there then restarts
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator SpawnGunBox(float time)
    {
        yield return new WaitForSeconds(time);
        int spawnIndex = Random.Range(0, gunBoxSpawns.Length);
        //start making the next one
        
        // if there isn't already a box in that slot, spawn a new one
        if (gunBoxSpawns[spawnIndex].childCount == 0)
        {
            GameObject gunBox = Instantiate(gunBoxPrefab, gunBoxSpawns[spawnIndex].transform);
            m_currentGunBoxes.Insert(spawnIndex, gunBox);
            gunBox.GetComponent<PowerUp>().lifeTime = gunBoxLifetime;
        }
        StartGunRoutine();
        
    }

    /// <summary>
    /// makes the random time to spawn a item box for and then starts the itembox spawn coroutine with that time
    /// </summary>
    private void StartPowerUpRoutine()
    {
        float time = Random.Range(minPowerupTimer, maxPowerupTimer);
        StartCoroutine(SpawnPowerUp(time));
    }

    /// <summary>
    /// waits for its time to be over then finds a itembox transform and spawns a itembox there then restarts
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator SpawnPowerUp(float time)
    {
        yield return new WaitForSeconds(time);
        int spawnIndex = Random.Range(0, powerUpSpawns.Length);
        //start making the next one
        StartPowerUpRoutine();
        // if there isn't already a box in that slot, spawn a new one
        if (powerUpSpawns[spawnIndex].childCount == 0)
        {
            GameObject powerUp = Instantiate(powerupBoxPrefab, powerUpSpawns[spawnIndex].transform);
            m_currentPowerupBoxes.Insert(spawnIndex, powerUp);
            powerUp.GetComponent<PowerUp>().lifeTime = powerupBoxLifetime;
        }
        
    }

    /// <summary>
    /// Called when the Stage is unloaded or destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // destroy all the powerups when the stage is unloaded
        foreach (GameObject obj in m_currentGunBoxes)
        {
            if (obj != gunBoxPrefab)
            {
                Destroy(obj);
            }
        }
        foreach (GameObject obj in m_currentPowerupBoxes)
        {
            if (obj != powerupBoxPrefab)
            {
                Destroy(obj);
            }
        }

        // stop the spawning coroutines
        StopAllCoroutines();
    }

    /// <summary>
    /// Spawn the round end lasers.
    /// </summary>
    private void MakeLasers()
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

    public void MakeFireWorks()
    {
        foreach(Transform t in fireworkSpawns)
        {
            Instantiate(fireworks, t);
        }
    }
}
