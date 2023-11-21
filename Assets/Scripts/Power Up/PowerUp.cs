//PowerUp - Cameron
// just has on collision
// Last edit: 1/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public Stage stage;
    public void Start()
    {
        if (lifeTime != 0)
        {
            Destroy(gameObject, lifeTime);
        }
    }
    public abstract void OnTriggerEnter(Collider other);

    public float lifeTime;
}
