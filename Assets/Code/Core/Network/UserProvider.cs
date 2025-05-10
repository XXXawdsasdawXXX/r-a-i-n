using System.Runtime.Serialization;
using Core.ServiceLocator;
using FishNet.Connection;
using FishNet.Object;
using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace Core.Network
{
    public class UserProvider : IService
    {
        public event Action HeroSetted; 
        public NetworkConnection Connection { get; private set; }
        public NetworkObject Hero { get; private set; }
        
        public void SetConnection(NetworkConnection connection)
        {
            Connection = connection;
        }

        public void SetHero(NetworkObject hero)
        {
            Hero = hero;
            
            HeroSetted?.Invoke();
        }
    }
}