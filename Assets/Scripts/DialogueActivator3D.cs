using DialogueSystem.Cutscenes;
using UnityEngine;


namespace DialogueSystem
{
    public class DialogueActivator3D : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueObject dialogueObject;
        [SerializeField] private string staticInteractText;
        private string interactText;

        void Start()
        {
            interactText = $"[E] {staticInteractText}";
        }

        public void SetInteractText(string text)
        {
            interactText = $"[E] {text}";
        }

        public void UpdateDialogueObject(DialogueObject dialogueObject)
        {
            this.dialogueObject = dialogueObject;
            UpdateDialogueEvents();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
            {
                player.Interactable = this;
                Player.Instance.ToggleInteractText(true, interactText);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
            {
                if (player.Interactable is DialogueActivator3D dialogueActivator && dialogueActivator == this)
                {
                    player.Interactable = null;
                    Player.Instance.ToggleInteractText(false);
                }
            }
        }

        private void UpdateDialogueEvents()
        {
            Player player = Player.Instance;
            if (dialogueObject == null)
            {
                Debug.Log($"Dialogue object is null! Game object: {gameObject.name}");
                return;
            }
            foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
            {
                if (responseEvents != null && responseEvents.DialogueObject == dialogueObject)
                {
                    player.DialogueUI.AddResponseEvents(responseEvents.Events);
                    break;
                }
            }
            player.DialogueUI.AddAllCutsceneEvents(GetComponents<DialogueCutsceneEvents>());
        }

        public void Interact(Player player)
        {
            UpdateDialogueEvents();

            Player.Instance.ToggleInteractText(false);
            player.DialogueUI.ShowDialogue(dialogueObject);
        }
    }

}
