using Code.CoreGame.Common.Collisions;
using Core.GameLoop;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using UnityEngine;

namespace Code.CoreGame.Harvest
{
    public class ResourceView : NetworkBehaviour, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }

        [field: SerializeField] public EResource Type { get; private set; }
        
        [SerializeField] private SpriteRenderer _view;
        [SerializeField] private InteractionTrigger _interactionTrigger;
        
        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

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
    }
}