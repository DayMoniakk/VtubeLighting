using UnityEngine;

namespace VtubeLighting.Tooltips
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private Tooltip tooltip;

        private static TooltipManager current;
        private void Awake() => current = this;

        public static void Show(string content, string header = "")
        {
            current.tooltip.SetText(content, header);
            current.tooltip.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            current.tooltip.gameObject.SetActive(false);
        }
    }
}