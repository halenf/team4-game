using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;

    [Header("Bullet Properties")]
    public float spread;
    public float bulletSpeed;
    public float bulletDamage;
    public bool shouldBounce;

    [Header("Player Properties")]
    public float baseRecoil;
    public float baseFireRate;
    public float ammoCapacity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(bool canBounce)
    {

    }
}
