using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.MainMenu.JoinToGame
{
    public class JoinToGameWindowView : UIWindowView
    {
        [field: SerializeField] public UIDropDown DropDownPreviousIPs { get; private set; }
        [field: SerializeField] public UIInputField InputFieldConnectionIP { get; private set; }
        [field: SerializeField] public UIButton ButtonConnection { get; private set; }
        [field: SerializeField] public UIButton ButtonClose { get; private set; }
    }
}