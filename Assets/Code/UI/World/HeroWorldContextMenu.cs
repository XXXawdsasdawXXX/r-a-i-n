using Core.GameLoop;
using Core.ServiceLocator;
using CoreGame.Card.Logic;
using CoreGame.Entities.Characters.Hero;
using Cysharp.Threading.Tasks;
using UI.Components;
using UnityEngine;

namespace UI.World
{
    public sealed class HeroWorldContextMenu : Essential.Mono, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }
        
        [SerializeField] private Hero _hero;
        [SerializeField] private RectTransform _body;
        [SerializeField] private UIButton _duelButton;
        
        private NetworkDuelService _duelService;

        
        public UniTask Initialize()
        {
            _duelService = Container.Instance.GetService<NetworkDuelService>();
            _setOpen(false);
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _duelButton.Clicked += _onDuelClicked;
        }

        public void Unsubscribe()
        {
            _duelButton.Clicked -= _onDuelClicked;
        }
        
        private void _onDuelClicked()
        {
            _duelService.OpenChallengeSetup(_hero.NetworkObject.ObjectId.ToString(), _hero.Name.Name);
        }
        
        private void _setOpen(bool isOpen)
        {
            if (_body != null)
            {
                _body.gameObject.SetActive(isOpen);
            }
        }
    }
}
