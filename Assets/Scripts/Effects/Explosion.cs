// explosion - Cameron
// destroys self after life time
// Last edit: 2/11/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Tooltip("Amount of time the explosion hitbox is active for.")]
    [Min(0)] public float lifeTime;
    [Tooltip("Radius of the explosion.")]
    [Min(0)] public float radius;

    [Header("Particle Systems")]
    public ParticleSystem fragments;
    public ParticleSystem blast;

    // Start is called before the first frame update
    void Start()
    {
        // Create the explosion collider
        SphereCollider collider = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        collider.isTrigger = true;
        collider.radius = radius;
        collider.center = Vector3.zero;

        // set details of the particle systems
        var fragmentsModule = fragments.GetComponent<ParticleSystem.MainModule>();
        fragmentsModule.startSpeed = radius * 3.75f;

        var blastModule = blast.GetComponent<ParticleSystem.MainModule>();
        blastModule.startSize = radius;

        StartCoroutine(Explode());
        Instantiate(fragments, transform.position, Quaternion.identity);
        Instantiate(blast, transform.position, Quaternion.identity);
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
