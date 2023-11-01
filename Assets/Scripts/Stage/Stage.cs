// Stage - Cameron, Halen
// Holds stage properties and details
// last edit 27/10/2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [Header("Spawn Transforms")]
    [Tooltip("put spawn transforms here")]
    public Transform[] spawns;
    [Tooltip("power up transforms")]
    public Transform[] powerUpSpawns;
    [Tooltip("gun box transforms")]
    public Transform[] gunBoxSpawns;

    [Header("Camera Properties")]
    public Transform cameraDefaultTransform;
    [Range(0, 180)] public float cameraFOV;

    [Header("Box Prefabs")]
    public GameObject gunBox;
    public GameObject itemBox;

    [Header("Box spawn timers")]
    [Tooltip("Minimum amount of time for a gun item box to spawn during gameplay.")]
    public float minGunSpawnTimer;
    [Tooltip("Maximum amount of time for a gun item box to spawn during gameplay.")]
    public float maxGunSpawnTimer;

    [Space(10)]

    [Tooltip("Minimum amount of time for a powerup item box to spawn during gameplay.")]
    public float minPowerUpSpawnTimer;
    [Tooltip("Maximum amount of time for a powerup item box to spawn during gameplay.")]
    public float maxPowerUpSpawnTimer;

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
    private void StartGunRoutine()
    {
        float time = Random.Range(minGunSpawnTimer, maxGunSpawnTimer);
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
                Instantiate(gunBox, gunBoxSpawns[i].transform);
            }
        }
        StartGunRoutine();
    }

    /// <summary>
    /// makes the random time to spawn a item box for and then starts the itembox spawn coroutine with that time
    /// </summary>
    private void StartPowerUpRoutine()
    {
        float time = Random.Range(minPowerUpSpawnTimer, maxPowerUpSpawnTimer);
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

        Instantiate(itemBox, powerUpSpawns[chosenBox].transform);

        StartPowerUpRoutine();
    }
}
