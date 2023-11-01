//stage - Cameron
//just stores spawn locations
// last edit 27/10/2023
using System.Collections;
using System.Collections.Generic;
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

    //box prefabs
    public GameObject gunBox;
    public GameObject itemBox;

    //used to make random timers
    public float minGunTimer;
    public float maxGunTimer;

    public float minPowerUpTimer;
    public float maxPowerUpTimer;

    /// <summary>
    /// call functions to start coroutines
    /// </summary>
    private void Start()
    {
        StartGunRoutine();
        StartPowerUpRoutine();
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
        int chosenBox = Random.Range(0, gunBoxSpawns.Length);

        for (int i = 0; i < gunBoxSpawns.Length; i++)
        {
            if (i == chosenBox)
            {
                Instantiate(gunBox, gunBoxSpawns[i].transform).GetComponent<PowerUp>().stage = this;
            }
        }
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
        int chosenBox = Random.Range(0, powerUpSpawns.Length);

        for (int i = 0; i < powerUpSpawns.Length; i++)
        {
            if (i == chosenBox)
            {
                Instantiate(itemBox, powerUpSpawns[i].transform).GetComponent<PowerUp>().stage = this;
            }
        }
    }

}
