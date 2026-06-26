using Core.Localization;
using Core.ServiceLocator;
using CoreGame.Card.Data;
using UI.Components;
using UI.Windows.Base;
using UI.Windows.Game.Card.Unit.Visual;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using CoreGame.Entities.Animation;
using TriInspector;
using UI.Windows.Game.Card.Unit.Fx;
using UI.Windows.Game.Card.Unit.Impacts;
using UnityEngine.UI;

namespace UI.Windows.Game.Card.Unit
{
    public class BattleUnitView : UIWindowView
    {
        public event Action Clicked;
        public event Action<BattleUnitView> HoverEntered;
        public event Action<BattleUnitView> HoverExited;

        public UIHighlightMaterialController HighlightController { get; private set; }
        public Material HighlightMaterialTemplate =>
            BattleHighlightStyle.ResolveHighlightMaterial(_resolveHighlightTemplateSource());
        public bool UsesSkeletalDisplay => _skeletalDisplay != null && _skeletalDisplay.IsActive;
        public bool IsRightSide => _isRightSide;
        
        [field: SerializeField] public UIImage Render { get; private set; }

        [SerializeField] private UIHighlightMaterialController.EType _highlightType = UIHighlightMaterialController.EType.Outline;

        [field: Title("Params")]
        [field: SerializeField] public UIImage HealthFill { get; private set; }
        [field: SerializeField] public UIText HealthText { get; private set; }
        [field: SerializeField] public UIBattleStateIcon Armor { get; private set; }
        [field: SerializeField] public UIBattleStateIcon Attack { get; private set; }
        
        [SerializeField] private UIText _companionInfo;
        [SerializeField] private UIButton _clickArea;
        [SerializeField] private BattleUnitSkeletalDisplay _skeletalDisplay;
        [SerializeField] private UnitVisualLibrary _visualLibrary;
        [SerializeField] private UnitVisualProfile _fallbackHeroProfile;

        [Title("Play Card FX")]
        [SerializeField] private RectTransform _fxRoot;
        [SerializeField] private ECardImpactType _defaultCardImpactType = ECardImpactType.ShaderPulse;
        [SerializeField] private UnitFxSettings _defaultCardFxSettings = new UnitFxSettings();
        [SerializeField] private List<CardFxBinding> _cardFxBindings = new List<CardFxBinding>();

        [Title("Reaction FX")]
        [SerializeField] private EUnitImpactType _defaultUnitImpactType = EUnitImpactType.SpriteSequence;
        [SerializeField] private UnitFxSettings _defaultReactionFxSettings = new UnitFxSettings();
        [SerializeField] private List<EffectReactionBinding> _effectReactionBindings = new List<EffectReactionBinding>();

        private readonly Dictionary<ECardImpactType, ICardImpact> _cardImpacts = new Dictionary<ECardImpactType, ICardImpact>();
        private readonly Dictionary<EUnitImpactType, IUnitImpact> _unitImpacts = new Dictionary<EUnitImpactType, IUnitImpact>();
       
        private UnitFxRunner _fxRunner;
        private Color _defaultRenderColor = Color.white;
        private bool _isRightSide;
        private BattleUnit _currentUnit;
        private UnitVisualProfile _activeProfile;
        private Material _defaultHighlightMaterial;
        private Sprite _staticRenderSprite;
        private string _appliedSkeletalUnitId;
        private UnitVisualProfile _appliedSkeletalProfile;
        
        
        private void OnEnable()
        {
            _cacheRenderDefaults();
            HighlightController = new UIHighlightMaterialController(_getHighlightGraphic(), _highlightType);
            _applyRenderMirror();
            _fxRunner = new UnitFxRunner(this);
            _initializeImpactDictionaries();
            _ensureSkeletalDisplay();

            if (_clickArea != null)
            {
                _clickArea.Clicked += _onClicked;
                _clickArea.Selected += _onHoverEntered;
                _clickArea.Deselected += _onHoverExited;
            }
        }

        private void OnDisable()
        {
            HighlightController?.Reset();
            _fxRunner?.Stop();
            _skeletalDisplay?.Clear();

            if (_clickArea != null)
            {
                _clickArea.Clicked -= _onClicked;
                _clickArea.Selected -= _onHoverEntered;
                _clickArea.Deselected -= _onHoverExited;
            }
        }
        
