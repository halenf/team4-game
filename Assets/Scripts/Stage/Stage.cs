//stage - Cameron
//just stores spawn locations
// last edit 1/11/2023
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : MonoBehaviour
{
    //array of spawn transforms
    [Tooltip("put spawn transforms here")]
    public Transform[] spawns;
    [Tooltip("power up transforms")]
    public Transform[] powerUpSpawns;
    [Tooltip("gun box transforms")]
    public Transform[] gunBoxSpawns;
    [Tooltip("end game laser transforms")]
    public Transform[] endLaserSpawns;
    [Tooltip("spikeBall laser transforms")]
    public Transform[] spikeBallSpawns;

    public Transform cameraDefaultTransform;

    //box prefabs
    [Tooltip("gun box prefab")]
    public GameObject gunBox;
    [Tooltip("power up prefab")]
    public GameObject itemBox;
    [Tooltip("end laser prefab")]
    public GameObject endLaser;
    [Tooltip("spike ball prefab")]
    public GameObject spikeBall;

    private GameObject m_currentItemBox;
    private GameObject m_currentGunBox;

    //used to make random timers
    [Tooltip("minimum time a gun box will appear in")]
    public float minGunTimer;
    [Tooltip("maximum time a gun box will appear in")]
    public float maxGunTimer;

    [Space(10)]

    [Tooltip("minimum time a power up will appear in")]
    public float minPowerUpTimer;
    [Tooltip("maximum time a power up will appear in")]
    public float maxPowerUpTimer;

    [Space(10)]

    [Tooltip("minimum time a spikeball will appear in")]
    public float minSpikeBallTimer;
    [Tooltip("maximum time a power up will appear in")]
    public float maxSpikeBallTimer;

    //regular timers
    [Tooltip("time till the end lasers show up")]
    public float roundTime;
    [Tooltip("time to display danger")]
    public float dangerTimer;

    //
    private bool madeLasers = false;

    /// <summary>
    /// call functions to start coroutines
    /// </summary>
    private void Start()
    {
        StartGunRoutine();
        StartPowerUpRoutine();
        StartSpikeBallRoutine();
    }

    private void Update()
    {
        if (!madeLasers)
        {
            if (roundTime > 0)
            {
                roundTime -= Time.deltaTime;
            }
            else
            {
                MakeLasers();
                madeLasers = true;
            }
        }
        
    }
    /// <summary>
    /// makes the random time to spawn a gun for and then starts the gunbox spawn coroutine with that time
    /// </summary>
    public void StartGunRoutine()
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
        int chosenBox = Random.Range(0, gunBoxSpawns.Length - 1);

        for (int i = 0; i < gunBoxSpawns.Length; i++)
        {
            if (i == chosenBox)
            {
                m_currentGunBox = Instantiate(gunBox, gunBoxSpawns[i].transform);
                m_currentGunBox.GetComponent<PowerUp>().stage = this;
            }
        }

        StartKillGunBox();
    }

    private void StartKillGunBox()
    {
        StartCoroutine(KillGunBoxRoutine());
    }

    private IEnumerator KillGunBoxRoutine()
    {
        if (m_currentGunBox != null)
        {
            yield return new WaitForSeconds(m_currentGunBox.GetComponent<PowerUp>().lifeTime);
            Destroy(m_currentGunBox);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
        StartGunRoutine();
    }

    public void StartSpikeBallRoutine()
    {
        float time = Random.Range(minSpikeBallTimer, maxSpikeBallTimer);
        StartCoroutine(SpawnSpikeBall(time));
    }



    /// <summary>
    /// waits for its time to be over then finds a gunbox transform and spawns a gunbox there then restarts
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator SpawnSpikeBall(float time)
    {
        yield return new WaitForSeconds(time);
        int chosenBall = Random.Range(0, spikeBallSpawns.Length - 1);

        for (int i = 0; i < spikeBallSpawns.Length; i++)
        {
            if (i == chosenBall)
            {
                Instantiate(spikeBall, spikeBallSpawns[i].transform);
            }
        }

        StartKillItemBox();
    }

    private void StartKillItemBox()
    {
        StartCoroutine(KillItemBox());
    }

    private IEnumerator KillItemBox()
    {
        if (m_currentItemBox != null)
        {
            yield return new WaitForSeconds(m_currentItemBox.GetComponent<PowerUp>().lifeTime);
            Destroy(m_currentItemBox);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
        StartPowerUpRoutine();
    }

    /// <summary>
    /// makes the random time to spawn a item box for and then starts the itembox spawn coroutine with that time
    /// </summary>
    public void StartPowerUpRoutine()
    {
        float time = Random.Range(minPowerUpTimer, maxPowerUpTimer);
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
        int chosenBox = Random.Range(0, powerUpSpawns.Length - 1);

        for (int i = 0; i < powerUpSpawns.Length; i++)
        {
            if (i == chosenBox)
            {
                m_currentItemBox = Instantiate(itemBox, powerUpSpawns[i].transform);
                m_currentItemBox.GetComponent<PowerUp>().stage = this;
            }
        }
    }

    private void MakeLasers()
    {
        for (int i = 0; i < endLaserSpawns.Length; i++)
        {
            Instantiate(endLaser, endLaserSpawns[i].transform.position, endLaserSpawns[i].transform.rotation);
        }
        GameManager.Instance.ShowDanger(dangerTimer);
    }

}
