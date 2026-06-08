using System;
using Core.Extensions;
using Core.GameLoop;
using Core.Input;
using Core.ServiceLocator;
using CoreGame.Common.Selectable;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CoreGame.Common.Collisions
{
    public sealed class InteractionTrigger : Trigger, IInitializeListener
    {
        public event Action InteractionPerformed;  
        public bool IsInitialized { get; set; }

        [SerializeField] private SelectableMaterial _selectableMaterial;
        
        private InputManager _inputManager;
        
        public UniTask Initialize()
        {
            _inputManager = Container.Instance.GetService<InputManager>();
            
            return UniTask.CompletedTask;
        }

        protected override void OnTriggerEnter2D(Collider2D col)
        {
            _selectableMaterial.Select();
            
            _subscribeToInputEvent(true);
            
            base.OnTriggerEnter2D(col);
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            _selectableMaterial.Deselect();
            
            _subscribeToInputEvent(false);
            
            base.OnTriggerExit2D(other);
        }

        private void _subscribeToInputEvent(bool flag)
        {
            if (flag)
            {
                _inputManager.ActionEnded += _tryInvokeInteraction;
            }
            else
            {
                _inputManager.ActionEnded -= _tryInvokeInteraction;
            }
        }

        private void _tryInvokeInteraction(EInputAction action)
        {
            if (action is EInputAction.Interaction)
            {
                InteractionPerformed?.Invoke();
            }
        }

#if UNITY_EDITOR

        [Space, Header("EDITOR")] 
        [SerializeField] private BoxCollider2D _boxCollider2D;
        private void OnDrawGizmos()
        {
            if (_boxCollider2D == null)
            {
                _boxCollider2D = GetComponentInChildren<BoxCollider2D>(true);
            }

            if (_boxCollider2D != null)
            {
                Color color = Color.gray;
                color.a = 0.4f;
                Gizmos.color = color;
                
                Gizmos.DrawCube(
                    _boxCollider2D.transform.position + _boxCollider2D.offset.AsVector3(),
                    _boxCollider2D.size.AsVector3());
            }
        }
#endif
    }
}