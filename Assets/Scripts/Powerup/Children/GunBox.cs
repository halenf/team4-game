//Gunbox - Cameron
// sets player gun on collision
// Last edit: 26/10/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBox : PowerUp
{
    public Gun[] guns;

    [Space(10)]

    [SerializeField] private Gun m_currentGun;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        m_currentGun = guns[Random.Range(0, guns.Length)];        
    }

    public override void OnTriggerStay(Collider other)
    {
        //if colliding with player
        if (m_isActive && other.gameObject.CompareTag("Player"))
        {
            //find player
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            player.SetGun(m_currentGun);

            //destroy self
            Destroy(gameObject);
        }
    }
}
