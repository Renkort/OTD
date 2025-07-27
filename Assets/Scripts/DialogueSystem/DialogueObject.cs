using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
    public class DialogueObject : ScriptableObject
    {
        [SerializeField] private DialogueSentence[] dialogue;
        [SerializeField] private Response[] responses;

        public bool HasResponses => responses != null && responses.Length > 0;

        public DialogueSentence[] Dialogue => dialogue;
        public Response[] Responses => responses;
    }

    [System.Serializable]
    public struct DialogueSentence
    {
        public string Name;
        [TextArea] public string Dialogue;
        public Sprite Portrait;
    }
}
