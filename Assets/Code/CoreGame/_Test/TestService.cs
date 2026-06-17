using Core.GameLoop;
using Core.Input;
using Core.ServiceLocator;
using CoreGame.Grid;
using CoreGame.PlayerCamera;
using Cysharp.Threading.Tasks;
using LiteNetLib.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace CoreGame._Test
{
    [Preserve]
    public class TestService : IMono, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }
        
        private GridService _gridService;
        private InputService _inputService;
        private CameraView _cameraView;
        
        public UniTask Initialize()
        {
            _gridService = Container.Instance.GetService<GridService>();
            _inputService = Container.Instance.GetService<InputService>();
            _cameraView = Container.Instance.GetView<CameraView>();
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _inputService.ActionEnded += InputServiceOnActionEnded;
        }

        public void Unsubscribe()
        {
            _inputService.ActionEnded -= InputServiceOnActionEnded;
        }

        private void InputServiceOnActionEnded(EInputAction obj)
        {
            if (obj is EInputAction.LeftClick)
            {
                Vector3 worldPoint = _cameraView.ScreenToWorldPoint(_inputService.MousePosition);
                
                worldPoint.z = 0;
                
                float2 position = _gridService.GetTilePosition(worldPoint);

                ETileType tileType = _gridService.GetTileType(worldPoint);
                
            }
        }
    }
}