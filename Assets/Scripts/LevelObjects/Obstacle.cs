// Obstacle - Halen
// Abstract class for level obstacles.
// Last edit: 30/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    public virtual void Start()
    {
        // Start the toggle timer for any object with it enabled
        StartCoroutine(ToggleTimer());
        // Get the animator component for any object that has one
        if (!(m_animator = GetComponent<Animator>())) m_animator = GetComponentInChildren<Animator>();
    }

    // active state
    [SerializeField] protected bool m_isActive;
    public virtual bool isActive
    { 
        get {  return m_isActive; }
        set { m_isActive = value; }
    }

    public virtual void ToggleState()
    {
        isActive = !isActive;
    }

    public virtual void ToggleState(bool state)
    {
        isActive = state;
    }

    // timed toggle
    [Tooltip("The object will toggle state every X seconds. Set 0 to disable this feature.")]
    [Min(0)] public float toggleTimer;

    protected IEnumerator ToggleTimer()
    {
        while(toggleTimer > 0)
        {
            yield return new WaitForSeconds(toggleTimer);
            isActive = !isActive;
        }
    }

    // animation
    protected Animator m_animator;

    // general
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
