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

    public GameObject gunBox;
    public GameObject itemBox;

    public float minGunTimer;
    public float maxGunTimer;

    public float minPowerUpTimer;
    public float maxPowerUpTimer;

    private void Start()
    {
        StartGunRoutine();
        StartPowerUpRoutine();
    }

    private void StartGunRoutine()
    {
        float time = Random.Range(minGunTimer, maxGunTimer);
        StartCoroutine(SpawnGunBox(time));
    }


    private IEnumerator SpawnGunBox(float time)
    {
        yield return new WaitForSeconds(time);
        int chosenBox = Random.Range(0, gunBoxSpawns.Length);

        for (int i = 0; i < gunBoxSpawns.Length; i++)
        {
            if (i == chosenBox)
            {
                Instantiate(gunBox, gunBoxSpawns[i].transform);
            }
        }
        StartGunRoutine();
    }

    private void StartPowerUpRoutine()
    {
        float time = Random.Range(minPowerUpTimer, maxPowerUpTimer);
        StartCoroutine(SpawnPowerUp(time));
    }


    private IEnumerator SpawnPowerUp(float time)
    {
        yield return new WaitForSeconds(time);
        int chosenBox = Random.Range(0, powerUpSpawns.Length);

        for (int i = 0; i < powerUpSpawns.Length; i++)
        {
            if (i == chosenBox)
            {
                Instantiate(itemBox, powerUpSpawns[i].transform);
            }
        }
        StartPowerUpRoutine();
    }

}
