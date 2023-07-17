using UnityEngine;
using TMPro;

namespace VtubeLighting.Translation
{
    public class TranslationText : MonoBehaviour, ITranslation
    {
        [SerializeField] private string translationKey;
        [SerializeField] private TMP_Text text;

        private void Reset() => text = GetComponent<TMP_Text>();

        private void Awake()
        {
            TranslationsManager.Instance.AddTextReference(this);
        }

        private void Start()
        {
            // Idk why but this fix the bug where the warning text is not localized if no Spout2 source are available when lauching the app
            if (translationKey == "warning.no_signal") RefreshLanguage(FindObjectOfType<TranslationsManager>());
        }

        public void RefreshLanguage(TranslationsManager translationsManager)
        {
            if (translationKey == "")
            {
                Debug.LogWarning("No translation key has been set for " + gameObject.name, gameObject);
                return;
            }

            text.text = translationsManager.GetTranslation(translationKey);
        }
    }
}