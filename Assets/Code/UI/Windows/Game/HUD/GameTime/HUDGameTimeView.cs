using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.HUD
{
    public class HUDGameTimeView : UIWindowView
    {
        [field: SerializeField] public UIImage ImageGameTime { get; private set; }
    }
}