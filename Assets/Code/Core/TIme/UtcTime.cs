using System;
using Core.Extensions;
using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using Essential;
using UnityEngine.Networking;

namespace Core.TIme
{
    public class UtcTime : IService, IInitializeListener, ILoadListener, IUpdateListener, IExitListener
    {
        public event Action OnDayStarted;
        public event Action OnNightStarted;

        private static readonly TimeSpan NightStart = new(22, 0, 0);
        private static readonly TimeSpan NightEnd = new(6, 0, 0);
        public bool IsInitialized { get; set; }
        public string RuntimeListenerName => "UserTime";
        public DateTime Current { get; private set; }
        
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
                Log.Info(this, $"[_initCurrentTime] Init google time. Time = {netTime}");
                if (!DateTime.TryParse(netTime, out DateTime current))
                {
                    Current = DateTime.UtcNow;
                    Log.Info(this, $"[_initCurrentTime] Lose google time parsing. Time = {Current}");
                }
                else
                {
                    Current = current;
                }
            }
            else
            {
                Current = DateTime.UtcNow;
                Log.Info(this, $"[_initCurrentTime] Init standalone time. Time = {Current}");
            }

            DateTime lastVisit = gameModel.GameEnterTime;

            gameModel.GameEnterTime = Current;

            _checkTimeOfDay();

            Log.Info(this, $"[_initCurrentTime] End init.\n" +
                           $"Is first visit = {!lastVisit.IsEqualDay(Current)}\n" +
                           $"Current time =  {Current}. Saving time = {lastVisit}");
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

                Log.Info(this, "[_checkTimeOfDay] Start night.");
            }
            else if (!isNightTime && _isNight)
            {
                _isNight = false;

                OnDayStarted?.Invoke();

                Log.Info(this, "[_checkTimeOfDay] Start day.");
            }
        }
    }
}