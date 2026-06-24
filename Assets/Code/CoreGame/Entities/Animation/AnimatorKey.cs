using System.Collections.Generic;
using UnityEngine;

namespace CoreGame.Entities.Animation
{
    public static class AnimatorKey
    {
        public enum ECharacterAnimationState  
        {
            Idle,
            Move,
            Harvest,
            Eat,
        }

        public enum EHarvestType
        {
            None = 0,
            Mine = 1,
            PickUp = 2,
        }
        
        public static readonly int PARAM_ANIMATION_SPEED = Animator.StringToHash("AnimationSpeed");
        public static readonly int PARAM_RESOURCE_VALUE = Animator.StringToHash("ResourceValue");

        public static readonly int PARAM_HARVEST_TYPE = Animator.StringToHash("HarvestType");
        public static readonly int PARAM_SPEED = Animator.StringToHash("Speed");
        public static readonly int PARAM_EAT = Animator.StringToHash("IsEat");

        private static readonly int STATE_IDLE = Animator.StringToHash("Idle");
        private static readonly int STATE_MOVE = Animator.StringToHash("Move");
        private static readonly int STATE_HARVEST = Animator.StringToHash("Harvest");
        private static readonly int STATE_EAT = Animator.StringToHash("Eat");
        private static readonly int STATE_MINE = Animator.StringToHash("Mine");
        private static readonly int STATE_PICK_UP = Animator.StringToHash("PickUp");
        
        public static readonly Dictionary<int, ECharacterAnimationState> CHARACTER_STATES = new()
        {
            {STATE_IDLE, ECharacterAnimationState.Idle},
            {STATE_MOVE, ECharacterAnimationState.Move},
            {STATE_HARVEST, ECharacterAnimationState.Harvest},
            {STATE_MINE, ECharacterAnimationState.Harvest},
            {STATE_PICK_UP, ECharacterAnimationState.Harvest},
            {STATE_EAT, ECharacterAnimationState.Eat},
        };
    }
}