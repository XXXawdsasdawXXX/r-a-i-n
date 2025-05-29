using Core.ServiceLocator;
using Essential;
using UnityEngine;
using UnityEngine.Scripting;

namespace CoreGame.Camera
{
    [Preserve]
    public class CameraView : MonoView
    {
        [field: SerializeField] public UnityEngine.Camera Camera { get; private set; }

        private void OnEnable()
        {
            Log.Info(this, $"OnEnable", Log.Orange);
        }

        public Vector3 ScreenToWorldPoint(Vector3 screenPosition)
        {
            return Camera != null ? Camera.ScreenToWorldPoint(screenPosition) : Vector3.zero;
        }
    }
}