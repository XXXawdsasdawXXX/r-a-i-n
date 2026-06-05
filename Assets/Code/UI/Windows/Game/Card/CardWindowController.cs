using Core.ServiceLocator;
using CoreGame.Card.Data;
using CoreGame.Card.Logic;
using CoreGame.Entities.Characters.Hero;
using Cysharp.Threading.Tasks;
using Essential;
using UI.Windows.Game.Card.Map;
using UI.Windows.Game.Card.Unit;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.Card
{
    public class CardWindowController : UIWindowController<CardWindowView>
    {
        private BattleService _battleService;
        private BattleModel _battleModel;
        private string _pendingCardId;
        private string _pendingMoveUnitId;
        private BattleSide _pendingMoveSide;
        private bool _isMoveTargetSelection;
        private bool _isMoveCellSelection;

        [SerializeField] private BattleUnitView _leftHeroView;
        [SerializeField] private BattleUnitView _rightHeroView;
        [SerializeField] private BattleSideView _leftSideView;
        [SerializeField] private BattleSideView _rightSideView;

        
        public override UniTask InitializeWindow(UIWindowManager manager)
        {
            _battleService = Container.Instance.GetService<BattleService>();
     
            _leftSideView.SetCellsHighlighted(false);
            _rightSideView.SetCellsHighlighted(false);
            
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
                _leftHeroView.Clicked += _onLeftHeroClicked;
                _rightHeroView.Clicked += _onRightHeroClicked;
                _bindCells(_leftSideView, true);
                _bindCells(_rightSideView, true);
            }
            else
            {
                _battleService.BattleStarted -= _openView;
                _battleService.BattleFinished -= _closeView;
                _battleService.TurnStarted -= _updateUnitViews;
                _battleService.CardPlayed -= _updateUnitViews;
                _leftHeroView.Clicked -= _onLeftHeroClicked;
                _rightHeroView.Clicked -= _onRightHeroClicked;
                _bindCells(_leftSideView, false);
                _bindCells(_rightSideView, false);
            }
        }

        private void _closeView(BattleModel _)
        {
            view.Close();
            _clearMoveSelection();
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
            _battleModel = battleModel;
            _leftHeroView.Set(battleModel?.SideA?.Hero);
            _rightHeroView.Set(battleModel?.SideB?.Hero);
            _updateHeroAnchors();
        }

        public bool TrySelectMoveTarget(string cardId)
        {
            if (_battleModel == null || string.IsNullOrEmpty(cardId))
            {
                return false;
            }

            _pendingCardId = cardId;
            _pendingMoveUnitId = null;
            _isMoveTargetSelection = true;
            _isMoveCellSelection = false;
            _leftHeroView.SetHighlighted(true);
            _rightHeroView.SetHighlighted(true);

            _leftSideView.SetCellsHighlighted(false);
            _rightSideView.SetCellsHighlighted(false);

            Log.Info(this, $"[MoveUI] select unit for move card={cardId}");
            return true;
        }

        private void _onLeftHeroClicked()
        {
            _onUnitClicked(_battleModel?.SideA?.Hero?.UnitId);
        }

        private void _onRightHeroClicked()
        {
            _onUnitClicked(_battleModel?.SideB?.Hero?.UnitId);
        }

        private void _onUnitClicked(string targetUnitId)
        {
            if (!_isMoveTargetSelection || string.IsNullOrEmpty(targetUnitId))
            {
                return;
            }

            BattleUnit unit = _battleService.FindUnit(targetUnitId);
            if (unit == null)
            {
                Log.Info(this, $"[MoveUI] unit not found {targetUnitId}");
                return;
            }

            BattleSide unitSide = BattleGridRules.GetOwnerSide(_battleModel, unit);
            BattleSide mySide = _getMySide();
            if (!ReferenceEquals(unitSide, mySide))
            {
                Log.Info(this, $"[MoveUI] reject unit from other side. unit={targetUnitId}");
                return;
            }

            if (_battleModel.Phase.Value is EBattlePhase.FirstSideTurn)
            {
                _leftSideView.SetCellsHighlighted(true);
            }
            else
            {
                _rightSideView.SetCellsHighlighted(true);
            }

            _pendingMoveUnitId = targetUnitId;
            _pendingMoveSide = unitSide;
            _isMoveTargetSelection = false;
            _isMoveCellSelection = true;
            _leftHeroView.SetHighlighted(false);
            _rightHeroView.SetHighlighted(false);

            Log.Info(this, $"[MoveUI] unit selected unit={targetUnitId}. Now click highlighted grid cell.");
        }

        private void _clearMoveSelection()
        {
            _pendingCardId = null;
            _pendingMoveUnitId = null;
            _pendingMoveSide = null;
            _isMoveTargetSelection = false;
            _isMoveCellSelection = false;
            _leftHeroView.SetHighlighted(false);
            _rightHeroView.SetHighlighted(false);
            _leftSideView?.SetCellsHighlighted(false);
            _rightSideView?.SetCellsHighlighted(false);
        }

        /// <summary>
        /// Временный тестовый метод для первых тестов.
        /// После выбора карты перемещения и юнита вызови из инспектора.
        /// </summary>
        [ContextMenu("Test Move -> Frontline Cell 0")]
        private void _testMoveFrontline0()
        {
            _tryMoveToCell(EBattleLine.Frontline, 0);
        }

        [ContextMenu("Test Move -> Frontline Cell 1")]
        private void _testMoveFrontline1()
        {
            _tryMoveToCell(EBattleLine.Frontline, 1);
        }

        [ContextMenu("Test Move -> Frontline Cell 2")]
        private void _testMoveFrontline2()
        {
            _tryMoveToCell(EBattleLine.Frontline, 2);
        }

        [ContextMenu("Test Move -> Backline Cell 0")]
        private void _testMoveBackline0()
        {
            _tryMoveToCell(EBattleLine.Backline, 0);
        }

        [ContextMenu("Test Move -> Backline Cell 1")]
        private void _testMoveBackline1()
        {
            _tryMoveToCell(EBattleLine.Backline, 1);
        }

        [ContextMenu("Test Move -> Backline Cell 2")]
        private void _testMoveBackline2()
        {
            _tryMoveToCell(EBattleLine.Backline, 2);
        }

        private void _tryMoveToCell(EBattleLine line, int cellIndex)
        {
            if (!_isMoveCellSelection || string.IsNullOrEmpty(_pendingMoveUnitId))
            {
                Log.Info(this, "[MoveUI] skip move: no selected unit/card");
                return;
            }

            BattleUnit unit = _battleService.FindUnit(_pendingMoveUnitId);
            if (unit == null)
            {
                Log.Info(this, $"[MoveUI] skip move: unit not found {_pendingMoveUnitId}");
                _clearMoveSelection();
                return;
            }

            bool cardApplied = _battleService.TryPlayCard(_pendingCardId, _pendingMoveUnitId);
            if (!cardApplied)
            {
                Log.Info(this, $"[MoveUI] card apply failed card={_pendingCardId} unit={_pendingMoveUnitId}");
                _clearMoveSelection();
                return;
            }

            bool moved = _battleService.TryMoveToCell(_pendingMoveUnitId, line, cellIndex);
            Log.Info(this, $"[MoveUI] move result card={_pendingCardId} unit={_pendingMoveUnitId} to={line}/{cellIndex} moved={moved}");

            if (!moved)
            {
                Log.Info(this, "[MoveUI] move failed after card apply. Try another cell.");
                return;
            }

            _clearMoveSelection();
        }

        private void _onCellClicked(BattleGridCellView cell)
        {
            if (cell == null || !_isMoveCellSelection)
            {
                return;
            }

            bool isLeftMoveSide = ReferenceEquals(_pendingMoveSide, _battleModel?.SideA);
            bool isOwnCell = isLeftMoveSide
                ? cell.Side == EBattleSideUi.Left
                : cell.Side == EBattleSideUi.Right;

            if (!isOwnCell)
            {
                Log.Info(this, $"[MoveUI] reject enemy cell side={cell.Side} for unit={_pendingMoveUnitId}");
                return;
            }

            Log.Info(this, $"[MoveUI] cell clicked line={cell.Line} cell={cell.CellIndex}");
            _tryMoveToCell(cell.Line, cell.CellIndex);
        }

        private void _bindCells(BattleSideView sideView, bool bind)
        {
            if (sideView == null)
            {
                return;
            }

            for (int i = 0; i < BattleGridRules.CELLS_PER_LINE; i++)
            {
                BattleGridCellView front = sideView.GetCell(EBattleLine.Frontline, i);
                BattleGridCellView back = sideView.GetCell(EBattleLine.Backline, i);

                _bindCell(front, bind);
                _bindCell(back, bind);
            }
        }

        private void _bindCell(BattleGridCellView cell, bool bind)
        {
            if (cell == null)
            {
                return;
            }

            if (bind)
            {
                cell.Clicked += _onCellClicked;
            }
            else
            {
                cell.Clicked -= _onCellClicked;
            }
        }

        private void _updateHeroAnchors()
        {
            if (_battleModel == null)
            {
                return;
            }

            _anchorHero(_leftHeroView, _leftSideView, _battleModel.SideA.Hero);
            _anchorHero(_rightHeroView, _rightSideView, _battleModel.SideB.Hero);
        }

        private void _anchorHero(BattleUnitView heroView, BattleSideView sideView, BattleUnit hero)
        {
            if (heroView == null || sideView == null || hero == null)
            {
                return;
            }

            BattleGridCellView targetCell = sideView.GetCell(hero.Line, hero.LineCellIndex);
            if (targetCell == null)
            {
                return;
            }

            heroView.transform.SetParent(targetCell.transform, false);
            heroView.transform.localPosition = Vector3.zero;
            heroView.transform.localScale = Vector3.one;

            Log.Info(this, $"[GridUI] anchor hero unit={hero.UnitId} line={hero.Line} cell={hero.LineCellIndex}");
        }

        private BattleSide _getMySide()
        {
            if (_battleModel == null)
            {
                return null;
            }

            Hero hero = Container.Instance.GetService<Core.Network.UserProvider>().GetHeroComponent<Hero>();
            string heroId = hero?.Model?.HeroId;

            if (!string.IsNullOrEmpty(heroId))
            {
                if (_battleModel.SideA.Hero.UnitId == heroId)
                {
                    return _battleModel.SideA;
                }

                if (_battleModel.SideB.Hero.UnitId == heroId)
                {
                    return _battleModel.SideB;
                }
            }

            return _battleModel.SideA;
        }
    }
}