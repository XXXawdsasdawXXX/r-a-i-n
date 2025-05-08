using UnityEngine;

namespace Code.CoreGame.Harvest
{
    [CreateAssetMenu(fileName = "Resource_", menuName = "Game/Resource/CommonResources")]
    public class CommonResourceConfig : ScriptableObject
    {
        [field: SerializeField] public ResourcesLibrary Library { get; private set;}
    }
}