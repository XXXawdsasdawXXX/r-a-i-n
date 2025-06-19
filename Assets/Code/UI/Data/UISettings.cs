using Plugins.Demigiant.DOTween.Modules;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Data
{
    [CreateAssetMenu(fileName = "Settings_UI", menuName = "Game/Settings/UI")]
    public class UISettings : ScriptableObject
    {
        [field: BoxGroup("Selected"), SerializeField] public ColorTweenData SelectedTween { get; private set; }
        [field: BoxGroup("Selected"), SerializeField] public ColorTweenData DeselectedTween { get; private set; }
        
        [field: Space]
        [field: BoxGroup("Ghost tab"), SerializeField] public FloatTweenData GhostTabAlpaTween { get; private set; }
        [field: BoxGroup("Ghost tab"), SerializeField] public FloatTweenData GhostTabMoveYTween { get; private set; }
    }
}