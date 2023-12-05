// Set Colour - Halen
// Changes the colour of a preset list of Mesh Renderers
// Last edit: 16/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SetColour : MonoBehaviour
{
    [Tooltip("Array of the objects that will have their colour changed. The children of the objects will also be searched for renderers.")]
    public GameObject[] objects;
    [Header("Brightness settings")]
    [Tooltip("the brightness of the player at max health")]
    [Min(0)]public float maxEmissionMultiplier;
    [Tooltip("the brightness of the player at max health")]
    [Min(0)]public float minEmissionMultiplier;
    [Tooltip("the light around the player")]
    public Light centerLight; 

    private Color m_setColour;

    /// <summary>
    /// Sets the colours of all the Renderers in 'renderers' to a specified colour.
    /// </summary>
    /// <param name="colour"></param>
    public void Set(Color colour)
    {
        // make a new empty list of renderers
        List<Renderer> renderers = new List<Renderer>();

        foreach (GameObject obj in objects)
        {
            // if the initial object has a renderer, add it to the list
            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer) renderers.Add(objRenderer);

            // add any renderers attached to the object to the list
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>());
        }

        // change the colour of all the renderers' materials
        foreach (Renderer renderer in renderers)
        {
            renderer.material.EnableKeyword("_EMISSION");
            
            if (renderer.material.IsKeywordEnabled("_EMISSION"))
            {
                renderer.material.SetColor("_EmissionColor", (maxEmissionMultiplier * colour));
            }
            renderer.material.SetColor("_Color", colour);
        }

        m_setColour = colour;
        //centerLight.color = m_setColour;
    }

    public void Set(Color colour, float emmisionPercentage)
    {
        // make a new empty list of renderers
        List<Renderer> renderers = new List<Renderer>();

        foreach (GameObject obj in objects)
        {
            // if the initial object has a renderer, add it to the list
            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer) renderers.Add(objRenderer);

            // add any renderers attached to the object to the list
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>());
        }

        // change the colour of all the renderers' materials
        foreach (Renderer renderer in renderers)
        {
            renderer.material.EnableKeyword("_EMISSION");

            if (renderer.material.IsKeywordEnabled("_EMISSION"))
            {
                renderer.material.SetColor("_EmissionColor", colour * (((maxEmissionMultiplier - minEmissionMultiplier) * emmisionPercentage) + minEmissionMultiplier));
            }
            renderer.material.SetColor("_Color", colour);
        }

        m_setColour = colour;
    }

    public Color GetColour()
    {
        return m_setColour;
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


