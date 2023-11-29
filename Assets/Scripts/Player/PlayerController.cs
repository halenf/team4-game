// Player Controller - Halen, Cameron
// Handles general player info, inputs, and actions
// Last edit: 15/11/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // component references
    private Rigidbody m_rb;
    private PlayerInput m_playerInput;
    private Gamepad m_controller;
    private Color m_color;

    // player ID
    public int id;

    [Space(10)]

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
    private Vector2 m_aimDirection = Vector3.right;

    public bool isDead
    {
        get { return m_currentHealth <= 0; }
    }

    [Header("Gun")]
    public Gun defaultGun;
    private Gun m_currentGun; // gun the player currently has
    [Min(0)] public float gunHoldDistance;
    private bool m_isShooting;
    private Vector3 m_indicatorPosition = new(1f, 0f, 0);

    [Header("Powerup Properties")]
    [SerializeField] private Powerup m_currentPowerup;
    [Min(0)] public float powerupTime;
    
    [Space(10)]

    [Min(0)] public int maxShieldHealth;
    [Min(1)] public float fireRateScalar;
    [Range(0, 1)] public float lowGravityScalar;
    [Min(1)] public int riccochetBounces;

    [Space(10)]

    public GameObject shieldPrefab;
    private GameObject m_shieldGameObject;

    private float m_powerupTimer;
    private int m_shieldCurrentHealth;
    
    [Header("Pickup Display")]
    public PickupIndicator indicatorCanvasPrefab;
    [Min(0)] public float indicatorSpawnHeight;
    [Min(0)] public float indicatorLifetime;
    public Sprite[] powerupIndicators;
    public Color[] powerupColours;

    [Header("Particle Effects")]
    public ParticleSystem bloodPrefab;

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
            if (m_shieldGameObject) Destroy(m_shieldGameObject);

            // only set timer if the powerup is not shield
            if (value != Powerup.Shield) m_powerupTimer = powerupTime;

            // Deactivation checks
            if (value == Powerup.None)
            {
                switch (m_currentPowerup)
                {
                    case (Powerup.LowGravity):
                        {
                            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-LOWGRAVITYDEACTIVATE");
                            break;
                        }
                    case (Powerup.FireRateUp):
                        {
                            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-RAPIDFIREDEACTIVATE");
                            break;
                        }
                }
            }

            // set powerup
            Powerup oldPowerUp = m_currentPowerup;
            m_currentPowerup = value;

            // create indicator
            if (m_currentPowerup != Powerup.None)
            {
                CreateOverhead(powerupIndicators[(int)m_currentPowerup - 1], powerupColours[(int)m_currentPowerup - 1]);
            }

            switch (m_currentPowerup)
            {
                case Powerup.Ricochet:
                {
                    break;
                }
                case Powerup.FireRateUp:
                {
                    SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-RAPIDFIREACTIVATE");
                    m_fireRate *= fireRateScalar;
                    break;
                }
                case Powerup.Shield:
                {
                    SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-SHIELDACTIVATE");
                    m_shieldCurrentHealth = maxShieldHealth;
                    m_shieldGameObject = Instantiate(shieldPrefab, transform);
                    break;
                }
                case Powerup.BigBullets:
                {
                    SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-BIGBULLETSACTIVATE");
                    break;
                }
                case Powerup.ExplodeBullets:
                {
                    break;
                }
                case Powerup.LowGravity:
                {
                    SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-LOWGRAVITYACTIVATE");
                    m_rb.mass *= lowGravityScalar;
                    break;
                }
                case Powerup.None:
                {
                    m_powerupTimer = 0f;
                    
                    
                    break;
                }
            }
        }
    }

    public void Init(Gamepad _controller, int _id, Color colour)
    {
        m_controller = _controller;
        id = _id;
        m_color = colour;
        GetComponentInChildren<SetColour>().Set(m_color); // Set player colour
    }

    private void Awake()
    {
        // Find attached components 
        m_rb = GetComponent<Rigidbody>();
        m_rb.mass = defaultMass;
        m_playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_aimDirection = transform.right;
        SetGun(defaultGun);      
    }

    // Update is called once per frame
    void Update()
    {
        m_currentGun.transform.localPosition = m_indicatorPosition;
        m_currentGun.transform.rotation = Quaternion.LookRotation(m_currentGun.transform.position - transform.position);

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
                m_currentGun.Shoot(id, bulletEffect, riccochetBounces);

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
            TakeDamage(7f, AnnouncerSubtitleDisplay.AnnouncementType.DeathSpikeball);
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
        m_indicatorPosition = new Vector3(m_aimDirection.x * gunHoldDistance, m_aimDirection.y * gunHoldDistance, 0);
        
    }

    public void OnDisconnect()
    {
        //GameManager.Instance.TogglePause(id);
        GameManager.Instance.Disconnected(id);
    }

    public void OnConnect()
    {
        GameManager.Instance.Reconnected();
    }

    public void OnPause(InputAction.CallbackContext value)
    {
        if (value.performed) GameManager.Instance.TogglePause(id);
    }

    /// <summary>
    /// Deal damage to the player and check if they are dead.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage, AnnouncerSubtitleDisplay.AnnouncementType announcementType)
    {
        // if the player is already dead, don't make them take damage
        if (isDead) return;
        
        // shield will block damage
        if (m_shieldCurrentHealth > 0)
        {
            m_shieldCurrentHealth--;
            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/PWR-SHIELDDEFLECT");
            if (m_shieldCurrentHealth == 0)
            {
                SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/PWR-SHIELDDEACTIVATE");
                Destroy(m_shieldGameObject);
            }
            return;
        }

        // deal damage
        m_currentHealth -= damage;

        //set emmisoin
        GetComponentInChildren<SetColour>().Set(m_color, m_currentHealth/maxHealth);

        // rumble controller
        Rumble(.2f, .5f, 1.5f);

        // if player is dead
        if (m_currentHealth <= 0)
        {
            // explode into blood
            for (int i = 0; i < 1 + Mathf.CeilToInt(damage); i++)
                Instantiate(bloodPrefab, transform.position, Random.rotation);

            // Play death sound
            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/SFX-PLAYERDEATHBLOODY");

            // remove this player from the target group
            GameManager.Instance.UpdateCameraTargetGroup();

            // Have announcer say something stupid
            GameManager.Instance.StartAnnouncement(announcementType);

            // let game manager know somebody died
            GameManager.Instance.CheckIsRoundOver();

            // deactivate player object
            gameObject.SetActive(false);
        }
        else
        {
            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/SFX-PLAYERDAMAGE");
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

        if (gun != defaultGun)
        {
            // Only sets an ammo capacity if the gun is a pickup gun and not the default
            m_currentAmmo = gun.ammoCapacity;

            // Display the gun the player picked up. dont display when changing back to the pistol
            CreateOverhead(gun.indicator, gun.colour);
        }
        else m_currentAmmo = -1;

        // set fire rate details
        m_fireRate = m_currentGun.baseFireRate;
        m_nextFireTime = Time.time + (1f / m_fireRate);

        //change gun material
        m_currentGun.ChangeMaterial(id);

        // Sets the gun's aim
        Vector3 indicatorPosition = new Vector3(m_aimDirection.x * gunHoldDistance, m_aimDirection.y * gunHoldDistance, 0);
        m_currentGun.transform.localPosition = indicatorPosition;
        m_currentGun.transform.rotation = Quaternion.LookRotation(m_currentGun.transform.position - m_rb.position);
    }

    /// <summary>
    /// creates a image above the player and destroys it after amount of seconds
    /// </summary>
    /// <param name="image"></param>
    public void CreateOverhead(Sprite image, Color colour)
    {
        PickupIndicator indicatorCanvas = Instantiate(indicatorCanvasPrefab, transform.position + new Vector3(0, indicatorSpawnHeight, 0), Quaternion.identity);
        indicatorCanvas.SetDisplayDetails(indicatorLifetime, image, colour);
        Destroy(indicatorCanvas, indicatorLifetime);
    }

    /// <summary>
    /// start rumble coroutine
    /// </summary>
    /// <param name="lowFrequncy"></param>
    /// <param name="highFrequency"></param>
    /// <param name="time"></param>
    public void Rumble(float lowFrequncy, float highFrequency, float time)
    {
        StartCoroutine(RumbleCoroutine(lowFrequncy, highFrequency, time));
    }

    private IEnumerator RumbleCoroutine(float lowFrequency, float highFrequency, float time)
    {
        m_controller.SetMotorSpeeds(lowFrequency, highFrequency);
        yield return new WaitForSeconds(time);

        m_controller.SetMotorSpeeds(0f, 0f);
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

    /// <summary>
    /// check for collisions 1.1 in the down direction
    /// </summary>
    /// <returns></returns>
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
        m_fireRate = m_currentGun.baseFireRate;
        if (m_shieldGameObject) Destroy(m_shieldGameObject);
    }

    private void OnEnable()
    {
        GetComponentInChildren<SetColour>().Set(m_color); // Set player colour
    }
}
