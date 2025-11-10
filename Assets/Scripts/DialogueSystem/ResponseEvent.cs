using UnityEngine;
using UnityEngine.Events;

namespace Akkerman.DialogueSystem
{
    [System.Serializable]
    public class ResponseEvent
    {
        [HideInInspector] public string name;
        [SerializeField] private UnityEvent onPickedResponse;

        public UnityEvent OnPickedResponse => onPickedResponse;
    }    
}
