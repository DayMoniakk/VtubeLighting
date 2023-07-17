using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace VtubeLighting.Translation
{
    public class TranslationsManager : MonoBehaviour
    {
        public static TranslationsManager Instance { get; private set; } // Only used for "TranslationText" scripts to avoid to manually reference them

        private int currentLanguage;

        private List<Dictionary<string, object>> translations = new();
        private List<ITranslation> translationTexts = new();

        private void Awake()
        {
            Instance = this;

            ScanTranslationsFolder();
        }

        public bool HandleStartupLanguage(int languageIndex, string languageName) // Called whenever the app starts
        {
            if (translations.Count < languageIndex) return false; // If no translations files are found we just exit
            if (languageIndex == 0) return false; // if the language is already set to english there's no need to update the whole user interface

            // This is to prevent the app from switching to a different language if new languages have been added or removed.
            if ((string)translations[languageIndex]["LANGUAGE_NAME"] == languageName)
            {
                SetLanguage(languageIndex);
                return true;
            }
            else return false;
        }

        private void ScanTranslationsFolder()
        {
            string[] validFiles = Directory.GetFiles(Application.dataPath + "/translations", "*.json");

            foreach (string file in validFiles)
            {
                LoadTranslation(file);
            }
        }

        private void LoadTranslation(string path)
        {
            string json = File.ReadAllText(path);
            Dictionary<string, object> translation = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            translations.Add(translation);
        }

        public string GetTranslation(string translationKey)
        {
            if (translations.Count == 0) return "ERROR: LANGUAGE FILE MISSING";
            else return (string)translations[currentLanguage][translationKey];
        }

        public List<string> GetLanguagesList()
        {
            List<string> langs = new();

            for (int i = 0; i < translations.Count; i++)
            {
                langs.Add((string)translations[i]["LANGUAGE_NAME"]);
            }

            if (langs.Count == 0) langs.Add("English");

            return langs;
        }

        public void SetLanguage(int index)
        {
            currentLanguage = index;

            foreach (ITranslation tt in translationTexts)
            {
                tt.RefreshLanguage(this);
            }
        }

        public void AddTextReference(ITranslation translationText) => translationTexts.Add(translationText);
    }
}