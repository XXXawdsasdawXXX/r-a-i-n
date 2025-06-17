using System;
using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Windows.Base;

namespace UI.Windows.MainMenu.DeleteHero
{
    public class DeleteHeroWindowController  : UIWindowController<DeleteHeroWindowView>, IInitializeListener
    {
        public event Action HeroDeleted;
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
                view.ButtonDelete.Clicked += _deleteHero;
            }
            else
            {
                view.ButtonDelete.Clicked -= _deleteHero;
            }
        }

        private void _deleteHero()
        {
            _gameModel.Heroes.Remove(_gameModel.Heroes[_gameModel.LastHeroIndex]);
            
            HeroDeleted?.Invoke();
        }
    }
}