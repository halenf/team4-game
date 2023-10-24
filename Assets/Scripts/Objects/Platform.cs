using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject[] debris;
    public int debrisCount;
    public float health;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            for (int i = 0; i < debrisCount; i++)
            {
                int random = Random.Range(0, debris.Length);
                GameObject currentDebris = Instantiate(debris[random], transform.position, Quaternion.identity);
                Destroy(currentDebris, 2.0f);
            }
            Destroy(gameObject);
        }
    }
}
