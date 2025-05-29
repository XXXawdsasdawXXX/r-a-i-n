using Core.GameLoop;
using CoreGame.Harvest;
using UI.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Game
{
    public class UIResourceValueText : Essential.Mono, ISubscriber
    {
        [FormerlySerializedAs("_resource")] [SerializeField] private ResourceSource _resourceSource;
        [SerializeField] private UIText _text;

        public void Subscribe()
        {
            _resourceSource.Changed += _onChanged;
        }

        public void Unsubscribe()
        {
            _resourceSource.Changed -= _onChanged;
        }

        private void _onChanged()
        {
            _text.SetText($"{_resourceSource.Current}/{_resourceSource.Config.MaxValue}");
        }
    }
}