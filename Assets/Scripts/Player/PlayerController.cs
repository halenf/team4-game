using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // component references
    private Rigidbody m_rb;
    private PlayerInput m_playerInput;

    [Header("Default Stats")]
    public float moveSpeed;
    public float maxHealth;

    // player's current stats
    private float m_currentHealth;
    private float m_fireRate;
    private float m_currentAmmo;
    private Vector3 m_moveForce;
    private Vector2 m_aimDirection;

    [Header("Starting Gun")]
    public Gun defaultGun;
    private Gun m_currentGun; // gun the player currently has

    [Header("Aim Indicator")]
    public GameObject aimIndicatorPrefab;
    public float aimIndicatorDistance;
    private GameObject m_aimIndicator; // reference to the indicator after it is instantiated
    
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_playerInput = GetComponent<PlayerInput>();
        if (!m_aimIndicator)
        {
            m_aimIndicator = Instantiate(aimIndicatorPrefab, gameObject.transform);
        }
        m_currentGun = defaultGun;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_rb.AddForce(m_moveForce, ForceMode.Force); // apply the force to the player
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        float inputValue = value.ReadValue<Vector2>().x; // Get the direction the player is holding
        m_moveForce = moveSpeed * new Vector3(inputValue, 0, 0); // calculate the magnitude of the force
    }

    public void OnShoot(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            m_rb.AddForce(m_currentGun.baseRecoil * -m_aimDirection, ForceMode.Impulse);
        }
    }

    public void Aim(InputAction.CallbackContext value)
    {
        m_aimDirection = value.ReadValue<Vector2>();
        Vector3 indicatorPosition = new Vector3(m_aimDirection.x * aimIndicatorDistance, m_aimDirection.y * aimIndicatorDistance, 0);
        m_aimIndicator.transform.localPosition = indicatorPosition;
        m_aimIndicator.transform.rotation = Quaternion.LookRotation(m_aimIndicator.transform.position - m_rb.position);
    }

    /// <summary>
    /// Deal damage to the player and check if they are dead.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        m_currentHealth -= damage;
        if (m_currentHealth <= 0) GameManager.Instance.deadPlayers++;
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
        m_currentGun = defaultGun;
        m_currentAmmo = m_currentGun.ammoCapacity;
        m_fireRate = m_currentGun.baseFireRate;
    }
}
