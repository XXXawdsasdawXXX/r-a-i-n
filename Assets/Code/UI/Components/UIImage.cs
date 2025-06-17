using DG.Tweening;
using Plugins.Demigiant.DOTween.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class UIImage : Essential.Mono
    {
        [SerializeField] private Image _image;

        private Tween _tween;


        public void SetFillAmount(float normalizedValue)
        {
            _image.fillAmount = normalizedValue;
        }

        public void Colorize(ColorTweenData colorTweenData)
        {
            _tween?.Kill();
            _tween = _image.DOColor(colorTweenData.Color, colorTweenData.Duration)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public float GetFillAmount()
        {
            return _image.fillAmount;
        }

        public void SetSprite(Sprite sprite)
        {
            _image.enabled = sprite != null;
            _image.sprite = sprite;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_image == null)
            {
                TryGetComponent(out _image);
            }
        }
#endif
    }
}