using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.CutsceneSystem
{
    [System.Serializable]
    public class CutsceneSegment
    {
        [HideInInspector] public string name;
        [field: SerializeField] public float SegmentTime { get; private set; }
        [field: SerializeField] public float TimeBeforeNextSegment { get; private set; }
        [SerializeField] private UnityEvent onOpenSegment;
        [SerializeField] private UnityEvent onCloseSegment;

        public UnityEvent OnOpenSegment => onOpenSegment;
        public UnityEvent OnCloseSegment => onCloseSegment;
    }
}
