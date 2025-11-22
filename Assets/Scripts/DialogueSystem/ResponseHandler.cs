using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using Akkerman.Localization;
using Akkerman.Audio;

namespace Akkerman.DialogueSystem
{
    public class ResponseHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform responseBox;
        [SerializeField] private RectTransform responseButtonTemplate;
        [SerializeField] private RectTransform responseContainer;
        [SerializeField] private AudioClip selectResponseSFX;

        private List<GameObject> tempResponseButtons = new List<GameObject>();
        private ResponseEvent[] responseEvents;
        private List<UnityEvent> dialogueResponses = new List<UnityEvent>();
        private DialogueUI dialogueUI;
        private DialogueResponseEvents[] allEvents;
        private DialogueResponseEvents currentEvents;

        private void Start()
        {
            dialogueUI = GetComponent<DialogueUI>();
        }

        private void Update()
        {
            if(responseBox.gameObject.activeInHierarchy)
                HandleInput();
        }

        private void HandleInput()
        {
            int responseIndex = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            { responseIndex = 0; }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            { responseIndex = 1; }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            { responseIndex = 2; }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            { responseIndex = 3; }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            { responseIndex = 4; }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            { responseIndex = 5; }

            if (responseIndex >= 0 && dialogueResponses.Count > responseIndex)
            {
                dialogueResponses[responseIndex]?.Invoke();
            }
        }

        public void AddResponseEvents(ResponseEvent[] responseEvents)
        {
            this.responseEvents = responseEvents;
        }

        public void AddAllResponseEvents(DialogueResponseEvents[] allEvents)
        {
            if (allEvents.Length == 0)
                return;
            this.allEvents = allEvents;
        }

        public void SetCurrentEvents(DialogueObject dialogueObject)
        {
            if (allEvents == null || allEvents.Length == 0)
                return;
            for (int i = 0; i < allEvents.Length; i++)
            {
                if (allEvents[i].DialogueObject == dialogueObject)
                {
                    currentEvents = allEvents[i];
                    break;
                }
                else
                {
                    currentEvents = null;
                    responseEvents = null;
                }
            }
            if (currentEvents != null)
                AddResponseEvents(currentEvents.Events);
        }

        public void ShowResponses(List<Response> responses)
        {
            if (dialogueResponses != null) dialogueResponses.Clear();
            float responseBoxHeight = 0f;

            for (int i = 0; i < responses.Count; i++)
            {
                Response response = responses[i];
                int responseIndex = i;

                GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
                responseButton.SetActive(true);
                DialogueSentence responceLocalizedText = LocalizationLoader.Instance.GetDialogueSentence(response.ResponseText);
                responseButton.GetComponentInChildren<TextMeshProUGUI>().text = $"[{i + 1}] " + responceLocalizedText.Dialogue;
                responseButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));
                dialogueResponses.Add(new UnityEvent());
                dialogueResponses[i].AddListener(() => OnPickedResponse(response, responseIndex));

                tempResponseButtons.Add(responseButton);
                responseBoxHeight += responseButtonTemplate.sizeDelta.y;
            }
            responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
            responseBox.gameObject.SetActive(true);

            FPS.Player.Instance.SetCursorVisible(true);
            FPS.Player.Instance.FreezePlayerActions(true, true);
        }

        private void OnPickedResponse(Response response, int responseIndex)
        {
            responseBox.gameObject.SetActive(false);
            FPS.Player.Instance.SetCursorVisible(false);
            FPS.Player.Instance.FreezePlayerActions(true, false);
            SoundFXHandler.Instance.PlaySoundFXClip(selectResponseSFX, FPS.Player.Instance.gameObject.transform, 0.2f);

            foreach (GameObject responseButton in tempResponseButtons)
            {
                Destroy(responseButton);
            }
            tempResponseButtons.Clear();

            if (responseEvents != null && responseIndex <= responseEvents.Length)
            {
                responseEvents[responseIndex].OnPickedResponse?.Invoke();
            }

            responseEvents = null;

            if (response.DialogueObject)
            {
                dialogueUI.ShowDialogue(response.DialogueObject);
            }
            else
            {
                FPS.Player.Instance.FreezePlayerActions(false, false);
                dialogueUI.CloseDialogueBox();
            }

        }
    }

}
