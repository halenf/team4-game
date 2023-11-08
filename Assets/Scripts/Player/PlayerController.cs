// Player Controller - Halen, Cameron
// Handles general player info, inputs, and actions
// Last edit: 2/11/23

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // component references
    private Rigidbody m_rb;
    private PlayerInput m_playerInput;
    [SerializeField]
    private GameObject m_shieldObject;
    private GameObject m_shieldGameObject;
    public TMP_Text playerCounter;

    [Header("Default Stats")]
    [Min(0)] public float moveSpeed;
    [Min(0)] public float maxHealth;
    [Min(0)] public float defaultMass;

    // player's current stats
    private float m_currentHealth;
    private float m_fireRate;
    private float m_currentAmmo;
    private float m_nextFireTime;
    private Vector3 m_moveForce;
    private Vector2 m_aimDirection;

    [Header("Gun")]
    public Gun defaultGun;
    private Gun m_currentGun; // gun the player currently has
    [Min(0)] public float gunHoldDistance;
    private bool m_isShooting;

    [Header("Input Properties")]
    public Gamepad controller;

    [Header("Powerup Properties")]
    [SerializeField] private Powerup m_currentPowerup;
    [Min(0)] public float powerupTime;
    [Space(20)]
    [Min(0)] public int maxShieldHealth;
    [Min(1)] public float fireRateScalar;
    [Range(0, 1)] public float lowGravityScalar;

    private float m_powerupTimer;
    private int m_shieldCurrentHealth;

    public enum Powerup
    {
        None,
        Ricochet,
        FireRateUp,
        Shield,
        BigBullets,
        ExplodeBullets,
        LowGravity
    }

    public Powerup currentPowerup
    {
        get { return m_currentPowerup; }
        set
        {
            // reset values
            m_fireRate = m_currentGun.baseFireRate;
            m_shieldCurrentHealth = 0;
            m_rb.mass = defaultMass;
            if (m_shieldGameObject != null) Destroy(m_shieldGameObject);

            // only set timer if the powerup is not shield
            if (value != Powerup.Shield) m_powerupTimer = powerupTime;

            // set powerup
            m_currentPowerup = value;
            switch (m_currentPowerup)
            {
                case Powerup.FireRateUp:
                {
                    m_fireRate *= fireRateScalar;
                    break;
                }
                case Powerup.Shield:
                {
                    m_shieldCurrentHealth = maxShieldHealth;
                    m_shieldGameObject = Instantiate(m_shieldObject, transform);
                    break;
                }
                case Powerup.Ricochet:
                { 
                    break;
                }
                case Powerup.BigBullets:
                {
                    break;
                }
                case Powerup.ExplodeBullets:
                {
                    break;
                }
                case Powerup.LowGravity:
                {
                    m_rb.mass *= lowGravityScalar;
                    break;
                }
                case Powerup.None:
                {
                    break;
                }
            }
        }
    }

    private void Awake()
    {
        // Find attached components 
        m_rb = GetComponent<Rigidbody>();
        m_playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_aimDirection = transform.forward;
        m_rb.mass = defaultMass;
        SetGun(defaultGun);      
    }

    // Update is called once per frame
    void Update()
    {
        // update powerup timer
        if (m_powerupTimer > 0) m_powerupTimer -= Time.deltaTime;
        // if the current powerup isn't the sheld, powerup timer is less than or equal to 0, disable the powerup
        if (currentPowerup != Powerup.Shield && m_powerupTimer <= 0 && currentPowerup != Powerup.None) currentPowerup = Powerup.None;

        // if player is shooting
        if(m_isShooting)
        {
            if (Time.time >= m_nextFireTime) // Only on button press and when the player can fire based on their fire rate
            {
                // old recoil force
                //m_rb.AddForce(m_currentGun.recoil * -Vector3.Normalize(m_aimDirection), ForceMode.Impulse); // Launch player away from where they're aiming

                // Determine if the current powerup affects shooting
                Bullet.BulletEffect bulletEffect;
                switch (m_currentPowerup)
                {
                    case Powerup.Ricochet:
                        bulletEffect = Bullet.BulletEffect.Ricochet;
                        break;
                    case Powerup.BigBullets:
                        bulletEffect = Bullet.BulletEffect.Big;
                        break;
                    case Powerup.ExplodeBullets:
                        bulletEffect = Bullet.BulletEffect.Explode;
                        break;
                    default:
                        bulletEffect = Bullet.BulletEffect.None;
                        break;
                }

                // shoot gun
                m_currentGun.Shoot(GameManager.Instance.GetPlayerID(this), bulletEffect);

                // ammo is only reduced if the player is not holding their default gun
                if (m_currentAmmo != -1) m_currentAmmo--;

                // Set the next time the player can shoot based on their fire rate
                m_nextFireTime = Time.time + (1f / m_fireRate);

                // If the player has run out of ammo, reset their gun
                if (m_currentAmmo == 0) SetGun(defaultGun);
            }
        }
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
            m_currentHealth -= 7;
        }
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        float inputValue = value.ReadValue<Vector2>().x; // Get the direction the player is trying to move
        m_moveForce = moveSpeed * new Vector3(inputValue, 0, 0); // calculate the magnitude of the force
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            m_isShooting = true;
        }
        if (value.canceled)
        {
            m_isShooting = false;
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

    public void OnDisconnect()
    {
        GameManager.Instance.TogglePause(this);
        GameManager.Instance.Disconnected(this);
    }

    public void OnConnect()
    {
        GameManager.Instance.Reconnected();
    }

    public void OnPause(InputAction.CallbackContext value)
    {
        if (value.performed) GameManager.Instance.TogglePause(this);
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
                Destroy(m_shieldGameObject);
            }
            return;
        }
        m_currentHealth -= damage;
        Rumble(.2f, .5f, 1.5f);
        if (m_currentHealth <= 0) // if player is dead
        {
            Rumble(1f, 1f, 120f);
            DisableInput();
            if (GameManager.Instance) GameManager.Instance.deadPlayers++;
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

    /// <summary>
    /// set a power up
    /// </summary>
    /// <param name="powerUp"></param>
   public void ActivatePowerUp(Powerup powerUp)
    {
        currentPowerup = powerUp;
    }

    //start rumble coroutine
    public void Rumble(float lowFrequncy, float highFrequency, float time)
    {
        StartCoroutine(RumbleCoroutine(lowFrequncy, highFrequency, time));
    }

    private IEnumerator RumbleCoroutine(float lowFrequency, float highFrequency, float time)
    {
        controller.SetMotorSpeeds(lowFrequency, highFrequency);
        yield return new WaitForSeconds(time);

        controller.SetMotorSpeeds(0f, 0f);
    }

    /// <summary>
    /// Change the player's controller mappings to a different set.
    /// </summary>
    /// <param name="mapName"></param>
    public void SetControllerMap(string mapName)
    {
        m_playerInput.SwitchCurrentActionMap(mapName);
    }

    /// <summary>
    /// Enable the player's input.
    /// </summary>
    public void EnableInput()
    {
        m_rb.isKinematic = false;
        m_playerInput.ActivateInput();
    }

    /// <summary>
    /// Disable the player's input.
    /// </summary>
    public void DisableInput()
    {
        m_rb.isKinematic = true;
        m_playerInput.DeactivateInput();
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 1.1f);
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
