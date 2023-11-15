using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEndLaser : MonoBehaviour
{
    [Tooltip("after this time in seconds the laser will stop moving")]
    public float timer;
    public float speed;
    private bool shouldMove = true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StopMoving());
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            transform.position = transform.position + (transform.right * (speed * Time.deltaTime));
        }
    }

    public IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(timer);
        shouldMove = false;
    }
}
