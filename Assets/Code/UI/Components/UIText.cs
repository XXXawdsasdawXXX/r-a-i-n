using TMPro;
using UnityEngine;

namespace UI.Components
{
    public class UIText : UISelectable
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        
        public void SetText(string text)
        {
            _textMeshPro.SetText(text);
        }

        public override void SetInteractable(bool isInteractable)
        {
            _textMeshPro.raycastTarget = isInteractable;
        }
    }
}