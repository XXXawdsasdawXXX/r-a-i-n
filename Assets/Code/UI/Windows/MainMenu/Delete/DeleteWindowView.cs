using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.MainMenu.Delete
{
    public class DeleteWindowView : UIWindowView
    {
        [field: SerializeField] public UIText TextName { get; private set; }
        [field: SerializeField] public UIImage ImageIcon { get; private set; }
        [field: SerializeField] public GameObject ObjectIcon { get; private set; }
        [field: SerializeField] public UIButton ButtonDelete { get; private set; }
        [field: SerializeField] public UIButton ButtonReturn { get; private set; }
    }
}