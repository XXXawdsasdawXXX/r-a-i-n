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
            Enter,
            Exit
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
        public static readonly int PARAM_ENTER = Animator.StringToHash("Enter");
        public static readonly int PARAM_EXIT = Animator.StringToHash("Exit");
        
        private static readonly int STATE_IDLE = Animator.StringToHash("Idle");
        private static readonly int STATE_MOVE = Animator.StringToHash("Move");
        private static readonly int STATE_EAT = Animator.StringToHash("Eat");
        
        private static readonly int STATE_HARVEST = Animator.StringToHash("Harvest");
        private static readonly int STATE_HARVEST_START = Animator.StringToHash("Harvest-start");
        private static readonly int STATE_HARVEST_END = Animator.StringToHash("Harvest-end");
        
        private static readonly int STATE_MINE = Animator.StringToHash("Mine");
        private static readonly int STATE_MINE_START = Animator.StringToHash("Mine-start");
        private static readonly int STATE_MINE_END = Animator.StringToHash("Mine-end");
        private static readonly int STATE_ENTER = Animator.StringToHash("Enter");
        private static readonly int STATE_EXIT = Animator.StringToHash("Exit");
        
        
        public static readonly Dictionary<int, ECharacterAnimationState> CHARACTER_STATES = new()
        {
            {STATE_IDLE, ECharacterAnimationState.Idle},
            {STATE_MOVE, ECharacterAnimationState.Move},
            {STATE_EAT, ECharacterAnimationState.Eat},
            {STATE_MINE, ECharacterAnimationState.Harvest},
            {STATE_MINE_START, ECharacterAnimationState.Harvest},
            {STATE_MINE_END, ECharacterAnimationState.Harvest},
            {STATE_HARVEST, ECharacterAnimationState.Harvest},
            {STATE_HARVEST_START, ECharacterAnimationState.Harvest},
            {STATE_HARVEST_END, ECharacterAnimationState.Harvest},
            {STATE_ENTER, ECharacterAnimationState.Enter},
            {STATE_EXIT, ECharacterAnimationState.Exit},
        };
    }
}