using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Windows.Base;

namespace UI.Windows.MainMenu.NewGame
{
    public class NewGameWindowController : UIWindowController<NewGameWindowView>, IInitializeListener
    {
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
                view.ButtonCreate.Clicked += _addWorld;
            }
            else
            {
                
            }
        }

        private void _addWorld()
        {
            _gameModel.Worlds.Add(new WorldModel
            {
                CreateTime = default,
                GameTime = default,
            });
        }
    }
}