using System;
using System.Collections.Generic;
using FishNet.Object;

namespace CoreGame.Entities.Characters
{
    public abstract class Character : NetworkBehaviour
    {
        public bool IsConstructed { get; protected set; }
        public Dictionary<Type, ICharacterComponent> Components { get; } = new();

        public abstract void InitializeComponents();

        public T GetCharacterComponent<T>() where T : class, ICharacterComponent
        {
            Type type = typeof(T);
            
            if (Components.ContainsKey(type))
            {
                return Components[typeof(T)] as T;
            }

            return null;
        }
    }
}