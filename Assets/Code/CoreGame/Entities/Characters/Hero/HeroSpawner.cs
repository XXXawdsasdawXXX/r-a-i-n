using System;
using System.Collections.Generic;
using Core.GameLoop;
using Core.Network;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

namespace CoreGame.Entities.Characters.Hero
{
    public class HeroSpawner : NetworkPool
    {
        private readonly Dictionary<NetworkConnection, NetworkObject> _heroes = new();
        

        public override void OnStartServer()
        {
            base.OnStartServer();

            networkManager.SceneManager.OnClientLoadedStartScenes += _sceneManagerOnClientLoadedStartScenes;
            networkManager.ServerManager.OnRemoteConnectionState += _serverManagerOnRemoveConnection;

            _spawnMissingHeroes();
        }

        public override void OnStopServer()
        {
            networkManager.SceneManager.OnClientLoadedStartScenes -= _sceneManagerOnClientLoadedStartScenes;
            networkManager.ServerManager.OnRemoteConnectionState -= _serverManagerOnRemoveConnection;

            base.OnStopServer();
        }

        protected override UniTask onSpawned(NetworkObject instance, NetworkConnection connection)
        {
            if (!IsServerInitialized)
            {
                return UniTask.CompletedTask;
            }

            networkManager.SceneManager.AddOwnerToDefaultScene(instance);

            _heroes[connection] = instance;

            _setUserHero(connection, instance);

            return UniTask.CompletedTask;
        }

        public void RegisterHeroListeners(Hero hero)
        {
            List<IGameListener> listeners = new(hero.GetComponentsInChildren<IGameListener>(true));

            foreach (ICharacterComponent component in hero.Components.Values)
            {
                if (component is IGameListener listener)
                {
                    listeners.Add(listener);
                }
            }

            registerGameListener(listeners.ToArray());
        }

        protected override IGameListener[] getGameListeners(in NetworkObject networkBehaviour)
        {
            Hero hero = networkBehaviour.GetComponent<Hero>();
            List<IGameListener> listeners = new(hero.GetComponentsInChildren<IGameListener>(true));

            foreach (ICharacterComponent component in hero.Components.Values)
            {
                if (component is IGameListener listener)
                {
                    listeners.Add(listener);
                }
            }

            return listeners.ToArray();
        }
        
        private void _sceneManagerOnClientLoadedStartScenes(NetworkConnection connection, bool isServer)
        {
            if (!isServer)
            {
                return;
            }

            _trySpawnHero(connection);
        }

        private void _spawnMissingHeroes()
        {
            if (!networkManager.IsServerStarted || !IsServerInitialized)
            {
                return;
            }

            foreach (NetworkConnection connection in networkManager.ServerManager.Clients.Values)
            {
                _trySpawnHero(connection);
            }

            NetworkConnection localConnection = networkManager.ClientManager.Connection;
            if (localConnection.IsValid)
            {
                _trySpawnHero(localConnection);
            }
        }

        private void _trySpawnHero(NetworkConnection connection)
        {
            if (!IsServerInitialized)
            {
                return;
            }

            if (!connection.IsValid || !connection.IsAuthenticated)
            {
                return;
            }

            if (!connection.LoadedStartScenes(true))
            {
                return;
            }

            if (_heroes.ContainsKey(connection))
            {
                return;
            }

            spawn(Vector3.zero, connection);
        }
        

        [TargetRpc]
        private void _setUserHero(NetworkConnection connection, NetworkObject instance)
        {
            UserProvider userProvider = Container.Instance.GetService<UserProvider>();
            userProvider.SetConnection(connection);
            userProvider.SetHero(instance);
        }

        private void _serverManagerOnRemoveConnection(NetworkConnection connection, RemoteConnectionStateArgs state)
        {
            if (state.ConnectionState != RemoteConnectionState.Stopped)
            {
                return;
            }

            _heroes.Remove(connection);
        }

        public bool TryGetConnectionForHeroObjectId(int heroObjectId, out NetworkConnection connection)
        {
            foreach (KeyValuePair<NetworkConnection, NetworkObject> entry in _heroes)
            {
                if (entry.Value != null && entry.Value.ObjectId == heroObjectId)
                {
                    connection = entry.Key;
                    return true;
                }
            }

            connection = null;
            return false;
        }

        public bool TryGetHeroObjectId(NetworkConnection connection, out int heroObjectId)
        {
            if (_heroes.TryGetValue(connection, out NetworkObject heroObject) && heroObject != null)
            {
                heroObjectId = heroObject.ObjectId;
                return true;
            }

            heroObjectId = 0;
            return false;
        }

        public bool TryGetHero(NetworkConnection connection, out Hero hero)
        {
            hero = null;
            if (!_heroes.TryGetValue(connection, out NetworkObject heroObject) || heroObject == null)
            {
                return false;
            }

            hero = heroObject.GetComponent<Hero>();
            return hero != null;
        }
    }
}