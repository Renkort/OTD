using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Akkerman.Audio
{
    public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private AudioClip hoveringCursorSFX;
        [SerializeField] private float volume = 0.2f;

        public void OnPointerEnter(PointerEventData data)
        {
            SoundFXHandler.Instance.PlaySoundFXClip(hoveringCursorSFX, AudioHandler.Instance.gameObject.transform, volume);
        }

    }    
}
