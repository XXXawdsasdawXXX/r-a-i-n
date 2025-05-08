using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using Essential;
using Core.GameLoop;
using Core.ServiceLocator;
using FishNet;

namespace Core.Network
{
    public class HeroPool : NetworkBehaviour, IService, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }
        public event Action<GameObject> HeroSpawned;
        public event Action<GameObject> PlayerHeroSpawned;

        public readonly Dictionary<NetworkConnection, NetworkObject> Heroes = new();
        private readonly Color _logColor = new(0.3f, 0.8f, 0.2f);

        [SerializeField] private NetworkObject _heroPrefab;

        private NetworkManager _networkManager;
        private UserProvider _userProvider;


        public UniTask Initialize()
        {
            _networkManager = InstanceFinder.NetworkManager;
            _userProvider = Container.Instance.GetService<UserProvider>();

            return UniTask.CompletedTask;
        }
        
        public void Subscribe()
        {
            _networkManager.SceneManager.OnClientLoadedStartScenes += _sceneManagerOnClientLoadedStartScenes;
            _networkManager.ServerManager.OnRemoteConnectionState += _serverManagerOnRemoveConnection;
        }

        public void Unsubscribe()
        {
            _networkManager.SceneManager.OnClientLoadedStartScenes -= _sceneManagerOnClientLoadedStartScenes;
            _networkManager.ServerManager.OnRemoteConnectionState -= _serverManagerOnRemoveConnection;
        }

        private void _sceneManagerOnClientLoadedStartScenes(NetworkConnection connection, bool isServer)
        {
            if (Heroes.ContainsKey(connection))
            {
                Log.Info(this, $"hero already exist", Log.Blue);
                return;
            }
            
            NetworkObject pooledInstantiated = _networkManager.GetPooledInstantiated(
                _heroPrefab,
                Vector3.zero,
                Quaternion.identity,
                true);

            _networkManager.ServerManager.Spawn(pooledInstantiated, connection);
            _networkManager.SceneManager.AddOwnerToDefaultScene(pooledInstantiated);

            Log.Info($"on client start scene -> " +
                     $"PrefabId: {_heroPrefab.PrefabId}, " +
                     $"CollectionId: {_heroPrefab.SpawnableCollectionId}", _logColor, this);

            Heroes[connection] = pooledInstantiated;
            
            Log.Info(this, $"is owner {pooledInstantiated.gameObject.name} is owner {pooledInstantiated.IsOwner}", Log.Blue);
            
            /*
            if (connection == InstanceFinder.ClientManager.Connection)
            {
                Log.Info(this, $"set hero", Log.Blue);
                _userProvider.SetConnection(connection);
                _userProvider.SetHero(pooledInstantiated);
            }
            */
            
            HeroSpawned?.Invoke(pooledInstantiated.gameObject);

            TargetHeroSpawned(connection, pooledInstantiated.gameObject);
        }

        
        [TargetRpc]
        private void TargetHeroSpawned(NetworkConnection connection, GameObject hero)
        {
            PlayerHeroSpawned?.Invoke(hero);
        }

        private void _serverManagerOnRemoveConnection(NetworkConnection connection, RemoteConnectionStateArgs state)
        {
            Log.Info($"on remove connection {Heroes?.ContainsKey(connection)} {state.ConnectionState}" +
                     $"\n nc id {connection.ClientId} state {state.ConnectionId}", _logColor, this);
        }

     
    }
}