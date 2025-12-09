using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Akkerman.DialogueSystem.Cutscenes;
using Akkerman.CutsceneSystem;
using System.Collections.Generic;

namespace Akkerman.DialogueSystem
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private Image portraitLabel;
        public bool IsOpen { get; private set; }
        private ResponseHandler responseHandler;
        private TypewriterEffect typewriterEffect;
        private CutsceneHandler cutsceneHandler;

        private void Start()
        {
            responseHandler = GetComponent<ResponseHandler>();
            typewriterEffect = GetComponent<TypewriterEffect>();
            cutsceneHandler = GetComponent<CutsceneHandler>();
            CloseDialogueBox();
        }

        public void ShowDialogue(DialogueObject dialogueObject)
        {
            FPS.Player.Instance.FreezePlayerActions(true, false);
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
        public void AddAllResponseEvents(DialogueResponseEvents[] allEvents)
        {
            responseHandler.AddAllResponseEvents(allEvents);
        }

        private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
        {
            dialogueObject.LoadDialogueSentences();
            cutsceneHandler.SetCurrentEvents(dialogueObject);
            responseHandler.SetCurrentEvents(dialogueObject);
            for (int i = 0; i < dialogueObject.Dialogue.Count; i++)
            {
                string dialogue = dialogueObject.Dialogue[i].Dialogue;
                nameLabel.text = dialogueObject.Dialogue[i].Name;
                portraitLabel.sprite = dialogueObject.Dialogue[i].Portrait;

                if (dialogueObject.Dialogue[i].Portrait == null)
                    portraitLabel.enabled = false;
                else
                    portraitLabel.enabled = true;

                yield return OpenCutsceneSegment(i);
                yield return RunTypingEffect(dialogue);

                textLabel.text = dialogue;

                if (i == dialogueObject.Dialogue.Count - 1 && dialogueObject.HasResponses)
                {
                    break;
                }

                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)
                || Input.GetMouseButtonDown(0));
                yield return CloseCutsceneSegment(i);
            }

            if (dialogueObject.HasResponses)
            {
                responseHandler.ShowResponses(dialogueObject.Responses);
            }
            else
            {
                cutsceneHandler.ClearCurrentEvents();
                FPS.Player.Instance.FreezePlayerActions(false, false);
                CloseDialogueBox();
            }

        }

        private IEnumerator OpenCutsceneSegment(int segmentIndex)
        {
            cutsceneHandler.OnOpenSegment(segmentIndex);
            while (cutsceneHandler.IsPlaying)
            {
                yield return null;
            }
        }
        private IEnumerator CloseCutsceneSegment(int segmentIndex)
        {
            cutsceneHandler.OnCloseSegment(segmentIndex);
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
            
            IsOpen = false;
            textLabel.text = string.Empty;
            dialogueBox.SetActive(false);
        }
    }
}
