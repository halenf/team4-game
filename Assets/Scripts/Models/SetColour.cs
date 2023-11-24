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
    public float maxEmissionMultiplyer;
    public float minEmissionMultiplyer;

    private Color m_setColour;

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
            renderer.material.EnableKeyword("_EMISSION");
            
            if (renderer.material.IsKeywordEnabled("_EMISSION"))
            {
                renderer.material.SetColor("_EmissionColor", (maxEmissionMultiplyer * colour));
            }
            renderer.material.SetColor("_Color", colour);
        }

        m_setColour = colour;
    }

    public void Set(Color colour, float emmisionPercentage)
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
            renderer.material.EnableKeyword("_EMISSION");

            if (renderer.material.IsKeywordEnabled("_EMISSION"))
            {
                renderer.material.SetColor("_EmissionColor", colour * (((maxEmissionMultiplyer - minEmissionMultiplyer) * emmisionPercentage) + minEmissionMultiplyer));
            }
            renderer.material.SetColor("_Color", colour);
        }

        m_setColour = colour;
    }

    /// <summary>
    /// Changes the emission intensity of the objects and all their children to a specified value.
    /// </summary>
    /// <param name="value"></param>
    public void SetEmissionIntensity(float value)
    {
        //Set(m_setColour, value);
    }
}


