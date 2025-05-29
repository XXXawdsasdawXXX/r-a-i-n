using UnityEngine;

namespace UI.Windows.Base
{
    public abstract class UIWindowController<UIView> : Essential.Mono where UIView : UIWindowView
    {
        [SerializeField] protected UIView view;

        protected void OnEnable()
        {
            subscribeToEvents(true);
        }

        private void OnDisable()
        {
            subscribeToEvents(false);
        }

        public void Open()
        {
            view.Open();
        }

        public void Close()
        {
            view.Close();
        }

        protected virtual void subscribeToEvents(bool flag)
        {
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (view == null && !TryGetComponent(out view))
            {
                view = gameObject.AddComponent<UIView>();
            }
        }
#endif
    }
}