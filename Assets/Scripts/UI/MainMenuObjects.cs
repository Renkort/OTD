using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuObjects : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI doorRandomText;
    [SerializeField] private List<string> randomTextes = new List<string>();

    void Awake()
    {
        doorRandomText.text = randomTextes[Random.Range(0, randomTextes.Count)];
    }
    
}
