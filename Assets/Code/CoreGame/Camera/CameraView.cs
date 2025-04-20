using Core.ServiceLocator;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.CoreGame.Camera
{
    [Preserve]
    public class CameraView : MonoView
    {
        [field: SerializeField] public UnityEngine.Camera Camera { get; private set; }

        public Vector3 ScreenToWorldPoint(Vector3 screenPosition)
        {
            return Camera != null ? Camera.ScreenToWorldPoint(screenPosition) : Vector3.zero;
        }
    }
}