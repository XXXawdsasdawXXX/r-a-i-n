using System;
using Core.Data;
using Core.GameLoop;
using Core.ServiceLocator;
using CoreGame.Entities.Characters.Interfaces;
using CoreGame.Entities.Params;
using CoreGame.Harvest;

namespace CoreGame.Entities.Characters.Controllers
{
    public class Miner : IUpdateListener, ICharacterComponent
    {
        public event Action Started;
        public event Action Ended;
        public string RuntimeListenerName => "Mainer";

        public Condition Condition { get; } = new();
        public MinerProcess Process { get; private set; }
        public bool IsMining { get; private set; }

        private readonly ResourceStorage _resourceStorage;
        
        private readonly IHarvestAnimator _animator;
        private readonly Health _health;

        public Miner(IHarvestAnimator animator, Health health)
        {
            _resourceStorage = Container.Instance.GetService<ResourceStorage>();
            
            _animator = animator;
            _health = health;
        }

        public void GameUpdate(float deltaTime)
        {
            if (!IsMining || !Condition.AreMet())
            {
                if (IsMining)
                {
                    StopHarvest();
                }

                return;
            }

            Process.CurrentTime += deltaTime;

            if (Process.CurrentTime >= Process.ResourceSource.Config.HarvestTime)
            {
                _health.UpdateHealth(-Process.ResourceSource.Config.HealthPrice);

                Process.ResourceSource.UpdateValue(-Process.ResourceSource.Config.ResourcePerTick);

                _resourceStorage.Add(Process.ResourceSource.Type, Process.ResourceSource.Config.ResourcePerTick);
                
                if (Process.ResourceSource.IsEnded)
                {
                    StopHarvest();
                }
                else
                {
                    Process.CurrentTime = 0;
                }
            }
        }

        public void StartHarvest(ResourceSource resourceSource)
        {
            Process = new MinerProcess
            {
                ResourceSource = resourceSource,
                CurrentTime = 0,
            };

            _animator.StartHarvest();

            IsMining = true;

            Started?.Invoke();
        }

        public void StopHarvest()
        {
            Process = null;

            _animator.StopHarvest();

            IsMining = false;

            Ended?.Invoke();
        }
    }

    public class MinerProcess
    {
        public ResourceSource ResourceSource;
        public float CurrentTime;
    }
}