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
            if (events != null && events.Length == dialogueObject.Dialogue.Length) return;

            if (events == null)
            {
                events = new CutsceneSegment[dialogueObject.Dialogue.Length];
            }
            else
            {
                Array.Resize(ref events, dialogueObject.Dialogue.Length);
            }

            for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
            {
                if (events[i] != null)
                {
                    events[i].name = dialogueObject.Dialogue[i].Dialogue;
                }
                events[i] = new CutsceneSegment() {name = dialogueObject.Dialogue[i].Dialogue};
            } 
        }
    }

}
