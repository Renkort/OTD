using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DialogueSystem
{
    public class ResponseHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform responseBox;
        [SerializeField] private RectTransform responseButtonTemplate;
        [SerializeField] private RectTransform responseContainer;

        private List<GameObject> tempResponseButtons = new List<GameObject>();
        private ResponseEvent[] responseEvents;
        private List<UnityEvent> dialogueResponses = new List<UnityEvent>();
        private DialogueUI dialogueUI;

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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            { dialogueResponses[0]?.Invoke(); }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            { dialogueResponses[1]?.Invoke(); }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            { dialogueResponses[2]?.Invoke(); }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            { dialogueResponses[3]?.Invoke(); }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            { dialogueResponses[4]?.Invoke(); }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            { dialogueResponses[5]?.Invoke(); }
        }

        public void AddResponseEvents(ResponseEvent[] responseEvents)
        {
            this.responseEvents = responseEvents;
        }

        public void ShowResponses(Response[] responses)
        {
            if (dialogueResponses != null) dialogueResponses.Clear();
            float responseBoxHeight = 0f;

            for (int i = 0; i < responses.Length; i++)
            {
                Response response = responses[i];
                int responseIndex = i;

                GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
                responseButton.SetActive(true);
                responseButton.GetComponent<TextMeshProUGUI>().text = $"[{i + 1}] " + response.ResponseText;
                responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));
                dialogueResponses.Add(new UnityEvent());
                dialogueResponses[i].AddListener(() => OnPickedResponse(response, responseIndex));

                tempResponseButtons.Add(responseButton);
                responseBoxHeight += responseButtonTemplate.sizeDelta.y;
            }
            responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
            responseBox.gameObject.SetActive(true);

            Player.Instance.SetCursorVisible(true);
        }

        private void OnPickedResponse(Response response, int responseIndex)
        {
            responseBox.gameObject.SetActive(false);

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
                dialogueUI.CloseDialogueBox();
            }

            Player.Instance.SetCursorVisible(false);
        }
    }

}
