using System.Collections.Generic;
using System.Text;
using CoreGame.Card.Data;
using DG.Tweening;
using UI.Components;
using UI.Windows.Base;
using UnityEngine;

namespace UI.Windows.Game.Card.Hover
{
    public class CardUnitHoverView : UIWindowView
    {
        [SerializeField] private UIText _unitHoverInfoText;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Vector2 _offset = new Vector2(160f, 0f);
        [SerializeField] private float _fadeDuration = 0.2f;

        private Tween _fadeTween;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            base.Close();
        }

        protected override void OnDestroy()
        {
            _killFadeTween();
            base.OnDestroy();
        }

        public void Show(BattleUnit unit, RectTransform unitRect, bool isRightSide)
        {
            _setPositionNearUnit(unitRect, isRightSide);
            
            _unitHoverInfoText.SetText(_buildUnitHoverInfo(unit));

            _canvasGroup.alpha = 0;
            
            base.Open();
            
            _killFadeTween();
        
            _canvasGroup.alpha = Mathf.Clamp01(_canvasGroup.alpha);
            _fadeTween = _canvasGroup.DOFade(1f, _fadeDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(body.gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(() => _fadeTween = null);
        }

        public void Hide()
        {
            _killFadeTween();
            _fadeTween = _canvasGroup.DOFade(0f, _fadeDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(body.gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(() =>
                {
                    _fadeTween = null;
                    base.Close();
                });
        }
        
        private void _setPositionNearUnit(RectTransform unitRect, bool isRightSide)
        {
            Vector3 worldPos = unitRect.position;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
            float offsetX = isRightSide ? -_offset.x : _offset.x;
            body.position = screenPoint + new Vector2(offsetX, _offset.y);
        }

        private static string _buildUnitHoverInfo(BattleUnit unit)
        {
            StringBuilder builder = new StringBuilder(128);
            int sectionsCount = 0;

            int summonTurnsLeft = _getSummonTurnsLeft(unit);
            bool isTemporary = summonTurnsLeft > 0;
            if (isTemporary)
            {
                builder.AppendLine("Временный юнит");
                sectionsCount++;
            }

            if (unit.AutoActionType != EAutoActionType.None)
            {
                if (sectionsCount > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("Авто-действие: ");
                builder.Append(_getAutoActionDescription(unit.AutoActionType));
                if (unit.AutoActionValue > 0f)
                {
                    builder.Append(" (");
                    builder.Append(Mathf.CeilToInt(unit.AutoActionValue));
                    builder.Append(')');
                }

                sectionsCount++;
            }

            List<string> statusLines = _collectStatusLines(unit.Statuses);
            if (statusLines.Count > 0)
            {
                if (sectionsCount > 0)
                {
                    builder.AppendLine();
                    builder.AppendLine();
                }

                builder.AppendLine("Эффекты:");
                foreach (string statusLine in statusLines)
                {
                    builder.Append("- ");
                    builder.AppendLine(statusLine);
                }

                sectionsCount++;
            }

            if (isTemporary)
            {
                if (sectionsCount > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("Осталось ходов: ");
                builder.Append(summonTurnsLeft);
                sectionsCount++;
            }

            if (sectionsCount == 0)
            {
                return "Нет активных эффектов";
            }

            return builder.ToString();
        }

        private static int _getSummonTurnsLeft(BattleUnit unit)
        {
            if (unit?.Statuses == null)
            {
                return 0;
            }

            for (int i = 0; i < unit.Statuses.Count; i++)
            {
                StatusEffect status = unit.Statuses[i];
                if (status != null && status.Type == EStatusType.SummonDuration)
                {
                    return Mathf.Max(0, status.Duration);
                }
            }

            return 0;
        }

        private static string _getAutoActionDescription(EAutoActionType autoActionType)
        {
            switch (autoActionType)
            {
                case EAutoActionType.AttackEnemyHero:
                    return "Атака вражеского героя";
                case EAutoActionType.GiveShieldToOwnerHero:
                    return "Щит союзному герою";
                default:
                    return "Нет";
            }
        }

        private static List<string> _collectStatusLines(List<StatusEffect> statuses)
        {
            List<string> lines = new List<string>();
            if (statuses == null || statuses.Count == 0)
            {
                return lines;
            }

            foreach (StatusEffect status in statuses)
            {
                if (status == null || status.Type == EStatusType.SummonDuration)
                {
                    continue;
                }

                StringBuilder line = new StringBuilder();
                line.Append(_getStatusDisplayName(status.Type));

                bool hasValue = status.Value > 0f;
                bool hasDuration = status.Duration > 0;
                if (hasValue || hasDuration)
                {
                    line.Append(" (");
                    if (hasValue)
                    {
                        line.Append(Mathf.CeilToInt(status.Value));
                    }

                    if (hasDuration)
                    {
                        if (hasValue)
                        {
                            line.Append(", ");
                        }

                        line.Append(status.Duration);
                        line.Append(" ход.");
                    }

                    line.Append(')');
                }

                lines.Add(line.ToString());
            }

            return lines;
        }

        private static string _getStatusDisplayName(EStatusType statusType)
        {
            switch (statusType)
            {
                case EStatusType.Bleed:
                    return "Кровотечение";
                case EStatusType.Poison:
                    return "Яд";
                case EStatusType.Burn:
                    return "Горение";
                case EStatusType.Electro:
                    return "Электро";
                case EStatusType.Stun:
                    return "Оглушение";
                case EStatusType.Weak:
                    return "Слабость";
                case EStatusType.Regeneration:
                    return "Регенерация";
                case EStatusType.EnergyCostReduction:
                    return "Снижение стоимости";
                case EStatusType.CritBoost:
                    return "Крит-усиление";
                case EStatusType.ArmorStance:
                    return "Стойка брони";
                default:
                    return statusType.ToString();
            }
        }

        private void _killFadeTween()
        {
            _fadeTween?.Kill();
            _fadeTween = null;
        }
    }
}