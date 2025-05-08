using System;

namespace Code.CoreGame.Common.Selectable
{
    public interface ISelectableView
    {
        public void Select();
        public void Deselect();
    }
}