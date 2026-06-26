using CoreGame.Entities.Animation;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CoreGame.Entities.Characters
{
    public class CharacterSkeletonAnimator : MonoBehaviour, IAnimationStateReader
    {
        private const float ENTER_EXIT_DURATION = 0.5f;

        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _viewBody;

        private AnimatorKey.ECharacterAnimationState _currentState = AnimatorKey.ECharacterAnimationState.Idle;
        public AnimatorKey.ECharacterAnimationState CurrentState => _currentState;

            
        private void Awake()
        {
            _animator ??= GetComponentInChildren<Animator>();
            _viewBody ??= _animator != null ? _animator.transform : transform;
        }

        public void Bind(Animator animator, Transform viewBody)
        {
            _animator = animator;
            _viewBody = viewBody ?? animator.transform;
        }

        public void SetFacing(bool faceRight)
        {
            if (_viewBody == null)
            {
                return;
            }

            Vector3 scale = _viewBody.localScale;
            float x = Mathf.Abs(scale.x) > 0.001f ? Mathf.Abs(scale.x) : 1f;
            scale.x = faceRight ? x : -x;
            _viewBody.localScale = scale;
        }

        public void PlayCastAnimation(AnimatorKey.ECardCastAnimation key)
        {
            if (key is AnimatorKey.ECardCastAnimation.None || !AnimatorKey.CAST_PARAMS.ContainsKey(key))
            {
                return;
            }
            
            _animator?.SetTrigger(AnimatorKey.CAST_PARAMS[key]);
            _animator?.Update(0f);
        }
        
        public void TriggerEnter()
        {
            _animator?.SetTrigger(AnimatorKey.PARAM_ENTER);
        }

        public void TriggerExit()
        {
            _animator?.SetTrigger(AnimatorKey.PARAM_EXIT);
        }

        public UniTask PlayEnter(CancellationToken cancellationToken = default)
        {
            TriggerEnter();
            return UniTask.Delay(TimeSpan.FromSeconds(ENTER_EXIT_DURATION), cancellationToken: cancellationToken);
        }

        public UniTask PlayExit(CancellationToken cancellationToken = default)
        {
            TriggerExit();
            return UniTask.Delay(TimeSpan.FromSeconds(ENTER_EXIT_DURATION), cancellationToken: cancellationToken);
        }

        public void EnteredState(int stateHash)
        {
            if (!AnimatorKey.CHARACTER_STATES.TryGetValue(stateHash, out AnimatorKey.ECharacterAnimationState state))
            {
                return;
            }

            _currentState = state;
        }

        public void ExitedState(int stateHash)
        {
            if (!AnimatorKey.CHARACTER_STATES.TryGetValue(stateHash, out AnimatorKey.ECharacterAnimationState state))
            {
                return;
            }

            if (_currentState == state)
            {
                _currentState = AnimatorKey.ECharacterAnimationState.Idle;
            }
        }
    }
}
