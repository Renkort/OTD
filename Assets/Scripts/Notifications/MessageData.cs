using UnityEngine;


namespace Akkerman.Notifications
{
    [CreateAssetMenu(fileName = "New Notification", menuName = "Message System/BaseNotification")]
    public class MessageData : ScriptableObject
    {
        [TextArea] public string Message;
        public float Duration;

    }
}
