using System;
using System.Collections.Generic;
using Core.ServiceLocator;
using Essential;
using FishNet;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace Core.Network
{
    public abstract class NetworkPool : NetworkBehaviour, IService
    {
        public event Action<NetworkObject> Spawned; 
        public event Action<NetworkObject> Despawned; 

        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private NetworkObject _prefab;

        private readonly List<NetworkObject> _instances = new();

        
        [ServerRpc(RequireOwnership = false)]
        public void Spawn(Vector3 position)
        {
            
            if ( !InstanceFinder.NetworkManager.IsServerStarted)
            {
                Log.Warning("NetworkManager is not active or server is not started.");
                return;
            }
            

            NetworkObject instance = InstanceFinder.NetworkManager.GetPooledInstantiated(_prefab, transform, true);

            if (instance == null)
            {
                Log.Warning("Failed to get pooled instance of item.");
                return;
            }

            instance.transform.position = position;
            
            InstanceFinder.NetworkManager.ServerManager.Spawn(instance);
            
            _instances.Add(instance);
            
            OnSpawned(instance);
            
            Spawned?.Invoke(instance);
        }

        [ServerRpc(RequireOwnership = false)]
        public void Despawn()
        {
            if (_instances.Count > 0)
            {
                NetworkObject lastInstance = _instances[^1];
                
                InstanceFinder.NetworkManager.ServerManager.Despawn(lastInstance);
                
                _instances.Remove(lastInstance);
                
                OnDespawned(lastInstance);
                
                Despawned?.Invoke(lastInstance);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void Despawn(NetworkObject instance)
        {
            if (_instances.Contains(instance))
            {
                InstanceFinder.NetworkManager.ServerManager.Despawn(instance);

                _instances.Remove(instance);
                
                OnDespawned(instance);
                
                Despawned?.Invoke(instance);
            }
            else
            {
                Log.Error(this, $"Pool has not object {instance.name}");
            }
        }

        protected virtual void OnSpawned(NetworkObject instance)
        {
            
        }

        protected virtual void OnDespawned(NetworkObject instance)
        {
            
        }
    }
}