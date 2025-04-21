using Core.GameLoop;
using Core.Interfaces;
using Core.Network;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using Essential;
using UnityEngine;

namespace Code.CoreGame.Camera
{
    public class CameraController : IMono, IInitializeListener, IStartListener, IUpdateListener
    {
        public bool IsInitialized { get; set; }
        public string RuntimeListenerName => "CameraController";
        
        private CameraView _cameraView;
        private UserProvider _userProvider;
        
        public UniTask Initialize()
        {
            _userProvider = Container.Instance.GetService<UserProvider>();
            
            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            _cameraView = Container.Instance.GetView<CameraView>();
         
            Log.Info(this, $"start. camera view = {_cameraView == null}" , Log.Orange);
            
            return UniTask.CompletedTask;
        }

        public void GameUpdate(float deltaTime)
        {
            if (_cameraView != null && _userProvider.Hero != null)
            {
                Vector3 position = _userProvider.Hero.transform.position;

                position.z = -10;
                
                _cameraView.transform.position = position;
                
                Log.Info(this, $"user id {_userProvider.Connection.ClientId}.", Log.Orange);
            }
            else
            {
                Log.Info(this, $"camera is null {_cameraView.Camera == null}. " +
                               $"user is null {_userProvider.Hero == null}.", Log.Orange);
            }
        }
    }
}