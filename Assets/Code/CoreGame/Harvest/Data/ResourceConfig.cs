using UnityEngine;

namespace Code.CoreGame.Harvest
{
    [CreateAssetMenu(fileName = "Resource_", menuName = "Game/Resource/Resource")]
    public class ResourceConfig : ScriptableObject
    {
        [field: SerializeField] public EResource Type { get; private set; }
        
        [field: SerializeField] public Sprite Image  { get; private set; }
        
        [field: SerializeField] public int MaxValue { get; private set; }
    }
}