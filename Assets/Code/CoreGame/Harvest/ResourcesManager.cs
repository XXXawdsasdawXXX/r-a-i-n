using Core.Extensions;
using Core.GameLoop;
using Core.Network;
using Core.Save;
using Core.ServiceLocator;
using CoreGame.Entities.Characters.Controllers;
using CoreGame.Entities.Characters.Hero;
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace CoreGame.Harvest
{
    public class ResourcesManager : Essential.Mono, IService, IInitializeListener, ILoadListener, ISubscriber
    {
        public bool IsInitialized { get; set; }

        [SerializeField] private ResourceCollection _resourceCollection;
        [SerializeField] private Resource[] _resources;

        private UserProvider _userProvider;
        private Hero _hero;
        private Mainer _mainer;
        private GameModel _gameModel;

        
        public UniTask Initialize()
        {
            _userProvider = Container.Instance.GetService<UserProvider>();
            _gameModel = Container.Instance.GetService<GameModel>();

            return UniTask.CompletedTask;
        }

        public UniTask GameLoad(GameModel model)
        {
            foreach (Resource resource in _resources)
            {
                string resourcePosition = resource.Position.AsString();
                if (model.World.SceneResources.ContainsKey(resourcePosition))
                {
                    resource.SetValue(model.World.SceneResources[resourcePosition]);
                }
                else
                {
                    model.World.SceneResources.Add(resourcePosition, resource.CurrentValue);
                }
            }
            
            return UniTask.CompletedTask;
        }
        
        public void Subscribe()
        {
            _userProvider.HeroCreated += _onHeroCreated;

            foreach (Resource resource in _resources)
            {
                resource.HarvestStarted += _onHarvestStarted;
                resource.Changed += _onChangedResource;
            }
        }

        public void Unsubscribe()
        {
            _userProvider.HeroCreated -= _onHeroCreated;

            foreach (Resource resource in _resources)
            {
                resource.HarvestStarted -= _onHarvestStarted;
                resource.Changed -= _onChangedResource;
            }
        }

        private void _onChangedResource(Resource resource)
        {
            _gameModel.World.SceneResources[resource.Position.AsString()] = resource.CurrentValue;
        }

        private void _onHeroCreated()
        {
            _hero = _userProvider.GetHeroComponent<Hero>();
            _mainer = _hero.GetCharacterComponent<Mainer>();
        }

        private void _onHarvestStarted(Resource resource)
        {
            if (!_mainer.IsMining)
            {
                _mainer.StartHarvest(resource);
            }
        }

#if UNITY_EDITOR

        [Button]
        private void _validateResources()
        {
            foreach (Resource resource in _resources)
            {
                resource.Validate(_resourceCollection.Library.Get(resource.Type));
                EditorUtility.SetDirty(resource);
            }

            AssetDatabase.SaveAssets();
        }

        [Button]
        private void _findResources()
        {
            _resources = GetComponentsInChildren<Resource>(true);
        }
#endif
 
    }
}