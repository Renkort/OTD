using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour
{
    [HideInInspector] public MessageData data;
    public TextMeshProUGUI MessageLabel;

    public void InitUI(MessageData data)
    {
        this.data = data;
        MessageLabel.text = data.Message;
    }
}
