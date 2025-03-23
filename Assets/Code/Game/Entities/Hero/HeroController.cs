using System;
using FishNet.Object;

namespace Game.Entities.Hero
{
    public class HeroController : NetworkBehaviour
    {
        public static event Action<HeroController> StartedAsOwner; 
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                return;
            }
            
            StartedAsOwner?.Invoke(this);
        }
        
    }
}