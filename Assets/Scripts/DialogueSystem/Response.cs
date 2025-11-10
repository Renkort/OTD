using UnityEngine;


namespace Akkerman.DialogueSystem
{
    [System.Serializable]
    public class Response
    {
        [SerializeField] private string responseText;
        [SerializeField] private DialogueObject dialogueObject;

        public string ResponseText => responseText;
        public DialogueObject DialogueObject => dialogueObject;

        public Response(string responseText, DialogueObject nextDialogue)
        {
            this.responseText = responseText;
            this.dialogueObject = nextDialogue;
        }
    }
}
