using Sirenix.OdinInspector;
using CoreGame.Card.Data;
using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.Card.Unit
{
    public class BattleUnitView : UIWindowView
    {
        [field: SerializeField] public UIImage Render { get; private set; }
        [field: Title("Params")]
        [field: SerializeField] public UIImage HealthFill { get; private set; }
        [field: SerializeField] public UIText HealthText { get; private set; }
        [field: SerializeField] public UIBattleStateIcon Armor { get; private set; }
        [field: SerializeField] public UIBattleStateIcon Attack { get; private set; }

        public void Set(BattleUnit unit)
        {
            if (unit == null)
            {
                Close();
                return;
            }

            Open();

            float maxHp = Mathf.Max(1f, unit.MaxHP);
            float hp = Mathf.Max(0f, unit.HP);

            HealthFill.SetFillAmount(hp / maxHp);
            HealthText.SetText($"{Mathf.CeilToInt(hp)}/{Mathf.CeilToInt(maxHp)}");

            _setStateIcon(Armor, unit.Armor);
            _setStateIcon(Attack, unit.AutoActionType == EAutoActionType.AttackEnemyHero ? unit.AutoActionValue : 0f);
        }

        private static void _setStateIcon(UIBattleStateIcon stateIcon, float value)
        {
            if (stateIcon?.Icon == null || stateIcon.Value == null)
            {
                return;
            }

            bool show = value > 0f;
            stateIcon.Icon.gameObject.SetActive(show);
            stateIcon.Value.gameObject.SetActive(show);

            if (show)
            {
                stateIcon.Value.SetText(Mathf.CeilToInt(value).ToString());
            }
        }
    }
}