using System;
using DG.Tweening;

namespace Plugins.Demigiant.DOTween.Modules
{
    [Serializable]
    public struct FloatTweenData
    {
        public float Value;
        public float Duration;
        public Ease Ease;
    }
}