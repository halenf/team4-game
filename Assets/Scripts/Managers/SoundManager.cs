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

    [Header("Resource Directories")]
    public string soundDirectory = "Sound/SoundEffects";
    public string musicDirectory = "Sound/Music";

    [Space(10)]

    public AudioClip defaultClip;

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

    public void PlayMusic(string filename)
    {
        AudioClip clip = (AudioClip) Resources.Load(musicDirectory + "/" + filename);
        soundSource.PlayOneShot(clip);
    }

    public void PlaySound(string filename)
    {
        AudioClip clip = (AudioClip) Resources.Load(soundDirectory + "/" + filename);
        soundSource.PlayOneShot(clip);
    }

    public void PlayFromSource(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void PlayFromSource(AudioSource source, string filename)
    {
        AudioClip clip = (AudioClip)Resources.Load(soundDirectory + "/" + filename);
        source.PlayOneShot(clip);
    }

    public void PlayAudioAtPoint(Vector3 position, AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }

    public void PlayAudioAtPoint(Transform position, AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, position.position);
    }

    public void PlayAudioAtPoint(Vector3 position, string filename)
    {
        AudioClip clip = (AudioClip)Resources.Load(soundDirectory + "/" + filename);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    public void PlayAudioAtPoint(Transform position, string filename)
    {
        AudioClip clip = (AudioClip)Resources.Load(soundDirectory + "/" + filename);
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position.position);
        }
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
