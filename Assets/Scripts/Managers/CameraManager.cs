// CameraManager - Halen
// Manages the main camera and its attached post processing volume
// last edit 22/11/2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    // Singleton instantiation
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Camera Objects")]
    public Camera mainCamera;
    public CinemachineVirtualCamera gameplayCamera;

    [Header("Post Processing")]
    public PostProcessVolume postProcessVolume;

    // timer for screen-shake
    private float m_timer;

    /// <summary>
    /// Set the position, rotation, and FOV of the camera.
    /// </summary>
    /// <param name="_transform"></param>
    public void SetCameraPosition(Transform _transform)
    {
        gameplayCamera.transform.position = _transform.position;
        gameplayCamera.transform.rotation = _transform.rotation;
    }

    /// <summary>
    /// Cause the camera to shake across the x and y planes for a specified amount of time.
    /// </summary>
    /// <param name="magnitude"></param>
    /// <param name="intensity"></param>
    /// <param name="time"></param>
    public void ScreenShake(float magnitude, float intensity, float time)
    {
        StartCoroutine(StartScreenShake(magnitude, intensity, time));
    }

    private IEnumerator StartScreenShake(float magnitude, float intensity, float time)
    {
        m_timer = time;
        while (m_timer > 0)
        {
            // Get a point for the camera to shake to
            Vector2 displacement = Random.insideUnitCircle.normalized * magnitude * m_timer / time;

            // shake that thang
            mainCamera.transform.localPosition = displacement;

            yield return new WaitForSeconds(1f / intensity);
        }

        // reset camera position
        mainCamera.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        // Always be updating the timer if it is active
        if (m_timer > 0) m_timer -= Time.deltaTime;

        if (Keyboard.current.digit4Key.isPressed) ScreenShake(3.5f, 5, 1.2f);
    }
}
