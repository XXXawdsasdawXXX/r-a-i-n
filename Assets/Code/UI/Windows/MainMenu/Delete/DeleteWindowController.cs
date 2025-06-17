using System;
using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Windows.Base;
using UI.Windows.MainMenu.Delete;
using UnityEngine;

namespace UI.Windows.MainMenu.DeleteHero
{
    public class DeleteWindowController  : UIWindowController<DeleteWindowView>, IInitializeListener
    {
        public event Action PressDeleted;
        public bool IsInitialized { get; set; }
      
        private GameModel _gameModel;

        public UniTask Initialize()
        {
            _gameModel = Container.Instance.GetService<GameModel>();
            
            return UniTask.CompletedTask;
        }

        protected override void subscribeToEvents(bool flag)
        {
            if (flag)
            {
                view.ButtonDelete.Clicked += _invokeDelete;
            }
            else
            {
                view.ButtonDelete.Clicked -= _invokeDelete;
            }
        }

        public void SetObservedObject(string objectName)
        {
            view.TextName.SetText(objectName);
            view.TextName.gameObject.SetActive(!string.IsNullOrEmpty(objectName));
        }
        
        public void SetObservedIcon(Sprite objectSprite)
        {
            view.ImageIcon.SetSprite(objectSprite);
            view.ImageIcon.gameObject.SetActive(objectSprite != null);
        }
        
        private void _invokeDelete()
        {
            PressDeleted?.Invoke();
        }
    }
}