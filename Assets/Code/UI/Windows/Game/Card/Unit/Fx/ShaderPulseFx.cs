using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UI.Windows.Game.Card.Unit.Impacts;
using UnityEngine.UI;

namespace UI.Windows.Game.Card.Unit.Fx
{
    public sealed class ShaderPulseFx : ICardImpact, IUnitImpact
    {
        private readonly BattleUnitView _view;

        public ShaderPulseFx(BattleUnitView view)
        {
            _view = view;
        }

        public async UniTask Play(UnitFxSettings settings, CancellationToken cancellationToken)
        {
            if (_view == null || settings == null || !_view.TryGetImpactTargets(out Graphic overlayGraphic))
            {
                return;
            }

            float duration = Mathf.Max(0.05f, settings.Duration);
            float targetScale = Mathf.Max(1f, settings.Scale);
            Color baseColor = _view.GetDefaultRenderColor();
            Color toColor = settings.Color;
            toColor.a = baseColor.a;

            overlayGraphic.color = baseColor;
            _view.SetImpactScale(1f);

            Sequence sequence = DOTween.Sequence()
                .SetUpdate(true)
                .Append(DOVirtual.Float(0f, 1f, duration * 0.4f, t =>
                {
                    overlayGraphic.color = Color.Lerp(baseColor, toColor, t);
                    _view.SetImpactScale(Mathf.Lerp(1f, targetScale, t));
                }))
                .Append(DOVirtual.Float(0f, 1f, duration * 0.6f, t =>
                {
                    overlayGraphic.color = Color.Lerp(toColor, baseColor, t);
                    _view.SetImpactScale(Mathf.Lerp(targetScale, 1f, t));
                }))
                .SetLink(_view.gameObject, LinkBehaviour.KillOnDisable);

            using (cancellationToken.Register(() => sequence.Kill()))
            {
                await sequence.AsyncWaitForCompletion().AsUniTask();
            }
        }
    }
}
