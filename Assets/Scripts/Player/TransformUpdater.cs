using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdater : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target) transform.position = target.position;
    }
}
