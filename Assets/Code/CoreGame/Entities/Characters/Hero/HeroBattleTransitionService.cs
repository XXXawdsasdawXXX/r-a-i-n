using Core.GameLoop;
using Core.Network;
using Core.ServiceLocator;
using CoreGame.Card.Data;
using CoreGame.Card.Logic;
using CoreGame.Entities.Characters;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace CoreGame.Entities.Characters.Hero
{
    public class HeroBattleTransitionService : IService, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }

        private BattleService _battleService;
        private UserProvider _userProvider;
        private bool _exitPlayedForCurrentBattle;

        public UniTask Initialize()
        {
            _battleService = Container.Instance.GetService<BattleService>();
            _userProvider = Container.Instance.GetService<UserProvider>();
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _battleService.BattleFinished += _onBattleFinished;
        }

        public void Unsubscribe()
        {
            _battleService.BattleFinished -= _onBattleFinished;
        }

        public void RunExitThen(Action proceed)
        {
            _runExitThen(proceed).Forget();
        }

        public UniTask EnsureExitBeforeBattleAsync()
        {
            if (_exitPlayedForCurrentBattle)
            {
                return UniTask.CompletedTask;
            }

            return _playExitOnLocalHero();
        }

        private async UniTaskVoid _runExitThen(Action proceed)
        {
            await _playExitOnLocalHero();
            proceed?.Invoke();
        }

        private async UniTask _playExitOnLocalHero()
        {
            CharacterSkeletonAnimator animator = _getLocalSkeletonAnimator();
            if (animator == null)
            {
                return;
            }

            HeroAnimation heroAnimation = _userProvider.GetHeroComponent<HeroAnimation>();
            if (heroAnimation != null)
            {
                heroAnimation.RequestExit();
            }
            else
            {
                animator.TriggerExit();
            }

            await animator.PlayExit();
            _exitPlayedForCurrentBattle = true;
        }

        private async void _onBattleFinished(BattleModel _)
        {
            _exitPlayedForCurrentBattle = false;
            await _playEnterOnLocalHero();
        }

        private async UniTask _playEnterOnLocalHero()
        {
            CharacterSkeletonAnimator animator = _getLocalSkeletonAnimator();
            if (animator == null)
            {
                return;
            }

            HeroAnimation heroAnimation = _userProvider.GetHeroComponent<HeroAnimation>();
            if (heroAnimation != null)
            {
                heroAnimation.RequestEnter();
            }
            else
            {
                animator.TriggerEnter();
            }

            await animator.PlayEnter();
        }

        private CharacterSkeletonAnimator _getLocalSkeletonAnimator()
        {
            if (_userProvider?.Hero == null)
            {
                return null;
            }

            HeroAnimation heroAnimation = _userProvider.GetHeroComponent<HeroAnimation>();
            if (heroAnimation != null)
            {
                return heroAnimation.SkeletonAnimator;
            }

            return _userProvider.Hero.GetComponentInChildren<CharacterSkeletonAnimator>(true);
        }
    }
}
