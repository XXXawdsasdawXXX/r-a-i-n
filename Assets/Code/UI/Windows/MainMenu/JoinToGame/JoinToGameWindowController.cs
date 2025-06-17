using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UI.Components;
using UI.Windows.Base;

namespace UI.Windows.MainMenu.JoinToGame
{
    public class JoinToGameWindowController : UIWindowController<JoinToGameWindowView>, 
        IInitializeListener, ILoadListener
    {
        public bool IsInitialized { get; set; }

        private UIElementPool<UIText> _pool;
        private GameModel _gameModel;
        private SaveService _saveService;
        private SettingsModel _settingsModel;
        
        public UniTask Initialize()
        {
            _saveService = Container.Instance.GetService<SaveService>();
            
            _pool.Initialize();

            return UniTask.CompletedTask;
        }

        public UniTask GameLoad(GameModel model)
        {
            _settingsModel = _saveService.ModelsContainer.UserSettings;

            view.DropDownPreviousIPs.SetOptions(_settingsModel.PreviousConnectedIPs);

            if (_settingsModel.PreviousConnectedIPs.Count > _settingsModel.LastConnectedIPIndex)
            {
                view.InputFieldConnectionIP.SetTextWithoutNotify(
                    _settingsModel.PreviousConnectedIPs[_settingsModel.LastConnectedIPIndex]);
            }

            return UniTask.CompletedTask;
        }

        protected override void subscribeToEvents(bool flag)
        {
            if (flag)
            {
                view.DropDownPreviousIPs.Changed += _onSetPreviousIP;
                view.InputFieldConnectionIP.Changed += _onChangedConnectionIP;
                view.ButtonConnection.Clicked += _onClickedConnection;
                view.ButtonClose.Clicked += Close;
            }
            else
            {
                view.DropDownPreviousIPs.Changed -= _onSetPreviousIP;
                view.InputFieldConnectionIP.Changed -= _onChangedConnectionIP;
                view.ButtonConnection.Clicked -= _onClickedConnection;
                view.ButtonClose.Clicked -= Close;
            }
        }

        private void _onSetPreviousIP(int IPIndex)
        {
            _settingsModel.LastConnectedIPIndex = IPIndex;
            
            view.InputFieldConnectionIP.SetTextWithoutNotify(
                _settingsModel.PreviousConnectedIPs[_settingsModel.LastConnectedIPIndex]);
        }

        private void _onChangedConnectionIP(string IP)
        {
            if (_settingsModel.PreviousConnectedIPs.Contains(IP))
            {
                for (int i = 0; i < _settingsModel.PreviousConnectedIPs.Count; i++)
                {
                    if (IP.Equals(_settingsModel.PreviousConnectedIPs[i]))
                    {
                        _settingsModel.LastConnectedIPIndex = i;

                        view.DropDownPreviousIPs.SetCurrent(i);
                    }
                }
            }
        }

        private void _onClickedConnection()
        {
            
            
        }
    }
}