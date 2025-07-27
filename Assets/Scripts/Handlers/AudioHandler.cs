using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public AudioClip musicClip => musicSource.clip;
    public AudioClip sfxClip => sfxSource.clip;

    void Awake()
    {
        if (Instance == null || Instance != this)
            Instance = this;
        if (musicSource == null || sfxSource == null)
            Debug.Log($"ERROR: Music or sfx Audio Source is null");
    }

    public void SetSfx(AudioClip sfx)
    {
        sfxSource.Stop();
        sfxSource.clip = sfx;
        sfxSource.Play();
    }

    public void SetMusic(AudioClip track)
    {
        musicSource.Stop();
        musicSource.clip = track;
        musicSource.Play();
    }
}
