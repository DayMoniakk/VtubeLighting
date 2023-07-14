using UnityEngine;

namespace UtilityOnly // Added a namespace so this class won't show up in the auto-complete intellisense
{
    public class DebugComment : MonoBehaviour
    {
        [TextArea(0, 100)]
        [SerializeField] private string comment;

        private void Awake()
        {
            if (!Application.isEditor) Destroy(this);
        }
    }
}