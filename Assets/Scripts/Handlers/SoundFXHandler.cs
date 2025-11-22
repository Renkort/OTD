using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.Audio
{
    
    public class SoundFXHandler : MonoBehaviour
    {
        public static SoundFXHandler Instance;
        [SerializeField] private AudioSource soundFXObject;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void PlaySoundFXClip(AudioClip audioClip)
        {
            PlaySoundFXClip(audioClip, FPS.Player.Instance.gameObject.transform, 1f);
        }

        public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
        {
            AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }

        public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
        {
            int rand = Random.Range(0, audioClips.Length);
            PlaySoundFXClip(audioClips[rand], spawnTransform, volume);
        }
    }
}
