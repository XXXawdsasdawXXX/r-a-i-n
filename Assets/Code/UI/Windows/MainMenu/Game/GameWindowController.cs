using Core.GameLoop;
using Core.Network;
using Core.Save;
using Core.ServiceLocator;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using UI.Components;
using UI.Windows.Base;
using UI.Windows.MainMenu.Connection.Legacy;
using UI.Windows.MainMenu.Hero;
using UnityEngine;

namespace UI.Windows.MainMenu.Game
{
    public class GameWindowController : UIWindowController<GameWindowView>, 
        IInitializeListener, ILoadListener, ISubscriber
    {
        public bool IsInitialized { get; set; }
        
        private GameModel _gameModel;
        private HeroWindowController _heroWindow;
        private ConnectionHandler _connectionHandler;
        private GameStateMachine _gameStateMachine;

        public UniTask Initialize()
        {
            _gameModel = Container.Instance.GetService<GameModel>();
            _connectionHandler = Container.Instance.GetService<ConnectionHandler>();
            _gameStateMachine = Container.Instance.GetService<GameStateMachine>();
            
            view.WorldsRadioGroup.Initialize();
            
            return UniTask.CompletedTask;
        }

        protected override void OnDestroy()
        {
            Unsubscribe();
            base.OnDestroy();
        }

        public void Subscribe()
        {
            windowManager.GetWindow<HeroWindowController>().HeroListChanged += _updateObjectLockerState;
            
            view.ButtonContinue.Clicked += _continueGame;
            view.ButtonJoin.Clicked += _openJoinWindow;
            view.TextUserIP.Clicked += _copyIpToBuffer;
            view.WorldsRadioGroup.Selected += _changeSelectedWorld;
        }

        public UniTask GameLoad(GameModel model)
        {
            view.TextUserIP.SetText($"IP: {ConnectionHandler.GetLocalIPAddress()}");
            
            _updateObjectLockerState();
            
            _updateWorldList();
            
            return UniTask.CompletedTask;
        }
        
        public void Unsubscribe()
        {
            windowManager.GetWindow<HeroWindowController>().HeroListChanged -= _updateObjectLockerState;
            
            view.ButtonContinue.Clicked -= _continueGame;
            view.ButtonJoin.Clicked -= _openJoinWindow;
            view.TextUserIP.Clicked -= _copyIpToBuffer;
            view.WorldsRadioGroup.Selected -= _changeSelectedWorld;
        }

        private void _continueGame()
        {
            _connectionHandler.StartHost();
            
            _gameStateMachine.SwitchState(typeof(CoreGameState));
        }

        private void _updateObjectLockerState()
        {
            view.ObjectLocker.SetActive(_gameModel.Heroes.Count == 0);
        }

        private void _openJoinWindow()
        {
            windowManager.OpenWindow<ConnectionWindowController>();
        }

        private void _changeSelectedWorld(int worldIndex)
        {
            _gameModel.LastWorldIndex = worldIndex;
        }

        private void _copyIpToBuffer()
        {
            GUIUtility.systemCopyBuffer = $"{ConnectionHandler.GetLocalIPAddress()}";
        }

        private void _updateWorldList()
        {
            if (_gameModel.Worlds.Count > _gameModel.LastWorldIndex)
            {
                view.WorldsRadioGroup.Pool.DisableAll();
            
                foreach (WorldModel modelWorld in _gameModel.Worlds)
                {
                    UIText worldTabView = view.WorldsRadioGroup.Pool.GetNext();
                
                    worldTabView.SetText(modelWorld.Name);
                }
                
                view.WorldsRadioGroup.Pool.Enabled[_gameModel.LastWorldIndex].Select();
            }
            
            view.ButtonContinue.SetInteractable(_gameModel.Worlds.Count > _gameModel.LastWorldIndex);
            view.ButtonDelete.SetInteractable(_gameModel.Worlds.Count > _gameModel.LastWorldIndex);
        }
    }
}