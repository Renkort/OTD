using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.CutsceneSystem
{    
    public class CutsceneActivator : MonoBehaviour
    {
        [SerializeField] private bool playOnAwake = false;
        [SerializeField] private float startDelay = 0f;
        [SerializeField] private CutsceneSegment[] segments;

        void Start()
        {
            if (playOnAwake)
                StartCoroutine(Activate());
        }

        private IEnumerator Activate()
        {
            yield return new WaitForSeconds(startDelay);
            ActivateCutscene();
        }

        public void ActivateCutscene()
        {
            if (segments.Length == 0)
                return;
            FPS.Player.Instance.CutsceneHandler.AddCutsceneEvents(segments);
            FPS.Player.Instance.CutsceneHandler.StartCutscene();
        }
    }
}
