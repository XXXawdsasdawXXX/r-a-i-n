using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class UIImage : Essential.Mono
    {
        [SerializeField] private Image _image;

        
        public void SetFillAmount(float normalizedValue)
        {
            _image.fillAmount = normalizedValue;
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

        private void OnValidate()
        {
            if (_image == null)
            {
                TryGetComponent(out _image);
            }
        }
    }
}