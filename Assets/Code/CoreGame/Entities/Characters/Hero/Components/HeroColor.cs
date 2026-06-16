using Core.GameLoop;
using Core.ServiceLocator;
using CoreGame.Entities.Params;
using DG.Tweening;
using FishNet.Object;
using Plugins.Demigiant.DOTween.Modules;
using UnityEngine;

namespace CoreGame.Entities.Characters.Hero
{
    public class HeroColor : NetworkBehaviour, ISubscriber, IExitListener
    {
        [SerializeField] private Health _health;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private ColorTweenData _damageTween;

        private Tween _tween;

        public override void OnStartClient()
        {
            base.OnStartClient();

            enabled = IsOwner;

            _damageTween = Container.Instance.GetSO<HeroSettings>().DamageTween;
        }

        public void Subscribe()
        {
            _health.TookDamage += _server_PlayDamageTween;
        }

        public void Unsubscribe()
        {
            _health.TookDamage -= _server_PlayDamageTween;
        }
        
        public void GameExit()
        {
            _tween?.Kill();
        }

        [ServerRpc]
        private void _server_PlayDamageTween()
        {
            _observer_PlayDamageTween();
        }

        [ObserversRpc]
        private void _observer_PlayDamageTween()
        {
            _tween?.Kill();
            _tween = _spriteRenderer.DOColor(_damageTween.Color, _damageTween.Duration).SetLoops(2, LoopType.Yoyo);
        }
    }
}