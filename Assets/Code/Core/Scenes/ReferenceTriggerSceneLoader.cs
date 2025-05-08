using System;
using System.Collections.Generic;
using Core.Network;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using Essential;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEditor.SearchService;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Core.Scenes
{
    public class ReferenceTriggerSceneLoader : NetworkBehaviour
    {
        public event Action<NetworkObject, EScene> MovedToAnotherScene;
        
        [Tooltip("Scenes to load.")] [SerializeField]
        private EScene _scene;
        
        private Dictionary<NetworkConnection, float> _triggeredTimes = new();

        [SerializeField] private int _stackedSceneHandle;
        [SerializeField] private bool _sceneStack;

        protected override void Start()
        {
   
        }

        private void OnDisable()
        {
            if (InstanceFinder.SceneManager != null)
            {

            }
        }

        private void SceneManagerOnOnLoadEnd(SceneLoadEndEventArgs obj)
        {
                        
            InstanceFinder.SceneManager.OnLoadEnd -= SceneManagerOnOnLoadEnd;
            if (InstanceFinder.IsServerStarted)
            {
                
                var loadedSceneName = obj.LoadedScenes.Length > 0 ? obj.LoadedScenes[0].name : obj.SkippedSceneNames[0]; 
                
                foreach (KeyValuePair<NetworkConnection, NetworkObject> connection in Container.Instance.GetService<HeroPool>().Heroes)
                {
                    if (connection.Key.IsHost)
                    {
                        Log.Info(this, $"uhooooo", Color.black );

                        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(loadedSceneName);
                        GameObject[] objects = scene.GetRootGameObjects();

                        foreach (GameObject variable in objects)
                        {
                            variable.SetActive(loadedSceneName == connection.Value.gameObject.scene.name);
                        }
                        
                        break;
                    }
                }
            }
            
            /*
            if (!obj.QueueData.AsServer)
            {
                return;
            }*/

            if (_sceneStack)
            {
                return;
            }

            if (_stackedSceneHandle != 0)
            {
                return;
            }

            if (obj.LoadedScenes.Length > 0)
            {
                _stackedSceneHandle = obj.LoadedScenes[0].handle;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            NetworkObject networkObject = col.GetComponent<NetworkObject>();

            Log.Info(this, $"{col.gameObject.name} trigger enter {InstanceFinder.NetworkManager.ServerManager.Started}", Color.cyan);
            
            if (networkObject == null)
            {
                return;
            }

            if (_triggeredTimes.TryGetValue(networkObject.Owner, out float time))
            {
                if (Time.time - time < 1f)
                {
                    Log.Info(this, $"{col.gameObject.name} trigger cooldown", Color.cyan);
                    return;
                }
            }

            InstanceFinder.SceneManager.OnLoadEnd += SceneManagerOnOnLoadEnd;

            
            _triggeredTimes[networkObject.Owner] = Time.time;


            //Container.Instance.GetService<SceneService>().LoadSceneAsync(_scene).Forget();
            LoadScene(networkObject);
            
            MovedToAnotherScene?.Invoke(networkObject, _scene);
        }
        

       [ServerRpc(RequireOwnership = false)]
        private void LoadScene(NetworkObject triggeringIdentity)
        {
            Log.Info(this, $"{triggeringIdentity.Owner.ClientId} try load scene", Color.cyan);

            if (!InstanceFinder.NetworkManager.IsServerStarted)
            {
                Log.Info(this, $"{triggeringIdentity.Owner.ClientId} !InstanceFinder.NetworkManager.IsServerStarted",
                    Color.cyan);

                return;
            }

            LoadOptions loadOptions = new()
            {
                AutomaticallyUnload = true, // Принудительно включаем, даже если флаг выключен в инспекторе
            };

            // Создание и настройка сцены
            SceneLoadData sceneLoadData = new(_scene.ToString());
            sceneLoadData.PreferredActiveScene = new(sceneLoadData.SceneLookupDatas[0]);
            sceneLoadData.ReplaceScenes = ReplaceOption.All; // Было OnlineOnly
            sceneLoadData.Options = new LoadOptions { AutomaticallyUnload = true };
            sceneLoadData.MovedNetworkObjects = new NetworkObject[] { triggeringIdentity };
            

            Log.Info(this,
                $"[SceneLoader] Loading scene: {_scene} with ReplaceOption: {sceneLoadData.ReplaceScenes}", Color.cyan);

            InstanceFinder.SceneManager.OnLoadEnd += SceneManagerOnOnLoadEnd;
            
            InstanceFinder.SceneManager.LoadConnectionScenes(triggeringIdentity.Owner, sceneLoadData);

        }

        /*private void LoadScene(NetworkObject triggeringIdentity)
        {
            if (!InstanceFinder.NetworkManager.IsServerStarted)
            {
                return;
            }
            
            List<NetworkObject> movedObjects = new();

            if (_moveAllObjects)
            {
                foreach (NetworkConnection item in InstanceFinder.ServerManager.Clients.Values)
                {
                    foreach (NetworkObject nob in item.Objects)
                        movedObjects.Add(nob);
                }
            }
            else if (_moveObject)
            {
                movedObjects.Add(triggeringIdentity);
            }

            LoadOptions loadOptions = new()
            {
                AutomaticallyUnload = _automaticallyUnload,
            };

            //Make scene data.
            SceneLoadData sceneLoadData = new(_scene.ToString());
            sceneLoadData.PreferredActiveScene = new(sceneLoadData.SceneLookupDatas[0]);
            sceneLoadData.ReplaceScenes = _replaceOption;
            sceneLoadData.Options = loadOptions;
            sceneLoadData.MovedNetworkObjects = movedObjects.ToArray();

            //Load for connection only.
            if (_connectionOnly)
            {
                InstanceFinder.SceneManager.LoadConnectionScenes(triggeringIdentity.Owner, sceneLoadData);
            }
            //Load for all clients.
            else
            {
                InstanceFinder.SceneManager.LoadGlobalScenes(sceneLoadData);
            }
        }*/
    }
}