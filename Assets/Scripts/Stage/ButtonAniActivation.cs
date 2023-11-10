//ButtonAniActivation - Jake Green
//Activates an animation when trigger box is entered
//8/11/2023
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAniActivation : MonoBehaviour
{
    public Animator[] animators;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (Animator animator in animators)
                animator.Play("Play");
        }
    }
}
