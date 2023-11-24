// Laser - Cameron
// Laser behaviour
// Last edit: 24/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Obstacle
{
    public bool isActive;
    public string[] killStrings;
    public ParticleSystem hitParticle;
    private ParticleSystem m_particleInScene;


    private LineRenderer lineRenderer;
    /// <summary>
    /// get line renderer
    /// </summary>
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //create the particles in the scene
        m_particleInScene = Instantiate(hitParticle);
    }

    /// <summary>
    /// do a ray cast out of the laser box and draw a line between its 2 ends and if it doesnt end just draw 100 in the local up direction
    /// </summary>
    void Update()
    {
        if (isActive)
        {
            //do a raycast
            RaycastHit hit;

            //if the ray doesnt hit any thing in 1000 units
            if (Physics.Raycast(transform.position, transform.up, out hit, 1000))
            {
                //draw line between here and end point of ray
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, hit.point);
                //move particles
                m_particleInScene.transform.position = hit.point;
                m_particleInScene.transform.LookAt(transform.position);

                //if hit a player
                if (hit.collider.gameObject.tag == "Player")
                {
                    //get player
                    PlayerController hitPlayer = hit.collider.gameObject.GetComponent<PlayerController>();
                    //do max damage
                    hitPlayer.TakeDamage(hitPlayer.maxHealth, killStrings);
                }
            }
            else //just draw line from here to 1000 up
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.up * 1000);
                m_particleInScene.transform.position = new Vector3(99999999f, 99999999f, 0);
            }
        }
    }

    // inherited methods - Halen
    public override void ToggleState()
    {
        isActive = !isActive;
    }

    public override void ToggleState(bool state)
    {
        isActive = state;
    }
}
