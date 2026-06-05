using Core.ServiceLocator;
using CoreGame.Card.Data;
using CoreGame.Card.Logic;
using Cysharp.Threading.Tasks;
using Essential;
using UI.Windows.Game.Card.Unit;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.Card
{
    public class CardWindowController : UIWindowController<CardWindowView>
    {
        private BattleService _battleService;

        [SerializeField] private BattleUnitView _leftHeroView;
        [SerializeField] private BattleUnitView _rightHeroView;

        
        public override UniTask InitializeWindow(UIWindowManager manager)
        {
            _battleService = Container.Instance.GetService<BattleService>();
            
            return base.InitializeWindow(manager);
        }

        public override void SubscribeToEvents(bool flag)
        {
            base.SubscribeToEvents(flag);

            Log.Info(this, "subscribe");
            
            if (flag)
            {
                _battleService.BattleStarted += _openView;
                _battleService.BattleFinished += _closeView;
                _battleService.TurnStarted += _updateUnitViews;
                _battleService.CardPlayed += _updateUnitViews;
            }
            else
            {
                _battleService.BattleStarted -= _openView;
                _battleService.BattleFinished -= _closeView;
                _battleService.TurnStarted -= _updateUnitViews;
                _battleService.CardPlayed -= _updateUnitViews;
            }
        }

        private void _closeView(BattleModel _)
        {
            view.Close();
            Log.Info(this, "Close view");
        }

        private void _openView(BattleModel model)
        {
            view.Open();
            _updateUnitViews(model);
            Log.Info(this, "Open view");
        }

        private void _updateUnitViews(BattleModel battleModel)
        {
            _leftHeroView?.Set(battleModel?.SideA?.Hero);
            _rightHeroView?.Set(battleModel?.SideB?.Hero);
        }
    }
}