using System;
using UnityEngine;

namespace Essential
{
    public abstract class Mono : MonoBehaviour, IEquatable<Mono>
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

        public bool Equals(Mono other)
        {
            return other != null && other.gameObject.GetHashCode().Equals(GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Mono)obj);
        }

        public override int GetHashCode()
        {
            return GetInstanceID();
        }
    }
}