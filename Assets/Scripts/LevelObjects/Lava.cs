// Lava - Cameron, Halen
// Kills player on contact. Hanldes material tiling based on object scale.
// Last edit: 17/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private void Start()
    {
        TileTexture();
    }

    private void Update()
    {
        // update the texture while in the editor
        if (transform.hasChanged)
        {
            TileTexture();
            transform.hasChanged = false;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController victim = other.gameObject.GetComponent<PlayerController>();
            victim.TakeDamage(victim.maxHealth, AnnouncerSubtitleDisplay.AnnouncementType.DeathFire);
        }
    }

    private void TileTexture()
    {
        // use the Y or Z component of the scale if it is bigger. for changing the tiling method based on if the lava is a floor or wall piece
        float scaleX = transform.localScale.x;
        float scaleY;
        if (transform.localScale.y > transform.localScale.z)
        {
            scaleY = transform.localScale.y;
            //scaleX = 1;
        }
        else scaleY = transform.localScale.z;
        
        // tile the texture correctly on the lava
        GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(scaleX, scaleY);
    }
}
