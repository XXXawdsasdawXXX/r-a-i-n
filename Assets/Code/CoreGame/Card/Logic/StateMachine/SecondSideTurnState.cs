using System;
using System.Linq;
using System.Threading;
using CoreGame.Card.Data;
using CoreGame.Card.Logic;
using CoreGame.Card.Logic.AI;
using Cysharp.Threading.Tasks;

namespace CoreGame.Card.Logic.StateMachine
{
    public class SecondSideTurnState : IBattleState, IAcceptPlayerInput
    {
        private readonly BattleStateMachine _machine;
        public EBattlePhase Phase => EBattlePhase.SecondSideTurn;
        public bool IsInitialized { get; set; }

        private CancellationTokenSource _cts;


        public SecondSideTurnState(BattleStateMachine machine)
        {
            _machine = machine;
        }

        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Enter()
        {
            _machine.Model.TurnTimeRemaining.Value = BattleModel.MAX_TURN_TIME;

            if (_machine.Model.SideB.Hero.AI != null)
            {
                _processAI();
            }
            else
            {
                _startTurnTimer().Forget();
            }

            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            return UniTask.CompletedTask;
        }

        public bool TryPlayCard(string cardId, string targetId)
        {
            return CardPlayRules.TryPlay(
                _machine.Model.SideB,
                _machine.Model.SideB.Hero,
                cardId,
                targetId,
                _machine.Model,
                _machine.Processor,
                _machine.FindUnit,
                _spendCard);
        }

        public bool TryMoveLine(string unitId)
        {
            BattleUnit unit = _machine.FindUnit(unitId);

            if (unit == null)
            {
                return false;
            }

            if (unit.Energy < unit.MoveLineCost)
            {
                return false;
            }

            if (unit.Statuses.Any(s => s.Type == EStatusType.Stun))
            {
                return false;
            }

            unit.Energy -= unit.MoveLineCost;
            unit.Line = unit.Line == EBattleLine.Frontline
                ? EBattleLine.Backline
                : EBattleLine.Frontline;

            return true;
        }

        public void EndTurn()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _machine.SwitchState(typeof(TurnResolutionState));
        }

        private void _processAI()
        {
            BattleUnit ai = _machine.Model.SideB.Hero;

            while (true)
            {
                AIAction action = ai.AI.SelectAction(ai, _machine.Model);
                if (action?.Card == null)
                {
                    break;
                }

                if (ai.Energy < action.Card.GetEnergyCost(ai.Stats))
                {
                    break;
                }

                BattleUnit target = _machine.FindUnit(action.TargetId);
                if (target == null)
                {
                    break;
                }

                _machine.Processor.ApplyCard(ai, action.Card, target, _machine.Model);

                _spendCard(_machine.Model.SideB, ai, action.Card);
            }

            EndTurn();
        }


        private static void _spendCard(BattleSide side, BattleUnit actor, CardBattleState card)
        {
            if (card == null)
            {
                return;
            }

            if (side.ContainsMandatoryCard(card))
            {
                // Mandatory-карты не должны попадать в deck/discard.
                // На следующем ходу создается новая обязательная копия через EnsureMandatoryCard().
                side.RemoveMandatoryCard(card);
                return;
            }

            if (card.Config.Charges > 0)
            {
                card.ChargesLeft--;

                if (card.ChargesLeft <= 0)
                {
                    actor.Hand.Remove(card);
                    actor.Discard.Remove(card);
                }
            }
            else
            {
                actor.Hand.Remove(card);
                actor.Discard.Add(card);
            }
        }

        private async UniTaskVoid _startTurnTimer()
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            float endTime = UnityEngine.Time.time + BattleModel.MAX_TURN_TIME;

            try
            {
                while (true)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _cts.Token);
                    
                    float remaining = endTime - UnityEngine.Time.time;

                    if (remaining <= 0)
                    {
                        _machine.Model.TurnTimeRemaining.Value = 0;
                        EndTurn();
                        return;
                    }

                    _machine.Model.TurnTimeRemaining.Value = remaining;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}