        public void Set(BattleUnit unit)
        {
            HighlightController?.Reset();
            _currentUnit = unit;
            
            if (unit == null)
            {
                _skeletalDisplay?.Clear();
                Close();
                return;
            }

            Open();
            _applyVisual(unit);
            _applyRenderMirror();
            _refreshHighlightController();

            float maxHp = Mathf.Max(1f, unit.MaxHP);
            float hp = Mathf.Max(0f, unit.HP);

            HealthFill.SetFillAmount(hp / maxHp);
            HealthText.SetText($"{Mathf.CeilToInt(hp)}/{Mathf.CeilToInt(maxHp)}");

            _setStateIcon(Armor, unit.Armor);
            _setStateIcon(Attack, unit.AutoActionType == EAutoActionType.AttackEnemyHero ? unit.AutoActionValue : 0f);
            _setCompanionInfo(unit);
        }

        public void SetSide(bool isRightSide)
        {
            _isRightSide = isRightSide;
            _skeletalDisplay?.SetFacing(!isRightSide);
            _applyRenderMirror();
        }
        
        public void PlayCardFx(ECardType cardType)
        {
            CardFxBinding binding = _resolveCardBinding(cardType);
            ECardImpactType impactType = binding != null ? binding.ImpactType : _defaultCardImpactType;
            if (_cardImpacts.TryGetValue(impactType, out ICardImpact impact))
            {
                _fxRunner?.Play(impact, binding?.Settings ?? _defaultCardFxSettings);
            }
        }

        public void PlayReactionFx(EEffectType effectType)
        {
            EffectReactionBinding binding = _resolveReactionBinding(effectType);
            EUnitImpactType impactType = binding != null ? binding.ImpactType : _defaultUnitImpactType;
            if (_unitImpacts.TryGetValue(impactType, out IUnitImpact impact))
            {
                _fxRunner?.Play(impact, binding?.Settings ?? _defaultReactionFxSettings);
            }
        }

        [Button]
        public void PlayCastAnimation(AnimatorKey.ECardCastAnimation castAnimation)
        {
            if (castAnimation == AnimatorKey.ECardCastAnimation.None || _skeletalDisplay == null)
            {
                return;
            }

            _skeletalDisplay.PlayCastAnimation(castAnimation);
        }
        
        private Material _resolveHighlightTemplateSource()
        {
            if (_defaultHighlightMaterial != null && BattleHighlightStyle.IsHighlightCompatible(_defaultHighlightMaterial))
            {
                return _defaultHighlightMaterial;
            }

            if (Render?.Image?.material != null && BattleHighlightStyle.IsHighlightCompatible(Render.Image.material))
            {
                return Render.Image.material;
            }

            return BattleHighlightStyle.HighlightMaterial;
        }

        private void _onClicked()
        {
            Clicked?.Invoke();
        }

        private void _onHoverEntered()
        {
            if (_currentUnit == null)
            {
                return;
            }

            HoverEntered?.Invoke(this);
        }

        private void _onHoverExited()
        {
            HoverExited?.Invoke(this);
        }

       

        private CardFxBinding _resolveCardBinding(ECardType cardType)
        {
            foreach (CardFxBinding binding in _cardFxBindings)
            {
                if (binding == null)
                {
                    continue;
                }

                if (cardType.HasFlag(binding.Matches))
                {
                    return binding;
                }
            }

            return null;
        }

        private EffectReactionBinding _resolveReactionBinding(EEffectType effectType)
        {
            foreach (EffectReactionBinding binding in _effectReactionBindings)
            {
                if (binding == null)
                {
                    continue;
                }

                if (binding.Matches == effectType)
                {
                    return binding;
                }
            }

            return null;
        }

        private void _initializeImpactDictionaries()
        {
            _cardImpacts.Clear();
            _unitImpacts.Clear();

            _cardImpacts[ECardImpactType.ShaderPulse] = new ShaderPulseFx(this);
            _cardImpacts[ECardImpactType.SpriteSequence] = new SpriteSequenceFx(this);

            _unitImpacts[EUnitImpactType.ShaderPulse] = new ShaderPulseFx(this);
            _unitImpacts[EUnitImpactType.SpriteSequence] = new SpriteSequenceFx(this);
        }

        private static void _setStateIcon(UIBattleStateIcon stateIcon, float value)
        {
            if (stateIcon?.Icon == null || stateIcon.Value == null)
            {
                return;
            }

            bool show = value > 0f;
            stateIcon.Icon.gameObject.SetActive(show);
            stateIcon.Value.gameObject.SetActive(show);

            if (show)
            {
                stateIcon.Value.SetText(Mathf.CeilToInt(value).ToString());
            }
        }

