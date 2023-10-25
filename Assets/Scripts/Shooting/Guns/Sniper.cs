// Sniper - Cameron
// just uses a basic shoot function
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Gun
{
    public override void Shoot(int playerID, bool shouldBounce)
    {
        //find spread rotation change
        Quaternion shootDirection = Quaternion.Euler(Random.Range(-spread, spread), 0, 0);

        // instantiate the bullet
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation * shootDirection);
        bullet.Init(playerID, bulletDamage, shouldBounce, bullet.transform.forward * bulletSpeed);
        // apply recoil to player
        transform.parent.GetComponent<Rigidbody>().AddForce(recoil * -transform.forward, ForceMode.Impulse);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
