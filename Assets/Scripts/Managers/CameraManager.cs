using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // camera reference
    public Camera mainCamera;

    /// <summary>
    /// Set the position, rotation, and FOV of the camera.
    /// </summary>
    /// <param name="_transform"></param>
    /// <param name="fov"></param>
    public void SetCameraProperties(Transform _transform, float fov)
    {
        SetPosition(_transform);
        SetFOV(fov);
    }

    private void SetPosition(Transform _transform)
    {
        transform.position = _transform.position;
        transform.rotation = _transform.rotation;
    }

    private void SetFOV(float fov)
    {
        mainCamera.fieldOfView = fov;
    }

    /// <summary>
    /// Cause the camera to shake across the x and y planes for a specified amount of time.
    /// </summary>
    /// <param name="magnitude"></param>
    /// <param name="time"></param>
    public void ScreenShake(float magnitude, float time)
    {
        StartCoroutine(StartScreenShake(magnitude, time));
    }

    private IEnumerator StartScreenShake(float magnitude, float time)
    {
        yield return new WaitForSeconds(time);
    }
}
