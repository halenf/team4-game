using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public void OnTriggerEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerController victim = other.gameObject.GetComponent<PlayerController>();
            victim.TakeDamage(victim.maxHealth);
        }
    }
}
