using UnityEngine;
using System;
using Akkerman.DialogueSystem;

namespace Akkerman.DialogueSystem
{
    
    public class DialogueResponseEvents : MonoBehaviour
    {
        [SerializeField] private DialogueObject dialogueObject;
        [SerializeField] private ResponseEvent[] events;

        public DialogueObject DialogueObject => dialogueObject;
        public ResponseEvent[] Events => events;

        public void OnValidate()
        {
            if (dialogueObject == null) return;
            if (dialogueObject.Responses == null) return;
            if (events != null && events.Length == dialogueObject.Responses.Count)
            {
                for (int i = 0; i < dialogueObject.Responses.Count; i++)
                {
                    events[i].name = dialogueObject.Responses[i].ResponseText;
                }
                return;
            }

            if (events == null)
            {
                events = new ResponseEvent[dialogueObject.Responses.Count];
            }
            else
            {
                Array.Resize(ref events, dialogueObject.Responses.Count);
            }

            for (int i = 0; i < dialogueObject.Responses.Count; i++)
            {
                Response response = dialogueObject.Responses[i];

                if (events[i] != null)
                {
                    events[i].name = response.ResponseText;
                    continue;
                }
                events[i] = new ResponseEvent() { name = response.ResponseText };
            }
        }
    }

    [System.Serializable]
    public struct ResponseEventContainer
    {
        public DialogueObject Dialogue;
        public ResponseEvent[] ResponseEvents;
    }
}
