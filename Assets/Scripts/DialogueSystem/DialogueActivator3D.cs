using Akkerman.DialogueSystem.Cutscenes;
using UnityEngine;
using Akkerman.InteractionSystem;
using Akkerman.SaveSystem;

namespace Akkerman.DialogueSystem
{
    public class DialogueActivator3D : InteractableObject, IDataPersistance
    {
        [SerializeField] private DialogueObject dialogueObject;
        [SerializeField] private string staticInteractText;

        void Start()
        {
            InteractText = staticInteractText;
            InteractAction.AddListener(ActivateDialogue);
        }

        public void UpdateDialogueObject(DialogueObject dialogueObject)
        {
            this.dialogueObject = dialogueObject;
            UpdateDialogueEvents();
        }

        public void UpdateInteractText(string newText)
        {
            InteractText = newText;
        }

        private void UpdateDialogueEvents()
        {
            FPS.Player player = FPS.Player.Instance;
            if (dialogueObject == null)
            {
                Debug.Log($"Dialogue object is null! Game object: {gameObject.name}");
                return;
            }
            // foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
            // {
            //     if (responseEvents != null && responseEvents.DialogueObject == dialogueObject)
            //     {
            //         player.DialogueUI.AddResponseEvents(responseEvents.Events);
            //         break;
            //     }
            // }
            player.DialogueUI.AddAllResponseEvents(GetComponents<DialogueResponseEvents>());
            player.DialogueUI.AddAllCutsceneEvents(GetComponents<DialogueCutsceneEvents>());
        }

        public void ActivateDialogue()
        {
            UpdateDialogueEvents();

            FPS.Player.Instance.ToggleInteractText(false);
            FPS.Player.Instance.DialogueUI.ShowDialogue(dialogueObject);
        }

        public void LoadData(GameData data)
        {
            // Dictionary<string, DialogueObject> dialogueObj = Player.Instance.DialogueUI.dialogueObjects;
            // if (dialogueObj == null)
            //     Debug.Log($"DialougeObjects in DialougeUI is null");
            // string dialogueId = "";
            // data.Dialogues.TryGetValue(gameObject.name, out dialogueId);
            // if (dialogueObj[dialogueId] != null)
            //     dialogueObject = dialogueObj[dialogueId];
        }

        public void SaveData(ref GameData data)
        {
            // Dictionary<DialogueObject, string> dialogueIds = Player.Instance.DialogueUI.dialogueIds;
            // //WARNING: GameObject name is using as uniq id for person name;
            // if (data.Dialogues.ContainsKey(gameObject.name))
            // {
            //     data.Dialogues[gameObject.name] = dialogueIds[dialogueObject];
            // }
            // else
            // {
            //     if (!dialogueIds.ContainsKey(dialogueObject))
            //     {
            //         string dialogueId = System.Guid.NewGuid().ToString();
            //         dialogueIds.Add(dialogueObject, dialogueId);
            //         Player.Instance.DialogueUI.dialogueObjects.Add(dialogueId, dialogueObject);
            //         Debug.Log($"Adding {dialogueId}");
            //     }
            //     data.Dialogues.Add(gameObject.name, dialogueIds[dialogueObject]);
            // }
        }
    }

}
