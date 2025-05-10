using Core.Network;
using FishNet.Object;

namespace Code.CoreGame.Entities.Characters.Hero
{
    public class HeroItemController :  NetworkBehaviour
    {
        private NetworkPool _networkItemPool;
        
        public override void OnStartClient()
        {
            /*base.OnStartClient();
            
            enabled = IsOwner;

            _networkItemPool = Container.Instance.GetService<NetworkPool>();
            
            Debug.Log($"spawn hero {_networkItemPool != null}");*/
        }

        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.F1))
            {
                _networkItemPool.Spawn(transform.position + Vector3.right);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                _networkItemPool.Despawn();
            }*/
        }
    }
}