using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.MainMenu.NewGame
{
    public class NewGameWindowView : UIWindowView
    {
        [field: SerializeField] public UIButton ButtonCreate { get; private set; }
        [field: SerializeField] public UIInputField InputFieldGameName { get; private set; }
    }
}