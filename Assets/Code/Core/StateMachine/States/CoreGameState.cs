using Core.AssetManagement;
using Core.Libraries.Assets;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine
{
    public class CoreGameState : IState
    {
        public bool IsInitialized { get;set; }

        private GameObject _coreGameCanvasPrefab;
        private GameObject _coreGameCanvasInstance;
        
        public UniTask Initialize()
        {
            _coreGameCanvasPrefab = Container.Instance.GetConfig<AssetLibrary>().Window.Get(AssetKey.CANVAS_CORE_GAME);

            return UniTask.CompletedTask;
        }

        public UniTask Enter()
        {
            _coreGameCanvasInstance = AssetProvider.Instantiate(_coreGameCanvasPrefab);
            
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            Object.Destroy(_coreGameCanvasInstance.gameObject);
            
            return UniTask.CompletedTask;
        }
    }
}