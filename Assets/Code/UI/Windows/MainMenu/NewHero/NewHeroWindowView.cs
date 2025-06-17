using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.MainMenu.NewHero
{
    public class NewHeroWindowView : UIWindowView
    { 
        [field: SerializeField] public UIInputField InputFieldHeroName { get; private set; }
        [field: SerializeField] public  UIButton ButtonCreate { get; private set; }
    }
}