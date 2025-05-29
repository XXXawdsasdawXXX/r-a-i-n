using Core.AssetManagement;
using Core.GameLoop;
using Core.Libraries.Assets;
using Core.Libraries.Configs;
using Core.Libraries.Installers;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using Essential;
using UnityEngine;

namespace Core.StateMachine
{
    public class BootstrapState : IState
    {
        public bool IsInitialized { get; set; }

        private readonly GameStateMachine _gameStateMachine;

        public BootstrapState(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public async UniTask Initialize()
        {
            InstallerLibrary installerLibrary = await AssetProvider
                .LoadScriptableObject<InstallerLibrary>(AssetKey.INSTALLER_LIBRARY_PATH);
            ConfigLibrary configLibrary = await AssetProvider
                .LoadScriptableObject<ConfigLibrary>(AssetKey.CONFIG_LIBRARY_PATH);

            AssetLibrary assetLibrary = configLibrary.Get<AssetLibrary>();

            if (Log.PROFILER_IS_ACTIVE)
            {
                AssetProvider.Instantiate(assetLibrary.Windows.Get(AssetKey.CANVAS_PROFILER));
            }

            ContextEntities projectContext = ContextBuilder.BuildContext(installerLibrary.ProjectsInstaller.GetTypes());
            projectContext.Services.Add(typeof(GameStateMachine), _gameStateMachine);
            Container container = new(projectContext);

            container.AddConfig(installerLibrary);
            foreach (ScriptableObject config in configLibrary.Configs)
            {
                container.AddConfig(config);
            }

            SaveService saveService = container.GetService<SaveService>();
            GameModel model = container.GetService<GameModel>();
            GameModel loadedModel = saveService.LoadLast<GameModel>() ?? new GameModel(); 
            
            model.CopyFrom(loadedModel);

            container.GetService<GameEventDispatcher>().Initialize();
        }

        public UniTask Enter()
        {
            _gameStateMachine.SwitchState(typeof(MainMenuState));

            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}