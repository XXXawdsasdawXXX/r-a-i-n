using System;
using CoreGame.Card.Data;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;
using UI.Windows.Game.Card;
using Essential;

namespace UI.Windows.Game.Card.Map
{
    public class BattleGridCellView : UIButton
    {
        public event Action<BattleGridCellView> Clicked;

        [field: SerializeField] public UIImage Render { get; private set; }
        [field: SerializeField] public EBattleLine Line { get; private set; }
        [field: SerializeField] public int CellIndex { get; private set; }
        [field: SerializeField] public EBattleSideUi Side { get; private set; }

        public Material HighlightMaterialTemplate => BattleHighlightStyle.HighlightMaterial != null
            ? BattleHighlightStyle.HighlightMaterial
            : Render?.Image?.material;
        public Material OccupiedHighlightMaterialTemplate => BattleHighlightStyle.OccupiedHighlightMaterial != null
            ? BattleHighlightStyle.OccupiedHighlightMaterial
            : HighlightMaterialTemplate;

        public bool HasOccupiedHighlightBinding => OccupiedHighlightMaterialTemplate != null;

     
        private UIHighlightMaterialController _uiHighlightController;
        private bool _occupied;

        private void Awake()
        {
            _uiHighlightController = new UIHighlightMaterialController(Render.Image);
            Log.Info(this, $"[HighlightCell] awake renderImage={Render?.Image != null} side={Side} line={Line} cell={CellIndex} template={HighlightMaterialTemplate?.name ?? "null"}");
        }

        public void SetHighlighted(bool value)
        {
            Log.Info(this, $"[HighlightCell] set highlighted value={value} occupied={_occupied} side={Side} line={Line} cell={CellIndex}");
            if (_occupied)
            {
                return;
            }
            
            if (!value)
            {
                _uiHighlightController?.Reset();
                return;
            }

            _uiHighlightController?.Apply(HighlightMaterialTemplate);
        }
        
        public void SetOccupied(bool value)
        {
            _occupied = value;
            Log.Info(this, $"[HighlightCell] set occupied value={value} side={Side} line={Line} cell={CellIndex}");

            if (_occupied)
            {
                if (OccupiedHighlightMaterialTemplate != null)
                {
                    _uiHighlightController?.Apply(OccupiedHighlightMaterialTemplate);
                }

                return;
            }

            _uiHighlightController?.Reset();
        }

        public void SetHighlightColor(Color color)
        {
            Log.Info(this, $"[HighlightCell] set color={color} side={Side} line={Line} cell={CellIndex}");
            _uiHighlightController?.SetColor(color);
        }

        protected override void onClick()
        {
            base.onClick();
            Clicked?.Invoke(this);
        }

        private void OnDestroy()
        {
            Log.Info(this, $"[HighlightCell] destroy side={Side} line={Line} cell={CellIndex}");
            _uiHighlightController?.Dispose();
            _uiHighlightController = null;
        }
    }

    public enum EBattleSideUi
    {
        Left = 0,
        Right = 1
    }
}
