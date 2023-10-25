// Lava - Cameron
// just kills player
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerController victim = other.gameObject.GetComponent<PlayerController>();
            victim.TakeDamage(victim.maxHealth);
        }
    }
}
