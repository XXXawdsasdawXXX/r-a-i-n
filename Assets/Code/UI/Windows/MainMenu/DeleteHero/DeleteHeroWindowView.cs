using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.MainMenu.DeleteHero
{
    public class DeleteHeroWindowView : UIWindowView
    {
        [field: SerializeField] public UIButton ButtonDelete { get; private set; }
    }
}