using System;
using Core.GameLoop;
using Core.Localization;
using Core.ServiceLocator;
using CoreGame.Card.Logic;
using CoreGame.Entities.Characters.Hero;
using Cysharp.Threading.Tasks;
using FishNet;
using UI.Components;
using UnityEngine;

namespace UI.World
{
    public sealed class HeroWorldContextMenu : Essential.Mono, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }
        
        public bool IsOpen => _isOpen;
        public event Action Closed;

        [SerializeField] 
        private Hero _hero;
        
        [SerializeField] 
        private RectTransform _body;
        
        [SerializeField] 
        private UIButton _duelButton;
        
        [SerializeField] 
        private UIText _duelButtonLabel;

        private NetworkDuelService _duelService;
        private LocalizationService _localization;
        private bool _isOpen;


        public UniTask Initialize()
        {
            _duelService = Container.Instance.GetService<NetworkDuelService>();
            _localization = Container.Instance.GetService<LocalizationService>();
            _setOpen(false);
            
            Debug.Log($"initialize context menu {_duelService != null}");
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            if (_duelButton != null)
            {
                _duelButton.Clicked += _onDuelClicked;
            }
        }

        public void Unsubscribe()
        {
            if (_duelButton != null)
            {
                _duelButton.Clicked -= _onDuelClicked;
            }
        }
        
        private void _onDuelClicked()
        {
            _duelService.OpenChallengeSetup(_hero.NetworkObject.ObjectId.ToString(), _hero.Name.Name);
        }

        
        private void _setOpen(bool isOpen)
        {
            _isOpen = isOpen;
            if (_body != null)
            {
                _body.gameObject.SetActive(isOpen);
            }
        }

        private static int _getOnlinePlayerCount()
        {
            if (InstanceFinder.IsServerStarted)
            {
                return InstanceFinder.ServerManager.Clients.Count;
            }

            return InstanceFinder.IsClientStarted ? 1 : 0;
        }
    }

 
}
