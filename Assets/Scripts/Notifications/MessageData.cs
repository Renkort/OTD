using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Notification", menuName = "Message System/BaseNotification")]
public class MessageData : ScriptableObject
{
    [TextArea] public string Message;
    public float Duration;

}
