using Core.Data;
using Core.GameLoop;
using Core.ServiceLocator;
using CoreGame.Time;
using Cysharp.Threading.Tasks;
using UI.Windows.Base;

namespace UI.Windows.HUD
{
    public class HUDGameTimeController : UIWindowController<HUDGameTimeView>,
        IInitializeListener,
        IStartListener,
        IUpdateListener
    {
        public bool IsInitialized { get; set; }
        public string RuntimeListenerName => "HUDWindowController";
        
        private GameTime _gameTime;
        private Cache<int> _lastUpdateMinute;
        private float _currentValue;

        public UniTask Initialize()
        {
            _gameTime = Container.Instance.GetService<GameTime>();

            _lastUpdateMinute = new Cache<int>();

            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            Open();

            return UniTask.CompletedTask;
        }

        public void GameUpdate(float deltaTime)
        {
            if (_lastUpdateMinute.Update(_gameTime.Current.Hours * 60 + _gameTime.Current.Minutes))
            {
                float gameTimeNormalize = _lastUpdateMinute.Value / 1440f;
             
                view.ImageGameTime.SetFillAmount(gameTimeNormalize);
            }
        }
    }
}