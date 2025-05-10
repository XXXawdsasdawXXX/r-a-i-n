using UnityEngine;

namespace Core.Libraries.Configs
{
    [CreateAssetMenu(fileName = "Library_Config", menuName = "Game/Library/Config")]
    public class ConfigLibrary : ScriptableObject
    {
        [field: SerializeField] public ScriptableObject[] Configs { get; private set; }

        public T Get<T>() where T : ScriptableObject
        {
            foreach (ScriptableObject config in Configs)
            {
                if (config is T typedConfig)
                {
                    return typedConfig;
                }
            }

            return null;
        }
    }
}