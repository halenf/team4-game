using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerCamera : MonoBehaviour
{
    public float distance;
    public void SetNewParent(Transform playerToFollow)
    {
        transform.parent = playerToFollow.transform;
        transform.localPosition = new Vector3(0, 0, -distance);
    }
}
