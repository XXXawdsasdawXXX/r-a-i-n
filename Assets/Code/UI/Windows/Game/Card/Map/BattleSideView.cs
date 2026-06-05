using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.Card.Map
{
    public class BattleSideView : UIWindowView
    {
        [SerializeField] private BattleGridCellView[] _frontCells = new BattleGridCellView[3];
        [SerializeField] private BattleGridCellView[] _backCells = new BattleGridCellView[3];

        public BattleGridCellView GetCell(CoreGame.Card.Data.EBattleLine line, int cellIndex)
        {
            BattleGridCellView[] cells = line == CoreGame.Card.Data.EBattleLine.Frontline
                ? _frontCells
                : _backCells;

            if (cells == null || cellIndex < 0 || cellIndex >= cells.Length)
            {
                return null;
            }

            return cells[cellIndex];
        }

        public void SetCellsHighlighted(bool value)
        {
            _setLineHighlighted(_frontCells, value);
            _setLineHighlighted(_backCells, value);
        }

        private static void _setLineHighlighted(BattleGridCellView[] cells, bool value)
        {
            if (cells == null)
            {
                return;
            }

            foreach (BattleGridCellView cell in cells)
            {
                if (cell == null)
                {
                    continue;
                }

                cell.SetHighlighted(value);
            }
        }
    }
}