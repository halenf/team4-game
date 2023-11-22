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

    private void Start()
    {
        m_depthOfField = CameraManager.Instance.postProcessVolume.profile.GetSetting<DepthOfField>();
        m_defaultFocusDistance = m_depthOfField.focusDistance;
        //Debug.Log("Focus distance: " + m_defaultFocusDistance);
    }

    // Whenever the gameplay camera is enabled, get the position data
    private void OnEnable()
    {   
        m_startCameraDistance = transform.position.z;
        //Debug.Log("Start Camera distance: " + m_startCameraDistance);
    }

    // Update is called once per frame
    void Update()
    {
        // Keep the depth of field focus at the same point in world space
        m_depthOfField.focusDistance.value = m_defaultFocusDistance - transform.position.z + m_startCameraDistance;
        //Debug.Log("Current Camera distance: " + transform.position.z);
    }
}
