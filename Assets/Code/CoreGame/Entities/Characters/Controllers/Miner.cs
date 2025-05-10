using System;
using Code.CoreGame.Harvest;
using Core.GameLoop;
using Core.Network;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Code.CoreGame.Entities.Characters.Controllers
{
    public class Miner : IUpdateListener
    {
        public event Action Started;
        public event Action Ended;
        public string RuntimeListenerName => "Mainer";
        public bool IsInitialized { get; set; }

        public MinerProcess Process { get; private set; }

        private UserProvider _userProvider;
        private Hero.Hero _hero;

        
        public void GameUpdate(float deltaTime)
        {
            if (Process == null || _hero == null)
            {
                return;
            }

            Process.CurrentTime += deltaTime;

            if (Process.CurrentTime >= Process.MaxTime)
            {
                _hero.Health.UpdateHealth(Process.Health);
                
                Process.CurrentTime = 0;
            }
        }

        public void StartHarvest(Resource resource)
        {
            Process = new MinerProcess
            {
                MaxTime = 5,
                CurrentTime = 0,
                Health = -3
            };
            
            _hero.Animation.StartHarvest();

            Started?.Invoke();
        }

        public void StopHarvest()
        {
            Process = null;
            
            _hero.Animation.StopHarvest();

            Ended?.Invoke();
        }
    }

    public class MinerProcess
    {
        public float MaxTime;
        public float CurrentTime;
        public int Health;
    }
}