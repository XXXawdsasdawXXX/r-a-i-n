using System;
using UnityEngine;

namespace Essential
{
    public abstract class Mono : MonoBehaviour
    {
        public static event Action<Mono> Started;
        public static event Action<Mono> Destroyed;
        
        protected virtual void Start()
        {
            Started?.Invoke(this);
        }
        
        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}