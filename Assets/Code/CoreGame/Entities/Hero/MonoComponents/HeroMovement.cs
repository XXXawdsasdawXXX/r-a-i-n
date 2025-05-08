using Core.Data;
using Core.GameLoop;
using Core.Input;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using UnityEngine;

namespace Code.CoreGame.Entities.Hero
{
    public class HeroMovement : NetworkBehaviour, IInitializeListener, IFixedUpdateListener
    {
        public bool IsInitialized { get; set; }
        public string RuntimeListenerName => "HeroMovement";
        public Condition Condition { get; } = new();
        
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private float _moveSpeed = 5f;

        private InputManager _inputManager;

        public UniTask Initialize()
        {
            _inputManager = Container.Instance.GetService<InputManager>();
            
            return UniTask.CompletedTask;
        }
        
        public void GameFixedUpdate(float fixedDeltaTime)
        {
            if (Condition.AreMet())
            {
                _rigidbody2D.velocity = _inputManager.Direction.normalized * _moveSpeed * fixedDeltaTime;
            }
        }
    }
}