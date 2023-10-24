using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lineRenderer;
    /// <summary>
    /// get line renderer
    /// </summary>
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// do a ray cast out of the laser box and draw a line between its 2 ends and if it doesnt end just draw 100 in the local up direction
    /// </summary>
    void FixedUpdate()
    {
        //do a raycast
        RaycastHit hit;

        //if the ray doesnt hit any thing in 1000 units
        if (Physics.Raycast(transform.position, transform.up, out hit, 1000))
        {
            //draw line between here and end point of ray
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
            //if hit a player
            if(hit.collider.gameObject.tag == "Player")
            {
                //get player
                PlayerController hitPlayer = hit.collider.gameObject.GetComponent<PlayerController>();
                //do max damage
                hitPlayer.TakeDamage(hitPlayer.maxHealth);
            }
        } else //just draw line from here to 1000 up
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.up * 1000);
        }

    }
}