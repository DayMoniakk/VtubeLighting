using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VtubeLighting.Tooltips
{
    public class Tooltip : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int characterWrapLimit = 80;

        [Header("References")]
        [SerializeField] private TMP_Text headerField;
        [SerializeField] private TMP_Text contentField;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private RectTransform rectTransform;

        private void Reset()
        {
            headerField = transform.GetChild(0).GetComponent<TMP_Text>();
            contentField = transform.GetChild(1).GetComponent<TMP_Text>();
            layoutElement = GetComponent<LayoutElement>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetText(string content, string header = "")
        {
            headerField.gameObject.SetActive(!string.IsNullOrEmpty(header));

            headerField.text = header;
            contentField.text = content;

            UpdateSize();
        }

        private void UpdateSize()
        {
            if (!headerField || !contentField) return;

            layoutElement.enabled = headerField.text.Length > characterWrapLimit || contentField.text.Length > characterWrapLimit;
        }

        private void Update()
        {
            Vector2 pos = Input.mousePosition;
            rectTransform.pivot = new(pos.x / Screen.width, pos.y / Screen.height);
            transform.position = pos;
        }
    }
}