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
    public Transform[] playerSpawns;
    [Tooltip("end game laser transforms")]
    public Transform[] endLaserSpawns;

    public Transform cameraDefaultTransform;

    public GameObject endLaserPrefab;

    //regular timers
    [Tooltip("time till the end lasers show up")]
    public float roundTime;

    //
    private bool madeLasers = false;

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

    private void MakeLasers()
    {
        for (int i = 0; i < endLaserSpawns.Length; i++)
        {
            Instantiate(endLaserPrefab, endLaserSpawns[i].transform.position, endLaserSpawns[i].transform.rotation);
        }
        GameManager.Instance.ShowDanger();
    }
}
