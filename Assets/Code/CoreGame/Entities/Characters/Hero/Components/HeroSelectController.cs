using Core.Data;
using Core.GameLoop;
using Core.Input;
using Core.ServiceLocator;
using CoreGame.Entities.Select;
using CoreGame.PlayerCamera;
using UnityEngine;

namespace CoreGame.Entities.Characters.Hero
{
    public class HeroSelectController : ICharacterComponent, ISubscriber, IUpdateListener
    {
        private const float PICK_RADIUS = 0.5f;

        public Condition Condition { get; } = new();
        
        private readonly CameraView _camera;
        private readonly InputService _input;

        private readonly Hero _hero;

        private SelectObject _selectObject;

        
        public HeroSelectController(Hero hero)
        {
            _hero = hero;

            _camera = Container.Instance.GetView<CameraView>();
            _input = Container.Instance.GetService<InputService>();
        }

        public void Subscribe()
        {
            _input.ActionEnded += _onInputActionEnded;
        }

        public void Unsubscribe()
        {
            _input.ActionEnded -= _onInputActionEnded;
        }

        public void GameUpdate(float deltaTime)
        {
            if (!Condition.AreMet())
            {
                return;
            }

            if (_tryGetSelectedObject(out SelectObject selectObject))
            {
                if (_selectObject == selectObject)
                {
                    return;
                }

                _clearSelection();
                
                _setSelection(selectObject);                
            }
            else if(_selectObject != null)
            {
                _clearSelection();
            }
        }

        private Vector2 _getMouseWorldPoint()
        {
            Vector3 screenPoint = _input.MousePosition;
            screenPoint.z = _camera.Camera.WorldToScreenPoint(Vector3.zero).z;
            return _camera.Camera.ScreenToWorldPoint(screenPoint);
        }

        private bool _tryGetSelectedObject(out SelectObject selectObject)
        {
            selectObject = null;

            Vector2 worldPoint = _getMouseWorldPoint();
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPoint, PICK_RADIUS);

            float bestSqrDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                SelectObject candidate = _resolveSelectObject(hit);
                if (candidate == null)
                {
                    continue;
                }

                Hero hero = hit.GetComponentInParent<Hero>();
                if (hero != null && hero.IsOwner)
                {
                    continue;
                }

                float sqrDistance = ((Vector2)hit.ClosestPoint(worldPoint) - worldPoint).sqrMagnitude;
                if (sqrDistance >= bestSqrDistance)
                {
                    continue;
                }

                bestSqrDistance = sqrDistance;
                selectObject = candidate;
            }

            return selectObject != null;
        }

        private static SelectObject _resolveSelectObject(Collider2D hit)
        {
            if (hit == null)
            {
                return null;
            }
            
            if (hit.TryGetComponent(out SelectObject selectObject))
            {
                return selectObject;
            }

            Hero hero = hit.GetComponentInParent<Hero>();
            return hero != null ? hero.SelectObject : null;
        }
        
        private void _clearSelection()
        {
            if (_selectObject != null)
            {
                _selectObject.Hover(false);
            }

            _selectObject = null;
        }

        private void _setSelection(SelectObject other)
        {
            _selectObject = other;
            
            switch (_selectObject.Type)
            {
                case SelectObject.EType.None:
                default:
                    break;
                
                case SelectObject.EType.Hero:
                case SelectObject.EType.Resource:
                case SelectObject.EType.Item:
                    _selectObject.Hover(true);
                    break;
            }
        }
        
        private void _onInputActionEnded(EInputAction action)
        {
            if (action != EInputAction.LeftClick || _selectObject == null)
            {
                return;
            }

            switch (_selectObject.Type)
            {
                case SelectObject.EType.None:
                case SelectObject.EType.Resource:
                case SelectObject.EType.Item:
                default:
                    break;
                
                case SelectObject.EType.Hero:
                    _selectObject.Press();
                    break;
            }
        }
    }
}