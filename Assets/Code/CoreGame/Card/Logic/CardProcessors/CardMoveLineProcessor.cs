using CoreGame.Card.Data;
using Essential;

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

            bool moved = BattleGridRules.TryMoveUnitToCell(battle, target, newLine, oldCell);

            Log.Info($"[MoveLineProcessor] actor={actor?.UnitId} target={target.UnitId} " +
                     $"from={oldLine}/{oldCell} to={newLine}/{oldCell} moved={moved}");
        }
    }
}