        private void _setCompanionInfo(BattleUnit unit)
        {
            if (_companionInfo == null)
            {
                return;
            }

            if (unit == null || !unit.IsCompanion)
            {
                _companionInfo.gameObject.SetActive(false);
                return;
            }

            int turnsLeft = unit.Statuses?
                .FirstOrDefault(status => status.Type == EStatusType.SummonDuration)?.Duration ?? 0;
            bool isTemporary = turnsLeft > 0;

            LocalizationService localization = LocalizationService.TryGet();
            string companionInfo = localization != null
                ? localization.BuildCompanionInfo(isTemporary, turnsLeft, Mathf.Max(0, unit.CompanionCardsPerTurn))
                : $"{(isTemporary ? $"Temporary: {turnsLeft} turn(s)" : "Lifetime: until death")} | Cards/turn: {Mathf.Max(0, unit.CompanionCardsPerTurn)}";
            _companionInfo.SetText(companionInfo);
            _companionInfo.gameObject.SetActive(true);
        }
        
        protected override void OnDestroy()
        {
            HighlightController?.Dispose();
            HighlightController = null;
            _fxRunner?.Stop();
            _fxRunner = null;
            _cardImpacts.Clear();
            _unitImpacts.Clear();
            
            base.OnDestroy();
        }

        public bool TryGetImpactTargets(out Graphic overlayGraphic)
        {
            if (UsesSkeletalDisplay && _skeletalDisplay?.DisplayGraphic != null)
            {
                overlayGraphic = _skeletalDisplay.DisplayGraphic;
                return true;
            }

            overlayGraphic = Render?.Image;
            return overlayGraphic != null;
        }

        public void SetImpactScale(float scale)
        {
            RectTransform target = _fxRoot != null ? _fxRoot : transform as RectTransform;
            if (target == null)
            {
                return;
            }

            target.localScale = Vector3.one * Mathf.Max(0.01f, scale);
        }

        public void ResetImpactVisualState()
        {
            if (_fxRoot != null || transform is RectTransform)
            {
                RectTransform target = _fxRoot != null ? _fxRoot : transform as RectTransform;
                target.localScale = Vector3.one;
            }

            if (UsesSkeletalDisplay && _skeletalDisplay.RawImage != null)
            {
                _skeletalDisplay.RawImage.color = _defaultRenderColor;
                _applyRenderMirror();
                return;
            }

            if (Render?.Image == null)
            {
                return;
            }

            Render.Image.color = _defaultRenderColor;
            _applyRenderMirror();
        }

        public Color GetDefaultRenderColor()
        {
            if (UsesSkeletalDisplay && _skeletalDisplay.RawImage != null)
            {
                return _skeletalDisplay.RawImage.color;
            }

            return _defaultRenderColor;
        }

        private void _cacheRenderDefaults()
        {
            if (UsesSkeletalDisplay && _skeletalDisplay.RawImage != null)
            {
                _defaultRenderColor = _skeletalDisplay.RawImage.color;
            }
            else if (Render?.Image != null)
            {
                _defaultRenderColor = Render.Image.color;
            }

            if (Render?.Image != null)
            {
                _staticRenderSprite = Render.Image.sprite;
                _defaultHighlightMaterial = Render.Image.material;
            }
        }

        private void _applyRenderMirror()
        {
            RectTransform mirrorTarget = _getMirrorTarget();
            if (mirrorTarget == null)
            {
                return;
            }

            Vector3 scale = mirrorTarget.localScale;
            float x = Mathf.Abs(scale.x) > 0.001f ? Mathf.Abs(scale.x) : 1f;
            scale.x = _isRightSide ? -x : x;
            mirrorTarget.localScale = scale;
        }

        private RectTransform _getMirrorTarget()
        {
            if (UsesSkeletalDisplay && _skeletalDisplay.RawImage != null)
            {
                return _skeletalDisplay.RawImage.rectTransform;
            }

            return Render?.Image?.rectTransform;
        }

        private Graphic _getHighlightGraphic()
        {
            if (UsesSkeletalDisplay && _skeletalDisplay?.DisplayGraphic != null)
            {
                return _skeletalDisplay.DisplayGraphic;
            }

            return Render?.Image;
        }

        private void _applyVisual(BattleUnit unit)
        {
            _activeProfile = _resolveProfile(unit);

            if (_canKeepSkeletalSession(unit))
            {
                _skeletalDisplay.SetFacing(!_isRightSide);
                return;
            }

            _skeletalDisplay?.Clear();
            _appliedSkeletalUnitId = null;
            _appliedSkeletalProfile = null;
            _restoreStaticRenderMaterial();

            if (_activeProfile == null)
            {
                if (Render?.Image != null)
                {
                    Render.Image.enabled = true;
                }

                return;
            }

            if (_activeProfile.DisplayMode == UnitVisualProfile.EDisplayMode.Skeletal)
            {
                RectTransform displayParent = Render?.Image?.rectTransform ?? transform as RectTransform;
                Material highlightTemplate = _resolveHighlightTemplateSource();
                _skeletalDisplay?.Apply(_activeProfile, !_isRightSide, displayParent, highlightTemplate);

                if (!UsesSkeletalDisplay)
                {
                    _restoreStaticRenderMaterial();
                    return;
                }

                _syncClickRaycastForSkeletalDisplay();
                _cacheRenderDefaults();
                _refreshHighlightController();
                _appliedSkeletalUnitId = unit.UnitId;
                _appliedSkeletalProfile = _activeProfile;
                return;
            }

            if (Render?.Image != null)
            {
                Render.Image.enabled = true;
                Render.SetSprite(_activeProfile.StaticSprite);
            }
        }

