using System;
using Core.Libraries;
using UnityEngine;

namespace CoreGame.Harvest
{
    [CreateAssetMenu(fileName = "Library_Resource", menuName = "Game/Resource/ResourcesLibrary")]
    public class ResourceCollection : ScriptableObject
    {
        [Serializable]
        public class ResourcesLibrary : Library<EResource, ResourceConfig>
        {
            protected override bool ThisIs(ResourceConfig value, EResource key)
            {
                return value.Type == key;
            }
        }
        
        [field: SerializeField] public ResourcesLibrary Library { get; private set;}
    }
}