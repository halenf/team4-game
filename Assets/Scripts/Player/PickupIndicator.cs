// Pickup Indicator - Halen
// World canvas that displays what powerup or gun the player gets from pickups.
// Last edit: 29/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupIndicator : MonoBehaviour
{
    [Header("Properties")]
    [Min(0)] public float riseSpeed;
    [Min(0)] public float spinSpeed;
    [Min(0)] public float dimSpeed;
    
    [Header("UI Object References")]
    [SerializeField] private Image m_icon;
    [SerializeField] private Light m_light;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // raise indicator up
        transform.position += new Vector3(0, riseSpeed * Time.deltaTime);

        // make indicator spin
        transform.Rotate(0, spinSpeed, 0, Space.Self);

        // lower the light brightness
        m_light.intensity -= dimSpeed;
    }

    public void SetDisplayDetails(float lifetime, Sprite sprite, Color lightColour)
    {
        Destroy(gameObject, lifetime);
        m_icon.sprite = sprite;
        m_light.color = lightColour;
    }
}
