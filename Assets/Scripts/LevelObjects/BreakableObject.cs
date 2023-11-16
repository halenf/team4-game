// platform - Cameron, Halen
// take damages and dies or explodes
// Last edit: 8/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject[] debris;
    public float debrisDestroyTimer;
    public float health;

    public GameObject destroyEffect;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Only try to create the debris if the object has debris set
            if (debris.Length > 0) foreach (GameObject obj in debris)
            {
                GameObject gameObject = Instantiate(obj, transform.position, Random.rotation);
                Destroy(gameObject, debrisDestroyTimer);
            }
            
            // only create the destroy effect if there is one
            if (destroyEffect)
            {
                GameObject gameObject = Instantiate(destroyEffect, transform.position, Quaternion.identity);
                Destroy(gameObject, debrisDestroyTimer);
            }

            Destroy(gameObject);
        }
    }
}
