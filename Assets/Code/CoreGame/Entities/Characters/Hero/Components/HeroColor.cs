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
        [SerializeField] private SpriteRenderer[] _spriteRenderers;

        private ColorTweenData _damageTween;

        private Sequence _sequence;
        
        
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
            _sequence?.Kill();
        }

        [ServerRpc]
        private void _server_PlayDamageTween()
        {
            _observer_PlayDamageTween();
        }

        [ObserversRpc]
        private void _observer_PlayDamageTween()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                _sequence.Join(spriteRenderer
                    .DOColor(_damageTween.Color, _damageTween.Duration)
                    .SetLoops(2, LoopType.Yoyo));
            }
        }
    }
}