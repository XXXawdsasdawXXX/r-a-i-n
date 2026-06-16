using Core.GameLoop;
using Core.Input;
using Core.Network;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace CoreGame.Entities.Select
{
    public class SelectService : IService, IInitializeListener, ISubscriber
    {
        public bool IsInitialized { get; set; }
        
        private PlayerInput _input;
        private UserProvider _userProvider;

        private SelectTrigger _selectTrigger;

        
        public UniTask Initialize()
        {
            _input = Container.Instance.GetService<PlayerInput>();
            _userProvider = Container.Instance.GetService<UserProvider>();

            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _input.ActionEnded += _onActionEnded;
        }

        public void Unsubscribe()
        {
            _input.ActionEnded -= _onActionEnded;
            ClearSelection();
        }

        public void SetSelection(SelectTrigger other)
        {
            if (_selectTrigger != null)
            {
                ClearSelection();
            }

            if (_selectTrigger.IsSelf(_userProvider.Id))
            {
               // return;
            }

            _selectTrigger = other;
            
            switch (_selectTrigger.Type)
            {
                case SelectTrigger.EType.None:
                default:
                    break;
                
                case SelectTrigger.EType.Hero:
                case SelectTrigger.EType.Resource:
                case SelectTrigger.EType.Item:
                    _selectTrigger.Hover(true);
                    break;
            }
        }

        public void ClearSelection()
        {
            if (_selectTrigger != null)
            {
                _selectTrigger.Hover(false);
            }

            _selectTrigger = null;
        }

        private void _onActionEnded(EInputAction action)
        {
            if (action != EInputAction.LeftClick || _selectTrigger == null)
            {
                return;
            }

            switch (_selectTrigger.Type)
            {
                case SelectTrigger.EType.None:
                case SelectTrigger.EType.Resource:
                case SelectTrigger.EType.Item:
                default:
                    break;
                
                case SelectTrigger.EType.Hero:
                    _selectTrigger.Press();
                    break;
            }
        }
    }
}
