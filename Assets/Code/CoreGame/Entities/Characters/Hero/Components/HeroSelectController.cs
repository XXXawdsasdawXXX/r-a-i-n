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
        private const string SELECTABLE_TAG = "Selectable";
        private static readonly int SelectableLayerMask = LayerMask.GetMask("Selectable");

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
            if (_tryGetSelectedObject(out SelectObject selectObject))
            {
                if (_selectObject == selectObject)
                {
                    return;
                }

                /*if (_selectObject.IsThis(_hero.Model.HeroId))
                {
                    // return;
                }*/

                _clearSelection();
                
                _setSelection(selectObject);                
            }
            else if(_selectObject != null)
            {
                _clearSelection();
            }
        }

        private bool _tryGetSelectedObject(out SelectObject selectObject)
        {
            selectObject = null;

            Vector3 worldPoint = _camera.ScreenToWorldPoint(_input.MousePosition);
            worldPoint.z = 0f;

            Collider2D hit = Physics2D.OverlapPoint(worldPoint, SelectableLayerMask);
            if (hit == null)
            {
                return false;
            }

            if (!hit.CompareTag(SELECTABLE_TAG))
            {
                return false;
            }

            if (!hit.TryGetComponent(out SelectObject selectable))
            {
                selectable = hit.GetComponentInParent<SelectObject>();
            }

            selectObject = selectable;

            return selectObject != null;
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