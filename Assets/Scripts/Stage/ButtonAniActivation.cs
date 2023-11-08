//ButtonAniActivation - Jake Green
//Activates an animation when trigger box is entered
//8/11/2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAniActivation : MonoBehaviour
{
    public Animator[] animators;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (Animator animator in animators)
            animator.Play("Play");
    }
}
