// explosion - Cameron
// destroys self after life time
// Last edit: 26/10/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}