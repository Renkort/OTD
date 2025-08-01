using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DialogueSystem.Cutscenes;
using System.Collections.Generic;

namespace DialogueSystem
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private Image portraitLabel;
        public bool IsOpen { get; private set; }
        public Dictionary<DialogueObject, string> dialogueIds = new();
        public Dictionary<string, DialogueObject> dialogueObjects = new();
        private ResponseHandler responseHandler;
        private TypewriterEffect typewriterEffect;
        private CutsceneHandler cutsceneHandler;

        private void Start()
        {
            responseHandler = GetComponent<ResponseHandler>();
            typewriterEffect = GetComponent<TypewriterEffect>();
            cutsceneHandler = GetComponent<CutsceneHandler>();
            //CloseDialogueBox();
            dialogueBox.SetActive(false);
        }

        public void ShowDialogue(DialogueObject dialogueObject)
        {
            Player.Instance.FreezePlayerActions(true, false);
            IsOpen = true;
            dialogueBox.SetActive(true);
            StartCoroutine(StepThroughDialogue(dialogueObject));
        }

        public void AddResponseEvents(ResponseEvent[] responseEvents)
        {
            responseHandler.AddResponseEvents(responseEvents);
        }
        public void AddAllCutsceneEvents(DialogueCutsceneEvents[] allEvents)
        {
            cutsceneHandler.AddAllCutsceneEvents(allEvents);
        }

        private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
        {
            cutsceneHandler.SetCurrentEvents(dialogueObject);
            for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
            {
                string dialogue = dialogueObject.Dialogue[i].Dialogue;
                nameLabel.text = dialogueObject.Dialogue[i].Name;
                portraitLabel.sprite = dialogueObject.Dialogue[i].Portrait;

                if (dialogueObject.Dialogue[i].Portrait == null)
                    portraitLabel.enabled = false;
                else
                    portraitLabel.enabled = true;

                yield return RunCutsceneSegment(i);
                yield return RunTypingEffect(dialogue);

                textLabel.text = dialogue;

                if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)
                {
                    break;
                }

                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E));
            }

            if (dialogueObject.HasResponses)
            {
                responseHandler.ShowResponses(dialogueObject.Responses);
            }
            else
            {
                cutsceneHandler.ClearCurrentEvents();
                CloseDialogueBox();
            }

        }

        private IEnumerator RunCutsceneSegment(int segmentIndex)
        {
            cutsceneHandler.OnOpenSegment(segmentIndex);

            while (cutsceneHandler.IsPlaying)
            {
                yield return null;
            }
        }
        private IEnumerator RunTypingEffect(string dialogue)
        {
            typewriterEffect.Run(dialogue, textLabel);

            while (typewriterEffect.IsRunning)
            {
                yield return null;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    typewriterEffect.Stop();
                }
            }
        }

        public void CloseDialogueBox()
        {
            portraitLabel.enabled = true;
            Player.Instance.FreezePlayerActions(false, false);
            IsOpen = false;
            textLabel.text = string.Empty;
            dialogueBox.SetActive(false);
        }
    }
}
