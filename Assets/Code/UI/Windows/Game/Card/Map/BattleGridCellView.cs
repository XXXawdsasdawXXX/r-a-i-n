using System;
using CoreGame.Card.Data;
using UI.Components;
using UnityEngine;

namespace UI.Windows.Game.Card.Map
{
    public class BattleGridCellView : UIButton
    {
        public event Action<BattleGridCellView> Clicked;

        [field: SerializeField] public EBattleLine Line { get; private set; }
        [field: SerializeField] public int CellIndex { get; private set; }
        [field: SerializeField] public EBattleSideUi Side { get; private set; }

        [SerializeField] private GameObject _highlight;

        public void SetHighlighted(bool value)
        {
            if (_highlight != null)
            {
                _highlight.SetActive(value);
            }
        }

        protected override void onClick()
        {
            base.onClick();
            Clicked?.Invoke(this);
        }
    }

    public enum EBattleSideUi
    {
        Left = 0,
        Right = 1
    }
}
