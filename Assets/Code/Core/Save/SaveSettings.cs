using UnityEngine;

namespace Core.Save
{
    [CreateAssetMenu(fileName = "Settings_SaveLoad", menuName = "Game/Settings/Save")]
    public class SaveSettings : ScriptableObject
    {
        [field: SerializeField] public GameModel DefaultModel { get; set; }
    }
}