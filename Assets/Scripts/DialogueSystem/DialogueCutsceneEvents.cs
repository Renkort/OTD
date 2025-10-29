using System;
using UnityEngine;

namespace DialogueSystem.Cutscenes
{
    public class DialogueCutsceneEvents : MonoBehaviour
    {
        [SerializeField] private DialogueObject dialogueObject;
        [SerializeField] private CutsceneSegment[] events;

        public DialogueObject DialogueObject => dialogueObject;
        public CutsceneSegment[] Events => events;

        public void OnValidate()
        {
            if (dialogueObject == null) return;
            if (events != null && events.Length == dialogueObject.Keys.Count) return;

            if (events == null)
            {
                events = new CutsceneSegment[dialogueObject.Keys.Count];
            }
            else
            {
                Array.Resize(ref events, dialogueObject.Keys.Count);
            }

            for (int i = 0; i < dialogueObject.Keys.Count; i++)
            {
                if (events[i] != null)
                {
                    events[i].name = dialogueObject.Keys[i];
                }
                events[i] = new CutsceneSegment() {name = dialogueObject.Keys[i]};
            } 
        }
    }

}
