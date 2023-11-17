//Gunbox - Cameron
// sets player gun on collision
// Last edit: 26/10/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBox : PowerUp
{
    public Gun[] Guns;

    private Gun currentGun;

    // Start is called before the first frame update
    void Awake()
    {
        currentGun = Guns[Random.Range(0, Guns.Length)];        
        
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //find player
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            player.SetGun(currentGun);

            player.CreateOverhead(currentGun.gameObject);

            //destroy self
            Destroy(gameObject);
        }
    }
}
