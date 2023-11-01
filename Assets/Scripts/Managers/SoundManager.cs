// EventSystemManager - Halen
// Updates the currently selected item for menus
// Last edit: 1/11/23

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
    // Static reference
    public static SoundManager Instance;

    [Header("Sound Sources")]
    public AudioSource musicSource;
    public AudioSource soundSource;

    [Space(10)]

    public AudioClip defaultClip;

    public string soundDirectory = "Sounds";

    // Singleton instantiation
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Use to play music through the SoundManager.
    /// </summary>
    /// <param name="clip"></param>
    public void PlayMusic(AudioClip clip)
    {
        musicSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Use to play sound effects through the SoundManager.
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }

    public void PlaySound(string filename)
    {
        AudioClip clip = (AudioClip) Resources.Load(soundDirectory + "/" + filename);
        soundSource.PlayOneShot(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // debug method
# if UNITY_EDITOR
        if (defaultClip && Keyboard.current.wKey.isPressed) PlayMusic(defaultClip);
# endif
    }
}
