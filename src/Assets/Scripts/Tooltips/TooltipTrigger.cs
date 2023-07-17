using UnityEngine;
using UnityEngine.EventSystems;

namespace VtubeLighting.Tooltips
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string header;
        [SerializeField][TextArea] private string content;

        private bool isOpen;

        public void OnPointerEnter(PointerEventData eventData)
        {
            isOpen = true;
            TooltipManager.Show(content, header);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isOpen = false;
            TooltipManager.Hide();
        }

        public void Refresh(string header, string content)
        {
            this.header = header;
            this.content = content;

            if (isOpen) TooltipManager.Show(content, header);
        }
    }
}