using System;
using Code.CoreGame.Common.Collisions;
using Core.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Code.CoreGame.Harvest
{
    [Serializable]
    public class Resource : Essential.Mono
    {
        [field: SerializeField] public EResource Type { get; private set; }
        [field: SerializeField] public float2 Position { get; private set; }

        [SerializeField] private InteractionTrigger _interactionTrigger;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        

        public void Subscribe()
        {
            _interactionTrigger.InteractionPerformed += OnInteractionPerformed;
        }

        public void Unsubscribe()
        {
            _interactionTrigger.InteractionPerformed -= OnInteractionPerformed;
        }

        private void OnInteractionPerformed()
        {
            
        }
        
        
#if UNITY_EDITOR
        public void Validate(ResourceConfig resourceConfig)
        {
            Position = transform.position.AsFloat2();
        }
#endif
    }
}