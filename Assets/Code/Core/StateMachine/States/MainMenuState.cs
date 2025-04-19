using Core.AssetManagement;
using Core.GameLoop;
using Core.Libraries.Assets;
using Core.Scenes;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine
{
    public class MainMenuState : IState
    {
        public bool IsInitialized { get; set; }

        private GameEventDispatcher _gameEventDispatcher;
        private SceneService _sceneService;
        
        private GameObject _mainMenuCanvasPrefab;

        public UniTask Initialize()
        {
            _gameEventDispatcher = Container.Instance.GetService<GameEventDispatcher>();
            
            _sceneService = Container.Instance.GetService<SceneService>();

            _mainMenuCanvasPrefab = Container.Instance.GetConfig<AssetLibrary>().Window.Get(AssetKey.CANVAS_MAIN_MENU);
            
            return UniTask.CompletedTask;
        }

        public async UniTask Enter()
        {
            _gameEventDispatcher.Dispose();
            
            await _sceneService.LoadSceneAsync(EScene.Menu);

            AssetProvider.Instantiate(_mainMenuCanvasPrefab);
            
            Container.Instance.Context.SetChildContext(ContextBuilder.BuildContext());
            
            _gameEventDispatcher.Register(Container.Instance.GetGameListeners());
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}