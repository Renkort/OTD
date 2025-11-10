using UnityEngine;
using Akkerman.DialogueSystem.Cutscenes;
using Akkerman.InteractionSystem;

namespace Akkerman.DialogueSystem
{
    public class DialogueActivator2D : MonoBehaviour, IInteractable
    {
        [Header("A collider is required")]
        [SerializeField] private DialogueObject dialogueObject;

        public void UpdateDialogueObject(DialogueObject dialogueObject)
        {
            this.dialogueObject = dialogueObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out FPS.Player player))
            {
                player.Interactable = this;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.TryGetComponent(out FPS.Player player))
            {
                if (player.Interactable is DialogueActivator2D dialogueActivator && dialogueActivator == this)
                {
                    player.Interactable = null;
                }
            }
        }
        public void Interact(FPS.Player player)
        {
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
            player.DialogueUI.ShowDialogue(dialogueObject);
        }
    }
}
