using System;
using Core.Extensions;
using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Core.Time
{
    public class UtcTime : IService, IInitializeListener, ILoadListener, IUpdateListener, IExitListener
    {
        public bool IsInitialized { get; set; }
        public string RuntimeListenerName => "UserTime";
        public DateTime Current { get; private set; }
        public event Action OnDayStarted;
        public event Action OnNightStarted;

        private static readonly TimeSpan NightStart = new(22, 0, 0);
        private static readonly TimeSpan NightEnd = new(6, 0, 0);
        
        private float _currentDeltaTime;
        private bool _isNight;

        private GameModel _gameModel;


        public UniTask Initialize()
        {
            _gameModel = Container.Instance.GetService<GameModel>();
        
            return UniTask.CompletedTask;
        }

        public async UniTask GameLoad(GameModel model)
        {
            await _checkUtcTime(model);
        }

        public void GameUpdate(float deltaTime)
        {
            _updateUtcTime(deltaTime);
        }
        
        public void GameExit()
        {
            _gameModel.GameExitTime = Current;
        }
        
        public bool IsNightTime()
        {
            TimeSpan timeOfDay = Current.TimeOfDay;

            if (NightStart < NightEnd)
            {
                return timeOfDay >= NightStart && timeOfDay < NightEnd;
            }

            return timeOfDay >= NightStart || timeOfDay < NightEnd;
        }

        private async UniTask _checkUtcTime(GameModel gameModel)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get("https://www.google.com");

            if (webRequest is { result: UnityWebRequest.Result.Success })
            {
                await webRequest.SendWebRequest();

                string netTime = webRequest.GetResponseHeader("date");
                if (!DateTime.TryParse(netTime, out DateTime current))
                {
                    Current = DateTime.UtcNow;
                }
                else
                {
                    Current = current;
                }
            }
            else
            {
                Current = DateTime.UtcNow;
            }

            DateTime lastVisit = gameModel.GameEnterTime;

            gameModel.GameEnterTime = Current;

            _checkTimeOfDay();

        }

        private void _updateUtcTime(float deltaTime)
        {
            Current += TimeSpan.FromSeconds(deltaTime);
        }

        private void _checkTimeOfDay()
        {
            bool isNightTime = IsNightTime();

            if (isNightTime && !_isNight)
            {
                _isNight = true;

                OnNightStarted?.Invoke();

            }
            else if (!isNightTime && _isNight)
            {
                _isNight = false;

                OnDayStarted?.Invoke();

            }
        }
    }
}