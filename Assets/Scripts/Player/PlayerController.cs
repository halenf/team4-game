// Player Controller - Halen, Cameron
// Handles general player info, inputs, and actions
// Last edit: 29/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // component references
    private Rigidbody m_rb;
    private CapsuleCollider m_collider;
    private PlayerInput m_playerInput;
    private RigBuilder m_rigBuilder;
    private Gamepad m_controller;
    private Color m_color;
    private Animator m_animator;

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

    [Header("Grounded Boxcast Properties")]
    [SerializeField] private LayerMask m_detectLayerMask;
    public bool isGrounded;

    [Header("Gun")]
    [SerializeField] private Transform m_gunTransform;
    public Gun defaultGun;
    private Gun m_currentGun; // gun the player currently has
    [Min(0)] public float gunHoldDistance;
    private bool m_isShooting;
    private Vector3 m_indicatorPosition = new(1f, 0f, 0);
    [Space(5)]
    

    [Header("Powerup Properties")]
    [SerializeField] private Powerup m_currentPowerup;
    [Min(0)] public float[] powerupTime;

    [Space(10)]

    [Min(0)] public int maxShieldHealth;
    [Min(1)] public float fireRateScalar;
    [Range(0, 1)] public float lowGravityScalar;
    [Min(1)] public int ricochetBounces;

    [Space(10)]

    public GameObject shieldPrefab;
    public GameObject ashPrefab;
    private GameObject m_shieldGameObject;

    private float m_powerupTimer;
    private int m_shieldCurrentHealth;

    [Header("Pickup Display")]
    public PickupIndicator indicatorCanvasPrefab;
    [Min(0)] public float indicatorSpawnHeight;
    public Sprite[] powerupIndicators;
    public Color[] powerupColours;
    public Sprite deathIndicator;

    [Header("Particle Effects")]
    public ParticleSystem bloodPrefab;
    [Tooltip("particle that plays randomly when the player is low health")]
    public ParticleSystem damagedParticlePrefab;
    [Tooltip("places where the sparks will play")]
    public Transform[] sparkLocations;
    [Tooltip(" minimum time between when damage sparks play on the player")]
    public float minSparkTimer;
    [Tooltip("maximum time between when damage sparks play on the player")]
    public float maxSparkTimer;
    private ParticleSystem m_sparkReference;

    [Header("Animation")]
    [Tooltip("The horizontal speed threshold at which the player is detected as 'moving'.")]
    [Min(0)] public float horizontalVelocityThreshold;
    [Tooltip("The threshold at which the direction the player is facing will change.")]
    [Range(0,1)] public float horizontalAimingThreshold;
    private float m_stoppedMovingTimer; // tracks how long the player has stopped moving for
    private bool m_facingRight; // tracks the direction the player is facing
    [Space(5)]
    [SerializeField] private TransformUpdater m_leftHandTarget;
    [SerializeField] private TransformUpdater m_rightHandTarget;
    [SerializeField] private TransformUpdater m_lookTarget;

    public bool facingRight // for model rotation
    {
        get { return m_facingRight; }
        set
        {
            if (value) m_animator.gameObject.transform.localRotation = Quaternion.Euler(0, 90, 0);
            else m_animator.gameObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
            m_facingRight = value;
        }
    }

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
            // only set timer if the powerup is not shield
            if (value != Powerup.Shield) m_powerupTimer = powerupTime[(int)value];

            // Deactivation checks
            if (value == Powerup.None)
            {
                switch (m_currentPowerup)
                {
                    case Powerup.LowGravity:
                        {
                            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-LOWGRAVITYDEACTIVATE");
                            m_rb.mass = defaultMass;
                            break;
                        }
                    case Powerup.FireRateUp:
                        {
                            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-RAPIDFIREDEACTIVATE");
                            m_fireRate = m_currentGun.baseFireRate;
                            break;
                        }
                    case Powerup.Shield:
                        {
                            m_shieldCurrentHealth = 0;
                            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Powerup-Ups/PWR-SHIELDDEACTIVATE");
                            if (m_shieldGameObject) Destroy(m_shieldGameObject);
                            break;
                        }
                }
            }

            // set powerup
            m_currentPowerup = value;

            // create indicator
            if (m_currentPowerup != Powerup.None)
            {
                CreateOverhead(powerupIndicators[(int)m_currentPowerup - 1], powerupColours[(int)m_currentPowerup - 1]);
                //Debug.Log("Spawned indicator for " + m_currentPowerup.ToString());
            }

            switch (m_currentPowerup)
            {
                case Powerup.Ricochet:
                    {
                        SoundManager.Instance.PlayAudioAtPoint(transform.position, "Power-Ups/PWR-BULLETRICOCHET");
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
        m_collider = GetComponent<CapsuleCollider>();
        m_playerInput = GetComponent<PlayerInput>();
        m_rigBuilder = GetComponentInChildren<RigBuilder>();
        m_animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // set initial direction to face based on ID
        if ((id + 1) % 2 == 0) facingRight = false;
        else facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        m_currentGun.transform.localPosition = m_indicatorPosition;
        m_currentGun.transform.rotation = Quaternion.LookRotation(m_currentGun.transform.position - m_gunTransform.position);

        // update powerup timer
        if (m_powerupTimer > 0) m_powerupTimer -= Time.deltaTime;

        // if the current powerup isn't the sheld, powerup timer is less than or equal to 0, and isn't already "none", disable the powerup
        if (currentPowerup != Powerup.Shield && m_powerupTimer <= 0 && currentPowerup != Powerup.None) currentPowerup = Powerup.None;

        // if player is shooting
        if (m_isShooting)
        {
            if (Time.time >= m_nextFireTime) // Only on button press and when the player can fire based on their fire rate
            {
                // if m_currentPowerup == left side, bulletEffect = right side
                // _ is default
                var bulletEffect = m_currentPowerup switch
                {
                    Powerup.Ricochet => Bullet.BulletEffect.Ricochet,
                    Powerup.BigBullets => Bullet.BulletEffect.Big,
                    Powerup.ExplodeBullets => Bullet.BulletEffect.Explode,
                    _ => Bullet.BulletEffect.None,
                };

                // shoot gun
                m_currentGun.Shoot(id, bulletEffect, ricochetBounces);

                // ammo is only reduced if the player is not holding their default gun
                if (m_currentAmmo != -1) m_currentAmmo--;

                // Set the next time the player can shoot based on their fire rate
                m_nextFireTime = Time.time + (1f / m_fireRate);

                // If the player has run out of ammo, reset their gun
                if (m_currentAmmo == 0) SetGun(defaultGun);
            }
        }

        // to avoid entering the idle state while swapping movement direction
        bool isMoving = Mathf.Abs(m_rb.velocity.x) >= horizontalVelocityThreshold;

        if (!isMoving) m_stoppedMovingTimer += Time.deltaTime;
        else m_stoppedMovingTimer = 0;

        m_animator.SetBool("IsMoving", isMoving);
        m_animator.SetFloat("StoppedMovingTimer", m_stoppedMovingTimer);

        // if player is grounded
        m_animator.SetBool("IsGrounded", isGrounded);

        // pass player vertical speed to animator
        m_animator.SetFloat("VerticalSpeed", Mathf.Abs(m_rb.velocity.y));
    }

    // FixedUpdate is called once per physic frame
    void FixedUpdate()
    {
        m_rb.AddForce(m_moveForce, ForceMode.Force); // apply the movement force to the player
        isGrounded = IsGrounded(); // update if player is grounded
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        float inputValue = value.ReadValue<Vector2>().x; // Get the direction the player is trying to move
        m_moveForce = moveSpeed * new Vector3(inputValue, 0, 0); // calculate the magnitude of the force

        // update animator parameter
        m_animator.SetFloat("HorizontalInput", Mathf.Abs(inputValue));

        // Check if the player is facing the opposite direction they are moving in
        m_animator.SetFloat("Direction", facingRight == (inputValue >= 0) ? 1f : -1f);
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

        // update model rotation when the threshold is met
        float inputValue = value.ReadValue<Vector2>().x;
        if (inputValue >= horizontalAimingThreshold) facingRight = true;
        if (inputValue <= -horizontalAimingThreshold) facingRight = false;
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
    /// <param name="damage">Amount of damage taken by the player.</param>
    /// <param name="announcementType">If lethal damage, the damage type that killed the player.</param>
    public void TakeDamage(float damage, AnnouncerSubtitleDisplay.AnnouncementType announcementType)
    {
        // if the player is already dead, don't make them take damage
        if (isDead) return;

        // shield will block damage
        if (m_shieldCurrentHealth > 0)
        {
            m_shieldCurrentHealth--;
            
            if (m_shieldCurrentHealth == 0)
            {
                currentPowerup = Powerup.None;
            }
            else
            {
                SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/PWR-SHIELDDEFLECT");
            }

            return;
        }

        // deal damage
        m_currentHealth -= damage;

        //if the player has been damaged enough start playing sparks
        if(m_currentHealth <= 4 && m_currentHealth >= 0.05f)
        {
            StartCoroutine(PlayDamageParticles());
        }

        //set emmisoin
        GetComponentInChildren<SetColour>().Set(m_color, m_currentHealth / maxHealth);

        // rumble controller and play damage sound when damage is taken
        if (m_currentHealth > 0)
        {
            Rumble(.2f, .4f, 0.6f);
            SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/SFX-PLAYERDAMAGE");
        }
        else // if player is dead
        {
            // explode into blood
            for (int i = 0; i < 1 + Mathf.CeilToInt(damage); i++)
                Instantiate(bloodPrefab, transform.position, Random.rotation);

            //make death indicator
            CreateOverhead(deathIndicator, GetComponent<SetColour>().GetColour());

            // remove this player from the target group
            GameManager.Instance.UpdateCameraTargetGroup();

            // Have announcer say something stupid
            GameManager.Instance.StartAnnouncement(announcementType);

            switch (announcementType)
            {
                case AnnouncerSubtitleDisplay.AnnouncementType.DeathFire:
                    {
                        GameObject ash = Instantiate(ashPrefab, transform.position, Quaternion.identity);
                        ash.transform.parent = FindObjectOfType<Stage>().gameObject.transform;
                        SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/SFX-PLAYERDEATHASH");
                        break;
                    }
                case AnnouncerSubtitleDisplay.AnnouncementType.DeathZapper:
                    {
                        GameObject ash = Instantiate(ashPrefab, transform.position, Quaternion.identity);
                        ash.transform.parent = FindObjectOfType<Stage>().gameObject.transform;
                        SoundManager.Instance.PlayAudioAtPoint(transform.position, "Obstacles/SFX-ZAPPERELECTRIFY");
                        break;
                    }
                default:
                    {
                        SoundManager.Instance.PlayAudioAtPoint(transform.position, "Player/SFX-PLAYERDEATHBLOODY");
                        break;
                    }
            }

            // let game manager know somebody died
            GameManager.Instance.CheckIsRoundOver();

            // play sound effect
            SoundManager.Instance.PlayAfterTime("Crowd/AMB-CROWDCHEERUPONDEATH", 2f);

            // make controller rumble
            GameManager.Instance.StartControllerRumbleRoutine(id, 0.3f, 0.6f, 0.6f);

            // deactivate player object
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Set's the player's current gun to a specified gun.
    /// </summary>
    /// <param name="gun"></param>
    public void SetGun(Gun gun)
    {
        if (m_currentGun) Destroy(m_currentGun.gameObject);
        m_currentGun = Instantiate(gun, m_gunTransform);

        if (gun != defaultGun)
        {
            // Only sets an ammo capacity if the gun is a pickup gun and not the default
            m_currentAmmo = gun.ammoCapacity;

            // Display the gun the player picked up. dont display when changing back to the default gun
            CreateOverhead(gun.indicator, gun.colour);
        }
        else m_currentAmmo = -1;

        // set fire rate details
        m_fireRate = m_currentGun.baseFireRate;
        m_nextFireTime = Time.time + (1f / m_fireRate);

        //change gun material
        m_currentGun.ChangeMaterial(id);

        // Sets the gun's aim
        Vector3 indicatorPosition = new(m_aimDirection.x * gunHoldDistance, m_aimDirection.y * gunHoldDistance, 0);
        m_currentGun.transform.localPosition = indicatorPosition;
        m_currentGun.transform.rotation = Quaternion.LookRotation(m_currentGun.transform.position - m_gunTransform.position);

        // attach the player's hands to the gun
        m_leftHandTarget.target = m_currentGun.leftHandPosition;
        m_rightHandTarget.target = m_currentGun.rightHandPosition;

        // have the player look down the gun's sight/barrel
        m_lookTarget.target = m_currentGun.transform;

        //m_rigBuilder.Build();
    }

    /// <summary>
    /// creates a image above the player and destroys it after amount of seconds
    /// </summary>
    /// <param name="image"></param>
    /// <param name="colour"></param>
    public void CreateOverhead(Sprite image, Color colour)
    {
        PickupIndicator indicatorCanvas = Instantiate(indicatorCanvasPrefab, transform.position + new Vector3(0, indicatorSpawnHeight, 0), Quaternion.identity);
        indicatorCanvas.SetDisplayDetails(image, colour);
    }

    private IEnumerator PlayDamageParticles()
    {
        while (m_currentHealth <= 3 * maxHealth / 4 && m_currentHealth > 0)
        {
            //at one of the positions the player has play the particle effect
            m_sparkReference = Instantiate(damagedParticlePrefab, sparkLocations[Random.Range(0, sparkLocations.Length)]);

            var sparkTrails = m_sparkReference.trails;
            Gradient gradient = new();
            gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(m_color, 0.0f), new GradientColorKey(m_color, 1.0f) },
                             new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
            sparkTrails.colorOverLifetime = gradient;

            //get an amount of time before the next spark based on health
            float time = ((maxSparkTimer - minSparkTimer) * (m_currentHealth / maxHealth)) + minSparkTimer + Random.Range(0f, 4f);
            yield return new WaitForSeconds(time);

            /*
            // lower the player's brightness
            Material particleMat = sparks.GetComponent<Material>();
            particleMat.EnableKeyword("_EMISSION");
            particleMat.SetColor("_EmissionColor", m_color);
            particleMat.SetColor("_Color", m_color);
            */
        }
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

        // reset rigs and their constraint's weights because they get set to 0 when the player is disabled for some reason
        foreach (RigLayer rigLayer in m_rigBuilder.layers)
        {
            Rig rig = rigLayer.rig;
            rig.weight = 1f;
            foreach (TwoBoneIKConstraint constraint in rig.gameObject.GetComponentsInChildren<TwoBoneIKConstraint>())
            {
                constraint.weight = 1f;
                constraint.data.targetPositionWeight = 1f;
                constraint.data.targetRotationWeight = 1f;
                constraint.data.hintWeight = 1f;
            }
            foreach (MultiAimConstraint constraint in rig.gameObject.GetComponentsInChildren<MultiAimConstraint>())
            {
                constraint.weight = 1f;
                constraint.data.sourceObjects = new WeightedTransformArray { new WeightedTransform(constraint.data.sourceObjects.GetTransform(0), 1f) };
                constraint.data.limits = new Vector2(-65f, 65f);
            }
        }
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
        return Physics.BoxCast(transform.position, Vector3.one / 2, Vector3.down, Quaternion.identity, m_collider.height / 2f, m_detectLayerMask);

        // old cast
        //return Physics.Raycast(transform.position, Vector3.down, m_collider.height);
    }

    /// <summary>
    /// Reset all active variables like health, ammo, and current gun.
    /// </summary>
    public void ResetPlayer()
    {
        m_currentHealth = maxHealth;
        m_aimDirection = transform.right;
        SetGun(defaultGun);
        currentPowerup = Powerup.None;
        GetComponentInChildren<SetColour>().Set(m_color, m_currentHealth / maxHealth);
        if (m_shieldGameObject) Destroy(m_shieldGameObject);
    }

    private void OnEnable()
    {
        // Set player colour
        GetComponentInChildren<SetColour>().Set(m_color);
        foreach(Transform sparkLocal in sparkLocations)
        {
            for(int i = 0; i < sparkLocal.childCount; i++)
            {
                Destroy(sparkLocal.GetChild(i).gameObject);
            }
        }
    }

    private void OnDisable()
    {
        // stop the damage particles coroutine
        StopCoroutine(PlayDamageParticles());
    }
}