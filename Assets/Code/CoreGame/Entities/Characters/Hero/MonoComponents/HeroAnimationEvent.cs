using Core.Audio;
using Core.GameLoop;
using Core.Libraries;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CoreGame.Entities.Characters.Hero
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
            //_audio ??= Container.Instance.GetService<AudioService>();
            _audio.OneShot(AudioEventLibrary.STEP, transform.position);
        }
    }
}