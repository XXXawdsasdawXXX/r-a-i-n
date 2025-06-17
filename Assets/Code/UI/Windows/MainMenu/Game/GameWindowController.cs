using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Windows.Base;
using UI.Windows.MainMenu.Hero;

namespace UI.Windows.MainMenu.Game
{
    public class GameWindowController : UIWindowController<GameWindowView>, 
        IInitializeListener, ILoadListener, ISubscriber
    {
        public bool IsInitialized { get; set; }
        
        private GameModel _gameModel;
        private HeroWindowController _heroWindow;
        
        public UniTask Initialize()
        {
            _gameModel = Container.Instance.GetService<GameModel>();    
            
            view.WorldsRadioGroup.Initialize();
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            windowManager.GetWindow<HeroWindowController>().HeroListChanged += _updateObjectLockerState;
            
            view.ButtonContinue.Clicked += _continueGame;
            view.ButtonJoin.Clicked += _openJoinWindow;
            view.TextUserIP.Clicked += _copyIpToBuffer;
            view.WorldsRadioGroup.Selected += _changeSelectedWorld;

        }

        public UniTask GameStart()
        {
            return UniTask.CompletedTask;
        }

        public UniTask GameLoad(GameModel model)
        {
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
            
        }

        private void _updateObjectLockerState()
        {
            view.ObjectLocker.SetActive(_gameModel.Heroes.Count == 0);
        }

        private void _openJoinWindow()
        {
            
        }

        private void _changeSelectedWorld(int worldIndex)
        {
            
        }

        private void _copyIpToBuffer()
        {
            
        }

        private void _updateWorldList()
        {
   
        }
    }
}