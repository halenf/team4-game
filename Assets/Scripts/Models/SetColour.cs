// Set Colour - Halen
// Changes the colour of a preset list of Mesh Renderers
// Last edit: 16/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColour : MonoBehaviour
{
    [Tooltip("Array of the objects that will have their colour changed. The children of the objects will also be searched for renderers.")]
    public GameObject[] objects;
    public float emissiveMultiplyer;

    /// <summary>
    /// Sets the colours of all the MeshRenderers in 'renderers' to a specified colour.
    /// </summary>
    /// <param name="colour"></param>
    public void Set(Color colour)
    {
        // make a new empty list of renderers
        List<MeshRenderer> renderers = new List<MeshRenderer>();

        foreach (GameObject obj in objects)
        {
            // if the initial object has a renderer, add it to the list
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            if (objRenderer) renderers.Add(objRenderer);

            // add any renderers attached to the object to the list
            renderers.AddRange(obj.GetComponentsInChildren<MeshRenderer>());
        }

        // change the colour of all the renderers' materials
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.material.IsKeywordEnabled("_EMISSION")) renderer.material.SetColor("_EmissionColor", colour * emissiveMultiplyer);
            renderer.material.SetColor("_Color", colour);
        }
    }
}


