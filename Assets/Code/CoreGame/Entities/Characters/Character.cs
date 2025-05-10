using System;
using System.Collections.Generic;
using FishNet.Object;

namespace Code.CoreGame.Entities.Characters
{
    public abstract class Character : NetworkBehaviour
    {
        public Dictionary<Type, ICharacterComponent> Components { get; protected set; } = new();
        
        public abstract void InitializeComponents();
        public abstract void InitializeComponentConditions();
        
    }
}