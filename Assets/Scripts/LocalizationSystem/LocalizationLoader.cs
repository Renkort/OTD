using System;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;

public class LocalizationLoader : MonoBehaviour
{
    public static LocalizationLoader Instance;

    private Dictionary<string, DialogueSentence> currentLanguageData;
    private Dictionary<string, PortraitEntry> portraitData;

    [SerializeField] private string currentLanguage = "ru";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        LoadLanguage(currentLanguage);
        LoadPortraits();

    }

    public void LoadLanguage(string languageCode)
    {
        currentLanguage = languageCode;
        currentLanguageData = new Dictionary<string, DialogueSentence>();

        TextAsset languageFile = Resources.Load<TextAsset>($"Localization/dialogues_{languageCode}");
        if (languageFile == null)
        {
            Debug.LogError($"Language file not found: dialogues_{languageCode}");
            return;
        }
        string[] lines = languageFile.text.Split('\n');
        string[] headers = lines[0].Split(new char[]{';', '\n', '\r'});

        int keyIndex = Array.IndexOf(headers, "key");
        int nameIndex = Array.IndexOf(headers, "character_name");
        int textIndex = Array.IndexOf(headers, "text");

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;

            string[] fields = lines[i].Split(new char[]{';', '\n', '\r'});
            if (fields.Length < 3) continue;

            DialogueSentence sentence = new DialogueSentence
            {
                Key = fields[keyIndex],
                Name = fields[nameIndex],
                Dialogue = fields[textIndex]
            };

            currentLanguageData[sentence.Key] = sentence;
        }
        Debug.Log($"Loaded {currentLanguageData.Count} sentences for language: {languageCode}");
    }

    private void LoadPortraits()
    {
        portraitData = new Dictionary<string, PortraitEntry>();

        TextAsset portraitFile = Resources.Load<TextAsset>($"Localization/portraits");
        if (portraitFile == null)
        {
            Debug.LogError("Portraits file not found!");
            return;
        }

        string[] lines = portraitFile.text.Split('\n');
        string[] headers = lines[0].Split(new char[]{';', '\n', '\r'});

        int keyIndex = Array.IndexOf(headers, "key");
        int portraitIndex = Array.IndexOf(headers, "portrait");
        int audioIndex = Array.IndexOf(headers, "audio_clip");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;

            string[] fields = lines[i].Split(';');
            if (fields.Length < 2) continue;

            PortraitEntry entry = new PortraitEntry
            {
                Key = fields[keyIndex],
                PortraitName = fields[portraitIndex],
                audioClip = fields.Length > audioIndex ? fields[audioIndex] : ""
            };

            portraitData[entry.Key] = entry;
        }
    }

    public DialogueSentence GetDialogueSentence(string key)
    {
        if (currentLanguageData.ContainsKey(key))
        {
            DialogueSentence sentence = currentLanguageData[key];
            if (portraitData.ContainsKey(key))
            {
                PortraitEntry portrait = portraitData[key];
                if (!string.IsNullOrEmpty(portrait.PortraitName))
                {
                    Sprite portraitSprite = Resources.Load<Sprite>($"Portraits/{portrait.PortraitName}");
                    sentence.Portrait = portraitSprite;
                }
                if (!string.IsNullOrEmpty(portrait.audioClip))
                {
                    AudioClip clip = Resources.Load<AudioClip>($"Audio/{portrait.audioClip}");
                    sentence.audioClip = clip;
                }    
            }


            return sentence;
        }

        Debug.LogError($"Dialogue sentence not found for key: {key}");
        return null;
    }

    public List<DialogueSentence> GetDialogueSentences(List<string> keys)
    {
        List<DialogueSentence> sentences = new List<DialogueSentence>();
        foreach (string key in keys)
        {
            DialogueSentence sentence = GetDialogueSentence(key);
            if (sentence != null)
                sentences.Add(sentence);
        }
        return sentences;
    }
}

[System.Serializable]
public struct PortraitEntry
{
    public string Key;
    public string PortraitName;
    public string audioClip;
}
