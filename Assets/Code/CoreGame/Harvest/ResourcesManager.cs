using Core.GameLoop;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Code.CoreGame.Harvest
{
    public class ResourcesManager : IService, IInitializeListener
    {
        public bool IsInitialized { get; set; }
        
        private ResourcePool _resourcePool;
        
        public UniTask Initialize()
        {
            _resourcePool = Container.Instance.GetService<ResourcePool>();
            
            return UniTask.CompletedTask;
        }
    }
}