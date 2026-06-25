using UnityEngine;

namespace UI.Windows.Game.Card.Unit.Visual
{
    [CreateAssetMenu(fileName = "UnitVisualProfile_", menuName = "Game/Battle/Unit Visual Profile")]
    public class UnitVisualProfile : ScriptableObject
    {
        public enum EDisplayMode
        {
            StaticSprite = 0,
            Skeletal = 1
        }

        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public EDisplayMode DisplayMode { get; private set; } = EDisplayMode.Skeletal;
        [field: SerializeField] public GameObject SkeletonPrefab { get; private set; }
        [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
        [field: SerializeField] public string ViewBodyChildName { get; private set; } = "body";
        [field: SerializeField] public float StudioCameraSize { get; private set; } = 0.85f;
        [field: SerializeField] public float StudioInstanceScale { get; private set; } = 3f;
        [field: SerializeField] public Vector2 StudioInstanceOffset { get; private set; }
        [field: SerializeField] public float UiDisplayScale { get; private set; } = 2.6f;
        [field: SerializeField] public Sprite StaticSprite { get; private set; }

        public Transform ResolveViewBody(Transform root)
        {
            if (root == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(ViewBodyChildName))
            {
                Transform child = root.Find(ViewBodyChildName);
                if (child != null)
                {
                    return child;
                }
            }

            return root;
        }
    }
}
