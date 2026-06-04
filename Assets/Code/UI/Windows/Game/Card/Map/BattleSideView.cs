using System.Collections.Generic;
using CoreGame.Card.Data;
using Cysharp.Threading.Tasks;
using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.Card.Map
{
    public class BattleSideView : UIWindowView
    {
        [SerializeField] private RectTransform[] _sideCells;

    }

    public class BattleSideController : UIWindowController<BattleSideView>
    {
        public Dictionary<BattleUnit, RectTransform> _unitCells;
    }

    public class BattleUnitView : UIWindowView
    {
        [field: SerializeField] public UIImage Render { get; private set; }
        
        
    }

    public class BattleUnitController : UIWindowController<BattleSideView>
    {
        public override UniTask InitializeWindow(UIWindowManager manager)
        {
            
            return base.InitializeWindow(manager);
        }
    }
}