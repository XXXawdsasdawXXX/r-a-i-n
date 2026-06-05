using CoreGame.Card.Data;
using Cysharp.Threading.Tasks;
using GameKit.Dependencies.Utilities;

namespace CoreGame.Card.Logic.StateMachine
{
    public class StartTurnState : IBattleState
    {
        public EBattlePhase Phase => EBattlePhase.StartTurn;
        public bool IsInitialized { get; set; }

        private readonly BattleStateMachine _machine;

        
        public StartTurnState(BattleStateMachine machine)
        {
            _machine = machine;
        }

        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Enter()
        {
            _machine.Model.TurnNumber++;

            _startTurn(_machine.Model.SideA.Hero);
            _startTurn(_machine.Model.SideB.Hero);

            _machine.SwitchState(typeof(FirstSideTurnState));
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }

        private static void _startTurn(BattleUnit unit)
        {
            unit.Energy = unit.MaxEnergy;
            _drawCardsToHandLimit(unit);
        }

        private static void _drawCardsToHandLimit(BattleUnit unit)
        {
            int cardsToDraw = unit.HandLimit - unit.Hand.Count;

            if (cardsToDraw <= 0)
            {
                return;
            }

            for (int i = 0; i < cardsToDraw; i++)
            {
                if (unit.Deck.Count == 0)
                {
                    _reshuffleDeck(unit);
                }

                if (unit.Deck.Count == 0)
                {
                    break;
                }

                CardBattleState card = unit.Deck[0];
                unit.Deck.RemoveAt(0);
                unit.Hand.Add(card);
            }
        }

        private static void _reshuffleDeck(BattleUnit unit)
        {
            if (unit.Discard.Count == 0)
            {
                return;
            }

            unit.Deck.AddRange(unit.Discard);
            unit.Discard.Clear();
            unit.Deck.Shuffle();
        }
    }
}