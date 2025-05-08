using System;
using System.Linq;
using Core.Data;
using Core.Extensions;
using Core.Network;
using Essential;
using FishNet.Object;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Code.CoreGame.Harvest
{
    public class ResourcePool : NetworkPool
    {
        [field: SerializeField] public Resource[] ResourcePoints { get; private set; }
        
        [SerializeField] private ResourcesLibrary _resourcesLibrary;
        
        protected override void OnSpawned(NetworkObject instance)
        {
            var position = instance.transform.position.AsFloat2();
            
            Resource point = ResourcePoints.First(p => p.Position.Equals(position));
         
            
            Log.Info(this, "on spawned");
        }


#if UNITY_EDITOR

        [Space, Header("EDITOR")]
        [SerializeField] private Transform _pointsRoot;

        [FormerlySerializedAs("_pointPrefab")] [SerializeField] private Resource _prefab;

        [Button]
        private void CreateNewResource(EResource type)
        {
            Resource[] newArray = new Resource[ResourcePoints.Length + 1];

            for (int i = 0; i < ResourcePoints.Length; i++)
            {
                newArray[i] = ResourcePoints[i];
            }
            
            newArray[^1] = PrefabUtility.InstantiatePrefab(_prefab, _pointsRoot) as Resource;

            newArray[^1].Validate(_resourcesLibrary.Get(type));
            
            ResourcePoints = newArray;
        }

        [Button]
        private void UpdatePointsPosition()
        {
            foreach (Resource point in ResourcePoints)
            {
                point.Validate(_resourcesLibrary.Get(point.Type));
                
                point.name = FormattableString.Invariant(
                    $"point_resource_{point.Type,-15}[{point.Position.x,6:0.00}:{point.Position.y,6:0.00}]");
            }
        }
#endif
        
    }
}