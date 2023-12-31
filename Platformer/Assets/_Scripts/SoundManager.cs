using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
  public static SoundManager instance;
    [SerializeField] private AudioSource _musicSource, _effectsSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySound(AudioClip clip)
        {
         _effectsSource.PlayOneShot(clip);
        }

    public void PlayMusic(AudioClip music)
    {
        _musicSource.PlayOneShot(music);
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

}
