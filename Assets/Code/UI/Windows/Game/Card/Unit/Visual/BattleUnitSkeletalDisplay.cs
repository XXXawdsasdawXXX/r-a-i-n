using CoreGame.Entities.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Game.Card.Unit.Visual
{
    public class BattleUnitSkeletalDisplay : MonoBehaviour
    {
        private UnitRenderStudio.Session _session;
        private RawImage _rawImage;
        private Material _defaultMaterial;
        private bool _isActive;
        private float _uiDisplayScale = 1f;

        public bool IsActive => _isActive;
        public RawImage RawImage => _rawImage;
        public Graphic DisplayGraphic => _rawImage;

        public void Apply(
            UnitVisualProfile profile,
            bool faceRight,
            RectTransform displayParent,
            Material highlightMaterialTemplate)
        {
            Clear();

            if (profile == null || profile.DisplayMode != UnitVisualProfile.EDisplayMode.Skeletal || displayParent == null)
            {
                return;
            }

            _uiDisplayScale = profile.UiDisplayScale > 0f ? profile.UiDisplayScale : 1f;
            _session = UnitRenderStudio.Instance.Rent(profile);
            if (_session == null)
            {
                return;
            }

            _ensureRawImage(displayParent);
            _session.Animator.SetFacing(faceRight);
            _rawImage.texture = _session.Texture;
            _rawImage.uvRect = _session.UvRect;
            _rawImage.color = Color.white;
            _applyHighlightMaterial(highlightMaterialTemplate);
            _applyUiScale(faceRight);
            _rawImage.gameObject.SetActive(true);
            _isActive = true;
        }

        public void SetFacing(bool faceRight)
        {
            _session?.Animator?.SetFacing(faceRight);
            _applyUiScale(faceRight);
        }

        public void Clear()
        {
            if (_session != null)
            {
                UnitRenderStudio.Instance.Release(_session);
                _session = null;
            }

            if (_rawImage != null)
            {
                _rawImage.texture = null;
                _rawImage.material = _defaultMaterial;
                _rawImage.gameObject.SetActive(false);
            }

            _isActive = false;
        }

        public void PlayCastAnimation(AnimatorKey.ECardCastAnimation castAnimation)
        {
            if (_session == null || _session.Animator == null)
            {
                return;
            }
            
            _session.Animator.PlayCastAnimation(castAnimation);
        }
        
        private void OnDisable()
        {
            Clear();
        }

        private void OnDestroy()
        {
            if (_defaultMaterial != null)
            {
                Destroy(_defaultMaterial);
                _defaultMaterial = null;
            }

            Clear();
        }

        private void _applyHighlightMaterial(Material highlightMaterialTemplate)
        {
            if (_rawImage == null)
            {
                return;
            }

            Material template = BattleHighlightStyle.CreateUiHighlightInstance(highlightMaterialTemplate);
            if (_defaultMaterial != null)
            {
                Destroy(_defaultMaterial);
                _defaultMaterial = null;
            }

            if (template == null)
            {
                _rawImage.material = null;
                return;
            }

            _defaultMaterial = template;
            _rawImage.material = _defaultMaterial;
        }

        private void _applyUiScale(bool faceRight)
        {
            if (_rawImage == null)
            {
                return;
            }

            float scale = Mathf.Max(0.1f, _uiDisplayScale);
            _rawImage.rectTransform.localScale = new Vector3(faceRight ? scale : -scale, scale, 1f);
        }

        private void _ensureRawImage(RectTransform parent)
        {
            if (_rawImage == null)
            {
                GameObject viewObject = new GameObject(
                    "skeletal-view",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(RawImage));

                RectTransform rect = viewObject.GetComponent<RectTransform>();
                rect.SetParent(parent, false);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.SetAsFirstSibling();

                _rawImage = viewObject.GetComponent<RawImage>();
                _rawImage.raycastTarget = false;
            }
            else
            {
                _rawImage.rectTransform.SetParent(parent, false);
                _rawImage.rectTransform.SetAsFirstSibling();
            }
        }
    }
}
