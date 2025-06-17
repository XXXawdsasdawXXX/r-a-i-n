using Sirenix.OdinInspector;
using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.MainMenu.Hero
{
    public class HeroWindowView : UIWindowView
    {
        [field: SerializeField] public UIRadioGroup<UIText> HeroesRadioGroup { get; private set; }
        
        [field: Space]
        
        [field: BoxGroup("Hero view"), SerializeField] public GameObject BodyHeroView { get; private set; }
        [field: BoxGroup("Hero view"), SerializeField] public UIHeroCardView HeroCard { get; private set; }

        [field: Space]
        
        [field: BoxGroup("Option buttons"), SerializeField] public UIButton ButtonNew { get; private set; }
        [field: BoxGroup("Option buttons"), SerializeField]  public UIButton ButtonSettings { get; private set; }
        [field: BoxGroup("Option buttons"), SerializeField]  public UIButton ButtonDelete { get; private set; }
    }
}