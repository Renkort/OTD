using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Akkerman.InventorySystem;

namespace Akkerman.Notifications
{
    
    public class NotificationHandler : MonoBehaviour
    {
        [SerializeField] private GameObject notificationUI;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private MessageUI messagePrefab;
        private Queue<MessageUI> messages = new Queue<MessageUI>();
        public static NotificationHandler Instance;

        // SPECIAL MESSAGES
        [SerializeField] private MessageData recieveInventoryItem;
        [SerializeField] private MessageData lostInventoryItem;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        // void Start()
        // {
        //     InventoryUI.Instance.OnRecieveItem += AddInventoryItemMessage;
        //     InventoryUI.Instance.OnRemoveItem += RemoveInventoryItemMessage;
        // }
        public void ShowNotification(MessageData notification)
        {
            StartCoroutine(ShowMessageUI(notification));
        }
        private IEnumerator ShowMessageUI(MessageData notification)
        {
            notificationUI.SetActive(true);
            GameObject newMessage = Instantiate(messagePrefab.gameObject, messageContainer);
            newMessage.GetComponent<MessageUI>().InitUI(notification);
            yield return new WaitForSeconds(notification.Duration);
            HideMessageUI(newMessage);
        }
        private void HideMessageUI(GameObject message)
        {
            Destroy(message);
            notificationUI.SetActive(false);
        }

        public void AddInventoryItemMessage(string itemName)
        {
            MessageData newMessage = ScriptableObject.CreateInstance<MessageData>();
            newMessage.Message = recieveInventoryItem.Message;
            newMessage.Message += itemName;
            newMessage.Duration = recieveInventoryItem.Duration;
            ShowNotification(newMessage);
        }

        public void RemoveInventoryItemMessage(string itemName)
        {
            MessageData newMessage = ScriptableObject.CreateInstance<MessageData>();
            newMessage.Message = lostInventoryItem.Message;
            newMessage.Message += itemName;
            newMessage.Duration = lostInventoryItem.Duration;
            ShowNotification(newMessage);
        }
    }
}
