using System;
using Core.GameLoop;
using Core.ServiceLocator;
using CoreGame.Harvest;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Components
{
    public class UIResourceBoxView : Essential.Mono, IInitializeListener, IPoolableUIElement
    {
        public bool IsInitialized { get; set; }
        [field: SerializeField] public EResource ResourceType { get; private set; }
    
        [SerializeField] private RectTransform _body;
        [SerializeField] private UIImage _icon; 
        [SerializeField] private UIText _textValue;
       
        private ResourceCollection _resourceCollection;
        
        public UniTask Initialize()
        {
            _resourceCollection = Container.Instance.GetSO<ResourceCollection>();   
                
            return UniTask.CompletedTask;
        }
        
        public void Enable()
        {
            _body.gameObject.SetActive(true);
        }

        public void Disable()
        {
            _body.gameObject.SetActive(false);
        }
        
        public void SetResource(EResource resourceType)
        {
            ResourceType = resourceType;
            UpdateIcon();
        }
        
        public void SetValue(string value)
        {
            _textValue.SetText(value);
        }

        public void UpdateIcon()
        {
            _icon.SetSprite(_resourceCollection.Library.Get(ResourceType).Icon);
        }

        public Action Clicked { get; }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
       
        }

        public void OnPointerExit(PointerEventData eventData)
        {
       
        }
    }
}