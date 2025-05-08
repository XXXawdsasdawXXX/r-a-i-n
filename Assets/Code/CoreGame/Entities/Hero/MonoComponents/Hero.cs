using Core.Network;
using Core.ServiceLocator;
using FishNet;
using FishNet.Object;
using UnityEngine;

namespace Code.CoreGame.Entities.Hero
{
    public class Hero: NetworkBehaviour
    {
        public override void OnStartClient()
        {
            if (IsOwner)
            {
                UserProvider userProvider = Container.Instance.GetService<UserProvider>();
                userProvider.SetConnection(InstanceFinder.ClientManager.Connection);
                userProvider.SetHero(GetComponent<NetworkObject>());

                Debug.Log($"[HeroClientTracker] Set local hero: {gameObject.name}");
            }
        }
    }
}