using System.Collections.Generic;
using Core.Scenes;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using Essential;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using Plugins.FishNet.Runtime.Managing.Object;
using UnityEngine;
using UnityEngine.Scripting;

namespace Core.GameLoop
{
    [Preserve]
    internal sealed class MonoSpawnTracker : IService, IStartListener, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }

        private ReferenceTriggerSceneLoader[] _triggers;

        private GameEventDispatcher _gameEventDispatcher;
        private PlayerSpawner _playerSpawner;

        private readonly HashSet<Essential.Mono> _observeMono = new();

        public UniTask Initialize()
        {
            _gameEventDispatcher = Container.Instance.GetService<GameEventDispatcher>();

            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            _triggers = GameObject.FindObjectsOfType<ReferenceTriggerSceneLoader>();

            return UniTask.CompletedTask;
        }

        public UniTask Subscribe()
        {
            Essential.Mono.Started += _onMonoStarted;
            Essential.Mono.Destroyed += _onMonoDestroyed;

            if (_triggers != null)
            {
                foreach (ReferenceTriggerSceneLoader trigger in _triggers)
                {
                    trigger.MovedToAnotherScene += TriggerOnMovedToAnotherScene;
                }
            }
            /*InstanceFinder.ServerManager.OnRemoteConnectionState += _onRemoteConnectionState;
            InstanceFinder.ClientManager.OnRemoteConnectionState += _onRemoteConnectionState;*/

            InstanceFinder.ServerManager.OnSpawn += _onMonoStarted;
            InstanceFinder.ServerManager.OnDespawn += _onMonoDestroyed;

            return UniTask.CompletedTask;
        }


        public void Unsubscribe()
        {
            Essential.Mono.Started -= _onMonoStarted;
            Essential.Mono.Destroyed -= _onMonoDestroyed;

            InstanceFinder.ServerManager.OnSpawn -= _onMonoStarted;
            InstanceFinder.ServerManager.OnDespawn -= _onMonoDestroyed;


            if (_triggers != null)
            {
                foreach (ReferenceTriggerSceneLoader trigger in _triggers)
                {
                    trigger.MovedToAnotherScene -= TriggerOnMovedToAnotherScene;
                }
            }

            /*InstanceFinder.ServerManager.OnRemoteConnectionState -= _onRemoteConnectionState;
            InstanceFinder.ClientManager.OnRemoteConnectionState -= _onRemoteConnectionState;*/
        }

        private void TriggerOnMovedToAnotherScene(NetworkObject arg1, EScene arg2)
        {
            IGameListener[] listeners = arg1.GetComponentsInChildren<IGameListener>(true);

            foreach (IGameListener listener in listeners)
            {
                _gameEventDispatcher.RemoveSpawnableListener(listener);
            }
        }

        private void _onRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
        {
            //connection.OnObjectRemoved += _onMonoDestroyed;

            if (args.ConnectionState is RemoteConnectionState.Stopped)
            {
                Log.Info(
                    $"_onRemoteConnectionState {args.ConnectionId} {args.ConnectionState} {connection.Objects.Count}");
                foreach (NetworkObject connectionObject in connection.Objects)
                {
                    _onMonoDestroyed(connectionObject);
                }
            }
        }

        private void _onMonoStarted(Essential.Mono obj)
        {
            Log.Info($"Mono started {obj.GetType().Name}", this);

            if (obj is IGameListener gameListener && _observeMono.Add(obj))
            {
                Log.Info($"Mono started {obj.GetType().Name} add to collection", this);
                _gameEventDispatcher.AddSpawnableListener(gameListener);
            }
        }

        private void _onMonoDestroyed(Essential.Mono obj)
        {
            if (obj is IGameListener gameListener && _observeMono.Remove(obj))
            {
                _gameEventDispatcher.RemoveSpawnableListener(gameListener);
            }
        }
    }
}