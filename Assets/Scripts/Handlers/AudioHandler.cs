using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance { get; private set; }

    [SerializeField] private AudioSource currentMusicSource;
    [SerializeField] private AudioSource nextMusicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private float crossFadeDuration = 3f;
    private bool isCrossFading = false;

    public AudioClip musicClip => currentMusicSource.clip;
    public AudioClip sfxClip => sfxSource.clip;

    void Awake()
    {
        if (Instance == null || Instance != this)
            Instance = this;
        if (currentMusicSource == null || sfxSource == null)
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
        currentMusicSource.Stop();
        currentMusicSource.clip = track;
        currentMusicSource.Play();
    }

    public void CrossFadeMusicTo(AudioClip audioClip)
    {
        if (isCrossFading) return;
        nextMusicSource.clip = audioClip;
        StartCoroutine(CrossFade());
    }

    IEnumerator CrossFade()
    {
        isCrossFading = true;

        float timer = 0;
        float startVolumeCurrent = currentMusicSource.volume;
        float startVolumeNext = nextMusicSource.volume;

        nextMusicSource.volume = 0f;
        nextMusicSource.Play();

        while (timer < crossFadeDuration)
        {
            timer += Time.deltaTime;

            currentMusicSource.volume = Mathf.Lerp(startVolumeCurrent, 0, timer / crossFadeDuration);
            nextMusicSource.volume = Mathf.Lerp(0, startVolumeNext, timer / crossFadeDuration);

            yield return null;
        }

        currentMusicSource.Stop();
        currentMusicSource.volume = startVolumeCurrent;

        AudioSource temp = currentMusicSource;
        currentMusicSource = nextMusicSource;
        nextMusicSource = temp;

        isCrossFading = false;
    }
}
