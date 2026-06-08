using CoreGame.Card.Data;

namespace CoreGame.Card.Logic.CardProcessors
{
    public class CardMoveLineProcessor : ICardProcessor
    {
        public void Process(CardEffectConfiguration effect, BattleUnit actor, BattleUnit target, BattleModel battle)
        {
            if (target == null)
            {
                return;
            }

            EBattleLine oldLine = target.Line;
            EBattleLine newLine = oldLine == EBattleLine.Frontline
                ? EBattleLine.Backline
                : EBattleLine.Frontline;
            int oldCell = target.LineCellIndex;

            BattleGridRules.TryMoveUnitToCell(battle, target, newLine, oldCell);
        }
    }
}
