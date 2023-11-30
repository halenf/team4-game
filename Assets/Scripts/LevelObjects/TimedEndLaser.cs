using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimedEndLaser : MonoBehaviour
{
    [Tooltip("after this time in seconds the laser will stop moving")]
    private float m_timer;
    private float m_speed;
    private bool shouldMove = true;
    // Start is called before the first frame update
    void Start()
    {
        if (m_timer != 0)
        {
            StartCoroutine(StopMoving());
        }
    }

    public void Init(float speed, float timer)
    {
        m_speed = speed;
        m_timer = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            transform.position = transform.position + (transform.right * (m_speed * Time.deltaTime));
        }
    }

    public IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(m_timer);
        shouldMove = false;
    }
}
