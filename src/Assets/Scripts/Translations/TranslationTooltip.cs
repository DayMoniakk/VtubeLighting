using UnityEngine;
using VtubeLighting.Tooltips;

namespace VtubeLighting.Translation
{
    public class TranslationTooltip : MonoBehaviour, ITranslation
    {
        [SerializeField] private string headerKey;
        [SerializeField] private string contentKey;
        [SerializeField] private TooltipTrigger tooltipTrigger;

        private void Reset()
        {
            tooltipTrigger = GetComponent<TooltipTrigger>();
        }

        private void Awake()
        {
            TranslationsManager.Instance.AddTextReference(this);
        }

        public void RefreshLanguage(TranslationsManager translationsManager)
        {
            if (contentKey == "")
            {
                Debug.LogWarning("No translation key has been set for " + gameObject.name, gameObject);
                return;
            }

            tooltipTrigger.Refresh(headerKey == "" ? "" : translationsManager.GetTranslation(headerKey), translationsManager.GetTranslation(contentKey));
        }
    }
}