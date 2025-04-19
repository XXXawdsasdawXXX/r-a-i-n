using Core.Audio;
using Core.GameLoop;
using Core.Libraries;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Code.CoreGame.Entities.Hero
{
    public class HeroAnimationEvent : Essential.Mono, IInitializeListener
    {
        public bool IsInitialized { get; set; }
        
        private AudioService _audio;
        
        public UniTask Initialize()
        {
            _audio = Container.Instance.GetService<AudioService>();
            
            return UniTask.CompletedTask;
        }

        private void PlayStep()
        {
            _audio.OneShot(AudioEventLibrary.STEP, transform.position);
        }
    }
}