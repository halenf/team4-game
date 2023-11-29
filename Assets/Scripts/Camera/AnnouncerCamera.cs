using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerCamera : MonoBehaviour
{
    public float distance;
    private Transform m_target;
    public void SetNewParent(Transform playerToFollow)
    {
        m_target = playerToFollow;
        
    }

    public void Update()
    {
        if (m_target)
        {
            transform.position = new Vector3(m_target.position.x, m_target.position.y, (m_target.position.z) - distance);
        }
    }
}
