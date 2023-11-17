// Obstacle - Halen
// Abstract class for level obstacles.
// Last edit: 17/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    public abstract void ToggleState();

    public abstract void ToggleState(bool state);
}
