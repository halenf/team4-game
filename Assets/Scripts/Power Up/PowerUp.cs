//PowerUp - Cameron
// just has on collision
// // Last edit: 25/10/23
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{

    public abstract void OnCollisionEnter(Collision other);
}
