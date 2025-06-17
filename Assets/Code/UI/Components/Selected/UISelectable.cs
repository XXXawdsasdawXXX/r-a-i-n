using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Components
{
    public abstract class UISelectable : Essential.Mono, 
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPoolableUIElement
    {
        public event Action Selected;
        public event Action Deselected;
        public event Action Clicked;

        [field: SerializeField] public int Index { get; private set; }
        [field: SerializeField] public bool IsSelected { get; private set; }
        [field: SerializeField] protected GameObject body { get; private set; }

        [SerializeField, CanBeNull] private UISelectableAnimation _selectableAnimation;
        
        void IPoolableUIElement.Enable()
        {
            body.SetActive(false);
        }

        void IPoolableUIElement.Disable()
        {
            body.SetActive(true);
        }

        public void SetIndex(int index)
        {
            Index = index;
        }
        
        public void Dispose()
        {
            Selected = null;
            Deselected = null;
            Clicked = null;
        }

        public virtual void Select()
        {
            if (_selectableAnimation != null)
            {
                _selectableAnimation.Select();
            }
            IsSelected = true;
        }

        public virtual void Deselect()
        {
            if (_selectableAnimation != null)
            {
                _selectableAnimation.Deselect();
            }
            IsSelected = false;
        }
        
        public abstract void SetInteractable(bool isInteractable);
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnter();
            Selected?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onClick();
            Clicked?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit();
            Deselected?.Invoke();
        }

        protected virtual void onEnter()
        {
        }

        protected virtual void onClick()
        {
        }

        protected virtual void onExit()
        {
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (body == null)
            {
                body = gameObject;
            }
        }
#endif
    }
}