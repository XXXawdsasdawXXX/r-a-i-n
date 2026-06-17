using System;

namespace CoreGame.Entities.Select
{
    [Serializable]
    public abstract class SelectableImpact
    {
         public abstract void Hovered(bool isHover);
         public abstract void Pressed();
         public abstract bool CanUnHover();
    }
}