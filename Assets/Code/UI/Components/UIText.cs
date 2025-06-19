using DG.Tweening;
using Plugins.Demigiant.DOTween.Modules;
using TMPro;
using UnityEngine;

namespace UI.Components
{
    public class UIText : UISelectable
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        private Tween _tween;

        public void SetText(string text)
        {
            _textMeshPro.SetText(text);
        }

        public override void SetInteractable(bool isInteractable)
        {
            _textMeshPro.raycastTarget = isInteractable;
        }

        public void Colorize(ColorTweenData tweenData)
        {
            _tween?.Kill();
            
            _tween = _textMeshPro.DOColor(tweenData.Color, tweenData.Duration)
                .SetEase(tweenData.Ease)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }
    }
}