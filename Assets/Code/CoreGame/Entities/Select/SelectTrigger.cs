using System;
using System.Collections.Generic;
using Essential;
using FishNet.Object;
using UnityEngine;

namespace CoreGame.Entities.Select
{
    public sealed class SelectTrigger : Essential.Mono
    {
        public enum EType
        {
            None, 
            Hero,
            Resource,
            Item
        }
        
        public event Action<SelectTrigger, bool> Hovered;

        [field: SerializeField]
        public EType Type { get; private set; }
        
        [SerializeField] 
        private NetworkObject _networkObject;

        [SerializeReference] private SelectableImpact[] _impacts;
        
        
        public bool IsSelf(string id)
        {
            return id.Equals(_networkObject.ObjectId.ToString());
        }
        
        public void Hover(bool isHover)
        {
            Hovered?.Invoke(this, isHover);
            
            foreach (SelectableImpact selectableImpact in _impacts)
            {
                selectableImpact.Pressed();
            }
            
            Log.Info($"{gameObject.name} is hover {isHover}");
        }

        public void Press()
        {
            foreach (SelectableImpact selectableImpact in _impacts)
            {
                selectableImpact.Pressed();
            }
            
            Log.Info($"{gameObject.name} pressed");
        }
    }
}