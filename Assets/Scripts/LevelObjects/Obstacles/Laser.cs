// Laser - Cameron
// Laser behaviour
// Last edit: 30/11/23

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : Obstacle
{
    public ParticleSystem hitParticle;
    private ParticleSystem m_particleInScene;
    private LineRenderer m_lineRenderer;
    public LayerMask mask;
    public float killWidth;
    
    public override void Start()
    {
        base.Start();
        
        // get the line renderer component
        m_lineRenderer = GetComponent<LineRenderer>();

        // create the hit particle in the scene
        m_particleInScene = Instantiate(hitParticle, transform);
    }

    /// <summary>
    /// do a ray cast out of the laser box and draw a line between its 2 ends and if it doesnt end just draw 100 in the local up direction
    /// </summary>
    void Update()
    {
        if (isActive)
        {
            //do a raycast
            RaycastHit lineHit;

            // do one ray cast to make the laser and kill players in it
            //if the ray doesnt hit any thing in 1000 units
            if (Physics.Raycast(transform.position, transform.up, out lineHit, 1000, mask, QueryTriggerInteraction.Ignore))
            {
                //draw line between here and end point of ray
                m_lineRenderer.SetPosition(0, transform.position);
                m_lineRenderer.SetPosition(1, lineHit.point);

                // activate the particle system
                if (!m_particleInScene.isPlaying) m_particleInScene.Play();

                //move particles
                m_particleInScene.gameObject.SetActive(true);
                m_particleInScene.transform.position = lineHit.point;
                m_particleInScene.transform.LookAt(transform.position);
            }
            else //just draw line from here to 1000 up
            {
                m_lineRenderer.SetPosition(0, transform.position);
                m_lineRenderer.SetPosition(1, transform.up * 1000);
                m_particleInScene.Stop();
            }

            RaycastHit boxHit;

            //do box cast so players cant go past the laser
            if (Physics.BoxCast(transform.position, new Vector3(killWidth / 2f, .5f, .5f), transform.up, out boxHit, Quaternion.identity, 1000))
            {
                //if hit a player
                if (boxHit.collider.gameObject.tag == "Player")
                {
                    //get player
                    PlayerController hitPlayer = boxHit.collider.gameObject.GetComponent<PlayerController>();
                    //do max damage
                    hitPlayer.TakeDamage(hitPlayer.maxHealth, AnnouncerSubtitleDisplay.AnnouncementType.DeathFire);
                }
                if (boxHit.collider.gameObject.tag == "Breakable")
                {
                    //get player
                    BreakableObject hitObject = boxHit.collider.gameObject.GetComponent<BreakableObject>();
                    //do max damage
                    hitObject.TakeDamage(hitObject.maxHealth);
                }
            }
        }
        else
        {
            m_lineRenderer.SetPositions(new Vector3[] { transform.position, transform.position } );
            m_particleInScene.Stop();
        }
    }

    private void OnDisable()
    {
        Destroy(m_particleInScene);
    }
}
