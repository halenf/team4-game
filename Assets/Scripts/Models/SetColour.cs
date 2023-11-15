// Set Colour - Halen
// Changes the colour of a preset list of Mesh Renderers
// Last edit: 15/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColour : MonoBehaviour
{
    [Tooltip("Array of the Mesh Renderers that will have their colour changed.")]
    public MeshRenderer[] renderers;
    
    /// <summary>
    /// Sets the colours of all the MeshRenderers in 'renderers' to a specified colour.
    /// </summary>
    /// <param name="colour"></param>
    public void Set(Color colour)
    {
        foreach (MeshRenderer renderer in renderers)
        {
            // Changes the emission colour if the material has it enabled, otherwise just the base colour
            if (renderer.material.IsKeywordEnabled("_EMISSION")) renderer.material.SetColor("_EmissionColor", colour);
            else renderer.material.SetColor("_Color", colour);
        }
    }
}
