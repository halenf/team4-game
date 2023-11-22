//PowerUp - Cameron
// just has on collision
// Last edit: 1/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public float lifeTime;

    public void Start()
    {
        if (lifeTime != 0)
        {
            Destroy(gameObject, lifeTime);
        }

        OnStart();
    }

    /// <summary>
    /// Add any code you need to run in Start here.
    /// </summary>
    public virtual void OnStart() { }

    public abstract void OnTriggerEnter(Collider other);

    
}
