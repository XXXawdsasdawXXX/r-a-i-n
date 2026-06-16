using Plugins.Demigiant.DOTween.Modules;
using TriInspector;
using UnityEngine;

namespace UI.Data
{
    [CreateAssetMenu(fileName = "Settings_UI", menuName = "Game/Settings/UI")]
    public class UISettings : ScriptableObject
    {
        [field: Group("Selected"), SerializeField] public ColorTweenData SelectedTween { get; private set; }
        [field: Group("Selected"), SerializeField] public ColorTweenData DeselectedTween { get; private set; }
        
        [field: Space]
        [field: Group("Ghost tab"), SerializeField] public FloatTweenData GhostTabAlpaTween { get; private set; }
        [field: Group("Ghost tab"), SerializeField] public FloatTweenData GhostTabMoveYTween { get; private set; }
    }
}