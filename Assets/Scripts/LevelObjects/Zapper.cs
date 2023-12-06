using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zapper : Obstacle
{
    [Tooltip("particle object")]
    public GameObject electricity;


    private void Update()
    {
        if(isActive)
        {
            electricity.SetActive(true);
        }
        else
        {
            electricity.SetActive(false);
        }
    }
}
