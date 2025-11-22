using System;
using System.Collections;
using System.Collections.Generic;
using Akkerman.DialogueSystem;
using UnityEngine;


namespace Akkerman.Localization
{
    
    public class LocalizationLoader : MonoBehaviour
    {
        public static LocalizationLoader Instance;

        private Dictionary<string, DialogueSentence> currentLanguageData;
        private Dictionary<string, PortraitEntry> portraitData;
        private Dictionary<string, string> languageData;

        [SerializeField] private string currentLanguage = "ru";
        [SerializeField] private Language language;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            LoadLocalizedDialogues(currentLanguage);
            LoadLanguage(currentLanguage);
            LoadPortraits();

        }

        public void LoadLanguage(string languageCode)
        {
            currentLanguage = languageCode;
            languageData = new Dictionary<string, string>();

            TextAsset languageFileUI = Resources.Load<TextAsset>($"Localization/{languageCode}/ui_{languageCode}");
            if (languageFileUI == null)
            {
                Debug.LogError($"Language file not found: ui_{languageCode}");
                return;
            }
            string[] lines = languageFileUI.text.Split('\n');
            string[] headers = lines[0].Split(new char[] { ';', '\n', '\r' });
            int keyIndex = Array.IndexOf(headers, "key");
            int valueIndex = Array.IndexOf(headers, "value");

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;

                string[] fields = lines[i].Split(new char[] { ';', '\n', '\r' });
                if (fields.Length < 2) continue;

                languageData[fields[keyIndex]] = fields[valueIndex];
            }
            Debug.Log($"Loaded {languageData.Count} UI lines for language: {languageCode}");

        }

        public void LoadLocalizedDialogues(string languageCode)
        {
            currentLanguage = languageCode;
            currentLanguageData = new Dictionary<string, DialogueSentence>();

            TextAsset languageFileDialogues = Resources.Load<TextAsset>($"Localization/{languageCode}/dialogues_{languageCode}");

            if (languageFileDialogues == null)
            {
                Debug.LogError($"Language file not found: dialogues_{languageCode}");
                return;
            }
            string[] lines = languageFileDialogues.text.Split('\n');
            string[] headers = lines[0].Split(new char[] { ';', '\n', '\r' });

            int keyIndex = Array.IndexOf(headers, "key");
            int nameIndex = Array.IndexOf(headers, "character_name");
            int textIndex = Array.IndexOf(headers, "text");

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;

                string[] fields = lines[i].Split(new char[] { ';', '\n', '\r' });
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
            string[] headers = lines[0].Split(new char[] { ';', '\n', '\r' });

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

        public string GetLocalizedLine(string key)
        {
            if (languageData.ContainsKey(key))
            {
                return languageData[key];
            }
            Debug.LogError($"Localized line not found for key: {key}");
            return key;
        }

        public enum Language
        {
            ru, en
        }
    }

    [System.Serializable]
    public struct PortraitEntry
    {
        public string Key;
        public string PortraitName;
        public string audioClip;
    }
}
