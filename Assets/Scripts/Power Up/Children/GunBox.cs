using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBox : PowerUp
{
    public Gun[] Guns;

    private Gun currentGun;

    // Start is called before the first frame update
    void Start()
    {
        currentGun = Guns[Random.Range(0, Guns.Length)];
    }

    public override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            //find player
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            player.SetGun(currentGun);

            //destroy self
            Destroy(gameObject);
        }
    }
}