        private bool _canKeepSkeletalSession(BattleUnit unit)
        {
            return unit != null
                   && _activeProfile != null
                   && _activeProfile.DisplayMode == UnitVisualProfile.EDisplayMode.Skeletal
                   && UsesSkeletalDisplay
                   && unit.UnitId == _appliedSkeletalUnitId
                   && ReferenceEquals(_activeProfile, _appliedSkeletalProfile);
        }

        private void _syncClickRaycastForSkeletalDisplay()
        {
            if (Render?.Image == null)
            {
                return;
            }

            // UIButton sits on the same object as this Image; disabling it breaks target selection clicks.
            Render.Image.enabled = true;
            Render.Image.raycastTarget = true;
            Render.Image.color = Color.clear;
            Render.Image.sprite = null;
        }

        private void _restoreStaticRenderMaterial()
        {
            if (Render?.Image == null)
            {
                return;
            }

            Render.Image.enabled = true;
            Render.Image.raycastTarget = true;
            Render.Image.color = _defaultRenderColor;

            if (_staticRenderSprite != null)
            {
                Render.Image.sprite = _staticRenderSprite;
            }

            if (_defaultHighlightMaterial != null)
            {
                Render.Image.material = _defaultHighlightMaterial;
            }
        }

        private void _refreshHighlightController()
        {
            HighlightController?.Dispose();
            Graphic highlightGraphic = _getHighlightGraphic();
            if (highlightGraphic != null)
            {
                HighlightController = new UIHighlightMaterialController(highlightGraphic, _highlightType);
            }
        }

        private UnitVisualProfile _resolveProfile(BattleUnit unit)
        {
            UnitVisualLibrary library = _visualLibrary;
            if (library == null && Container.Instance != null)
            {
                library = _tryGetVisualLibrary();
            }

            UnitVisualProfile profile = library != null
                ? library.Resolve(unit.VisualProfileId, unit.IsCompanion)
                : null;

            if (profile != null)
            {
                return profile;
            }

            return unit.IsCompanion ? null : _fallbackHeroProfile;
        }

        private static UnitVisualLibrary _tryGetVisualLibrary()
        {
            try
            {
                return Container.Instance.GetSO<UnitVisualLibrary>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void _ensureSkeletalDisplay()
        {
            if (_skeletalDisplay == null)
            {
                _skeletalDisplay = GetComponent<BattleUnitSkeletalDisplay>();
            }

            if (_skeletalDisplay == null)
            {
                _skeletalDisplay = gameObject.AddComponent<BattleUnitSkeletalDisplay>();
            }
        }

        [Serializable]
        private sealed class CardFxBinding
        {
            [SerializeField] private ECardType _matches;
            [SerializeField] private ECardImpactType _impactType = ECardImpactType.ShaderPulse;
            [SerializeField] private UnitFxSettings _settings = new UnitFxSettings();

            public ECardType Matches => _matches;
            public ECardImpactType ImpactType => _impactType;
            public UnitFxSettings Settings => _settings;

            public CardFxBinding(ECardType matches, ECardImpactType impactType, UnitFxSettings settings)
            {
                _matches = matches;
                _impactType = impactType;
                _settings = settings;
            }
        }

        [Serializable]
        private sealed class EffectReactionBinding
        {
            [SerializeField] private EEffectType _matches;
            [SerializeField] private EUnitImpactType _impactType = EUnitImpactType.SpriteSequence;
            [SerializeField] private UnitFxSettings _settings = new UnitFxSettings();

            public EEffectType Matches => _matches;
            public EUnitImpactType ImpactType => _impactType;
            public UnitFxSettings Settings => _settings;

            public EffectReactionBinding(EEffectType matches, EUnitImpactType impactType, UnitFxSettings settings)
            {
                _matches = matches;
                _impactType = impactType;
                _settings = settings;
            }
        }

        private enum ECardImpactType
        {
            ShaderPulse = 0,
            SpriteSequence = 1
        }

        private enum EUnitImpactType
        {
            ShaderPulse = 0,
            SpriteSequence = 1
        }
    }
}