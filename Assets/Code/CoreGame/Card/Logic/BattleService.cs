using System;
using System.Collections.Generic;
using System.Linq;
using Core.GameLoop;
using Core.Save;
using Core.ServiceLocator;
using CoreGame.Card.Data;
using CoreGame.Card.Logic.StateMachine;
using Cysharp.Threading.Tasks;
using Essential;

namespace CoreGame.Card.Logic
{
    public class BattleService : IService, IInitializeListener, IExitListener
    {
        public bool IsInitialized { get; set; }
        public event Action<BattleModel> BattleStarted;
        public event Action<BattleModel> TurnStarted;
        public event Action<BattleModel> BattleFinished;
        public event Action<BattleModel> CardPlayed;
        
        private BattleStateMachine _machine;

        private List<HeroModel> _battleHeroes = new List<HeroModel>();

        public UniTask Initialize()
        {
            _machine = Container.Instance.GetService<BattleStateMachine>();
            
            return UniTask.CompletedTask;
        }

        public void StartBattle(HeroModel attacker, HeroModel defender, EBattleMode mode = EBattleMode.PvE)
        {
            _battleHeroes.Add(attacker); 
            _battleHeroes.Add(defender); 
            attacker.InBattle = true;
            defender.InBattle = true;
            
            _machine.StartBattle(attacker, defender, mode);
            _machine.Model.Phase.SubscribeProperty(_onPhaseChanged);
            
            BattleStarted?.Invoke(_machine.Model);
       
            Log.Info(this, "Start battle");            
        }

        public bool TryPlayCard(string cardId, string targetId)
        {
            if (_machine.CurrentState is IAcceptPlayerInput acceptPlayerInput)
            {
                if (acceptPlayerInput.TryPlayCard(cardId, targetId))
                {
                    CardPlayed?.Invoke(_machine.Model);
                    _tryFinishBattleAfterAction();
                    return true;
                }
            }
            
            return false;
        }

        public bool TryMoveLine(string unitId)
        {
            bool success = (_machine.CurrentState as IAcceptPlayerInput)?.TryMoveLine(unitId) ?? false;
            Log.Info(this, $"[TryMoveLine] unit={unitId} success={success}");
            if (success)
            {
                CardPlayed?.Invoke(_machine.Model);
                _tryFinishBattleAfterAction();
            }

            return success;
        }

        public bool TryMoveToCell(string unitId, EBattleLine line, int cellIndex)
        {
            if (!(_machine.CurrentState is IAcceptPlayerInput acceptPlayerInput))
            {
                return false;
            }

            BattleUnit unit = _machine.FindUnit(unitId);
            if (unit == null)
            {
                return false;
            }

            BattleSide side = BattleGridRules.GetOwnerSide(_machine.Model, unit);
            if (side == null)
            {
                return false;
            }

            bool needLineSwitch = unit.Line != line;
            if (needLineSwitch && !acceptPlayerInput.TryMoveLine(unitId))
            {
                Log.Info(this, $"[TryMoveToCell] move-line failed unit={unitId}");
                return false;
            }

            bool occupied = side.GetAllUnits()
                .Where(u => u != null && u.HP > 0)
                .Where(u => u.UnitId != unit.UnitId)
                .Any(u => u.Line == line && u.LineCellIndex == cellIndex);

            if (occupied)
            {
                Log.Info(this, $"[TryMoveToCell] target occupied unit={unitId} target={line}/{cellIndex}");
                return false;
            }

            bool moved = BattleGridRules.TryMoveUnitToCell(_machine.Model, unit, line, cellIndex);
            Log.Info(this, $"[TryMoveToCell] unit={unitId} target={line}/{cellIndex} moved={moved}");

            if (moved)
            {
                CardPlayed?.Invoke(_machine.Model);
            }

            return moved;
        }

        public void EndTurn()
        {
            (_machine.CurrentState as IAcceptPlayerInput)?.EndTurn();
        }

        public BattleUnit FindUnit(string unitId)
        {
            return _machine.FindUnit(unitId);
        }

        private void _tryFinishBattleAfterAction()
        {
            if (_machine.Model == null)
            {
                return;
            }

            if (_machine.Model.Phase.Value == EBattlePhase.Finished || _machine.CurrentState is EndBattleState)
            {
                return;
            }

            bool isSideADead = _machine.Model.SideA?.Hero == null || _machine.Model.SideA.Hero.HP <= 0;
            bool isSideBDead = _machine.Model.SideB?.Hero == null || _machine.Model.SideB.Hero.HP <= 0;

            if (!isSideADead && !isSideBDead)
            {
                return;
            }

            _machine.SwitchState(typeof(EndBattleState));
        }

        private void _onPhaseChanged(EBattlePhase phase)
        {
            switch (phase)
            {
                case EBattlePhase.WaitingBattle:
                    break;
                case EBattlePhase.StartBattle:
                    BattleStarted?.Invoke(_machine.Model);
                    break;
                case EBattlePhase.StartTurn:
                    break;
                case EBattlePhase.FirstSideTurn:
                    TurnStarted?.Invoke(_machine.Model);
                    break;
                case EBattlePhase.SecondSideTurn:
                    TurnStarted?.Invoke(_machine.Model);
                    break;
                case EBattlePhase.Resolution:
                    break;
                case EBattlePhase.Finished:
                    foreach (HeroModel hero in _battleHeroes)
                    {
                        hero.InBattle = false;
                    }
                    _battleHeroes.Clear();
                    _machine.Model.Phase.UnsubscribeProperty(_onPhaseChanged);
                    BattleFinished?.Invoke(_machine.Model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        public void GameExit()
        {
            foreach (HeroModel hero in _battleHeroes)
            {
                hero.InBattle = false;
            }
        }
    }
}