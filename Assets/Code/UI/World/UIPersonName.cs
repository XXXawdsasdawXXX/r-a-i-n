using Core.GameLoop;
using CoreGame.Entities.Params;
using Cysharp.Threading.Tasks;
using UI.Components;
using UnityEngine;

namespace UI.World
{
    public class UIPersonName : Essential.Mono, IStartListener ,ISubscriber
    {
        [SerializeField] private PersonName _personName;
        [SerializeField] private UIText _uiText;

        public UniTask GameStart()
        {
            _uiText.SetText(_personName.Name);
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _personName.Changed += _setName;
        }

        public void Unsubscribe()
        {
            _personName.Changed -= _setName;
        }

        private void _setName(string objectName)
        {
            _uiText.SetText(objectName);
        }
    }
}