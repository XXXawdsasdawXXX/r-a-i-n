using Core.ServiceLocator;
using CoreGame.Card.Logic;
using CoreGame.Card.Logic.Network;
using Cysharp.Threading.Tasks;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.BattleLobby
{
    public class BattleLobbyWindowController : UIWindowController<BattleLobbyWindowView>
    {
        private NetworkBattleService _networkBattleService;

        public override UniTask InitializeWindow(UIWindowManager manager)
        {
            _networkBattleService = Container.Instance.GetService<NetworkBattleService>();
            view.Close();
            return base.InitializeWindow(manager);
        }

        public override void SubscribeToEvents(bool flag)
        {
            if (view == null)
            {
                Debug.LogWarning("BattleLobbyWindowView is not assigned.");
                return;
            }

            if (flag)
            {
                _networkBattleService.LobbyStateChanged += _onLobbyStateChanged;
                view.ButtonCancel.Clicked += _onCancelClicked;
                view.ButtonStart.Clicked += _onStartClicked;
            }
            else
            {
                _networkBattleService.LobbyStateChanged -= _onLobbyStateChanged;
                view.ButtonCancel.Clicked -= _onCancelClicked;
                view.ButtonStart.Clicked -= _onStartClicked;
            }
        }

        private void _onLobbyStateChanged(BattleLobbyState state)
        {
            if (view == null)
            {
                return;
            }

            if (!state.IsOpen)
            {
                Close();
                return;
            }

            view.TextStatus.SetText(state.GetStatusText());
            view.TextHint.SetText(state.GetHintText());

            view.ButtonStart.gameObject.SetActive(state.IsHost);
            view.ButtonStart.SetInteractable(state.CanStart);

            Open();
        }

        private void _onCancelClicked()
        {
            _networkBattleService.RequestLeaveLobby();
            Close();
        }

        private void _onStartClicked()
        {
            _networkBattleService.RequestStartLobby();
        }
    }
}
