using FishNet.Object;
using UnityEngine;

namespace Code.CoreGame.Entities.Hero
{
    public class HeroStateObserver : NetworkBehaviour
    {
        [field: SerializeField] public EHeroState State { get; private set; }
        
        
    }
}