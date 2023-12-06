using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zapper : Obstacle
{
    [Tooltip("particle object")]
    public ParticleSystem electricity;

    public override bool isActive
    { 
        get => base.isActive;
        set
        {
            electricity.gameObject.SetActive(value);
            
            base.isActive = value;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerController victim = other.gameObject.GetComponent<PlayerController>();
                victim.TakeDamage(victim.maxHealth, AnnouncerSubtitleDisplay.AnnouncementType.DeathFire);
            }
        }
    }
}
