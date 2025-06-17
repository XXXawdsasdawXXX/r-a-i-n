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
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _heroWindow.HeroListChanged += _updateObjectLockerState;
            
            view.TextUserIP.Clicked += _copyIpToBuffer;
            view.TextUserIP.Clicked += _copyIpToBuffer;
            
            view.WorldsRadioGroup.Initialize();
        }

        public UniTask GameStart()
        {
            _heroWindow = windowManager.GetWindow<HeroWindowController>();
            
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
            _heroWindow.HeroListChanged -= _updateObjectLockerState;
        }

        private void _updateObjectLockerState()
        {
            view.ObjectLocker.SetActive(_gameModel.Heroes.Count == 0);
        }

        private void _copyIpToBuffer()
        {
            
        }

        private void _updateWorldList()
        {
   
        }
    }
}