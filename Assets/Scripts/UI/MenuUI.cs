using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuUI : MonoBehaviour
{
    [Header("Menu UI")]
    public GameObject defaultSelectedObject;

    // indicator options
    [SerializeField] protected Image m_selectedIndicator;
    [Tooltip("The horizontal and vertical padding on both sides of the indicator.")]
    [SerializeField] protected Vector2 padding;

    protected virtual void OnEnable()
    {
        EventSystemManager.Instance.inMenu = true;
        EventSystemManager.Instance.SetButtonIndicator(m_selectedIndicator, padding);
    }

    protected virtual void OnDisable()
    {
        EventSystemManager.Instance.inMenu = false;
    }
}
