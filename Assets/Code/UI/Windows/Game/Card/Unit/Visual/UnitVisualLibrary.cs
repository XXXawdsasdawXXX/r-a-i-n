using System.Collections.Generic;
using UnityEngine;

namespace UI.Windows.Game.Card.Unit.Visual
{
    [CreateAssetMenu(fileName = "Library_UnitVisual", menuName = "Game/Battle/Unit Visual Library")]
    public class UnitVisualLibrary : ScriptableObject
    {
        [SerializeField] private UnitVisualProfile _defaultHeroProfile;
        [SerializeField] private UnitVisualProfile _defaultCompanionProfile;
        [SerializeField] private UnitVisualProfile[] _profiles = new UnitVisualProfile[0];

        private Dictionary<string, UnitVisualProfile> _lookup;

        public UnitVisualProfile DefaultHeroProfile => _defaultHeroProfile;
        public UnitVisualProfile DefaultCompanionProfile => _defaultCompanionProfile;

        public UnitVisualProfile Resolve(string profileId, bool isCompanion)
        {
            _ensureLookup();

            if (!string.IsNullOrEmpty(profileId) && _lookup.TryGetValue(profileId, out UnitVisualProfile profile))
            {
                return profile;
            }

            return isCompanion ? _defaultCompanionProfile : _defaultHeroProfile;
        }

        private void _ensureLookup()
        {
            if (_lookup != null)
            {
                return;
            }

            _lookup = new Dictionary<string, UnitVisualProfile>();
            foreach (UnitVisualProfile profile in _profiles)
            {
                if (profile == null || string.IsNullOrEmpty(profile.Id))
                {
                    continue;
                }

                _lookup[profile.Id] = profile;
            }
        }

        private void OnEnable()
        {
            _lookup = null;
        }
    }
}
