using System.Collections;
using UnityEngine;

namespace Akkerman.Audio
{
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
            if (Instance != null)
            {
                Debug.Log("Found more than one AudioHandler. Destroying newest one.");
                Destroy(this);
                return;
            }
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

        public void StopAllSound()
        {
            currentMusicSource.Stop();
            sfxSource.Stop();
        }

        public void CrossFadeMusicTo(AudioClip audioClip)
        {
            if (isCrossFading) return;
            nextMusicSource.clip = audioClip;
            Debug.Log("CROSSFADING MUSIC");
            StartCoroutine(CrossFade());
        }

        IEnumerator CrossFade()
        {
            isCrossFading = true;

            float timer = 0;
            float startVolumeCurrent = currentMusicSource.volume;
            float startVolumeNext = nextMusicSource.volume;
            float inverseDuration = 1f / crossFadeDuration;

            nextMusicSource.volume = 0f;
            nextMusicSource.Play();

            AudioSource current = currentMusicSource;
            AudioSource next = nextMusicSource;

            float lastUpdateTime = 0f;
            const float updateInterval = 0.1f;
            int updateCount = 0;

            while (timer < crossFadeDuration)
            {
                timer += Time.deltaTime;

                if (Time.time - lastUpdateTime >= updateInterval)
                {
                    float progress = timer * inverseDuration;
                    current.volume = Mathf.Lerp(startVolumeCurrent, 0, progress);
                    next.volume = Mathf.Lerp(0, startVolumeNext, progress);
                    lastUpdateTime = Time.time;
                    updateCount++;
                }

                yield return null;
            }
            current.volume = 0f;
            next.volume = startVolumeNext;

            current.Stop();
            current.volume = startVolumeCurrent;

            AudioSource temp = currentMusicSource;
            currentMusicSource = nextMusicSource;
            nextMusicSource = temp;

            isCrossFading = false;
        }
    }
}
