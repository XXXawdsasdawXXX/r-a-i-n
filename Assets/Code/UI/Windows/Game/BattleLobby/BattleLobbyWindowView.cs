using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.BattleLobby
{
    public class BattleLobbyWindowView : UIWindowView
    {
        [field: SerializeField] public UIText TextStatus { get; private set; }
        [field: SerializeField] public UIText TextHint { get; private set; }
        [field: SerializeField] public UIButton ButtonCancel { get; private set; }
        [field: SerializeField] public UIButton ButtonStart { get; private set; }
    }
}
