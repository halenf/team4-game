// Lava - Cameron
// just kills player
// Last edit: 17/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public void Start()
    {
        Vector3 scale = transform.localScale;

        gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scale.x, scale.y);
    }
    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerController victim = other.gameObject.GetComponent<PlayerController>();
            victim.TakeDamage(victim.maxHealth, "death by lava");
        }
    }
}
