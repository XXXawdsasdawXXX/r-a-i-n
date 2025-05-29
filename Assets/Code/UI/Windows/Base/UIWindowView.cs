using UnityEngine;

namespace UI.Windows.Base
{
    public abstract class UIWindowView : Essential.Mono
    {
        [SerializeField] protected RectTransform body;

        public virtual void Open()
        {
            body.gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            body.gameObject.SetActive(false);
        }
    }
}