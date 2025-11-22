using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Akkerman.Localization;

namespace Akkerman.DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
    public class DialogueObject : ScriptableObject
    {
        [SerializeField] private List<string> dialogueKeys;
        [SerializeField] private List<DialogueSentence> dialogue;
        [SerializeField] private List<Response> responses;

        public bool HasResponses => responses != null && responses.Count > 0;

        public List<string> Keys => dialogueKeys;
        public List<DialogueSentence> Dialogue => dialogue;
        public List<Response> Responses => responses;

        public void LoadDialogueSentences()
        {
            dialogue = LocalizationLoader.Instance.GetDialogueSentences(dialogueKeys);
        }
    }

    [System.Serializable]
    public class DialogueSentence
    {
        public string Key;
        public string Name;
        [TextArea] public string Dialogue;
        public Sprite Portrait;
        public AudioClip audioClip;
    }
}
