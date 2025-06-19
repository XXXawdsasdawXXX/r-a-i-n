using Core.GameLoop;
using CoreGame.Entities.Params;
using Cysharp.Threading.Tasks;
using Essential;
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
            
            Log.Info($"game start set name {_personName.Name}");
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _personName.Changed += SetName;
        }

        public void Unsubscribe()
        {
            _personName.Changed -= SetName;
        }

        private void SetName(string objectName)
        {
            Log.Info($"3", this);
            
            _uiText.SetText(objectName);
        }
    }
}