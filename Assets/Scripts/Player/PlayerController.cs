// Player Controller - Halen, Cameron
// Handles general player info, inputs, and actions
// Last edit: 25/10/23

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // component references
    private Rigidbody m_rb;
    private PlayerInput m_playerInput;
    [SerializeField]
    private GameObject m_sheildObject;

    [Header("Default Stats")]
    [Min(0)] public float moveSpeed;
    [Min(0)] public float maxHealth;

    // player's current stats
    private float m_currentHealth;
    private float m_fireRate;
    private float m_currentAmmo;
    private float m_nextFireTime;
    private Vector3 m_moveForce;
    private Vector2 m_aimDirection;

    [Header("Powerup Stats")]
    public float ricochetTimer;
    public float fireRateMultiplier;
    public float fireRateTimer;
    public int shieldHealth;

    // powerup tracking
    public float powerUpTime;
    private float m_powerupTimer;
    private int m_shieldCurrentHealth;
    public enum Powerup
    {
        None,
        Ricochet,
        FireRateUp,
        Shield
    }
    private Powerup m_currentPowerup;
    public Powerup currentPowerup
    {
        get { return m_currentPowerup; }
        set
        {
            m_currentPowerup = value;

            if (m_currentPowerup == Powerup.FireRateUp)
            {
                m_fireRate *= fireRateMultiplier;
                m_shieldCurrentHealth = 0;
                m_sheildObject.SetActive(false);
                m_powerupTimer = powerUpTime;
            }
            else if (m_currentPowerup == Powerup.Shield)
            {
                m_shieldCurrentHealth = shieldHealth;
                m_sheildObject.SetActive(true);
                m_fireRate = m_currentGun.baseFireRate;
                m_powerupTimer = powerUpTime;
            } else if (m_currentPowerup == Powerup.Ricochet)
            {
                m_fireRate = m_currentGun.baseFireRate;
                m_shieldCurrentHealth = 0;
                m_sheildObject.SetActive(false);
                m_powerupTimer = powerUpTime;
            } else if (m_currentPowerup == Powerup.None)
            {
                m_fireRate = m_currentGun.baseFireRate;
                m_shieldCurrentHealth = 0;
                m_sheildObject.SetActive(false);
            }
            
        }
    }

    [Header("Gun")]
    public Gun defaultGun;
    private Gun m_currentGun; // gun the player currently has
    [Min(0)] public float gunHoldDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_playerInput = GetComponent<PlayerInput>();
        m_aimDirection = transform.forward;
        SetGun(defaultGun);

    }

    // Update is called once per frame
    void Update()
    {
        // 
        if (m_powerupTimer > 0) m_powerupTimer -= Time.deltaTime;
        if (currentPowerup != Powerup.Shield && m_powerupTimer <= 0 && currentPowerup != Powerup.None) currentPowerup = Powerup.None;

    }

    // FixedUpdate is called once per physic frame
    void FixedUpdate()
    {
        m_rb.AddForce(m_moveForce, ForceMode.Force); // apply the force to the player
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Spike Ball")
        {
            maxHealth -= 7;
        }
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        float inputValue = value.ReadValue<Vector2>().x; // Get the direction the player is trying to move
        m_moveForce = moveSpeed * new Vector3(inputValue, 0, 0); // calculate the magnitude of the force
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (value.performed && Time.time >= m_nextFireTime) // Only on button press and when the player can fire based on their fire rate
        {
            if (m_currentAmmo != -1) m_currentAmmo--; //Cameron

            //m_rb.AddForce(m_currentGun.recoil * -Vector3.Normalize(m_aimDirection), ForceMode.Impulse); // Launch player away from where they're aiming
            bool ricochet = false;
            if (m_currentPowerup == Powerup.Ricochet) ricochet = true;

            m_currentGun.Shoot(gameObject.GetInstanceID(), ricochet);

            m_nextFireTime = Time.time + (1f / m_fireRate); // Set the next time the player can shoot based on their fire rate

            if (m_currentAmmo == 0)
            {
                SetGun(defaultGun);
            }
            
        }
    }

    public void Aim(InputAction.CallbackContext value)
    {
        if (value.ReadValue<Vector2>().sqrMagnitude != 0) // if the player is not aiming, keep their last known aim direction
            m_aimDirection = Vector3.Normalize(value.ReadValue<Vector2>()); // Get the direction the player is aiming

        // Set the position and rotation of the aim indicator
        Vector3 indicatorPosition = new Vector3(m_aimDirection.x * gunHoldDistance, m_aimDirection.y * gunHoldDistance, 0);
        m_currentGun.transform.localPosition = indicatorPosition;
        m_currentGun.transform.rotation = Quaternion.LookRotation(m_currentGun.transform.position - m_rb.position);
    }

    /// <summary>
    /// Deal damage to the player and check if they are dead.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if (m_shieldCurrentHealth > 0)
        {
            m_shieldCurrentHealth--;
            if (m_shieldCurrentHealth == 0)
            {
                m_sheildObject.SetActive(false);
            }
            return;
        }
        m_currentHealth -= damage;
        if (m_currentHealth <= 0) // if player is dead
        {
            DisableInput();
            if (GameManager.Instance)
                GameManager.Instance.deadPlayers++;
        }
    }

    /// <summary>
    /// Set's the player's current gun to a specified gun.
    /// </summary>
    /// <param name="gun"></param>
    public void SetGun(Gun gun)
    {
        if (m_currentGun) Destroy(m_currentGun.gameObject);
        m_currentGun = Instantiate(gun, gameObject.transform);

        // Only sets an ammo capacity if the gun is a pickup gun and not the default
        if (gun != defaultGun) m_currentAmmo = gun.ammoCapacity;
        else m_currentAmmo = -1;

        m_fireRate = m_currentGun.baseFireRate;
        m_nextFireTime = Time.time;
    }


   public void ActivateRicochet()
    {
        currentPowerup = Powerup.Ricochet;
    }

    public void IncreaseFireRate()
    {
        currentPowerup = Powerup.FireRateUp;
    }

    public void ActivateSheild()
    {
        currentPowerup = Powerup.Shield;
    }

    /// <summary>
    /// Change the player's controller mappings to a different set.
    /// </summary>
    /// <param name="mapName"></param>
    public void SetControllerMap(string mapName)
    {
        
    }

    /// <summary>
    /// Enable the player's input.
    /// </summary>
    public void EnableInput()
    {
        Debug.Log(name + " player input enabled.");
        m_playerInput.ActivateInput();
    }

    /// <summary>
    /// Disable the player's input.
    /// </summary>
    public void DisableInput()
    {
        Debug.Log(name + " player input disabled.");
        m_playerInput.DeactivateInput();
    }

    /// <summary>
    /// Reset all active variables like health, ammo, and current gun.
    /// </summary>
    public void ResetPlayer()
    {
        m_currentHealth = maxHealth;
        SetGun(defaultGun);
        m_currentAmmo = m_currentGun.ammoCapacity;
        m_fireRate = m_currentGun.baseFireRate;
    }
}
