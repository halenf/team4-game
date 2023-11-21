// CameraDistanceHandler - Halen
// Keeps the depth of field focus at the same point in world space.
// last edit 22/11/2023

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraDistanceHandler : MonoBehaviour
{
    private DepthOfField m_depthOfField; // component reference
    private float m_startCameraDistance; // initial depth of the camera
    private float m_defaultFocusDistance; // focus distance set in the post processing volume

    // Start is called before the first frame update
    void Awake()
    {
        m_depthOfField = CameraManager.Instance.postProcessVolume.GetComponent<DepthOfField>();
        m_defaultFocusDistance = m_depthOfField.focusDistance;

        m_startCameraDistance = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the depth of field focus at the same point in world space
        m_depthOfField.focusDistance.value = m_defaultFocusDistance + transform.position.z - m_startCameraDistance;
    }
}
