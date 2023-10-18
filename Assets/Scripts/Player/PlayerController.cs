using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    
    public float moveSpeed;
    public float maxHealth;

    private float currentHealth;
    public float fireRate;
    public float currentAmmo;

    public Gun defaultGun;
    public Gun currentGun;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        rb.AddForce(new Vector3(value.ReadValue<Vector2>().x * moveSpeed, value.ReadValue<Vector2>().y * moveSpeed, 0), ForceMode.Impulse);
    }

    public void OnShoot(InputAction.CallbackContext value)
    {

    }

    public void Aim(InputAction.CallbackContext value)
    {

    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Debug.Log(name + " player is dead.");
    }

    public void EnableInput()
    {
        Debug.Log(name + " player input enabled.");
    }

    public void DisableInput()
    {
        Debug.Log(name + " player input disabled.");
    }

    public void ResetPlayer()
    {
        currentHealth = maxHealth;

    }
}
