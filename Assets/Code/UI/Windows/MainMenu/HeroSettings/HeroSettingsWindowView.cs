using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.MainMenu.HeroSettings
{
    public class HeroSettingsWindowView : UIWindowView
    {
        [field: SerializeField] public UIText TextHeroName { get; private set; }
        [field: SerializeField] public UIText TextGameTime { get; private set; }
    }
}